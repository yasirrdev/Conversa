import { useState } from "react"
import { Link } from "react-router-dom"
import { useAuth } from "../../../context/AuthContext"
import "../styles/auth.css"
import Logo from "../../../assets/logo.png"

export default function Signin() {
  const [name, setName] = useState("")
  const [phone, setPhone] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [acceptTerms, setAcceptTerms] = useState(false)
  const { register, login} = useAuth()

  const handlePhoneChange = (e) => {
    let value = e.target.value.replace(/\D/g, "")
    
    if (value.length > 9) value = value.slice(0, 9)

    if (value.length > 6) {
      value = `${value.slice(0, 3)} ${value.slice(3, 6)} ${value.slice(6)}`
    } else if (value.length > 3) {
      value = `${value.slice(0, 3)} ${value.slice(3)}`
    }

    setPhone(value)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (password !== confirmPassword) {
      console.error("❌ Las contraseñas no coinciden")
      return
    }

    try {
      await register(name, phone.replace(/\s/g, ""), password)
      console.log("✅ Registro exitoso con:", { name, phone })
      await login(phone.replace(/\s/g, ""), password)
    } catch (error) {
      console.error("❌ Error en registro:", error.message)
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header">
          <img src={Logo || "/placeholder.svg"} alt="Conversa Logo" className="auth-logo" />
          <h1>Crear Cuenta</h1>
          <p>Únete a la comunidad de Conversa</p>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="name">Nombre Completo</label>
            <input
              type="text"
              id="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Tu nombre"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="phone">Número de Teléfono</label>
            <input
              type="text"
              id="phone"
              value={phone}
              onChange={handlePhoneChange}
              placeholder="666 666 666"
              maxLength="11" // Máximo 9 números + 2 espacios
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
              placeholder="Crea una contraseña"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirmar Contraseña</label>
            <input
              type="password"
              id="confirmPassword"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Confirma tu contraseña"
              required
            />
          </div>

          <div className="form-options">
            <div className="remember-me">
              <input
                type="checkbox"
                id="terms"
                checked={acceptTerms}
                onChange={(e) => setAcceptTerms(e.target.checked)}
                required
              />
              <label htmlFor="terms">Acepto los términos y condiciones</label>
            </div>
          </div>

          <button type="submit" className="auth-button">
            Crear Cuenta
          </button>
        </form>

        <div className="auth-footer">
          <p>
            ¿Ya tienes una cuenta?{" "}
            <Link to="/login" className="auth-link">
              Inicia Sesión
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
