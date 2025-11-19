namespace AzureQnAChatbot.Models
{
    public class QnAResponse
    {
        public QnAAnswer[] Answers { get; set; } = Array.Empty<QnAAnswer>();
        public QnAAnswer? BestAnswer => Answers.Length > 0 ? Answers[0] : null;
    }

    public class QnAAnswer
    {
        public string[] Questions { get; set; } = Array.Empty<string>();
        public string Answer { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string? Source { get; set; }
    }
}