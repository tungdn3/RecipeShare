import { defineStore } from 'pinia';
import { searchApi, socialApi } from 'boot/axios';
import { IRecipeCard } from 'src/interfaces/Recipe';
import { ref } from 'vue';
import { ICountItem, IPageResult } from 'src/interfaces/Common';
import { useQuasar } from 'quasar';

export const useNewRecipesStore = defineStore('new-recipes', () => {
  const $q = useQuasar();
  const recipes = ref<IRecipeCard[]>([]);
  const recipePageResult = ref<IPageResult<IRecipeCard> | null>(null);
  const isLoading = ref(false);

  async function getNewRecipes(pageNumber = 1) {
    isLoading.value = true;
    try {
      const response = await searchApi.get<IPageResult<IRecipeCard>>(
        `new?pageSize=9&pageNumber=${pageNumber}`
      );
      recipePageResult.value = response.data;
      recipes.value =
        pageNumber === 1
          ? response.data.items
          : [...recipes.value, ...response.data.items];

      fillLikeCounts(recipes.value);
      fillCommentCounts(recipes.value);

      return recipePageResult.value;
    } catch (e) {
      console.error(e);
      $q.notify({
        message: 'Failed to refesh new posts',
        color: 'negative',
      });
      return [];
    } finally {
      isLoading.value = false;
    }
  }

  async function fillLikeCounts(recipes: IRecipeCard[]) {
    const recipeIds = recipes.map((x) => x.id);
    if (!recipeIds || !recipeIds.length) {
      return;
    }
    const result = await socialApi.post<ICountItem[]>('likes/count', recipeIds);
    recipes = recipes.map((x) => {
      const like = result.data.find((l) => l.id === x.id);
      x.likeCount = like?.count;
      return x;
    });
  }

  async function fillCommentCounts(recipes: IRecipeCard[]) {
    const recipeIds = recipes.map((x) => x.id);
    if (!recipeIds || !recipeIds.length) {
      return;
    }
    const result = await socialApi.post<ICountItem[]>(
      'comments/count',
      recipeIds
    );
    recipes = recipes.map((x) => {
      const comment = result.data.find((l) => l.id === x.id);
      x.commentCount = comment?.count;
      return x;
    });
  }

  return {
    recipes,
    isLoading,
    getNewRecipes,
  };
});
