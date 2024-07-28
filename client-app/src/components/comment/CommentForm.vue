<template>
  <div class="row">
    <div class="col-12 row items-center">
      <q-avatar size="md">
        <img :src="currentUserAvatarUrl" />
      </q-avatar>
      <q-input
        v-model="content"
        autogrow
        :label="isEdit ? 'Edit comment' : 'Add a comment...'"
        class="q-pl-md col-grow"
      />
    </div>
    <div v-if="content" class="col-12 q-mt-sm row justify-end">
      <q-btn
        flat
        size="sm"
        color="white"
        text-color="black"
        label="Cancel"
        @click="onCancel"
      />
      <q-btn
        size="sm"
        class="q-ml-sm"
        color="primary"
        :label="isEdit ? 'Save' : 'Comment'"
        :disable="!content"
        @click="$emit('submit', content)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAuth0 } from '@auth0/auth0-vue';

defineOptions({
  name: 'CommentForm',
});

defineExpose({
  reset,
});

export interface CommentFormProps {
  isEdit?: boolean;
  initialContent?: string;
}

const props = defineProps<CommentFormProps>();

const auth0 = useAuth0();
const currentUserAvatarUrl = ref<string | undefined>(auth0.user.value?.picture);
const content = ref<string>(props.initialContent ?? '');
const emit = defineEmits(['submit', 'cancel']);

function onCancel() {
  emit('cancel');
  reset();
}

function reset() {
  content.value = '';
}
</script>
