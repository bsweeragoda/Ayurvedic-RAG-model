class AyurBotChat {
    constructor() {
        this.chatMessages = document.getElementById('chatMessages');
        this.messageInput = document.getElementById('messageInput');
        this.sendButton = document.getElementById('sendButton');
        this.imageInput = document.getElementById('imageInput');

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

        this.imageInput.addEventListener('change', () => this.sendImage());
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
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ message: message })
            });

            const data = await response.json();
            this.addBotResponse(data);
        } catch (error) {
            this.addMessage('Sorry, I encountered an error. Please try again.', false);
        }
    }

    async sendImage() {
        const file = this.imageInput.files[0];
        if (!file) return;

        const formData = new FormData();
        formData.append('image', file);

        this.addMessage(`Uploaded image: ${file.name}`, true);

        try {
            const response = await fetch('/Chat/SendMessage', {
                method: 'POST',
                body: formData
            });

            const data = await response.json();
            this.addBotResponse(data);
        } catch (error) {
            this.addMessage('Error processing image. Please try again.', false);
        }

        this.imageInput.value = '';
    }

    addMessage(message, isUser) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${isUser ? 'user-message' : 'bot-message'} alert ${isUser ? 'alert-primary' : 'alert-info'}`;
        messageDiv.innerHTML = `<strong>${isUser ? 'You:' : 'Bot:'}</strong> ${message}`;

        this.chatMessages.appendChild(messageDiv);
        this.scrollToBottom();
    }

    addBotResponse(response) {
        if (response.success) {
            let message = response.message;
            if (response.prediction) {
                message += `<br><br><strong>Disease:</strong> ${response.prediction.disease}<br>`;
                message += `<strong>Confidence:</strong> ${(response.prediction.confidence * 100).toFixed(2)}%<br>`;
                message += `<strong>Ayurvedic Advice:</strong> ${response.prediction.ayurvedicRemedies}`;
            }
            this.addMessage(message, false);
        } else {
            this.addMessage(response.message || 'Sorry, I could not process your request.', false);
        }
    }

    scrollToBottom() {
        this.chatMessages.scrollTop = this.chatMessages.scrollHeight;
    }
}

// Initialize chat when page loads
document.addEventListener('DOMContentLoaded', () => {
    new AyurBotChat();
});