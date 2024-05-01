<template>
  <q-page class="q-pa-sm">
    <h4>{{ recipe?.title }}</h4>
    <div class="row">
      <div class="col-12 col-md-8 q-pr-sm">
        <div class="row">
          <q-img :src="recipe?.imageUrl" fit="cover"></q-img>
        </div>
        <div class="row q-my-sm">
          {{ recipe?.description }}
        </div>
        <div class="row q-my-sm">
          {{ recipe?.instructions }}
        </div>
      </div>
      <div class="col-12 col-md-4">
        <div class="row justify-between">
          <div class="col-sm-auto q-pr-sm">Preparation time</div>
          <div>{{ recipe?.preparationMinutes }} mins</div>
        </div>

        <div class="row justify-between">
          <div class="col-sm-auto q-pr-sm">Cooking time</div>
          <div>{{ recipe?.cookingMinutes }} mins</div>
        </div>

        <div class="row justify-between q-pt-md">
          <div class="col-sm-auto q-pr-sm">Category</div>
          <div>{{ recipe?.categoryName }}</div>
        </div>

        <div class="row justify-between">
          <div class="col-sm-auto q-pr-sm">Posted by</div>
          <div>{{ recipe?.userName }}</div>
        </div>

        <div class="row justify-between q-pt-md">
          <div
            v-for="ingredient in recipe?.ingredients"
            :key="ingredient"
            class="col-12"
          >
            {{ ingredient }}
          </div>
        </div>
      </div>
    </div>
    <q-separator class="q-mt-lg" />

    <div>
      <CommentList />
    </div>

    <q-separator />

    <CommentForm />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import CommentList from 'src/components/comment/CommentList.vue';
import CommentForm from 'src/components/comment/CommentForm.vue';
import { useRoute } from 'vue-router';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { Recipe } from 'src/interfaces/Recipe';

defineOptions({
  name: 'RecipeDetails',
});

const route = useRoute();
const id = ref(Number.parseInt(route.params.id as string));
const recipeStore = useMyRecipesStore();
const recipe = ref<Recipe | null>();

onMounted(async () => {
  recipe.value = await recipeStore.getMyRecipeById(id.value);
});
</script>
