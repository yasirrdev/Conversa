import { createContext, useContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { LOGIN_URL, SIGNUP_URL } from "../lib/endpoints/config";
import { FETCH_POST } from "../lib/endpoints/useFetch";
import { Link } from "react-router-dom";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {

  const [token, setToken] = useState(null);
  const [userId, setUserId] = useState(null);
  const [userInfo, setUserInfo] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const extractUserId = (accessToken) => {
    try {
      const decoded = jwtDecode(accessToken);
  
      return decoded.ID ? String(decoded.ID) : decoded.id ? String(decoded.id) : null;
    } catch (error) {
      console.error("⚠️ Error al decodificar JWT:", error);
      return null;
    }
  };
  

  useEffect(() => {
    const savedToken =
      localStorage.getItem("accessToken") ||
      sessionStorage.getItem("accessToken");

    if (savedToken) {
      const extractedUserId = extractUserId(savedToken);
      if (extractedUserId) {
        setToken(savedToken);
        setUserId(extractedUserId);
        setUserInfo(jwtDecode(savedToken));
        setIsAuthenticated(true);
      } else {
        logout();
      }
    }
  }, []);

  const login = async (identification, password, rememberMe) => {
    try {
      const data = await FETCH_POST(LOGIN_URL, { identification, password });
      console.log("Raw response:", data);
  
      let accessToken = typeof data === 'string' ? data : data.accessToken || data.token;
      if (!accessToken) throw new Error("⚠️ Token de acceso inválido");
  
      const decodedUserId = extractUserId(accessToken);
      if (!decodedUserId) throw new Error("⚠️ No se pudo extraer el ID del usuario");
  
      setToken(accessToken);
      setUserId(decodedUserId);
      setUserInfo(jwtDecode(accessToken));
      setIsAuthenticated(true);
  
      if (rememberMe) {
        localStorage.setItem("accessToken", accessToken);
      } else {
        sessionStorage.setItem("accessToken", accessToken);
      }
  
      console.log("✅ Login exitoso:", { identification, rememberMe });
    } catch (error) {
      console.error("❌ Error en login:", error.message);
      throw error;
    }
  };

  const register = async (name, phone, password) => {
    try {
      const formData = new FormData();
      formData.append("Name", name);
      formData.append("Phone", phone);
      formData.append("Password", password);

      const response = await fetch(SIGNUP_URL, {
        method: "POST",
        headers: {
          accept: "*/*",
        },
        body: formData,
      });

      if (!response.ok) {
        throw new Error(`⚠️ Error en registro: ${response.status}`);
      }

      const data = await response.json();

      console.log("✅ Registro exitoso:", data);

      return data;
    } catch (error) {
      console.error("❌ Error en registro:", error);
      throw error;
    }
  };

  const logout = () => {
    setToken(null);
    setUserId(null);
    setUserInfo(null);
    setIsAuthenticated(false);
    localStorage.removeItem("accessToken");
    sessionStorage.removeItem("accessToken");
  };

  return (
    <AuthContext.Provider
      value={{
        token,
        userId,
        userInfo,
        login,
        register,
        logout,
        isAuthenticated,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe usarse dentro de un AuthProvider");
  }
  return context;
};
