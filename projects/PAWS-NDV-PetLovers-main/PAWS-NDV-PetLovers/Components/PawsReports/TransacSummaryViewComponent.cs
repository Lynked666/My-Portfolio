using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.PawsReports
{
    public class TransacSummaryViewComponent:ViewComponent
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PAWS_NDV_PetLoversContext _context;

        public TransacSummaryViewComponent(IHttpContextAccessor httpContextAccessor, PAWS_NDV_PetLoversContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync(ReportsVm vcm)
        {
            // Ensure HttpContext is not null
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Content("HttpContext is not available.");
            }

            // Get the user's role from session or claims
            var userRole = httpContext.Session.GetString("UserRole");

            // Check if the user is Staff and restrict access
            if (userRole == "Staff")
            {
                // Return an empty result or a "no access" view

                return Content("You do not have access to this component");
            }

            // Check if vcm is null before accessing its properties
            if (vcm == null)
            {
                return Content("Invalid parameters provided.");
            }

            if (vcm.logical_dateError == true)
            {
                ModelState.AddModelError("", "The end date cannot be earlier than the start date.");
                return View(vcm);
            }

            // Fetch billing data based on date filters
            var billing = await _context.Billings
                .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                          && (!vcm.endDate.HasValue || d.date <= vcm.endDate))
                .Include(b => b.purchase)
                .Include(b => b.diagnostics)
                    .ThenInclude(d => d.pet)
                    .ThenInclude(p => p.owner)
                .ToListAsync();

            return View(new ReportsVm
            {
                IBilling = billing,
                startDate = vcm.startDate,
                endDate = vcm.endDate,
                Filtered = vcm.Filtered,
                activeReportTab = ReportTab.transacSum
            });
        }

    }
}
