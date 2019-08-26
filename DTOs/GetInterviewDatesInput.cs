using System;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class GetInterviewDatesInput
    {
        [Required]
        public Guid ApplicantId { get; set; }
    }
}
