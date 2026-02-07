using PAWS_NDV_PetLovers.Models.Appointments;
using System.ComponentModel.DataAnnotations;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class Owner
    {
        [Key]
        [Display (Name ="Client Id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string? fname { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string? lname { get; set; }

        [Display(Name = "Middle name")]
        public string? mname { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string? gender { get; set; }

        [Required]
        [MaxLength(11)]
        [Display(Name = "Contact number")]
        [DataType(DataType.PhoneNumber)]
        public string? contact { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string? email { get; set; }

        [Required]
        [Display(Name = "Home Address")]
        public string? address { get; set; }

        [Display(Name = "Registered Date")]
        [DataType(DataType.Date)]
        public DateTime? registeredDate { get; set; }

        [Display(Name = "Last Update")]
        [DataType(DataType.Date)]
        public DateTime? lastUpdate { get; set; }

        //Navigation Property - used to tell entity framework that 2 entities have relationship
        public ICollection<Pet> Pets { get; set; } // Navigation property for related pets
    }
}
