using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lanina.Public.Web.Api.Models
{
    public class Applicant
    {
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        public string EmailConfirmationKey { get; set; }
        public bool IsEmailConfirmed { get; set; }
        [Required]
        public string MobilePhone { get; set; }
        public string MobilePhoneConfirmationKey { get; set; }
        public bool IsMobilePhoneConfirmed { get; set; }
        public string Linkedin { get; set; }
        public string Github { get; set; }
        public string CoverLetter { get; set; }
        public ApplicantStatus Status { get; set; }
        public List<Interview> Interviews { get; set; }

        public string Flag { get; set; }

        public string RejectReason { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return Name + " " + Surname;
            }
        }
    }
}
