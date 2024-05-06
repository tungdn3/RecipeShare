import { authGuard } from '@auth0/auth0-vue';
import RecipeList from 'src/pages/recipe/RecipeList.vue';
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
        component: () => import('pages/recipe/RecipeDetails.vue'),
      },
      {
        path: '',
        component: RecipeList,
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
      { path: '', component: () => import('pages/user/SettingPage.vue') },
    ],
  },
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/IndexPage.vue') }],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
