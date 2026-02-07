using PAWS_NDV_PetLovers.Models.Records;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Transactions
{
    public class DiagnosticDetails
    {
        [Key]
        public int diagnosticDet_Id { get; set; }

        [Display(Name = "Details")]
        public string? details { get; set; }


        [ForeignKey("Diagnostics")]
        [Display(Name ="Diagnostic ID")]
        public int diagnosticsId { get; set; }

        [ForeignKey("Services")]
        public int serviceId { get; set; }

        [Required]
        [Display(Name = "Total Service Payment")]
        public double totalServicePayment { get; set; }


        /* public string? vaccineName { get; set; }*/

        /* [Display(Name = "Prescription")]
           public string? prescription { get; set; }*/

        //navigation Property

        public Services? Services { get; set; } 

    }
}