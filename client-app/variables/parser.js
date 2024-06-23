const dotenv = require('dotenv');
const systemEnvs = {};

Object.keys(process.env).forEach((key) => {
  if (key.startsWith('QUASAR')) {
    systemEnvs[key] = process.env[key];
  }
});

const files = {
  ...dotenv.config({ path: 'variables/.env' }).parsed,
  ...dotenv.config({ path: `variables/.env.${process.env.ENVIRONMENT}` })
    .parsed,
  ...systemEnvs,
};

module.exports = () => {
  Object.keys(files, (key) => {
    if (typeof files[key] !== 'string') {
      files[key] = JSON.stringify(files[key]);
    }
  });
  return files;
};
