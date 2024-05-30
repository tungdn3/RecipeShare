<template>
  <div
    v-if="isRefreshing || isDeleting || isEditing"
    class="row q-ml-md q-my-md"
  >
    <q-spinner-dots color="secondary" size="30px" />
  </div>
  <div v-if="items.length">
    <q-infinite-scroll @load="onLoad" :offset="250">
      <CommentCard
        v-for="comment in items"
        :key="comment.id"
        :avatar-url="comment.userAvatarUrl"
        :content="comment.content"
        :created-at="new Date(comment.createdAt)"
        :id="comment.id"
        :user-name="comment.userDisplayName"
        :reply-count="comment.replyCount"
        :recipe-id="recipeId"
        :recipe-author-id="recipeAuthorId"
        :user-id="comment.userId"
        @delete="() => handleDelete(comment.id)"
        @edit="(content) => handleEdit(comment.id, content)"
      />
      <template v-slot:loading>
        <div class="row justify-center q-my-md">
          <q-spinner-dots color="secondary" size="40px" />
        </div>
      </template>
    </q-infinite-scroll>
  </div>
</template>

<script setup lang="ts">
import { socialApi } from 'src/boot/axios';
import { ICommentDisplay } from 'src/interfaces/Comment';
import { IPageResult } from 'src/interfaces/Common';
import { onMounted, ref } from 'vue';
import CommentCard from 'src/components/comment/CommentCard.vue';
import { useAuth0 } from '@auth0/auth0-vue';

defineOptions({
  name: 'CommentList',
});

export interface CommentListProps {
  recipeId: number;
  recipeAuthorId: string;
}

const props = withDefaults(defineProps<CommentListProps>(), {
  recipeId: 1,
});

const emit = defineEmits(['countChanged']);

defineExpose({
  refresh,
});

const auth0 = useAuth0();
const pageSize = 10;
const pageResult = ref<IPageResult<ICommentDisplay> | undefined>(undefined);
const items = ref<ICommentDisplay[]>([]);
const isRefreshing = ref(false);
const isDeleting = ref(false);
const isEditing = ref(false);

onMounted(() => {
  refresh();
});

async function refresh() {
  isRefreshing.value = true;
  try {
    await getComments(1);
  } finally {
    isRefreshing.value = false;
  }
}

async function onLoad(index: number, done: (stop?: boolean) => void) {
  if (!pageResult.value?.totalPages || index >= pageResult.value?.totalPages) {
    done(true);
    return;
  }
  if (index === pageResult.value?.pageNumber) {
    await getComments(index + 1);
    done(!pageResult.value.hasNextPage);
  }
}

async function getComments(pageNumber: number) {
  const response = await socialApi.get<IPageResult<ICommentDisplay>>(
    `comments?recipeId=${props.recipeId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
  pageResult.value = response.data;
  items.value =
    pageNumber === 1
      ? response.data.items
      : items.value.concat(response.data.items);

  emit('countChanged', response.data.totalCount);
}

async function handleDelete(id: number) {
  isDeleting.value = true;
  try {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await socialApi.delete(`comments/${id}`, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    if (response.status === 200 || response.status === 204) {
      refresh();
    }
  } finally {
    isDeleting.value = false;
  }
}

async function handleEdit(id: number, content: string) {
  if (!content) {
    return;
  }
  isEditing.value = true;
  try {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await socialApi.put(
      `comments/${id}`,
      {
        content: content,
      },
      {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    if (response.status === 200 || response.status === 201) {
      isEditing.value = false;
      setTimeout(() => {
        refresh();
      }, 100);
    }
  } catch {
    isEditing.value = false;
  }
}
</script>
