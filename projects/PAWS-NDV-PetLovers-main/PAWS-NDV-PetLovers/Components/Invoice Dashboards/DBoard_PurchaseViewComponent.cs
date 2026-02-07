using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.Dashboards
{
    public class DBoard_PurchaseViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public DBoard_PurchaseViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var IPurchase = await _context.Purchases
                .Include(p => p.purchaseDetails)
                .Where(p => string.IsNullOrEmpty(p.status) && string.IsNullOrEmpty(p.diagnosisId_holder.ToString()))
                .ToListAsync();

            var tvm = new TransactionsVm
            {
                IPurchase = IPurchase
            };

            return View(tvm);
        }
    }
}
