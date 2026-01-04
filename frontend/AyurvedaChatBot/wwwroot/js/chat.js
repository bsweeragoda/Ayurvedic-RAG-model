class AyurBotChat {
    constructor() {
        this.chatMessages = document.getElementById('chatMessages');
        this.messageInput = document.getElementById('messageInput');
        this.sendButton = document.getElementById('sendButton');

        this.initializeEventListeners();
    }

    initializeEventListeners() {
        this.sendButton.addEventListener('click', () => this.sendMessage());

        this.messageInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });
    }

    async sendMessage() {
        const message = this.messageInput.value.trim();
        if (!message) return;

        this.addMessage(message, true);
        this.messageInput.value = '';

        try {
            const response = await fetch('/Chat/SendMessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                // 🔥 MUST MATCH ChatRequest.Question
                body: JSON.stringify({ Question: message })
            });

            if (!response.ok) {
                const err = await response.text();
                throw new Error(err);
            }

            const data = await response.json();
            this.addBotMessage(data.answer);

        } catch (error) {
            this.addBotMessage('Sorry, an error occurred while processing your request.');
            console.error('Chat error:', error);
        }
    }

    addMessage(message, isUser) {
        const div = document.createElement('div');
        div.className = `message ${isUser ? 'user-message' : 'bot-message'}`;
        div.innerHTML = `<strong>${isUser ? 'You' : 'AyurBot'}:</strong> ${message}`;
        this.chatMessages.appendChild(div);
        this.scrollToBottom();
    }

    addBotMessage(message) {
        this.addMessage(message, false);
    }

    scrollToBottom() {
        this.chatMessages.scrollTop = this.chatMessages.scrollHeight;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new AyurBotChat();
});
