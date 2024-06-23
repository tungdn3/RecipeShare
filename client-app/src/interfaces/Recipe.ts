import { IUser } from './Common';

export interface IRecipeCard {
  id: number;
  title: string;
  imageUrl: string;
  preparationMinutes: number;
  cookingMinutes: number;
  likeCount: number | undefined;
  commentCount: number | undefined;
}

export interface IRecipeAdd {
  title: string;
  preparationMinutes: number;
  cookingMinutes: number;
  categoryId: number;
  description: string;
  ingredients: string[];
  instructions: string;
  imageFileName: string | null;
  isPublished: boolean;
}

export interface IRecipeEdit {
  id: number;
  title: string;
  preparationMinutes: number;
  cookingMinutes: number;
  categoryId: number;
  description: string;
  ingredients: string[];
  instructions: string;
  imageFileName: string | null;
  isPublished: boolean;
}

export interface IRecipe {
  id: number;
  categoryId: number;
  categoryName: string;
  title: string;
  description: string;
  imageUrl: string;
  imageFileName: string;
  preparationMinutes: number;
  cookingMinutes: number;
  ingredients: string[];
  instructions: string;
  user: IUser;
  createdAt: Date;
  updatedAt: Date;
  publishedAt: Date;
  isPublished: boolean;
}
