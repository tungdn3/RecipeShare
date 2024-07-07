<template>
  <q-editor
    ref="editorRef"
    v-model="model"
    min-height="5rem"
    placeholder="Step-by-step instructions"
    @paste="onPaste"
    style="width: 100%"
  />
</template>

<script setup lang="ts">
import { QEditor } from 'quasar';
import { ref } from 'vue';

const model = defineModel<string>();
const editorRef = ref<QEditor | null>(null);

defineOptions({
  name: 'InstructionEditor',
});

function onPaste(evt: any) {
  if (!editorRef.value) return;
  // Let inputs do their thing, so we don't break pasting of links.
  if (evt.target.nodeName === 'INPUT') return;
  let text, onPasteStripFormattingIEPaste;
  evt.preventDefault();
  evt.stopPropagation();
  if (evt.originalEvent && evt.originalEvent.clipboardData.getData) {
    text = evt.originalEvent.clipboardData.getData('text/plain');
    editorRef.value.runCmd('insertText', text);
  } else if (evt.clipboardData && evt.clipboardData.getData) {
    text = evt.clipboardData.getData('text/plain');
    editorRef.value.runCmd('insertText', text);
  } else if (
    (window as any).clipboardData &&
    (window as any).clipboardData.getData
  ) {
    if (!onPasteStripFormattingIEPaste) {
      onPasteStripFormattingIEPaste = true;
      editorRef.value.runCmd('ms-pasteTextOnly', text);
    }
    onPasteStripFormattingIEPaste = false;
  }
}
</script>

<style lang="scss"></style>
