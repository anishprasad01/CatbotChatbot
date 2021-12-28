//CancelDialog class
//Used when the used types help or quit to end dialog execution and restart

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CatBotChatBot
{
    public class CancelDialog : ComponentDialog
    {
        private const string CancelMsgText = "Quitting...";

        public CancelDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                switch (text)
                {
                    //random easter eggs
                    case "what is a cat?":
                    case "what's a cat":
                        await innerDc.Context.SendActivityAsync(MessageFactory.Text("A feline"), cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);
                    case "devinfo":
                        await innerDc.Context.SendActivityAsync(MessageFactory.Text("Anish Prasad 2021"), cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);
                    //when the user types on of these, serve them the intro hero card again and then cancel dialogs to they can start over
                    case "help":
                    case "stop":
                    case "?":
                        var card = IntroCard.getCard();
                        var response = MessageFactory.Attachment(card.ToAttachment());
                        await innerDc.Context.SendActivityAsync(response, cancellationToken);
                        return await innerDc.CancelAllDialogsAsync(cancellationToken);
                    //cancels execution
                    case "cancel":
                    case "quit":
                        var cancelMessage = MessageFactory.Text(CancelMsgText, CancelMsgText, InputHints.IgnoringInput);
                        await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);
                        return await innerDc.CancelAllDialogsAsync(cancellationToken);
                }
            }
            return null;
        }
    }
}
