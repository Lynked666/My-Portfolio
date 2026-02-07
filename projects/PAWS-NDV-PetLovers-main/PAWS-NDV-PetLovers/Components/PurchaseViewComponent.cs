using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Transactions;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components
{
    public class PurchaseViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public PurchaseViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id, bool? errorMessage, bool? RemoveErrorMessage)
        {

            if (errorMessage == true)
            {
                // Add an error message to the ViewData to display on the view
                ViewData["ErrorMessage"] = "No products were selected. Please select at least one product to proceed.";

            }

            if(RemoveErrorMessage == true)
            {
                ViewData["RemoveErrorMessage"] = "Please enter tht total number of decrement";
            }

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


            var vm = new TransactionsVm
            {
                Diagnostics = await _context.Diagnostics
                .Include(p => p.pet)
                .ThenInclude( p=> p.owner)
                .Include(d => d.IdiagnosticDetails)
                .ThenInclude(dd => dd.Services)
                .FirstOrDefaultAsync(d => d.diagnostic_Id == id),
                Purchase = Purchase,
                IProducts = await _context.Products.Include(p => p.category).ToListAsync(),
                IPurchaseDetails = PurchaseItems,
       
            };
       
            return View(vm);
        }
    }
}
