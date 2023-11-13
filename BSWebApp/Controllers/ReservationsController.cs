using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BSWebApp.Data;
using BSWebApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using NuGet.Protocol;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;

namespace BSWebApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ResDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public ReservationsController(ResDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated) 
            {
                if (User.IsInRole("Member"))
                {
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                    if (user != null)
                    {
                        var currentUserId = user.Id;

                        var resDBContext = _context.Reservation
                        .Where(r => r.UserId == currentUserId) // Filter reservations for the current user
                        .Include(r => r.Sitting)
                        .Include(r => r.User);

                        return View(await resDBContext.ToListAsync());
                    }
                } else if (User.IsInRole("Staff"))
                {
                    var resDBContext = _context.Reservation
                        .Where(r => r.Status == "Pending")
                        .Include(r => r.Sitting).Include(r => r.User);
                    return View(await resDBContext.ToListAsync());
                } else if (User.IsInRole("Admin"))
                {
                    var resDBContext = _context.Reservation.Include(r => r.Sitting).Include(r => r.User);
                    return View(await resDBContext.ToListAsync());
                }
            } else
            {
                return RedirectToAction("Home");
            }
            return View();
        }
        
        public async Task<IActionResult> CollectUserDetails(AppUser user)
        {
            var userJson = JsonConvert.SerializeObject(user);
                
            // User is not authenticated, create a guest account
            var guestId = GenerateGuestId(); // Generate a unique guestId
            var guestPsw = "#Guest123";

            // Retrieve user details from TempData
            
            if (!string.IsNullOrEmpty(userJson))
            {
                var userDetails = JsonConvert.DeserializeObject<AppUser>(userJson);

                // Create a new AppUser (guest user) and populate with user details
                var guest = new AppUser
                {
                    Id = userDetails.Id,
                    UserName = guestId, // Set username to guestId
                    PasswordHash = guestPsw, // Set the password hash for #guest123
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Phone = userDetails.Phone,
                    Email = userDetails.Email,
                    NormalizedEmail = userDetails.Email,
                    PhoneNumber = userDetails.Phone,
                    // Set other user details here, if needed
                };

                // Add the guest user to the Identity framework
                var result = await _userManager.CreateAsync(guest, guestPsw);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Guest");
                    var guestJson = JsonConvert.SerializeObject(guest);
                    TempData["UserId"] = guestJson; // Convert the guest Id to string
                    return RedirectToAction("Create");
                }
                else
                {
                    // Handle the case where user creation failed
                    return RedirectToAction("GuestFailed"); // Redirect to an error page
                }
            }
            else
            {
                // Handle the case where user details were not found in TempData
                return RedirectToAction("GuestFailed");
            }

            
        }
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> STGuest(AppUser user)
        {
            var userJson = JsonConvert.SerializeObject(user);

            // User is not authenticated, create a guest account
            var guestId = GenerateGuestId(); // Generate a unique guestId
            var guestPsw = "#Guest123";

            // Retrieve user details from TempData

            if (!string.IsNullOrEmpty(userJson))
            {
                var userDetails = JsonConvert.DeserializeObject<AppUser>(userJson);

                // Create a new AppUser (guest user) and populate with user details
                var guest = new AppUser
                {
                    Id = userDetails.Id,
                    UserName = guestId, // Set username to guestId
                    PasswordHash = guestPsw, // Set the password hash for #guest123
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Phone = userDetails.Phone,
                    Email = userDetails.Email,
                    NormalizedEmail = userDetails.Email,
                    PhoneNumber = userDetails.Phone,
                    // Set other user details here, if needed
                };

                // Add the guest user to the Identity framework
                var result = await _userManager.CreateAsync(guest, guestPsw);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Guest");
                    var guestJson = JsonConvert.SerializeObject(guest);
                    TempData["UserId"] = guestJson; // Convert the guest Id to string
                    return RedirectToAction("ResDetails");
                }
                else
                {
                    // Handle the case where user creation failed
                    return RedirectToAction("GuestFailed"); // Redirect to an error page
                }
            }
            else
            {
                // Handle the case where user details were not found in TempData
                return RedirectToAction("GuestFailed");
            }


        }

        public IActionResult Guest()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Create");

            } 
            else {
                return View(); 
            }
        }

        [Authorize(Roles = "Staff")]
        public IActionResult phoneResDetails()
        {
            if (User.IsInRole("Staff"))
            {
                return View();
            }
            return RedirectToAction("Guest");
        }
        [Authorize(Roles = "Staff")]
        public IActionResult ResDetails()
        {
            if (User.IsInRole("Staff"))
            {
                ViewData["SittingList"] = _context.Sitting.Select(s => new SelectListItem
                {
                    Value = s.SittingID.ToString(),
                    Text = $"{s.SittingType} {s.StartTime.ToString("hh\\:mm")} to {s.EndTime.ToString("hh\\:mm")}"
                });
                return View();
            }
            return RedirectToAction("Guest");
        }


        // GET: Reservations/Create
        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["SittingList"] = _context.Sitting.Select(s => new SelectListItem
            {
                Value = s.SittingID.ToString(),
                Text = $"{s.SittingType} {s.StartTime.ToString("hh\\:mm")} to {s.EndTime.ToString("hh\\:mm")}"
            });
            return View();
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Sitting)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReservationsId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult AssignTables(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = _context.Reservation
                .Include(r => r.Sitting)
                .Include(r => r.User)
                .FirstOrDefault(m => m.ReservationsId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Create a new ReservedTables model and set its ReservationId
            var reservedTable = new ReservedTables { ReservationId = id };

            // Create a list of available tables for assignment
            var availableTables = _context.Tables
                .Where(table => !_context.ReservedTables
                .Any(rt => rt.ReservationId == id && rt.tableId == table.Id))
                .ToList();

            ViewData["TableList"] = availableTables
                .Select(table => new SelectListItem
                {
                    Value = table.Id.ToString(),
                    Text = $"Table: {table.TableID}"
                });

            return View("AssignTables", reservedTable);
        }


        [HttpPost]
        public async Task<IActionResult> AssignTables(int id, int tableId, [Bind("ReservationsId,GuestCount,Status,Notes,ReservationDateTime,SittingID,ReservationSource,UserId")] ReservedTables model)
        {
            // Check if the current user is authorized to assign tables
            if (User.IsInRole("Staff") || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the selected reservation by ID
                    var reservation = await _context.Reservation.FindAsync(id);

                    if (reservation == null)
                    {
                        return NotFound();
                    }

                    // Check if the reservation is in a valid state to be confirmed and assigned a table
                    if (reservation.Status == "Pending")
                    {

                        // Set the tableId based on the selected value in the dropdown
                        // Assuming tableId is an int

                        // Check if the selected table is available
                        var isTableAvailable = await _context.ReservedTables
                            .AllAsync(rt => rt.tableId != tableId &&
                                           rt.Reservation.ReservationDateTime == reservation.ReservationDateTime);

                        if (isTableAvailable)
                        {
                           


                            // Manually set the ReservedTables properties
                            var reservedTable = new ReservedTables
                            {
                                ReservationId = id,
                                tableId = tableId
                            };
                        

                            // Update the reservation status to "Confirmed"
                            reservation.Status = "Confirmed";

                            // Save changes to the database
                            _context.ReservedTables.Add(reservedTable);
                            await _context.SaveChangesAsync();

                            // Redirect to a success page or list of reservations
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Handle the case where the selected table is not available
                            return RedirectToAction("TableNotAvailable");
                        }
                    }
                    else
                    {
                        // Handle the case where the reservation is not in a pending state
                        return RedirectToAction("InvalidReservationStatus");
                    }
                }
                else
                {
                    // If model validation fails, return to the form
                    return View("AssignTables", model);
                }
            }

            // Handle unauthorized access here
            return RedirectToAction("AccessDenied");
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationsId,GuestCount,Status,Notes,ReservationDateTime,SittingID,UserId,Duration")] Reservation reservation)
        {
            // Set reservationSource and status
            reservation.ReservationSource = "Website";
            reservation.Status = "Pending";

            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                reservation.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Use the authenticated user's UserId
            }
            else
            {
                string guestJson = TempData["UserId"] as string;
                if (!string.IsNullOrEmpty(guestJson))
                {
                    var guest = JsonConvert.DeserializeObject<AppUser>(guestJson);
                    reservation.UserId = guest.Id;
                   
                }

            }

            // Fetch the selected sitting
            var selectedSitting = _context.Sitting.FirstOrDefault(s => s.SittingID == reservation.SittingID);

            if (selectedSitting == null)
            {
                // Handle the case where the selected sitting is not found
                return RedirectToAction("SittingNotFound");
            }

            // Extract the time portion of ReservationDateTime
            var reservationTime = reservation.ReservationDateTime.TimeOfDay;

            // Check if the reservation time is within the sitting time range
            if (reservationTime < selectedSitting.StartTime || reservationTime > selectedSitting.EndTime)
            {
                // Reservation time is not within the sitting time range
                ModelState.AddModelError("ReservationDateTime", "Reservation time must be within the sitting time range.");
                ViewData["SittingList"] = _context.Sitting.Select(s => new SelectListItem
                {
                    Value = s.SittingID.ToString(),
                    Text = $"{s.SittingType} {s.StartTime.ToString("hh\\:mm")} to {s.EndTime.ToString("hh\\:mm")}"
                });
                return View(reservation);
            }

            // Set ViewBag for the selected sitting
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                var resId = reservation.ReservationsId;
                return RedirectToAction("CompletePage", new {resId = resId}); // Redirect to a confirmation page
            } else
            {
                Debug.WriteLine(ModelState.Values.ToJson());
            }

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> STRes([Bind("ReservationsId,GuestCount,Status,Notes,ReservationDateTime,SittingID,UserId,Duration,ReservationSource")] Reservation reservation)
        {
            // Set reservationSource and status
            ;
            reservation.Status = "Pending";


            string guestJson = TempData["UserId"] as string;
            if (!string.IsNullOrEmpty(guestJson))
            {
                var guest = JsonConvert.DeserializeObject<AppUser>(guestJson);
                reservation.UserId = guest.Id;

            }
            // Fetch the selected sitting
            var selectedSitting = _context.Sitting.FirstOrDefault(s => s.SittingID == reservation.SittingID);

            if (selectedSitting == null)
            {
                // Handle the case where the selected sitting is not found
                return RedirectToAction("SittingNotFound");
            }

            // Extract the time portion of ReservationDateTime
            var reservationTime = reservation.ReservationDateTime.TimeOfDay;

            // Check if the reservation time is within the sitting time range
            if (reservationTime < selectedSitting.StartTime || reservationTime > selectedSitting.EndTime)
            {
                // Reservation time is not within the sitting time range
                ModelState.AddModelError("ReservationDateTime", "Reservation time must be within the sitting time range.");
                ViewData["SittingList"] = _context.Sitting.Select(s => new SelectListItem
                {
                    Value = s.SittingID.ToString(),
                    Text = $"{s.SittingType} {s.StartTime.ToString("hh\\:mm")} to {s.EndTime.ToString("hh\\:mm")}"
                });
                return View(reservation);
            }

            // Set ViewBag for the selected sitting
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                var resId = reservation.ReservationsId;
                return RedirectToAction("CompletePage", new { resId = resId }); // Redirect to a confirmation page
            }
            else
            {
                Debug.WriteLine(ModelState.Values.ToJson());
            }

            return View(reservation);
        }

        private static readonly Random random = new Random();
        private const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateGuestId()
        {
            char[] guestId = new char[12];
            for (int i = 0; i < guestId.Length; i++)
            {
                guestId[i] = allowedChars[random.Next(0, allowedChars.Length)];
            }
            return new string(guestId);
        }

        public IActionResult CompletePage(Reservation reservation, int resId)
        {
            // Retrieve the reservation based on the provided resId, including related data
            reservation = _context.Reservation
                .Include(r => r.Sitting)
                .Include(r => r.User)
                .FirstOrDefault(r => r.ReservationsId == resId);

            if (reservation != null)
            {
                return View(reservation); // Pass the reservation object to the view
            }

            // Handle the case where the reservation information is not found
            return View("ReservationNotFound"); // You can create a view for this scenario
        }


        // GET: Reservations/Edit/5
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["SittingID"] = new SelectList(_context.Sitting, "SittingID", "SittingType", reservation.SittingID);
            //ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationsId,GuestCount,Status,Notes,ReservationDateTime,SittingID,ReservationSource,UserId")] Reservation reservation)
        {
            if (id != reservation.ReservationsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationsId))
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
            ViewData["SittingID"] = new SelectList(_context.Sitting, "SittingID", "SittingType", reservation.SittingID);
           // ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Sitting)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReservationsId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, [Bind("ReservationsId,GuestCount,Status,Notes,ReservationDateTime,SittingID,ReservationSource,UserId")] Reservation reservation)
        {
            if (id != reservation.ReservationsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reservation.Status = "Cancled";
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationsId))
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
            ViewData["SittingID"] = new SelectList(_context.Sitting, "SittingID", "SittingType", reservation.SittingID);
            // ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "Id", reservation.UserId);
            return View(reservation);
        }





        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'ResDBContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Reservation?.Any(e => e.ReservationsId == id)).GetValueOrDefault();
        }

       

    }
}
