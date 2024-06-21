<template>
  <q-page class="q-pa-sm">
    <h4 class="text-primary">My Recipes</h4>
    <div class="row">
      <div
        class="col-12 col-sm-6 col-md-4 q-px-sm q-py-md"
        v-for="recipe in recipes"
        :key="recipe.id"
      >
        <RecipeCard
          :id="recipe.id"
          :title="recipe.title"
          :cooking-minutes="recipe.cookingMinutes"
          :preparation-minutes="recipe.preparationMinutes"
          :image-url="recipe.imageUrl"
          :nb-of-likes="recipe.likeCount"
          :nb-of-comments="recipe.commentCount"
        />
      </div>
    </div>
    <div v-if="isLoading">
      <q-spinner color="primary" size="3em" />
    </div>
    <div
      v-if="recipePageResult?.hasNextPage"
      class="row justify-center q-my-md"
      @click="
        () =>
          myRecipesStore.getMyRecipes((recipePageResult?.pageNumber ?? 0) + 1)
      "
    >
      <q-btn color="primary">Load More</q-btn>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import RecipeCard from 'src/components/recipe/RecipeCard.vue';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { storeToRefs } from 'pinia';

defineOptions({
  name: 'RecipeList',
});

const myRecipesStore = useMyRecipesStore();
const { recipes, recipePageResult, isLoading } = storeToRefs(myRecipesStore);
</script>
