import "./home.css";
import Logo from "../../assets/logo.png";
import { Link } from "react-router-dom"; // Importación corregida
import ChatComponent from "../../chatComponent";
export default function Home() {
    return (
        <div className="home">
            <div className="hero">
                <div className="hero-content">
                    <h1>Conversa</h1>
                    <p className="tagline">Conectate. Comunicate. Colabora.</p>
                    <Link to="/login" className="cta-button">
                        Empieza Ahora!
                    </Link>
                </div>

                <div className="hero-image">
                    <img
                        src={Logo || "/placeholder.svg"}
                        alt="Conversa Logo"
                        className="hero-logo"
                    />

                    <div className="blob"></div>
                    <div className="circle-1"></div>
                    <div className="circle-2"></div>
                    <div className="circle-3"></div>
                </div>
            </div>

            <div className="features">
                <h2>¿Porque elegir Conversa?</h2>
                <div className="feature-cards">
                    <div className="feature-card">
                        <div className="feature-icon">💬</div>
                        <h3>Chat en Tiempo Real</h3>
                        <p>¡Conecta instantáneamente con tus amigos!.</p>
                    </div>
                    <div className="feature-card">
                        <div className="feature-icon">🔒</div>
                        <h3>Chat Seguro</h3>
                        <p>¡Tus conversaciones siempre serán seguras!.</p>
                    </div>
                    <div className="feature-card">
                        <div className="feature-icon">🌐</div>
                        <h3>Chat Global</h3>
                        <p>¡Comunícate desde cualquier rincón del mundo!.</p>
                    </div>
                </div>
            </div>

            <div className="testimonials">
                <h2>¿Qué opinan nuestros usuarios?</h2>
                <div className="testimonial-container">
                    <div className="testimonial">
                        <p>
                            "Conversa ha transformado la forma en que nuestro equipo se
                            comunica. ¡Es intuitivo y hermoso!"
                        </p>
                        <div className="testimonial-author">- J. Tejada</div>
                    </div>
                    <div className="testimonial">
                        <p>
                            "La mejor plataforma de mensajería que he utilizado. Interfaz
                            clara y servicio confiable".
                        </p>
                        <div className="testimonial-author">- R. Lopez</div>
                    </div>
                </div>
            </div>
            <ChatComponent /> {/* Corrección: Usar PascalCase en el JSX */}

            <footer>
                <div className="footer-content">
                    <div className="footer-logo">Conversa</div>
                    <div className="footer-links">
                        <a href="#">Acerca de Nosotros</a>
                        <a href="#">Características</a>
                        <a href="#">Contáctanos</a>
                    </div>
                    <div className="footer-social">
                        <a href="#" className="social-icon">
                            📱
                        </a>
                        <a href="#" className="social-icon">
                            💻
                        </a>
                        <a href="#" className="social-icon">
                            📧
                        </a>
                    </div>
                </div>
                <div className="footer-bottom">
                    <p>© 2025 Conversa. All rights reserved.</p>
                </div>
            </footer>
        </div>
    );
}