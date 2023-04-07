using BCInsight.BAL.Repository;
using BCInsight.DAL;
using BCInsight.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BCInsight.Controllers
{
    public class LoginController : BaseController
    {
        //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUser _user;
        //TblAdminLog saveadminlog = new TblAdminLog();
        public LoginController(IUser user)
        {
            _user = user;
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public ActionResult Login(UserViewModel model)
        {
            try
            {
                var cu = new HttpCookie("admin_id");
                cu.Expires = DateTime.Now.AddMinutes(-1);
                cu.Value = "";
                Response.Cookies.Add(cu);

                if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Password))
                {
                    //User isActiveUser = _user.FindBy(c => (c.UserName == model.UserName || c.Email == model.UserName) && c.Password == model.Password).FirstOrDefault();
                    User isActiveUser = _user.FindBy(c => (c.UserName == model.UserName || c.Mobile == model.UserName) && c.Password == model.Password).FirstOrDefault();
                    if (isActiveUser != null)
                    {
                        if (!isActiveUser.IsDeleted)
                        {
                            if (isActiveUser.Role.ToLower() == "admin")
                            {
                                var c = new HttpCookie("admin_id");
                                c.Expires = DateTime.Now.AddYears(+1);
                                c.Value = isActiveUser.Id.ToString();
                                Response.Cookies.Add(c);

                                FormsAuthentication.SetAuthCookie(isActiveUser.Id.ToString(), true);

                                Session["admin_id"] = isActiveUser.Id;
                                string hostName = Dns.GetHostName();
                                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["loginfail"] = "Unauthenticated user";
                            }
                        }
                        else
                        {
                            TempData["loginfail"] = "User is deleted";
                        }
                    }
                    else
                    {
                        TempData["loginfail"] = "Wrong username or password";
                    }
                }
                else
                {
                    TempData["loginfail"] = "Enter username or password";
                }

            }
            catch (Exception e)
            {
                //Log.Error(e);
                //this.SetNotification("Check Internet Connection", NotificationEnum.Warning);
            }
            return RedirectToAction("Index", "Login");
        }

        public ActionResult Logout()
        {
            int userId = Convert.ToInt32(Session["admin_id"]);
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            FormsAuthentication.SignOut();
            Session["admin_id"] = null;
            Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Request.Cookies.Add(cookie);

            var c = new HttpCookie("admin_id");
            c.Expires = DateTime.Now.AddMinutes(-1);
            c.Value = "";
            Response.Cookies.Add(c);

            return RedirectToAction("Index", "Login");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(UserViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("UserName", "Enter UserName.");
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("password", "Enter Password.");
            }
            if (string.IsNullOrEmpty(model.Newpassword))
            {
                ModelState.AddModelError("Newpassword", "Enter New Password.");
            }
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Newpassword))
            {
                return View("ChangePassword", model);
            }
            else
            {
                //User isActiveUser = _user.FindBy(c => (c.UserName == model.UserName || c.Email == model.UserName) && c.Password == model.Password && c.IsDeleted == false).FirstOrDefault();
                User isActiveUser = _user.FindBy(c => (c.UserName == model.UserName || c.Mobile == model.UserName) && c.Password == model.Password && c.IsDeleted == false).FirstOrDefault();
                if (isActiveUser != null)
                {
                    model.UserId = Convert.ToInt32(Session["admin_id"].ToString());
                    User savemodel = _user.FindBy(c => c.Id == model.UserId && c.IsDeleted == false).FirstOrDefault();
                    if (savemodel != null)
                    {
                        savemodel.Password = model.Newpassword;
                        _user.Edit(savemodel);
                        _user.Save();
                        //this.SetNotification("Password Change successfully", NotificationEnum.Success);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["loginfail"] = "Wrong username or password";
                }
            }
            return RedirectToAction("ChangePassword", "Login");

        }

        public ActionResult Users()
        {
            List<User> adminuserslist = new List<User>();
            try
            {
                adminuserslist = _user.GetAll().OrderBy(c => c.Name).ToList();
            }
            catch (Exception e)
            {
                //Log.Error(e);
            }

            return View(adminuserslist);
        }

        public ActionResult Register(int? id)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                if (id != null && id > 0)
                {
                    User savemodel = _user.FindBy(c => c.Id == id && c.IsDeleted == false).FirstOrDefault() ?? new User();
                    model.UserId = savemodel.Id;
                    model.UserName = savemodel.UserName;
                    model.Password = savemodel.Password;
                    model.Confirmpassword = savemodel.Password;
                    model.Name = savemodel.Name;
                    model.Email = savemodel.Email;
                    model.Role = savemodel.Role;
                }
            }
            catch (Exception e)
            {
                //Log.Error(e);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(UserViewModel model)
        {
            try
            {
                User savemodel = _user.FindBy(c => c.Id == model.UserId).FirstOrDefault() ?? new User();
                //User isActiveUser = _user.FindBy(c => (c.UserName == model.UserName || c.Email == model.UserName)).FirstOrDefault();
              User isActiveUser = _user.FindBy(c => (c.UserName == model.UserName || c.Mobile == model.UserName)).FirstOrDefault();
                if (isActiveUser != null)
                {
                    TempData["Message"] = "UserName or Mobile already exist";
                    return View("Register", model);
                }
                savemodel.Id = model.UserId;
                savemodel.UserName = model.UserName;
                savemodel.Password = model.Password;
                savemodel.Name = model.Name;
                savemodel.Email = model.Email;
                savemodel.Role = model.Role;
                savemodel.IsDeleted = false;
                if (model.UserId > 0)
                {
                    _user.Edit(savemodel);
                    _user.Save();
                }
                else
                {
                    savemodel.Password = model.Password;
                    _user.Add(savemodel);
                    _user.Save();
                }

            }
            catch (Exception e)
            {
                TempData["Message"] = e.Message;
                return View("Register", model);
            }

            return RedirectToAction("Users");

        }

        [HttpGet]
        public ActionResult deleteUser(int? Id)
        {
            try
            {
                User DelUserd = _user.FindBy(c => c.Id == Id).FirstOrDefault();
                if (DelUserd != null)
                {
                    _user.Delete(DelUserd);
                    _user.Save();
                    //this.SetNotification("User Deleted Successfully.", NotificationEnum.Success);
                }
            }
            catch (Exception e)
            {
                //Log.Error(e);
            }
            return RedirectToAction("Users");
        }


    }
}