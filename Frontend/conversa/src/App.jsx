import "./App.css";
import Home from "./components/home/home.jsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LogIn from "./components/auth/LogIn/LogIn.jsx";
import SignIn from "./components/auth/SignIn/SignIn.jsx";
import Chat from "./components/chats/chats.jsx";
function App() {
  return (
      <div className="App">
        <Routes>
          <Route path="/" element={<Chat />} />
          <Route path="/home" element={<Home />} />
          <Route path="/signin" element={<SignIn />} />
          <Route path="/login" element={<LogIn />} />
        </Routes>
      </div>
  );
}

export default App;
