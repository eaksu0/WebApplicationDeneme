using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;

        // --- REGISTER ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            string firstName, string lastName, string email,
            string phone, string userName, string password, int? roleId)
        {
            // kullanıcı adı benzersiz mi?
            var exists = await _db.AppUsers.AnyAsync(u => u.UserName == userName);
            if (exists)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten kullanılıyor.";
                return View();
            }

            var user = new AppUser
            {
                Name = firstName ?? string.Empty,
                Surname = lastName ?? string.Empty,
                Email = email ?? string.Empty,
                PhoneNumber = phone ?? string.Empty,
                UserName = userName ?? string.Empty,
                Password = password ?? string.Empty, // Not: demo. Gerçekte HASH zorunlu.
                RoleId = roleId
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();

            // kayıt sonrası rolüyle birlikte tekrar yükleyip giriş yap
            var userWithRole = await _db.AppUsers
                                        .Include(u => u.Role)
                                        .FirstAsync(u => u.Id == user.Id);

            await SignInUserAsync(userWithRole);
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }

        // --- LOGIN ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password, string? returnUrl = null)
        {
            var user = await _db.AppUsers
                                .Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
                return View();
            }

            await SignInUserAsync(user);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }

        // --- LOGOUT ---
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => Content("Bu işlem için yetkiniz yok.");

        // --- Ortak: cookie’ye claim yazarak giriş yap ---
        private async Task SignInUserAsync(AppUser user)
        {
            // İsimler null ise boş yap
            var fullName = $"{user.Name ?? string.Empty} {user.Surname ?? string.Empty}".Trim();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("FullName", fullName)
            };

            // Rol bilgisi varsa claim ekle
            if (user.Role != null && !string.IsNullOrWhiteSpace(user.Role.Name))
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });
        }
    }
}
