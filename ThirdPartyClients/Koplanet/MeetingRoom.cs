using System;
using System.Collections.Generic;

namespace Lanina.Public.Web.Api.ThirdPartyClients.Koplanet
{
    public class MeetingRoom
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Name { get; set; }
        public string PicturePath { get; set; }
        public decimal Price { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<Reservation> Reservations { get; set; }
        public TimeSpan CancellationDuration { get; set; }
    }
}
