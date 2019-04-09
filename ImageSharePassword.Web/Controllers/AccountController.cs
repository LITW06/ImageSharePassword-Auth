using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ImageSharePassword.Data;
using Microsoft.Ajax.Utilities;

namespace ImageSharePassword.Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            db.AddUser(user, password);
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            var user = db.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(email, true);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult MyAccount()
        {
            var db = new ImageDb(Properties.Settings.Default.ConStr);
            var images = db.GetUserImages(User.Identity.Name);
            return View(images);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var db = new ImageDb(Properties.Settings.Default.ConStr);
            var imageIds = db.GetUserImages(User.Identity.Name).Select(i => i.Id);
            if (!imageIds.Contains(id))
            {
                return Redirect("http://www.google.com"); // go away....
            }
            db.DeleteImage(id);
            return RedirectToAction("MyAccount");
        }

        [Authorize]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}