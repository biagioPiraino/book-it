<template>
  <v-container class="ma-0 pa-0">
    <v-breadcrumbs :items="items">
      <template v-slot:divider>
        <v-icon icon="mdi-chevron-right"/>
      </template>
    </v-breadcrumbs>
  </v-container>

  <v-container fluid class="mx-3 mb-3 pa-0 d-flex align-center justify-space-between">
    <div class="text-h3">{{ title }}</div>
    <v-btn rounded="xl" v-if="createExits" class="me-6" color="primary" prepend-icon="mdi-plus" :to="`/${context}/create`">
      New {{entityToCreate}}
    </v-btn>
    <v-btn rounded="xl" v-if="deleteExits" class="me-6" color="error" prepend-icon="mdi-delete" @click="deleteEntity">
      Delete
    </v-btn>
  </v-container>
</template>

<script setup>
defineProps({
  items: Array,
  title: String,
  createExits: Boolean,
  deleteExits: Boolean,
  entityToCreate: String,
  context: String
})

const emit = defineEmits(  ['delete'])

function deleteEntity(){
  emit("delete");
}
</script>
