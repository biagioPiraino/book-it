const BaseEndpoint = "http://localhost:5068/api/"

const Endpoints = Object.freeze({
  developmentUpdateUser: BaseEndpoint + "Users",
  developmentGetUser: BaseEndpoint + "Users",
  developmentGetDesks: BaseEndpoint + "Desks/Geolocation",
  developmentBookDeskSlot: BaseEndpoint + "Desks/",
  developmentGetBookedSlots: BaseEndpoint + "Desks/Booked-Desks/",
})

export default Endpoints
