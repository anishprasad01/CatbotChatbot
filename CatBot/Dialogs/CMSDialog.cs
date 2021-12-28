// CMSDialog Class
// This class holds all of the user questions code
//At each step, the results are cached in the UserInfoCache property

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace CatBotChatBot
{
    public class CMSDialog : CancelDialog
    {
        private readonly IStatePropertyAccessor<UserInfoCache> _userProfileAccessor;

        public CMSDialog(UserState userState)
            : base(nameof(CMSDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserInfoCache>("UserProfile");

            // Define the main dialog and its related components.
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NameStepAsync,
                ActivityLevelStepAsync,
                SoundLevelStepAsync,
                HabitatStepAsync,
                KidsStepAsync,
                PetsStepAsync,
                AllergenStepAsync,
                FurPatternStepAsync,
                FurColorStepAsync,
                HairLengthStepAsync,
                SizeStepAsync,
                SummaryStepAsync,
                ConfirmStepAsync,
                GetUserLocationStepAsync,
                LinkOutStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
  
        //get the user's name
        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Welcome to Pawsibilities - The Cat Match System!"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("I'm going to ask you some questions about what your ideal pet cat might be and show you some of the important considerations to make. Then I'll provide you some suggestions for how to learn more."), cancellationToken);

            stepContext.Values["CurrentCat"] = new Cat();
            string message = "Let's get started with a simple one. What is your name?";
            var promptMessage = MessageFactory.Text(message, message, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //process the user's name and ask about activity level
        private async Task<DialogTurnResult> ActivityLevelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = stepContext.Result.ToString();
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hey there {(string)stepContext.Values["name"]}! It's nice to meet you."));
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Now to the questions about cats! How active would you prefer your cat to be?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Very Active", "Moderately Active", "Mildly Active" }),
                    RetryPrompt = MessageFactory.Text("Please click or type one of the provided options.")
                }, cancellationToken);
        }

        //process activity and ask about sound
        private async Task<DialogTurnResult> SoundLevelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.ActivityLevel = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("How often would you like your cat to meow?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Frequently", "Regularly", "Rarely" }),
                }, cancellationToken);
        }

        //process sound and ask about habitat
        private async Task<DialogTurnResult> HabitatStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.SoundLevel = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Do you want an indoor or an outdoor cat?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Indoor", "Outdoor" }),
                }, cancellationToken);
        }

        //process habitat and ask abut kids
        private async Task<DialogTurnResult> KidsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.Habitat = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Do you have children 8 years of age or younger?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
                }, cancellationToken);
        }
        
        //process kids and ask about pets
        private async Task<DialogTurnResult> PetsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.HasKids = ((FoundChoice)stepContext.Result).Value.Equals("Yes");
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Do you have other pets?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
                }, cancellationToken);
        }

        //process pets and ask about allergies
        private async Task<DialogTurnResult> AllergenStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.HasPets = ((FoundChoice)stepContext.Result).Value.Equals("Yes");
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Are you allergic to cats?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
                }, cancellationToken);
        }

        //process allergies and ask about fur pattern
        private async Task<DialogTurnResult> FurPatternStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.IsHypoallergenic = ((FoundChoice)stepContext.Result).Value.Equals("Yes");
            string message = "Cats come in all sorts of fur patterns. What type of fur pattern are you interested in?";
            var promptMessage = MessageFactory.Text(message, message, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //process fur pattern and ask about fur color
        private async Task<DialogTurnResult> FurColorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.FurPattern = (string)stepContext.Result;
            string message = "What fur color would you be interested in? (This could even be your favorite color, but keep in mind that cats don't come in neon purple!)";
            var promptMessage = MessageFactory.Text(message, message, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //process fur color and ask about hair length
        private async Task<DialogTurnResult> HairLengthStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.FurColor = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("How long would you like your cat's hair to be?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Long", "Moderate", "Short" }),
                }, cancellationToken);
        }

        //process hair length and ask about size
        private async Task<DialogTurnResult> SizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.HairLength = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("How big would your ideal cat be?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Big", "Medium", "Small" }),
                }, cancellationToken);
        }

        //summarize all the previously collected information for the user
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CurrentCat = (Cat)stepContext.Values["CurrentCat"];
            CurrentCat.Size = ((FoundChoice)stepContext.Result).Value;
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserInfoCache(), cancellationToken);
            userProfile.Name = (string)stepContext.Values["name"];
            userProfile.CurrentCat = CurrentCat;
            userProfile.CatBuilds.Add(CurrentCat);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Let's summarize your choices."));

            var message = $"Your preferred cat is a {CurrentCat.Size.ToLower()} sized {CurrentCat.Habitat.ToLower()} cat that is {CurrentCat.ActivityLevel.ToLower()} and speaks {CurrentCat.SoundLevel.ToLower()}.";

            if (CurrentCat.HasKids)
            {
                message += $" You have young children";
            }
            else 
            {
                message += $" You do not have young children";
            }

            if (CurrentCat.HasPets)
            {
                message += $" and you have other pets.";
            }
            else
            {
                message += $" and you do not have other pets.";
            }

            if (CurrentCat.IsHypoallergenic)
            {
                message += $" Since you're allergic to cats, you'd like one that is hypoallergenic.";
            }
            else
            {
                message += $" Since you're not allergic to cats, a hypoallergenic breed isn't as important.";
            }


            message += $" Lastly, you'd prefer a cat that has {CurrentCat.FurColor.ToLower()} {CurrentCat.FurPattern.ToLower()} fur with {CurrentCat.HairLength.ToLower()} length hair.";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Was that correct?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
                }, cancellationToken);
        
        }

        //ask if the collected info is correct. if the user says no, restart the dialog
        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = (FoundChoice)stepContext.Result;
            var done = choice.Value == "Yes";
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserInfoCache(), cancellationToken);
            
            if (done)
            {
                return await stepContext.NextAsync();
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Oh no! Let's start again."));
                userProfile.CatBuilds.RemoveAt(userProfile.CatBuilds.Count-1);
                return await stepContext.ReplaceDialogAsync(nameof(CMSDialog), null, cancellationToken);
            }
        }

        //run the process to get the user's location and show them nearby shelters
        private async Task<DialogTurnResult> GetUserLocationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string message = "Let's find you a shelter nearby where you might meet you next best friend! What is your location? You can enter a city, or a specific address.";
            var promptMessage = MessageFactory.Text(message, message, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //show the user a final hero card with links out to information that they provided during the questions
        private async Task<DialogTurnResult> LinkOutStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await ShelterFinder.SendShelterCardAsync(stepContext, cancellationToken, (string)stepContext.Result);
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserInfoCache(), cancellationToken);
            Cat currCat = userProfile.CurrentCat;

            List<CardAction> cards = new List<CardAction>()
                {
                    //TODO: SEARCH FEATURES?
                    new CardAction(ActionTypes.OpenUrl, $"Learn about {currCat.FurPattern.ToLower()} cats", null, $"{currCat.FurPattern.ToLower()}", $"{currCat.FurPattern.ToLower()}", $"https://www.bing.com/search?q={currCat.FurPattern}%20cats"),
                    new CardAction(ActionTypes.OpenUrl, $"Learn about {currCat.FurColor.ToLower()} cats", null, $"{currCat.FurColor.ToLower()}", $"{currCat.FurColor.ToLower()}", $"https://www.bing.com/search?q={currCat.FurColor}%20cats"),
                    new CardAction(ActionTypes.OpenUrl, $"Learn about {currCat.ActivityLevel.ToLower()} cats", null, $"{currCat.ActivityLevel.ToLower()}", $"{currCat.ActivityLevel.ToLower()}", $"https://www.bing.com/search?q={currCat.ActivityLevel}%20cat"),
                    new CardAction(ActionTypes.OpenUrl, $"Learn about {currCat.Habitat.ToLower()} cats", null, $"{currCat.Habitat.ToLower()}", $"{currCat.Habitat.ToLower()}", $"https://www.bing.com/search?q={currCat.Habitat}%20cat"),
                    new CardAction(ActionTypes.OpenUrl, $"Learn about {currCat.Size.ToLower()} sized cats", null, $"{currCat.Size.ToLower()}", $"{currCat.Size.ToLower()}", $"https://www.bing.com/search?q={currCat.Size}%20size%20cat"),
                };

            if (currCat.HasKids) {
                cards.Add(new CardAction(ActionTypes.OpenUrl, $"Learn about cats that are good with young children", null, $"Learn about cats that are good with young children", $"Learn about cats that are good with young children", $"https://www.bing.com/search?q=cats%20that%20are%20good%20with%20young%20kids"));
            }

            if (currCat.IsHypoallergenic) {
                cards.Add(new CardAction(ActionTypes.OpenUrl, $"Learn about hypoallergenic cats", null, $"hypoallergenic", $"hypoallergenic", $"https://www.bing.com/search?q=hypoallergenic%20cat"));
            }

            if (currCat.HasPets)
            {
                cards.Add(new CardAction(ActionTypes.OpenUrl, $"Learn about cats that are good with other pets", null, $"other pets", $"other pets", $"https://www.bing.com/search?q=cats%20that%20are%20good%20with%20other%20pets"));
            }

            var card = new HeroCard
            {
                Title = "Thanks for using the CMS!",
                Text = @"Try some of these links to learn more about cats based on your selections.",
                Buttons = cards,
            };
            var response = MessageFactory.Attachment(card.ToAttachment());
            await stepContext.Context.SendActivityAsync(response, cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
