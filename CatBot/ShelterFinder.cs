//ShelterFinder class
//This file holds the code used to query the user for their location and the call the Bing Maps API to get a list of nearby shelters where they can adopt a cat
//It is kept here to reduce redundant code

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CatBotChatBot
{
    public class ShelterFinder
    {
        //We will send the user a card with a map image and links to the located shelters
        public static async Task SendShelterCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken, string userAddress)
        {
            //define a separator and a shelter limit
            string separator = "||";
            int shelterSearchThreshold = 3;

            //call FindShelters
            Task<string> getSheltersTaskString = FindShelters(userAddress, shelterSearchThreshold, separator);

            //if there's an issue, like a bad location, return error
            if (((string)getSheltersTaskString.Result).Equals("ERROR"))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Invalid Location"), cancellationToken);
            }
            else
            {
                //split up the string, and loop through the resultant array to build the hero card
                string[] sheltersString = ((string)getSheltersTaskString.Result).Split(separator);

                List<CardAction> buttonList = new List<CardAction>();

                for (int i = 1; i <= shelterSearchThreshold; i++)
                {
                    buttonList.Add(new CardAction(ActionTypes.OpenUrl, $"{i}. {sheltersString[i]}", null, $"{sheltersString[i]}", $"{sheltersString[i]}", $"https://www.bing.com/search?q={sheltersString[i]}"));
                }

                var shelterCard = new HeroCard
                {
                    Title = "Pet Shelter Finder",
                    Text = $"Here are {shelterSearchThreshold} pet shelters near your location. Use the buttons below to look them up and see if they might have the newest member of your family waiting!",
                    Images = new List<CardImage>() { new CardImage(sheltersString[shelterSearchThreshold + 1]) },
                    Buttons = buttonList,
                };
                var response = MessageFactory.Attachment(shelterCard.ToAttachment());
                //send the card to the user
                await stepContext.Context.SendActivityAsync(response, cancellationToken);
            }
        }
        private static async Task<string> FindShelters(string userAddress, int shelterSearchThreshold, string separator)
        {
            //We need to make the address API friendly without spaces
            string userAddressTransformed = userAddress.Replace(" ", "%20");
            using (var httpClient = new HttpClient())
            {
                try
                {
                    //query the Locations API to get the coordinates for the user's location and extract the coordinates from the returned JSON
                    HttpResponseMessage userLocationResponse = await httpClient.GetAsync($"http://dev.virtualearth.net/REST/v1/Locations/{userAddressTransformed}?o=json&key={key}");
                    userLocationResponse.EnsureSuccessStatusCode();
                    string userLocationResponseBody = await userLocationResponse.Content.ReadAsStringAsync();
                    dynamic userLocation = JsonConvert.DeserializeObject(userLocationResponseBody);
                    double xCoord = userLocation.resourceSets[0].resources[0].point.coordinates[0];
                    double yCoord = userLocation.resourceSets[0].resources[0].point.coordinates[1];

                    //query the Local Search API to get a list of nearby shelters and extract their coordinates and names from the returned JSON
                    HttpResponseMessage sheltersResponse = await httpClient.GetAsync($"https://dev.virtualearth.net/REST/v1/LocalSearch/?query=cat%20shelter&userLocation={xCoord},{yCoord}&key={key}");
                    sheltersResponse.EnsureSuccessStatusCode();
                    string sheltersResponseBody = await sheltersResponse.Content.ReadAsStringAsync();
                    dynamic shelterLocations = JsonConvert.DeserializeObject(sheltersResponseBody);

                    string returnString = "";
                    string pushPinString = "";

                    for (int i = 0; i < shelterSearchThreshold; i++)
                    {
                        if (i == shelterSearchThreshold - 1)
                        {
                            pushPinString += $"pp={shelterLocations.resourceSets[0].resources[i].point.coordinates[0]},{shelterLocations.resourceSets[0].resources[i].point.coordinates[1]};;{i + 1}";
                            returnString += $"{separator}{shelterLocations.resourceSets[0].resources[i].name}";
                        }
                        else
                        {
                            pushPinString += $"pp={shelterLocations.resourceSets[0].resources[i].point.coordinates[0]},{shelterLocations.resourceSets[0].resources[i].point.coordinates[1]};;{i + 1}&";
                            returnString += $"{separator}{shelterLocations.resourceSets[0].resources[i].name}";
                        }
                    }

                    //use the constructed string to build the url to get the map image from the Imagery API
                    string mapQueryUrl = $"https://dev.virtualearth.net/REST/v1/Imagery/Map/AerialWithLabels?{pushPinString}&mapLayer=TrafficFlow&key={key}";

                    //concatenate everything into one string to return to the calling method
                    //this is a bit of a hack, I do intend to fix this, but my time was limited
                    returnString += $"{separator}{mapQueryUrl}";
                    return returnString;
                }
                catch(Exception e) {
                    return "ERROR";
                }
            }
                
        }
    }
}
