using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Appointments
{
    [ServiceFilter(typeof(AuthFilter))]
    public class Appointments : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public Appointments(PAWS_NDV_PetLoversContext context)
        {
            this._context = context;
        }


        public IActionResult Dashboard(string? tabName,AppointmentVm vcm)
        {
            if(vcm.activeAppointTab == null)
            {
                vcm.activeAppointTab = AppointmentTab.booking;
            }
            if(tabName == "followUp")
            {
                vcm.activeAppointTab = AppointmentTab.followUp;
            }
            if (tabName == "monitoring")
            {
                vcm.activeAppointTab = AppointmentTab.monitoring;
            }
            return View(vcm);
        }

        public IActionResult SwitchToTab(string tabName)
        {
            var vm = new AppointmentVm();

            switch (tabName)
            {
                case "booking":
                    vm.activeAppointTab = AppointmentTab.booking;
                    break;

                case "followUp":
                    vm.activeAppointTab = AppointmentTab.followUp;
                    break;


                case "monitoring":
                    vm.activeAppointTab = AppointmentTab.monitoring;
                    break;

                default:
                    vm.activeAppointTab = AppointmentTab.booking;
                    break;
            }

            return RedirectToAction(nameof(Dashboard), vm);
        }

        public async Task<IActionResult> Index()
        {

            var appDetails = await GetAllAsync();

            return View(appDetails);
        }





        #region == Functions == 
        public async Task<AppointmentVm> GetAllAsync()
        {
            // Fetch appointment details from the database, including related Services and Appointment entities.
            var Appointments = await _context.Appointments
                .Include(a => a.IAppDetails)
                .ThenInclude(ad => ad.Services)
                .Where(a => a.remarks == null)
                .ToListAsync();

            var owners = await _context.Owners.ToListAsync();

        
            var vm = new AppointmentVm
            {
                IAppointments = Appointments,
                IOwner = owners
            };

            return vm;
        }

        public async Task GetServices()
        {
            var services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync();

            ViewBag.Services = new SelectList(services, "serviceId", "serviceName");
        }

        public async Task<IActionResult> AppointmentHistory()
        {
            var AppDetails = await _context.AppointmentDetails.Include(ad => ad.Appointment).Include(ad => ad.Services).ToListAsync();

            var groupById = AppDetails
                .GroupBy(a => a.Appointment.AppointId)
                .Select(g => new AppointmentGroup
                {
                    Appointment = g.First().Appointment,
                    Details = g.ToList()
                })
                .ToList();

            var vm = new AppointmentVm
            {
                AppointmentGrouping = groupById
            };

            return View(vm);
        }

        
        public async Task<AppointmentVm> GetCreateViews()
        {

            //vm instantiation for owners
            var Owner = await _context.Owners.ToListAsync();
            var Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync();

            var appointments = await _context.Appointments.ToListAsync();

            var appDetails = new AppointmentVm
            {
                IOwner = Owner,
                IlistServices = Services,
                IAppointments = appointments
            };
            return appDetails;
        }


        public bool AppointmentExist(string fname, string lname,string mname)
        {
            var appDetails = _context.AppointmentDetails.Include(a => a.Appointment);

            return appDetails.Any(a => a.Appointment.fname == fname
            && a.Appointment.lname == lname 
            && a.Appointment.mname == mname 
            &&  string.IsNullOrEmpty(a.Appointment.remarks));
        }

        #endregion

        //ready to fetch function for JQUERY
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(DateTime date)
        {
            var dateFromView = date.Date;

            // Current date and hour
            var currentDate = DateTime.Now;
            var today = currentDate.Date;
            var currentHour = currentDate.Hour;

            // Fetch appointments and determine available times
            var appointmentsOnDate = await _context.Appointments
                .Where(a => a.date == date)
                .Select(a => a.time)
                .ToListAsync();

            // Convert to a comma-separated string
            var convertedToString = string.Join(",", appointmentsOnDate);

            // Declare List storage variables
            var am = new List<string>();
            var pm = new List<string>();

            for (int hour = 1; hour < 12; hour++)
            {
                var time = $"{hour}:00";

                if (hour >= 8 && hour <= 11) // AM Times
                {
                    // If it's today, check the current hour condition
                    if (dateFromView == today)
                    {
                        // Only add AM times greater than the current hour
                        if (!convertedToString.Contains(time) && hour > currentHour)
                        {
                            am.Add(time);
                        }
                    }
                    else
                    {
                        // For future dates, just check if the time doesn't exist
                        if (!convertedToString.Contains(time))
                        {
                            am.Add(time);
                        }
                    }
                }
                else if (hour >= 1 && hour <= 4) // PM Times
                {
                    var addHour = hour + 12;
                    var timePm = $"{hour}:00";

                    if (dateFromView == today)
                    {
                        // Only add PM times greater than the current hour
                        if (!convertedToString.Contains(timePm) && addHour > currentHour)
                        {
                            pm.Add(timePm + " pm");
                        }
                    }
                    else
                    {
                        // For future dates, just check if the time doesn't exist
                        if (!convertedToString.Contains(timePm))
                        {
                            pm.Add(timePm + " pm");
                        }
                    }
                }
            }

            // Return the available times as JSON
            return Json(new { availableAM = am, availablePM = pm });

        }

        [HttpGet]
        public async Task<IActionResult> Create(DateTime date)
        {
            // Populate ViewBag.Services or other necessary data
            await GetServices();

            // Fetch owners, services, and appointments from the database
            var owners = await _context.Owners.ToListAsync();
            var services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync();
            var appointments = await _context.Appointments.ToListAsync();

            // Retrieve appointments on the selected date
            var appointmentsOnDate = await _context.Appointments
                .Where(a => a.date == date)
                .Select(a => a.time)
                .ToListAsync();

            // Get current date and time for comparison
            var currentDate = DateTime.Now.Date;
            var currentTime = DateTime.Now.TimeOfDay;

            // Convert appointments times to string
            var convertedToString = string.Join(" ", appointmentsOnDate);
            var am = new List<string>();
            var pm = new List<string>();

            for (int hour = 1; hour < 12; hour++)
            {
                var time = $"{hour}:00";

                // Determine AM or PM
                if (hour >= 8 && hour <= 11) // AM Times
                {
                    if (date == currentDate && new TimeSpan(hour, 0, 0) < currentTime)
                    {
                        // Skip past times for the current day
                        continue;
                    }

                    if (!convertedToString.Contains(time))
                    {
                        am.Add(time);
                    }
                }
                else if (hour >= 1 && hour <= 4) // PM Times
                {
                    time = $"{hour + 12}:00"; // Convert to 24-hour format for PM

                    if (date == currentDate && new TimeSpan(hour + 12, 0, 0) < currentTime)
                    {
                        // Skip past PM times for the current day
                        continue;
                    }

                    if (!convertedToString.Contains(time))
                    {
                        pm.Add(time);
                    }
                }
            }

            // Create the view model and set the provided date
            var viewModel = new AppointmentVm
            {
                IOwner = owners,
                IlistServices = services,
                IAppointments = appointments,
                AvailableAM = am,
                AvailablePM = pm,
                Appointment = new Appointment { date = date } // Set input field date by default (this is routed from index)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Appointment")] AppointmentVm viewModel) //appointment vm is declared
        {

            //load service
            await GetServices();

            //specifying the target property
            var appointment = viewModel.Appointment;

            //data will be stored here, it errors if duplicate data is detected
            var services = new HashSet<String>();

            //get the id of serviceID inside the IAppDetails
            var service = appointment.IAppDetails.Select(d => d.serviceID).ToList();


            foreach (var serviceName in service)
            {
                //check the name if unique
                if (!services.Add(serviceName.ToString()))
                {
                    ModelState.AddModelError("", "Duplicate selection of service name is invalid");

                    //create view instantiation
                    var vm = await GetCreateViews();
                    return View("Create", vm);
                }
            }

            if (appointment.contact.Length < 11)
            {
                ModelState.AddModelError("", $"Contact number below 11 is not valid");
                //vm for create  object instantiation
                var vm = await GetCreateViews();
                return View("Create", vm);
            }

            if (AppointmentExist(appointment.fname, appointment.lname, appointment.mname))
            {
                ModelState.AddModelError("", $"Invalid \'{appointment.fname} {appointment.lname}\' is on pending");

                //vm for create  object instantiation
                var vm = await GetCreateViews();
                return View("Create", vm);

            }
            //save to db
            _context.Add(appointment);
            await _context.SaveChangesAsync();

            //vm for index object instantiation
            var appDetails = await GetAllAsync();

            var vcm = new AppointmentVm();
            vcm.activeAppointTab = AppointmentTab.booking;
            vcm.created = true;
            return RedirectToAction(nameof(Dashboard), vcm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRemarks(string fname, string lname, int appointmentId)
        {

            var verifyOwner = _context.Owners.FirstOrDefault(a => a.fname == fname && a.lname == lname);

            if (verifyOwner == null)
            {
                ModelState.AddModelError("", "Registration required");
                var vm = await GetAllAsync();
                return View("Index", vm);
            }


            //get owner Objects 
            var owner = await _context.Owners
                .Where(o => o.fname == fname && o.lname == lname)
                .Select(o => o).FirstOrDefaultAsync();

            await _context.SaveChangesAsync();

            //object instantiation

            TransactionsVm tvm = new TransactionsVm
            {
                Owner = owner
            };
            //direct to different view and controller
            /*return RedirectToAction("ActionOrViewName", "ControllerName", route);*/


            return RedirectToAction("DiagnosAppointment", "C_Diagnostics", new { id = tvm.Owner.id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, DateTime? time, DateTime getdate)
        {

            if (id == null)
            {
                return NotFound();
            }

            //fetch the appointment with includes
            var appointment = await _context.Appointments
                .Include(a => a.IAppDetails) 
                .ThenInclude(a => a.Services)
                .FirstOrDefaultAsync(a => a.AppointId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.time = DateTime.MinValue;
            await _context.SaveChangesAsync();

            //remove time to be seen again in dropdown
            appointment.date = getdate;
            //set the removed time as default
            appointment.time = time.Value;
            //vm instantiationn
            AppointmentVm vm = new AppointmentVm
            {
                Appointment = appointment,
                Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync(),
                timeHolder = time.Value

            };

            return View("Edit", vm);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  AppointmentVm appoint)
        {
            var appointments = appoint.Appointment;

            if (id != appointments.AppointId)
            {
                return NotFound();
            }

            if (appointments.contact.Length < 11)
            {
                ModelState.AddModelError("", $"Contact number below 11 is not valid");

                var app = await _context.Appointments
                  .Include(a => a.IAppDetails)
                  .ThenInclude(a => a.Services)
                  .FirstOrDefaultAsync(a => a.AppointId == id);

                // VM instantiation
                AppointmentVm vm = new AppointmentVm
                {
                    Appointment = app,
                    Services = await _context.Services.Where(s => string.IsNullOrEmpty(s.status)).ToListAsync()
                };

                return View("Edit", vm);
            }

            try
            {
                // Get the existing appointment to update
                var existingAppointment = await _context.Appointments
                    .Include(a => a.IAppDetails)
                    .FirstOrDefaultAsync(a => a.AppointId == id);

                if (existingAppointment == null)
                {
                    return NotFound();
                }

                // Remove existing services
                _context.AppointmentDetails.RemoveRange(existingAppointment.IAppDetails);

                // Update appointment details
                existingAppointment.fname = appointments.fname;
                existingAppointment.mname = appointments.mname;
                existingAppointment.lname = appointments.lname;
                existingAppointment.contact = appointments.contact;
                existingAppointment.date = appointments.date;
                existingAppointment.time = appointments.time;
               // existingAppointment.remarks = appointments.remarks;

                // Add new appointment details from the ViewModel
                foreach (var detail in appoint.Appointment.IAppDetails)
                {
                    // Only add if a new service ID is provided
                    if (detail.serviceID.HasValue)
                    {
                        existingAppointment.IAppDetails.Add(new AppointmentDetails
                        {
                            AppointId = existingAppointment.AppointId,
                            serviceID = detail.serviceID
                        });
                    }
                }

                // Save changes
                await _context.SaveChangesAsync();

                var vcm = new AppointmentVm();
                vcm.activeAppointTab = AppointmentTab.booking;
                vcm.updated = true;

                return RedirectToAction(nameof(Dashboard), vcm);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        public bool AppointmentExist(int id)
        {
            return _context.Appointments.Any(a => a.AppointId == id);
        }

        public async Task<IActionResult> ResetDateTime(int? id, DateTime? prevSched)
        { 
            if(id == null)
            {
                return NotFound();
            }


            var appointment = await _context.Appointments.FindAsync(id);

            if(appointment != null)
            {
                //back prevTime as default
                appointment.time = prevSched.Value;
                await _context.SaveChangesAsync();
            }
            

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the appointment and related details
            var appointment = await _context.Appointments
                .Include(a => a.IAppDetails)   // Load related details
                .ThenInclude(ad => ad.Services)
                .FirstOrDefaultAsync(a => a.AppointId == id);

            if (appointment == null)
            {
                return NotFound();
            }


            var vm = new AppointmentVm
            {
                Appointment = appointment,
                IAppDetails = appointment.IAppDetails.ToList(), // Convert HashSet to List if necessary using .ToList()
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            //include the existing details appointment
            var appointment = await _context.Appointments
                .Include(a => a.IAppDetails)
                .FirstOrDefaultAsync(a => a.AppointId == id);
             
            if (appointment != null)
            {
                appointment.remarks = "Cancelled";
                appointment.time = DateTime.MinValue;
                _context.Appointments.Update(appointment);
                /*_context.Appointments.Remove(appointment);*/
                await _context.SaveChangesAsync();
            }

            var vm = new AppointmentVm();
            vm.updated = true;
            vm.activeAppointTab = AppointmentTab.booking;
            return RedirectToAction(nameof(Dashboard), vm);
        }
    }
}