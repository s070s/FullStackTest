
namespace Api.ServiceUtilities.SeederService
{
    // POCO for trainer-related seed data loaded from trainer.json
    public class TrainerData
    {
        public required string[] TrainerPrefixes { get; set; }
        public required string[] TrainerMiddleNames { get; set; }
        public required string[] TrainerSuffixes { get; set; }
        public required TrainerBioData TrainerBio { get; set; }
    }

    // POCO for trainer bio details loaded from trainer.json
    public class TrainerBioData
    {
        public required string[] Adjectives { get; set; }
        public required string[] Fillers { get; set; }
        public required string[] Templates { get; set; }
    }
}