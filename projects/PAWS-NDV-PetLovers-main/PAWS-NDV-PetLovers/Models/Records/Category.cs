using System.ComponentModel.DataAnnotations;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class Category
    {
        [Key]
        [Display(Name ="Category Id")]
        public int id { get; set; }

        [Required(ErrorMessage ="Name is required")]
        [Display(Name = "Category Name")]
        public string? categoryName { get; set; }

        [Required(ErrorMessage ="Description is required")]
        [Display(Name = "Description")]
        public string? description { get; set;}

        [Display(Name = "Registered Date")]
        [DataType(DataType.Date)]
        public DateTime? registeredDate { get; set; }
     
        [Display(Name = "Last Update")]
        [DataType(DataType.Date)]
        public DateTime? lastUpdate { get; set; }

        //navigation property 12m
        public ICollection<Product> Products { get; set; }


    }
}
