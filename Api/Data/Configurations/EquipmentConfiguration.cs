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
        }
    }
}