using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Appointments;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Records
{
    [ServiceFilter(typeof(AuthFilter))]
    public class OwnersController : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public OwnersController(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }


        //pet area
        #region == Pets code area || EDIT > ADD NEW PET == 

        public async Task<IActionResult> addNewPet([Bind("id,petName,species,breed,bdate,age,color,gender,registeredDate,ownerId")] Pet pet)
        {
            if(pet == null)
            {
                return View(pet);
            }
            
            if (PetExists(pet.petName, pet.species ,pet.breed, pet.ownerId))
            {
                ModelState.AddModelError("", "Pet exist Already");

                //for jquery code
                return Json(new { success = false, message = "Pet Exist Already" });
            }

            if (pet != null)
            {
                //for modal

                _context.Add(pet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pet added successfully!";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid data" });


        }


        private bool PetExists(string petName, string specie,string breed, int ownerId)
        {
            return _context.Pets.Any(e => e.petName == petName && e.species == specie  && e.breed == breed &&
           e.ownerId == ownerId);
            /* return _context.Pets.Any(e => e.id == pet.id && e.petName == pet.petName &&
             e.bdate == pet.bdate && e.age == pet.age && e.breed == pet.breed && e.ownerId == pet.ownerId);*/
        }
        private bool PetExistsById(int id)
        {
            return _context.Pets.Any(e => e.id == id);
        }

        #endregion


        // GET: Owners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Owners.ToListAsync());
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create 
        //variables inside the parameters are the receiver of appointment route*
        public IActionResult Create(string? fname, string? lname, string? mname, string? contact, string? btnCnl)
        {

            var date = DateTime.Now;
            var today = date.Date;

           
            var owner = new Owner
            {
                registeredDate = today,
                fname = fname,
                lname = lname,
                mname = mname,
                contact = contact
            };

            //pass to vm
            var vm = new RecordsVm
            {
                Owner = owner,
                btnCnl = btnCnl //for cancel button if came from appointment, pass to view
            };

            return View(vm);
        }


        // POST: Owners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CreatePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string btnCnl,[Bind("id,fname,lname,mname,gender,contact,email,address,registeredDate,Pets")] Owner owner)
        {

            var vm = new RecordsVm
            {
                Owner = owner,
                btnCnl = btnCnl //get from view
            };

            var verifyOwner = _context.Owners
                .Where(m => m.fname == owner.fname && m.lname == owner.lname && m.mname == owner.mname)
                .FirstOrDefault();

            if (verifyOwner == null)
            {
                if (owner.contact.Length < 11)
                {
                    ModelState.AddModelError("", "Contact number below 11 is not valid");

                    return View("Create", vm);
                }

                // Use a HashSet to track pet names and check for duplicates
                var petNames = new HashSet<string>();

                foreach (var pet in owner.Pets)
                {
                    if (!petNames.Add(pet.petName))
                    {
                        ModelState.AddModelError("", $"Duplicate pet name '{pet.petName}' is invalid");
                        return View("Create", vm);
                    }
                }

                _context.Add(owner);
                await _context.SaveChangesAsync(); 

                // Get the newly created owner's ID
                var newOwnerId = owner.id;

                // Check if there is an appointment that matches the owner's details but does not yet have an ownerId
                var ownerExistAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.AppointId.ToString())
                        && x.fname == owner.fname
                        && x.lname == owner.lname
                        && x.mname == owner.mname
                        && string.IsNullOrEmpty(x.remarks));

                if (ownerExistAppointment != null)
                {
                    // Update the appointment's ownerId
                    ownerExistAppointment.ownerId = newOwnerId;
                    _context.Update(ownerExistAppointment);
                    await _context.SaveChangesAsync(); // Save changes to the appointment

                    var vcm = new AppointmentVm
                    {
                        created = true
                    };
                    return RedirectToAction("Dashboard", "Appointments", vcm); // Redirect to another controller
                }

                TempData["SuccessMessage"] = "Successfully Created";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Owner already exists");
            }

                
            return View("Create", vm);
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id, string? btnCnl)
        {


            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);

            if (owner == null)
            {
                return NotFound();
            }

            // Retrieve related pets
            var updatedOwner = await _context.Owners
                                    .Include(o => o.Pets)
                                    .FirstOrDefaultAsync(o => o.id == id);

            //view Model Instantiations for VIEWS
            var data = new RecordsVm
            {
                Owner = updatedOwner,
                IPets = updatedOwner.Pets,
                btnCnl = btnCnl
            };


            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> EditOwner(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
       
        }

    // POST: Owners/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOwner(int id, [Bind("id,fname,lname,mname,gender,contact,email,address,registeredDate,lastUpdate,Pets")] Owner owner)
        {
            if (id != owner.id)
            {
                return NotFound();
            }

            if(string.IsNullOrEmpty(owner.contact) || owner.contact.Length < 11)
            {
                ModelState.AddModelError("","Please input a contact number correctly");

                // Retrieve the updated data to redisplay the form
                var owners = await _context.Owners
                                        .Include(o => o.Pets)
                                        .FirstOrDefaultAsync(o => o.id == id);

           
                return View(owners);
            }

            // Update the lastUpdate column
            owner.lastUpdate = DateTime.Now;

            try
            {
                    _context.Update(owner);
                await _context.SaveChangesAsync();

                // Retrieve the updated data to redisplay the form
                var updatedOwner = await _context.Owners
                                        .Include(o => o.Pets)
                                        .FirstOrDefaultAsync(o => o.id == id);

                var data = new RecordsVm
                {
                    Owner = updatedOwner,
                    IPets = updatedOwner.Pets
                };
                TempData["SuccessMessage"] = "Successfully Updated";

                return RedirectToAction("Edit", new {id = owner.id } );
          
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(owner.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        //edit pet button
        [HttpGet]
        public async Task<IActionResult> EditPet(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            // Assuming you want to edit a specific pet in the context of a client
            var clientVm = new RecordsVm
            {
                Pet = await _context.Pets.FindAsync(id)
            };

            if (clientVm.Pet == null)
            {
                return NotFound();
            }

            return View(clientVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPet(int? id, [Bind("id","petName","specie","breed","bdate","color",
            "registeredDate","gender")] Pet pet)
        {
            if (id != pet.id)
            {
                return NotFound(id);
            }

            try
            {
                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!PetExistsById(pet.id))
                {
                    return NotFound();
                }
            }

            //converting into Client View Models

            //vm instantiation
            var viewModel = new RecordsVm
            {
                Pet = pet

            };
           
            return View(viewModel);
        }


        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner != null)
            {
                _context.Owners.Remove(owner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //checking if id exist in the db
        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.id == id);
        }


    }
}

