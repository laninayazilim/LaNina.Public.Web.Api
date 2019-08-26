using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.DTOs
{
    public class AddInterviewDatesInput
    {
        [Required]
        public List<DateTime> Dates { get; set; }
    }
}
