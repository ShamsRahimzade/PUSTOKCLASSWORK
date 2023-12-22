using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PustokSH.DAL;
using PustokSH.Model;
using PustokSH.Utilities.Enums;
using PustokSH.Utilities.Extensions;
using PustokSH.ViewModels;
using System.Drawing;

namespace PustokSH.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole>roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) { return RedirectToAction("Index", "Home"); }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid) return View();
            if (CheckString.IsDigit(registerVM.Name))
            {
                ModelState.AddModelError("Name", "adda reqem olamaz");
                return View();
            }
            if (CheckString.IsDigit(registerVM.SurName))
            {
                ModelState.AddModelError("SurName", "soyadda reqem olamaz");
                return View();
            }
            if (!registerVM.Email.IsValidEmail())
            {
                ModelState.AddModelError("Email", "Email duzgun deil");
                return View();
            }

            AppUser user = new AppUser
            {
                Name = CheckString.CapitalizeName(registerVM.Name),
                SurName = CheckString.CapitalizeName(registerVM.SurName),
                Email = registerVM.Email,
                UserName = registerVM.UserName     
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");

        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnURL)
        {
            if (!ModelState.IsValid) return View();
            AppUser appUser = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (appUser is null)
            {
                appUser = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
                if (appUser is null)
                {
                    ModelState.AddModelError(String.Empty, "Tanimiram seni, yuru git ishine kardeshim");

                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.IsRemembered, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "bloklanmisan, ozunu oldurme ikideqa dincel sora duzgun yaz");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Tanimiram seni, yuru git ishine kardeshim");
                return View();
            }
            if (returnURL is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnURL);

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RolesCreate()
        {
            foreach (var item in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
                }
            }
            return RedirectToAction("Index", "Home");
        }

    }
 }

