<script setup>
import PageTopSection from "@/components/TopSection.vue";

let items = [{
  title: 'Home',
  disabled: true,
  to: null,
}]
</script>

<template>
  <v-container class="fill-height mx-3">

    <PageTopSection :items="items" title="Welcome"/>

    <v-responsive class="fill-height mx-3">
      <div class="text-h6">
        Welcome to the booking system portal.
      </div>
      <div class="text-body-1">
        To start managing your bookings please visit the <router-link to="/bookings">booking</router-link> section.
      </div>
    </v-responsive>
  </v-container>
</template>

<style scoped>

</style>
