using PAWS_NDV_PetLovers.Models.Records;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Appointments
{
    public class Appointment
    {
        [Key]
        public int AppointId { get; set; }

        [ForeignKey("Owners")]
        public int? ownerId { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string? fname { get; set; }


        [Display(Name = "Middle name")]
        public string? mname { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string? lname { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(11)]
        [Display(Name = "Contact #")]
        public string contact { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]

        public DateTime time { get; set; }


        [Display(Name = "Remarks")]
        public string? remarks { get; set; }

        //navigation property

        //This will serve as parameterless Constructor bc of "?" nullable

        public Owner OwnerNav { get; set; }
        public IList<AppointmentDetails>? IAppDetails { get; set; }
    
   
    }
}
