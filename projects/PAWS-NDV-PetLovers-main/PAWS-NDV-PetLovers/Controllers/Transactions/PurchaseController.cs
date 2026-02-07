using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Transactions;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Transactions
{
    [ServiceFilter(typeof(AuthFilter))]
    public class PurchaseController : Controller
    {

        //dependency injection
        private readonly PAWS_NDV_PetLoversContext _context;

        public PurchaseController(PAWS_NDV_PetLoversContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Purchase = await _context.Purchases.Include(p => p.purchaseDetails).Where( p =>  p.diagnosisId_holder == 0 && string.IsNullOrEmpty(p.status)).ToListAsync();

          
            return View(Purchase);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var tvm = new TransactionsVm
            {
                IProducts = await _context.Products.Include(p => p.category).Where(p => p.quantity > 0).ToListAsync()
            };

            return View(tvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionsVm tvm)
        {

            //check if tvm,purchase and purchasedetails is null or missing
            if (tvm?.Purchase?.purchaseDetails == null || !tvm.Purchase.purchaseDetails.Any())
            {
                ModelState.AddModelError("", "Please select at least one product.");

                return await ReloadView();
            }

            // Compute total payment and update product quantities
            var purchasedTotals = new List<double>();

            //get the selected product Id from view
            var productIds = tvm.Purchase.purchaseDetails.Select(pd => pd.productId).Distinct().ToList();

            // retrieve only the selectedID from DB_Products - Contains
            var products = await _context.Products.Where(p => productIds.Contains(p.id)).ToListAsync();

            // retrieved product convert to dictionary (to make them as object)
            var productDictionary = products.ToDictionary(p => p.id);

            foreach (var purchaseDetail in tvm.Purchase.purchaseDetails)
            {
                if (productDictionary.TryGetValue(purchaseDetail.productId, out var product))
                {
                    // Validate quantity
                    if (purchaseDetail.quantity > product.quantity)
                    {
                        ModelState.AddModelError("", $"Ordered quantity for product '{product.productName}' exceeds available stock.");

                       return await ReloadView();
                    }

                    // Calculate total
                    double productTotal = (double)product.sellingPrice * purchaseDetail.quantity;
                    purchasedTotals.Add(productTotal);

                    // Update product quantity
                    product.quantity -= purchaseDetail.quantity;
                }
                else
                {
                    ModelState.AddModelError("", $"Product with ID {purchaseDetail.productId} not found.");
          
                    return await ReloadView();
                }
            }

            // Set total payment
           /* tvm.Purchase.totalProductPayment = purchasedTotals.Sum();*/

            // Add the purchase to context
            _context.Purchases.Add(tvm.Purchase);

            // Update product quantities
            _context.Products.UpdateRange(products);

            // Save changes
            await _context.SaveChangesAsync();


            // Helper to reload view with products
            async Task<IActionResult> ReloadView() =>
                View(new TransactionsVm
                {
                    IProducts = await _context.Products.Include(p => p.category).Where(p => p.quantity > 0).ToListAsync()
                });


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var PurchaseFirstAsync = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .ThenInclude(p => p.product)
                .ThenInclude(p => p.category)
                .FirstOrDefaultAsync(p => p.purchaseId == id);
            

            if(PurchaseFirstAsync == null)
            {
                return NotFound();
            }

            var tvm = new TransactionsVm
            {
                Purchase = PurchaseFirstAsync
            };

            return View();
        }
    }
}
