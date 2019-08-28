using Itenso.TimePeriod;
using Lanina.Public.Web.Api.ThirdPartyClients.Google;
using Lanina.Public.Web.Api.ThirdPartyClients.Koplanet;
using Lanina.Public.Web.Api.ThirdPartyClients.Mail;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;

namespace Lanina.Public.Web.Api
{
    public class LaNinaInterviewManager
    {
        public class ReserveMeetingRoomOutput
        {
            public MeetingRoom MeetingRoom { get; set; }
            public int? ReservationId { get; set; }

            public ReserveMeetingRoomOutput()
            {
                ReservationId = null;
                MeetingRoom = null;
            }
        }

        private const string MailSubject = "Full Stack Yazılım Uzmanı Başvurusu Hak.";

        private readonly KoplanetClient _koplanetClient;
        private readonly MailClient _mailClient;
        private readonly GoogleCalendarClient _googleCalendarClient;
        private readonly IHostingEnvironment _env;

        public LaNinaInterviewManager(IHostingEnvironment env, KoplanetClient koplanetClient, MailClient mailClient, GoogleCalendarClient googleCalendarClient)
        {
            _koplanetClient = koplanetClient;
            _mailClient = mailClient;
            _googleCalendarClient = googleCalendarClient;
            _env = env;
        }

        private ReserveMeetingRoomOutput ReserveMeetingRoom(DateTime reservationDateTime, string fullName)
        {
            var retVal = new ReserveMeetingRoomOutput();

            var myRange = new TimeRange(reservationDateTime, reservationDateTime.AddHours(1));

            var bearerToken = _koplanetClient.GetToken();

            //ilgili tarihte odaların kayıtlarını getir
            var meetingRoomData = _koplanetClient.GetMeetingRoomData(
                bearerToken,
                reservationDateTime.Date);

            var availableMeetingRooms = meetingRoomData
           .Where(r => !r.Reservations.Any(t => new TimeRange(reservationDateTime.Date.Add(t.Time), t.Duration).HasInside(myRange)))
           .ToList();

            MeetingRoom reservedMeetingRoom = null;

            foreach (var roomName in _koplanetClient.RoomNamesOrderedByPriority)
            {
                reservedMeetingRoom = availableMeetingRooms.FirstOrDefault(r => r.Name.Contains(roomName));
                if (reservedMeetingRoom != null)
                {
                    var newReservation = _koplanetClient.ReserveMeetingRoom(
                            bearerToken, reservedMeetingRoom.Id, reservationDateTime, $"Interview - {fullName}");

                    if (newReservation != null && newReservation.Id > 0)
                    {
                        retVal.MeetingRoom = reservedMeetingRoom;
                        retVal.ReservationId = newReservation.Id;
                        break;
                    }
                }
            }

            return retVal;
        }

        public void DeleteMeetingRoomReservation(int reservationId)
        {
            var bearerToken = _koplanetClient.GetToken();
            _koplanetClient.DeleteBooking(bearerToken, reservationId);
        }

        public ReserveMeetingRoomOutput ProcessInterview(
            DateTime reservationDateTime,
            string name,
            string surname,
            string phone,
            string email,
            bool reserveMeetingRoom,
            string adminEmail)
        {
            var retVal = new ReserveMeetingRoomOutput();

            var googleCredPath = _env.ContentRootPath + "/google/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user";

            try
            {
                if (reserveMeetingRoom)
                    retVal = ReserveMeetingRoom(reservationDateTime, $"{name} {surname}");

                if (retVal.MeetingRoom == null)
                {
                    SendCouldNotReserveMeetingRoomMail($"{name} {surname}", adminEmail);

                    _googleCalendarClient.CreateEvent(
                    reservationDateTime, "NA!", $"{name} {surname}", phone, email, googleCredPath);

                    return retVal;
                }

                _googleCalendarClient.CreateEvent(
                    reservationDateTime, retVal.MeetingRoom.Name, $"{name} {surname}", phone, email, googleCredPath);

            }
            catch (Exception ex)
            {
                SendExceptionMail(ex.ToString(), adminEmail);
            }

            return retVal;
        }

        public void SendInvitationForFirstInterviewMail(string to, string name, string key, string adminEmail)
        {
            var body = File.ReadAllText(_env.ContentRootPath +
                "/MailTemplates/invitationForFirstInterviewMailTemplate.txt")
                .Replace("{{name}}", name)
                .Replace("{{key}}", key);

            _mailClient.SendMail(to, body, MailSubject, adminEmail, adminEmail);
        }

        public void SendInvitationForSecondInterviewMail(string to, string name, string key, string adminEmail)
        {
            var body = File.ReadAllText(_env.ContentRootPath +
                "/MailTemplates/invitationForSecondInterviewMailTemplate.txt")
                .Replace("{{name}}", name)
                .Replace("{{key}}", key);

            _mailClient.SendMail(to, body, MailSubject, adminEmail, adminEmail);
        }

        public void SendInterviewDateSetMail(string to, string name, DateTime interviewDate, string adminEmail)
        {
            var body = File.ReadAllText(_env.ContentRootPath +
                "/MailTemplates/interviewDateSetMailTemplate.txt")
                .Replace("{{name}}", name)
                .Replace("{{interviewDate}}", interviewDate.ToString("dd.MM.yyyy HH:mm"));

            _mailClient.SendMail(to, body, MailSubject, adminEmail, adminEmail);
        }

        public void SendRejectApplicantMail(string to, string name, string reason)
        {
            var body = File.ReadAllText(_env.ContentRootPath +
                "/MailTemplates/rejectApplicantMailTemplate.txt")
                .Replace("{{name}}", name)
                .Replace("{{reason}}", reason);

            _mailClient.SendMail(to, body, MailSubject);
        }

        public void SendApplicationReceivedMail(string to, string name, string emailConfirmationKey, string adminEmail, string resumePath)
        {
            var body = File.ReadAllText(_env.ContentRootPath +
                "/MailTemplates/applicationReceivedMailTemplate.txt")
                .Replace("{{name}}", name)
                .Replace("{{emailConfirmationKey}}", emailConfirmationKey);

            _mailClient.SendMail(to, body, MailSubject, bcc: adminEmail, filePath: resumePath);
        }

        private void SendCouldNotReserveMeetingRoomMail(string fullName, string adminEmail)
        {
            var text = $"Could not reserve meeting room for {fullName}";
            _mailClient.SendMail(adminEmail, text, text);
        }

        private void SendExceptionMail(string exception, string adminEmail)
        {
            _mailClient.SendMail(adminEmail, exception, "New Exception From Interview Client");
        }

        internal void SendInterviewReminderMail(string to, string name, DateTime interviewDate, string adminEmail )
        {
            var body = File.ReadAllText(_env.ContentRootPath +
                "/MailTemplates/interviewReminderMailTemplate.txt")
                .Replace("{{name}}", name)
                .Replace("{{interviewDate}}", interviewDate.ToString("dd.MM.yyyy HH:mm"));

            _mailClient.SendMail(to, body, MailSubject, adminEmail, adminEmail);
        }
    }
}
