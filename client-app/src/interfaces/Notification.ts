export interface INotification {
  id: number;
  fromUserId: string;
  fromUserDisplayName: string;
  toUserId: string;
  recipeId: number;
  parentCommentId: number | undefined | null;
  commentId: number | undefined | null;
  replyId: number | undefined | null;
  type: 'comment' | 'reply' | 'like';
}
