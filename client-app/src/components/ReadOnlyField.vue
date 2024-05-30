<template>
  <q-item>
    <q-item-section class="bg-secondary text-dark q-pa-sm">
      <q-item-label>{{ label }}</q-item-label>
    </q-item-section>

    <q-item-section v-if="!noValue" class="field-value q-pa-sm">
      <q-item-label>{{ value }}</q-item-label>
    </q-item-section>
  </q-item>
</template>

<script setup lang="ts">
defineOptions({
  name: 'ReadOnlyField',
});

export interface ReadOnlyFieldProps {
  label: string;
  value?: string;
  noValue?: boolean;
}

withDefaults(defineProps<ReadOnlyFieldProps>(), {
  label: '',
  value: '',
  noValue: false,
});
</script>

<style scoped>
.field-value {
  background-color: cornsilk;
  margin-left: 0;
}
</style>
