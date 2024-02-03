import { makeAutoObservable } from "mobx";

class UserStore{
    userid:number = -1
    nickName:string = ''
    constructor(){
        makeAutoObservable(this);
    }

    setUserId(value:number){
        this.userid = value;
    }

    setNickName(value:string){
        this.nickName = value;
    }


}

export default new UserStore();
