import { defineConfig } from 'cypress';
import webpackPreprocessor from '@cypress/webpack-preprocessor';
import { resolve as _resolve } from 'path';
import { retrieveContentfulData } from './cypress/helpers/retrieve-contentful-data';

const { defaultOptions } = webpackPreprocessor;


Object.assign(defaultOptions.webpackOptions, {
  resolve: {
    ...defaultOptions.webpackOptions.resolve,
  },
});


export default defineConfig({
  chromeWebSecurity: false,
  video: true,
  reporter: "cypress-multi-reporters",
  reporterOptions: {
    "configFile": "reporter-config.json"
  },
  retries: {
    runMode: 1
  },
  e2e: {
    setupNodeEvents(on, _) {
      on('task', {
        log(message) {
          console.log(message);

          return null;
        },
        table(message) {
          console.table(message);

          return null;
        },
        fetchContentfulData() {
          return retrieveContentfulData();
        }
      }),
        on('file:preprocessor', webpackPreprocessor(defaultOptions));
    },
  },
});
