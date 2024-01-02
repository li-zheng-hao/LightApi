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
    setToken(token:string){
        this.token = token
    }
    getToken(){
        return this.token
    }
    logOut(){
        this.token = ''
    }

}

export default new UserStore();