import RecipeList from 'src/pages/recipe/RecipeList.vue';
import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/IndexPage.vue') }],
  },
  {
    path: '/recipes',
    component: () => import('layouts/MainLayout.vue'),
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
        //component: () => import('pages/recipe/RecipeList.vue'),
        component: RecipeList,
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
