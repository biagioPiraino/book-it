import { defineStore } from 'pinia'

export const useGlobalSnackbar = defineStore('snackbar', {
  state: () => ({
    show: false,
    icon: "",
    message: "",
    color: ""
  }),
  getters: { },
  actions: {
    showSnackbar(icon, message, color) {
      this.show = true;
      this.icon = icon;
      this.message = message;
      this.color = color;
    }
  }
})
