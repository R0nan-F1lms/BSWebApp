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
    public class ReservedTablesController : Controller
    {
        private readonly ResDBContext _context;

        public ReservedTablesController(ResDBContext context)
        {
            _context = context;
        }

        // GET: ReservedTables
        public async Task<IActionResult> Index()
        {
            var resDBContext = _context.ReservedTables.Include(r => r.Reservation).Include(r => r.Tables);
            return View(await resDBContext.ToListAsync());
        }

        // GET: ReservedTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ReservedTables == null)
            {
                return NotFound();
            }

            var reservedTables = await _context.ReservedTables
                .Include(r => r.Reservation)
                .Include(r => r.Tables)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservedTables == null)
            {
                return NotFound();
            }

            return View(reservedTables);
        }

        // GET: ReservedTables/Create
        public IActionResult Create()
        {
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationsId", "ReservationsId");
            ViewData["tableId"] = new SelectList(_context.Tables, "Id", "Id");
            return View();
        }

        // POST: ReservedTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,tableId")] ReservedTables reservedTables)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservedTables);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationsId", "ReservationsId", reservedTables.ReservationId);
            ViewData["tableId"] = new SelectList(_context.Tables, "Id", "Id", reservedTables.tableId);
            return View(reservedTables);
        }

        // GET: ReservedTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReservedTables == null)
            {
                return NotFound();
            }

            var reservedTables = await _context.ReservedTables.FindAsync(id);
            if (reservedTables == null)
            {
                return NotFound();
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationsId", "ReservationsId", reservedTables.ReservationId);
            ViewData["tableId"] = new SelectList(_context.Tables, "Id", "Id", reservedTables.tableId);
            return View(reservedTables);
        }

        // POST: ReservedTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,tableId")] ReservedTables reservedTables)
        {
            if (id != reservedTables.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservedTables);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservedTablesExists(reservedTables.ReservationId))
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
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationsId", "ReservationsId", reservedTables.ReservationId);
            ViewData["tableId"] = new SelectList(_context.Tables, "Id", "Id", reservedTables.tableId);
            return View(reservedTables);
        }

        // GET: ReservedTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ReservedTables == null)
            {
                return NotFound();
            }

            var reservedTables = await _context.ReservedTables
                .Include(r => r.Reservation)
                .Include(r => r.Tables)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservedTables == null)
            {
                return NotFound();
            }

            return View(reservedTables);
        }

        // POST: ReservedTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ReservedTables == null)
            {
                return Problem("Entity set 'ResDBContext.ReservedTables'  is null.");
            }
            var reservedTables = await _context.ReservedTables.FindAsync(id);
            if (reservedTables != null)
            {
                _context.ReservedTables.Remove(reservedTables);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservedTablesExists(int id)
        {
          return (_context.ReservedTables?.Any(e => e.ReservationId == id)).GetValueOrDefault();
        }
    }
}
