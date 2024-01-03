import { useState } from "react";

const LoginPage = () => {
  const [data, updateData] = useState({
    userName: "11",
    password: "22",
  });
  return (
    <div>
      <h1>Login</h1>
      <div>{data.userName}</div>
      <div>{data.password}</div>
    </div>
  );
};

export default LoginPage;
