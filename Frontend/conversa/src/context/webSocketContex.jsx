import React, { createContext, useContext, useState, useCallback } from 'react';
import PropTypes from 'prop-types';

const WebSocketContext = createContext(null);

export const useWebSocket = () => {
    const context = useContext(WebSocketContext);
    if (!context) {
        throw new Error('useWebSocket debe ser usado dentro de un WebSocketProvider');
    }
    return context;
};

export const WebSocketProvider = ({ children }) => {
    const [sockets, setSockets] = useState({});
    const [messages, setMessages] = useState({});

    const connectWebSocket = useCallback((userId, token) => {
        if (!token) {
            console.error(`No se proporcionó un token JWT para el usuario ${userId}`);
            return;
        }

        if (sockets[userId]) {
            console.log(`El WebSocket para el usuario ${userId} ya está conectado`);
            return;
        }

        const wsUrl = `wss://localhost:7075/ws?token=${token}`;
        console.log(`Intentando conectar al WebSocket para ${userId} en: ${wsUrl}`);
        const ws = new WebSocket(wsUrl);

        ws.onopen = () => {
            console.log(`Usuario ${userId} conectado al servidor WebSocket`);
            setSockets((prev) => ({ ...prev, [userId]: ws }));
        };

        ws.onmessage = (event) => {
            const data = JSON.parse(event.data);
            console.log(`Mensaje recibido para el usuario ${userId}:`, data);
        
            setMessages((prev) => {
                const userMessages = prev[userId] || [];
        
                if (data.action === "new_message") {
                    console.log(`Añadiendo nuevo mensaje recibido para ${userId}:`, data);
                    return {
                        ...prev,
                        [userId]: [...userMessages, data],
                    };
                } else if (data.action === "message_status") {
                console.log(`Actualización de estado recibida para ${userId}: messageId=${data.messageId}, status=${data.status}`);
                
                return {
                    ...prev,
                    [userId]: userMessages.map((msg) => {
                        if (msg.tempId && msg.status === "pending") {
                            // Si el tempId coincide con un mensaje en "pending", actualiza con el messageId real
                            return { ...msg, messageId: data.messageId, status: data.status, tempId: undefined };
                        } else if (msg.messageId === data.messageId) {
                            // Si el messageId coincide, solo actualiza el estado
                            return { ...msg, status: data.status };
                        }
                        return msg;
                    }),
                };
            }
                return prev;
            });
        };

        ws.onclose = (event) => {
            console.log(`Usuario ${userId} desconectado del servidor WebSocket. Código: ${event.code}, Razón: ${event.reason}`);
            setSockets((prev) => {
                const newSockets = { ...prev };
                delete newSockets[userId];
                return newSockets;
            });
        };

        ws.onerror = (error) => {
            console.error(`Error en WebSocket para el usuario ${userId}:`, error);
            setSockets((prev) => {
                const newSockets = { ...prev };
                delete newSockets[userId];
                return newSockets;
            });
        };
    }, [sockets]);

    const disconnectWebSocket = useCallback((userId) => {
        const socket = sockets[userId];
        if (socket) {
            socket.close();
            setSockets((prev) => {
                const newSockets = { ...prev };
                delete newSockets[userId];
                return newSockets;
            });
            console.log(`WebSocket para el usuario ${userId} desconectado manualmente`);
        }
    }, [sockets]);

    const sendMessage = useCallback((userId, receiverId, content) => {
        const socket = sockets[userId];
        if (!socket) {
            console.error(`No se puede enviar mensaje: WebSocket para el usuario ${userId} no está conectado`);
            return;
        }
    
        const tempId = `temp-${Date.now()}-${Math.random().toString(36).substring(2)}`;
    
        const message = {
            action: "send_message",
            receiverId,
            content,
        };
    
        socket.send(JSON.stringify(message));
        console.log(`Mensaje enviado desde ${userId} a ${receiverId}:`, message);
    
        // Guardar mensaje localmente con estado "pending"
        setMessages((prev) => ({
            ...prev,
            [userId]: [
                ...(prev[userId] || []),
                {
                    tempId, // ID temporal
                    senderId: userId,
                    receiverId,
                    content,
                    status: "pending", // Estado hasta recibir confirmación del WebSocket
                    sentAt: new Date().toISOString(), // Timestamp local
                },
            ],
        }));
    }, [sockets]);

    const markMessagesAsRead = useCallback((userId, contactId) => {
        const socket = sockets[userId];
        if (!socket) {
            console.error(`No se puede marcar como leído: WebSocket para el usuario ${userId} no está conectado`);
            return;
        }

        const message = {
            action: "mark_as_read",
            contactId: contactId,
        };

        socket.send(JSON.stringify(message));
        console.log(`Solicitud para marcar mensajes como leídos enviada desde ${userId} para ${contactId}:`, message);

        // Actualización local para los mensajes recibidos
        setMessages((prev) => {
            const userMessages = prev[userId] || [];
            return {
                ...prev,
                [userId]: userMessages.map((msg) =>
                    msg.senderId === contactId ? { ...msg, status: "read" } : msg
                ),
            };
        });
    }, [sockets]);

    const contextValue = {
        sockets,
        messages,
        sendMessage,
        markMessagesAsRead,
        connectWebSocket,
        disconnectWebSocket,
        isConnected: (userId) => !!sockets[userId],
    };

    return (
        <WebSocketContext.Provider value={contextValue}>
            {children}
        </WebSocketContext.Provider>
    );
};

WebSocketProvider.propTypes = {
    children: PropTypes.node.isRequired,
};