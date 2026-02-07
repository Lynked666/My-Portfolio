using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components
{
    public class BillingViewComponent : ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public BillingViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id, bool? PaymentErrorMessage)
        {

            var diagnostics = await _context.Diagnostics
                .Include(d => d.IdiagnosticDetails)
                .ThenInclude(dd => dd.Services)
                .FirstOrDefaultAsync(d => d.diagnostic_Id == id);



            var purchase = await _context.PurchaseDetails
                .Include(p => p.Purchase)
                .Include(p => p.product)
                .Where(p => p.Purchase.diagnosisId_holder == id)
                .ToListAsync();


            var tvm = new TransactionsVm
            {
                Diagnostics = diagnostics,
                IPurchaseDetails = purchase,
                PaymentErrorMessage = PaymentErrorMessage
            };

            return View(tvm);
        }
    }
}
