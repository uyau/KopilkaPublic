using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kopilka.ViewModels; 
using kopilka.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace kopilka.Controllers
{
    public class AccountController : Controller
    {
        private KopilkaContext db;
        public AccountController (KopilkaContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Login (LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if(user!=null)
                {
                    await Authenticate(model.Login); 
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        //отправка пароля на почту
        [HttpGet]
        public IActionResult SendToEmailPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendToEmailPassword(SendToEmailPassword model)
        {
            if (ModelState.IsValid)
            {
                var email = db.Users.FirstOrDefault(u => u.Login == model.Email).Login;
                var password = db.Users.FirstOrDefault(u => u.Login == model.Email).Password;
                if (email!=null)
                {
                    EmailService emailService = new EmailService();
                    emailService.SendEmailPassword(email,password);
                    return RedirectToAction("Login");
                }
                else
                ModelState.AddModelError("", "something went wrong..");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                   
                    db.Users.Add(new User { Login = model.Login, Password = model.Password });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Login);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Email уже используется");
            }
            return View(model);
        }
        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");

        }
    }
}
