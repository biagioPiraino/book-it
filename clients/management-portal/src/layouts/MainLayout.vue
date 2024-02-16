<template>
  <v-app>
    <v-layout>
      <v-navigation-drawer
        v-model="drawer"
        :rail="rail"
        border="right 0"
        style="background: linear-gradient(white,lightskyblue)"
        @click="rail = false"
      >
        <v-list>
          <v-list-item
            :prepend-avatar="user.picture"
            :title="user.name"
          >
            <template v-slot:append>
              <v-btn
                variant="text"
                icon="mdi-chevron-left"
                @click.stop="rail = !rail"
              ></v-btn>
            </template>
          </v-list-item>
        </v-list>

        <v-list density="compact" nav>
          <v-list-item @click="goToHome" prepend-icon="mdi-view-dashboard" title="Home" value="home"/>
          <v-list-item @click="goToDesks" prepend-icon="mdi-desk" title="Desks" value="desks"/>
        </v-list>

        <template v-slot:append>
          <v-list density="compact" nav>
            <v-list-item @click="logoutFromPortal" prepend-icon="mdi-logout" title="Log out" value="logout"/>
          </v-list>
        </template>

      </v-navigation-drawer>
      <v-main>
        <v-snackbar
          location="top right"
          variant="tonal"
          :color="snackbar.color"
          v-model="snackbar.show"
          :timeout="2000"
        >
          <v-icon class="mb-1 me-1" :icon="snackbar.icon"/>
          {{ snackbar.message }}
        </v-snackbar>
        <router-view/>
      </v-main>
    </v-layout>
  </v-app>
</template>

<script setup>
import {ref} from "vue";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import {useAuth0} from "@auth0/auth0-vue";
import router from "@/router";

const {logout, user} = useAuth0();
const snackbar = ref(useGlobalSnackbar());
let drawer = ref(true)
let rail = ref(true)

function goToHome() {
  router.push({path: "/"})
}

function goToDesks() {
  router.push({path: "/desks"})
}

function logoutFromPortal() {
  logout({logoutParams: {returnTo: window.location.origin}});
}
</script>
