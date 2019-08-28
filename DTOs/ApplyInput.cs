using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class ApplyInput
    {
        [StringLength(25)]
        [Required]
        public string Name { get; set; }

        [StringLength(25)]
        [Required]
        public string Surname { get; set; }

        [StringLength(50)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string MobilePhone { get; set; }

        public string Github { get; set; }

        public string Linkedin { get; set; }

        [StringLength(1000)]
        public string CoverLetter { get; set; }

        [Required]
        public IFormFile Resume { get; set; }

        public string Flag { get; set; }
    }
}
