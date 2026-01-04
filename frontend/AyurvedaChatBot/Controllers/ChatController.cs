using Microsoft.AspNetCore.Mvc;
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

        // Load chat UI
        public IActionResult Index()
        {
            return View();
        }

        // Receive message from frontend JS
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new
                {
                    answer = "Please enter a valid message."
                });
            }

            var response = await _chatService.SendToRag(request.Question);

            if (!response.Success)
            {
                return StatusCode(500, new
                {
                    answer = "Failed to process your request."
                });
            }

            return Json(new
            {
                answer = response.Message
            });
        }
    }

    // DTO for JSON binding
    public class ChatRequest
    {
        public string Question { get; set; }
    }
}
