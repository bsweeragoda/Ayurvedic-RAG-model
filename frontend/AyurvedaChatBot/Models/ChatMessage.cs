namespace AyurBotFrontend.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessageType { get; set; } // "text", "image", "symptoms"
    }

    public class DiseasePrediction
    {
        public string Disease { get; set; }
        public double Confidence { get; set; }
        public string Description { get; set; }
        public string AyurvedicRemedies { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DiseasePrediction Prediction { get; set; }
    }
}