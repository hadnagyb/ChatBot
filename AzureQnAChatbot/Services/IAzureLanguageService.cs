using AzureQnAChatbot.Models;

namespace AzureQnAChatbot.Services
{
    public interface IAzureLanguageService
    {
        Task<QnAResponse> GetAnswerAsync(string question);
    }
}