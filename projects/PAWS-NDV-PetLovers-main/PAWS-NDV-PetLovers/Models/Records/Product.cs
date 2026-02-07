using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class Product
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name ="Product Name")]
        public string? productName { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double? sellingPrice { get; set; }

        [Required]
        [Display(Name = "Supplier Price")]
        public double? supplierPrice { get; set; }

        [Required]
        [Display(Name = "Qty On Hand")]
        public int? quantity { get; set; }

        [Required]
        [Display(Name ="Registered Date")]
        [DataType(DataType.Date)]
        public DateTime? registeredDate { get; set; }

        [Display(Name = "Last Update")]
        [DataType(DataType.Date)]
        public DateTime? lastUpdate { get; set; }


       
        [ForeignKey("Category")]
        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        //navigation property
        public Category? category { get; set; }

        public IList<StockAdjustment>? stockAdjustmentNav { get; set; }

        [NotMapped] // This prevents it from being mapped to a database column
        public int? addQnty { get; set; }
     
    }
}
