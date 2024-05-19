import { defineStore } from 'pinia';
import Category from 'src/interfaces/Category';
import { useAuth0 } from '@auth0/auth0-vue';
import { api } from 'src/boot/axios';

export const useCategoryStore = () => {
  const categoryStore = defineStore('category', {
    state: () => {
      return {
        categories: [] as Category[],
        isLoading: false,
      };
    },
    getters: {},
    actions: {
      async fetchCategories() {
        this.isLoading = true;
        const auth0 = useAuth0();
        const accessToken = await auth0.getAccessTokenSilently();
        try {
          const response = await api.get<Category[]>('/management/categories', {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          });
          this.categories = response.data;
          return this.categories;
        } catch (e) {
          return [];
        } finally {
          this.isLoading = false;
        }
      },
    },
  });

  const innerStore = categoryStore();
  if (innerStore.categories.length === 0) {
    innerStore.fetchCategories();
  } else {
    console.log(
      '------- categories exist, no need to fetch',
      innerStore.categories
    );
  }

  return innerStore;
};
