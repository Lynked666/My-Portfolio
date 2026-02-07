using Microsoft.AspNetCore.Mvc;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.Account
{
    public class CreateStaffViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public CreateStaffViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(UserAccountVm vm)
        {

            if (vm.isNull)
            {
                ModelState.AddModelError("", "Please fill-up username or password");
           
            }

            if (vm.isExist)
            {
                ModelState.AddModelError("", "This name is already in use and currently active.");
            }
            return View();
        }
    }
}
