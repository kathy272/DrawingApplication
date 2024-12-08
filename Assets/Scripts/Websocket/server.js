const WebSocket = require('ws');

// Create a WebSocket server
const wss = new WebSocket.Server({ host: '0.0.0.0', port: 8080 });

// Store connected clients
let sender = null;
let receiver = null;

wss.on('connection', (ws) => {
    console.log('New client connected');

    if (!receiver) {
        receiver = ws;
        console.log('Receiver connected');
       // receiver.send('You are the receiver');
    }
    else if (!sender) {
        sender = ws;
        console.log('Sender connected');
       // sender.send('You are the sender');
    }
    // Handle incoming messages
    ws.on('message', (message) => {
        console.log('Received:', message);
        const messageStr = message.toString();
        if (!messageStr.trim()) {
            console.log('Empty message received, skipping');
            return;
        }
        try {
            const parsedMessage = JSON.parse(messageStr);
            console.log('Parsed message:', parsedMessage);
        if (ws === sender) {
            // Send data from sender to receiver
            if (receiver) {
                console.log('Forwarding message to receiver');
                receiver.send(JSON.stringify(parsedMessage));            } else {
                console.log('Receiver not connected yet');
            }
        }

        if (ws === receiver) {
            if(sender){
            console.log('Receiver message:', parsedMessage);
            sender.send(JSON.stringify(parsedMessage))
        }}

    } catch (err) {
        console.error('Error parsing message:', err);
    }
    });

    // Handle client disconnection
    ws.on('close', () => {
        if (ws === sender) {
            sender = null;
            console.log('Sender disconnected');
        } else if (ws === receiver) {
            receiver = null;
            console.log('Receiver disconnected');
        }
    });
});

console.log('WebSocket server running on ws://localhost:8080');
