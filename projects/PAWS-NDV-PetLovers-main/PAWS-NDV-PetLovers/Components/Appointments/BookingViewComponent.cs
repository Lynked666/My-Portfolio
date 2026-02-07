using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.Appointments
{
    public class BookingViewComponent :ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public BookingViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }
        

        public async Task<IViewComponentResult> InvokeAsync(bool? updated, bool? created, bool? cancelled)
        {
            // Fetch appointment details from the database, including related Services and Appointment entities.
            var Appointments = await _context.Appointments
                .Include(a => a.IAppDetails)
                .ThenInclude(ad => ad.Services)
                .Where(a => a.remarks == null)
                .ToListAsync();

            var owners = await _context.Owners.ToListAsync();


            if (updated == true)
            {
                TempData["SuccessMessage"] = "Updated Successfully";
            }

            if (created == true)
            {
                TempData["SuccessMessage"] = "Created Successfully";
            }
           
            var vm = new AppointmentVm
            {
                IAppointments = Appointments,
                IOwner = owners,
            };


            return View(vm);
        }
    }
}
