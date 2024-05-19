import { createAuth0 } from '@auth0/auth0-vue';
import { boot } from 'quasar/wrappers';

// "async" is optional;
// more info on params: https://v2.quasar.dev/quasar-cli/boot-files
export default boot(({ app }) => {
  app.use(
    createAuth0({
      domain: 'tungdev.au.auth0.com',
      clientId: '4I0IrD6iHzSWdQF0QUb6SMIYdlxDTb3f',
      authorizationParams: {
        redirect_uri: window.location.origin,
        audience: 'https://dev-recipe-share.com',
      },
    })
  );
});
