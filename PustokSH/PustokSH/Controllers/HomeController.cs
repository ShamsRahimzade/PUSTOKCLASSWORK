using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.ViewModels;


namespace PustokSH.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> Slides = await _context.Slides.OrderBy(x => x.Order).ToListAsync();
            List<Feature> Features =await  _context.Features.ToListAsync();
            List<Book>books=await _context.Books
                .Where(x=>x.IsDeleted==false)
                .Include(b=>b.author)
                .Include(b=>b.genre)
                .Include(b=>b.bookImages)
                .ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Books=books,
                Slides = Slides,
                Features = Features,
                Discountbooks=books.Where(b=>b.Discount>0).Take(4).ToList(),
                Expensivebooks= books.OrderByDescending(b => b.SalePrice).Take(4).ToList(),
                Newbooks = books.OrderByDescending(b => b.Id).Take(4).ToList()
            };
            return View(homeVM);
        }
        

    }
}
