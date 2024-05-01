export interface RecipeOverview {
  id: number;
  title: string;
  imageUrl: string;
  preparationMinutes: number;
  cookingMinutes: number;
  nbOfLikes: number;
  nbOfComments: number;
}

export interface RecipeAdd {
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

export interface RecipeEdit {
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

export interface Recipe {
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
  userId: string;
  userName: string;
  createdAt: Date;
  updatedAt: Date;
  publishedAt: Date;
  isPublished: boolean;
}
