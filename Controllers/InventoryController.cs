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
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventorie
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.inventory.Include(i => i.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inventorie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.inventory == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventory
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.inventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventorie/Create
        public IActionResult Create()
        {
            ViewData["Name"] = new SelectList(_context.products, "ProductId", "Name");
            return View();
        }

        // POST: Inventorie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("inventoryId,ProductId,Quantity,WarehouseLocation")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "ProductId", inventory.ProductId);
            return View(inventory);
        }

        // GET: Inventorie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.inventory == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventory.FindAsync(id);

       //     var inventory = await _context.inventory
       //.Include(i => i.Product) // Assuming the navigation property is named "Product"
       //.FirstOrDefaultAsync(i => i.ProductId == id);

            if (inventory == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "Name", inventory.ProductId);
            return View(inventory);
        }

        // POST: Inventorie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("inventoryId,ProductId,Quantity,WarehouseLocation")] Inventory inventory)
        {
            if (id != inventory.inventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.inventoryId))
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
            ViewData["ProductId"] = new SelectList(_context.products, "ProductId", "ProductId", inventory.ProductId);
            return View(inventory);
        }

        // GET: Inventorie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.inventory == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventory
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.inventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.inventory == null)
            {
                return Problem("Entity set 'ApplicationDbContext.inventory'  is null.");
            }
            var inventory = await _context.inventory.FindAsync(id);
            if (inventory != null)
            {
                _context.inventory.Remove(inventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
          return (_context.inventory?.Any(e => e.inventoryId == id)).GetValueOrDefault();
        }
    }
}
