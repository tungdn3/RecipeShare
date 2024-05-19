<template>
  <q-page>
    <div class="column q-py-lg q-px-md">
      <q-form @submit="search" class="q-gutter-md">
        <q-input v-model="searchText" label="Search recipes..." />
        <div id="suggestion-list">
          <SearchSuggestionList
            v-if="showSuggestionTexts"
            :items="suggestionTexts"
            @selected="onSuggestionTextSelected"
          />
        </div>
      </q-form>

      <div v-if="!isSearching" class="row">
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
        <q-spinner color="primary" size="3em" />
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { IRecipeCard } from 'src/interfaces/Recipe';
import RecipeCard from 'src/components/recipe/RecipeCard.vue';
import SearchSuggestionList from 'src/components/SearchSuggestionList.vue';
import { ref, watch } from 'vue';
import { searchApi } from 'boot/axios';
import { IPageResult } from 'src/interfaces/Common';

defineOptions({
  name: 'IndexPage',
});

const searchText = ref('');
const pageNumber = ref(1);
const pageSize = 10;
const categoryId = ref<number | undefined>(undefined);
const isSearching = ref(false);
const suggestionTexts = ref<string[]>([]);
const showSuggestionTexts = ref(false);
let isSuggestionSelected = false;
const recipes = ref<IRecipeCard[]>([]);

watch(searchText, async (newValue) => {
  console.log('-------- search text changed', searchText, isSuggestionSelected);
  if (isSuggestionSelected) {
    isSuggestionSelected = false;
    return;
  }
  if (!newValue) {
    suggestionTexts.value = [];
    return;
  }
  suggestionTexts.value = await getAutoComplete(newValue);
  showSuggestionTexts.value = true;
});

async function getAutoComplete(searchText: string) {
  const response = await searchApi.get<string[]>(
    `/search/complete?query=${searchText}`
  );
  return response.data;
}

function onSuggestionTextSelected(text: string) {
  isSuggestionSelected = true;
  searchText.value = text;
  search();
}

async function search() {
  showSuggestionTexts.value = false;
  let url = `/search?query=${searchText.value}&pageNumber=${pageNumber.value}&pageSize=${pageSize}`;
  if (categoryId.value) {
    url = url + `&categoryId=${categoryId.value}`;
  }
  const result = await searchApi.get<IPageResult<IRecipeCard>>(url);
  recipes.value = result.data.items;
}
</script>

<style scoped>
#suggestion-list {
  position: absolute;
  z-index: 1000;
  width: 100%;
  padding-right: 1rem;
}
</style>
