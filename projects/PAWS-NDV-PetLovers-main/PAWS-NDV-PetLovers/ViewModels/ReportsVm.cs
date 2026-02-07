using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.Models.Transactions;

namespace PAWS_NDV_PetLovers.ViewModels
{
    public class ReportsVm
    {
        public Appointment Appointment { get; set; }

        public Product Product { get; set; }

        public string? Status { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string SelectType { get; set; }

        public bool Filtered {  get; set; }

        public string reportType { get; set; }

        public bool? logical_dateError { get; set; }
        public bool? null_dateError { get; set; }

        public IEnumerable<Appointment> IAppointment { get; set; }

        public IEnumerable<PetFollowUps> IPetFollowUps { get; set; }
        

        public IEnumerable<Product> IProducts { get; set; }

        public IEnumerable<Billing>IBilling { get; set; }

        public ReportTab activeReportTab { get; set; }
    }


    public enum ReportTab
    {
     /*   stockAdjust,*/
        stockLevel,
        transacSum
    }
}
