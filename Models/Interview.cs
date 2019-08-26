namespace Lanina.Public.Web.Api.Models
{
    public class Interview
    {
        public int Id { get; set; }                
        public InterviewDate Date { get; set; }
        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; }
        public int? KoplanetReservationId { get; set; }
        public string MeetingRoomName { get; set; }
    }
}
