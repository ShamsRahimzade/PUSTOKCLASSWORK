using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.Areas.PustokAdmin.ViewModels;
using PustokSH.DAL;
using PustokSH.Model;

namespace PustokSH.Areas.PustokAdmin.Controllers
{
    [Area("PustokAdmin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            int count=await _context.tags.CountAsync();
            List<Tag>tags= await _context.tags.Skip((page-1)*2).Take(2)
                .Include(t=>t.bookTags).ToListAsync();
            PaginatedVM<Tag> paginatedVM = new PaginatedVM<Tag>
            {
                items = tags,
                TotalPage = Math.Ceiling((double)count / 2),
                CurrentedPage=page
            };
            return View(paginatedVM);
        }
        public IActionResult Create()
        {
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            bool result= await _context.tags.AnyAsync(t=>t.Name==tag.Name);
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (result)
            {
                ModelState.AddModelError("Name", "Eyni adli tag yarana bilmez");
                return View();
            }

            await _context.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.tags.FirstOrDefaultAsync(x => x.Id == id);
            if (tag == null) return NotFound();

            return View(tag);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Tag tag)
        {
            if (!ModelState.IsValid) return View();
            Tag existed = await _context.tags.FirstOrDefaultAsync(x => x.Id == id);
            if (existed == null) return NotFound();
            bool result = await _context.tags.AnyAsync(t => t.Name == tag.Name && t.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda tag artiq movcuddur");
                return View();

            }
            existed.Name = tag.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.tags.FirstOrDefaultAsync(x => x.Id == id);
            if (tag == null) return NotFound();
            _context.tags.Remove(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //public IActionResult Detail()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Detail(int id,Tag tag)
        //{
        //    if (!ModelState.IsValid) return View();
        //    Tag existed = await _context.tags.FirstOrDefaultAsync(x => x.Id == id);
        //    if (existed == null) return NotFound();
        //    bool result = await _context.tags.AnyAsync(a => a.Name == existed.Name && a.Id != id);
        //    if (result)
        //    {
        //        ModelState.AddModelError("FullName", "Bu adda author artiq movcuddur");
        //        return View();

        //    }
        //    existed.Name = tag.Name;
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

    }
}
