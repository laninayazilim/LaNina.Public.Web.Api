using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lanina.Public.Web.Api.Models
{
    public class ApplyToken
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
