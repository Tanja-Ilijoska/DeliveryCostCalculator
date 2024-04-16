import React from 'react'
import ReactDOM from 'react-dom/client'
import AppMenuBar from './AppMenuBar.tsx';
import App from './App.tsx'
import './index.css'
import axios from 'axios';
//import Deliveries from './Deliveries.tsx';

axios.defaults.baseURL = 'https://localhost:7059'


ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
      <AppMenuBar/>
      <App />
  </React.StrictMode>,
)
