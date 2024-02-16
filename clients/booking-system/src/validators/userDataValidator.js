import Division from "@/models/division";

class UserDataValidator{
  constructor() {
    this.titleRules = [
      value => {
        if (value) return true
        return 'Insert a title.'
      },
    ]

    this.givenNameRules = [
      value => {
        if (value) return true
        return 'Insert your given name.'
      },
    ]

    this.familyNameRules = [
      value => {
        if (value) return true
        return 'Insert your family name.'
      },
    ]

    this.nicknameRules = [
      value => {
        if (value) return true
        return 'Insert your nickname.'
      },
    ]

    this.divisionRules = [
      value => {
        if (!value) return 'Select a division.'
        if (!Object.values(Division).includes(value)) return 'Select a valid division.'
        return true;
      },
    ]

    this.emailRules = [
      value => {
        if (/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/.test(value)) return true
        return 'Insert a valid email.'
      },
    ]
  }
}

export default UserDataValidator;
