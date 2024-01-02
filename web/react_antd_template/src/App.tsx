import { useState } from "react";
import { Button } from "antd";
import IconBottom from "./components/icons/IconBottom";
import Page1 from "./components/Page1";
import Page2 from "./components/Page2";
import { observer } from "mobx-react";
import userStore from "./stores/UserStore";

const App = () => {
  const [count, setCount] = useState(0);
  const clickHandle=()=>{
    userStore.inc();
    console.log('click',userStore.count);
    
  }
  return (
    <>
      <Button type="primary" onClick={clickHandle}>
        点击按钮
      </Button>
      <div>点击了{userStore.count}</div>
      <IconBottom
        style={{ fontSize: "32px", color: "red", cursor: "pointer" }}
      ></IconBottom>
      <div flex>
        <Page1 />
        <Page2 />
      </div>
    </>
  );
};

export default observer(App);
