import { BrowserRouter as Router, Routes, Route } from "react-router-dom"
import NavMenu from "./components/NavMenu";
import CreateUserForm from "./pages/CreateUserForm"
import GetUserList from "./pages/GetUserList";
import './App.css'

function App() {

  return (
    <>
      <Router>
        <h1 className="top-right-title">User Management</h1>
        <NavMenu />
        <hr />
        <Routes>
          <Route path="/create-user" element={<CreateUserForm />} />
          <Route path="/user-list" element={<GetUserList />} />
        </Routes>
      </Router>
    </>
  )
}

export default App
