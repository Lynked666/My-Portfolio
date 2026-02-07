using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.Account
{
    public class AccountListViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public AccountListViewComponent(PAWS_NDV_PetLoversContext context)
        {
           _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userAccounts = await _context.UserAccounts
                .Where(a => a.userType != "Admin")
                .ToListAsync();

            return View(new UserAccountVm
            {
                IuserAccount = userAccounts
            });
        }
    }
}
