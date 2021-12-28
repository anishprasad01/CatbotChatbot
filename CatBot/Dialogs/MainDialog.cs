// MainDialog class

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace CatBotChatBot
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger _logger;
        public MainDialog(ILogger<MainDialog> logger, UserState userState)
            : base(nameof(MainDialog))
        {
            _logger = logger;
            
            // Define the main dialog and its related components.
            AddDialog(new CMSDialog(userState));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //invoke the CMS dialog the run through questions to the user
            return await stepContext.BeginDialogAsync(nameof(CMSDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //end the dialog once finished
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
