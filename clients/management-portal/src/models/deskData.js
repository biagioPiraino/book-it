import AddressData from "@/models/addressData";

class DeskData {
  constructor() {
    this.deskId = "";
    this.unit = "";
    this.building = "";
    this.floor = undefined;
    this.availableScreens = undefined;
    this.deskType = undefined;
    this.address = new AddressData();
    this.slots = []
  }
}

export default DeskData;
