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

            // NOTE: Ensure navigation properties in ExerciseDefinition and Equipment models are set up for this many-to-many relationship.

            // TIP: If you add new relationships or properties to ExerciseDefinition, update this configuration accordingly.

            // EF Core will create the join table "ExerciseEquipments" automatically unless further configuration is needed.

            // CAUTION: Changing table names or relationships here may require a new migration and database update.
        }
    }
}