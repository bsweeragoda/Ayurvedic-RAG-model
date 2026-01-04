using AyurBotFrontend.Models;

namespace AyurBotFrontend.Services
{
    public interface IChatService
    {
        // Task<ApiResponse> SendTextToAnalysis(string symptoms);
        // Task<ApiResponse> SendImageToAnalysis(IFormFile image);
        // Task<string> TranslateToEnglish(string sinhalaText);
        // Task<string> TranslateToSinhala(string englishText);
        Task<ApiResponse> SendToRag(string query);
    }
}