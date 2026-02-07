using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PAWS_NDV_PetLovers.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Transactions
{
    public class Billing
    {
        [Key]
        public int billingId { get; set; }

        [ForeignKey("Diagnostics")]
        public int? DiagnosticsId { get; set; }

        [ForeignKey("Purchases")]
        public int? PurchaseId { get; set; }

        [DataType(DataType.Date)]
        public DateTime date { get;set; }

        [Display(Name ="Grand Total")]
        public double? grandTotal { get; set; }

        [Display(Name = "Cash Receive")]
        public double? cashReceive { get; set; }

        [Display(Name = "Change Amount")]
        public double? changeAmount { get; set; }


        //navigation property

        public Diagnostics? diagnostics { get; set; }

        public Purchase? purchase { get; set; }


        public IList<StockAdjustment>? stockAdjustments { get; set; }

    }
}
