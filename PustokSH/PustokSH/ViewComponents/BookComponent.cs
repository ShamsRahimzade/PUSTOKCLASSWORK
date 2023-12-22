using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.ViewModels;

namespace PustokSH.ViewComponents
{
    public class BookComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public BookComponent(AppDbContext context) 
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string tagname)
        {
            List<Book> books=await _context.Books
                .Where(x=>x.IsDeleted==false&& x.bookTags.Any(y=>y.tag.Name==tagname))
                .Include(x=>x.bookTags).ThenInclude(x=>x.tag)
                .Include(x=>x.bookImages)
                .Include(x=>x.author).ToListAsync();
                 return View(books);
        }
    }
}

