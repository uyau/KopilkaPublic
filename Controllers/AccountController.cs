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
using Microsoft.AspNetCore.Authorization;

namespace kopilka.Controllers
{
    public class AccountController : Controller
    {
        private KopilkaContext db;
        private EmailService EmailService;
        public AccountController(KopilkaContext context, EmailService emailService)
        {
            db = context;
            EmailService = emailService;
        }
        
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
             if(model!=null)
             { 
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Login);
                    return Json(true);
                }
             }
            
            return Json(false);
        }
        //отправка пароля на почту
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendToEmailPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendToEmailPassword([FromBody] SendToEmailPassword model)
        {

            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Email);
            if (user != null)
            {
                EmailService.SendEmailPassword(user.Login, user.Password);
                return Json(true);
            }

            return Json(false);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {

                    db.Users.Add(new User { Login = model.Login, Password = model.Password });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Login);

                return Json(true);
                }

            return Json(false);
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
