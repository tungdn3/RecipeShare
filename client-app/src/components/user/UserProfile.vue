<template>
  <div>
    <div v-if="!isAuthenticated">
      <q-btn flat round icon="group_add">
        <q-menu>
          <q-list style="min-width: 100px">
            <q-item clickable v-close-popup>
              <q-item-section>Register</q-item-section>
            </q-item>

            <q-separator />

            <q-item clickable v-close-popup @click="login">
              <q-item-section>Log In</q-item-section>
            </q-item>
          </q-list>
        </q-menu>
      </q-btn>
    </div>
    <div v-if="isAuthenticated">
      <q-btn round>
        <q-avatar>
          <img :src="user.picture" />
        </q-avatar>
        <q-menu>
          <q-list style="min-width: 100px">
            <q-item v-close-popup>
              <q-item-section>{{ user.name }}</q-item-section>
            </q-item>

            <q-separator />

            <q-item clickable v-close-popup @click="goToMyRecipes">
              <q-item-section>My Posts</q-item-section>
            </q-item>

            <q-separator />

            <q-item clickable v-close-popup>
              <div @click="logout">
                <q-item-section>Log Out</q-item-section>
              </div>
            </q-item>
          </q-list>
        </q-menu>
      </q-btn>
    </div>
  </div>
</template>

<!-- <script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { useRouter } from 'vue-router';

defineOptions({
  name: 'UserProfile',
});

const auth0 = useAuth0();
console.log('------ auth0', auth0);
const router = useRouter();

function login() {
  auth0.loginWithRedirect();
}

function logout() {
  auth0.logout({
    logoutParams: {
      returnTo: window.location.origin,
    },
  });
}

function goToMyRecipes() {
  router.push('/recipes');
}
</script> -->

<script>
import { useAuth0 } from '@auth0/auth0-vue';
import { useRouter } from 'vue-router';

export default {
  setup() {
    const auth0 = useAuth0();
    console.log('-------- user', auth0.user);
    const router = useRouter();

    function login() {
      auth0.loginWithRedirect();
    }

    function logout() {
      auth0.logout({
        logoutParams: {
          returnTo: window.location.origin,
        },
      });
    }

    function goToMyRecipes() {
      router.push('/recipes');
    }

    return {
      user: auth0.user,
      isAuthenticated: auth0.isAuthenticated,
      isLoading: auth0.isLoading,
      login,
      logout,
      goToMyRecipes,
    };
  },
};
</script>
