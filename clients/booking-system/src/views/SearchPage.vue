<template>
  <v-container fluid>
    <PageTopSection :items="breadcrumbs" title="Search available desks"/>
    <v-container fluid>
      <v-container fluid>
        <v-row class="ga-0" no-gutters>
          <v-col cols="12" sm="12" md="6">
            <GMapAutocomplete
              class="py-1 px-3 mb-3 border rounded"
              placeholder="Search a location"
              @place_changed="updateSearchPosition"
            />
            <GMapMap
              class="pe-md-3"
              :key="renderMap"
              :center="center"
              :zoom="initialMapZoom"
              map-type-id="terrain"
              style="width: auto; min-height: 700px; border-radius: 5px"
            >
              <GMapMarker
                :key="marker.id"
                v-for="marker in markers"
                :position="marker.position"
                :icon="marker.icon"
                @click="loadDesksDetails(marker.id)"
              />
            </GMapMap>
          </v-col>
          <v-col cols="12" sm="12" md="6">
            <div class="text-h6 mb-2 text-secondary">Desks details</div>

            <v-container
              v-if="desksStore.currentSelection === ''"
              class="py-3 align-center text-center">
              <div class="mt-2 text-caption">No location selected</div>
            </v-container>

            <v-card
              v-else
              class="mb-3"
              :key="desk.deskId"
              v-for="desk in desksStore.desks[desksStore.currentSelection]"
              :title="desk.unit"
              :subtitle="`${desk.address.addressLine1}, ${desk.address.city}, ${desk.address.postcode}, ${desk.building}, ${desk.floor} floor.`"
              :text="`${DeskType[desk.deskType]} desk, with ${desk.availableScreens} screens available.`">
              <v-card-actions>
                <v-container class="py- align-center d-flex justify-center ga-4">
                  <v-chip
                    :color="!slot.isAvailable || (slot.isBooked && slot.userId !== user.sub) ? 'error' : (slot.isBooked && slot.userId === user.sub) ? 'success' : 'primary'"
                    :key="slot.slotId"
                    v-for="slot in desk.slots"
                    :text="DayOfWeek[(new Date(slot.day)).getDay()]"
                    @click="bookDeskSlot(desk.deskId, slot.id)"
                  />
                </v-container>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-container>
  </v-container>
</template>

<script setup>
import PageTopSection from "@/components/TopSection.vue";
import {onBeforeMount, ref} from "vue";
import Endpoints from "@/models/endpoints";
import {useAuth0} from "@auth0/auth0-vue";
import {useDesksStore} from "@/store/desksStore";
import {useGlobalSnackbar} from "@/store/snackbarStore";
import DeskType from "@/models/deskType";
import DayOfWeek from "@/models/dayOfWeek";

const breadcrumbs = [
  {
    title: 'Home',
    disabled: false,
    to: '/'
  },
  {
    title: 'Search',
    disabled: true,
    to: null
  }
]

const {user, getAccessTokenSilently} = useAuth0();
const initialMapZoom = ref(12);

// The initial location will be set to be London by default
// The user will than be located by the navigator.geolocation (dom property)
// and a marker will be displayed on the map
const center = ref({lat: 51.5072, lng: 0.1276});
const markers = [];
const desksStore = useDesksStore();
const snackbar = useGlobalSnackbar();

let renderMap = ref(0);

onBeforeMount(() => {
  locateUser();
})

function locateUser() {
  navigator.geolocation.getCurrentPosition((position) => {
    center.value = {
      lat: position.coords.latitude,
      lng: position.coords.longitude
    }

    markers.push({
      id: "user-search",
      position: {
        lat: position.coords.latitude,
        lng: position.coords.longitude,
      },
      icon: {
        url: "http://maps.google.com/mapfiles/ms/icons/purple-dot.png"
      }
    })
  })
}

async function updateSearchPosition(place) {
  center.value = {
    lat: place.geometry.location.lat(),
    lng: place.geometry.location.lng()
  }

  let latitude = place.geometry.location.lat();
  let longitude = place.geometry.location.lng();
  let userSearchIndex = markers.findIndex(x => x.id === "user-search")

  markers[userSearchIndex].position = {
    lat: latitude,
    lng: longitude
  }

  await loadNearbyDesks(latitude, longitude);
}

async function loadNearbyDesks(latitude, longitude) {
  const baseApiEndpoint = Endpoints[`${process.env.NODE_ENV}GetDesks`]
  const token = await getAccessTokenSilently();

  const params = {latitude: latitude, longitude: longitude};
  const queryParams = new URLSearchParams(params).toString();
  const fullApiEndpoint = `${baseApiEndpoint}?${queryParams}`;

  const response = await fetch(fullApiEndpoint, {
    method: "GET",
    headers: {Authorization: 'Bearer ' + token}
  });

  // We first clear all the data related to previous search
  cleanMapMarkers();
  desksStore.clear();

  switch (response.status) {
    case 200: {
      // In case our search is successful and desks are found in the proximity of the locality searched
      // we update the markers array, and we populate the store with the new data
      let serialisedResponse = await response.json();
      let desks = serialisedResponse.data;
      desks.forEach(addMarker);
      desksStore.populateStore(desks);
      break;
    }
  }

  mapStateHasChanged();
  console.log(desksStore.desks)
}

function cleanMapMarkers() {
  // This function will clear the existing markers array leaving only the user search one inside it
  let userSearchMarker = markers.find(x => x.id === "user-search");
  markers.length = 0;
  markers.push(userSearchMarker);
}

function addMarker(desk) {
  // We insert only one marker for postcode found. The details about the desk associated with a single postcode
  // will be retrieved from a pinia store
  if (markers.find(x => x.id === desk.address.postcode) !== undefined) return;

  markers.push({
    id: desk.address.postcode,
    position: {
      lat: desk.latitude,
      lng: desk.longitude,
    },
    icon: {
      url: "http://maps.google.com/mapfiles/ms/icons/red-dot.png"
    }
  })
}

function mapStateHasChanged() {
  renderMap.value += 1;
}

async function loadDesksDetails(id) {
  if (id === "user-search") return;

  // We avoid reloading the details in case the use click the same marker more than one time
  if (desksStore.currentSelection === id) return;

  // The id passed in this case will be the postcode of the marker, which is the key of the dictionary in the store
  await desksStore.getDesks(id);
}

async function bookDeskSlot(deskId, slotId){
  const apiEndpoint = Endpoints[`${process.env.NODE_ENV}BookDeskSlot`] + deskId + '/book-slot/' + slotId;
  const token = await getAccessTokenSilently();

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
      await snackbar.showSnackbar("mdi-hand-okay", "Booking updated successfully!", "success");

      const userSearchIndex = markers.findIndex(x => x.id === "user-search");

      const previousSelection = desksStore.currentSelection
      await loadNearbyDesks(markers[userSearchIndex].position.lat, markers[userSearchIndex].position.lng);
      await loadDesksDetails(previousSelection)
      break;
    }
    default: {
      snackbar.showSnackbar("mdi-close-box-outline", "An error occurred while updating a booking.", "error");
      break;
    }
  }
}

</script>
