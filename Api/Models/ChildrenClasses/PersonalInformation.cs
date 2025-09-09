using Api.Models.BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;
namespace Api.Models.ChildrenClasses
{
    public class PersonalInformation : BaseEntity
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        //Basal Metabolic Rate
        public double? BMR { get; set; }
        //Body Mass Index
        public double? BMI { get; set; }   
        [NotMapped]
        public int? Age => DateOfBirth.HasValue
            ? (int)((DateTime.UtcNow - DateOfBirth.Value).TotalDays / 365.25)
            : null;
    }
}