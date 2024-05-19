<template>
  <q-page class="q-pa-sm">
    <h4>{{ recipe?.title }}</h4>
    <div class="row">
      <div v-if="recipe" class="col-12 col-md-8 q-pr-sm">
        <div class="row">
          <q-img :src="recipe.imageUrl" fit="cover"></q-img>
        </div>
        <div class="row q-my-sm">
          {{ recipe.description }}
        </div>
        <div class="row q-my-sm">
          {{ recipe.instructions }}
        </div>
      </div>
      <div v-else class="col-12 col-md-8 row justify-center">
        <q-spinner color="secondary" size="3rem" />
      </div>
      <div class="col-12 col-md-4">
        <ReadOnlyField
          class="q-pt-none"
          label="Preparation time"
          :value="recipe ? `${recipe.preparationMinutes} min` : ''"
        />
        <ReadOnlyField
          label="Cooking time"
          :value="recipe ? `${recipe.cookingMinutes} min` : ''"
        />

        <div class="q-my-md"></div>

        <ReadOnlyField
          label="Category"
          :value="recipe ? recipe.categoryName : ''"
        />
        <ReadOnlyField
          label="Posted by"
          :value="recipe ? recipe.userName : ''"
        />

        <div class="q-my-md"></div>

        <div class="col-12">
          <ReadOnlyField
            v-for="ingredient in recipe?.ingredients"
            :label="ingredient"
            :key="ingredient"
            no-value
          />
        </div>
      </div>
      <div class="col-12 q-mt-lg">
        <q-btn outline color="secondary" icon="thumb_up" />
        <q-btn :outline="false" color="primary" icon="thumb_up" />
      </div>
    </div>
    <q-separator class="q-mt-lg" />

    <CommentForm />

    <q-separator />

    <CommentList />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import CommentList from 'src/components/comment/CommentList.vue';
import CommentForm from 'src/components/comment/CommentForm.vue';
import { useRoute } from 'vue-router';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { IRecipe } from 'src/interfaces/Recipe';
import ReadOnlyField from 'src/components/ReadOnlyField.vue';

defineOptions({
  name: 'RecipeDetails',
});

const route = useRoute();
const id = ref(Number.parseInt(route.params.id as string));
const recipeStore = useMyRecipesStore();
const recipe = ref<IRecipe | null>();

onMounted(async () => {
  recipe.value = await recipeStore.getMyRecipeById(id.value);
});
</script>
