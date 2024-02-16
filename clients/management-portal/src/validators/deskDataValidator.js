class DeskDataValidator{
  constructor() {
    this.unitRules = [
      value => {
        if (value) return true
        return 'Insert a unit name or identifier.'
      },
    ]

    this.buildingRules = [
      value => {
        if (value) return true
        return 'Insert a building.'
      },
    ]

    this.floorRules = [
      value => {
        if (value) return true
        return 'Insert a floor.'
      },
    ]

    this.screenRules = [
      value => {
        if (value) return true
        return 'Insert the amount of screens available.'
      },
    ]

    this.typeRules = [
      value => {
        if (value) return true
        return 'Select a desk type.'
      },
    ]
  }
}

export default DeskDataValidator;
