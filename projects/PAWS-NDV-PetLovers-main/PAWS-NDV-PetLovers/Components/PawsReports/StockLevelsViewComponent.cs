using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAWS_NDV_PetLovers.Data;
using PAWS_NDV_PetLovers.ViewModels;

namespace PAWS_NDV_PetLovers.Components.ProductMgmt
{
    public class StockLevelsViewComponent:ViewComponent
    {
        private readonly PAWS_NDV_PetLoversContext _context;

        public StockLevelsViewComponent(PAWS_NDV_PetLoversContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportsVm vcm)
        {
            await GetAllCategory();

            if (vcm.SelectType != "allCategory")
            {
                switch (vcm.Status)
                {
                    case "Low":
                        await GetSelectedCategory(vcm.SelectType);
                        return View(new ReportsVm
                        {
                            IProducts = await _context.Products
                            .Include(p => p.category)
                            .Where(p => p.quantity <= 10 && p.category.categoryName == vcm.SelectType)
                            .ToListAsync(),
                            Status = vcm.Status,
                            SelectType = vcm.SelectType,
                            Filtered = vcm.Filtered,
                            activeReportTab = ReportTab.stockLevel
                        });

                    case "High":
                        await GetSelectedCategory(vcm.SelectType);

                        return View(new ReportsVm
                        {
                            IProducts = await _context.Products
                            .Include(p => p.category)
                            .Where(p => p.quantity >= 11 && p.category.categoryName == vcm.SelectType)
                            .ToListAsync(),
                            Status = vcm.Status,
                            SelectType = vcm.SelectType,
                            Filtered = vcm.Filtered,
                            activeReportTab = ReportTab.stockLevel
                        });


                    case "All":
                        await GetSelectedCategory(vcm.SelectType);
                        return View(new ReportsVm
                        {
                            IProducts = await _context.Products
                            .Include(p => p.category)
                            .Where(p => p.category.categoryName == vcm.SelectType)
                            .ToListAsync(),
                            Status = vcm.Status,
                            SelectType = vcm.SelectType,
                            Filtered = vcm.Filtered,
                            activeReportTab = ReportTab.stockLevel
                        });
                }
            }
            else
            {
                //SelectType == allCategory
                switch (vcm.Status)
                {
                    case "Low":
                        return View(new ReportsVm
                        {
                            IProducts = await _context.Products
                           .Where(p => p.quantity <= 10)
                           .Include(p => p.category)
                           .ToListAsync(),
                            Status = vcm.Status,
                            SelectType = vcm.SelectType,
                            Filtered = vcm.Filtered,
                            activeReportTab = ReportTab.stockLevel
                        });
                        
                    case "High":
                
                        return View( new ReportsVm
                        {
                            IProducts = await _context.Products
                            .Where(p => p.quantity >= 11)
                            .Include(p => p.category)
                            .ToListAsync(),
                            Status = vcm.Status,
                            SelectType = vcm.SelectType,
                            Filtered = vcm.Filtered,
                            activeReportTab = ReportTab.stockLevel
                        });
                      

                    case "All":
                       
                        return View(new ReportsVm
                        {
                            IProducts = await _context.Products.ToListAsync(),
                            Status = vcm.Status,
                            SelectType = vcm.SelectType,
                            Filtered = vcm.Filtered,
                            activeReportTab = ReportTab.stockLevel
                        });
                }
            }
            return View();
        }

        #region == functions ==
        public async Task GetAllCategory()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryList = new SelectList(categories, "categoryName", "categoryName");
        }

        public async Task GetSelectedCategory(string selectType)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryList = new SelectList(categories, "categoryName", "categoryName", selectType);
        }
        #endregion
    }
}
