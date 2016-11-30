using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using BotPlanning.Models;
using System.Collections.Generic;
using BotPlanning.DataModels;

namespace BotPlanning
{
    //BotPlanningevent
    //c0e9b1af-6c84-48d0-a116-a6b4c07bae0f
    //a53kOwp0P1pJFBSYLRePd7Y
    //ngrok http -host-header=rewrite 9000
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
               
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                StateClient stateClient = activity.GetStateClient(); /// get info from user
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                String username = activity.From.Id;
                bool isWeatherRequest = false;
                var userMessage = activity.Text.ToLower();
                string endOutput = "Sorry, I can not understand your request, Please specific with keyword: weather,show plan,clear etc. Type keyword to see all the keyword";
                if (userMessage.Equals("hello") || userMessage.Equals("hi") || userMessage.Equals("sup"))
                {
                    if (userData.GetProperty<bool>("SentGreeting"))
                    {
                        endOutput = "Hello again";
                    }
                    else
                    {
                        endOutput = "Hello there, How can I help you with your planning today?";
                        userData.SetProperty<bool>("SentGreeting", true);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }
                }

                if (userMessage.ToLower().Equals("new timeline"))
                {
                    Timeline timeline = new Timeline()
                    {
                        Anger = 0.5,
                        Contempt = 0.5,
                        Disgust = 0.5,
                        Fear = 0.5,
                        Happiness = 0.5,
                        Neutral = 0.5,
                        Sadness = 0.5,
                        Surprise = 0.5,
                        Date = DateTime.Now
                    };

                    await AzureManager.AzureManagerInstance.AddTimeline(timeline);

                    isWeatherRequest = false;

                    endOutput = "New timeline added [" + timeline.Date + "]";
                }

                if (userMessage.ToLower().Equals("get plan table"))
                {
                    isWeatherRequest = false;
                    List<Plan> planList = await AzureManager.AzureManagerInstance.GetPlanTable() ;
                    endOutput = "";
                    endOutput = "";
                    if (planList.Count > 0)
                    {
                        string timeSchedule = "";
                        foreach (Plan p in planList)
                        {
                            endOutput += "-----------------------------------------";
                            endOutput += "Your plan on [" + p.date + "]\r\n";
                            if (p.time9 != null) { timeSchedule += "\t [@9:00 =>" + p.time9 + "]\r\n"; }
                            if (p.time10 != null) { timeSchedule += "\t [@10:00 =>" + p.time10 + "]\r\n"; }
                            if (p.time11 != null) { timeSchedule += "\t [@11:00 =>" + p.time11 + "]\r\n"; }
                            if (p.time12 != null) { timeSchedule += "\t [@12:00 =>" + p.time12 + "]\r\n"; }
                            if (p.time13 != null) { timeSchedule += "\t [@13:00 =>" + p.time13 + "]\r\n"; }
                            if (p.time14 != null) { timeSchedule += "\t [@14:00 =>" + p.time14 + "]\r\n"; }
                            if (p.time15 != null) { timeSchedule += "\t [@15:00 =>" + p.time15 + "]\r\n"; }
                            if (p.time16 != null) { timeSchedule += "\t [@16:00 =>" + p.time16 + "]\r\n"; }
                            endOutput += timeSchedule;
                            endOutput += "-----------------------------------------";
                        }

                    }
                    else
                    {
                        endOutput = "No plan.";                      
                    }

                }

                if (userMessage.Contains("clear"))
                {
                    endOutput = "user data has been cleared";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                }

                if (userMessage.Equals("keywords") || userMessage.Equals("keyword"))
                {
                    endOutput = "Keywords are : [weather (location)], [show plan] , [addplan (your plan name)], [clear].";
                }

