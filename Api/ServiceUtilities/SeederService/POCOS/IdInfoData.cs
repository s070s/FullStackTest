using System.Collections.Generic;

namespace Api.ServiceUtilities.SeederService
{
    // POCO for identity info seed data loaded from idinfo.json
    public class IdInfoData
    {
        public required string[] LastNames { get; set; }
        public required string[] Countries { get; set; }
        public required Dictionary<string, string[]> CitiesByCountry { get; set; }
        public required string[] States { get; set; }
        public required Dictionary<string, string[]> StreetNamesByCountry { get; set; }
    }
}