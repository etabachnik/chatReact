import React, { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const NewChat = () => {
    const [name, setName] = useState('');
    const [room, setRoom] = useState('');
    const [message, setMessage] = useState('');
    const [participants, setParticipants] = useState([]);
    const [messages, setMessages] = useState([]);

    const hubConnectionRef = useRef(null);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('chat')
            .build();

        hubConnectionRef.current = connection;

        connection.start().then(() => {
            console.log('Connected to the chat server');
        });

        return () => {
            connection.stop().then(() => {
                console.log('Disconnected from the chat server');
            });
        };
    }, []);

    const handleNameChange = (e) => {
        setName(e.target.value);
    };

    const handleRoomChange = (e) => {
        setRoom(e.target.value);
    };

    const handleSendMessage = () => {
        if (message) {
            hubConnectionRef.current.invoke('SendMessage', room, message);
            setMessage('');
        }
    };

    const handleJoinRoom = () => {
        hubConnectionRef.current.invoke('Join', room);
    };

    const handleLeaveRoom = () => {
        hubConnectionRef.current.invoke('Leave', room);
        setRoom('');
        setMessages([]);
    };

    useEffect(() => {
        hubConnectionRef.current.on('ReceiveMessage', (user, receivedMessage) => {
            setMessages((prevMessages) => [...prevMessages, { user, message: receivedMessage }]);
        });

        hubConnectionRef.current.on('Login', (user) => {
            setParticipants((prevParticipants) => [...prevParticipants, user]);
        });

        hubConnectionRef.current.on('Logout', (user) => {
            setParticipants((prevParticipants) =>
                prevParticipants.filter((participant) => participant !== user)
            );
        });
    }, []);

    return (
        <div>
            <div>
                <input type="text" value={name} onChange={handleNameChange} placeholder="Enter your name" />
            </div>
            <div>
                <input type="text" value={room} onChange={handleRoomChange} placeholder="Enter room name" />
                <button onClick={handleJoinRoom}>Join Room</button>
                <button onClick={handleLeaveRoom}>Leave Room</button>
            </div>
            <div>
                <h3>Participants:</h3>
                <ul>
                    {participants.map((participant, index) => (
                        <li key={index}>{participant}</li>
                    ))}
                </ul>
            </div>
            <div>
                <h3>Messages:</h3>
                <ul>
                    {messages.map((msg, index) => (
                        <li key={index}>
                            <strong>{msg.user}:</strong> {msg.message}
                        </li>
                    ))}
                </ul>
            </div>
            <div>
                <input
                    type="text"
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder="Enter your message"
                />
                <button onClick={handleSendMessage}>Send</button>
            </div>
        </div>
    );
};

export default NewChat;
