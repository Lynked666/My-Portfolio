using Microsoft.CodeAnalysis;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.Models.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace PAWS_NDV_PetLovers.Models.Appointments
{
    public class PetFollowUps
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Diagnostics")]
        public int diagnosticsId { get; set; } 

        [ForeignKey("Services")]
        public int serviceId { get; set; }
    
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime date { get; set; }

        public string? status { get; set; }


        //navigation Property

        public Diagnostics? Diagnostics { get; set; }
        public Services? Services { get; set; }
    }
}
