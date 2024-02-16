<template>
  <loading-auth v-if="isLoading"/>
  <v-container v-else class="pa-0 ma-0" fluid>
    <router-view v-if="isAuthenticated" />
    <login-page v-else/>
  </v-container>
</template>

<script setup>
import LoginPage from "@/views/LoginPage.vue";
import {useAuth0} from "@auth0/auth0-vue";
import LoadingAuth from "@/components/LoadingAuth.vue";

const { isAuthenticated, isLoading } = useAuth0();

</script>
