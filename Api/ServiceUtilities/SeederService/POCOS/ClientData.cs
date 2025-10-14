
namespace Api.ServiceUtilities.SeederService
{
    // POCO for client-related seed data loaded from client.json

    public class ClientData
    {
        public required string[] ClientPrefixes { get; set; }
        public required string[] ClientMiddleNames { get; set; }
        public required string[] ClientSuffixes { get; set; }
        public required Dictionary<string, string[]> ClientBios { get; set; }
    }
}