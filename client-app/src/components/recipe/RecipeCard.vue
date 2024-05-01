<template>
  <div class="recipe-card" @click="goToRecipeDetails">
    <q-card>
      <img :src="props.imageUrl" />

      <q-card-section class="q-pt-md q-pb-sm text-primary">
        <div class="text-h6">{{ props.title }}</div>
      </q-card-section>

      <q-separator inset />

      <q-card-section class="q-pt-none">
        <div class="row justify-between q-mt-sm">
          <div class="col-6 row justify-start">
            <div>
              <q-icon color="secondary" name="wash" class="q-mr-xs" />
              <span>{{ props.preparationMinutes }}'</span>
            </div>

            <q-separator vertical class="q-mx-sm" />

            <div class="">
              <q-icon name="access_alarm" color="secondary" class="q-mr-xs" />
              <span>{{ props.cookingMinutes }}'</span>
            </div>
          </div>

          <div class="col-6 row justify-end">
            <div>
              <q-icon name="favorite" color="secondary" />
              {{ props.nbOfLikes }}
            </div>

            <q-separator vertical class="q-mx-sm" />

            <div>
              <q-icon name="comment" color="secondary" />
              {{ props.nbOfComments }}
            </div>
          </div>
        </div>
      </q-card-section>
    </q-card>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';

const props = defineProps({
  id: Number,
  title: String,
  imageUrl: String,
  preparationMinutes: Number,
  cookingMinutes: Number,
  nbOfLikes: Number,
  nbOfComments: Number,
});

defineOptions({
  name: 'RecipeCard',
});

const router = useRouter();

function goToRecipeDetails() {
  router.push({ path: '/recipes/' + props.id });
}
</script>

<style>
.recipe-card:hover {
  cursor: pointer;
  border: solid 1px darkgreen;
  border-radius: 5px;
}
</style>
