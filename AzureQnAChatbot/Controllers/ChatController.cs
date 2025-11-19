using Microsoft.AspNetCore.Mvc;
using AzureQnAChatbot.Services;
using AzureQnAChatbot.Models;

namespace AzureQnAChatbot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IAzureLanguageService _languageService;

        public ChatController(IAzureLanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpPost("ask")]
        public async Task<ActionResult<QnAResponse>> AskQuestion([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("A kérdés mező kötelező");
            }

            try
            {
                var response = await _languageService.GetAnswerAsync(request.Question);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }
    }

    public class ChatRequest
    {
        public string Question { get; set; } = string.Empty;
        public string? Context { get; set; }
    }
}