<template>
  <v-container fluid>

    <PageTopSection :items="breadcrumbs" title="Manage your desks" :create-exits="true" entity-to-create="Desk" context="desks"/>

    <v-container fluid>
      <v-data-table-server
        v-model:items-per-page="ViewSettings.ItemsPerPage"
        :key="renderTable"
        :items-length="totalLength"
        :headers="tableHeaders"
        :loading="loading"
        :items="tableItems"
        :search="search"
        item-value="deskId"
        :loading-text="ViewSettings.LoadingTableMessage"
        @update:options="loadTableItems"
        @update:items-per-page="loadTableItems">

        <template v-slot:item.address="{item}">
          <div>
            {{
              item.address.addressLine1 + ', ' +
              item.address.postcode + ', ' +
              item.address.city + ', ' +
              item.address.country
            }}
          </div>
        </template>

        <template v-slot:item.deskType="{item}">
          <div>{{ DeskType[item.deskType] }}</div>
        </template>

        <template v-slot:item.manage="{item}">
          <v-item-group
            class="d-flex justify-lg-space-evenly pt-3 pb-3"
          >
            <v-btn
              color="error"
              icon="mdi-delete-circle-outline"
              border
              height="40"
              variant="text"
              width="40"
              @click="showDeleteDialog = true"
            />

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
                    @click="() => deleteDesk(item.deskId)">
                    Submit
                  </v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>

            <v-btn
              color="primary"
              icon="mdi-circle-edit-outline"
              border
              height="40"
              variant="text"
              width="40"
              @click="() => navigateToEdit(item.deskId, item.unit)"
            />
          </v-item-group>
        </template>
      </v-data-table-server>
    </v-container>
  </v-container>
</template>

<script setup>
import {ref} from "vue";
import PageTopSection from "@/components/TopSection.vue";
import DeskType from "../models/deskType";
import ViewSettings from "@/models/viewSettings";
import Endpoints from "@/models/endpoints";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import router from "@/router";
import {useDeskStore} from "@/store/deskStore";
import {useAuth0} from "@auth0/auth0-vue";

const { getAccessTokenSilently } = useAuth0();
const tableHeaders = [
  {title: 'Unit', key: 'unit', align: 'start', sortable: false},
  {title: 'Address', key: 'address', align: 'start', sortable: false},
  {title: 'Building', key: 'building', align: 'start', sortable: true},
  {title: 'Floor', key: 'floor', align: 'start', sortable: false},
  {title: 'Type', key: 'deskType', align: 'start', sortable: true},
  {title: 'Screens', key: 'availableScreens', align: 'start', sortable: true},
  {title: 'Manage', key: 'manage', align: 'center', sortable: false}
];

const snackbar = useGlobalSnackbar();
const deskStore = useDeskStore();
const breadcrumbs = [
  {
    title: 'Home',
    disabled: false,
    to: '/',
  },
  {
    title: 'Desks',
    disabled: true,
    to: null,
  },
]

let loading = ref(true);
let tableItems = ref([]);
let totalLength = ref(0);
let search = ref('');
let showDeleteDialog = ref(false);
let renderTable = ref(0);

async function loadTableItems({page, itemsPerPage, sortBy}) {
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}GetDesks`];
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "GET",
    headers: {Authorization: 'Bearer ' + token}
  });

  switch (response.status)
  {
    case 200:
      let data = await response.json();
      totalLength.value = data.dataCount;

      const start = (page - 1) * itemsPerPage;
      const end = start + itemsPerPage;
      const items = data.data.slice();

      if (sortBy.length) {
        const sortKey = sortBy[0].key;
        const sortOrder = sortBy[0].order;
        items.sort((a, b) => {
          const aValue = a[sortKey];
          const bValue = b[sortKey];
          return sortOrder === 'desc' ? bValue - aValue : aValue - bValue;
        })
      }

      tableItems.value = items.slice(start, end);
      loading.value = false;
      break;
    case 204:
      tableItems.value = [];
      totalLength.value = 0;
      loading.value = false;
      break;
  }
}

async function deleteDesk(deskId) {
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}DeleteDesk`] + '/' + deskId;
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "DELETE",
    headers: {Authorization: 'Bearer ' + token}
  });

  switch (response.status)
  {
    case 204:
      snackbar.showSnackbar("mdi-hand-okay", "Desk deleted successfully!", "success");
      tableStateHasChanged();
      break;
    default:
      snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while deleting the desk.", "error");
      break;
  }

  showDeleteDialog.value = false;
}

async function navigateToEdit(deskId, deskName){
  deskStore.populateStore(deskId, deskName);
  await router.push({path: '/desks/edit'});
}

function tableStateHasChanged() {
  renderTable.value += 1;
}

</script>
