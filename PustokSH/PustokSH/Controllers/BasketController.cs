using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.ViewModels;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace PustokSH.Areas.PustokAdmin.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .ThenInclude(u => u.book)
                    .ThenInclude(u => u.bookImages.Where(p => p.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (var item in appUser.BasketItems)
                {
                    items.Add(new BasketItemVM
                    {
                        Id = item.BookId,
                        Product = item.book.Name,
                        Price = item.book.SalePrice,
                        Quantity = item.Count,
                        Total = item.Count * item.book.SalePrice,
                        Img = item.book.bookImages.FirstOrDefault()?.Image
                    });
                }
            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    foreach (var item in cookies)
                    {
                        Book book = await _context.Books
                            .Include(p => p.bookImages.Where(pi => pi.IsPrimary == true))
                            .FirstOrDefaultAsync(p => p.Id == item.Id);
                        if (book is not null)
                        {
                            BasketItemVM basketItem = new BasketItemVM
                            {
                                Id = book.Id,
                                Product = book.Name,
                                Img = book.bookImages.FirstOrDefault()?.Image,
                                Price = book.SalePrice,
                                Quantity = item.Count,
                                Total = book.SalePrice * item.Count
                            };
                            items.Add(basketItem);
                        }
                    }
                }
            }
                BasketIndexVM vm = new BasketIndexVM()
            {
                Books = _context.Books.Include(x => x.author).Include(x => x.bookImages).ToList(),
                BasketItemVM = items
            };

            return View(vm);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Book book = await _context.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (book is null) return NotFound();
            List<BasketItemVM> items = new List<BasketItemVM>();
            foreach (var cookie in items)
            {
                Book newbook = await _context.Books
                    .Where(x => x.IsDeleted == false)
                    .Include(p => p.bookImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(p => p.Id == cookie.Id);

                if (newbook is not null)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Id = newbook.Id,
                        Product = newbook.Name,
                        Img = newbook.bookImages.FirstOrDefault().Image,
                        Price = newbook.SalePrice - newbook.Discount,
                        Quantity = cookie.Quantity,
                        Total = (newbook.SalePrice - newbook.Discount) * cookie.Quantity
                    };
                    items.Add(item);
                }
            }
            if (User.Identity.IsAuthenticated) 
            {
                AppUser appUser = await _userManager.Users
                        .Include(u => u.BasketItems)
                         .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                      if (appUser is null) return NotFound();
                      BasketItem basketItem = appUser.BasketItems.FirstOrDefault(bi => bi.BookId == book.Id);
               
                if (basketItem is null)
                {
                    basketItem = new BasketItem
                    {
                       AppUserId = appUser.Id,
                       BookId = book.Id,
                       Price=book.SalePrice,
                        Count = 1
                    };
                    appUser.BasketItems.Add(basketItem);
                }

                else
                {
                    basketItem.Count++;
                }
            }
            else
            {
                List<BasketCookieItemVM> basketVM;
                if (Request.Cookies["Basket"] is null)
                {
                    basketVM = new List<BasketCookieItemVM>();
                    BasketCookieItemVM basket = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basketVM.Add(basket);
                }
                else
                {
                    basketVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    BasketCookieItemVM exist = basketVM.FirstOrDefault(b => b.Id == id);
                    if (exist is null)
                    {
                        BasketCookieItemVM basket = new BasketCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };
                        basketVM.Add(basket);
                    }

                    else
                    {
                        exist.Count++;
                    }
                }
                string json = JsonConvert.SerializeObject(basketVM);
                Response.Cookies.Append("Basket", json);



             
            }
           


            return PartialView("_BasketPartial", items);
        }
        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }
        public async Task<IActionResult> RemoveFromBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Book book = await _context.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (book is null) return NotFound();
            List<BasketCookieItemVM> basketVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM exist = basketVM.FirstOrDefault(b => b.Id == id);
            basketVM.Remove(exist);

            string json = JsonConvert.SerializeObject(basketVM);
            Response.Cookies.Append("Basket", json);

            return RedirectToAction(nameof(Index), "Basket");

        }
        public async Task<IActionResult> DecBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Book book = await _context.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (book is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                  .Include(u => u.BasketItems)

                  .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser is null) return NotFound();
                BasketItem basketItem = appUser.BasketItems.FirstOrDefault(bi => bi.BookId == book.Id);

            }

            else
            {

                List<BasketCookieItemVM> basketCookieItemVMs = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                BasketCookieItemVM existed = basketCookieItemVMs.FirstOrDefault(b => b.Id == id);
                if (existed is not null)
                {
                    if (existed.Count > 1)
                    {
                        existed.Count--;
                    }
                    else
                    {
                        basketCookieItemVMs.Remove(existed);
                    }
                }
                //string json = JsonConvert.SerializeObject(basketCookieItemVMs);
                //Response.Cookies.Append("Basket", json);
            }
            List<BasketCookieItemVM> basketVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM exist = basketVM.FirstOrDefault(b => b.Id == id);
            if (exist is not null)
            {
                if (exist.Count > 1)
                {
                    exist.Count--;
                }
                else
                {
                    basketVM.Remove(exist);
                }
            }
            string json = JsonConvert.SerializeObject(basketVM);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> IncBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Book book = await _context.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (book is null) return NotFound();
            List<BasketCookieItemVM> basketVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM exist = basketVM.FirstOrDefault(b => b.Id == id);
            if (exist is not null)
            {
                exist.Count++;
            }
            string json = JsonConvert.SerializeObject(basketVM);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }

    }
}
