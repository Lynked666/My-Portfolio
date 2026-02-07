using PAWS_NDV_PetLovers.Models.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class StockAdjustment
    {
        [Key]
        public int stockAdj_Id { get; set; }

        public string? source { get; set; }

        public DateTime date { get; set; }

        public int stock { get; set; }

        [ForeignKey("Product")]
        public int? productId { get; set; }

        [ForeignKey("Billing")]
        public int? billing_Id { get; set; }

        //navigation Property
        public Product? productsNav { get; set; }
        public Billing? billingNav { get; set; }
    }
}
