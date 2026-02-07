using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class Pet
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        [Display(Name = "Pet name")]
        public string? petName { get; set; }

        [Required]
        [Display(Name = "Specie")]
        public string? species { get; set; }

        [Required]
        [Display(Name = "Breed")]
        public string? breed { get; set; }

        [Required]
        [Display(Name = "Birth date")]
        [DataType(DataType.Date)]
        public DateTime? bdate { get; set; }

        [Required]
        public int? age { get; set; }

        [Required]
        [Display(Name = "Color")]
        public string? color { get; set; }

        [Required]
        [MaxLength(6)]
        [Display(Name = "Gender")]
        public string? gender { get; set; }

        [Display(Name = "Last Update")]
        [DataType(DataType.Date)]
        public DateTime? lastUpdate { get; set; }

        [Required]
        [Display(Name = "Registered Date")]
        [DataType(DataType.Date)]
        public DateTime? registeredDate { get; set; }

        [ForeignKey("Owner")]
        public int ownerId { get; set; }

        //navigation property
        public Owner? owner { get; set; }
    }


}