                if (userMessage.Equals("show plan") || userMessage.Equals("showplan")) {

                    List<Plan> planList = await AzureManager.AzureManagerInstance.GetUserPlan(username);
                    endOutput = ""; 
                    if (planList.Count > 0)
                    {
                        string timeSchedule = "";
                        foreach (Plan p in planList)
                        {
                            endOutput += "-----------------------------------------";
                            endOutput += "Your plan on [" + p.date + "]\r\n";
                            if (p.time9 != null ) { timeSchedule += "\t [@9:00 =>" + p.time9 +"]\r\n";  }
                            if (p.time10 != null) { timeSchedule += "\t [@10:00 =>" + p.time10 + "]\r\n"; }
                            if (p.time11 != null ) { timeSchedule += "\t [@11:00 =>" + p.time11 + "]\r\n"; }
                            if (p.time12 != null ) { timeSchedule += "\t [@12:00 =>" + p.time12 + "]\r\n"; }
                            if (p.time13 != null ) { timeSchedule += "\t [@13:00 =>" + p.time13 + "]\r\n"; }
                            if (p.time14 != null ) { timeSchedule += "\t [@14:00 =>" + p.time14 + "]\r\n"; }
                            if (p.time15 != null ) { timeSchedule += "\t [@15:00 =>" + p.time15 + "]\r\n"; }
                            if (p.time16 != null ) { timeSchedule += "\t [@16:00 =>" + p.time16 + "]\r\n"; }
                            endOutput += timeSchedule;
                            endOutput += "-----------------------------------------";
                        }
                     
                    }
                    else {
                        endOutput = "You do not have any plan.";
                        isWeatherRequest = false;
                    }
                }

                if (userMessage.Length >= 8)
                {

                    if (userMessage.Substring(0, 5).Equals("event")) {
                        HttpClient client = new HttpClient();
                        string location = userMessage.Substring(6);
                        string x = await client.GetStringAsync(new Uri("http://api.eventful.com/json/events/search?app_key=2LK4BR6cknDWqWHP&id=20218701&page_size=3&location=" + location));
                        EventObject.RootObject rootObject;
                        rootObject = JsonConvert.DeserializeObject<EventObject.RootObject>(x);
                        EventObject.Events events = rootObject.events ;
                        List<EventObject.Event> eventlist = events.@event;

                        Activity eventReply = activity.CreateReply($"Event at {location}");
                        eventReply.Recipient = activity.From;
                        eventReply.Type = "message";
                        eventReply.Attachments = new List<Attachment>();
                        eventReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        endOutput = "1";
                        foreach (EventObject.Event e in eventlist) {
                            endOutput = "112";
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: e.image.url));
                            List<CardAction> cardButtons = new List<CardAction>();
                            CardAction plButton = new CardAction()
                            {
                                Value = e.url,
                                Type = "openUrl",
                                Title = "More Info"
                            };
                            cardButtons.Add(plButton);
                            plButton = new CardAction()
                            {
                                Value = "add plan "+ e.title,
                                Type = "imBack",
                                Title = "Add to your plan"
                            };
                            cardButtons.Add(plButton);
                            ThumbnailCard plCard = new ThumbnailCard()
                            {
                                Title = e.title,
                                Subtitle = "At "+e.venue_name,
                                Images = cardImages,
                                Buttons = cardButtons
                            };
                            Attachment plAttachment = plCard.ToAttachment();
                            eventReply.Attachments.Add(plAttachment);

                        }

