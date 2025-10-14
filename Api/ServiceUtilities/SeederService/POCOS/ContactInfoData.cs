namespace Api.ServiceUtilities.SeederService
{
    // POCO for contact info seed data loaded from contactinfo.json
    public class ContactInfoData
    {
        public required string[] EmailDomains { get; set; }
    }
}