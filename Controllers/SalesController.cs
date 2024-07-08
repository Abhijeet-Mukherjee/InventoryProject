using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryProject.Data;
using InventoryProject.services;

namespace InventoryProject.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        public SalesController(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;   
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sales.Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            var products = _context.products
        .Join(
            _context.inventory,
            p => p.ProductId,
            inv => inv.ProductId,
            (p, inv) => new { Product = p, Inventory = inv }
        )
        .Where(joined => joined.Inventory.Quantity > 0)
        .Select(joined => joined.Product)
        .ToList();

            ViewData["ProductId"] = new SelectList(products, "ProductId", "Name");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("Customer,Id,ProductId,Timestamp,QuantitySold,TotalAmount")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                var inventory = await _context.inventory.FirstOrDefaultAsync(inv => inv.ProductId == sale.ProductId);

                if (inventory == null || sale.QuantitySold > inventory.Quantity)
                {
                    ViewBag.ErrorMessage = "Quantity should not be more than available.";
                    ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "Name", sale.ProductId);
                    return View(sale);
                }

                _context.Add(sale);
                await _context.SaveChangesAsync();

                bool stockUpdated = await UpdateStock(sale.ProductId, sale.QuantitySold);
                if (!stockUpdated)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "Name", sale.ProductId);
            return View(sale);
        }

        private async Task<bool> UpdateStock(int productId, int quantitySold)
        {
            // Find the inventory entry for the product
            var inventory = await _context.inventory
                                        .SingleOrDefaultAsync(i => i.ProductId == productId);

            // If inventory entry not found, product is not in stock
            if (inventory == null)
            {
                return false; // Product not found in inventory
            }

            // Check if there is sufficient quantity to sell
            if (inventory.Quantity < quantitySold)
            {
                return false; // Insufficient stock
            }

            // Reduce the quantity in inventory by quantitySold
            inventory.Quantity -= quantitySold;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "ProductId", sale.ProductId);
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Customer,Id,ProductId,Timestamp,QuantitySold,TotalAmount")] Sale sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id))
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
            ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "ProductId", sale.ProductId);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sales == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sales'  is null.");
            }
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
          return (_context.Sales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
