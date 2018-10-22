using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using serwisSpolecznosciowy.Models;

namespace serwisSpolecznosciowy.Controllers
{
    [HandleError]
    public class LoginController : Controller
    {
        public static string LoggedUser { get; set; } = null;
        public static bool IsUserAdmin = false;

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        [Route("Login/{login}", Name = "UserLogging")]
        public ActionResult Login(string login)
        {
            ViewBag.LoginResourceError = string.Empty;
            if (Session["loggedUser"] == null)
            {
                ViewBag.Message = "Login using input below";
                LoggedUser = login;

                if (login == "admin") { IsUserAdmin = true; }
                else
                {
                    IsUserAdmin = false;
                }

                foreach (var user in UserController.Users)
                {
                    if (user.Login == login)
                    {
                        Session["loggedUser"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                }
                /* Logging didnt work */
                if ((login == null || login.Equals(string.Empty)) && HttpContext.Request.HttpMethod == "POST")
                {
                    ViewBag.LoginResourceError = "EMPTY LOGIN INPUT";
                }
                else if (login != null) // If there is logging with specified login which doesnt exist
                    ViewBag.LoginResourceError = "USER DOESNT EXIST";

                return View();
            }

            return RedirectToAction("Index", "Home");

        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}