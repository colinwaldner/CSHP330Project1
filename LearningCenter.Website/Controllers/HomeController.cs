using LearningCenter.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LearningCenter.Business.Interfaces;
using LearningCenter.Business.Managers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LearningCenter.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly IClassManager _classManager;

        public HomeController(IUserManager userManager, IClassManager classManager)
        {
            _userManager = userManager;
            _classManager = classManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Denied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ReturnUrl"] = Request.Query["returnUrl"];

            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Login(login.Email, login.Password);

                if (user == null)
                {
                    ModelState.AddModelError("","Username and Password does not match.");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(new LearningCenter.Website.Models.UserViewModel 
                    { 
                        Id = user.Id,
                        Email = user.Email
                    });
                    HttpContext.Session.SetString("User", json);

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, "User"),
                };

                    var claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = false,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = false,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        IssuedUtc = DateTimeOffset.UtcNow,
                        // The time at which the authentication ticket was issued.
                    };

                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        authProperties).Wait();

                    return Redirect(returnUrl ?? "~/");
                }
            }
            else
            {

            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(login);
        }

        [Authorize]
        public IActionResult Logoff()
        {
            HttpContext.Session.Remove("User");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("~/");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["ReturnUrl"] = Request.Query["returnUrl"];

            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                if (!register.Password.Equals(register.ConfirmPassword))
                {
                    ModelState.AddModelError("","Confirm Password and Password must be the same");
                }
                else
                {
                    var new_user = _userManager.Register(register.Email, register.Password, false);

                    if (new_user != null)
                    {
                        return Login(new LoginViewModel { Email = new_user.Email, Password = new_user.Password }, "~/"); 
                    }
                }                
            }

            return View(register);
        }
        [Authorize]
        public IActionResult ClassList()
        {
            var class_list = _classManager.Classes.Select(s => new ClassViewModel
            {
                Name = s.Name,
                Description = s.Description,
                Price = s.Price
            }).ToList();

            return View(class_list);
        }
        [Authorize]
        public IActionResult Enroll()
        {
            var class_list = _classManager.Classes.Select(s => new ClassViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price
            }).ToList();

            return View(class_list);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Enroll(int id)
        {
            var user = JsonConvert.DeserializeObject<UserViewModel>(HttpContext.Session.GetString("User"));

            if (ModelState.IsValid)
            {
                var classes = _userManager.EnrolledClasses(user.Id);

                if (classes.Any(x => x.Id == id))
                {
                    ModelState.AddModelError("", "You are already enrolled in that class.");
                }
                else
                {
                    if (_userManager.Enroll(user.Id, id))
                    {
                        return StudentClasses();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("","Something went wrong... Cannot enroll in that class.");
            }            

            return Enroll();
        }
        [Authorize]
        public IActionResult StudentClasses()
        {
            var user = JsonConvert.DeserializeObject<UserViewModel>(HttpContext.Session.GetString("User"));

            var class_list = _userManager.EnrolledClasses(user.Id).Select(s => new ClassViewModel
            {
                Name = s.Name,
                Description = s.Description,
                Price = s.Price
            }).ToList();

            return View("StudentClasses", class_list);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
