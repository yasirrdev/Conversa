import "./home.css";
import Logo from "../../assets/logo.png";
import { Link } from "react-router-dom";

export default function Home() {
  return (
    <div className="home">
      <div className="hero">
        <div className="hero-content">
          <h1>Conversa</h1>
          <p className="tagline">Conéctate. Comunícate. Colabora.</p>
          <Link to="/login" className="cta-button">
            ¡Empieza ahora!
          </Link>
        </div>

        <div>
          <img
            src={Logo || "/placeholder.svg"}
            alt="Logo de Conversa"
            className="hero-logo"
          />
        </div>
      </div>

      <div className="features">
        <h2>¿Por qué elegir Conversa?</h2>
        <div className="feature-cards">
          <div className="feature-card">
            <div className="feature-icon">💬</div>
            <h3>Chat en Tiempo Real</h3>
            <p>¡Conéctate instantáneamente con tus amigos!</p>
          </div>
          <div className="feature-card">
            <div className="feature-icon">🔒</div>
            <h3>Chat Seguro</h3>
            <p>¡Tus conversaciones siempre serán seguras!</p>
          </div>
          <div className="feature-card">
            <div className="feature-icon">🌐</div>
            <h3>Chat Global</h3>
            <p>¡Comunícate desde cualquier rincón del mundo!</p>
          </div>
        </div>
      </div>

      <div className="testimonials">
        <h2>¿Qué opinan nuestros usuarios?</h2>
        <div className="testimonial-container">
          <div className="testimonial">
            <p>
              "Conversa ha transformado la forma en que nuestro equipo se comunica. 
              ¡Es intuitivo y hermoso!"
            </p>
            <div className="testimonial-author">- J. Tejada</div>
          </div>
          <div className="testimonial">
            <p>
              "La mejor plataforma de mensajería que he utilizado. Interfaz clara y servicio confiable."
            </p>
            <div className="testimonial-author">- R. López</div>
          </div>
        </div>
      </div>

      <footer>
        <div className="footer-bottom">
          <p>&copy; 2025 Conversa. Todos los derechos reservados.</p>
        </div>
      </footer>
    </div>
  );
}
