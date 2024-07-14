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
              v-model="selectedCategoryId"
              :options="categoryIds"
              :display-value="`${
                categories.find((x) => x.id === selectedCategoryId)?.name ?? ''
              }`"
              :option-label="(id) => categories.find((x) => x.id == id)?.name"
              :loading="isLoading"
              :disable="isLoading"
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
            />
          </div>
        </div>
      </div>

      <div>
        <div class="text-h5">Status</div>
        <div class="q-pa-sm">
          <div class="row">
            <q-radio
              v-model="isPublished"
              val="0"
              label="Draft"
              color="secondary"
            />
            <q-radio
              v-model="isPublished"
              val="1"
              label="Publish"
              color="primary"
              text
            />
          </div>
        </div>
      </div>

      <div class="q-pa-sm">
        <div class="row justify-end">
          <q-btn
            color="primary"
            @click="save"
            label="Save"
            :loading="isSaving"
            class="q-mr-sm"
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
import { managementApi } from 'src/boot/axios';
import { useCategoryStore } from 'src/stores/category-store';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';

defineOptions({
  name: 'RecipeAddPage',
});

const title = ref('');
const preparationMinutes = ref(15);
const cookingMinutes = ref(15);
const selectedCategoryId = ref<number | undefined>(undefined);
const description = ref('');
const ingredients = ref<string[]>(['']);
const instructions = ref('');
const isSaving = ref(false);
const isPublished = ref('1');
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

async function save() {
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

  isSaving.value = true;
  try {
    const imageFileNames = await uploadPhotos();
    await recipeStore.create({
      categoryId: selectedCategoryId.value ?? 0,
      cookingMinutes: cookingMinutes.value,
      description: description.value,
      ingredients: ingredients.value.filter((x) => x),
      instructions: instructions.value,
      isPublished: isPublished.value === '1',
      imageFileName:
        imageFileNames && imageFileNames[0] ? imageFileNames[0] : null,
      preparationMinutes: preparationMinutes.value,
      title: title.value,
    });

    Notify.create({
      message: 'Recipe saved',
      color: 'positive',
    });

    if (isPublished.value) {
      router.push({ path: '/my-recipes' });
    }
  } finally {
    isSaving.value = false;
  }
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

  const response = await managementApi.post('images', formData, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
  return response.data;
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
