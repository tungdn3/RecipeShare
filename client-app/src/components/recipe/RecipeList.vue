<template>
  <div class="row">
    <div class="col-12 text-h4 text-primary">New Recipes</div>
    <div class="col-12 row">
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
  </div>
</template>

<script setup>
import { useNewRecipesStore } from 'src/stores/new-recipes-store';
import { storeToRefs } from 'pinia';
import RecipeCard from 'src/components/recipe/RecipeCard.vue';

defineOptions({
  name: 'RecipeList',
});

const newRecipesStore = useNewRecipesStore();
const { recipes, isLoading } = storeToRefs(newRecipesStore);
</script>
