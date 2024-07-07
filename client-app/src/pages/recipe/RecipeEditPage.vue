<template>
  <q-page class="column">
    <h4>Edit recipe</h4>
    <q-form v-if="model" ref="recipeForm" class="q-mb-lg q-pl-sm q-pt-sm">
      <div>
        <div class="text-h5">Basic</div>
        <div class="q-pa-sm">
          <div class="row">
            <q-input
              outlined
              v-model="model.title"
              label="Title"
              :rules="[
                (val) => val.length > 0 || 'Required',
                (val) => val.length <= 200 || 'Max length 200',
              ]"
              class="col-12 q-pr-sm q-py-md"
            />
            <q-input
              outlined
              v-model="model.preparationMinutes"
              label="Preparation minutes"
              :rules="[(val) => val >= 0 || 'Positive number only']"
              class="col-12 col-md-4 q-pr-sm q-py-md"
            />
            <q-input
              outlined
              v-model="model.cookingMinutes"
              label="Cooking minutes"
              :rules="[(val) => val >= 0 || 'Positive number only']"
              class="col-12 col-md-4 q-pr-sm q-py-md"
            />
            <q-select
              outlined
              v-model="model.categoryId"
              :options="categoryIds"
              :display-value="`${
                categories.find((x) => x.id === model?.categoryId)?.name ?? ''
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
              v-model="model.description"
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
            v-for="(_, index) in model.ingredients"
            :key="index"
          >
            <q-input
              outlined
              v-model="model.ingredients[index]"
              label="Ingredient"
              class="col q-pr-sm q-py-md"
              placeholder="e.g. 1 carrot"
              :rules="[
                (val) =>
                  (val && val.length > 0) ||
                  (model && model.ingredients.some((x) => x)) ||
                  'Required',
              ]"
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
          <div class="row q-pr-sm">
            <InstructionEditor
              v-model="model.instructions"
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
    <div v-else class="row justify-center">
      <q-spinner color="primary" size="3em" :thickness="10" />
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { storeToRefs } from 'pinia';
import { Notify, QForm, QUploader } from 'quasar';
import { managementApi } from 'src/boot/axios';
import { IRecipeEdit } from 'src/interfaces/Recipe';
import { useCategoryStore } from 'src/stores/category-store';
import { useMyRecipesStore } from 'src/stores/my-recipes-store';
import { computed, onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import InstructionEditor from 'src/components/recipe/InstructionEditor.vue';
import sanitizeHtml from 'sanitize-html';

defineOptions({
  name: 'RecipeEditPage',
});

const isPublished = ref('1');
const isSaving = ref(false);
const photoUploader = ref<QUploader | undefined>(undefined);

const categoryStore = useCategoryStore();
const { categories, isLoading } = storeToRefs(categoryStore);
const categoryIds = computed(() =>
  categories ? categories.value.map((x) => x.id) : []
);

const recipeStore = useMyRecipesStore();
const router = useRouter();
const route = useRoute();
const id = Number.parseInt(route.params.id as string);
const recipeForm = ref<InstanceType<typeof QForm> | null>(null);
const auth0 = useAuth0();

const model = ref<IRecipeEdit | null>(null);

onMounted(async () => {
  const recipe = await recipeStore.getMyRecipeById(id);
  if (!recipe) {
    Notify.create({
      message: 'Something went wrong. Please try again later.',
      color: 'negative',
    });
    return;
  }
  model.value = {
    categoryId: recipe.categoryId,
    cookingMinutes: recipe.cookingMinutes,
    description: recipe.description,
    id: id,
    imageFileName: recipe.imageFileName,
    ingredients: recipe.ingredients,
    instructions: sanitizeHtml(recipe.instructions),
    isPublished: recipe.isPublished,
    preparationMinutes: recipe.preparationMinutes,
    title: recipe.title,
  };
  isPublished.value = recipe.isPublished ? '1' : '0';

  setTimeout(async () => {
    if (photoUploader.value && recipe.imageUrl) {
      const response = await fetch(recipe.imageUrl, {
        mode: 'cors',
        headers: {
          'Access-Control-Allow-Origin': window.origin,
        },
      });
      const data = await response.blob();
      const metadata = {
        type: 'image/jpeg',
      };
      const file = new File([data], recipe.imageFileName, metadata);
      photoUploader.value.addFiles([file]);
    }
  }, 200);
});

function removeIngredient(index: number) {
  model.value?.ingredients.splice(index, 1);
}

function addIngredient() {
  model.value?.ingredients.push('');
}

async function save() {
  if (!model.value || !recipeForm.value) {
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
    model.value.imageFileName =
      imageFileNames && imageFileNames[0] ? imageFileNames[0] : null;
    model.value.isPublished = isPublished.value === '1';
    model.value.instructions = sanitizeHtml(model.value.instructions);

    await recipeStore.update(id, model.value);

    Notify.create({
      message: 'Recipe saved',
      color: 'positive',
    });

    if (model.value.isPublished) {
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
  const savedFileNames: string[] = [];
  const accessToken = await auth0.getAccessTokenSilently();
  const formData = new FormData();
  let hasNewFile = false;
  for (const file of photoUploader.value.files as File[]) {
    if (
      model.value &&
      model.value.imageFileName &&
      file.name === model.value.imageFileName
    ) {
      // file not change, no need to upload
      savedFileNames.push(model.value.imageFileName);
    } else {
      formData.append('files', file);
      hasNewFile = true;
    }
  }

  if (hasNewFile) {
    const response = await managementApi.post<string[]>('images', formData, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    for (const fileName of response.data ?? []) {
      savedFileNames.push(fileName);
    }
  }
  return savedFileNames;
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
