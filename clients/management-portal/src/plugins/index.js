/**
 * plugins/index.js
 *
 * Automatically included in `./src/main.js`
 */

// Plugins
import vuetify from './vuetify'
import pinia from '../store'
import router from '../router'
import piniaPluginPersistedState from "pinia-plugin-persistedstate"
import {createAuth0} from "@auth0/auth0-vue";

export function registerPlugins(app) {
  app
    .use(vuetify)
    .use(router)
    .use(pinia.use(piniaPluginPersistedState))
    .use(
      createAuth0({
        domain: "",
        clientId: "",
        authorizationParams:{
          redirect_uri: window.location.origin,
          audience: "", // api server identifier
        }
      })
    )
}
