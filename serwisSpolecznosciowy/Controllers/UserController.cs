using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using serwisSpolecznosciowy.Models;

namespace serwisSpolecznosciowy.Controllers
{
    [RoutePrefix("User")]
    [HandleError]
    public class UserController : Controller
    {
        public static List<User> Users { get; set; } = new List<User>() { new User("admin", new List<string>(), DateTime.Now) };

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [Route("Add")]
        public ActionResult Add() // GET
        {
            ViewBag.Message = "Add user using form below";
            var userInSession = Session["loggedUser"] as User;
            if(userInSession == null) // user isnt logged
                return RedirectToAction("NotFound_404", "Error");
            else if (userInSession.Login == "admin") //  loggedUser is admin
                return View();
            else // logged but not admin
                return RedirectToAction("NotFound_404", "Error");
        }

        [HttpPost]
        [Route("Add")]
        public ActionResult Add(User user) // POST
        {
            ViewBag.AddResourceMessage = string.Empty;

            if(user.Login == null)
            {
                ViewBag.AddResourceMessage = "EMPTY LOGIN INPUT";
                return View();
            }
            foreach (var userInUser in Users)
            {
                if (user.Login == userInUser.Login) // user with that login exists in database
                {
                    ViewBag.AddResourceMessage = "USER EXISTS";
                    return View();
                }
            }
            var userInSession = Session["loggedUser"] as User;
            user.DateOfCreation = DateTime.Now;
            user.Friends = new List<string>();
            Users.Add(user);

            ViewBag.AddResourceMessage = "USER ADDED";
            return View();
        }

        [HandleError]
        [Route("Del")]
        [Route("Del/{login}", Name ="UserDeletingGET")]
        public ActionResult Delete(string login) // GET
        {
            ViewBag.Message = "Delete user using form below";
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null || userInSession.Login != "admin") // user isnt logged or loggedUser isnt admin
                return RedirectToAction("NotFound_404", "Error");

            ViewBag.DeleteResourceMessage = string.Empty;
            if (login == "admin") // cannot delete admin
            {
                ViewBag.DeleteResourceMessage = "DELETE ADMIN";
                return View();
            }
            else if (login == string.Empty || login == null)
            {
                if (HttpContext.Request.HttpMethod == "POST") ViewBag.DeleteResourceMessage = "EMPTY LOGIN INPUT";
                return View();
            }

            bool isDeleted = false;
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Login == login)
                {
                    isDeleted = true;
                    foreach(var friendLogin in Users[i].Friends)
                    {
                        foreach(var user in Users)
                        {
                            if (friendLogin == user.Login)
                            {
                                user.Friends.Remove(Users[i].Login); // friend also needs to delete user which is deleted
                                break;
                            }
                        }
                    }
                    Users.Remove(Users[i]); // delete user
                }
            }

            if (!isDeleted) // there is no user with that login
            {
                ViewBag.DeleteResourceMessage = "USER DOESNT EXIST";
                return View();
            }

            return RedirectToAction("List");

            
        }

        [Route("List")]
        public ActionResult List()
        {
            var userInSession = Session["loggedUser"] as User;
            if(userInSession == null || userInSession.Login != "admin") // user isnt logged or  loggedUser isnt admin
                return RedirectToAction("NotFound_404", "Error");
            return View(Users);
        }

        public ActionResult Init()
        {
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null || userInSession.Login != "admin") // user isnt logged or loggedUser isnt admin
                return RedirectToAction("NotFound_404", "Error");
                

            TempData["InitResourceMessage"] = string.Empty;
            User userFirst = new User("pierwszy", new List<string>() { "drugi", "trzeci"}, DateTime.Now);
            User userSec = new User("drugi", new List<string>() { "pierwszy" }, DateTime.Now);
            User userThird = new User("trzeci", new List<string>() { "pierwszy", "czwarty" }, DateTime.Now);
            User userFourth = new User("czwarty", new List<string>() { "trzeci", "piaty", "szosty" }, DateTime.Now);
            User userFifth = new User("piaty", new List<string>() { "czwarty" }, DateTime.Now);
            User userSixth = new User("szosty", new List<string>() { "czwarty" }, DateTime.Now);
            foreach(var user in Users)
            {
                if (user.Login == userFirst.Login || user.Login == userSec.Login ||
                    user.Login == userThird.Login || user.Login == userFourth.Login ||
                    user.Login == userFifth.Login || user.Login == userSixth.Login)
                {
                    TempData["InitResourceMessage"] = "CANNOT INIT";
                    return RedirectToAction("Index", "Home");
                }
            }
            Users.Add(userFirst);
            Users.Add(userSec);
            Users.Add(userThird);
            Users.Add(userFourth);
            Users.Add(userFifth);
            Users.Add(userSixth);
            TempData["InitResourceMessage"] = "INIT DONE";
            return RedirectToAction("Index", "Home");
        }
    }
}