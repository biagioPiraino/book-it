<template>
  <v-container fluid>

    <PageTopSection :items="breadcrumbs" title="Manage your bookings"/>

    <v-container fluid>
      <v-data-table-server
        items-per-page="10"
        :key="renderTable"
        :items-length="totalLength"
        :headers="tableHeaders"
        :loading="loading"
        :items="tableItems"
        item-value="deskId"
        loading-text="Loading bookings..."
        show-expand
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


        <template v-slot:item.slots="{item}">
          <v-chip color="primary" variant="outlined" size="small">{{ item.slots.length }} booked slots</v-chip>
        </template>

        <template v-slot:expanded-row="{ columns, item }">
          <tr :key="slot.slotId" v-for="slot in item.slots">
            <td class="ps-10" :colspan="2">
              Booking # {{ slot.id }}, booked for
              <span class="font-weight-bold">
                {{ getFullDescriptiveDate(slot.day, slot.startingTime, slot.endingTime) }}
              </span>
            </td>
            <td class="" :colspan="columns.length">
              <v-btn color="error" size="small" variant="outlined" @click="cancelBooking(item.deskId, slot.id)" >Cancel booking</v-btn>
            </td>
          </tr>

        </template>

      </v-data-table-server>
    </v-container>
  </v-container>
</template>

<script setup>
import {ref} from "vue";
import PageTopSection from "@/components/TopSection.vue";
import DeskType from "../models/deskType";
import Endpoints from "@/models/endpoints";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import {useAuth0} from "@auth0/auth0-vue";
import DayOfWeekFull from "@/models/dayOfWeekFull";
import Months from "@/models/months";

const {user, getAccessTokenSilently} = useAuth0();
const tableHeaders = [
  {title: 'Unit', key: 'unit', align: 'start', sortable: false},
  {title: 'Address', key: 'address', align: 'start', sortable: false},
  {title: 'Building', key: 'building', align: 'start', sortable: true},
  {title: 'Floor', key: 'floor', align: 'start', sortable: false},
  {title: 'Type', key: 'deskType', align: 'start', sortable: true},
  {title: 'Screens', key: 'availableScreens', align: 'start', sortable: true},
  {title: 'Slots', key: 'slots', align: 'center', sortable: false}
];

const snackbar = useGlobalSnackbar();
const breadcrumbs = [
  {
    title: 'Home',
    disabled: false,
    to: '/',
  },
  {
    title: 'Bookings',
    disabled: true,
    to: null,
  },
]

let loading = ref(true);
let tableItems = ref([]);
let totalLength = ref(0);
let renderTable = ref(0);

async function loadTableItems({page, itemsPerPage, sortBy}) {
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}GetBookedSlots`] + user.value.sub;
  const token = await getAccessTokenSilently();

  const response = await fetch(apiEndpoint, {
    method: "GET",
    headers: {Authorization: 'Bearer ' + token}
  });

  switch (response.status) {
    case 200: {
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
    }
    case 204: {
      tableItems.value = [];
      totalLength.value = 0;
      loading.value = false;
      break;
    }
  }
}

async function cancelBooking(deskId, slotId){
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}BookDeskSlot`] + deskId + '/book-slot/' + slotId;
  const token = await getAccessTokenSilently();

  console.log(deskId)
  console.log(slotId)
  const response = await fetch(apiEndpoint, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: 'Bearer ' + token
    },
    body: JSON.stringify(user.value.sub)
  });

  switch (response.status) {
    case 200: {
      await snackbar.showSnackbar("mdi-hand-okay", "Booking cancelled successfully!", "success");
      tableStateHasChanged();
      break;
    }
    default: {
      snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while cancelling a booking.", "error");
      break;
    }
  }
}

function tableStateHasChanged() {
  renderTable.value += 1;
}

function getFullDescriptiveDate(day, startingTime, endingTime) {
  const date = new Date(day);
  const startingDate = new Date(startingTime);
  const endingDate = new Date(endingTime);

  return DayOfWeekFull[date.getDay()] + ', ' + date.getDay() + ' of ' + Months[date.getMonth()] +
    ', from ' + startingDate.getHours() + ' to ' + endingDate.getHours();
}

</script>
