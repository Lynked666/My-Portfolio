using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Records
{
    [ServiceFilter(typeof(AuthFilter))]
    public class PetsController : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public PetsController(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ViewHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Fetched selected diagnostic ID
            var findDiagnostic = await _context.Diagnostics
                .Include(d => d.pet)
                .ThenInclude(d => d.owner)
                .Include(d => d.IdiagnosticDetails)
                .ThenInclude(dd => dd.Services)
                .FirstAsync(d => d.diagnostic_Id == id);

            TransactionsVm tvm = new TransactionsVm
            {
                Diagnostics = findDiagnostic
            };

            return View(tvm);
        }

        [HttpGet]
        public async Task<IActionResult> DiagnosHistory(int? id, int? ownerId)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Fetched selected diagnostic ID
            var findDiagnostic = await _context.Diagnostics
                .Include(d => d.pet)
                .ThenInclude(d => d.owner)
                .Include(d => d.IdiagnosticDetails)
                .ThenInclude(dd => dd.Services)
                .Where(d => d.petId == id && !string.IsNullOrEmpty(d.status)).ToListAsync();


            // Pass ownerId to the view using ViewBag or ViewData
            ViewBag.OwnerId = ownerId;

            return View(findDiagnostic);
        }

        // GET: Pets
        public async Task<IActionResult> Index()
        {
            var pAWS_NDV_PetLoversContext = _context.Pets.Include(p => p.owner);
            return View(await pAWS_NDV_PetLoversContext.ToListAsync());
        }

        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.owner)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: Pets/Create
        public IActionResult Create()
        {
            ViewData["ownerId"] = new SelectList(_context.Owners, "id", "address");
            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,petName,species,breed,bdate,age,color,gender,registeredDate,ownerId")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ownerId"] = new SelectList(_context.Owners, "id", "address", pet.ownerId);
            return View(pet);
        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);

            if (pet == null)
            {
                return NotFound();
            }

        
            return View(pet);
        }

        // POST: Pets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,petName,species,breed,bdate,age,color,gender,registeredDate,lastUpdate,ownerId")] Pet pet)
        {
            if (id != pet.id)
            {
                return NotFound();
            }

            try
            {
                pet.lastUpdate = DateTime.Now;

                _context.Update(pet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully Updated";

                return RedirectToAction("Edit", "Owners", new { id = pet.ownerId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(pet.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.owner)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.id == id);
        }
    }
}
