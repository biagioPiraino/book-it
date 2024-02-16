import AddressData from "@/models/addressData";

class UserData {
  constructor() {
    this.id = ""
    this.title = "";
    this.givenName = "";
    this.familyName = "";
    this.nickname = "";
    this.emailAddress = "";
    this.isFederatedUser = false;
    this.division = undefined;
    this.address = new AddressData();
  }

  buildAuth0AccountInfo(authUser){
    this.id = authUser.sub;
    this.emailAddress = authUser.email;
    this.givenName = authUser.given_name;
    this.familyName = authUser.family_name;
    this.nickname = authUser.nickname;
    this.isFederatedUser = authUser.sub.includes("google");
  }

  buildUserInfo(userData){
    this.title = userData.title;
    this.division = userData.division;
    this.address = userData.address;
  }
}

export default UserData;
