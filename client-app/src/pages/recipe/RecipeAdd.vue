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

            <!-- <q-select
              outlined
              v-model="selectedCategory"
              :options="[1, 2, 3]"
              :rules="[(val) => (val && val.id > 0) || 'Required']"
              label="Category"
              class="col-12 col-md-4 q-pr-sm q-py-md"
            /> -->

            <q-select
              outlined
              v-model="selectedCategoryId"
              :options="categoryIds"
              :display-value="`${
                categories.find((x) => x.id === selectedCategoryId)?.name ?? ''
              }`"
              :option-label="(id) => categories.find((x) => x.id == id)?.name"
              :loading="isLoading"
              :disable="isLoading"
              :rules="[(val) => (val && val > 0) || 'Required']"
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
          <div class="row q-py-md">
            <q-uploader
              ref="photoUploader"
              style="max-width: 300px; min-height: 250px"
              label="Choose a photo - Max size 2 MB"
              accept=".jpg, image/*"
              max-file-size="2097152"
              hide-upload-btn
              @rejected="onPhotoRejected"
              rule
            />
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
            label="Save & Publish"
            :loading="isSavingPublish"
            :disable="isSavingDraft"
          />
        </div>
      </div>
    </q-form>
  </q-page>
</template>

<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { storeToRefs } from 'pinia';
import { Notify, QForm, QUploader } from 'quasar';
import { useCategoryStore } from 'src/stores/category-store';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';

defineOptions({
  name: 'RecipeAdd',
});

const title = ref('');
const preparationMinutes = ref(15);
const cookingMinutes = ref(15);
const selectedCategoryId = ref<number | undefined>(undefined);
const description = ref('');
const ingredients = ref<string[]>(['']);
const instructions = ref('');
const isSavingDraft = ref(false);
const isSavingPublish = ref(false);
const photoUploader = ref<QUploader | undefined>(undefined);

const categoryStore = useCategoryStore();
const { categories, isLoading } = storeToRefs(categoryStore);
const categoryIds = computed(() =>
  categories ? categories.value.map((x) => x.id) : []
);

const recipeStore = useMyRecipesStore();
const router = useRouter();
const recipeForm = ref<InstanceType<typeof QForm> | null>(null);
const auth0 = useAuth0();

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

  if (!photoUploader.value) {
    Notify.create({
      message: 'Something went wrong. Please try again later.',
      color: 'negative',
    });
    throw new Error('The photo uploader has not been initialized.');
  }

  const imageFileNames = await uploadPhotos();
  await recipeStore.save({
    categoryId: selectedCategoryId.value ?? 0,
    cookingMinutes: cookingMinutes.value,
    description: description.value,
    ingredients: ingredients.value,
    instructions: instructions.value,
    isPublished: true,
    imageFileName:
      imageFileNames && imageFileNames[0] ? imageFileNames[0] : null,
    preparationMinutes: preparationMinutes.value,
    title: title.value,
  });

  Notify.create({
    message: 'Recipe saved',
    color: 'positive',
  });
}

async function uploadPhotos() {
  if (!photoUploader.value || photoUploader.value.files.length === 0) {
    return [];
  }
  const accessToken = await auth0.getAccessTokenSilently();
  const formData = new FormData();
  for (const file of photoUploader.value.files) {
    formData.append('files', file);
  }

  const response = await fetch('https://localhost:7000/management/images', {
    method: 'POST',
    body: formData,
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
  const data = await response.json();
  return data;
}

function onPhotoRejected() {
  Notify.create({
    type: 'negative',
    message: 'Photo invalid',
  });
}
</script>

<style>
.q-uploader__subtitle {
  display: none;
}
</style>
