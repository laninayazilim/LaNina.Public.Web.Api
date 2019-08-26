using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Lanina.Public.Web.Api.ThirdPartyClients.Koplanet
{
    public class KoplanetClient
    {
        //INFO: Metodları Postman generate etti        

        private readonly KoplanetSettings _settings;
        public KoplanetClient(KoplanetSettings settings)
        {
            _settings = settings;
        }

        public IList<string> RoomNamesOrderedByPriority {
            get {
                return _settings.RoomNamesOrderedByPriority.AsReadOnly();
            }
        }

        /// <summary>
        /// Verilen tarihte maslaktaki odaların bilgilerini getirmek için
        /// </summary>
        public List<MeetingRoom> GetMeetingRoomData(string bearerToken, DateTime date)
        {
            var client = new RestClient("https://api.kolektifhouse.co/meetingroomreservation/availablemeetingroom");

            var request = new RestRequest(Method.POST);
            AddCommonHeaders(request, "https://koplanet.kolektifhouse.co/dashboard/reservations/newbooking");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "55");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", "api.kolektifhouse.co");                        
            request.AddHeader("Authorization", $"Bearer {bearerToken}");
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");
            request.AddHeader("Accept-Language", "tr-TR");
            request.AddHeader("Referer", "https://koplanet.kolektifhouse.co/dashboard/reservations/newbooking");            
            request.AddParameter("undefined", "{\"locationId\":5,\"date\":\""+date.ToString("o")+"+03:00\"}", ParameterType.RequestBody);

            var response =  client.Execute<List<MeetingRoom>>(request);

            return JsonConvert.DeserializeObject<List<MeetingRoom>>(response.Content);
        }

        /// <summary>
        /// Verilen bilgiler ile koplanette 1sa rezervasyon oluşturur
        /// </summary>
        public NewReservation ReserveMeetingRoom(string bearerToken, int meetingRoomId, DateTime date, string title)
        {
            var client = new RestClient("https://api.kolektifhouse.co/meetingRoomReservation");

            var request = new RestRequest(Method.POST);
            AddCommonHeaders(request, "https://koplanet.kolektifhouse.co/dashboard/reservations/newbooking");
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");
            request.AddHeader("Accept-Language", "tr-TR");            
            request.AddHeader("Authorization", $"Bearer {bearerToken}");
            request.AddParameter("undefined", 
                "{\"participantIds\":["+string.Join(",",_settings.ParticipantIds)+"],\"title\":\""+title+"\",\"note\":\"Interview notes\",\"meetingRoomId\":"+meetingRoomId.ToString()+",\"duration\":\"01:00:00\",\"date\":\""+date.ToString("s")+"\",\"status\":\"Active\"}", ParameterType.RequestBody);

            var response = client.Execute(request);

            return JsonConvert.DeserializeObject<NewReservation>(response.Content);
        }

        public string GetToken()
        {
            var client = new RestClient("https://api.kolektifhouse.co/token");

            var request = new RestRequest(Method.POST);
            AddCommonHeaders(request, "https://koplanet.kolektifhouse.co/auth/SignIn");
            request.AddHeader("Host", "api.kolektifhouse.co");            
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");            
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");           
            request.AddHeader("Accept-Language", "en-US");
            request.AddHeader("Content-Length", "146");
            request.AddHeader("Connection", "keep-alive");
            request.AddParameter("text/plain", $"loading=false&username={_settings.Email}&password={_settings.Password}&errors%5BemailValidation%5D=false&errors%5BsignIn%5D=false&grant_type=password", ParameterType.RequestBody);

            var response = client.Execute(request);

            return JsonConvert.DeserializeObject<TokenResponse>(response.Content).Access_Token;
        }

        public void DeleteBooking(int reservationId)
        {
            var client = new RestClient($"https://api.kolektifhouse.co/meetingRoomReservation/{reservationId}");

            var request = new RestRequest(Method.DELETE);
            AddCommonHeaders(request, "https://koplanet.kolektifhouse.co/dashboard/reservations/myreservations");            
            request.AddHeader("Accept-Language", "en-US");
            
            IRestResponse response = client.Execute(request);
        }

        private void AddCommonHeaders(RestRequest request, string referer)
        {
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Origin", "https://koplanet.kolektifhouse.co");
            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.80 Safari/537.36");
            request.AddHeader("Referer", referer);
        }
    }
}
