using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.Models.Records;
using System.Collections;

namespace PAWS_NDV_PetLovers.ViewModels
{
    public class RecordsVm
    {

        //Client
        public Owner Owner { get; set; }
        public Pet Pet { get; set; }

        //Model Pets Ienumerable
        public IEnumerable<Pet> IPets { get; set; }


        //Product Management
        public Category Category { get; set; }

        public Product Product { get; set; }

        public IEnumerable<Product> IProducts { get; set; }

        public Appointment? Appointment { get; set; }


        public IEnumerable<StockAdjustment> IstockAdjustment { get; set; }
        public string? btnCnl { get; set; }
    }
}
