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
          :value="recipe ? recipe.user.displayName : ''"
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
      <div v-if="recipe" class="col-12 q-mt-lg">
        <q-btn
          v-if="!likeId"
          outline
          size="sm"
          padding="xs"
          color="secondary"
          icon="favorite_border"
          :label="formatNumber(likeCount)"
          @click="onLike"
        />
        <q-btn
          v-else
          :outline="false"
          size="sm"
          padding="xs"
          color="primary"
          icon="favorite"
          :label="formatNumber(likeCount)"
          @click="onUnlike"
        />
      </div>
    </div>
    <q-separator class="q-mt-lg" />

    <div class="text-h6 text-primary q-mt-md">
      {{ formatNumber(commentCount ?? 0) }} comments
    </div>

    <CommentForm
      v-if="!isCommentSubmitting"
      ref="commentForm"
      class="q-mt-md"
      @submit="submitComment"
    />

    <div v-if="isCommentSubmitting" class="row q-ml-md q-my-md">
      <q-spinner-dots color="secondary" size="30px" />
    </div>

    <CommentList
      ref="commentList"
      :recipe-id="id"
      :recipe-author-id="recipe?.user.id ?? ''"
      class="q-mt-md"
      @count-changed="(val) => (commentCount = val)"
    />
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
import formatNumber from 'src/utilities/format-number';
import { socialApi } from 'src/boot/axios';
import { useAuth0 } from '@auth0/auth0-vue';
import { ICountItem } from 'src/interfaces/Common';

defineOptions({
  name: 'RecipeDetailsPage',
});

const route = useRoute();
const auth0 = useAuth0();
const id = ref(Number.parseInt(route.params.id as string));
const recipeStore = useMyRecipesStore();
const recipe = ref<IRecipe | null>();
const likeId = ref<number | undefined>(undefined);
const likeCount = ref<number | undefined>(undefined);
const commentCount = ref<number | undefined>(undefined);
const isCommentSubmitting = ref(false);
const commentForm = ref<InstanceType<typeof CommentForm> | null>(null);
const commentList = ref<InstanceType<typeof CommentList> | null>(null);

onMounted(async () => {
  recipe.value = await recipeStore.getMyRecipeById(id.value);
  refreshLikeStatus();
  getLikeCount();
});

async function refreshLikeStatus() {
  const accessToken = await auth0.getAccessTokenSilently();
  const likeStatusResponse = await socialApi.get<{ id: number }>(
    `likes?recipeId=${id.value}`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );
  likeId.value = likeStatusResponse.data.id;
}

async function getLikeCount() {
  const result = await socialApi.post<ICountItem[]>('likes/count', [id.value]);
  likeCount.value = result.data?.find((x) => x.id === id.value)?.count;
}

async function onLike() {
  const accessToken = await auth0.getAccessTokenSilently();
  const response = await socialApi.post<number>(
    'likes',
    {
      recipeId: id.value,
    },
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );
  if (response.status == 200 || response.status == 201) {
    likeId.value = response.data;
  }
}

async function onUnlike() {
  const accessToken = await auth0.getAccessTokenSilently();
  const response = await socialApi.delete(`likes/${likeId.value}`, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
  if (response.status == 200 || response.status == 204) {
    likeId.value = undefined;
  }
}

async function submitComment(content: string | undefined) {
  console.log('---------- submit comment', content);
  if (!content) {
    return;
  }
  isCommentSubmitting.value = true;
  try {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await socialApi.post(
      'comments',
      {
        recipeId: id.value,
        parentId: null,
        content: content,
      },
      {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    if (response.status === 200 || response.status === 201) {
      commentForm.value?.reset();
      isCommentSubmitting.value = false;
      setTimeout(() => {
        commentList.value?.refresh();
      }, 100);
    }
  } catch {
    isCommentSubmitting.value = false;
  }
}
</script>
