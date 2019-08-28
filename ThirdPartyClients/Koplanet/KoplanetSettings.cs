using System.Collections.Generic;

namespace Lanina.Public.Web.Api.ThirdPartyClients.Koplanet
{
    public class KoplanetSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<int> ParticipantIds { get; set; }
        public List<string> RoomNamesOrderedByPriority { get; set; }
    }
}
