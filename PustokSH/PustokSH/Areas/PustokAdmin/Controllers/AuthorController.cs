using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.Areas.PustokAdmin.ViewModels;
using PustokSH.DAL;
using PustokSH.Model;

namespace PustokSH.Areas.PustokAdmin.Controllers
{
    [Area("PustokAdmin")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            int count=await _context.Authors.CountAsync();
            List<Author> authorList =await _context.Authors.Skip((page-1)*2).Take(2)
                .Include(a=>a.books).ToListAsync();
            PaginatedVM<Author> paginatedVM = new PaginatedVM<Author>
            {
                items = authorList,
                CurrentedPage = page,
                TotalPage=Math.Ceiling((double)count/3),
            };
            return View(paginatedVM);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Author author)
        {
            bool result = await _context.Authors.AnyAsync(x => x.FullName == author.FullName);

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (result)
            {
                ModelState.AddModelError("Fullname", "Eyni adli yazici yarana bilmez");
                return View();
            }

            await _context.AddAsync(author);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Author author= await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (author == null) return NotFound();

            return View(author);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Author author)
        {
            if (!ModelState.IsValid) return View();
            Author existed = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (existed == null) return NotFound();
            bool result=await _context.Authors.AnyAsync(a=>a.FullName==author.FullName&& a.Id!=id);
            if (result)
            {
                ModelState.AddModelError("FullName", "Bu adda author artiq movcuddur");
                return View();
                
            }
            existed.FullName = author.FullName;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Author author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (author == null) return NotFound();
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
