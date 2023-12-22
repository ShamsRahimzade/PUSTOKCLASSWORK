using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.Areas.PustokAdmin.ViewModels;
using PustokSH.Areas.PustokAdmin.ViewModels;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.Utilities;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PustokSH.Areas.PustokAdmin.Controllers
{
    [Area("PustokAdmin")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index(int page=1)
        {
            int count = await _context.Books.Where(b=>b.IsDeleted==false).CountAsync();
            List<Book> books = await _context.Books
                .Where(b => b.IsDeleted == false).Skip((page - 1) * 4).Take(4)
                .Include(b => b.bookImages)
                .Include(b => b.author)
                .Include(b => b.genre)
                .ToListAsync();
            PaginatedVM<Book> paginatedVM = new PaginatedVM<Book>
            {
                items = books,
                TotalPage = Math.Ceiling((double)count / 5),
                CurrentedPage=page
            };

            return View(paginatedVM);
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            CreateBookVM bookCreateVM = new CreateBookVM();
            bookCreateVM.tags = await _context.tags.ToListAsync();
            bookCreateVM.authors = await _context.Authors.ToListAsync();
            bookCreateVM.genres = await _context.Genres.ToListAsync();
            return View(bookCreateVM);
        }
        //[Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookVM bookvm)
        {

            if (!ModelState.IsValid)
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();
                return View(bookvm);
            }

            if (!await _context.Authors.AnyAsync(a => a.Id == bookvm.AuthorId))
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();
                ModelState.AddModelError("Id", "Author");
                return View(bookvm);
            }
            if (!await _context.Genres.AnyAsync(g => g.Id == bookvm.GenreId))
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();
                ModelState.AddModelError("Id", "Genre");
                return View(bookvm);
            }
            if (bookvm.tagIds != null)
            {

                foreach (var item in bookvm.tagIds)
                {
                    if (!await _context.tags.AnyAsync(b => b.Id == item))
                    {
                        bookvm.tags = await _context.tags.ToListAsync();
                        bookvm.authors = await _context.Authors.ToListAsync();
                        bookvm.genres = await _context.Genres.ToListAsync();
                        ModelState.AddModelError("tagIds", "not found tag");
                        return View(bookvm);
                    }
                }
            }

            if (!bookvm.MainPhoto.ValidateType("image/"))
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();

                ModelState.AddModelError("MainPhoto", "filetype");
                return View(bookvm);
            }
            if (!bookvm.MainPhoto.ValiDateSize(7))
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();

                ModelState.AddModelError("MainPhoto", "filsize");
                return View(bookvm);
            }
            if (!bookvm.HoverPhoto.ValidateType("image/"))
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();

                ModelState.AddModelError("HoverPhoto", "filetype");
                return View(bookvm);
            }
            if (!bookvm.HoverPhoto.ValiDateSize(7))
            {
                bookvm.tags = await _context.tags.ToListAsync();
                bookvm.authors = await _context.Authors.ToListAsync();
                bookvm.genres = await _context.Genres.ToListAsync();

                ModelState.AddModelError("HoverPhoto", "filesize");
                return View(bookvm);
            }
            BookImage main = new BookImage
            {
                IsPrimary = true,
                Image = await bookvm.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "slide")
            };
            BookImage hover = new BookImage
            {
                IsPrimary = false,
                Image = await bookvm.HoverPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "slide")
            };
            Book book = new Book
            {
                Name = bookvm.Name,
                AuthorId = bookvm.AuthorId,
                GenreId = bookvm.GenreId,
                SalePrice = bookvm.SalePrice,
                CostPrice = bookvm.CostPrice,
                Desc = bookvm.Desc,
                Page = bookvm.Page,
                IsDeleted = false,
                IsAvailable = true,
                bookTags = new List<BookTag>(),
                bookImages = new List<BookImage> { main, hover }
            };

            TempData["Message"] = "";
            foreach (var item in bookvm.Photos ?? new List<IFormFile>())
            {
                if (!item.ValidateType("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{item.FileName} adli filein tipi uygun deyil</p>";
                    continue;
                }
                if (!item.ValiDateSize(7))
                {
                    TempData["Mesaage"] += $"<p class=\"text-danger\">{item.FileName} adli filein olcusu uygun deil</p>";
                    continue;
                }
                book.bookImages.Add(new BookImage { IsPrimary = null, Image = await item.CreateFileAsync(_env.WebRootPath, "uploads", "slide") });
            }
            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            Book book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null) return NotFound();

            book.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Book book = await _context.Books
               .Include(x => x.bookTags)
               .Include(x => x.bookImages)

               .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null) return NotFound();

            UpdateBookVM updateVM = new UpdateBookVM
            {
                Name = book.Name,
                AuthorId = book.AuthorId,
                GenreId = book.GenreId,
                Page = book.Page,
                SalePrice = book.SalePrice,
                CostPrice = book.CostPrice,
                Discount = book.Discount,
                Desc = book.Desc,
                IsAvailable = book.IsAvailable,
                Images = book.bookImages,
                authors = await _context.Authors.ToListAsync(),
                genres = await _context.Genres.ToListAsync(),
                tags = await _context.tags.ToListAsync(),
                tagIds = book.bookTags.Select(x => x.TagId).ToList()

            };

            return View(updateVM);
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateBookVM updateBookVM)
        {
            if (!ModelState.IsValid)
            {
                updateBookVM.tags = await _context.tags.ToListAsync();
                updateBookVM.authors = await _context.Authors.ToListAsync();
                updateBookVM.genres = await _context.Genres.ToListAsync();
                return View(updateBookVM);
            };
            Book existed = await _context.Books.Include(x => x.bookTags).FirstOrDefaultAsync(b => b.Id == id);
            if (existed == null) return NotFound();

            if (!await _context.Authors.AnyAsync(a => a.Id == updateBookVM.AuthorId)) return NotFound();
            if (!await _context.Genres.AnyAsync(g => g.Id == updateBookVM.GenreId)) return NotFound();
            //if (!await _context.tags.AnyAsync(g => g.Id == updateBookVM.)) return View();
            if (updateBookVM.tagIds != null)
            {

                foreach (var item in existed.bookTags)
                {
                    if (!updateBookVM.tagIds.Exists(tId => tId == item.TagId))
                    {
                        updateBookVM.tags = await _context.tags.ToListAsync();
                        updateBookVM.authors = await _context.Authors.ToListAsync();
                        updateBookVM.genres = await _context.Genres.ToListAsync();
                        _context.BookTags.Remove(item);
                    }
                }
                foreach (var item in updateBookVM.tagIds)
                {
                    if (!existed.bookTags.Any(bt => bt.TagId == item))
                    {
                        existed.bookTags.Add(new BookTag
                        {
                            TagId = item
                        });
                    }
                }
            }
            if (updateBookVM.MainPhoto is not null)
            {

                BookImage oldmain = existed.bookImages.FirstOrDefault(x => x.IsPrimary == true);
                oldmain.Image.DeleteFile(_env.WebRootPath, "uploads/book");
                existed.bookImages.Remove(oldmain);
                existed.bookImages.Add(new BookImage
                {
                    Image = await updateBookVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "slide"),
                    IsPrimary = true

                });
            }
            if (updateBookVM.HoverPhoto is not null)
            {

                BookImage oldhover = existed.bookImages.FirstOrDefault(x => x.IsPrimary == false);
                oldhover.Image.DeleteFile(_env.WebRootPath, "uploads/book");
                existed.bookImages.Remove(oldhover);
                existed.bookImages.Add(new BookImage
                {
                    Image = await updateBookVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "uploads/book"),
                    IsPrimary = false

                });
            }
            if (updateBookVM.ImageIds is null)
            {
                updateBookVM.ImageIds = new List<int>();
            }

            List<BookImage> removable = existed.bookImages.Where(pi => !updateBookVM.ImageIds.Exists(x => x == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (BookImage photo in removable)
            {
                photo.Image.DeleteFile(_env.WebRootPath, "uploads/book");
                existed.bookImages.Remove(photo);
            }

            if (updateBookVM.Photos is not null)
            {
                foreach (IFormFile item in updateBookVM.Photos)
                {
                    if (!item.ValidateType("image/"))
                    {
                        ModelState.AddModelError("Photos", "Photos typei uygun deil");
                        updateBookVM.authors = await _context.Authors.ToListAsync();
                        updateBookVM.genres = await _context.Genres.ToListAsync();
                        updateBookVM.tags = await _context.tags.ToListAsync();

                        return View(updateBookVM);
                    }
                    if (item.ValiDateSize(300))
                    {
                        ModelState.AddModelError("Photos", "Photos sizei uygun deil");
                        updateBookVM.authors = await _context.Authors.ToListAsync();
                        updateBookVM.genres = await _context.Genres.ToListAsync();
                        updateBookVM.tags = await _context.tags.ToListAsync();

                        return View(updateBookVM);
                    }

                    existed.bookImages.Add(new BookImage
                    {
                        Image =await item.CreateFileAsync(_env.WebRootPath, "uploads","slide"),
                        IsPrimary = null
                    });
                }

                existed.Name = updateBookVM.Name;
                existed.Desc = updateBookVM.Desc;
                existed.CostPrice = updateBookVM.CostPrice;
                existed.SalePrice = updateBookVM.SalePrice;
                existed.Discount = updateBookVM.Discount;
                existed.GenreId = updateBookVM.GenreId;
                existed.AuthorId = updateBookVM.AuthorId;
                existed.Page = updateBookVM.Page;
                existed.IsAvailable = updateBookVM.IsAvailable;
                await _context.SaveChangesAsync();
            }
                return RedirectToAction(nameof(Index));
        }

    } 

}
