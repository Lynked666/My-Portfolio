using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using System.Runtime.Intrinsics.X86;

namespace PAWS_NDV_PetLovers.Controllers.Appointments
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ServicesController : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public ServicesController(PAWS_NDV_PetLoversContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {

            var services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync();

            return View(services);
        }
        public async Task<IActionResult> VoidServices()
        {

            var services = await _context.Services.Where(s => !string.IsNullOrEmpty(s.status)).ToListAsync();

            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(bool serviceType,[Bind("serviceId,serviceName,status,serviceCharge, followUp")] Services service)
        {
            
            if(string.IsNullOrEmpty(service.serviceName)|| service.serviceCharge == 0)
            {
                ModelState.AddModelError("", $"Fill up the input fields completely");
                return View(service);
            }

            if (ServiceNameExist(service.serviceName))
            {
                ModelState.AddModelError("", $"Service Name {service.serviceName} already exists!");
                return View(service);
            }

            if(serviceType == true)
            {
                service.serviceType = "Laboratory Test";
            }
            else
            {
                service.serviceType = "NA";
            }

            // Add the service to the database
            _context.Add(service);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Successfully Created";

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }
        #region == Functions ==
        public bool ServiceNameExist(string serviceName)
        {
            return _context.Services.Any(s => s.serviceName == serviceName && string.IsNullOrEmpty(s.status));
        }

        public bool ServiceIdExist(int id)
        {
            return _context.Services.Any(s => s.serviceId == id);
        }

        #endregion
        [HttpGet]
        public IActionResult Edit(int id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var serviceId = _context.Services.Find(id);

            if (serviceId == null)
            {
                return NotFound();
            }

            return View(serviceId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("serviceId,serviceName,serviceCharge,followUp,")] Services updatedService)
        {
            if (id != updatedService.serviceId)
            {
                return NotFound();
            }

            var existingService = await _context.Services.FindAsync(id);

            if (existingService == null)
            {
                return NotFound();
            }

            // Update only the desired fields
            existingService.serviceName = updatedService.serviceName;
            existingService.serviceCharge = updatedService.serviceCharge;
            existingService.followUp = updatedService.followUp;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Message"] = "Successfully Updated";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceIdExist(updatedService.serviceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }




        [HttpGet]

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var find = _context.Services.Find(id);

            if (find == null)
            {
                return NotFound();
            }

            return View(find);

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findAsycn = await _context.Services.FindAsync(id);

            if (findAsycn == null)
            {
                return NotFound();
            }

            //update column
            findAsycn.status = "void";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

   /*     [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findAsycn = await _context.Services.FindAsync(id);

            if (findAsycn == null)
            {
                return NotFound();
            }

            _context.Services.Remove(findAsycn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/
    }
}
