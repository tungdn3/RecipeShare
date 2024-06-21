import { useAuth0 } from '@auth0/auth0-vue';
import { defineStore } from 'pinia';
import { api } from 'boot/axios';
import {
  IRecipe,
  IRecipeAdd,
  IRecipeEdit,
  IRecipeCard,
} from 'src/interfaces/Recipe';
import { ref } from 'vue';
import { IPageResult } from 'src/interfaces/Common';

export const useMyRecipesStore = defineStore('my-recipes', () => {
  const auth0 = useAuth0();
  const recipes = ref<IRecipeCard[]>([]);
  const recipePageResult = ref<IPageResult<IRecipeCard> | null>(null);
  const isLoading = ref(false);

  async function getMyRecipes(pageNumber = 1) {
    isLoading.value = true;
    const accessToken = await auth0.getAccessTokenSilently();
    try {
      console.log('--------- getting my recipes using axios', pageNumber);
      const response = await api.get<IPageResult<IRecipeCard>>(
        `/management/recipes?pageSize=9&pageNumber=${pageNumber}`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        }
      );
      recipePageResult.value = response.data;
      recipes.value =
        pageNumber === 1
          ? response.data.items
          : [...recipes.value, ...response.data.items];
      return recipePageResult.value;
    } catch (e) {
      console.error(e);
      return [];
    } finally {
      isLoading.value = false;
    }
  }

  async function getMyRecipeById(id: number): Promise<IRecipe | null> {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await api.get(`/management/recipes/${id}`, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    if (response.status < 200 || response.status >= 400) {
      throw new Error(`Failed to get recipe id ${id}.`);
    }
    return response.data;
  }

  async function create(recipe: IRecipeAdd) {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await api.post('/management/recipes', recipe, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    if (response.status < 200 || response.status >= 400) {
      throw new Error('Failed to create recipe.');
    } else {
      getMyRecipes(); // not wait
    }
  }

  async function update(id: number, recipe: IRecipeEdit) {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await api.put(`/management/recipes/${id}`, recipe, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    if (response.status < 200 || response.status >= 400) {
      throw new Error('Failed to update recipe.');
    } else {
      getMyRecipes(); // not wait
    }
  }

  getMyRecipes();
  return {
    recipes,
    recipePageResult,
    isLoading,
    getMyRecipes,
    getMyRecipeById,
    create,
    update,
  };
});
