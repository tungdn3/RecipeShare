import { boot } from 'quasar/wrappers';
import * as signalR from '@microsoft/signalr';
import { getAccessToken } from './auth';
import { useNotificationsStore } from 'src/stores/notifications-store';
import { INotification } from 'src/interfaces/Notification';

const gatewayBaseUrl = process.env.QUASAR_GATEWAY_BASE_URL ?? '';
const hubUrl = `${gatewayBaseUrl}/notification/hub`;

export default boot(({ app }) => {
  const notificationStore = useNotificationsStore();
  const notificationHub = () => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: getAccessToken,
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    async function start() {
      try {
        await connection.start();
        console.log('SignalR Connected.');
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    }

    connection.onclose(async () => {
      await start();
    });

    connection.on('ReceiveMessage', function (message: INotification) {
      notificationStore.add(message);
    });

    // Start the connection.
    start();
  };

  app.use(notificationHub);
});
