<template>
  <div class="list">
    <div
      class="item"
      v-for="(item, index) in props.items"
      :key="index"
      @click="$emit('selected', item)"
    >
      {{ item }}
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  items: Array<string>,
});
</script>

<style scoped>
.list {
  background-color: lightgray;
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
