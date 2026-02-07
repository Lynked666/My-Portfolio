using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.Appointments
{
    public class FollowUpViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public FollowUpViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool? updated)
        {
            var PetFollowUp = await _context.PetFollowUps
           .Where(p => string.IsNullOrEmpty(p.status))
           .Include(f => f.Diagnostics)
               .ThenInclude(d => d.IdiagnosticDetails)
                   .ThenInclude(d => d.Services)
           .Include(f => f.Diagnostics)
               .ThenInclude(d => d.pet)
                   .ThenInclude(p => p.owner)
           .ToListAsync();

            if (updated == true)
            {
                TempData["SuccessMessage"] = "Updated Successfully";
            }


            var vm = new AppointmentVm
            {
                IPetFollowUps = PetFollowUp,
                Services = await _context.Services.ToListAsync()
            };

            return View(vm);
        }
    }
}