                        await connector.Conversations.SendToConversationAsync(eventReply);
                        return Request.CreateResponse(HttpStatusCode.OK);


                    }

                    else if (userMessage.Substring(0, 7).Equals("weather"))
                    {
                        isWeatherRequest = true;
                        HttpClient client = new HttpClient();
                        string location = userMessage.Substring(8);
                        string x = await client.GetStringAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=" + location + "&units=metric&APPID=440e3d0ee33a977c5e2fff6bc12448ee"));
                        WeatherObject.RootObject rootObject;
                        rootObject = JsonConvert.DeserializeObject<WeatherObject.RootObject>(x);

                        string cityName = rootObject.name;
                        string temp = rootObject.main.temp + "°C";
                        string pressure = rootObject.main.pressure + "hPa";
                        string humidity = rootObject.main.humidity + "%";
                        string wind = rootObject.wind.deg + "°";
                        string icon = rootObject.weather[0].icon;
                        int cityId = rootObject.id;
                        // return our reply to the user
                        Activity weatherReply = activity.CreateReply($"Current weather for {cityName}");
                        weatherReply.Recipient = activity.From;
                        weatherReply.Type = "message";
                        weatherReply.Attachments = new List<Attachment>();
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "http://openweathermap.org/img/w/" + icon + ".png"));

                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "https://openweathermap.org/city/" + cityId,
                            Type = "openUrl",
                            Title = "More Info"
                        };
                        cardButtons.Add(plButton);
                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = cityName + " Weather",
                            Subtitle = "Temperature " + temp + ", pressure " + pressure + ", humidity  " + humidity + ", wind speeds of " + wind,
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        weatherReply.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(weatherReply);
                    }
                    else if (userMessage.Substring(0, 8).Equals("add plan")) {

                        string subject = userMessage.Substring(9);
                        userData.SetProperty<String>("subject", subject);
                        Activity replyToConversation = activity.CreateReply("Planning " + subject);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        List<CardAction> cardButtons = new List<CardAction>();
                        DateTime day = DateTime.Today.AddDays(1);
                        CardAction dayButton = new CardAction()
                        {
                            Value = "on " + day.ToString("dd/MM/yyyy"),
                            Type = "imBack",
                            Title = day.ToString("dd/MM/yyyy")
                        };
                        cardButtons.Add(dayButton);
                        day = DateTime.Today.AddDays(2);
                        dayButton = new CardAction()
                        {
                            Value = "on " + day.ToString("dd/MM/yyyy"),
                            Type = "imBack",
                            Title = day.ToString("dd/MM/yyyy")
                        };
                        cardButtons.Add(dayButton);
                        day = DateTime.Today.AddDays(3);
                        dayButton = new CardAction()
                        {
                            Value = "on " + day.ToString("dd/MM/yyyy"),
                            Type = "imBack",
                            Title = day.ToString("dd/MM/yyyy")
                        };
                        cardButtons.Add(dayButton);
                        day = DateTime.Today.AddDays(4);
                        dayButton = new CardAction()
                        {
                            Value = "on " + day.ToString("dd/MM/yyyy"),
                            Type = "imBack",
                            Title = day.ToString("dd/MM/yyyy")
                        };
                        cardButtons.Add(dayButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = "Available Time",
                            Subtitle = "Choose your date",
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(replyToConversation);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else if (userMessage.Substring(0, 2).Equals("on")) {
                        String datestring = userMessage.Substring(3);

                        List<Plan> planList = await AzureManager.AzureManagerInstance.GetUserPlanOnDate(username, datestring);
                        userData.SetProperty<String>("date", datestring);

                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        ////DateTime date = Convert.ToDateTime(datestring);

                        Activity replyToConversation = activity.CreateReply("Planning time");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        List<CardAction> cardButtons = new List<CardAction>();
                        string[] times = { "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00" };
                        CardAction timeButton;
                        if (planList.Count == 0) { /// Date has no plan (havent been created before)
                            userData.SetProperty<bool>("createnew", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            foreach (string s in times) {
                                timeButton = new CardAction(
                                    value: "at " + s,
                                    type: "imBack",
                                    title: s
                                    );
                                cardButtons.Add(timeButton);
                            }

                        } else
                        { // The selected date have plan in it , so try to figure it out which one is free
                            userData.SetProperty<bool>("createnew", false);
                            userData.SetProperty<String>("planid", planList[0].ID);
                            userData.SetProperty<Plan>("planlist", planList[0]);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            List<string> availble = new List<string>();
                            ///foreach (Plan p in planList) {
                            Plan p = planList[0];
                            if (p.time9 == "" || p.time9 == null) { availble.Add(times[0]); }
                            if (p.time10 == "" || p.time10 == null) { availble.Add(times[1]); }
                            if (p.time11 == "" || p.time11 == null) { availble.Add(times[2]); }
                            if (p.time12 == "" || p.time12 == null) { availble.Add(times[3]); }
                            if (p.time13 == "" || p.time13 == null) { availble.Add(times[4]); }
                            if (p.time14 == "" || p.time14 == null) { availble.Add(times[5]); }
                            if (p.time15 == "" || p.time15 == null) { availble.Add(times[6]); }
                            if (p.time16 == "" || p.time16 == null) { availble.Add(times[7]); }
                            //    }
                            if (availble.Count > 0)
                            {
                                foreach (string s in availble)
                                {
                                    timeButton = new CardAction(
                                        value: "at " + s,
                                        type: "imBack",
                                        title: s
                                        );
                                    cardButtons.Add(timeButton);
                                }
                            }
                            else {
                                string subject = userData.GetProperty<string>("subject");
                                timeButton = new CardAction(
                                    value: "add plan " + subject,
                                    type: "imBack",
                                    title: "There is no availble time for this day, Please select other time."

                                    );
                                cardButtons.Add(timeButton);

                            }
                            availble.Clear();
                        }
                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = "Available Time",
                            Subtitle = "Choose your time",
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);

                        await connector.Conversations.SendToConversationAsync(replyToConversation);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else if (userMessage.Substring(0, 2).Equals("at"))
                    {
                        string time = userMessage.Substring(3);
                        string date = userData.GetProperty<string>("date");
                        bool createNew = userData.GetProperty<bool>("createnew");
                        string subject = userData.GetProperty<string>("subject");
                        string planid = userData.GetProperty<string>("planid");
                        Plan planlist = userData.GetProperty<Plan>("planlist");
                        int index = Int32.Parse(time.Substring(0, 2));
                        string[] subjectList = new string[17];
                        int important = -1;
                        string note = "";
                        for (int i = 9; i < 17; i++) {
                            if (i == index) { subjectList[i] = subject; important = i; note = subject + "-" + i; }
                            else { subjectList[i - 9] = ""; }
                        }
                        if (createNew)
                        {
                            Plan plan = new Plan()
                            {
                                username = username,
                                date = date,
                                time9 = subjectList[9],
                                time10 = subjectList[10],
                                time11 = subjectList[11],
                                time12 = subjectList[12],
                                time13 = subjectList[13],
                                time14 = subjectList[14],
                                time15 = subjectList[15],
                                time16 = subjectList[16]
                            };
                            await AzureManager.AzureManagerInstance.AddPlan(plan);
                            endOutput = "Done adding " + subject + " on " + date + " at: " + time;
                        }
                        else {
                            Plan plan = new Plan();

                            plan.ID = planid;
                            plan.date = date;
                            plan.username = username;
                            plan.time9 = planlist.time9;
                            plan.time10 = planlist.time10;
                            plan.time11 = planlist.time11;
                            plan.time12 = planlist.time12;
                            plan.time13 = planlist.time13;
                            plan.time14 = planlist.time14;
                            plan.time15 = planlist.time15;
                            plan.time16 = planlist.time16;
                            if (subjectList[0] != "")
                            {
                                plan.time9 = subjectList[9]; note += "---9";
                            }
                            else if (subjectList[1] != "")
                            {
                                plan.time10 = subjectList[10]; note += "---10";
                            }
                            else if (subjectList[2] != "")
                            {
                                plan.time11 = subjectList[11]; note += "---11";
                            }
                            else if (subjectList[3] != "")
                            {
                                plan.time12 = subjectList[12]; note += "---12";
                            }
                            else if (subjectList[4] != "")
                            {
                                plan.time13 = subjectList[13]; note += "---13";
                            }
                            else if (subjectList[5] != "")
                            {
                                plan.time14 = subjectList[14]; note += "---14";
                            }
                            else if (subjectList[6] != "")
                            {
                                plan.time15 = subjectList[15]; note += "---115";
                            }
                            else if (subjectList[7] != "")
                            {
                                plan.time16 = subjectList[16]; note += "---16";
                            }

                            //  Plan newplan = plan;

                            await AzureManager.AzureManagerInstance.updatePlan(plan);
                            endOutput = "Done updating Plan " + subject + " on " + date + " at: " + time;
                        }


                    }
                }    
                if (!isWeatherRequest)
                {
                    Activity infoReply = activity.CreateReply(endOutput);
                    await connector.Conversations.ReplyToActivityAsync(infoReply);
                }
                

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}