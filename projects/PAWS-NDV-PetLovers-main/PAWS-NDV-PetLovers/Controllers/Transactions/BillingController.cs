using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.Models.Transactions;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Transactions
{
    [ServiceFilter(typeof(AuthFilter))]
    public class BillingController : Controller
    {
        private PAWS_NDV_PetLoversContext _context;
        public BillingController(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public IActionResult Index(TransactionsVm vcm)

        {
            if (vcm.activeBoardTab == null)
            {
                vcm.activeBoardTab = DBoardTab.DBoard_Diagnostics;
            }
            return View(vcm);
        }


        public IActionResult SwitchToDboardTab(string? tabName)
        {
            var vm = new TransactionsVm();

            switch (tabName)
            {
                case "DBoard_Diagnostics":
                    vm.activeBoardTab = DBoardTab.DBoard_Diagnostics;
                    break;


                case "DBoard_Purchase":
                    vm.activeBoardTab = DBoardTab.DBoard_Purchase;
                    break;

                default:
                    vm.activeBoardTab = DBoardTab.DBoard_Diagnostics;
                    break;
            }
            return RedirectToAction(nameof(Index), vm);
        }

        public IActionResult SwitchToTab(string? tabName)
        {
            var vm = new TransactionsVm();

            switch (tabName)
            {
                case "Diagnostics":
                    vm.activeBillingTab = BillingTab.Diagnosis;
                    break;

                case "Purchase":
                    vm.activeBillingTab = BillingTab.Purchase;
                    break;

                case "Billing":
                    vm.activeBillingTab = BillingTab.Billing;
                    break;

                default:
                    vm.activeBillingTab = BillingTab.Diagnosis;
                    break;
            }

            return RedirectToAction(nameof(Index), vm);
        }


        public IActionResult SwitchToHistoryTab(string? tabName)
        {
            var vm = new TransactionsVm();

            switch (tabName)
            {
                case "All":
                    vm.activeHistoryTab = BillingHistoryTab.All;
                    break;

                case "Diagnostics":
                    vm.activeHistoryTab = BillingHistoryTab.Diagnostics;
                    break;

                case "Purchase":
                    vm.activeHistoryTab = BillingHistoryTab.Purchase;
                    break;

            default:
                vm.activeHistoryTab = BillingHistoryTab.All;
                break;
            }

            return RedirectToAction(nameof(BillingHistory), vm);
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int? id, bool? errorMessage, bool? RemoveErrorMessage, bool? PaymentErrorMessage, TransactionsVm tvm)
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
                .FirstOrDefaultAsync(d => d.diagnostic_Id == id);


            if (tvm.activeBillingTab == null)
            {
                tvm.activeBillingTab = BillingTab.Diagnosis;
            }

            TransactionsVm vm = new TransactionsVm
            {
                Diagnostics = firstAsync,
                activeBillingTab = tvm.activeBillingTab,
                errorMessage = errorMessage,
                RemoveErrorMessage = RemoveErrorMessage,
                PaymentErrorMessage = PaymentErrorMessage
            };

            return View(vm);

        }


        //billing switch Tabs
        public IActionResult SwitchToBillingTab(int? id, string? tabName)
        {

            //object instantiation
            var vm = new TransactionsVm();

            switch (tabName)
            {
                case "Diagnosis":
                    vm.activeBillingTab = BillingTab.Diagnosis;
                    break;

                case "Purchase":
                    vm.activeBillingTab = BillingTab.Purchase;
                    break;

                case "Billing":
                    vm.activeBillingTab = BillingTab.Billing;
                    break;
            }

            return RedirectToAction(nameof(Edit), new { id = id, vm.activeBillingTab });
        }


        [HttpPost]
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
        }

        #region == Functions == 
        private bool DiagnosticExists(int id)
        {
            return _context.Diagnostics.Any(e => e.diagnostic_Id == id);
        }

        private bool PurchaseExists(int? id)
        {
            return _context.Purchases.Any(p => p.purchaseId == id);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchaseDiagnostics(int? diagnosticid, Purchase purchase)
        {
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
                    .FirstOrDefaultAsync(d => d.diagnostic_Id == diagnosticid);

                var purchaseItems = await _context.PurchaseDetails
                    .Include(p => p.product)
                    .ThenInclude(p => p.category)
                    .Include(p => p.Purchase)
                    .Where(p => p.Purchase.diagnosisId_holder == diagnosticid && string.IsNullOrEmpty(p.Purchase.status))
                    .ToListAsync();

                // Calculate the total price again
                /*double purchasePayment = purchaseItems.Sum(item => (double)item.product.sellingPrice * (int)item.quantity);*/

                // Reload the view model with the original data and an error message
                var viewModel = new TransactionsVm
                {
                    Diagnostics = diagnostics,
                    Purchase = await _context.Purchases
                        .Include(p => p.purchaseDetails)
                        .ThenInclude(p => p.product)
                        .ThenInclude(p => p.category)
                        .FirstOrDefaultAsync(p => p.diagnosisId_holder == diagnosticid),
                    Services = await _context.Services.ToListAsync(),
                    IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                    IPurchaseDetails = purchaseItems,
                    /* totalPurchasePayment = purchasePayment,*/
                    activeBillingTab = BillingTab.Purchase
                };

                TempData["Title"] = "Successfully Creted";
                return RedirectToAction("Edit", new { id = diagnosticid, errorMessage = true, viewModel.activeBillingTab });
            }

            //if purchase not null in the View
            var existingPurchase = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .ThenInclude(pd => pd.product)
                .FirstOrDefaultAsync(p => p.diagnosisId_holder == diagnosticid);

            if (existingPurchase == null)
            {
                foreach (var details in purchase.purchaseDetails)
                {
                    // Update product quantity in the database
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.id == details.productId);
                    if (product != null)
                    {
                        product.quantity -= details.quantity;
                    }
                    _context.UpdateRange(product);
                }
                _context.Add(purchase);
        
            }
            else
            {
                foreach (var details in purchase.purchaseDetails)
                {
                    // Check if the product already exists in the purchase details
                    var existingDetail = existingPurchase.purchaseDetails
                        .FirstOrDefault(pd => pd.productId == details.productId);

                    if (existingDetail != null)
                    {
                        // Update the quantity
                        existingDetail.quantity += details.quantity;

                        // Update product quantity in the database
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.id == details.productId);
                        if (product != null)
                        {
                            product.quantity -= details.quantity;
                        }
                    }
                    else
                    {
                        // Add new product detail
                        existingPurchase.purchaseDetails.Add(details);

                        // Update product quantity in the database
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.id == details.productId);
                        if (product != null)
                        {
                            product.quantity -= details.quantity;
                        }
                    }
                }

                _context.Update(existingPurchase);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            var updatedPurchaseItems = await _context.PurchaseDetails
                .Include(p => p.product)
                .ThenInclude(p => p.category)
                .Where(p => p.Purchase.diagnosisId_holder == diagnosticid && string.IsNullOrEmpty(p.Purchase.status))
                .ToListAsync();

            var updatedTvm = new TransactionsVm
            {
                Diagnostics = await _context.Diagnostics.FirstOrDefaultAsync(d => d.diagnostic_Id == diagnosticid),
                Purchase = existingPurchase,
                Services = await _context.Services.ToListAsync(),
                IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                activeBillingTab = BillingTab.Purchase
            };

            return RedirectToAction("Edit", new { id = diagnosticid, updatedTvm.activeBillingTab });
        }



        [HttpGet]
        public async Task<IActionResult> EditDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Diagnostics = await _context.Diagnostics
                .Include(p => p.IdiagnosticDetails)
                .ThenInclude(p => p.Services)
                .FirstOrDefaultAsync(p => p.diagnostic_Id == id);

            var tvm = new TransactionsVm
            {
                Diagnostics = Diagnostics,
                activeBillingTab = BillingTab.Diagnosis

            };
            return View(tvm);
        }

        [HttpPost]
        [ActionName("EditDetails")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetailsPost(int? id, [Bind("Diagnostics")] TransactionsVm tvm)
        {

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
            //fetched the data in database
            var diagnostic = await _context.Diagnostics
                .Include(d => d.pet)
                .ThenInclude(d => d.owner)
                .Include(d => d.IdiagnosticDetails)
                .ThenInclude(dd => dd.Services)
                .FirstOrDefaultAsync(p => p.diagnostic_Id == id);

            var vm = new TransactionsVm
            {
                Diagnostics = diagnostic,
                activeBillingTab = BillingTab.Diagnosis
            };

            return RedirectToAction("Edit", new { id = vm.Diagnostics.diagnostic_Id, vm.activeBillingTab }); // Redirect to a different action after saving
        }

        [HttpGet]
        public async Task<IActionResult> RemoveProduct(int? id)
        {

            var PurchaseItems = await _context.PurchaseDetails
              .Include(p => p.product)
              .ThenInclude(p => p.category)
              .Include(p => p.Purchase)
              .Where(p => p.Purchase.diagnosisId_holder == id && string.IsNullOrEmpty(p.Purchase.status))
              .ToListAsync();


            TransactionsVm vm = new TransactionsVm
            {
                IPurchaseDetails = PurchaseItems
            };
            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProduct(int? diagnosticId, int? purchaseId, int? productId, int? quantity)
        {
            if (quantity == 0 || quantity == null)
            {

                //for passing tvm object to  edit
                var vm = new TransactionsVm
                {
                    activeBillingTab = BillingTab.Purchase,
                };
                return RedirectToAction("Edit", new { id = diagnosticId, vm.activeBillingTab, RemoveErrorMessage = true });
            }

            // Find the purchase detail that matches the purchase and product
            var purchaseDetails = await _context.PurchaseDetails
                .Include(p => p.product)
                .FirstOrDefaultAsync(pd => pd.purchaseId == purchaseId && pd.product.id == productId);

            // Find the corresponding purchase for delete if details is 0
            var purchase = await _context.Purchases
                .Include(p => p.purchaseDetails) // Include details to check remaining items
                .FirstOrDefaultAsync(p => p.purchaseId == purchaseId);

            if (purchaseDetails != null && quantity > 0)
            {
                // Restore product quantity
                var product = purchaseDetails.product; //naka ref na sa product table.
                product.quantity += quantity;
                _context.Update(product);

                // Reduce the quantity of the product in the purchase details
                if (purchaseDetails.quantity > quantity)
                {
                    purchaseDetails.quantity -= quantity ?? 0;
                    _context.Update(purchaseDetails); // Update the purchase details with new quantity
                }
                else
                {
                    // If the quantity becomes zero or less, remove the product from the purchase
                    _context.PurchaseDetails.Remove(purchaseDetails);
                }

                // Adjust the total product payment in the purchase
                /*purchase.totalProductPayment -= purchaseDetails.sellingPrice * quantity;*/

                // If there are no other purchase details left, remove the purchase record
                if (!purchase.purchaseDetails.Any())
                {
                    _context.Purchases.Remove(purchase);
                }

                await _context.SaveChangesAsync();
            }

            var tvm = new TransactionsVm
            {
                activeBillingTab = BillingTab.Purchase
            };

            return RedirectToAction("Edit", new { id = diagnosticId, tvm.activeBillingTab });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BillPayment(int? diagnosticId, [Bind("Billing")] TransactionsVm tvm)
        {
            // Extract the Billing object from the ViewModel
            var Billing = tvm.Billing;

            // Retrieve the diagnostic record
            var diagnostic = await _context.Diagnostics
                .Include(d => d.IdiagnosticDetails)
                .FirstOrDefaultAsync(d => d.diagnostic_Id == diagnosticId && string.IsNullOrEmpty(d.status));

            // Initialize the ViewModel for the view
            var vm = new TransactionsVm
            {
                Diagnostics = diagnostic,
                activeBillingTab = BillingTab.Billing
            };

            // Check if cash received is valid
            if (Billing.cashReceive < Billing.grandTotal)
            {
                // Return to the Edit view if cash received is less than the grand total
                return RedirectToAction("Edit", new { id = diagnosticId, PaymentErrorMessage = true, vm.activeBillingTab });
            }

            // Get the related purchase
            var purchase = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .FirstOrDefaultAsync(p => p.diagnosisId_holder == diagnosticId && string.IsNullOrEmpty(p.status));



            // Define the complete status
            const string complete = "Complete";

            // Update the status of the purchase if it exists
            if (purchase != null)
            {
                purchase.status = complete;
                _context.Update(purchase);
            }

            // Update the status of the diagnostic if it exists
            if (diagnostic != null)
            {
                diagnostic.status = complete;
                _context.Update(diagnostic);
            }

            // Create a new billing entry
            var billingEntry = new Billing
            {
                // Assign the properties based on your model
                date = DateTime.UtcNow, // Set the date to now or customize as needed
                grandTotal = Billing.grandTotal,
                cashReceive = Billing.cashReceive,
                changeAmount = Billing.cashReceive - Billing.grandTotal,
                DiagnosticsId = diagnostic?.diagnostic_Id, // Assigning the foreign key to Diagnostics
                PurchaseId = purchase?.purchaseId // Assigning the foreign key to Purchases (ensure you have this property in Purchase model)
            };

            // Add the new billing entry to the context
            _context.Billings.Add(billingEntry);

            // Save changes to the database
            await _context.SaveChangesAsync(); // Save billing first to get billingId



            if (purchase != null)
            {
                var stockAdjustments = new List<StockAdjustment>();

                foreach (var detail in purchase.purchaseDetails)
                {
                    //stockAdjustment Entry
                    var stockAdjustment = new StockAdjustment
                    {
                        source = "Sales",
                        date = billingEntry.date,
                        stock = detail.quantity,
                        productId = detail.productId,
                        billing_Id = billingEntry.billingId

                    };
                    //pass the entry to the Object
                    stockAdjustments.Add(stockAdjustment);
                }

                _context.StockAdjustments.AddRange(stockAdjustments);
                await _context.SaveChangesAsync();
            }

            // Redirect to the index view after successful payment
            TempData["Message"] = "Payment Completed";
            return RedirectToAction("Index");

        }

        //Billing Index
        public async Task<IActionResult> BillingHistory(TransactionsVm vcm)
        {
            if (vcm.activeHistoryTab == null)
            {
                vcm.activeHistoryTab = BillingHistoryTab.All;
            }
            return View(vcm);
      
        }



        [HttpGet]
        public async Task<IActionResult> BillingHistoryView(int? billingId)
        {
            var Billing = await _context.Billings
                .Include(b => b.diagnostics)
                    .ThenInclude(d => d.IdiagnosticDetails)
                    .ThenInclude(dd => dd.Services)
                .Include(b => b.diagnostics.pet.owner)
                .Include(b => b.purchase)
                    .ThenInclude(p => p.purchaseDetails)
                    .ThenInclude(p => p.product)
                .FirstOrDefaultAsync(b => b.billingId == billingId);

            var tvm = new TransactionsVm
            {
                Billing = Billing
            };

            return View(tvm);
        }


        //View Model method for CreatePurchaseOnly - noted
        private async Task<TransactionsVm> GetPurchaseTransactionViewModel()
        {
            var purchaseDetails = await _context.PurchaseDetails
                .Include(p => p.product)
                .ThenInclude(p => p.category)
                .Include(p => p.Purchase)
                .ToListAsync();

            var products = await _context.Products.Include(p => p.category).Where(p => p.quantity > 0).ToListAsync();

            return new TransactionsVm
            {
                IPurchaseDetails = purchaseDetails,
                IProducts = products
            };
        }

        [HttpGet]
        public async Task<IActionResult> CreatePurchase()
        {
            //get from method
            var vm = await GetPurchaseTransactionViewModel();
            return View(vm);
        }

        //PurchaseOnly
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchase(Purchase purchase)
        {
            if (purchase.purchaseDetails == null || !purchase.purchaseDetails.Any())
            {
                ModelState.AddModelError("", "No products were selected. Please select at least one product to proceed.");

                var vm = await GetPurchaseTransactionViewModel();

                return View("CreatePurchase", vm);
             
            }

            // Check if the entire model is valid
            if (!ModelState.IsValid)
            {
                var vm = await GetPurchaseTransactionViewModel();

                return View("CreatePurchase", vm);
            }

            if (string.IsNullOrWhiteSpace(purchase.customerName))
            {
                purchase.customerName = "NA";
            }

         
            //Product Process
            //get from view
            var ProductIds = purchase.purchaseDetails.Select(p => p.productId).Distinct().ToList();


            // fetched the selected Products only via ID
            var Products = await _context.Products
                .Where(p => ProductIds.Contains(p.id))
                .ToListAsync();


            //Convert to dictionary
            var ProductDictionary = Products.ToDictionary(p => p.id);

            // looping
            foreach(var details in purchase.purchaseDetails)
            {
                //get specific quantity
                var quantity = details.quantity;

                //matched the id to the dictionary
                if(ProductDictionary.TryGetValue(details.productId, out var Product))
                {
                    //update
                    Product.quantity -= quantity;
                }
            }


            _context.Add(purchase);
            _context.UpdateRange(Products);

            TempData["Message"] = "Successfully Created";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new  TransactionsVm{ activeBoardTab = DBoardTab.DBoard_Purchase});
        }

        [HttpGet]
        public async Task<IActionResult> EditPurchase(int purchaseId, bool? RemoveErrorMessage, bool? errorMessage, bool? PaymentErrorMessage)
        {

            var Purchase = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .ThenInclude(p => p.product)
                .ThenInclude(p => p.category)
                .FirstOrDefaultAsync(p => p.purchaseId == purchaseId);

            //VIEW Cart

            var PurchaseItems = await _context.PurchaseDetails
                .Include(p => p.product)
                .ThenInclude(p => p.category)
                .Include(p => p.Purchase)
                .Where(p => p.Purchase.purchaseId == purchaseId && string.IsNullOrEmpty(p.Purchase.status))
                .ToListAsync();
            
            if (RemoveErrorMessage == true)
            {
                ViewData["RemoveErrorMessage"] = "Please enter tht total number of decrement";
            }


            if (errorMessage == true)
            {
                // Add an error message to the ViewData to display on the view
                ViewData["ErrorMessage"] = "No products were selected. Please select at least one product to proceed.";

            }


            var vm = new TransactionsVm
            {
             
                Purchase = Purchase,
                IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                IPurchaseDetails = PurchaseItems,

            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPurchase(int? purchaseId,[Bind("Purchase")]TransactionsVm vm)
        {
            var PurchaseView = vm.Purchase;

            var Purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.purchaseId == purchaseId);


            if (string.IsNullOrWhiteSpace(PurchaseView.customerName))
            {
                Purchase.customerName = "NA";
            }
              Purchase.customerName = PurchaseView.customerName;
                _context.Update(Purchase);
                await _context.SaveChangesAsync();

            return RedirectToAction("EditPurchase", new { purchaseId = purchaseId });
          /*  return RedirectToAction("Index", new TransactionsVm { activeBoardTab = DBoardTab.DBoard_Purchase });*/
        }



        //for remove purchase Only
        [HttpGet]
        public async Task<IActionResult> RemoveProductFromPurchaseOnly(int? purchaseId)
        {

            var PurchaseItems = await _context.PurchaseDetails
              .Include(p => p.product)
              .ThenInclude(p => p.category)
              .Include(p => p.Purchase)
              .Where(p => p.Purchase.purchaseId == purchaseId && string.IsNullOrEmpty(p.Purchase.status))
              .ToListAsync();


            TransactionsVm vm = new TransactionsVm
            {
                IPurchaseDetails = PurchaseItems
            };
            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProductFromPurchaseOnly(int? purchaseId, int? productId, int? quantity)
        {
            if (quantity == 0 || quantity == null)
            {

                return RedirectToAction("EditPurchase", new { purchaseId = purchaseId,  RemoveErrorMessage = true });
            }

            // Find the purchase detail that matches the purchase and product
            var purchaseDetails = await _context.PurchaseDetails
                .Include(p => p.product)
                .FirstOrDefaultAsync(pd => pd.purchaseId == purchaseId && pd.product.id == productId);

            // Find the corresponding purchase for delete if details is 0
            var purchase = await _context.Purchases
                .Include(p => p.purchaseDetails) // Include details to check remaining items
                .FirstOrDefaultAsync(p => p.purchaseId == purchaseId);

            if (purchaseDetails != null && quantity > 0)
            {
                // Restore product quantity
                var product = purchaseDetails.product; //naka ref na sa product table.
                product.quantity += quantity;
                _context.Update(product);

                // Reduce the quantity of the product in the purchase details
                if (purchaseDetails.quantity > quantity)
                {
                    purchaseDetails.quantity -= quantity ?? 0;
                    _context.Update(purchaseDetails); // Update the purchase details with new quantity
                }
                else
                {
           
                    _context.PurchaseDetails.Remove(purchaseDetails);
                }

                if (!purchase.purchaseDetails.Any())
                {
                    _context.Purchases.Remove(purchase);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("EditPurchase", new { purchaseId = purchaseId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPurchaseAdd(int? purchaseId, Purchase purchase)
        {
            if (purchase == null || purchase.purchaseDetails == null || !purchase.purchaseDetails.Any())
            {
                 var purchaseItems = await _context.PurchaseDetails
                    .Include(p => p.product)
                    .ThenInclude(p => p.category)
                    .Include(p => p.Purchase)
                    .Where(p => p.Purchase.purchaseId == purchaseId && string.IsNullOrEmpty(p.Purchase.status))
                    .ToListAsync();

                var viewModel = new TransactionsVm
                {

                    Purchase = await _context.Purchases
                        .Include(p => p.purchaseDetails)
                        .ThenInclude(p => p.product)
                        .ThenInclude(p => p.category)
                        .FirstOrDefaultAsync(p => p.purchaseId == purchaseId),
                    IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                    IPurchaseDetails = purchaseItems,
                };



                return RedirectToAction("EditPurchase", new { purchaseId = purchaseId, errorMessage = true });
            }

            var existingPurchase = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .ThenInclude(pd => pd.product)
                .FirstOrDefaultAsync(p => p.purchaseId == purchaseId);

            if (existingPurchase == null)
            {
                _context.Add(purchase);
            }
            else
            {
                foreach (var details in purchase.purchaseDetails)
                {
                    // Check if the product already exists in the purchase details
                    var existingDetail = existingPurchase.purchaseDetails
                        .FirstOrDefault(pd => pd.productId == details.productId);

                    if (existingDetail != null)
                    {
                        // Update the quantity
                        existingDetail.quantity += details.quantity;

                        // Update product quantity in the database
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.id == details.productId);
                        if (product != null)
                        {
                            product.quantity -= details.quantity;
                        }
                    }
                    else
                    {
                        // Add new product detail
                        existingPurchase.purchaseDetails.Add(details);

                        // Update product quantity in the database
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.id == details.productId);
                        if (product != null)
                        {
                            product.quantity -= details.quantity;
                        }
                    }
                }

                _context.Update(existingPurchase);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            var updatedPurchaseItems = await _context.PurchaseDetails
                .Include(p => p.product)
                .ThenInclude(p => p.category)
                .Where(p => p.Purchase.purchaseId == purchaseId && string.IsNullOrEmpty(p.Purchase.status))
                .ToListAsync();

            var updatedTvm = new TransactionsVm
            {
                Purchase = existingPurchase,
                IProducts = await _context.Products.Include(p => p.category).ToListAsync()
            };

            return RedirectToAction("EditPurchase", new { purchaseId = purchaseId});
        }

        [HttpPost]
        public async Task<IActionResult> DeletePurchase(int? purchaseId)
        {
            if (purchaseId == null)
            {
                return NotFound();
            }

            var Purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.purchaseId == purchaseId);

            if(Purchase != null)
            {
                _context.Remove(Purchase);
            }

            TempData["Message"] = "Successfully Deleted";

             await _context.SaveChangesAsync();
            return RedirectToAction("Index", new TransactionsVm { activeBoardTab = DBoardTab.DBoard_Purchase });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BillPaymentPurchase(int? purchaseId, string? customerName, [Bind("Billing")] TransactionsVm tvm)
        {
            // Extract the Billing object from the ViewModel
            var Billing = tvm.Billing;

            // Get the related purchase
            var purchase = await _context.Purchases
                .Include(p => p.purchaseDetails) // Include purchase details to get products
                .FirstOrDefaultAsync(p => p.purchaseId == purchaseId && string.IsNullOrEmpty(p.status));

            // Define the complete status
            const string complete = "Complete";

            // Update the status of the purchase if it exists
            if (purchase != null)
            {
                purchase.status = complete;

                if (string.IsNullOrWhiteSpace(customerName))
                {
                    purchase.customerName = "NA";
                }
                else
                {
                    purchase.customerName = customerName;
                }

                _context.Update(purchase);
                await _context.SaveChangesAsync();
            }

            // Create a new billing entry
            var billingEntry = new Billing
            {
                // Assign the properties based on your model
                date = DateTime.UtcNow, // Set the date to now or customize as needed
                grandTotal = Billing.grandTotal,
                cashReceive = Billing.cashReceive,
                changeAmount = Billing.cashReceive - Billing.grandTotal,
                PurchaseId = purchase?.purchaseId // Assigning the foreign key to Purchases
            };

            // Add the new billing entry to the context
            _context.Billings.Add(billingEntry);
            await _context.SaveChangesAsync(); // Save billing first to get billingId

            // Create Stock Adjustment entries
            if (purchase != null)
            {

                //noted here. not referring to navigation
                var stockAdjustments = new List<StockAdjustment>();

                foreach (var detail in purchase.purchaseDetails)
                {
                    var stockAdjustment = new StockAdjustment
                    {
                        source = "Sales",
                        date = billingEntry.date, 
                        stock = detail.quantity, 
                        productId = detail.productId, 
                        billing_Id = billingEntry.billingId 
                    };

                    stockAdjustments.Add(stockAdjustment);
                }

                // Add all stock adjustments to the context
                _context.StockAdjustments.AddRange(stockAdjustments);
                await _context.SaveChangesAsync(); // Save the stock adjustments
            }

            // Redirect to the index view after successful payment
            TempData["Message"] = "Payment Completed";
            return RedirectToAction("Index", new TransactionsVm { activeBoardTab = DBoardTab.DBoard_Purchase });
        }


        [HttpGet]
        public async Task<IActionResult> Diagnosis(string searchTerm)
        {
            // Create an empty list initially
            List<Owner> owners = new List<Owner>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Fetch owners based on the search term, with EF's `Where` clause to filter results
                owners = await _context.Owners
                    .Where(o => o.fname.Contains(searchTerm) || o.lname.Contains(searchTerm) || o.mname.Contains(searchTerm)
                    || (o.fname + " " + o.lname).Contains(searchTerm)
                    || (o.fname + o.lname).Contains(searchTerm)) // Adjust this condition based on your requirements
                    .ToListAsync();
            }

            var vm = new TransactionsVm
            {
                IOwner = owners
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDiagnosis(int id, string? btnCnl, int? followupId)
        {
           
            if(id == null)
            {
                return NotFound();
            }
            
            var owner = await _context.Owners.Include(o => o.Pets).FirstOrDefaultAsync( o => o.id == id);

            if(owner == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
             .Include(a => a.IAppDetails)
             .ThenInclude(a => a.Services)
             .FirstOrDefaultAsync(a => a.ownerId == id && string.IsNullOrEmpty(a.remarks));


            var followUp = await _context.PetFollowUps.Include(f => f.Diagnostics).FirstOrDefaultAsync(f => f.Id == followupId);


            if (followUp != null)
            {
                //vm instantiationn
                TransactionsVm aVm = new TransactionsVm
                {
                    Owner = owner,
                    Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync(),
                    PetFollowUps = followUp,
                    btnCnl = btnCnl,
                    AppointType = "followUp"
                    /*  SelectedServices = idList*/
                };
                return View(aVm);
            }
            if (appointment != null)
            {
                TempData["Message"] = "This client has an existing appointment. Selected services will be applied to this form.";
                //vm instantiationn 
                TransactionsVm aVm = new TransactionsVm
                {
                    Owner = owner,
                    Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync(),
                    Appointment = appointment,
                    btnCnl = btnCnl,
                    AppointType = "booking"
                    /*  SelectedServices = idList*/
                };

                return View(aVm);
            }

            var vm = new TransactionsVm
            {
                Owner = owner,
                Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync(),
                btnCnl = btnCnl
             };

            return View(vm);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiagnosis(int? appointId, int? followUpId ,[Bind("Diagnostics")] TransactionsVm tvm)
        {
            var diagnostics = tvm.Diagnostics;

          

            var appointment = await _context.Appointments
                .Include(a => a.IAppDetails) // Include appointment details for comparison
                .FirstOrDefaultAsync(a => a.AppointId == appointId);


            var petfollowUp = await _context.PetFollowUps.FindAsync(followUpId);


            if (petfollowUp != null)
            {
                // Update remarks and save changes
                petfollowUp.status = "Attended";
                _context.Update(petfollowUp);

            }

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
            TempData["Message"] = "Successuly Created";
            // Redirect to the Billing page
            return RedirectToAction("Index", "Billing");
        }


    }
}
       

