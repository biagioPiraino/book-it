<template>
  <v-container class="fill-height">
    <v-responsive class="align-center text-center fill-height">
      <v-row class="d-flex align-center justify-center" no-gutters>
        <v-col cols="12" xs="12">
          <v-img height="300" src="@/assets/desk-logo.png" />
        </v-col>
        <v-col cols="12" xs="12">
          <div class="text-body-2 font-weight-light mb-n1">Welcome to</div>
          <div class="text-h2 font-weight-bold pt-3">Desk Portal</div>
          <div class="text-caption py-3">Please login to access</div>
          <v-btn color="primary" size="large" @click="login" variant="elevated">
            <v-icon icon="mdi-login" size="medium" start/>
            Log in
          </v-btn>
        </v-col>
      </v-row>

    </v-responsive>
  </v-container>
</template>

<script setup>
import {useAuth0} from "@auth0/auth0-vue";
const {loginWithRedirect} = useAuth0();

function login() {
  loginWithRedirect();
}
</script>
