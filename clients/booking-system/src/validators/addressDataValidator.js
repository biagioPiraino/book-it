class AddressDataValidator{
  constructor() {
    this.addressLine1Rules = [
      value => {
        if (value) return true
        return 'Insert an address.'
      },
    ]

    this.postcodeRules = [
      value => {
        if (/^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$/i.test(value)) return true
        return 'Insert a valid postcode (A123BC).'
      },
    ]

    this.cityRules = [
      value => {
        if (value) return true
        return 'Insert a city.'
      },
    ]

    this.countryRules = [
      value => {
        if (!value) return 'Select a country.'
        if (value !== "United Kingdom") return 'Select a valid country.'
        return true;
      },
    ]
  }
}

export default AddressDataValidator;
