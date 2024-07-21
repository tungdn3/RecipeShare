<template>
  <q-page>
    <div class="column q-px-md" style="margin-top: 3rem">
      <q-form @submit="search">
        <q-input
          outlined
          debounce="300"
          v-model="searchText"
          label="Search recipes..."
          @blur="() => (showSuggestionTexts = false)"
        >
          <template v-slot:append>
            <q-btn
              type="submit"
              round
              color="primary"
              icon="search"
              size="sm"
              @click="search"
              :disable="!searchText"
            />
          </template>
        </q-input>
        <div id="suggestion-list">
          <SearchSuggestionList
            v-if="showSuggestionTexts"
            :items="suggestionTexts"
            @selected="onSuggestionTextSelected"
          />
        </div>
      </q-form>

      <div v-if="!isSearching" class="row q-mt-lg">
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
      <div v-else>
        <q-spinner color="primary" size="3em" />
      </div>
    </div>
    <div class="q-mt-lg q-px-md" style="margin-bottom: 3rem">
      <RecipeList />
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { IRecipeCard } from 'src/interfaces/Recipe';
import RecipeCard from 'src/components/recipe/RecipeCard.vue';
import SearchSuggestionList from 'src/components/SearchSuggestionList.vue';
import RecipeList from 'src/components/recipe/RecipeList.vue';
import { ref, watch } from 'vue';
import { searchApi, socialApi } from 'boot/axios';
import { ICountItem, IPageResult } from 'src/interfaces/Common';

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
const suggestionsLoading = ref<boolean>();

watch(searchText, async (newValue) => {
  if (isSuggestionSelected) {
    isSuggestionSelected = false;
    return;
  }
  if (!newValue) {
    suggestionTexts.value = [];
    showSuggestionTexts.value = false;
    recipes.value = [];
    return;
  }
  suggestionTexts.value = await getAutoComplete(newValue);
  showSuggestionTexts.value = suggestionTexts.value.length > 0;
});

async function getAutoComplete(searchText: string) {
  suggestionsLoading.value = true;
  try {
    const response = await searchApi.get<string[]>(
      `complete?query=${searchText}`
    );
    return response.data;
  } finally {
    suggestionsLoading.value = false;
  }
}

function onSuggestionTextSelected(text: string) {
  isSuggestionSelected = true;
  searchText.value = text;
  search();
}

async function search() {
  showSuggestionTexts.value = false;
  let url = `?query=${searchText.value}&pageNumber=${pageNumber.value}&pageSize=${pageSize}`;
  if (categoryId.value) {
    url = url + `&categoryId=${categoryId.value}`;
  }
  const result = await searchApi.get<IPageResult<IRecipeCard>>(url);
  recipes.value = result.data.items;
  fillLikeCounts();
  fillCommentCounts();
}

async function fillLikeCounts() {
  const recipeIds = recipes.value?.map((x) => x.id);
  if (!recipeIds || !recipeIds.length) {
    return;
  }
  const result = await socialApi.post<ICountItem[]>('likes/count', recipeIds);
  recipes.value = recipes.value.map((x) => {
    const like = result.data.find((l) => l.id === x.id);
    x.likeCount = like?.count;
    return x;
  });
}

async function fillCommentCounts() {
  const recipeIds = recipes.value?.map((x) => x.id);
  if (!recipeIds || !recipeIds.length) {
    return;
  }
  const result = await socialApi.post<ICountItem[]>(
    'comments/count',
    recipeIds
  );
  recipes.value = recipes.value.map((x) => {
    const comment = result.data.find((l) => l.id === x.id);
    x.commentCount = comment?.count;
    return x;
  });
}
</script>

<style scoped>
#suggestion-list {
  position: absolute;
  z-index: 1000;
  width: 100%;
}
</style>
