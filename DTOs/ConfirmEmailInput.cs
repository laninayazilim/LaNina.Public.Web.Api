using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class ConfirmEmailInput
    {
        [Required]
        public string EmailConfirmationKey { get; set; }
    }
}
