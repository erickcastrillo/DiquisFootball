import * as React from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import router from 'router';
import 'simplebar/dist/simplebar.min.css';
import 'nprogress/nprogress.css';
import 'assets/scss/icons.scss';

import 'assets/scss/nanite-theme.scss';

import 'react-toastify/dist/ReactToastify.css';
import 'core-js/stable';
import 'regenerator-runtime/runtime';

import jQuery from 'jquery'; // some plugins still require jQuery -- range-slider, datepicker, dragula.
// @ts-ignore
window.$ = window.jQuery = jQuery;

import { I18nextProvider } from 'react-i18next';
import i18n from './i18n'; // Import your i18n configuration

const container = document.getElementById('root');
const root = createRoot(container!);

root.render(
  <React.Fragment>
    <I18nextProvider i18n={i18n}>
      <RouterProvider router={router} />
    </I18nextProvider>
  </React.Fragment>
);
