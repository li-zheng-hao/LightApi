import { Button } from "antd";
import {  useNavigate } from "react-router-dom";

const HongPage = () => {
  const navigator = useNavigate();
  const goPage = (path: string) => {
    navigator(path);
  };

  return (
    <div>
      <h1>Home</h1>
      <Button onClick={() => goPage("page1")}>Page1</Button>
      <Button onClick={() => goPage("page2")}>Page2</Button>
    </div>
  );
};

export default HongPage;
