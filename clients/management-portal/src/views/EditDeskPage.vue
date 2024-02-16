<template>
  <v-container fluid>
    <PageTopSection
      :items="breadcrumbs"
      :title="`Editing ${deskStore.deskName}`"
      :create-exits="false"
      :delete-exits="true"
      @delete="showDeleteDialog = true"
    />

    <v-container v-if="loading" class="py-3 align-center text-center">
      <v-progress-circular color="primary" indeterminate="true"/>
      <div class="mt-2 text-caption">Loading {{ deskStore.deskName }}</div>
    </v-container>

    <v-form v-else @submit.prevent ref="editDeskForm">
      <v-container class="py-0" fluid>
        <v-container class="pa-0" fluid>
          <v-row class="ga-0" no-gutters>
            <v-col cols="12" sm="12" md="6">
              <div class="text-h6 mb-2 text-secondary">Desk details</div>
              <v-row no-gutters class="ga-3">
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="deskValidator.unitRules"
                                v-model="desk.unit" color="secondary" label="Unit" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="deskValidator.buildingRules"
                                v-model="desk.building" color="secondary" label="Building" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="deskValidator.floorRules"
                                v-model="desk.floor" color="secondary" label="Floor" type="number"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="deskValidator.screenRules"
                                v-model="desk.availableScreens" color="secondary" label="Available Screens"
                                type="number" variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-combobox
                    :items="Object.values(DeskType)" :rules="deskValidator.typeRules"
                    :model-value="DeskType[desk.deskType]"
                    @update:model-value="item => desk.deskType=parseInt(Object.keys(DeskType).find(key => DeskType[key] === item))"
                    variant="outlined" color="secondary" label="Type"/>
                </v-col>
                <div class="text-h6 mb-2 text-secondary">Address details</div>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="addressValidator.addressLine1Rules"
                                color="secondary" v-model="desk.address.addressLine1" label="Address Line 1"
                                type="input" variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field color="secondary" v-model="desk.address.addressLine2" label="Address Line 2"
                                type="input" variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="addressValidator.postcodeRules"
                                color="secondary" v-model="desk.address.postcode" label="Postcode" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="addressValidator.cityRules"
                                color="secondary" v-model="desk.address.city" label="City" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-combobox
                    :items="Object.values(Country)" :rules="addressValidator.countryRules"
                    :model-value="Country[desk.address.country]"
                    @update:model-value="item => desk.address.country=Object.keys(Country).find(key => Country[key] === item)"
                    variant="outlined" color="secondary" label="Country"/>
                </v-col>
              </v-row>
              <v-container class="d-flex justify-end ga-1 pe-sm-0 pe-md-8" fluid>
                <v-btn :to="'/desks'" variant="outlined" color="error">Cancel</v-btn>
                <v-btn @click="submit" type="submit" color="primary">Submit</v-btn>
              </v-container>
            </v-col>

            <v-col cols="12" sm="12" md="6">
              <div class="text-h6 text-secondary">Desk slots</div>
              <v-list lines="three" :key="renderSlots">
                <v-list-item
                  v-for="(item, i) in desk.slots"
                  :key="i"
                  :value="item"
                  color="primary"
                  rounded="shaped">

                  <template v-slot:prepend>
                    <v-icon v-if="item.isBooked" color="error" icon="mdi-calendar-check"/>
                    <v-icon v-else-if="item.isAvailable" color="success" icon="mdi-calendar-check"/>
                    <v-icon v-else color="error" icon="mdi-cancel"/>
                  </template>

                  <v-list-item-title v-text="(new Date(item.day)).toDateString()"/>
                  <v-list-item-subtitle v-text="`Starts at: ${(new Date(item.startingTime)).toLocaleTimeString()}`"/>
                  <v-list-item-subtitle v-text="`End at: ${(new Date(item.endingTime)).toLocaleTimeString()}`"/>

                  <template v-slot:append>
                    <v-btn
                      v-if="item.isAvailable"
                      color="error"
                      prepend-icon="mdi-cancel"
                      rounded
                      height="40"
                      variant="tonal"
                      @click="updateAvailability(item.id, false)">
                      Cancel Availability
                    </v-btn>
                    <v-btn
                      v-else
                      color="success"
                      prepend-icon="mdi-gate-open"
                      rounded
                      height="40"
                      variant="tonal"
                      @click="updateAvailability(item.id, true)">
                      Open Availability
                    </v-btn>
                  </template>
                </v-list-item>
              </v-list>
            </v-col>
          </v-row>

        </v-container>
      </v-container>
    </v-form>

    <v-dialog
      transition="dialog-top-transition"
      v-model="showDeleteDialog"
      width="auto">
      <v-card rounded elevation="10" class="py-2 px-1">
        <v-card-title>Deleting a desk, are you sure?</v-card-title>
        <v-divider color="error" thickness="3" class="mt-2"/>
        <v-card-text class="px-4">
          Be aware that deleting cannot be undone. Submit to proceed.
        </v-card-text>

        <v-card-actions>
          <v-btn
            color="error"
            variant="text"
            @click="showDeleteDialog = false">
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            variant="text"
            @click="deleteDesk">
            Submit
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  </v-container>
</template>

