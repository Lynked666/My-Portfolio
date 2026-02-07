using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.Models.Records;

namespace PAWS_NDV_PetLovers.ViewModels
{
    public class AppointmentVm
    {
        public Appointment? Appointment { get; set; }

        public AppointmentDetails? AppointmentDetails { get; set; }

        public List<Services>? IlistServices { get; set; }

        public List<PetFollowUps> IPetFollowUps { get; set; }

        //for  CREATE View
        public IEnumerable<Owner> IOwner { get; set; }


        //selected services from appointment
        public IEnumerable<Services>? Services { get; set; } // Add this property to hold the services list

        //List for index View

        public List<AppointmentDetails> IAppDetails { get; set; }

        //GROUP FOR INDEX UNIQUE ID
        public List<AppointmentGroup> AppointmentGrouping { get; set; }

        public List<Appointment> IAppointments { get; set; }


        public DateTime? timeHolder { get; set; }

        public List<string> AvailableAM { get; set; }
        public List<string> AvailablePM { get; set; }
        public AppointmentTab activeAppointTab { get; set; }


        //monitoring prperties
        public bool? updated { get; set; }
        public bool? created { get; set; }

        public bool? logical_dateError { get; set; }
        public bool? null_dateError { get; set; }

        public string? Status { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string? SelectType { get; set; }

        public bool? Filtered { get; set; }

        public string? reportType { get; set; }

        public IEnumerable<Appointment> IEAppointment { get; set; }

        public IEnumerable<PetFollowUps> IEPetFollowUps { get; set; }
    }

    public enum AppointmentTab
    {
        booking,
        followUp,
        monitoring
    };


    //grouping technique to display 1 ID distinctly.
    public class AppointmentGroup
    {
        public Appointment Appointment { get; set; }

        public List<AppointmentDetails> Details { get; set; }

        public IEnumerable<Owner>? Owner { get; set; } // for related Owners
    }
}
