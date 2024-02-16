import {defineStore} from "pinia";

export const useDesksStore = defineStore('desksStore', {
  state: () => ({
    desks: {},
    currentSelection: ""
  }),
  getters: {},
  actions: {
    populateStore(desks) {
      desks.forEach(this.addDeskToStore);
    },

    addDeskToStore(desk){
      if (this.desks[desk.address.postcode] === undefined) {
        this.desks[desk.address.postcode] = [desk]
      }
      else {
        this.desks[desk.address.postcode].push(desk)
      }
    },

    getDesks(postcode) {
      this.currentSelection = postcode;
      return this.desks[postcode]
    },

    clear() {
      this.desks = {};
      this.currentSelection = "";
    }
  }
})
