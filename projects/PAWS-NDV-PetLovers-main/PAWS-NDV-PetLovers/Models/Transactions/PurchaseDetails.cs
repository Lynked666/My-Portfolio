using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PAWS_NDV_PetLovers.Models.Records;

namespace PAWS_NDV_PetLovers.Models.Transactions
{
    public class PurchaseDetails
    {
        [Key]
        public int purchaseDet_Id { get; set; }

        [ForeignKey("PurchaseFk")]
        public int purchaseId { get; set; }


        [ForeignKey("Product")]
        public int productId { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int quantity { get; set; }

        [Required]
        [Display(Name = "Selling Price")]
        public double sellingPrice { get; set; }

        //Navigation Property
        public Product? product { get; set; } 

        public Purchase? Purchase { get; set; }
    }
}
