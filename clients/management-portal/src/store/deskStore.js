import { defineStore } from 'pinia'

export const useDeskStore = defineStore('deskStore', {
  state: () => ({
    deskId: "",
    deskName: ""
  }),
  getters: { },
  actions: {
    populateStore(deskId, deskName) {
      this.deskId = deskId;
      this.deskName = deskName;
    },
    clear() {
      this.deskId = "";
      this.deskName = "";
    }
  },
  persist: true
})
