// @ts-ignore - zone.js is required by Angular but may not expose type declarations in this setup
import 'zone.js';

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
bootstrapApplication(App, appConfig).catch((err) => console.error(err));
