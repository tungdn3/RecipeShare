<template>
  <div class="row">
    <div class="col-12 row items-center">
      <q-avatar size="lg">
        <img :src="currentUserAvatarUrl" />
      </q-avatar>
      <q-input
        v-model="replyContent"
        autogrow
        label="Add a reply..."
        class="q-pl-md col-grow"
      />
    </div>
    <div class="col-12 q-mt-sm row justify-end">
      <q-btn
        flat
        size="sm"
        color="white"
        text-color="black"
        label="Cancel"
        @click="$emit('cancel')"
      />
      <q-btn
        size="sm"
        class="q-ml-sm"
        color="secondary"
        label="Reply"
        :disable="!replyContent"
        @click="$emit('submit', replyContent)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAuth0 } from '@auth0/auth0-vue';

defineOptions({
  name: 'ReplyForm',
});

const auth0 = useAuth0();
const currentUserAvatarUrl = ref<string | undefined>(auth0.user.value?.picture);
const replyContent = ref('');
</script>
