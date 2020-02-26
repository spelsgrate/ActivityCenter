using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeltExam.Models;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists in db
                var existingUser = dbContext.UserList.FirstOrDefault(u => u.Email == newUser.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                // Hash new user's password and save new user to db
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.UserList.Add(newUser);
                dbContext.SaveChanges();
                UserSession = newUser.UserId;
                return RedirectToAction("dashboard");
            }
            return View("Index");
        }

      
        [HttpPost("login")]
        public IActionResult Login(LoginUser User)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.UserList.FirstOrDefault(u => u.Email == User.LoginEmail);
                if (user == null)
                {
                    ModelState.AddModelError("LoginEmail", "This Email doesn't exist, please Register");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(User, user.Password, User.LoginPassword);
                    if (result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "3Incorrect Password, Please try again or register");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        return Redirect("dashboard");
                    }
                }
            }
            return View("Index");
        }

         [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("/");
            }
            else
            {
                User User = dbContext.UserList.FirstOrDefault(x => x.UserId == HttpContext.Session.GetInt32("UserId"));
                List<Occasion> Activities = dbContext.ActList.Include(x => x.Coordinator).Include(y => y.Attendees).ThenInclude(z => z.SingleUser).ToList();
                ViewBag.Activities = Activities.OrderBy(z => z.Date);
                ViewBag.u = User;
                return View();
            }
        }

        [HttpGet("newactivity")]
        public IActionResult NewAct() {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("/");
            }
            else
            {
                return View();
            }
        }

        [HttpPost("create")]
        public IActionResult Create(Occasion newOccasion)
         {
            if (ModelState.IsValid)
            {
                Join NewJoin = new Join();
                newOccasion.UserID = (int)HttpContext.Session.GetInt32("UserId");
                dbContext.ActList.Add(newOccasion);
                dbContext.SaveChanges();


                NewJoin.OccasionId = newOccasion.OccasionId;
                NewJoin.UserId = newOccasion.UserID;
                dbContext.Joinee.Add(NewJoin);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId == null)
                {
                    return RedirectToAction("/");
                }
                else
                {
                    return View("NewAct");
                }
            }
        }

        [HttpGet("activity/{OccasionId}")]
        public IActionResult ActivityDisplay(int OccasionId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("/");
            }
            else
            {
                User User =dbContext.UserList.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
                Occasion Activity =dbContext.ActList.Include(a => a.Coordinator).Include(b => b.Attendees).ThenInclude(c => c.SingleUser).FirstOrDefault(d => d.OccasionId == OccasionId);
                ViewBag.a = Activity;
                ViewBag.u = User;
                return View();
            }
        }

        [HttpGet("delete/{OccasionId}")]
        public IActionResult Delete(int OccasionId)
        {
            Occasion Activity = dbContext.ActList.FirstOrDefault(a => a.OccasionId == OccasionId);
            dbContext.ActList.Remove(Activity);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("join/{OccasionId}/{UserId}")]
        public IActionResult Attend(int OccasionId, int UserId)
        {
            Join NewJoin = new Join();
            NewJoin.UserId = UserId;
            NewJoin.OccasionId = OccasionId;
            dbContext.Joinee.Add(NewJoin);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{OccasionId}/{UserId}")]
        public IActionResult Leave(int OccasionId, int UserId)
        {
            Join Leave = dbContext.Joinee.FirstOrDefault(a => a.OccasionId == OccasionId && a.UserId == UserId);
            dbContext.Joinee.Remove(Leave);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}


