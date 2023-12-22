using Microsoft.AspNetCore.Mvc;
using PustokSH.DAL;

namespace PustokSH.Areas.PustokAdmin.Controllers
{
    [Area("PustokAdmin")]
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;

        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
