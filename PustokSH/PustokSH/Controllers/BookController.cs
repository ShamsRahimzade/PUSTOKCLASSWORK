using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace PustokSH.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int Id)
        {

            Book book = _context.Books
                 .Include(x => x.author)
                .Include(x => x.genre)
                .Include(x => x.bookImages)
                .Include(x=>x.bookTags).ThenInclude(t=>t.tag)
           . FirstOrDefault(x => x.Id == Id);


            if (book == null)
            {
                return NotFound();
            }
            List<Book> relatedbooks=_context.Books
                .Where(x => x.GenreId ==book.GenreId && x.Id!= book.Id)
                .Include(x=>x.bookImages)
                .Include(x=>x.author)
                .ToList();
            DetailVM VM = new DetailVM
            {
                relatedbooks = relatedbooks,
                book = book,
               
            };
            return View(VM);
        }
    }
}
