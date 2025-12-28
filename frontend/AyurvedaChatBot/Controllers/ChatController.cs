using Microsoft.AspNetCore.Mvc;
using AyurBotFrontend.Models;
using AyurBotFrontend.Services;

namespace AyurBotFrontend.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, IFormFile image)
        {
            try
            {
                ApiResponse response;

                if (image != null)
                {
                    // Handle image analysis
                    response = await _chatService.SendImageToAnalysis(image);
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    // Handle text message
                    // Check if message is in Sinhala
                    if (ContainsSinhala(message))
                    {
                        var englishText = await _chatService.TranslateToEnglish(message);
                        response = await _chatService.SendTextToAnalysis(englishText);

                        // Translate response back to Sinhala
                        if (response.Success && !string.IsNullOrEmpty(response.Message))
                        {
                            response.Message = await _chatService.TranslateToSinhala(response.Message);
                        }
                    }
                    else
                    {
                        response = await _chatService.SendTextToAnalysis(message);
                    }
                }
                else
                {
                    response = new ApiResponse { Success = false, Message = "Please provide either text or image." };
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        private bool ContainsSinhala(string text)
        {
            // Simple check for Sinhala Unicode range
            return text.Any(c => c >= 0x0D80 && c <= 0x0DFF);
        }
    }
}