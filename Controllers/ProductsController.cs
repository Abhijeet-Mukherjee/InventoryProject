using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryProject.Data;
using InvertoryProject.Data;

namespace InventoryProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return _context.products != null ? 
                          View(await _context.products.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.products'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,CreatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }
       
            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,CreatedDate,Percentage_change")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string a = Request.Form["selectedOption"].ToString();
                    if (a == "bypercentage")
                    {
                        decimal val1 = Convert.ToDecimal(Request.Form["Percentage_change"]);
                        decimal val2 = product.Price- (product.Price * val1 / 100);
                        product.Price = Convert.ToInt32(val2);
                    }
                    else if (a == "byamount")
                    {
                       
                    }
                    
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.products'  is null.");
            }
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                _context.products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }


        //public async Task AdjustProductPrice(int id, decimal adjustment, bool isPercentage)
        //{
        //    var product = await _context.products.FindAsync(id);
        //    if (product == null) return;

        //    if (isPercentage)
        //    {
        //        product.Price -= product.Price * (adjustment / 100);
        //    }
        //    else
        //    {
        //        product.Price -= adjustment;
        //    }

        //    if (product.Price < 0)
        //    {
        //        product.Price = 0;
        //    }

        //    _context.Products.Update(product);
        //    await _context.SaveChangesAsync();
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDiscount()
        {
            string a = Request.Form["selectedOption"].ToString();
            int newPrice = Convert.ToInt32(Request.Form["new_val"]);
            if (ModelState.IsValid)
            {

                var products = await _context.products.ToListAsync();  

                foreach (var product in products)
                {
                    if (a == "bypercentage")
                    {
                       
                        decimal val = product.Price - (product.Price * newPrice / 100);
                        product.Price = Convert.ToInt32(val);
                    }
                    else if (a == "byamount")
                    {
                        product.Price = product.Price - newPrice;
                    }

                      
                    _context.Update(product);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));  
            }         
            return View(newPrice);
        }


        public async Task<IActionResult> Search(string productName)
        {
            var products = await _context.products
                                    .Where(p => string.IsNullOrEmpty(productName) || p.Name.Contains(productName))
                                    .ToListAsync();

            return View("Index", products);
        }


    }
}
