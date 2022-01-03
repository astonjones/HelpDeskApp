using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskApp.Models
{
    public partial class Ticket
    {
        [Key]
        public double Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public double PhoneNumber { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        public string ?Status { get; set; }
        [Required]
        public int Urgency { get; set; }
    }
}
