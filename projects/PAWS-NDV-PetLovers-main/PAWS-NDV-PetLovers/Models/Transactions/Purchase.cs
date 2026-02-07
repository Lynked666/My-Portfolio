using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Transactions
{
    public class Purchase
    {

        [Key]
        [Display(Name = "Purchace ID")]
        public int purchaseId { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime? date { get; set; }

        [Display(Name = "Status")]
        public string? status { get; set; }


        /*[Display(Name = "Total Payment")]
        public double? totalProductPayment { get; set; }*/

        public int? diagnosisId_holder { get; set; } //container sa diagnostics add on

        [Display(Name ="Customer Name")]
        public string? customerName { get; set; }

        //Navigation Property
        public IList<PurchaseDetails> purchaseDetails { get; set; }

    }
}
