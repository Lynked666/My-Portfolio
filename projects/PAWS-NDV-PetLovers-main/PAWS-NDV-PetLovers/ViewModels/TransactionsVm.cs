
using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.Models.Transactions;

namespace PAWS_NDV_PetLovers.ViewModels
{
    public class TransactionsVm
    {
        public Diagnostics? Diagnostics { get; set; }
        public DiagnosticDetails? DiagnosticDetails { get; set; }
        public Pet? Pets { get; set; }

        // Add a SelectedServiceId field to track the selected service
        public int? SelectedServiceId { get; set; }
        public Purchase? Purchase { get; set; }

        public Billing? Billing { get; set; }

        public PetFollowUps PetFollowUps {get;set;}

        //For displaying name in diagnostic Appointments
        public Appointment? Appointment { get; set; }
        public AppointmentDetails? AppointmentDetails { get; set; }

        //selected services from appointment
        public List<int>? SelectedServices { get; set; }

        //for Appointment Diagnostica
        public Owner? Owner { get; set; }

        //FOR EDIT diagnostic Add-On
        public IEnumerable<PurchaseDetails>? IPurchaseDetails { get; set; }


        public IEnumerable<Purchase>? IPurchase { get; set; }

        //View display
        public IEnumerable<Diagnostics>? IDiagnostics { get; set; }

        //payment part
        public IEnumerable<Services>? Services { get; set; } // Add this property to hold the services list

        //billing display
        public IEnumerable<Billing>? IBilling { get; set; }

        public IEnumerable<Owner> IOwner { get; set; }

        //add on Product

        public IEnumerable<Product>? IProducts { get; set; }

       /* public double? grandTotal { get; set; }

        public double? totalServicePayment { get; set; }*/

        public double? totalPurchasePayment { get; set; }


        //View Components ENUMS and Properties
        public DBoardTab? activeBoardTab { get; set; }
        public BillingTab? activeBillingTab { get; set; }

        public BillingHistoryTab? activeHistoryTab { get; set; }

        public bool? errorMessage { get; set;  }
        public bool? RemoveErrorMessage { get; set; }

        public bool? PaymentErrorMessage { get; set; }
        public string? btnCnl { get; set; }

        public string? AppointType { get;set; }
   
    }
    public enum BillingTab
    {
        Diagnosis,
        Purchase,
        Billing
    };


    public enum DBoardTab
    {
        DBoard_Diagnostics,
        DBoard_Purchase
    };


    public enum BillingHistoryTab
    {
       All,
       Diagnostics,
       Purchase
    };

}


