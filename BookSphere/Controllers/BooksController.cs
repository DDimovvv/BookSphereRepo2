using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookSphere.Data;
using BookSphere.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookSphere.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,PublicationDate,Genre,Pages,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                if (_context.Books.Any(b => b.Title == book.Title))
                {
                    ModelState.AddModelError("Title", "Book with this title already exists");
                    return View(book);
                }   
                if (book.Pages < 1)
                {
                    ModelState.AddModelError("Pages", "Number of pages must be greater than 0");
                    return View(book);
                }
                if (book.PublicationDate > DateTime.Now)
                {
                    ModelState.AddModelError("PublicationDate", "Publication date cannot be in the future");
                    return View(book);
                }
                if (_context.Authors.Any(a => book.Author == a.FullName && book.PublicationDate <= a.DateOfBirth) && _context.Authors.Select(a => book.Author == a.FullName) != null)
                {
                    ModelState.AddModelError("PublicationDate", "A book can't be published before its author's date of birth");
                    return View(book);
                }
                if (!string.IsNullOrWhiteSpace(book.Author)) // Ensure author name is not null or empty
                {
                    var existingAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == book.Author);
                    if (existingAuthor == null)
                    {
                        // If the author does not exist, create and add a new one
                        existingAuthor = new Author { FullName = book.Author };
                        _context.Authors.Add(existingAuthor);
                        await _context.SaveChangesAsync(); // Save to generate the new Author's Id
                    }
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();
                    _context.BookAuthor.Add(new BookAuthor { BookId = book.Id, AuthorId = existingAuthor.Id });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Author", "Author name cannot be empty");
                    return View(book);
                }

            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,PublicationDate,Genre,Pages,Description")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Books.Any(b => b.Title == book.Title && b.Id != book.Id))
                    {
                        ModelState.AddModelError("Title", "Book with this title already exists");
                        return View(book);
                    }
                    if (book.Pages < 1)
                    {
                        ModelState.AddModelError("Pages", "Number of pages must be greater than 0");
                        return View(book);
                    }
                    if(book.PublicationDate > DateTime.Now)
                    {
                        ModelState.AddModelError("PublicationDate", "Publication date cannot be in the future");
                        return View(book);
                    }
                    if (_context.Authors.Any(a => book.Author == a.FullName && book.PublicationDate <= a.DateOfBirth) && _context.Authors.Select(a => book.Author == a.FullName) != null)
                    {
                        ModelState.AddModelError("PublicationDate", "A book can't be published before its author's date of birth");
                        return View(book);
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View();
        }
        [HttpPost]
        //search method
        public async Task<IActionResult> Search(string title)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Title.ToLower() == title);
            if (book == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Details), new {id = book.Id});
        }
    }
}
