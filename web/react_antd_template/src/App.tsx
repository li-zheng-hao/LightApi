import { useState } from 'react'
import './App.css'
import { Button } from 'antd'
import IconBottom from './components/icons/IconBottom'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
    <Button type="primary" onClick={()=>setCount(count+1)}>点击按钮</Button>
    <div>点击了{count}</div>
    <IconBottom style={{ fontSize: '32px',color:'red',cursor:"pointer" }}></IconBottom>
    </>
  )
}

export default App
