using System;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class RejectApplicantInput
    {
        [Required]
        public string ApplicantKey { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}