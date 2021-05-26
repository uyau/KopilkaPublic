using kopilka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using kopilka.ViewModels;


namespace kopilka.Controllers
{
    public class HomeController : Controller
    {
        KopilkaContext db;
        public HomeController(KopilkaContext context)
        {
            db = context;
        }
        
        //check db for NULL
        [Authorize]
        public IActionResult Index()
        {
            //проверка бд на наличие таблицы пользователя
            var userIdentity = HttpContext.User.Identity;
            var userName = userIdentity.Name;
            var user = db.Users.FirstOrDefault(x => x.Login == userName);
            var userTable = db.Ranges.ToList().Where(x => x.UserId == user.Id);

            if (!userTable.Any())
            {
                return RedirectToAction("SetRange");
            }
            else
            {
                return View();
            }
        }
        //внесение cуммы в копилку. изменение значение из внесено false на true
        [HttpPost]
        public IActionResult Index(int selectedNumber)
        {
            var userIdentity = HttpContext.User.Identity;
            var userName = userIdentity.Name;
            var user = db.Users.FirstOrDefault(x => x.Login == userName);

            var check = db.Ranges.Where(x => x.MoneyboxRange == selectedNumber && x.UserId == user.Id);
            if (!check.Any())
            {
                ViewBag.message = "некорректное значение либо число не входит в диапазон копилки";
                return View();
            }


           foreach ( var row in db.Ranges.Where(x => x.MoneyboxRange == selectedNumber && x.UserId==user.Id))
           {
                //меняем статус числа из "не внесено" в "внесено"
                if (row.IsDonated == false)
                {
                    row.IsDonated = true;
                    break;
                }
                //если число внесено
                //находим два ближайших не внесенных числа которые больше и меньше введенного числа.
           
                if (row.IsDonated==true)
                {
                    object upNumber = null;
                    object downNumber = null;
                    
                    var downNumbers = db.Ranges.ToList().Where(x=>x.MoneyboxRange < selectedNumber && x.UserId == user.Id && x.IsDonated == false);
                    if (downNumbers.Any())
                    {
                        downNumber = downNumbers.Max(x=>x.MoneyboxRange);
                    }
                    var upNumbers = db.Ranges.ToList().Where(x=>x.MoneyboxRange > selectedNumber && x.UserId == user.Id && x.IsDonated == false);
                    if (upNumbers.Any())
                    {
                        upNumber = upNumbers.Min(x => x.MoneyboxRange);
                    }
                   
                    ViewBag.upNumber = upNumber;
                    ViewBag.downNumber = downNumber;
                    return View();
                }
           }
            db.SaveChanges();
            return View();
        }
        public IActionResult DeleteTable()
        {
            var userIdentity = HttpContext.User.Identity;
            var userName = userIdentity.Name;
            var user = db.Users.FirstOrDefault(x => x.Login == userName);
            foreach (var row in db.Ranges.Where(x =>  x.UserId == user.Id))
            {
                db.Ranges.Remove(row);
            }
            db.SaveChanges();
                return RedirectToAction("SetRange");
        }

        //set rande of moneybox. create table
        [HttpGet]
        public IActionResult SetRange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetRange(int selectedRange)
        {
            var userIdentity = HttpContext.User.Identity;
            var userName = userIdentity.Name;
            var user = db.Users.FirstOrDefault(x => x.Login == userName);

            for (int i = 1; i <= selectedRange; i++)
            {
                Models.Range range = new Models.Range { MoneyboxRange = i, IsDonated = false, UserId=user.Id};
                await db.Ranges.AddAsync(range);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //table to list
        public IActionResult RangesToList()
        {
            var userIdentity = HttpContext.User.Identity;
            var userName = userIdentity.Name;
            var user = db.Users.FirstOrDefault(x => x.Login == userName);
            var userTable = db.Ranges.ToList().Where(x => x.UserId == user.Id).OrderBy(x => x.MoneyboxRange);
            if (!userTable.Any())
            {
                return RedirectToAction("SetRange");
            }
            return View(userTable);
        }
    }
}