<template>
  <v-container fluid>
    <PageTopSection :items="breadcrumbs" title="Your account"/>

    <v-container v-if="loading" class="py-3 align-center text-center">
      <v-progress-circular color="primary" indeterminate="true"/>
      <div class="mt-2 text-caption">Loading user information</div>
    </v-container>

    <v-form v-else @submit.prevent ref="editUserDataForm">
      <v-container class="py-0" fluid>
        <v-container class="pa-0" fluid>
          <v-row class="ga-0" no-gutters>
            <v-col cols="12" sm="12" md="6">
              <div class="text-h6 mb-2 text-secondary">Account details</div>
              <v-row no-gutters class="ga-3">
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="userValidator.titleRules"
                                v-model="userData.title" color="secondary" label="Title" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-row no-gutters class="mb-0">
                    <v-col cols="6" class="pe-sm-0 pe-md-8">
                      <v-text-field :rules="userValidator.givenNameRules"
                                    v-model="userData.givenName" color="secondary" label="Given name" type="input"
                                    variant="outlined" :disabled="userData.isFederatedUser"/>
                    </v-col>
                    <v-col cols="6">
                      <v-text-field :rules="userValidator.familyNameRules"
                                    v-model="userData.familyName" color="secondary" label="Family name" type="input"
                                    variant="outlined" :disabled="userData.isFederatedUser"/>
                    </v-col>
                  </v-row>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-combobox :rules="userValidator.divisionRules"
                              :items="Object.values(Division)"
                              :model-value="Division[userData.division]"
                              @update:model-value="item => userData.division=parseInt(Object.keys(Division).find(key => Division[key] === item))"
                              variant="outlined" color="secondary" label="Division"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="userValidator.nicknameRules"
                                v-model="userData.nickname" color="secondary" label="Nickname" type="input"
                                variant="outlined" :disabled="userData.isFederatedUser"/>
                </v-col>
                <v-col cols="12" class="pe-sm-0 pe-md-8">
                  <v-text-field :rules="userValidator.emailRules"
                                v-model="userData.emailAddress" color="secondary" label="Email" type="input"
                                variant="outlined" :disabled="userData.isFederatedUser"/>
                </v-col>
              </v-row>
            </v-col>

            <v-col cols="12" sm="12" md="6">
              <div class="text-h6 mb-2 text-secondary">Address details</div>
              <v-row no-gutters class="ga-3">
                <v-col cols="12">
                  <v-text-field :rules="addressValidator.addressLine1Rules"
                                color="secondary" v-model="userData.address.addressLine1" label="Address Line 1"
                                type="input" variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-text-field color="secondary" v-model="userData.address.addressLine2" label="Address Line 2"
                                type="input" variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-text-field :rules="addressValidator.postcodeRules"
                                color="secondary" v-model="userData.address.postcode" label="Postcode" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-text-field :rules="addressValidator.cityRules"
                                color="secondary" v-model="userData.address.city" label="City" type="input"
                                variant="outlined"/>
                </v-col>
                <v-col cols="12">
                  <v-combobox
                    :items="Object.values(Country)" :rules="addressValidator.countryRules"
                    :model-value="Country[userData.address.country]"
                    @update:model-value="item => userData.address.country=Object.keys(Country).find(key => Country[key] === item)"
                    variant="outlined" color="secondary" label="Country"/>
                </v-col>
              </v-row>
              <v-container class="d-flex justify-end ga-1 pe-sm-0" fluid>
                <v-btn @click="submit" type="submit" color="primary">Update</v-btn>
              </v-container>
            </v-col>

          </v-row>

        </v-container>
      </v-container>
    </v-form>

  </v-container>

</template>

<script setup>
import PageTopSection from "@/components/TopSection.vue";
import {onBeforeMount, ref} from "vue";
import {useAuth0} from "@auth0/auth0-vue";
import UserData from "@/models/userData";
import AddressDataValidator from "@/validators/addressDataValidator";
import UserDataValidator from "@/validators/userDataValidator";
import Country from "@/models/country";
import Division from "@/models/division";
import Endpoints from "@/models/endpoints";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import router from "@/router";

const breadcrumbs = [
  {
    title: 'Home',
    disabled: false,
    to: '/'
  },
  {
    title: 'Account',
    disabled: true,
    to: null
  },
]
const {user, getAccessTokenSilently} = useAuth0();
const addressValidator = ref(new AddressDataValidator());
const userValidator = ref(new UserDataValidator());
const snackbar = useGlobalSnackbar();

let editUserDataForm = ref(null);
let loading = ref(false);
let userData = ref(new UserData());

onBeforeMount(() => {
  loadUserModel();
})

async function loadUserModel() {
  loading.value = true;
  userData.value.buildAuth0AccountInfo(user.value);

  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}GetUser`] + '/' + userData.value.id;
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "GET",
    headers: {Authorization: 'Bearer ' + token}
  });

  switch (response.status) {
    case 200: {
      let data = await response.json();
      userData.value.buildUserInfo(data.data);
      break;
    }
    default:
      break;
  }

  loading.value = false;
}

async function submit() {
  const isFormValid = await editUserDataForm.value.validate();

  if (isFormValid.valid) {
    const apiEndpoint = Endpoints[`${process.env.NODE_ENV}UpdateUser`] + '/' + userData.value.id;
    const token = await getAccessTokenSilently();

    const response = await fetch(apiEndpoint, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: 'Bearer ' + token
      },
      body: JSON.stringify(userData.value)
    });

    switch (response.status) {
      case 200:
        await snackbar.showSnackbar("mdi-hand-okay", "Account updated successfully!", "success");
        await router.push({path: '/'});
        break;
      default:
        snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while updating your account.", "error");
        break;
    }
  }
}

</script>
