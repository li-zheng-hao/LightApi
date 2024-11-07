import { Button } from 'antd';
import { Outlet, useNavigate } from 'react-router-dom';

const HongPage = () => {
  const navigator = useNavigate();
  const goPage = (path: string) => {
    navigator(path);
  };

  return (
    <div>
      <h1 className="text-3xl font-bold underline">Home</h1>
      <Button onClick={() => goPage('page1')}>Page1</Button>
      <Button onClick={() => goPage('page2')}>Page2</Button>
      <Outlet></Outlet>
    </div>
  );
};

export default HongPage;
