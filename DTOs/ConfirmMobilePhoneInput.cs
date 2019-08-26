using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class ConfirmMobilePhoneInput
    {
        [Required]
        public string MobilePhoneConfirmationKey { get; set; }
    }
}
