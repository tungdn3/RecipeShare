export interface ICommentDisplay {
  id: number;
  parentId: number;
  content: string;
  createdAt: Date;
  updatedAt: Date;
  replyCount: number;
  userId: string;
  userDisplayName: string;
  userAvatarUrl: string | undefined;
}
