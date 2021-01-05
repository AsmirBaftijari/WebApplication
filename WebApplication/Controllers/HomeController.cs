using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private DBModels2 db = new DBModels2();

        // GET: Eintraeges
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null)
            {
                var Eintraeges = db.Eintraeges.Where(E => E.Autorisiert_ID != null);
                return View(db.Eintraeges.ToList());
            }
            else
            {
                var Eintraeges = db.Eintraeges.Include(E => E.Eintrag_Id);
                return View(db.Eintraeges.ToList());
            }
        }

        public ActionResult About()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Authorise(User user)
        {
            using (DBModels db = new DBModels())
            {
                var userDetail = db.Users.Where(x => x.UserName == user.UserName && x.Password == user.Password).FirstOrDefault();

                if (userDetail == null)
                {
                    user.LoginErrorMsg = "Falsches Passwort oder Benutzer eingegeben";
                    return View("About", user);
                }
                else
                {
                    Session["userID"] = userDetail.UserID;
                    Session["userName"] = user.UserName;
                    Session["isAdmin"] = true;
                    return RedirectToAction("Index", "Eintraeges");
                }
            }
        }
        public ActionResult LogOut()
        {
            int userId = (int)Session["userID"];
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Contact()
        {
            return View();
        }
        public string CalculateHash(string Password)
        {
            using (var algorithm = SHA512.Create()) //or MD5 SHA256 etc.
            {
                var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Password));

                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public bool CheckMatch(string hash, string Password)
        {
            try
            {
                var parts = hash.Split(':');
                var salt = Convert.FromBase64String(parts[0]);
                var bytes = KeyDerivation.Pbkdf2(Password, salt, KeyDerivationPrf.HMACSHA512, 10000, 16);

                return Equals(Convert.ToBase64String(bytes));
            }
            catch
            {
                return false;

            }
        }
    }
}