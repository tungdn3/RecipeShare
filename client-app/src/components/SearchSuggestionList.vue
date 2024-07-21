<template>
  <q-list dense bordered padding class="rounded-borders list">
    <q-item
      clickable
      v-ripple
      v-for="(item, index) in props.items"
      :key="index"
      @click="$emit('selected', item)"
    >
      <q-item-section> {{ item }} </q-item-section>
    </q-item>
  </q-list>
</template>

<script setup lang="ts">
const props = defineProps({
  items: Array<string>,
});
</script>

<style scoped>
.list {
  background-color: rgb(252, 254, 251);
  border-top-left-radius: 0;
  border-top-right-radius: 0;
  margin-top: 0;
  z-index: 1000;
}

.item {
  padding-left: 1rem;
  padding-right: 1rem;
  padding-top: 0.2rem;
  padding-bottom: 0.2rem;
}

.item:hover {
  background-color: #b2d371;
}
</style>
