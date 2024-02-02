import { makeAutoObservable } from "mobx";

class UserStore{
    token:string
    count:number = 1
    constructor(){
        this.token = ''
        makeAutoObservable(this);
    }
    inc(){
        this.count=this.count+1
    }
    desc(){
        this.count--
    }
    logOut(){
        this.token = ''
    }

}

export default new UserStore();
