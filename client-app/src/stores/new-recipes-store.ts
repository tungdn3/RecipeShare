import { defineStore } from 'pinia';
import { managementApi } from 'boot/axios';
import { IRecipeCard } from 'src/interfaces/Recipe';
import { ref } from 'vue';
import { IPageResult } from 'src/interfaces/Common';

export const useNewRecipesStore = defineStore('new-recipes', () => {
  const recipes = ref<IRecipeCard[]>([]);
  const recipePageResult = ref<IPageResult<IRecipeCard> | null>(null);
  const isLoading = ref(false);

  async function getNewRecipes(pageNumber = 1) {
    isLoading.value = true;
    try {
      const response = await managementApi.get<IPageResult<IRecipeCard>>(
        `/search/new?pageSize=9&pageNumber=${pageNumber}`
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

  getNewRecipes();
  return {
    recipes,
    recipePageResult,
    isLoading,
    getNewRecipes,
  };
});
