using System;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class SetInterviewDateInput
    {
        [Required]
        public string ApplicantKey { get; set; }
        [Required]
        public string InterviewDateKey { get; set; }
        [Required]
        public DateTime InterviewDate { get; set; }
    }
}
