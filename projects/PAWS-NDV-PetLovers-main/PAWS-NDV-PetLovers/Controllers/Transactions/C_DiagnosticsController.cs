using Microsoft.AspNetCore.Mvc;
using PAWS_NDV_PetLovers.Models.Transactions;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.Controllers.Appointments;
using System.Linq;

namespace PAWS_NDV_PetLovers.Controllers.Transactions
{
    [ServiceFilter(typeof(AuthFilter))]
    public class C_DiagnosticsController : Controller
    {

        private readonly PAWS_NDV_PetLoversContext _context;

        public C_DiagnosticsController(PAWS_NDV_PetLoversContext context)
        {
            this._context = context;
        }

/*   This function was transfer to the Billing named CreateDiagnosis
        [HttpGet]
        public async Task<IActionResult> Create(int? petId)
        {

            //id handling if null
            if (petId == null)
            {
                return NotFound();
            }

            var firstPet = await _context.Pets.Include(p => p.owner).FirstOrDefaultAsync(p => p.id == petId);



            if (firstPet == null)
            {
                return NotFound();
            }
          

            var appointment = await _context.Appointments
                 .Include(a => a.IAppDetails)
                 .ThenInclude(a => a.Services)
                 .FirstOrDefaultAsync(a => a.ownerId== firstPet.ownerId && string.IsNullOrEmpty(a.remarks));

            if (appointment != null)
            {
                //vm instantiationn
                TransactionsVm vm = new TransactionsVm
                {
                    Pets = firstPet,
                    //assigning fk column PetId based on routed id
                    Diagnostics =  new Diagnostics { petId = (int)petId } ,
                    Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync(),
                    Appointment = appointment
                };

                return View(vm);
            }

            //vm instantiationn
            TransactionsVm tVm = new TransactionsVm
            {
                Pets = firstPet,
                //assigning fk column PetId based on routed id
                Diagnostics = new Diagnostics { petId = (int)petId },
                Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync()

            };

            return View(tVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? ownerId, [Bind("Diagnostics")] TransactionsVm tvm)
        {
            var diagnostics = tvm.Diagnostics;

            // Fetch owner and appointment details
            var owner = await _context.Owners.FindAsync(ownerId);
            var appointment = await _context.Appointments
                .Include(a => a.IAppDetails) // Include appointment details for comparison
                .FirstOrDefaultAsync(a => a.ownerId == ownerId && string.IsNullOrEmpty(a.remarks));

            if (appointment != null)
            {
                // Update remarks and save changes
                appointment.remarks = "Completed";
                _context.Update(appointment);
            }

            // Filter out diagnostics follow-up entries where the date is null or uninitialized
            diagnostics.IPetFollowUps = diagnostics.IPetFollowUps
                .Where(f => f.date != DateTime.MinValue)
                .ToList();

            if (diagnostics.IPetFollowUps.Count > 0)
            {
                // Assign the diagnosticsId and validate serviceId for each follow-up entry
                foreach (var followUp in diagnostics.IPetFollowUps)
                {

                    // Check if serviceId is valid before proceeding
                    var service = await _context.Services.FindAsync(followUp.serviceId);
                   
                    if (service == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Service ID selected for the follow-up.");
                        //need viewmodel here for incase
                        return View(tvm); // Return the view if there's an error with serviceId
                    }

                    _context.Update(followUp); // Update the follow-up entry with the IDs
                }
            }

            // Add the new Diagnostics entity and save changes
            _context.Add(diagnostics);
            await _context.SaveChangesAsync();

            // Redirect to the Billing page
            return RedirectToAction("Index", "Billing");
        }*/
        /*ownerfk must not be fk to avoid duplicate insertion of OWNER FK*/
        #region == Abandoned methods (for source) ==

        //Functions

