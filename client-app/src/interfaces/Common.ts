export interface IPageResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ICountItem {
  id: number;
  count: number;
}

export interface IUser {
  id: string;
  displayName: string;
  avatarUrl: string;
}
