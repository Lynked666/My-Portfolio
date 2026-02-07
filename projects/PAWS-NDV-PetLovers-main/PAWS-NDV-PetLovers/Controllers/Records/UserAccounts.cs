using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.PawsSevices;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Records
{
    [ServiceFilter(typeof(AuthFilter))]
    public class UserAccounts : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public UserAccounts(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(u => u.acc_Id == UserId());

            if(userAccount == null)
            {
                return NotFound();
            }

            return View(new UserAccountVm
            {
                userAccount = userAccount
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userAccount = await _context.UserAccounts.FindAsync(UserId());

            var vm = new UserAccountVm
            {
                userAccount = userAccount
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([Bind("userAccount")]UserAccountVm vm)
        {
            var userAccount = vm.userAccount;

            var findUser = await _context.UserAccounts.FindAsync(UserId());

            if (findUser == null)
            {
                ModelState.AddModelError("", "Unable to Update. Please contact the Admin");

                var model = new UserAccountVm
                { userAccount = userAccount };

                return View(model);
            }


            if (userAccount.contact.Count() < 11)
            {
                ModelState.AddModelError("", "Phone number must be at least 11 digits");

                var model = new UserAccountVm
                { userAccount = userAccount };

                return View(model);
            }
            
            //if true
            findUser.fname = userAccount.fname;
            findUser.lname = userAccount.lname;
            findUser.mname = userAccount.mname;
            findUser.email = userAccount.email;
            findUser.contact = userAccount.contact;


            HttpContext.Session.SetString("fullName", $"{userAccount.fname} {userAccount.lname}");

            await _context.SaveChangesAsync();

            TempData["Message"] = "Successfully Updated";
            return RedirectToAction(nameof(Profile));

        }
        public int? UserId()
        {
            return (int)HttpContext.Session.GetInt32("UserId");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string? oldPassword, string? password, string? ConfirmPassword)
        {
            var hashedPassword = HashingService.HashData(oldPassword);

            var userAccount = await _context.UserAccounts.FindAsync(UserId());

            if (userAccount == null)
            {
                return NotFound();
            }

            if (userAccount.passWord != hashedPassword)
            {
                // Add error to ModelState to display on the page
                ModelState.AddModelError("", "Old password is incorrect");

                // Return the same view with HasPasswordError set to true, so the modal opens
                var model = new UserAccountVm
                {
                    userAccount = userAccount,
                    HasPasswordError = true
                };
                return View("Profile", model);
            }

            else if (password != ConfirmPassword)
            {

                ModelState.AddModelError("", "Password are not match");

                var model = new UserAccountVm
                {
                    userAccount = userAccount,
                    HasPasswordError = true
                };
                return View("Profile", model);
            }
            else
            {

                if (password.Count() <= 8)
                {
                    ModelState.AddModelError("", "Password below 8 characters is invalid");

                    var mdl = new UserAccountVm
                    {
                        userAccount = userAccount,
                        HasPasswordError = true
                    };
                    return View("Profile", mdl);

                }

                var newPassword = HashingService.HashData(password);

                //update passwort
                userAccount.passWord = newPassword;
                var model = new UserAccountVm
                {
                    userAccount = userAccount
                };

                TempData["Message"] = "Password Successfully Updated ";
                await _context.SaveChangesAsync();
                return View("Profile", model);
            }
        }

        public IActionResult Account(UserAccountVm vm)
        {
            if (vm.activeAccountTab == null)
            {
                vm.activeAccountTab = AccountTab.accountList;
            }
            return View(vm);
        }

        //admin actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAccount(int userId, string action)
        {

           
            var userAccount = await _context.UserAccounts.FindAsync(userId);

            if (userAccount != null)
            {
                switch (action)
                {
                    case "Reset":
                        var hashData = HashingService.HashData(userAccount.userName);
                        userAccount.passWord = hashData;
                        userAccount.IsPasswordChanged = false;
                        TempData["Message"] = "Account Reset Successful";
                        break;

                    case "Deactivate":
                        userAccount.IsActive = false;
                        TempData["Message"] = "Deactivation Successful";
                        break;

                    case "Activate":
                        TempData["Message"] = "Activation Successful";
                        userAccount.IsActive = true;
                        break;


                    case "Delete":
                        TempData["Message"] = "Successfully Deleted";
                        _context.UserAccounts.Remove(userAccount);
                        //if mag add ug options about sa transac2 dapat attribute na nga deleted ang ibutang
                        //ma error nani siya kung mag butang nakag name sa ga process sa purchase or diagnosis.
                        await _context.SaveChangesAsync();
                        break;


                    default:
                        // Handle unknown action if necessary
                        break;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Account));
        }

        public IActionResult SwitchToTab(string tabName)
        {
            var vm = new UserAccountVm();

            switch (tabName)
            {
                case "accountList":
                    vm.activeAccountTab = AccountTab.accountList;
                    break;

                case "createStaff":
                    vm.activeAccountTab = AccountTab.createStaff;
                    break;

                default:
                    vm.activeAccountTab = AccountTab.accountList;
                    break;
            }

            return RedirectToAction(nameof(Account), vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(string fname, string lname)
        {
            var vm = new UserAccountVm();

         
            if (string.IsNullOrWhiteSpace(fname) || string.IsNullOrWhiteSpace(lname))
            {
                vm.isNull = true;
                vm.activeAccountTab = AccountTab.createStaff;

                return RedirectToAction("Account", vm);
            }

            //name already exist
            var userAccountCheck = await _context.UserAccounts.FirstOrDefaultAsync(u => u.fname == fname && u.lname == lname && u.IsActive == true);

            if (userAccountCheck != null)
            {
                vm.isExist = true;
                vm.activeAccountTab = AccountTab.createStaff;

                return RedirectToAction("Account", vm);
            }

            var fullname = $"{fname}{lname}";

            var username_Passsowrd = $"ndv.{fullname.ToLower()}";

            var hashData = HashingService.HashData(username_Passsowrd);

           //capitalizing first letter
           var Fname = char.ToUpper(fname[0]) +fname.Substring(1);
            var Lname = char.ToUpper(lname[0]) + lname.Substring(1);

            if (ModelState.IsValid)
            {
                var userAccount = new UserAccount
                {
                    fname = Fname,
                    lname = Lname,
                    userName = username_Passsowrd,
                    passWord = hashData,
                    IsActive = true,
                    dateCreated = DateTime.Now,
                    userType = "Staff"
                };

                _context.UserAccounts.Add(userAccount);
                await _context.SaveChangesAsync();
            }

            vm.activeAccountTab = AccountTab.accountList;
            TempData["Message"] = "Successfully Created";
            return RedirectToAction("Account", vm );
        }

        public async Task<IActionResult> SignUp()
        {

            var staffAcc = await _context.UserAccounts.FirstOrDefaultAsync(u => u.acc_Id == UserId());


            if (!(string.IsNullOrEmpty(staffAcc.fname) && string.IsNullOrEmpty(staffAcc.lname)))
            {
             
                return View(new UserAccountVm
                {
                    userAccount = staffAcc
                });
            }

            if ((string.IsNullOrEmpty(staffAcc.email)
                && string.IsNullOrEmpty(staffAcc.contact)
                && staffAcc.IsPasswordChanged == false))
            {
                return View(new UserAccountVm
                {
                    fromReset = true,
                });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> SignUp(string phoneNumber,string? passWord, string? confirmpassWord, UserAccountVm userAcctVm)
        {
            var userAcct = userAcctVm.userAccount;


            if(userAcct.contact.Length < 11)
            {
                ModelState.AddModelError("", "Contact number below 11 is not valid");
                return View(new UserAccountVm
                {
                    userAccount = userAcct
                });
            }
            //conditions
            if (string.IsNullOrWhiteSpace(passWord) || string.IsNullOrWhiteSpace(confirmpassWord))
            {
                ModelState.AddModelError("", "Please input Password");
                return View( new UserAccountVm{
                    userAccount = userAcct
                });
            }

            
            if (passWord.Count() <= 8)
            {
                ModelState.AddModelError("","Password below 8 characters is invalid");
                return View(new UserAccountVm
                {
                    userAccount = userAcct
                });
            }

            if(passWord != confirmpassWord)
            {
                ModelState.AddModelError("", "Password doesn't match");
                return View(new UserAccountVm
                {
                    userAccount = userAcct
                });
            }

            //check
            var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(u => u.acc_Id == UserId());

            //Any = it doesn’t process or load unnecessary data.
            var emailExists = await _context.UserAccounts
              .AnyAsync(u => u.email == userAcct.email);


            if (emailExists)
            {
                ModelState.AddModelError("","Email address already exist");
                return View(new UserAccountVm
                {
                    userAccount = userAcct
                });
            }
            //if found
            if (userAccount != null)
            {

                var hashing = HashingService.HashData(passWord);

                //updateUser
                userAccount.fname = userAcct.fname;
                userAccount.lname = userAcct.lname;
                userAccount.mname = userAcct.mname;
                userAccount.contact = userAcct.contact;
                userAccount.email = userAcct.email;
                userAccount.passWord = hashing;
                userAccount.IsPasswordChanged = true;

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("PasswordChanged", "true");
                HttpContext.Session.SetString("fullName", $"{userAccount.fname} {userAccount.lname}");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return NotFound();
            }
         
        }

    
    }
}