        /*[HttpGet]
        public async Task<IActionResult> DiagnosAppointment(int Appointmentid, string fname, string lname, string contact, List<int> serviceId)
        {

            var getOwner = await _context.Owners.Include(o => o.Pets)
           .FirstOrDefaultAsync(o => o.fname == fname && o.lname == lname && o.contact == contact);


            TransactionsVm tvm = new TransactionsVm
            {
                Owner = getOwner,
                SelectedServices = serviceId,
                Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync() //status can be updated when service is deleted
            };

            return View(tvm);
        }*/
        /// <summary>

        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="tvm"></param>
        /// <returns></returns>
        //Owner Diagnosis
        /*[HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            // Extract only the date part (ignoring time)

            if (id == null)
            {
                return NotFound();
            }


            //fetched the data in database
            var firstAsync = await _context.Diagnostics
                .Include(d => d.pet)
                .ThenInclude(d => d.owner)
                .Include(d => d.IdiagnosticDetails)
                .ThenInclude(dd => dd.Services)
                .Include(d => d.PurchaseNav)
                .ThenInclude(d => d.purchaseDetails)
                .ThenInclude(d => d.product)
                .ThenInclude(d => d.category)
                .FirstOrDefaultAsync(d => d.diagnostic_Id == id);

            var Purchase = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .ThenInclude(p => p.product)
                .ThenInclude(p => p.category)
                .FirstOrDefaultAsync(p => p.diagnosisId_holder == id);

            //VIEW Cart

            var PurchaseItems = await _context.PurchaseDetails
                .Include(p => p.product)
                .ThenInclude(p => p.category)
                .Include(p => p.Purchase)
                .Where(p => p.Purchase.diagnosisId_holder == id && string.IsNullOrEmpty(p.Purchase.status))
                .ToListAsync();

            // Calculate total price
            double purchasePayment = PurchaseItems.Sum(item => (double)item.product.sellingPrice * (int)item.quantity);



            TransactionsVm tvm = new TransactionsVm
            {
                Diagnostics = firstAsync,

                Purchase = Purchase,

                Services = await _context.Services.ToListAsync(),

                IProducts = await _context.Products.Include(p => p.category).ToListAsync(),

                IPurchaseDetails = PurchaseItems,
                totalPurchasePayment = purchasePayment

            };

            return View(tvm);

        }*/

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Diagnostics")] TransactionsVm tvm)
        {
            double totalPurchase = 0;

            // Extract the Diagnostics object from the view model
            var diagnostics = tvm.Diagnostics;


            // Referenced from view for Update detail Results
            foreach (var detail in diagnostics.IdiagnosticDetails)
            {
                // FindAsync the DetailID
                var existingDetail = await _context.DiagnosticDetails.FindAsync(detail.diagnosticDet_Id);

                // Update the diagnosis details
                if (existingDetail != null)
                {
                    existingDetail.details = detail.details;

                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiagnosticExists(diagnostics.diagnostic_Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            //for Diagnostic Bill Display

            var diagnostic = await _context.Diagnostics
               .Include(d => d.PurchaseNav)
                   .ThenInclude(p => p.purchaseDetails)
                   .ThenInclude(p => p.product)
               .Include(d => d.pet)
                   .ThenInclude(p => p.owner)
               .Include(d => d.IdiagnosticDetails)
                   .ThenInclude(dd => dd.Services)
               .Where(d => string.IsNullOrEmpty(d.status))
               .ToListAsync();

            var purchase = await _context.Purchases
                .Include(p => p.purchaseDetails).ToListAsync();

            TransactionsVm vm = new TransactionsVm
            {
                IDiagnostics = diagnostic,
                IPurchase = purchase
            };

            return RedirectToAction("Index", "BillingDashboard"); // Redirect to a different action after saving
        }*/

        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> CreatePurchase(Purchase purchase)
         {
             // Check if the purchase object or purchase details are null or empty
             if (purchase == null || purchase.purchaseDetails == null || !purchase.purchaseDetails.Any())
             {
                 // Fetch the original data for the view
                 var diagnostics = await _context.Diagnostics
                     .Include(d => d.pet)
                     .ThenInclude(p => p.owner)
                     .Include(d => d.IdiagnosticDetails)
                     .ThenInclude(dd => dd.Services)
                     .Include(d => d.PurchaseNav)
                     .ThenInclude(d => d.purchaseDetails)
                     .ThenInclude(pd => pd.product)
                     .ThenInclude(p => p.category)
                     .FirstOrDefaultAsync(d => d.diagnostic_Id == purchase.diagnosisId_holder);

                 var purchaseItems = await _context.PurchaseDetails
                     .Include(p => p.product)
                     .ThenInclude(p => p.category)
                     .Include(p => p.Purchase)
                     .Where(p => p.Purchase.diagnosisId_holder == purchase.diagnosisId_holder && string.IsNullOrEmpty(p.Purchase.status))
                     .ToListAsync();

                 // Calculate the total price again
                 double purchasePayment = purchaseItems.Sum(item => (double)item.product.sellingPrice * (int)item.quantity);

                 // Reload the view model with the original data and an error message
                 var viewModel = new TransactionsVm
                 {
                     Diagnostics = diagnostics,
                     Purchase = await _context.Purchases
                         .Include(p => p.purchaseDetails)
                         .ThenInclude(p => p.product)
                         .ThenInclude(p => p.category)
                         .FirstOrDefaultAsync(p => p.diagnosisId_holder == purchase.diagnosisId_holder),
                     Services = await _context.Services.ToListAsync(),
                     IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                     IPurchaseDetails = purchaseItems,
                     totalPurchasePayment = purchasePayment

                 };

                 // Add an error message to the ViewData to display on the view
                 ViewData["ErrorMessage"] = "No products were selected. Please select at least one product to proceed.";

                 // Return the Edit view with the original data and error message
                 return View("Edit", viewModel);
             }

             // Storage for calculated totals
             var Purchased = new List<double>();

             // Selected Product Ids
             var ProductIds = purchase.purchaseDetails
                 .Select(P => P.productId)
                 .Distinct()
                 .ToList();

             // Get products from the database based on selected product Ids
             var Products = await _context.Products.Where(p => ProductIds.Contains(p.id)).ToListAsync();

             // Convert the list of products to a dictionary for easy lookup
             var ProductDictionary = Products.ToDictionary(p => p.id);

             // Process each purchase detail and update total price
             foreach (var details in purchase.purchaseDetails)
             {
                 var price = details.sellingPrice;
                 var quantity = details.quantity;

                 // Calculate total price (if quantity is 0, it defaults to 1)
                 var total = price * (quantity > 0 ? quantity : 1);

                 // Add total to the list of totals
                 Purchased.Add(total);

                 // Update product quantity in the database
                 if (ProductDictionary.TryGetValue(details.productId, out var product))
                 {
                     product.quantity -= quantity;
                 }
             }

             // Sum all the product totals and update the purchase
           *//*  purchase.totalProductPayment = Purchased.Sum();*//*

             // Add the purchase and update the products in the database
             _context.Add(purchase);
             _context.UpdateRange(Products);

             // Attempt to save changes
             try
             {
                 await _context.SaveChangesAsync();
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!PurchaseExists(purchase.diagnosisId_holder))
                 {
                     // Fetch the original data for the view
                     var diagnostics = await _context.Diagnostics
                         .Include(d => d.pet)
                         .ThenInclude(p => p.owner)
                         .Include(d => d.IdiagnosticDetails)
                         .ThenInclude(dd => dd.Services)
                         .Include(d => d.PurchaseNav)
                         .ThenInclude(d => d.purchaseDetails)
                         .ThenInclude(pd => pd.product)
                         .ThenInclude(p => p.category)
                         .FirstOrDefaultAsync(d => d.diagnostic_Id == purchase.diagnosisId_holder);

                     var purchaseItems = await _context.PurchaseDetails
                         .Include(p => p.product)
                         .ThenInclude(p => p.category)
                         .Include(p => p.Purchase)
                         .Where(p => p.Purchase.diagnosisId_holder == purchase.diagnosisId_holder && string.IsNullOrEmpty(p.Purchase.status))
                         .ToListAsync();

                     // Calculate the total price again
                     double purchasePayment = purchaseItems.Sum(item => (double)item.product.sellingPrice * (int)item.quantity);

                     // Reload the view model with the original data
                     var viewModel = new TransactionsVm
                     {
                         Diagnostics = diagnostics,
                         Purchase = await _context.Purchases
                             .Include(p => p.purchaseDetails)
                             .ThenInclude(p => p.product)
                             .ThenInclude(p => p.category)
                             .FirstOrDefaultAsync(p => p.diagnosisId_holder == purchase.diagnosisId_holder),
                         Services = await _context.Services.ToListAsync(),
                         IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                         IPurchaseDetails = purchaseItems,
                         totalPurchasePayment = purchasePayment
                     };

                     ViewData["ErrorMessage"] = "The purchase does not exist.";
                     return View("Edit", viewModel);
                 }
                 else
                 {
                     throw;
                 }
             }

             // Fetch the updated diagnostic data and purchase details for the Edit view
             var updatedDiagnostics = await _context.Diagnostics
                 .Include(d => d.pet)
                 .ThenInclude(p => p.owner)
                 .Include(d => d.IdiagnosticDetails)
                 .ThenInclude(dd => dd.Services)
                 .Include(d => d.PurchaseNav)
                 .ThenInclude(d => d.purchaseDetails)
                 .ThenInclude(pd => pd.product)
                 .ThenInclude(p => p.category)
                 .FirstOrDefaultAsync(d => d.diagnostic_Id == purchase.diagnosisId_holder);

             var updatedPurchaseItems = await _context.PurchaseDetails
                 .Include(p => p.product)
                 .ThenInclude(p => p.category)
                 .Include(p => p.Purchase)
                 .Where(p => p.Purchase.diagnosisId_holder == purchase.diagnosisId_holder && string.IsNullOrEmpty(p.Purchase.status))
                 .ToListAsync();

             // Calculate the total price again
             double updatedPurchasePayment = updatedPurchaseItems.Sum(item => (double)item.product.sellingPrice * (int)item.quantity);

             // Reload the view model with the updated data
             var updatedTvm = new TransactionsVm
             {
                 Diagnostics = updatedDiagnostics,
                 Purchase = await _context.Purchases
                     .Include(p => p.purchaseDetails)
                     .ThenInclude(p => p.product)
                     .ThenInclude(p => p.category)
                     .FirstOrDefaultAsync(p => p.diagnosisId_holder == purchase.diagnosisId_holder),
                 Services = await _context.Services.ToListAsync(),
                 IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                 IPurchaseDetails = updatedPurchaseItems,
                 totalPurchasePayment = updatedPurchasePayment
             };

             // Return the updated Edit view with the current data
             return View("Edit", updatedTvm);


         }
         private bool DiagnosticExists(int id)
         {
             return _context.Diagnostics.Any(e => e.diagnostic_Id == id);
         }

         private bool PurchaseExists(int? id)
         {
             return _context.Purchases.Any(p => p.purchaseId == id);
         }
 */
        #endregion
    }
}