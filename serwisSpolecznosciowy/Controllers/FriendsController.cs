using serwisSpolecznosciowy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace serwisSpolecznosciowy.Controllers
{
    [RoutePrefix("Friends")]
    [HandleError]
    public class FriendsController : Controller
    {
        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }

        [Route("Add")] //POST
        [Route("Add/{login}")] // GET
        public ActionResult Add(string login)
        {
            ViewBag.Message = "Add friend using form below";
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null) // user isnt logged
                return RedirectToAction("NotFound_404", "Error");

            ViewBag.FriendsAddResourceMessage = string.Empty;

            if (login == null || login.Equals(string.Empty)) // empty login input
            {
                if (HttpContext.Request.HttpMethod == "POST") ViewBag.FriendsAddResourceMessage = "EMPTY LOGIN INPUT";
                return View();
            }

            if (userInSession.Login == login)
            {
                ViewBag.FriendsAddResourceMessage = "ADD YOURSELF";
                return View();
            }

            foreach (var friend in userInSession.Friends)
            {
                if (friend == login) // user has friend with that login
                {
                    ViewBag.FriendsAddResourceMessage = "FRIEND EXISTS";
                    return View();
                }
            }

            bool doesUserExist = false;
            foreach (var user in UserController.Users)
            {
                if (user.Login == login)
                {
                    user.Friends.Add(userInSession.Login); // friend who is added also needs to add logged User
                    doesUserExist = true;
                }
            }

            if (!doesUserExist) // user with that login doesnt exist
            {
                ViewBag.FriendsAddResourceMessage = "USER DOESNT EXIST";
                return View();
            }

            userInSession.Friends.Add(login); // loggedUser adds friend
            ViewBag.FriendsAddResourceMessage = "FRIEND ADDED";
            return View();
        }

        [Route("Del")] //POST
        [Route("Del/{login}")] // GET
        public ActionResult Delete(string login)
        {
            ViewBag.Message = "Delete friend using form below";
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null) // user isnt logged
                return RedirectToAction("NotFound_404", "Error");

            ViewBag.FriendsDeleteResourceMessage = string.Empty;

            if (login == null || login.Equals(string.Empty)) // empty login input
            {
                if (HttpContext.Request.HttpMethod == "POST") ViewBag.FriendsDeleteResourceMessage = "EMPTY LOGIN INPUT";
                return View();
            }

            if (userInSession.Login == login) // cannot delete yourself
            {
                ViewBag.FriendsDeleteResourceMessage = "DELETE YOURSELF";
                return View();
            }

            bool doesUserHasFriendWithThatLogin = false;
            foreach (var friend in userInSession.Friends)
            {
                if (friend == login) // user has friend with that login
                {
                    doesUserHasFriendWithThatLogin = true;
                    break;
                }
            }

            if (!doesUserHasFriendWithThatLogin) // user doesnt have friend with that login
            {
                ViewBag.FriendsDeleteResourceMessage = "FRIEND DOESNT EXIST";
                return View();
            }

            bool doesUserExist = false;
            foreach (var user in UserController.Users)
            {
                if (user.Login == login)
                {
                    user.Friends.Remove(userInSession.Login); // friend who is deleted also needs to delete logged User
                    doesUserExist = true;
                }
            }

            if (!doesUserExist) // user with that login doesnt exist
            {
                ViewBag.FriendsDeleteResourceMessage = "USER DOESNT EXIST";
                return View();
            }

            userInSession.Friends.Remove(login); // loggedUser deletes friend
            ViewBag.FriendsDeleteResourceMessage = "FRIEND DELETED";
            return View();
        }

        [Route("List")]
        public ActionResult List()
        {
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null) // user isnt logged 
                return RedirectToAction("NotFound_404", "Error");
            var liOfFriends = new List<User>();
            foreach (var friendLogin in userInSession.Friends)
                foreach (var user in UserController.Users)
                    if (user.Login == friendLogin)
                        liOfFriends.Add(user);

            return View(liOfFriends);
        }

        [Route("Import")]
        public ActionResult ImportFriends()
        {
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null) // user isnt logged 
                return RedirectToAction("NotFound_404", "Error");
            return View();
        }

        [HttpPost]
        [Route("Import")]
        public ActionResult ImportFriends(HttpPostedFileBase friendsTxt)
        {
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null) // user isnt logged 
                return RedirectToAction("NotFound_404", "Error");

            ViewBag.FriendsImportResourceMessage = string.Empty;

            if (friendsTxt != null && friendsTxt.ContentLength > 0)
            {
                var stream = Request.Files["friendsTxt"].InputStream;
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();

                var friendLogins = text.Split(',');
                //userInSession.Friends.Clear();
                foreach(var user in UserController.Users)
                {
                    foreach(var friendLogin in friendLogins)
                    {
                        if (user.Login == friendLogin) // friend from file is in db of Users; this method can only add friends that exist in database of Users
                        {
                            if (userInSession.Login != friendLogin) // cannot add yourself
                            {
                                var isFriendAlreadyInUserFriends = userInSession.Friends.Any(item => item == friendLogin);
                                if (!isFriendAlreadyInUserFriends)
                                {
                                    userInSession.Friends.Add(friendLogin); // user adds friend
                                    var friend = UserController.Users.Find(item => item.Login == friendLogin);
                                    friend.Friends.Add(userInSession.Login);  // friend adds user
                                }
                            }
                        }
                    }
                }

                ViewBag.FriendsImportResourceMessage = "IMPORT SUCCESS";
                //var fileName = Path.GetFileName(friendsTxt.FileName);
                //friendsTxt.SaveAs(Path.Combine(directory, fileName));
                return RedirectToAction("List", "Friends");
            }
            else
            {
                ViewBag.FriendsImportResourceMessage = "WRONG FILE";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("Export")]
        public ActionResult ExportFriends()
        {
            string name = @"F:\example.txt";
            var userInSession = Session["loggedUser"] as User;
            if (userInSession == null) // user isnt logged 
                return RedirectToAction("NotFound_404", "Error");

            FileInfo fileInfo = new FileInfo(name);
            // Delete the file if it exists.
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (FileStream fs = fileInfo.Create())
            {
                string friends = string.Empty;
                foreach (var friendLogin in userInSession.Friends)
                {
                    friends += friendLogin + ',';
                }

                Byte[] info = new UTF8Encoding(true).GetBytes(friends);
                fs.Write(info, 0, info.Length);
            }


            return File(fileInfo.OpenRead(), "text/plain", userInSession.Login + "_friends.txt");
        }
    }
}