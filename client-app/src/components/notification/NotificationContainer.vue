<template>
  <q-btn dense round flat icon="notifications">
    <q-badge v-if="items.length > 0" color="red" floating>
      {{ items.length }}
    </q-badge>
    <q-menu @show="markAsSeen">
      <q-list style="min-width: 100px">
        <q-item
          v-for="item in notificationPage?.items"
          :key="item.id"
          clickable
          v-close-popup
          @click="() => navigateTo(item)"
        >
          <q-item-section
            >{{ item.fromUserDisplayName }}
            {{ getAction(item) }}</q-item-section
          >
        </q-item>
      </q-list>
    </q-menu>
  </q-btn>
</template>

<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { useQuasar } from 'quasar';
import { notificationApi } from 'src/boot/axios';
import { IPageResult } from 'src/interfaces/Common';
import { INotification } from 'src/interfaces/Notification';
import { onMounted, onUnmounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const auth0 = useAuth0();
const $q = useQuasar();
const router = useRouter();
const notificationPage = ref<IPageResult<INotification> | null>(null);
const items = ref<INotification[]>([]);

let timer = setInterval(() => {
  getNotifications(1);
}, 15000);

async function getNotifications(pageNumber: number, shouldNotify = true) {
  if (!auth0.isAuthenticated) {
    return;
  }
  const accessToken = await auth0.getAccessTokenSilently();
  let url = `notifications?pageNumber=${pageNumber}&pageSize=10`;
  if (items.value.length > 0) {
    const item = items.value[0];
    url += `&lastId=${item.id}`;
  }
  const response = await notificationApi.get<IPageResult<INotification>>(url, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
  if (response.status === 200) {
    notificationPage.value = response.data;
    items.value = [...response.data.items, ...items.value];
    if (shouldNotify) {
      notifyLastActivity();
    }
  }
}

async function markAsSeen() {
  const accessToken = await auth0.getAccessTokenSilently();
  if (!accessToken) {
    return;
  }
  const ids = notificationPage.value?.items?.map((x) => x.id) ?? [];
  if (ids.length === 0) {
    return;
  }
  const response = await notificationApi.post(
    'notifications/mark-as-seen',
    ids,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );
  if (response.status === 200) {
    items.value = [];
  }
}

function getAction(noti: INotification) {
  switch (noti.type?.toLocaleLowerCase()) {
    case 'comment':
      return 'commented on your recipe';
    case 'like':
      return 'liked your recipe';
    case 'reply':
      return 'replied your comment';
  }
}

function navigateTo(noti: INotification) {
  router.push({
    path: `/recipes/${noti.recipeId}`,
    // todo: navigate exactly to the comment/reply
  });
}

function notifyLastActivity() {
  if (!notificationPage.value) {
    return;
  }
  const lastNotification = notificationPage.value.items[0];
  if (lastNotification) {
    $q.notify({
      message: `<strong>${
        lastNotification.fromUserDisplayName
      }</strong> ${getAction(lastNotification)}`,
      html: true,
      position: 'top-right',
      actions: [
        {
          icon: 'close',
          color: 'white',
          round: true,
          handler: () => {
            /* ... */
          },
        },
      ],
    });
  }
}

onMounted(() => {
  getNotifications(1, false);
});

onUnmounted(() => {
  if (timer) {
    clearInterval(timer);
  }
});
</script>
