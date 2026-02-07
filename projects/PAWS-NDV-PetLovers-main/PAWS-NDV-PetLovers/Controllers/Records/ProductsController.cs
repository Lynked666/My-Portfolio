using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.Models.Records;
using PAWS_NDV_PetLovers.ViewModels;
using System.Drawing;

namespace PAWS_NDV_PetLovers.Controllers.Records
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ProductsController : Controller
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public ProductsController(PAWS_NDV_PetLoversContext context)
        {
            this._context = context;
        }



        //Stock Adjustments
        public async Task<IActionResult> StockAdjustment()
        {
            var stocks =  new RecordsVm
            {
                IstockAdjustment = await _context.StockAdjustments.Include(s => s.productsNav).ToListAsync()
            };

            return View(stocks);
        }


        public async Task<IActionResult> Index()
        {

            //including category navigation to display each name
            var categoryInclude = _context.Products.Include(p => p.category);

            return View(await categoryInclude.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            //to fill the selectList for categoryId
            await SetCategoryListAsync();
         
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,productName,supplierPrice,sellingPrice,quantity,registeredDate,CategoryId")] Product product)
        {
            if (product == null)
            {
                // Handle the null product scenario
                ModelState.AddModelError("", "Product cannot be null.");
                await SetCategoryListAsync();
                return View(product);
            }


            // if inputs are null, this is the solution
            try
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully Created ";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                if (ProductExistByName(product.productName))
                {
                    await SetCategoryListAsync();
                    ModelState.AddModelError("", $"Product name '{product.productName}' already exists.");

                    await SetCategoryListAsync();
                    return View(product);
                }
            }
            await SetCategoryListAsync();
            return View(product);
      
        }
    

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            await SetCategoryListAsync();

            if (id == null)
            {
                return NotFound();
            }

            var editProduct = await _context.Products.FindAsync(id);

            if(editProduct == null)
            {
                return NotFound();
            }

            return View(editProduct);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            await SetCategoryListAsync();

            // Check if addQnty has a value
            if (product.stockAdjustmentNav[0].stock != 0 )
            {
                //In the view, only the stock was inputed, here's the remaining necessary column that needs to be updated.
                product.stockAdjustmentNav[0].productId = product.id;
                product.stockAdjustmentNav[0].date = DateTime.Now;
                product.stockAdjustmentNav[0].source = "Order";

                // Update the quantity in the product
                product.quantity += product.stockAdjustmentNav[0].stock;

                _context.StockAdjustments.Add(product.stockAdjustmentNav[0]);
            }

            // Update the last update time
            product.lastUpdate = DateTime.Now;

            // Save the updated product
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Successfully Updated";
            return RedirectToAction("Index");
        }   

   
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int? id)
        {
         
            var products = await _context.Products.FindAsync(id);

            if(products != null)
            {
                _context.Products.Remove(products);

                TempData["SuccessMessage"] = "Deleted Successfully";
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        
        }


        #region == funtions ==
        private async Task SetCategoryListAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryList = new SelectList(categories, "id", "categoryName");
        }
        private bool ProductExistByName(string? productName)
        {
            return _context.Products.Any(p => p.productName == productName);
        }

        private bool ProductExist(int id)
        {
            return _context.Products.Any(p => p.id == id);
        }
        #endregion
    }

}
