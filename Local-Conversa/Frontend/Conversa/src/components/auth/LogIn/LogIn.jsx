import { useState } from "react"
import { Link } from "react-router-dom"
import { useAuth } from "../../../context/AuthContext"
import "../styles/auth.css"
import Logo from "../../../assets/logo.png"

export default function Login() {
  const [password, setPassword] = useState("")
  const [rememberMe, setRememberMe] = useState(false)
  const { login } = useAuth()

  const [identification, setIdentification] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(identification, password, rememberMe); // Usar 'identification'
      console.log("✅ Login exitoso con:", { identification, password, rememberMe });
    } catch (error) {
      console.error("❌ Error en login:", error.message);
    }
  };
  
  
  const handlePhoneChange = (e) => {
    let value = e.target.value.replace(/\D/g, "")
    
    if (value.length > 9) value = value.slice(0, 9)

    if (value.length > 6) {
      value = `${value.slice(0, 3)} ${value.slice(3, 6)} ${value.slice(6)}`
    } else if (value.length > 3) {
      value = `${value.slice(0, 3)} ${value.slice(3)}`
    }

    setIdentification(value)
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header">
          <img src={Logo || "/placeholder.svg"} alt="Conversa Logo" className="auth-logo" />
          <h1>Iniciar Sesión</h1>
          <p>Bienvenido de nuevo a Conversa</p>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="phone">Número de Teléfono</label>
            <input
              type="tel" 
              id="phone"
              value={identification}
              onChange={handlePhoneChange}
              placeholder="Ej: 665551123"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Contraseña</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Tu contraseña"
              required
            />
          </div>

          <div className="form-options">
            <div className="remember-me">
              <input
                type="checkbox"
                id="remember"
                checked={rememberMe}
                onChange={(e) => setRememberMe(e.target.checked)}
              />
              <label htmlFor="remember">Recordarme</label>
            </div>
          </div>

          <button type="submit" className="auth-button">
            Iniciar Sesión
          </button>
        </form>

        <div className="auth-footer">
          <p>
            ¿No tienes una cuenta?{" "}
            <Link to="/signin" className="auth-link">
              Regístrate
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
