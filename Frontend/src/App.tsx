import { BrowserRouter as Router, Routes, Route } from "react-router-dom"
import NavMenu from "./components/NavMenu";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import WelcomePage from "./pages/WelcomePage";
import './App.css'

function App() {

  return (
    <>
      <Router>
        <h1 className="top-right-title">User Management</h1>
        <NavMenu />
        <hr />
        <Routes>
          <Route path="/" element={<  WelcomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Routes>
      </Router>
    </>
  )
}

export default App
