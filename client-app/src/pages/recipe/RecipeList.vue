<template>
  <q-page class="q-pa-sm">
    <h4>Recipes</h4>
    <div v-if="!isLoading" class="row">
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
          :nb-of-likes="recipe.nbOfLikes"
          :nb-of-comments="recipe.nbOfComments"
        />
      </div>
    </div>
    <div v-else>
      <h3>Loading...</h3>
    </div>
    <div class="row justify-center q-my-md">
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
const { recipes, isLoading } = storeToRefs(myRecipesStore);
</script>
