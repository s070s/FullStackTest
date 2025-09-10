using Api.Models.BaseClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Api.Models.ChildrenClasses
{
    public class PersonalInformation : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string? FirstName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string? LastName { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(10)]
        public string? ZipCode { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        [Range(0, 500)]
        public double? Weight { get; set; }

        [Range(0, 300)]
        public double? Height { get; set; }

        //Basal Metabolic Rate
        [Range(0, 10000)]
        public double? BMR { get; set; }

        //Body Mass Index
        [Range(0, 100)]
        public double? BMI { get; set; }
        [NotMapped]
        public int? Age => DateOfBirth.HasValue
            ? (int)((DateTime.UtcNow - DateOfBirth.Value).TotalDays / 365.25)
            : null;
    }
}