using System.Text;
using Newtonsoft.Json;
using AyurBotFrontend.Models;

namespace AyurBotFrontend.Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // public async Task<ApiResponse> SendTextToAnalysis(string symptoms)
        // {
        //     try
        //     {
        //         // Call Student A's API for text analysis
        //         var apiUrl = _configuration["ApiEndpoints:TextAnalysis"];
        //         var content = new StringContent(
        //             JsonConvert.SerializeObject(new { text = symptoms }),
        //             Encoding.UTF8,
        //             "application/json"
        //         );

        //         var response = await _httpClient.PostAsync(apiUrl, content);
        //         var responseContent = await response.Content.ReadAsStringAsync();

        //         return JsonConvert.DeserializeObject<ApiResponse>(responseContent);
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ApiResponse
        //         {
        //             Success = false,
        //             Message = $"Error: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ApiResponse> SendImageToAnalysis(IFormFile image)
        // {
        //     try
        //     {
        //         // Call your CNN model API for image analysis
        //         var apiUrl = _configuration["ApiEndpoints:ImageAnalysis"];

        //         using var formData = new MultipartFormDataContent();
        //         using var stream = image.OpenReadStream();
        //         using var content = new StreamContent(stream);

        //         formData.Add(content, "image", image.FileName);

        //         var response = await _httpClient.PostAsync(apiUrl, formData);
        //         var responseContent = await response.Content.ReadAsStringAsync();

        //         return JsonConvert.DeserializeObject<ApiResponse>(responseContent);
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ApiResponse
        //         {
        //             Success = false,
        //             Message = $"Error: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<string> TranslateToEnglish(string sinhalaText)
        // {
        //     // Implement Google Translate API call
        //     // This is a placeholder - you'll need to implement actual translation
        //     return sinhalaText; // For now, return as is
        // }

        // public async Task<string> TranslateToSinhala(string englishText)
        // {
        //     // Implement Google Translate API call
        //     return englishText; // For now, return as is
        // }

        public async Task<ApiResponse> SendToRag(string query)
        {
            var payload = new
            {
                query = query,
                k = 5
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:8000/rag/query",
                payload
            );

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "RAG API call failed"
                };
            }

            var json = await response.Content.ReadFromJsonAsync<RagApiResponse>();

            return new ApiResponse
            {
                Success = true,
                Message = json.Answer
            };
        }

        private class RagApiResponse
        {
            public bool Success { get; set; }
            public string Answer { get; set; }
            public List<string> Sources { get; set; }
        }

    }
}
