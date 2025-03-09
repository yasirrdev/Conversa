import "./App.css";
import Home from "./components/home/home.jsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LogIn from "./components/auth/LogIn/LogIn.jsx";
import SignIn from "./components/auth/SignIn/SignIn.jsx";
function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/signin" element={<SignIn />} />
          <Route path="/login" element={<LogIn />} />

        </Routes>
      </div>
    </Router>
  );
}

export default App;
