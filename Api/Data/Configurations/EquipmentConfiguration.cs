using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.ToTable("Equipments");
            builder.HasKey(e => e.Id);

            //Many-to-many: Equipment <-> ExerciseDefinition
            builder
                .HasMany(e => e.Exercises)
                .WithMany(ed => ed.Equipments)
                .UsingEntity(j => j.ToTable("ExerciseDefinitionEquipment"));

            // NOTE: Ensure navigation properties in Equipment and ExerciseDefinition models are set up for this many-to-many relationship.

            // TIP: If you add new relationships or properties to Equipment, update this configuration accordingly.

            // EF Core will create the join table "ExerciseDefinitionEquipment" automatically unless further configuration is needed.

            // CAUTION: Changing table names or relationships here may require a new migration and database update.
        }
    }
}