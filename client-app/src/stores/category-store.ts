import { defineStore } from 'pinia';
import Category from 'src/interfaces/Category';
import { useAuth0 } from '@auth0/auth0-vue';
import { managementApi } from 'src/boot/axios';
import { useQuasar } from 'quasar';

export const useCategoryStore = () => {
  const $q = useQuasar();
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
          const response = await managementApi.get<Category[]>('categories', {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          });
          this.categories = response.data;
          return this.categories;
        } catch (e) {
          $q.notify({
            message: 'Failed to get category list. Please try again later.',
            color: 'negative',
            actions: [
              {
                icon: 'close',
                color: 'white',
                round: true,
                handler: () => {
                  /* ... */
                },
              },
            ],
          });
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
  }

  return innerStore;
};
