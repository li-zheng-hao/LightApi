import { useState } from "react";
import { Button } from "antd";
import { observer } from "mobx-react";
import userStore from "./stores/UserStore";
import { NavLink, Route, Routes } from "react-router-dom";
import Login from "./views/Login";
import Home from "./views/Home";

const App = () => {
  const [count, setCount] = useState(0);
  const clickHandle = () => {
    userStore.inc();
    console.log("click", userStore.count);
  };
  return (
    <>
        <NavLink to="">
          <Button>首页</Button>
        </NavLink>
        <NavLink to="/login">
          <Button>登录</Button>
        </NavLink>
        <Routes>
        <Route path="/" element={<Home />} />
        <Route path="*" element={<Login />} />
      </Routes>
    </>
  );
};

export default observer(App);
