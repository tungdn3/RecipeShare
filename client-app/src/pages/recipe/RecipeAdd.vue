<template>
  <q-page class="column q-pt-md">
    <h4>Add a new recipe</h4>
    <q-form ref="recipeForm" class="q-mb-lg q-pl-sm q-pt-sm">
      <div>
        <div class="text-h5">Basic</div>
        <div class="q-pa-sm">
          <div class="row">
            <q-input
              outlined
              v-model="title"
              label="Title"
              :rules="[
                (val) => val.length > 0 || 'Required',
                (val) => val.length <= 200 || 'Max length 200',
              ]"
              class="col-12 q-pr-sm q-py-md"
            />
            <q-input
              outlined
              v-model="preparationMinutes"
              label="Preparation minutes"
              :rules="[(val) => val >= 0 || 'Positive number only']"
              class="col-12 col-md-4 q-pr-sm q-py-md"
            />
            <q-input
              outlined
              v-model="cookingMinutes"
              label="Cooking minutes"
              :rules="[(val) => val >= 0 || 'Positive number only']"
              class="col-12 col-md-4 q-pr-sm q-py-md"
            />
            <q-select
              outlined
              v-model="selectedCategory"
              :options="categories"
              :display-value="`${selectedCategory?.name ?? ''}`"
              :option-label="(item) => item.name"
              :loading="isLoading"
              :disable="isLoading"
              :rules="[(val) => val.id > 0 || 'Required']"
              label="Category"
              class="col-12 col-md-4 q-pr-sm q-py-md"
            />
          </div>
        </div>
      </div>

      <div>
        <div class="text-h5">Description</div>
        <div class="q-pa-sm">
          <div class="row">
            <q-input
              outlined
              type="textarea"
              v-model="description"
              label="Description"
              :rules="[(val) => val.length <= 2000 || 'Max length 2000']"
              class="col-12 q-pr-sm q-py-md"
            />
          </div>
        </div>
      </div>

      <div>
        <div class="text-h5">Ingredients</div>
        <div class="q-pa-sm">
          <div
            class="row items-center"
            v-for="(_, index) in ingredients"
            :key="index"
          >
            <q-input
              outlined
              v-model="ingredients[index]"
              label="Ingredient"
              class="col q-pr-sm q-py-md"
              placeholder="e.g. 1 carrot"
            />
            <div class="q-pr-sm q-py-md">
              <q-btn
                color="negative"
                text-color="black"
                label="Remove"
                @click="removeIngredient(index)"
              />
            </div>
          </div>
          <div class="row q-pr-sm q-py-md">
            <q-btn
              color="positive"
              @click="addIngredient"
              label="Add Ingredient"
            />
          </div>
        </div>
      </div>

      <div>
        <div class="text-h5">Instructions</div>
        <div class="q-pa-sm">
          <div class="row">
            <q-input
              outlined
              type="textarea"
              v-model="instructions"
              :rules="[
                (val) => val.length > 0 || 'Required',
                (val) => val.length <= 2000 || 'Max length 2000',
              ]"
              label="Instruction"
              class="col-12 q-pr-sm q-py-md"
            />
          </div>
        </div>
      </div>

      <div>
        <div class="text-h5">Photo</div>
        <div class="q-pa-sm">
          <div class="row">
            <q-file
              outlined
              clearable
              v-model="photo"
              label="Choose a photo"
              class="col-12 q-pr-sm q-py-md"
            >
              <template v-slot:prepend>
                <q-icon name="attach_file" />
              </template>
            </q-file>
          </div>
        </div>
      </div>

      <div class="q-pa-sm">
        <div class="row">
          <q-btn
            color="secondary"
            @click="saveDraft"
            label="Save Draft"
            :loading="isSavingDraft"
            :disable="isSavingPublish"
            class="q-mr-sm"
          />
          <q-btn
            color="primary"
            @click="saveAndPublish"
            label="Save and Publish"
            :loading="isSavingPublish"
            :disable="isSavingDraft"
          />
        </div>
      </div>
    </q-form>
  </q-page>
</template>

<script setup lang="ts">
import { storeToRefs } from 'pinia';
import { Notify, QForm } from 'quasar';
import { Category } from 'src/components/models';
import { useCategoryStore } from 'src/stores/category-store';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

defineOptions({
  name: 'RecipeAdd',
});

const title = ref('');
const preparationMinutes = ref(15);
const cookingMinutes = ref(15);
const selectedCategory = ref<Category | undefined>(undefined);
const description = ref('');
const ingredients = ref<string[]>(['']);
const instructions = ref('');
const photo = ref(null);
const isSavingDraft = ref(false);
const isSavingPublish = ref(false);

const categoryStore = useCategoryStore();
const { categories, isLoading } = storeToRefs(categoryStore);
const recipeStore = useMyRecipesStore();
const router = useRouter();
const recipeForm = ref<InstanceType<typeof QForm> | null>(null);

function removeIngredient(index: number) {
  ingredients.value.splice(index, 1);
}

function addIngredient() {
  ingredients.value.push('');
}

async function saveDraft() {
  isSavingDraft.value = true;
  try {
    await trySubmit();
  } finally {
    isSavingDraft.value = false;
  }
}

async function saveAndPublish() {
  isSavingPublish.value = true;
  try {
    await trySubmit();
    router.push({ path: '/recipes' });
  } finally {
    isSavingPublish.value = false;
  }
}

async function trySubmit() {
  // verify the form
  // then upload image
  if (!recipeForm.value) {
    return;
  }
  const isValid = await recipeForm.value.validate();
  if (!isValid) {
    Notify.create({
      message: 'Please check your input',
      color: 'negative',
    });
    throw new Error('The recipe form value is invalid.');
  }
  await recipeStore.save({
    categoryId: selectedCategory.value?.id ?? 0,
    cookingMinutes: cookingMinutes.value,
    description: description.value,
    ingredients: ingredients.value,
    instructions: instructions.value,
    isPublished: true,
    photo: '',
    preparationMinutes: preparationMinutes.value,
    title: title.value,
  });
  Notify.create({
    message: 'Recipe saved',
    color: 'positive',
  });
}
</script>
