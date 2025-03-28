using Catalogue.Data;
using Catalogue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.Controllers
{
    public class UserController :Controller
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.created_at = DateTime.Now;
                _context.Add(user);  // Adaugă utilizatorul în context
                await _context.SaveChangesAsync();  // Salvează modificările în baza de date
                return RedirectToAction(nameof(Index));  // După salvare, redirecționează spre Index
            }
            return View(user);
        }

        // GET: User/Index - Afișează utilizatorii din baza de date
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());  // Afișează lista de utilizatori
        }
    }
}
