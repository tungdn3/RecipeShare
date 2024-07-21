<template>
  <q-layout view="hhh LpR fff">
    <q-header elevated class="bg-primary text-white" height-hint="98">
      <q-toolbar>
        <q-btn dense flat round icon="menu" @click="toggleLeftDrawer" />

        <!-- <q-avatar>
          <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg" />
        </q-avatar> -->

        <q-toolbar-title>Recipe Share</q-toolbar-title>

        <NotificationContainer />
        <UserProfile class="q-ml-md" />
      </q-toolbar>

      <q-tabs align="center">
        <q-route-tab to="/" label="Home" />
        <q-route-tab to="/my-recipes" label="My Recipes" />
        <q-route-tab to="/my-recipes/add" label="Submit A Recipe" />
      </q-tabs>
    </q-header>

    <q-drawer show-if-above v-model="leftDrawerOpen" side="left" bordered>
      <!-- drawer content -->
    </q-drawer>

    <q-drawer show-if-above v-model="rightDrawerOpen" side="right" bordered>
      <!-- drawer content -->
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>

    <q-footer elevated class="bg-grey-8 text-white">
      <div class="row justify-center">
        Copyright 2024 RecipeShare. All rights reserved
      </div>
    </q-footer>
  </q-layout>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import UserProfile from '../components/user/UserProfile.vue';
import NotificationContainer from 'src/components/notification/NotificationContainer.vue';

const leftDrawerOpen = ref(false);
const rightDrawerOpen = ref(false);

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}
</script>
