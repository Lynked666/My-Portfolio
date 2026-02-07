using PAWS_NDV_PetLovers.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class Services
    {
        [Key]
        [Display(Name = "Service Id")]
        public int serviceId { get; set; }

        [Required]
        [Display(Name ="Service Name")]
        [StringLength(80)]
        public string? serviceName {  get; set; }

        [Required (ErrorMessage = "Please enter a value greater than 0")]
        [Display(Name = "Service Charge")]
        public double serviceCharge { get; set; }

        [Display(Name = "Laboratory Test")]
        public string? serviceType { get; set; }

        [Display(Name = "Available for Follow-up visit?")]
        public bool followUp { get; set; }

        public string? status { get; set; }

    
    }
}
