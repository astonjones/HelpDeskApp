using System;
using System.Collections.Generic;

namespace HelpDeskApp.Models
{
    public partial class Ticket
    {
        public double Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Location { get; set; } = null!;
        public double PhoneNumber { get; set; }
        public string Category { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Urgency { get; set; }
    }
}
