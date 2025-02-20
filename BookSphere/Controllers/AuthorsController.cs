using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookSphere.Data;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace BookSphere.Controllers
{
    [Authorize]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
        .Include(a => a.BookAuthors)
        .ThenInclude(ba => ba.Book)
        .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,DateOfBirth")] Author author)
        {
            if (ModelState.IsValid)
            { 
                if (_context.Authors.FirstOrDefault(a => a.FullName == author.FullName) == null)
                {
                    if(author.DateOfBirth > DateTime.Now)
                    {
                        ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future.");
                        return View(author);
                    }
                    _context.Add(author);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("FullName", "Author already exists.");
                }
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,DateOfBirth")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Authors.Any(a => a.FullName == author.FullName && a.Id != author.Id))
                    {
                        ModelState.AddModelError("FullName", "Author with this name already exists.");
                        return View(author);
                    }
                    if (author.DateOfBirth > DateTime.Now)
                    {
                        ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future.");
                        return View(author);
                    }
                    if (_context.Books.Any(b => b.Author == author.FullName && b.PublicationDate <= author.DateOfBirth))
                    {
                        ModelState.AddModelError("DateOfBirth", "Author cannot be born after the publication date of their book(s).");
                        return View(author);
                    }
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Search(string full_name)
        {
            if (string.IsNullOrEmpty(full_name))
            {
                return BadRequest("Search parameter is invalid.");
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.FullName != null && a.FullName.ToLower() == full_name.ToLower());

            if (author == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = author.Id });
        }

    }
}
