<template>
  <q-field
    ref="fieldRef"
    v-model="model"
    label-slot
    borderless
    style="margin-top: -24px"
    :rules="[(val) => (!!val && val !== '<br>') || 'Required']"
  >
    <template #control>
      <q-editor
        v-model="model"
        min-height="12rem"
        @paste="onPaste"
        :toolbar="[
          ['bold', 'italic', 'strike', 'underline'],
          ['quote', 'unordered', 'ordered'],
          [
            {
              label: $q.lang.editor.fontSize,
              icon: $q.iconSet.editor.fontSize,
              fixedLabel: true,
              fixedIcon: true,
              list: 'no-icons',
              options: [
                'size-1',
                'size-2',
                'size-3',
                'size-4',
                'size-5',
                'size-6',
              ],
            },
            {
              label: $q.lang.editor.defaultFont,
              icon: $q.iconSet.editor.font,
              fixedIcon: true,
              list: 'no-icons',
              options: [
                'default_font',
                'arial',
                'arial_black',
                'comic_sans',
                'courier_new',
                'impact',
                'lucida_grande',
                'times_new_roman',
                'verdana',
              ],
            },
          ],
          ['undo', 'redo'],
        ]"
        :fonts="{
          arial: 'Arial',
          arial_black: 'Arial Black',
          comic_sans: 'Comic Sans MS',
          courier_new: 'Courier New',
          impact: 'Impact',
          lucida_grande: 'Lucida Grande',
          times_new_roman: 'Times New Roman',
          verdana: 'Verdana',
        }"
        style="width: 100%; margin-bottom: -8px"
        class="q-pt-none"
        :style="
          fieldRef && fieldRef.hasError ? 'border-color: var(--q-negative)' : ''
        "
      />
    </template>
  </q-field>
</template>

<script setup lang="ts">
import { QEditor, QField } from 'quasar';
import { ref } from 'vue';

const model = defineModel<string>({
  default: '',
});
const editorRef = ref<QEditor | null>(null);
const fieldRef = ref<QField | null>(null);

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
