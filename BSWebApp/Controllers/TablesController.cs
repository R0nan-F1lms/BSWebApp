using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BSWebApp.Data;
using BSWebApp.Models;

namespace BSWebApp.Controllers
{
    public class TablesController : Controller
    {
        private readonly ResDBContext _context;

        public TablesController(ResDBContext context)
        {
            _context = context;
        }

        // GET: Tables
        public async Task<IActionResult> Index()
        {
            var resDBContext = _context.Tables.Include(t => t.Area);
            return View(await resDBContext.ToListAsync());
        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var tables = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tables == null)
            {
                return NotFound();
            }

            return View(tables);
        }

        // GET: Tables/Create
        public IActionResult Create()
        {
            ViewData["AreaID"] = new SelectList(_context.Area, "AreaId", "AreaId");
            return View();
        }

        // POST: Tables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TableID,AreaID,TableStatus")] Tables tables)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tables);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaID"] = new SelectList(_context.Area, "AreaId", "AreaId", tables.AreaID);
            return View(tables);
        }

        // GET: Tables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var tables = await _context.Tables.FindAsync(id);
            if (tables == null)
            {
                return NotFound();
            }
            ViewData["AreaID"] = new SelectList(_context.Area, "AreaId", "AreaId", tables.AreaID);
            return View(tables);
        }

        // POST: Tables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TableID,AreaID,TableStatus")] Tables tables)
        {
            if (id != tables.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tables);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TablesExists(tables.Id))
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
            ViewData["AreaID"] = new SelectList(_context.Area, "AreaId", "AreaId", tables.AreaID);
            return View(tables);
        }

        // GET: Tables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var tables = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tables == null)
            {
                return NotFound();
            }

            return View(tables);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tables == null)
            {
                return Problem("Entity set 'ResDBContext.Tables'  is null.");
            }
            var tables = await _context.Tables.FindAsync(id);
            if (tables != null)
            {
                _context.Tables.Remove(tables);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TablesExists(int id)
        {
          return (_context.Tables?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