<script setup>
import PageTopSection from "@/components/TopSection.vue";
import DeskType from "@/models/deskType";
import Country from "@/models/country";
import DeskData from "@/models/deskData";
import {onBeforeMount, ref} from "vue";
import Endpoints from "@/models/endpoints";
import router from "@/router";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import {useDeskStore} from "@/store/deskStore";
import DeskDataValidator from "@/validators/deskDataValidator";
import AddressDataValidator from "@/validators/addressDataValidator";
import {useAuth0} from "@auth0/auth0-vue";

const { getAccessTokenSilently } = useAuth0();
const snackbar = useGlobalSnackbar();
const deskStore = useDeskStore();

const editDeskForm = ref(null);
const desk = ref(new DeskData());
const deskValidator = ref(new DeskDataValidator());
const addressValidator = ref(new AddressDataValidator());
let renderSlots = ref(0);

const breadcrumbs = [
  {
    title: 'Home',
    disabled: false,
    to: '/',
  },
  {
    title: 'Desks',
    disabled: false,
    to: '/desks',
  },
  {
    title: 'Edit',
    disabled: true,
    to: null,
  },
]

let loading = ref(false);
let showDeleteDialog = ref(false);

onBeforeMount(() => {
  loadDesk();
})

async function loadDesk() {
  loading.value = true;

  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}GetDesk`] + '/' + deskStore.deskId;
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "GET",
    headers: {Authorization: 'Bearer ' + token}
  });

  switch (response.status) {
    case 200:
      let data = await response.json();
      desk.value = data.data;
      break;
    default:
      break;
  }

  loading.value = false;
}

async function submit() {
  const isFormValid = await editDeskForm.value.validate();

  if (isFormValid.valid) {
    const apiEndpoint = Endpoints[`${process.env.NODE_ENV}UpdateDesk`] + '/' + deskStore.deskId;
    const token = await getAccessTokenSilently();

    const response = await fetch(apiEndpoint, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: 'Bearer ' + token
      },
      body: JSON.stringify(desk.value)
    });

    switch (response.status) {
      case 200:
        await snackbar.showSnackbar("mdi-hand-okay", "Desk updated successfully!", "success");
        deskStore.clear()
        await router.push({path: '/desks'});
        break;
      default:
        snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while updating the desk.", "error");
        break;
    }
  }
}

async function deleteDesk() {
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}DeleteDesk`] + '/' + deskStore.deskId;
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "DELETE",
    headers: {Authorization: 'Bearer ' + token}
  });

  switch (response.status)
  {
    case 204:
      snackbar.showSnackbar("mdi-hand-okay", "Desk deleted successfully!", "success");
      deskStore.clear();
      await router.push({path: "/desks"})
      break;
    default:
      snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while deleting the desk.", "error");
      break;
  }
}

async function updateAvailability(slotId, status) {
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}UpdateDeskAvailability`] + '/' + slotId + '/' + status;
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: 'Bearer ' + token
    },
    body: JSON.stringify(desk.value)
  });

  switch (response.status) {
    case 200:
      await snackbar.showSnackbar("mdi-hand-okay", "Desk availability updated successfully!", "success");
      let data = await response.json();
      desk.value.slots = data.data.slots
      break;
    default:
      snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while updating the desk.", "error");
      break;
  }
}

</script>
