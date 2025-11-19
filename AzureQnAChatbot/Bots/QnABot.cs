using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using AzureQnAChatbot.Services;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureQnAChatbot.Bots
{
    public class QnABot : ActivityHandler
    {
        private readonly IAzureLanguageService _languageService;

        public QnABot(IAzureLanguageService languageService)
        {
            _languageService = languageService;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userQuestion = turnContext.Activity.Text;
            
            if (string.IsNullOrWhiteSpace(userQuestion))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Kérlek, adj meg egy kérdést!"), cancellationToken);
                return;
            }

            // Get answer from Azure Language Service
            var response = await _languageService.GetAnswerAsync(userQuestion);

            if (response.BestAnswer != null && response.BestAnswer.Confidence > 0.3)
            {
                var replyText = new StringBuilder();
                replyText.AppendLine(response.BestAnswer.Answer);
                
                // Add confidence score for transparency
                replyText.AppendLine($"\n_(Bizalom: {response.BestAnswer.Confidence:P1})_");
                
                // If there are alternative answers, show them
                if (response.Answers.Length > 1)
                {
                    replyText.AppendLine("\n**Egyéb lehetséges válaszok:**");
                    foreach (var altAnswer in response.Answers.Skip(1).Take(2))
                    {
                        if (altAnswer.Confidence > 0.1)
                        {
                            replyText.AppendLine($"• {altAnswer.Answer} _(Bizalom: {altAnswer.Confidence:P1})_");
                        }
                    }
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText.ToString()), cancellationToken);
            }
            else
            {
                var noAnswerText = "Sajnos nem találtam pontos választ a kérdésedre. " +
                                 "Próbáld meg másképp fogalmazni a kérdést, vagy érdeklődj más témában.";
                await turnContext.SendActivityAsync(MessageFactory.Text(noAnswerText), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeMessage = "Üdvözöllek a QnA Chatbot-ban! 🎯\n\n" +
                                       "Tegyél fel nekem kérdéseket, és igyekszem a legjobb válaszokat megadni " +
                                       "az Azure Language Service segítségével.\n\n" +
                                       "Példa kérdések:\n" +
                                       "• Mik a nyitvatartási rendszer?\n" +
                                       "• Hogyan tudok rendelni?\n" +
                                       "• Mik a szállítási feltételek?";
                    
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeMessage), cancellationToken);
                }
            }
        }
    }
}