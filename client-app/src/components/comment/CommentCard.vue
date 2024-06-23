<template>
  <CommentForm
    v-if="isEditMode"
    is-edit
    :initial-content="props.content"
    @submit="onEdit"
    @cancel="() => (isEditMode = false)"
    class="col"
  />

  <div
    v-else
    class="row no-wrap q-pa-sm q-my-sm"
    :class="{ deleting: deleting }"
  >
    <div class="">
      <q-avatar size="md">
        <img :src="avatarUrl" />
      </q-avatar>
    </div>
    <div class="q-pl-sm right-container">
      <div class="row">
        <div
          class="text-primary text-bold"
          :class="{
            owner: recipeAuthorId === userId,
            'bg-secondary': recipeAuthorId === userId,
            'q-px-sm': recipeAuthorId === userId,
          }"
        >
          {{ userName }}
        </div>
        <div class="q-ml-sm created-at">{{ formatDate(props.createdAt) }}</div>
      </div>
      <div class="row">
        <div>{{ props.content }}</div>

        <q-btn
          v-if="isCommentAuthor"
          flat
          dense
          :disable="deleting"
          size="sm"
          icon="more_horiz"
          class="context-btn"
        >
          <q-menu>
            <q-list dense style="min-width: 100px">
              <q-item
                clickable
                v-close-popup
                class="text-primary"
                @click="() => (isEditMode = true)"
              >
                <q-item-section avatar>
                  <q-icon size="xs" name="edit" />
                </q-item-section>
                <q-item-section>Edit</q-item-section>
              </q-item>
              <q-item
                clickable
                v-close-popup
                class="text-negative"
                @click="onDelete"
              >
                <q-item-section avatar>
                  <q-icon size="xs" name="delete" />
                </q-item-section>
                <q-item-section>Delete</q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </q-btn>
      </div>
      <!-- <div>read more</div> -->
      <div class="">
        <q-btn
          flat
          dense
          color="primary"
          label="Reply"
          size="sm"
          icon="reply"
          @click="() => (isReplyFormShown = true)"
        />

        <ReplyForm
          v-if="isReplyFormShown"
          @cancel="() => (isReplyFormShown = false)"
          @submit="submitReply"
        />

        <div v-if="isReplySubmitting" class="row q-ml-md q-my-sm">
          <q-spinner-dots color="secondary" size="30px" />
        </div>
      </div>
      <div v-if="repliesLatestCount || props.replyCount" class="q-mt-xs">
        <q-expansion-item
          dense
          expand-separator
          v-model="isRepliesShown"
          icon="question_answer"
          :label="`${formatNumber(
            repliesLatestCount ?? props.replyCount
          )} replies`"
          header-class="text-primary text-bold q-pl-sm"
        >
          <ReplyList
            v-if="isRepliesShown"
            :parent-id="props.id"
            :recipe-id="recipeId"
            :author-id="recipeAuthorId"
            :recipe-author-id="recipeAuthorId"
            :page-number="repliesPageNumber"
            :has-next-page="repliesHasNextPage"
            :is-loading="repliesIsLoading"
            :items="replies"
            @load="loadReplies"
            @delete="handleReplyDelete"
          />
        </q-expansion-item>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import formatDate from 'src/utilities/format-date';
import formatNumber from 'src/utilities/format-number';
import ReplyList from './ReplyList.vue';
import ReplyForm from './ReplyForm.vue';
import { computed, ref, watch } from 'vue';
import { socialApi } from 'src/boot/axios';
import { useAuth0 } from '@auth0/auth0-vue';
import { ICommentDisplay } from 'src/interfaces/Comment';
import { IPageResult } from 'src/interfaces/Common';
import CommentForm from './CommentForm.vue';

export interface CommentCardProps {
  id: number;
  parentId?: number;
  recipeId: number;
  content: string;
  createdAt: Date;
  replyCount: number;
  avatarUrl?: string;
  userName: string;
  userId: string;
  recipeAuthorId: string;
}

const props = defineProps<CommentCardProps>();
const emit = defineEmits(['delete', 'edit']);

let isRepliesLoaded = false;
const isRepliesShown = ref(false);
const isReplyFormShown = ref(false);
const auth0 = useAuth0();

let repliesPageNumber = 1;
const repliesPageSize = 10;
const repliesHasNextPage = ref(false);
const repliesIsLoading = ref(false);
const replies = ref<ICommentDisplay[]>([]);
const repliesLatestCount = ref<number | undefined>(undefined);
const isReplySubmitting = ref(false);
const deleting = ref(false);
const isCommentAuthor = computed(() => auth0.user.value?.sub === props.userId);
const isReplyDeleting = ref(false);
const isEditMode = ref(false);

watch(isRepliesShown, async (newValue) => {
  if (!newValue || !props.replyCount || isRepliesLoaded) {
    return;
  }
  loadReplies(1);
});

async function loadReplies(pageNumber: number) {
  if (!isReplySubmitting.value) {
    repliesIsLoading.value = true;
  }
  try {
    const response = await socialApi.get<IPageResult<ICommentDisplay>>(
      `comments/${props.id}/replies?pageNumber=${pageNumber}&pageSize=${repliesPageSize}`
    );
    repliesHasNextPage.value = response.data.hasNextPage;
    replies.value =
      pageNumber === 1
        ? response.data.items
        : replies.value.concat(response.data.items);

    pageNumber++;
    isRepliesLoaded = true;
    repliesLatestCount.value = response.data.totalCount;
  } finally {
    repliesIsLoading.value = false;
  }
}

async function submitReply(content: string) {
  isReplySubmitting.value = true;
  isReplyFormShown.value = false;
  try {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await socialApi.post(
      'comments',
      {
        recipeId: props.recipeId,
        parentId: props.id,
        content: content,
      },
      {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    if (response.status === 200 || response.status === 201) {
      isReplyFormShown.value = false;
      await loadReplies(1);
      isRepliesShown.value = true;
    }
  } finally {
    isReplySubmitting.value = false;
  }
}

function onDelete() {
  deleting.value = true;
  emit('delete');
}

async function onEdit(content: string) {
  if (!content) {
    return;
  }
  emit('edit', content);
  isEditMode.value = false;
}

async function handleReplyDelete(replyId: number) {
  isReplyDeleting.value = true;
  try {
    const accessToken = await auth0.getAccessTokenSilently();
    const response = await socialApi.delete(`comments/${replyId}`, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    if (response.status === 200 || response.status === 204) {
      loadReplies(1);
    }
  } finally {
    isReplyDeleting.value = false;
  }
}
</script>

<style scoped>
.right-container {
  width: 100%;
}

.owner {
  border-radius: 8px;
}

.created-at {
  font-size: smaller;
  align-content: center;
}

.context-btn {
  position: absolute;
  right: 0;
  margin-right: 8px;
}

.deleting {
  opacity: 0.5;
}
</style>
