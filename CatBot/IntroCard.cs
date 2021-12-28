//IntroCard class
//Stores the intro card to avoid redundant code. could be refactored and expanded upon to add more cards
//Image Credit: https://images3.alphacoders.com/738/thumb-1920-738230.jpg

using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace CatBotChatBot
{
    public class IntroCard
    {
        public static HeroCard getCard()
        {
            return new HeroCard
            {
                Title = "Welcome to the Catbot Chatbot!",
                Text = @" This bot aims to help with everything from looking up cat pictures to helping you find find a new feline friend. Here are some things you can try to get started. Type help at any time to see this menu again.",
                Images = new List<CardImage>() { new CardImage("https://images3.alphacoders.com/738/thumb-1920-738230.jpg") },
                Buttons = new List<CardAction>()
                    {
                        new CardAction(ActionTypes.OpenUrl, "Cat Pictures", null, "Cat Pictures", "Cat Pictures", "https://www.bing.com/images/search?q=cats&qs=n&form=QBILPG&sp=-1&pq=cats&sc=8-4&cvid=7CF74B8795604E5EB289F6641ABDB133&first=1&tsc=ImageBasicHover"),
                        new CardAction(ActionTypes.OpenUrl, "Cat Videos", null, "Cat Videos", "Cat Videos", "https://www.bing.com/videos/search?q=cats&qs=n&form=QBFVBS&sp=-1&pq=cats&sc=8-4&sk=&cvid=219A7559B2AC4B78BFCB8DF99D32F393"),
                        new CardAction(ActionTypes.PostBack, "Pawsibilities - The Cat Match System", null, "Pawsibilities - Cat Match System (CMS)", "Pawsibilities - Cat Match System (CMS)", "Pawsibilities - Cat Match System (CMS)"),
                        new CardAction(ActionTypes.OpenUrl, "Find a Pet Shelter Near You", null, "Pet Shelter", "Pet Shelter", "https://www.bing.com/maps?q=pet+shelters+near+me"),
                    }
            };
        }
    }
}
