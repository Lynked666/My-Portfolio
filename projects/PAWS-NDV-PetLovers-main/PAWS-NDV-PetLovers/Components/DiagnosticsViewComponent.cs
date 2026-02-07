using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Component
{
    public class DiagnosticsViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public DiagnosticsViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {

            var diagnostics = await _context.Diagnostics
                            .Include(d => d.PurchaseNav)
                            .ThenInclude(p => p.purchaseDetails)
                            .ThenInclude(p => p.product)
                            .Include(d => d.pet)
                            .ThenInclude(p => p.owner)
                            .Include(d => d.IdiagnosticDetails)
                            .ThenInclude(dd => dd.Services)
                            .FirstOrDefaultAsync(d => string.IsNullOrEmpty(d.status) && id == d.diagnostic_Id);
       

            var vm = new TransactionsVm
            {
                Diagnostics = diagnostics,

            };

            return View(vm);
        }
    }
}
