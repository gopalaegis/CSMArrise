using BCInsight.BAL.Repository;
using BCInsight.DAL;
using BCInsight.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using IronXL.Xml.Spreadsheet;
using Microsoft.Practices.ObjectBuilder2;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace BCInsight.Controllers
{
    public class UserController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IUser _user;
        ISite _site;
        ISiteUser _siteUser;
        IUserdepartment _userdepartment;
        public UserController(IUser user, ISite site, ISiteUser siteUser, IUserdepartment userdepartment)
        {
            _user = user;
            _site = site;
            _siteUser = siteUser;
            _userdepartment = userdepartment;
        }
        public ActionResult Index()
        {
            List<UserViewModel> _retList = new List<UserViewModel>();
            try
            {
                using (var entities = new admin_csmariseEntities())
                {
                    var siteUser = (from su in entities.SiteUser
                                    join s in entities.Site on su.SiteId equals s.Id
                                    where su.IsDeleted == false && s.IsDeleted == false
                                    select new
                                    {
                                        siteUserId = su.Id,
                                        siteId = su.SiteId,
                                        siteName = s.Name,
                                        userId = su.UserId,
                                        su.IsDeleted
                                    }).ToList();

                    var deptUser = (from dp in entities.UserDepartment
                                    join d in entities.Department on dp.DepartmentId equals d.Id
                                    where dp.IsDeleted == false && d.IsDeleted == false
                                    select new
                                    {
                                        userdeprtmentId = dp.Id,
                                        userId = dp.UserId,
                                        departmentname = d.Name,
                                        departmentId = dp.DepartmentId,
                                        dp.IsDeleted
                                    }).ToList();



                    var userList = entities.User.Where(x => x.IsDeleted == false).ToList();

                    if (userList != null)
                    {
                        foreach (var item in userList)
                        {
                            string sDateTime = string.Empty, eDateTime = string.Empty;
                            if (item.StartTime != null)
                            {
                                string startDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("yyyy-MM-dd ") + item.StartTime.ToString();
                                if (DateTime.TryParse(startDateTime, out DateTime Temp) == true)
                                {
                                    sDateTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("hh:mm tt");
                                }
                            }

                            if (item.EndTime != null)
                            {
                                string endDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("yyyy-MM-dd ") + item.EndTime.ToString();
                                if (DateTime.TryParse(endDateTime, out DateTime Temp1) == true)
                                {
                                    eDateTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("hh:mm tt");
                                }
                            }

                            string s_name = string.Empty;
                            var siteNames = siteUser.Where(x => x.userId == item.Id).Select(x => x.siteName).ToList();
                            if (siteNames != null && siteNames.Count > 0)
                                s_name = siteNames.JoinStrings(", ");

                            string d_name = string.Empty;
                            var deptName = deptUser.Where(x => x.userId == item.Id).Select(x => x.departmentname).ToList();
                            if (deptName != null && deptName.Count > 0)
                            {
                                d_name = deptName.JoinStrings(", ");
                            }
                            var managerName = userList.Where(x => x.Id == item.Manager).Select(x => x.Name).FirstOrDefault();


                            _retList.Add(new UserViewModel()
                            {
                                UserId = item.Id,
                                Name = item.Name,
                                Role = item.Role,
                                Mobile = item.Mobile,
                                UserName = item.UserName,
                                Password = item.Password,
                                Email = item.Email,
                                Salary = item.Salary,
                                Remarks = item.Remarks,
                                SiteName = s_name,
                                CreatedOn = item.CreatedOn,
                                IsDeleted = item.IsDeleted,
                                DepartmentName = d_name,
                                ManagerName = managerName,
                                StartTime = sDateTime,
                                EndTime = eDateTime,
                                WeekOffDay = item.WeekOffDay != null ? item.WeekOffDay.ToUpper() : string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(_retList);
        }

        public ActionResult AddUser(int? Id)
        {
            UserViewModel user = new UserViewModel();
            List<SelectListItem> siteItemList = new List<SelectListItem>();
            List<SelectListItem> deptItemList = new List<SelectListItem>();
            List<SelectListItem> UserList = new List<SelectListItem>();
            try
            {
                using (var entities = new admin_csmariseEntities())
                {
                    var sites = entities.Site.Where(x => x.IsDeleted == false && x.IsClosed == false).Select(x => new
                    {
                        x.Id,
                        x.Name,
                    }).OrderBy(o => o.Name).ToList();

                    if (sites != null && sites.Count > 0)
                    {
                        foreach (var item in sites)
                        {
                            siteItemList.Add(new SelectListItem()
                            {
                                Value = item.Id.ToString(),
                                Text = item.Name,
                                Selected = false
                            });
                        }
                    }

                    var Department = entities.Department.Where(x => x.IsDeleted == false).Select(x => new
                    {
                        x.Id,
                        x.Name,
                    }).OrderBy(o => o.Name).ToList();

                    if (Department != null && Department.Count > 0)
                    {
                        foreach (var item in Department)
                        {
                            deptItemList.Add(new SelectListItem()
                            {
                                Value = item.Id.ToString(),
                                Text = item.Name,
                                Selected = false
                            });
                        }
                    }

                    var managerList = entities.User.Where(x => x.IsDeleted == false && x.Role.Trim().Replace(" ", "").ToLower() == "manager").Select(x => new
                    {
                        x.Id,
                        x.UserName,
                    }).OrderBy(o => o.UserName).ToList();

                    if (managerList != null && managerList.Count > 0)
                    {
                        foreach (var item in managerList)
                        {
                            UserList.Add(new SelectListItem()
                            {
                                Value = item.Id.ToString(),
                                Text = item.UserName,
                                Selected = false
                            });
                        }
                    }
                    ViewBag.ManagerList = UserList.OrderBy(x => x.Text).ToList();

                    if (Id != null && Id > 0)
                    {
                        var userdata = _user.FindBy(x => x.Id == Id).FirstOrDefault();
                        user.UserId = userdata.Id;
                        user.Name = userdata.Name;
                        user.UserName = userdata.UserName;
                        user.Password = userdata.Password;
                        user.Mobile = userdata.Mobile;
                        user.Email = userdata.Email;
                        user.Role = userdata.Role;
                        user.Salary = userdata.Salary;
                        user.ManagerId = (int)userdata.Manager;
                        user.StartTime = userdata.StartTime.ToString();
                        user.EndTime = userdata.EndTime.ToString();
                        user.WeekOffDay = userdata.WeekOffDay;

                        var userSites = _siteUser.FindBy(x => x.UserId == Id).Select(x => x.SiteId).ToList();
                        if (userSites != null && userSites.Count > 0)
                        {
                            foreach (var item in userSites)
                            {
                                var data = siteItemList.Where(x => x.Value == item.ToString()).FirstOrDefault();
                                if (data != null)
                                {
                                    data.Selected = true;

                                    siteItemList.Remove(data);
                                    siteItemList.Add(data);
                                }
                            }
                        }

                        var dList = _userdepartment.FindBy(x => x.UserId == Id).Select(x => x.DepartmentId).ToList();
                        if (dList != null && dList.Count > 0)
                        {
                            foreach (var dept in dList)
                            {
                                var dptlist = deptItemList.Where(x => x.Value == dept.ToString()).FirstOrDefault();
                                if (dptlist != null)
                                {
                                    dptlist.Selected = true;
                                    deptItemList.Remove(dptlist);
                                    deptItemList.Add(dptlist);
                                }
                            }
                        }

                    }

                    ViewBag.SiteList = siteItemList.OrderBy(x => x.Text).ToList();
                    ViewBag.department = deptItemList.OrderBy(x => x.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult AddUser(UserViewModel model)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "somthing went wrong, please try again letter");
            List<int> siteIds = new List<int>();
            List<string> sError = new List<string>();
            //List<Site> sites = new List<Site>();
            List<int> deptIds = new List<int>();
            try
            {
                var site = model.SiteName.Split(',');
                foreach (var item in site)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                        siteIds.Add(Convert.ToInt32(item));
                };

                string startDateTime = string.Empty, endDateTime = string.Empty;
                TimeSpan? StartTime, EndTime;

                if (model.StartTime != null && model.StartTime.Length == 6)
                    model.StartTime = "0" + model.StartTime;
                if (model.EndTime != null && model.EndTime.Length == 6)
                    model.EndTime = "0" + model.EndTime;

                if (model.StartTime != null)
                {
                    startDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("yyyy-MM-dd ") + model.StartTime.Trim().ToString().ToUpper();
                    if (DateTime.TryParse(startDateTime, out DateTime Temp) == true)
                    {
                        StartTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mmtt", CultureInfo.InvariantCulture).TimeOfDay;
                    }
                }

                if (model.EndTime != null)
                {
                    endDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("yyyy-MM-dd ") + model.EndTime.Trim().ToString().ToUpper();
                    if (DateTime.TryParse(endDateTime, out DateTime Temp1) == true)
                    {
                        EndTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mmtt", CultureInfo.InvariantCulture).TimeOfDay;
                    }
                }

                using (var entity = new admin_csmariseEntities())
                {
                    //sites = entity.Site.ToList();
                    if (model.UserId > 0)
                    {
                        var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                        if (user == null)
                        {
                            var msg = $"User not found. please try again.";
                            tuple = new Tuple<bool, string>(false, msg);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var exist = _user.FindBy(x => x.Mobile == model.Mobile && x.IsDeleted == false && x.Id != model.UserId).FirstOrDefault();
                            if (exist != null)
                            {
                                var msg = $"User with Mobile: {model.Mobile} already exist";
                                tuple = new Tuple<bool, string>(false, msg);
                                return Json(tuple, JsonRequestBehavior.AllowGet);
                            }

                            user.Id = user.Id;
                            user.Name = model.Name;
                            user.UserName = model.UserName;
                            if (string.IsNullOrEmpty(model.Password))
                            {
                                user.Password = user.Password;
                            }
                            else
                            {
                                user.Password = model.Password;
                            }

                            user.Mobile = model.Mobile;
                            user.Email = model.Email;
                            user.Role = model.Role;
                            user.Salary = model.Salary;
                            user.Manager = model.ManagerId;
                            user.WeekOffDay = model.WeekOffDay;
                            if (!string.IsNullOrEmpty(startDateTime))
                                user.StartTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mmtt", CultureInfo.InvariantCulture).TimeOfDay;
                            if (!string.IsNullOrEmpty(endDateTime))
                                user.EndTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mmtt", CultureInfo.InvariantCulture).TimeOfDay;
                            user.ModifiedBy = LoginUserId();
                            user.ModifiedOn = DateTime.Now;

                            _user.Edit(user);
                            _user.Save();
                        }

                        var userId = user.Id;

                        var suList = entity.SiteUser.Where(x => x.UserId == user.Id).ToList() ?? new List<SiteUser>();
                        foreach (var item in suList)
                        {
                            entity.SiteUser.Remove(item);
                            entity.SaveChanges();
                        }
                        if (siteIds.Count > 0)
                        {
                            foreach (int id in siteIds)
                            {
                                SiteUser siteuser = new SiteUser()
                                {
                                    SiteId = id,
                                    UserId = user.Id,
                                    IsDeleted = false,
                                    Remarks = string.Empty,
                                    CreatedBy = LoginUserId(),
                                    CreatedOn = DateTime.Now
                                };
                                entity.SiteUser.Add(siteuser);
                                entity.SaveChanges();
                            }
                        }
                        if (!string.IsNullOrEmpty(model.DepartmentIds))
                        {
                            var dept = model.DepartmentIds.Split(',');
                            foreach (var item in dept)
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                    deptIds.Add(Convert.ToInt32(item));
                            };

                            var deptlist = entity.UserDepartment.Where(x => x.UserId == user.Id).ToList() ?? new List<UserDepartment>();
                            foreach (var deptid in deptlist)
                            {
                                entity.UserDepartment.Remove(deptid);
                                entity.SaveChanges();
                            }

                            if (deptIds.Count > 0)
                            {
                                foreach (var d in deptIds)
                                {
                                    UserDepartment userDepartment = new UserDepartment()
                                    {
                                        UserId = user.Id,
                                        DepartmentId = d,
                                        CreatedBy = LoginUserId(),
                                        CreatedOn = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    entity.UserDepartment.Add(userDepartment);
                                    entity.SaveChanges();
                                }
                            }
                        }
                        TempData["success"] = "Record updated successfully";
                        tuple = new Tuple<bool, string>(true, string.Empty);
                        return Json(tuple, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var user = _user.FindBy(x => x.Mobile == model.Mobile && x.IsDeleted == false).FirstOrDefault();
                        if (user != null)
                        {
                            var msg = $"User with {model.Mobile} already exist";
                            tuple = new Tuple<bool, string>(false, msg);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }

                        User userData = new User()
                        {
                            Name = model.Name,
                            UserName = model.UserName,
                            Password = GeneratePassword(),
                            Mobile = model.Mobile,
                            Email = model.Email,
                            Role = model.Role,
                            Salary = model.Salary,
                            Manager = model.ManagerId,
                            WeekOffDay = model.WeekOffDay,
                            IsDeleted = false,
                            CreatedBy = LoginUserId(),
                            CreatedOn = DateTime.Now
                        };

                        if (!string.IsNullOrEmpty(startDateTime))
                            userData.StartTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mmtt", CultureInfo.InvariantCulture).TimeOfDay;
                        if (!string.IsNullOrEmpty(endDateTime))
                            userData.EndTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mmtt", CultureInfo.InvariantCulture).TimeOfDay;

                        entity.User.Add(userData);
                        var save = entity.SaveChanges();

                        if (save <= 0)
                        {
                            var msg = "Error occured while creating new user";
                            tuple = new Tuple<bool, string>(false, msg);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }

                        var userid = userData.Id;
                        if (!string.IsNullOrEmpty(model.DepartmentIds))
                        {
                            var dept = model.DepartmentIds.Split(',');
                            foreach (var item in dept)
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                    deptIds.Add(Convert.ToInt32(item));
                            };

                            var deptlist = entity.UserDepartment.Where(x => x.UserId == userData.Id).ToList() ?? new List<UserDepartment>();
                            foreach (var deptid in deptlist)
                            {
                                entity.UserDepartment.Remove(deptid);
                                entity.SaveChanges();
                            }

                            if (deptIds.Count > 0)
                            {
                                foreach (var d in deptIds)
                                {
                                    UserDepartment userDepartment = new UserDepartment()
                                    {
                                        UserId = userid,
                                        DepartmentId = d,
                                        CreatedBy = LoginUserId(),
                                        CreatedOn = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    entity.UserDepartment.Add(userDepartment);
                                    entity.SaveChanges();
                                }
                            }
                        }

                        try
                        {
                            var suList = entity.SiteUser.Where(x => x.UserId == userData.Id).ToList() ?? new List<SiteUser>();
                            foreach (var item in suList)
                            {
                                entity.SiteUser.Remove(item);
                                entity.SaveChanges();
                            }
                            if (siteIds.Count > 0)
                            {
                                foreach (int id in siteIds)
                                {
                                    SiteUser siteuser = new SiteUser()
                                    {
                                        SiteId = id,
                                        UserId = userData.Id,
                                        IsDeleted = false,
                                        Remarks = string.Empty,
                                        CreatedBy = LoginUserId(),
                                        CreatedOn = DateTime.Now
                                    };
                                    entity.SiteUser.Add(siteuser);
                                    entity.SaveChanges();
                                }
                                tuple = new Tuple<bool, string>(true, string.Empty);
                                return Json(tuple, JsonRequestBehavior.AllowGet);
                            }
                            TempData["success"] = "User Added successfully";
                            tuple = new Tuple<bool, string>(true, string.Empty);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            var msg = "User details saved. Unable assign user site, please contact your administrator.";
                            tuple = new Tuple<bool, string>(false, msg);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                var msg = "Error occured please contact your administrator.";
                tuple = new Tuple<bool, string>(false, msg);
                return Json(tuple, JsonRequestBehavior.AllowGet);
            }
        }

        public string GeneratePassword()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            return r;
        }

        public ActionResult DeleteUser(int Id)
        {
            try
            {
                var user = _user.FindBy(c => c.Id == Id).FirstOrDefault();
                if (user != null)
                {
                    user.IsDeleted = user.Id == 7 || user.Id == 9 ? false : true;
                    user.DeletedBy = LoginUserId();
                    user.DeletedOn = DateTime.Now;
                    user.ModifiedBy = LoginUserId();
                    user.ModifiedOn = DateTime.Now;

                    _user.Edit(user);
                    _user.Save();

                    var siteUser = _siteUser.FindBy(x => x.UserId == user.Id).ToList();
                    foreach (var item in siteUser)
                    {
                        _siteUser.Delete(item);
                        _siteUser.Save();
                    }

                    var deptuser = _userdepartment.FindBy(x => x.UserId == user.Id).ToList();
                    foreach (var item in deptuser)
                    {
                        _userdepartment.Delete(item);
                        _userdepartment.Save();
                    }
                    TempData["success"] = "Record Deleted successfully";
                }
                else
                    TempData["errorMessage"] = "User record not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return RedirectToAction("Index");
        }
    }
}