using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.Appointments
{
    public class MonitoringViewComponent : ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public MonitoringViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(AppointmentVm vcm)
        {

            //validations
            if (vcm.null_dateError != null || vcm.logical_dateError != null)
            {
                if (vcm.null_dateError == true)
                {
                    ModelState.AddModelError("", "Both the start date and end date are required.");
                }
                if (vcm.logical_dateError == true)
                {
                    ModelState.AddModelError("", "The end date cannot be earlier than the start date.");
                }

                return View(vcm);
            }

            if (vcm.reportType == "booking")
            {
                if (vcm.Status != null)
                {
                    if (vcm.Status == "inProgress")
                    {
                        var Inprog_Appp = await _context.Appointments
                            .Include(a => a.IAppDetails)
                            .ThenInclude(a => a.Services)
                            .Include(a => a.OwnerNav)
                            .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                                    && (!vcm.endDate.HasValue || d.date <= vcm.endDate)
                                    && string.IsNullOrEmpty(d.remarks))
                            .ToListAsync();

                        return View(new AppointmentVm
                        {
                            IEAppointment = Inprog_Appp,
                            Status = vcm.Status,
                            startDate = vcm.startDate,
                            SelectType = vcm.SelectType,
                            endDate = vcm.endDate,
                            Filtered = vcm.Filtered,
                            reportType = vcm.reportType,
                            activeAppointTab = AppointmentTab.monitoring
                        });
                    }
                    else
                    {
                        //completed Status
                        var Inprog_Appp = await _context.Appointments
                            .Include(a => a.IAppDetails)
                            .ThenInclude(a => a.Services)
                            .Include(a => a.OwnerNav)
                            .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                                    && (!vcm.endDate.HasValue || d.date <= vcm.endDate)
                                    && d.remarks == "Completed")
                            .ToListAsync();

                        return View(new AppointmentVm
                        {
                            IEAppointment = Inprog_Appp,
                            Status = vcm.Status,
                            startDate = vcm.startDate,
                            SelectType = vcm.SelectType,
                            endDate = vcm.endDate,
                            Filtered = vcm.Filtered,
                            reportType = vcm.reportType,
                            activeAppointTab = AppointmentTab.monitoring
                        });
                    }
                }

                //SelectType = all
                var app = await _context.Appointments
                            .Include(a => a.IAppDetails)
                            .ThenInclude(a => a.Services)
                            .Include(a => a.OwnerNav)
                            .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                                    && (!vcm.endDate.HasValue || d.date <= vcm.endDate)
                                    && d.remarks != "Cancelled")
                            .ToListAsync();

                //IEappointment is not receiving in data from controller, so this is my alternative approach
                var vm = new AppointmentVm
                {
                    IEAppointment = app,

                    Status = vcm.Status,
                    SelectType = vcm.SelectType,
                    Filtered = vcm.Filtered,
                    startDate = vcm.startDate,
                    endDate = vcm.endDate,
                    reportType = vcm.reportType,
                    activeAppointTab = AppointmentTab.monitoring

                };

                return View(vm);
            }

            else if (vcm.reportType == "followUp")
            {
                if (vcm.Status != null)
                {
                    if (vcm.Status == "inProgress")
                    {
                        return View(new AppointmentVm
                        {
                            IPetFollowUps = await _context.PetFollowUps
                            .Include(p => p.Diagnostics)
                                .ThenInclude(d => d.pet)
                                    .ThenInclude(dp => dp.owner)
                            .Include(a => a.Services)
                            .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                                     && (!vcm.endDate.HasValue || d.date <= vcm.endDate)
                                     && string.IsNullOrEmpty(d.status))
                            .ToListAsync(),
                            Status = vcm.Status,
                            startDate = vcm.startDate,
                            SelectType = vcm.SelectType,
                            endDate = vcm.endDate,
                            Filtered = vcm.Filtered,
                            reportType = vcm.reportType,
                            activeAppointTab = AppointmentTab.monitoring
                        });
                    }
                    else
                    {
                        //COompleted FollowUp
                        return View(new AppointmentVm
                        {
                            IPetFollowUps = await _context.PetFollowUps
                            .Include(p => p.Diagnostics)
                                 .ThenInclude(d => d.pet)
                                     .ThenInclude(dp => dp.owner)
                           .Include(a => a.Services)
                           .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                                    && (!vcm.endDate.HasValue || d.date <= vcm.endDate)
                                    && d.status == "Attended"
                                    && !string.IsNullOrEmpty(d.status))
                           .ToListAsync(),
                            Status = vcm.Status,
                            startDate = vcm.startDate,
                            SelectType = vcm.SelectType,
                            endDate = vcm.endDate,
                            Filtered = vcm.Filtered,
                            reportType = vcm.reportType,
                            activeAppointTab = AppointmentTab.monitoring
                        });
                    }
                }

                //Select Type == All
                return View(new AppointmentVm
                {
                    IPetFollowUps = await _context.PetFollowUps
                    .Include(p => p.Diagnostics)
                         .ThenInclude(d => d.pet)
                             .ThenInclude(dp => dp.owner)
                   .Include(a => a.Services)
                   .Where(d => (!vcm.startDate.HasValue || d.date >= vcm.startDate)
                            && (!vcm.endDate.HasValue || d.date <= vcm.endDate)
                            && d.status != "Cancelled")
                   .ToListAsync(),
                    Status = vcm.Status,
                    startDate = vcm.startDate,
                    SelectType = vcm.SelectType,
                    endDate = vcm.endDate,
                    Filtered = vcm.Filtered,
                    reportType = vcm.reportType,
                    activeAppointTab = AppointmentTab.monitoring
                 });
            }
            return View();
        }
    }
}

        #region == functions for Appointment Reports ==
        /*        public async Task<AppointmentVm> GetAppointments()
                {
                    var reportVm = new AppointmentVm
                    {
                        IAppointment = await _context.Appointments
                             .Include(a => a.OwnerNav)
                             .Include(a => a.IAppDetails)
                                .ThenInclude(d => d.Services)
                             .Where(a => a.remarks != "Cancelled")
                             .ToListAsync(),
                    };
                    return reportVm;*/
    
        #endregion
  
