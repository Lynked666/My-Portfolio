using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Dashboard
{
    [ServiceFilter(typeof(AuthFilter))]
    public class HomeController : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public HomeController(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var currentDate = DateTime.Now;
            var today = currentDate.Date;


            var appointments = await _context.Appointments
                .Where(a => a.date == today
                && string.IsNullOrEmpty(a.remarks))
                .Include(a => a.IAppDetails)
                .ThenInclude(ad => ad.Services)
                .Select(a => a)
                .ToListAsync();

            var followUp = await _context.PetFollowUps
                 .Where(a => a.date == today
                && string.IsNullOrEmpty(a.status))
                 .Include(f => f.Diagnostics)
                     .ThenInclude(d => d.pet)
                     .ThenInclude(p => p.owner)
                 .Include(f => f.Services)
                 .Select(f => f)
                 .ToListAsync();

            var products = await _context.Products
                .Where(p => p.quantity <= 10)
                .Include(p => p.category)
                .Select(p => p)
                .ToListAsync();


            var vm = new ReportsVm
            {
                IAppointment = appointments,
                IPetFollowUps = followUp,
                IProducts = products
            };
            return View(vm);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
