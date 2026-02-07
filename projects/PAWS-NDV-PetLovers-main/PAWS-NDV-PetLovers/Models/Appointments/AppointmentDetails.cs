using PAWS_NDV_PetLovers.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAWS_NDV_PetLovers.Models.Appointments
{
    public class AppointmentDetails
    {
        [Key]
        [Display(Name = "Details Id")]
        public int AppDetailsId { get; set; }

        [ForeignKey("Appointment")]
        [Display(Name = "Appointment Id")]
        public int AppointId { get; set; }

        [ForeignKey("Services")]
        public int? serviceID { get; set; }

        // Navigation Property
        public Appointment? Appointment { get; set; }
        public Services? Services { get; set; } // Corrected to single object
    }
}
