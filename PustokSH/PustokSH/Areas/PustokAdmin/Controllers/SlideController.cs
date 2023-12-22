using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.Areas.PustokAdmin.ViewModels;
using PustokSH.Areas.PustokAdmin.ViewModels;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.Utilities;

namespace PustokSH.Areas.PustokAdmin.Controllers
{
    [Area("PustokAdmin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

       
        public async Task<IActionResult> Index(int page=1)
        {
            int count=await _context.Slides.CountAsync();
            List<Slide> slides = await _context.Slides.Skip((page-1)*2).Take(2)
                .ToListAsync();
            PaginatedVM<Slide> vm = new PaginatedVM<Slide>
            {
                items = slides,
                TotalPage = Math.Ceiling((double)count / 4),
                CurrentedPage=page
            };
            return View(vm);
        }

        public IActionResult Create() 
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slideVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "Photo bosh gonderile bilmez");
                return View();
            }

            if (!slideVM.Photo.ValiDateSize(2))
            {
                ModelState.AddModelError("Photo", "Photo 2mb`dan boyuk ola bilmez");
                return View();
            }

           string FileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slide");
			Slide slide = new Slide
			{
				Img = FileName,
				Title1 = slideVM.Title1,
				Title2 = slideVM.Title2,
				Order = slideVM.Order,
				Desc = slideVM.Desc
			};
			await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

		}


		public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();

            return View(slide);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            existed.Img.DeleteFile(_env.WebRootPath, "uploads", "slide"); 
            _context.Slides.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
			if (id <= 0) return BadRequest();
			Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			if (existed is null) return NotFound();
			UpdateSlideVM slideVM = new UpdateSlideVM
			{
				Title1 = existed.Title1,
				Title2 = existed.Title2,
				Img = existed.Img,
				Order = existed.Order,
				Desc = existed.Desc
			};
			return View(slideVM);
		}
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slideVM)
        {
			Slide exists = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
			if (exists is null) return NotFound();
			if (!ModelState.IsValid)
			{
				return View(slideVM);

			}
			if (slideVM.Photo is not null)
			{
				if (!slideVM.Photo.ValidateType("image/"))
				{
					ModelState.AddModelError("Photo", "Photo bosh gonderile bilmez");
					return View(slideVM);
				}

				if (!slideVM.Photo.ValiDateSize(2))
				{
					ModelState.AddModelError("Photo", "Photo 2mb`dan boyuk ola bilmez");
					return View(slideVM);
				}
				string fileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slide");
				exists.Img.DeleteFile(_env.WebRootPath, "uploads", "slide");
				exists.Img = fileName;


			}
			exists.Title1 = slideVM.Title1;
			exists.Title2 = slideVM.Title2;
			exists.Desc = slideVM.Desc;
			exists.Order=slideVM.Order;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
