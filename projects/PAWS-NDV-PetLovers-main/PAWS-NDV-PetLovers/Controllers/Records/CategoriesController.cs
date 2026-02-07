using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Controllers.Records
{
    [ServiceFilter(typeof(AuthFilter))]
    public class CategoriesController : Controller
    { 
        //dependency Injection
        private readonly PAWS_NDV_PetLoversContext _context;

        public CategoriesController(PAWS_NDV_PetLoversContext context)
        {
            this._context = context;
        }

        //Get
        public async Task<IActionResult> Index()
        {


            return View(await _context.Categories.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,categoryName,description,registeredDate,Products")] Category category)
        {

            var registerDate = DateTime.Now;

            if (category == null)
             {
                 return NotFound();
             }

             if (CategoryExist(category.categoryName))
             {
                 ModelState.AddModelError("", "Category already exist");
                 return View(category);

             }


            /*check product*/
            // Use a HashSet to track pet names and check for duplicates
            var products = new HashSet<string>();

            foreach (var product in category.Products)
            {
                if (!products.Add(product.productName))
                {
                    ModelState.AddModelError("",$"Duplicate Product '{product.productName}' is invalid ");
                    return View(category);
                }
            }

            //add registerDate individualyy
            foreach(var product in category.Products)
            {
                product.registeredDate = registerDate;
            }

            _context.Add(category);
            await _context.SaveChangesAsync();


            var stockAdjustments = new List<StockAdjustment>();
            foreach (var product in category.Products)
            {
                //Entry
                var stockAdjustment = new StockAdjustment
                {
                    source = "Order",
                    date = (DateTime)product.registeredDate,
                    stock = (int)product.quantity,
                    productId = product.id,
                };
                stockAdjustments.Add(stockAdjustment);

            }
            _context.StockAdjustments.AddRange(stockAdjustments);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Successfully Created!";

                return RedirectToAction(nameof(Index));
        }

        #region == function == 
        private bool CategoryExist(string categoryName)
        {
            return _context.Categories.Any(c => c.categoryName == categoryName);

        }
        private bool ProductExist(string productName)
        {
            return _context.Products.Any(c => c.productName == productName);

        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
 
            //retrieve related products
            var updatedCategory = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.id == id);
            
            if(updatedCategory == null)
            {
                return NotFound();
            }

            return View(updatedCategory);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,[Bind("id,categoryName,description,lastUpdate")]Category category)
        {
            //id check
            if(id == null)
            {
                return NotFound();
            }
            var findCategory = await _context.Categories.FindAsync(id);

            if (findCategory == null)
            {
                return NotFound();
            }


            try
            {

                findCategory.lastUpdate = DateTime.Now;
                findCategory.categoryName = category.categoryName;
                findCategory.description = category.description;

                _context.Update(findCategory);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully Updated";

      
                return RedirectToAction("Index");
            }
            catch(DbUpdateConcurrencyException)
            {
                //verify to the database
                if (!CategoryExist(category.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }
        //check if Id exist

        private bool CategoryExist(int id)
        {
            return _context.Categories.Any(c => c.id == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
            return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);   

            if(category == null)
            {
            return NotFound();
            }

            return View(category);

        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.id == id);
            
            if(category == null)
            {
                return NotFound();
            }

            return View(category);


        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }

}
