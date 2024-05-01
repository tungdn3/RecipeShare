import { useAuth0 } from '@auth0/auth0-vue';
import { defineStore } from 'pinia';
import { Recipe, RecipeAdd, RecipeOverview } from 'src/interfaces/Recipe';
import { ref } from 'vue';

export const useMyRecipesStore = defineStore('my-recipes', () => {
  const auth0 = useAuth0();
  const recipes = ref<RecipeOverview[]>([]);
  const isLoading = ref(false);

  async function getMyRecipes() {
    isLoading.value = true;
    const accessToken = await auth0.getAccessTokenSilently();
    try {
      const response = await fetch(
        'https://localhost:7000/management/recipes',
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        }
      );
      const data = await response.json();
      recipes.value = data;
      return recipes.value;
    } catch (e) {
      return [];
    } finally {
      isLoading.value = false;
    }
  }

  async function getMyRecipeById(id: number): Promise<Recipe | null> {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await fetch(
      `https://localhost:7000/management/recipes/${id}`,
      {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );
    if (!response.ok) {
      throw new Error(`Failed to get recipe id ${id}.`);
    }
    const data = await response.json();
    return data;
  }

  async function save(recipe: RecipeAdd) {
    if (!auth0) {
      return;
    }
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await fetch('https://localhost:7000/management/recipes', {
      method: 'POST',
      body: JSON.stringify(recipe),
      headers: {
        'Content-type': 'application/json; charset=UTF-8',
        Authorization: `Bearer ${accessToken}`,
      },
    });
    console.log('---------- save', response);
    if (!response.ok) {
      throw new Error('Failed to save recipe.');
    } else {
      await getMyRecipes();
    }
  }

  getMyRecipes();
  return { recipes, isLoading, getMyRecipes, getMyRecipeById, save };
});
