import { Button } from "antd";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

const LoginPage = () => {
  const [data, updateData] = useState({
    userName: "11",
    password: "22",
  });
  const navigator = useNavigate();

  const goHome=()=>{
    navigator("/");
  }
  return (
    <div>
      <h1>Login</h1>
      <div>{data.userName}</div>
      <div>{data.password}</div>
      <Button onClick={goHome}>跳转到首页</Button>
    </div>
  );
};

export default LoginPage;
