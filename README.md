# book-it

book-it is a desk-booking prototype platform built in .NET Core 7 and Vue.js.


## Architecture

The system is composed of one API that serves both a booking system platform and a management portal (now referred also as "clients").

Shell scripts have been created to automate weekly tasks to update desks' booking slots.

The data will be stored in a MongoDB database.


## Security

Authentication and authorization are managed using Auth0.

Loose CORS policies have been put in place to allow communications between clients and the API.


## Functionalities

### Booking System

The Booking System will allow a user to:
* Search for available desks near a specific postcode (UK only)
* View available desks and booking slots from a map
* Booking a specific slot
* Cancel a booking
* View and manage all the bookings from a summary table
* Update the user account information

### Management Portal

The Management Portal will allow a user to:
* Create desks
* Update desks' details
* Update the availability of booking slots
* Delete desks

### Scripts

The update-desks-slots shell script is a script that runs weekly and whose only purposes are: 
* Remove all the outdated booking slots (all the slots related to the previous working week that are by definition not bookable anymore)
* Create new booking slots for the working week ahead


## Third Party Integrations

[Vuetify](https://vuetifyjs.com/en/) components have been used for the Vue.js clients.

[Google Cloud APIs](https://cloud.google.com/apis/) have been used to display available desks on a map.

[Pinia](https://pinia.vuejs.org/) has been used a store library and state management framework for the Vue.js clients.

[Postcodes.io](https://api.postcodes.io/) API has been used to the retrieve geolocation of a specific postcode (UK only).
