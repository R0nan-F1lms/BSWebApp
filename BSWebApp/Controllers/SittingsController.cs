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
    public class SittingsController : Controller
    {
        private readonly ResDBContext _context;

        public SittingsController(ResDBContext context)
        {
            _context = context;
        }

        // GET: Sittings
        public async Task<IActionResult> Index()
        {
              return _context.Sitting != null ? 
                          View(await _context.Sitting.ToListAsync()) :
                          Problem("Entity set 'ResDBContext.Sitting'  is null.");
        }

        // GET: Sittings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sitting == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sitting
                .FirstOrDefaultAsync(m => m.SittingID == id);
            if (sitting == null)
            {
                return NotFound();
            }

            return View(sitting);
        }

        // GET: Sittings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sittings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SittingID,SittingType,StartTime,EndTime")] Sitting sitting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sitting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sitting);
        }

        // GET: Sittings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sitting == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sitting.FindAsync(id);
            if (sitting == null)
            {
                return NotFound();
            }
            return View(sitting);
        }

        // POST: Sittings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SittingID,SittingType,StartTime,EndTime")] Sitting sitting)
        {
            if (id != sitting.SittingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sitting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SittingExists(sitting.SittingID))
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
            return View(sitting);
        }

        // GET: Sittings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sitting == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sitting
                .FirstOrDefaultAsync(m => m.SittingID == id);
            if (sitting == null)
            {
                return NotFound();
            }

            return View(sitting);
        }

        // POST: Sittings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sitting == null)
            {
                return Problem("Entity set 'ResDBContext.Sitting'  is null.");
            }
            var sitting = await _context.Sitting.FindAsync(id);
            if (sitting != null)
            {
                _context.Sitting.Remove(sitting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SittingExists(int id)
        {
          return (_context.Sitting?.Any(e => e.SittingID == id)).GetValueOrDefault();
        }
    }
}
