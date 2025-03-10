import React, { useState } from 'react';
import { useWebSocket } from '../context/webSocketContex';

function ChatComponent() {
    const { sockets, messages, sendMessage, markMessagesAsRead, connectWebSocket, disconnectWebSocket, isConnected } = useWebSocket();
    const [content, setContent] = useState("");
    const [receiverId, setReceiverId] = useState("");
    const [selectedUserId, setSelectedUserId] = useState("");
    const [connectionStatus, setConnectionStatus] = useState("Desconectado");

    const users = {
        "271511d9-c376-4e5d-8158-6fbe67aa1ecc": {
            name: "Christian",
            token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjI3MTUxMWQ5LWMzNzYtNGU1ZC04MTU4LTZmYmU2N2FhMWVjYyIsIk5hbWUiOiJDaHJpc3RpYW4iLCJQaG9uZSI6IjExMTExMTExMSIsIlN0YXR1cyI6IkhpLCBJJ20gdXNpbmcgQ29udmVyc2EhISEiLCJuYmYiOjE3NDE1NDQ2NDAsImV4cCI6MTc0MTU1MTg0MCwiaWF0IjoxNzQxNTQ0NjQwfQ.8GMNAQ2KIwIsGVek8tW_dDR_tgN9vs12W7h8Uj3W9Uw"
        },
        "0e206d92-9382-457c-afc1-34d4b9b49b2e": {
            name: "Yasir",
            token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjBlMjA2ZDkyLTkzODItNDU3Yy1hZmMxLTM0ZDRiOWI0OWIyZSIsIk5hbWUiOiJZYXNpciIsIlBob25lIjoiMjIyMjIyMjIyIiwiU3RhdHVzIjoiSGksIEknbSB1c2luZyBDb252ZXJzYSEhISIsIm5iZiI6MTc0MTU0NTg0MywiZXhwIjoxNzQxNTUzMDQzLCJpYXQiOjE3NDE1NDU4NDN9.mnI3E6vi7PrNwnEi1RtINq8TXm62-KvQjzsBPoLqVWc"
        }
    };

    const handleConnect = () => {
        if (!selectedUserId) {
            alert("Por favor, selecciona un usuario para conectar.");
            return;
        }

        const user = users[selectedUserId];
        if (user) {
            setConnectionStatus(`Conectando como ${user.name}...`);
            connectWebSocket(selectedUserId, user.token);
        }
    };

    const handleDisconnect = () => {
        if (!selectedUserId) {
            alert("Por favor, selecciona un usuario para desconectar.");
            return;
        }

        disconnectWebSocket(selectedUserId);
        setConnectionStatus("Desconectado");
    };

    const handleSendMessage = () => {
        if (!content || !receiverId || !selectedUserId) {
            alert("Por favor, completa todos los campos");
            return;
        }
        sendMessage(selectedUserId, receiverId, content);
        setContent("");
    };

    const handleMarkAsRead = (contactId) => {
        if (!selectedUserId) {
            alert("Por favor, selecciona un usuario para marcar mensajes como leídos.");
            return;
        }
        markMessagesAsRead(selectedUserId, contactId);
    };

    console.log(`Mensajes para ${selectedUserId}:`, messages[selectedUserId]);

    return (
        <div>
            <h1>Simulación de Chat</h1>

            <div>
                <h3>Conectar al WebSocket</h3>
                <select value={selectedUserId} onChange={(e) => setSelectedUserId(e.target.value)}>
                    <option value="">Seleccionar usuario</option>
                    {Object.entries(users).map(([id, user]) => (
                        <option key={id} value={id}>{user.name}</option>
                    ))}
                </select>
                <button onClick={handleConnect} disabled={isConnected(selectedUserId)}>
                    Conectar
                </button>
                <button onClick={handleDisconnect} disabled={!isConnected(selectedUserId)}>
                    Desconectar
                </button>
                <p>Estado: {isConnected(selectedUserId) ? `Conectado como ${users[selectedUserId]?.name}` : connectionStatus}</p>
            </div>

            <div>
                <h3>Enviar mensaje</h3>
                <select value={receiverId} onChange={(e) => setReceiverId(e.target.value)}>
                    <option value="">Seleccionar destinatario</option>
                    {Object.entries(users).map(([id, user]) => (
                        <option key={id} value={id}>{user.name}</option>
                    ))}
                </select>
                <input
                    type="text"
                    placeholder="Escribe tu mensaje"
                    value={content}
                    onChange={(e) => setContent(e.target.value)}
                />
                <button onClick={handleSendMessage} disabled={!isConnected(selectedUserId)}>
                    Enviar mensaje
                </button>
            </div>

            <div>
                <h3>Marcar mensajes como leídos</h3>
                <button onClick={() => handleMarkAsRead("271511d9-c376-4e5d-8158-6fbe67aa1ecc")} disabled={!isConnected(selectedUserId)}>
                    Marcar mensajes de Christian como leídos
                </button>
                <button onClick={() => handleMarkAsRead("0e206d92-9382-457c-afc1-34d4b9b49b2e")} disabled={!isConnected(selectedUserId)}>
                    Marcar mensajes de Yasir como leídos
                </button>
            </div>

            {selectedUserId && (
                <div>
                    <h3>Mensajes de {users[selectedUserId]?.name}</h3>
                    <div>
                        <h4>Mensajes Enviados</h4>
                        {messages[selectedUserId] && messages[selectedUserId].filter((msg) => msg.senderId === selectedUserId).length > 0 ? (
                            <ul>
                                {messages[selectedUserId]
                                    .filter((msg) => msg.senderId === selectedUserId)
                                    .map((msg) => (
                                        <li key={msg.messageId || msg.tempId}>
                                            Para: {users[msg.receiverId]?.name || msg.receiverId} - {msg.content} (Estado: {msg.status})
                                        </li>
                                    ))}
                            </ul>
                        ) : (
                            <p>No hay mensajes enviados</p>
                        )}
                        <h4>Mensajes Recibidos</h4>
                        {messages[selectedUserId] && messages[selectedUserId].filter((msg) => msg.receiverId === selectedUserId).length > 0 ? (
                            <ul>
                                {messages[selectedUserId]
                                    .filter((msg) => msg.receiverId === selectedUserId)
                                    .map((msg) => (
                                        <li key={msg.messageId || msg.tempId}>
                                            De: {users[msg.senderId]?.name || msg.senderId} - {msg.content} (Estado: {msg.status})
                                        </li>
                                    ))}
                            </ul>
                        ) : (
                            <p>No hay mensajes recibidos</p>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}

export default ChatComponent;