<template>
  <div>
    <CommentCard
      v-for="comment in items"
      :key="comment.id"
      :avatar-url="''"
      :content="comment.content"
      :created-at="new Date(comment.createdAt)"
      :id="comment.id"
      :user-name="comment.userDisplayName"
      :parent-id="comment.id"
      :reply-count="comment.replyCount"
      :recipe-author-id="recipeAuthorId"
      :user-id="comment.userId"
      :recipe-id="recipeId"
      @delete="$emit('delete', comment.id)"
    />

    <div v-if="isLoading" class="row q-ml-md q-my-md">
      <q-spinner-dots color="secondary" size="30px" />
    </div>

    <q-btn
      v-if="hasNextPage && !isLoading"
      size="sm"
      color="white"
      text-color="black"
      label="Load more"
      @click="$emit('load', pageNumber + 1)"
    />
  </div>
</template>

<script setup lang="ts">
import { ICommentDisplay } from 'src/interfaces/Comment';
import CommentCard from 'src/components/comment/CommentCard.vue';

defineOptions({
  name: 'ReplyList',
});

export interface ReplyListProps {
  recipeId: number;
  recipeAuthorId: string;
  items: Array<ICommentDisplay>;
  pageNumber: number;
  isLoading: boolean;
  hasNextPage: boolean;
}

defineProps<ReplyListProps>();
defineEmits(['load', 'delete']);
</script>
