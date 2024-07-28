import { defineStore } from 'pinia';
import { ref } from 'vue';
import { INotification } from 'src/interfaces/Notification';

export const useNotificationsStore = defineStore('notifications', () => {
  const notfications = ref<INotification[]>([]);

  function add(notification: INotification) {
    notfications.value.unshift(notification);
  }

  return {
    notfications,
    add,
  };
});
