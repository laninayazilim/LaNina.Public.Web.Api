using System;
using System.Collections.Generic;

namespace Lanina.Public.Web.Api.DTOs
{
    public class InterviewDto
    {
        public string ApplicantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Github { get; set; }
        public string Linkedin { get; set; }
        public string InterviewDate { get; set; }
        public string MeetingRoomName { get; set; }
        public int? KoplanetReservationId { get; set; }
    }
    public class GetInterviewsOutput
    {
        public List<InterviewDto> Interviews { get; set; }
    }
}
