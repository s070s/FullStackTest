using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class ExerciseDefinitionConfiguration : IEntityTypeConfiguration<ExerciseDefinition>
    {
        public void Configure(EntityTypeBuilder<ExerciseDefinition> builder)
        {
            builder.ToTable("ExerciseDefinitions");
            builder.HasKey(ed => ed.Id);
            
            // Many-to-many: ExerciseDefinition <-> Equipment
            builder
                .HasMany(ed => ed.Equipments)
                .WithMany(e => e.Exercises)
                .UsingEntity(j => j.ToTable("ExerciseEquipments"));
        }
    }
}