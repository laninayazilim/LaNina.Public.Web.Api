using System;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class InviteApplicantInput
    {
        [Required]
        public string ApplicantKey { get; set; }
        [Required]
        public InvitationType InvitationType { get; set; }
    }

    public enum InvitationType
    {
        FirstInterview,
        SecondInterview
    }
}
