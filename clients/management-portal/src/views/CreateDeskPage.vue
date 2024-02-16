<template>
  <v-container fluid>
    <PageTopSection :items="breadcrumbs" title="Create a new desk" :create-exits="false"/>

    <v-form @submit.prevent ref="createDeskForm">
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
                    @update:model-value="item => desk.deskType=parseInt(Object.keys(DeskType).find(key => DeskType[key] === item))"
                    variant="outlined" color="secondary" label="Type"/>
                </v-col>
              </v-row>
            </v-col>

            <v-col cols="12" sm="12" md="6">
              <div class="text-h6 mb-2 text-secondary">Address details</div>
              <v-row no-gutters class="ga-3">
                <v-col cols="12">
                  <v-text-field :rules="addressValidator.addressLine1Rules"
                                color="secondary" v-model="desk.address.addressLine1" label="Address Line 1"
                                type="input" variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-text-field color="secondary" v-model="addressValidator.addressLine2" label="Address Line 2"
                                type="input" variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-text-field :rules="addressValidator.postcodeRules"
                                color="secondary" v-model="desk.address.postcode" label="Postcode" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-text-field :rules="addressValidator.cityRules"
                                color="secondary" v-model="desk.address.city" label="City" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-combobox
                    :items="Object.values(Country)" :rules="addressValidator.countryRules"
                    @update:model-value="item => desk.address.country=Object.keys(Country).find(key => Country[key] === item)"
                    variant="outlined" color="secondary" label="Country"/>
                </v-col>
              </v-row>
            </v-col>
          </v-row>
        </v-container>
      </v-container>

      <v-container class="d-flex justify-end ga-1" fluid>
        <v-btn :to="'/desks'" variant="outlined" color="error">Cancel</v-btn>
        <v-btn @click="submit" type="submit" color="primary">Submit</v-btn>
      </v-container>
    </v-form>

  </v-container>
</template>

<script setup>
import PageTopSection from "@/components/TopSection.vue";
import DeskType from "@/models/deskType";
import Country from "@/models/country";
import DeskData from "@/models/deskData";
import {ref} from "vue";
import Endpoints from "@/models/endpoints";
import router from "@/router";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import DeskDataValidator from "@/validators/deskDataValidator";
import AddressDataValidator from "@/validators/addressDataValidator";
import {useAuth0} from "@auth0/auth0-vue";

const { getAccessTokenSilently } = useAuth0();
const desk = ref(new DeskData());
const createDeskForm = ref(null);
const snackbar = useGlobalSnackbar();
const deskValidator = ref(new DeskDataValidator());
const addressValidator = ref(new AddressDataValidator());

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
    title: 'Create',
    disabled: true,
    to: null,
  },
]

async function submit() {
  const isFormValid = await createDeskForm.value.validate();
  if (isFormValid.valid) {
    const apiEndpoint = Endpoints[`${process.env.NODE_ENV}CreateDesk`]
    const token = await getAccessTokenSilently();

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: 'Bearer ' + token
      },
      body: JSON.stringify(desk.value)
    });

    switch (response.status) {
      case 201:
        await snackbar.showSnackbar("mdi-hand-okay", "Desk created successfully!", "success");
        await router.push({path: '/desks'});
        break;
      default:
        snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while creating the desk.", "error");
        break;
    }
  }
}

</script>
