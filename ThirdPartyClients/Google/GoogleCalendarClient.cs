using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Threading;

namespace Lanina.Public.Web.Api.ThirdPartyClients.Google
{
    public class GoogleCalendarClient
    {
        private readonly GoogleCalendarSettings _settings;

        public GoogleCalendarClient(GoogleCalendarSettings settings)
        {
            _settings = settings;
        }

        public void CreateEvent(DateTime date, string roomName, string name, string phone, string email, string googleCredPath)
        {
            var myEventStart = new EventDateTime();
            myEventStart.DateTime = date;
            myEventStart.TimeZone = _settings.TimeZone;

            var myEventEnd = new EventDateTime();
            myEventEnd.DateTime = date.AddHours(1);
            myEventEnd.TimeZone = _settings.TimeZone;
            var reminders = new Event.RemindersData()
            {
                UseDefault = false,
                Overrides = new EventReminder[] {
                            new EventReminder() { Method = "email", Minutes = 2 * 60 },
                            new EventReminder() { Method = "popup", Minutes = 60 }
                }
            };



            var myEvent = new Event();
            myEvent.Summary = $"Interview - {name}";
            myEvent.Description = $"{name}, {email}, {phone}";
            myEvent.Start = myEventStart;
            myEvent.End = myEventEnd;
            myEvent.Location = $"Kolektif House - Maslak, {roomName}";
            myEvent.Reminders = reminders;
            
            //File.AppendAllText("googleCalendar.txt", JsonConvert.SerializeObject(myEvent));

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets {
                    ClientId = _settings.ClientId,
                    ClientSecret = _settings.ClientSecret,
                },
                new[] { CalendarService.Scope.Calendar },
                "user",
                CancellationToken.None, new FileDataStore(googleCredPath)).Result;

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _settings.ApplicationName,
            });
            
            EventsResource.InsertRequest request = service.Events.Insert(myEvent, _settings.CalendarId);
            Event createdEvent = request.Execute();
        }
    }
}
