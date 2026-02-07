using Microsoft.AspNetCore.Mvc;

namespace PAWS_NDV_PetLovers.Controllers.Appointments
{
    [ServiceFilter(typeof(AuthFilter))]
    public class AppointmentsDetails : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
