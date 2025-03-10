import "./App.css";
import Home from "./components/home/home.jsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LogIn from "./components/auth/LogIn/LogIn.jsx";
import SignIn from "./components/auth/SignIn/SignIn.jsx";
import { WebSocketProvider } from "../context/webSocketContex.jsx";
import ChatComponent from "./chatComponent.jsx";
const CHRISTIAN_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjI3MTUxMWQ5LWMzNzYtNGU1ZC04MTU4LTZmYmU2N2FhMWVjYyIsIk5hbWUiOiJDaHJpc3RpYW4iLCJQaG9uZSI6IjExMTExMTExMSIsIlN0YXR1cyI6IkhpLCBJJ20gdXNpbmcgQ29udmVyc2EhISEiLCJuYmYiOjE3NDE1NDQ2NDAsImV4cCI6MTc0MTU1MTg0MCwiaWF0IjoxNzQxNTQ0NjQwfQ.8GMNAQ2KIwIsGVek8tW_dDR_tgN9vs12W7h8Uj3W9Uw";
const YASIR_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjBlMjA2ZDkyLTkzODItNDU3Yy1hZmMxLTM0ZDRiOWI0OWIyZSIsIk5hbWUiOiJZYXNpciIsIlBob25lIjoiMjIyMjIyMjIyIiwiU3RhdHVzIjoiSGksIEknbSB1c2luZyBDb252ZXJzYSEhISIsIm5iZiI6MTc0MTU0NTg0MywiZXhwIjoxNzQxNTUzMDQzLCJpYXQiOjE3NDE1NDU4NDN9.mnI3E6vi7PrNwnEi1RtINq8TXm62-KvQjzsBPoLqVWc";

const USERS = {
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjI3MTUxMWQ5LWMzNzYtNGU1ZC04MTU4LTZmYmU2N2FhMWVjYyIsIk5hbWUiOiJDaHJpc3RpYW4iLCJQaG9uZSI6IjExMTExMTExMSIsIlN0YXR1cyI6IkhpLCBJJ20gdXNpbmcgQ29udmVyc2EhISEiLCJuYmYiOjE3NDE1NDQ2NDAsImV4cCI6MTc0MTU1MTg0MCwiaWF0IjoxNzQxNTQ0NjQwfQ.8GMNAQ2KIwIsGVek8tW_dDR_tgN9vs12W7h8Uj3W9Uw": CHRISTIAN_TOKEN, // Christian
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjBlMjA2ZDkyLTkzODItNDU3Yy1hZmMxLTM0ZDRiOWI0OWIyZSIsIk5hbWUiOiJZYXNpciIsIlBob25lIjoiMjIyMjIyMjIyIiwiU3RhdHVzIjoiSGksIEknbSB1c2luZyBDb252ZXJzYSEhISIsIm5iZiI6MTc0MTU0NTg0MywiZXhwIjoxNzQxNTUzMDQzLCJpYXQiOjE3NDE1NDU4NDN9.mnI3E6vi7PrNwnEi1RtINq8TXm62-KvQjzsBPoLqVWc": YASIR_TOKEN,     // Yasir
};

function App() {
    return (
        <WebSocketProvider users={USERS}>
            <Router>
                <div className="App">
                    <Routes>
                        <Route path="/home" element={<Home />} />
                        <Route path="/signin" element={<SignIn />} />
                        <Route path="/login" element={<LogIn />} />
                    </Routes>
                </div>
            </Router>
            <ChatComponent />
        </WebSocketProvider>
    );
}

export default App;