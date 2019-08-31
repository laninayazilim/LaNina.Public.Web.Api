using System;
using System.ComponentModel.DataAnnotations;

namespace Lanina.Public.Web.Api.Models
{
    public class InterviewDate
    {
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public Interview Interview { get; set; }        
        public int? InterviewId { get; set; }
    }
}
