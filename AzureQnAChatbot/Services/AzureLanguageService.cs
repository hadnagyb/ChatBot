using Azure;
using Azure.AI.Language.QuestionAnswering;
using AzureQnAChatbot.Models;
using Microsoft.Extensions.Configuration;

namespace AzureQnAChatbot.Services
{
    public class AzureLanguageService : IAzureLanguageService
    {
        private readonly QuestionAnsweringClient _client;
        private readonly QuestionAnsweringProject _project;

        public AzureLanguageService(IConfiguration configuration)
        {
            var endpoint = configuration["AzureLanguageService:Endpoint"] ?? 
                          throw new ArgumentNullException("AzureLanguageService:Endpoint is missing");
            var key = configuration["AzureLanguageService:Key"] ?? 
                     throw new ArgumentNullException("AzureLanguageService:Key is missing");
            var projectName = configuration["AzureLanguageService:ProjectName"] ?? 
                             throw new ArgumentNullException("AzureLanguageService:ProjectName is missing");
            var deploymentName = configuration["AzureLanguageService:DeploymentName"] ?? 
                               throw new ArgumentNullException("AzureLanguageService:DeploymentName is missing");

            var credential = new AzureKeyCredential(key);
            _client = new QuestionAnsweringClient(new Uri(endpoint), credential);
            _project = new QuestionAnsweringProject(projectName, deploymentName);
        }

        public async Task<QnAResponse> GetAnswerAsync(string question)
        {
            return await GetAnswerAsync(question, null);
        }

        public async Task<QnAResponse> GetAnswerAsync(string question, string? context)
        {
            var options = new AnswersOptions
            {
                ConfidenceThreshold = 0.3,
                Size = 3,
                ShortAnswerOptions = new ShortAnswerOptions
                {
                    ConfidenceThreshold = 0.3,
                    Size = 1
                }
            };

            AnswersResult response;
            
            if (string.IsNullOrEmpty(context))
            {
                response = await _client.GetAnswersAsync(question, _project, options);
            }
            else
            {
                // For context-based queries, use different approach
                var request = new
                {
                    question = question,
                    context = context
                };
                
                // Simple implementation - you might need to adjust this based on your needs
                response = await _client.GetAnswersAsync(question, _project, options);
            }

            var answers = response.Answers.Select(a => new QnAAnswer
            {
                Answer = a.Answer,
                Confidence = a.Confidence ?? 0,
                Source = a.Source,
                Questions = a.Questions?.ToArray() ?? Array.Empty<string>()
            }).ToArray();

            return new QnAResponse
            {
                Answers = answers
            };
        }
    }
}