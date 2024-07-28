import { createAuth0 } from '@auth0/auth0-vue';
import { boot } from 'quasar/wrappers';

const authClient = createAuth0({
  domain: process.env.QUASAR_AUTH0_DOMAIN || '',
  clientId: process.env.QUASAR_AUTH0_CLIENT_ID || '',
  authorizationParams: {
    redirect_uri: window.location.origin,
    audience: process.env.QUASAR_AUTH0_AUDIENCE || '',
  },
});
// "async" is optional;
// more info on params: https://v2.quasar.dev/quasar-cli/boot-files
export default boot(({ app, router }) => {
  app.config.globalProperties.$router = router;
  app.use(authClient);
});

export const getAccessToken = async () => {
  const accessToken = await authClient.getAccessTokenSilently();
  return accessToken;
};
