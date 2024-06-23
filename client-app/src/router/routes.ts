import { authGuard } from '@auth0/auth0-vue';
import MyRecipesPage from 'src/pages/recipe/MyRecipesPage.vue';
import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/my-recipes',
    component: () => import('layouts/MainLayout.vue'),
    beforeEnter: authGuard,
    children: [
      { path: 'add', component: () => import('pages/recipe/RecipeAdd.vue') },
      {
        path: ':id/edit',
        component: () => import('pages/recipe/RecipeEdit.vue'),
      },
      {
        path: ':id',
        component: () => import('pages/recipe/RecipeDetailsPage.vue'),
      },
      {
        path: '',
        component: MyRecipesPage,
      },
    ],
  },
  {
    path: '/user',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      {
        path: 'settings',
        component: () => import('pages/user/SettingsPage.vue'),
      },
      { path: '', component: () => import('pages/user/SettingsPage.vue') },
    ],
  },
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', component: () => import('pages/IndexPage.vue') },
      {
        path: 'recipes/:id',
        component: () => import('pages/recipe/RecipeDetailsPage.vue'),
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
