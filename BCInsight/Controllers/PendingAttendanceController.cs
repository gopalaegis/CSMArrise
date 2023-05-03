using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCInsight.Models;
using BCInsight.BAL.Repository;
using BCInsight.DAL;

namespace BCInsight.Controllers
{
    public class PendingAttendanceController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IUser _user;
        admin_csmariseEntities entities = new admin_csmariseEntities();
        // GET: PendingAttendance
        List<SelectListItem> Userlist = new List<SelectListItem>();

        public PendingAttendanceController(IUser user)
        {
            _user = user;
        }
        
        public ActionResult Index()
        {
            PendingAttendanceViewModel userattendence = new PendingAttendanceViewModel();
            try
            {
                var user = entities.User.Where(x => x.IsDeleted == false && x.Role != "admin").Select(x => new
                {
                    x.Id,
                    x.Name,
                }).OrderBy(o => o.Name).ToList();

                if (user != null && user.Count > 0)
                {
                    foreach (var item in user)
                    {
                        Userlist.Add(new SelectListItem()
                        {
                            Value = item.Id.ToString(),
                            Text = item.Name,
                            Selected = false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            ViewBag.Userlist = Userlist.OrderBy(x => x.Text).ToList();
            return View(userattendence);
        }

        [HttpPost]
        public ActionResult Index(PendingAttendanceViewModel model)
        {
            List<int> userIds = new List<int>();
            PendingAttendanceViewModel attendance = new PendingAttendanceViewModel();
            try
            {
                var startDate = DateTime.ParseExact(model.ToDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact(model.FromDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                var totaldays = Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d)).ToList();

                var useridlist = model.Users.Split(',');
                foreach (var item in useridlist)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                        userIds.Add(Convert.ToInt32(item));
                };

                foreach (var userid in userIds)
                {
                    var userdata = entities.User.Where(x => x.Id == userid).FirstOrDefault();

                    if (userdata != null)
                    {
                        foreach (var date in totaldays)
                        {
                            var isexits = entities.Attendance.Where(x => x.Date == date && x.UserId == userid).FirstOrDefault();

                            DateTime checkIn, checkOut;
                            if (userdata.StartTime != null)
                                checkIn = new DateTime(date.Year, date.Month, date.Day, userdata.StartTime.Value.Hours, userdata.StartTime.Value.Minutes, 0);
                            else
                                checkIn = new DateTime(date.Year, date.Month, date.Day, 10, 00, 00);

                            if (userdata.EndTime != null)
                                checkOut = new DateTime(date.Year, date.Month, date.Day, userdata.EndTime.Value.Hours, userdata.EndTime.Value.Minutes, 0);
                            else
                                checkOut = new DateTime(date.Year, date.Month, date.Day, 20, 00, 00);


                            if (isexits == null)
                            {
                                Attendance obj = new Attendance()
                                {
                                    UserId = userid,
                                    Date = date,
                                    ClockIn = checkIn,
                                    ClockOut = checkOut,
                                    Remarks = "from adjust attendance",
                                    CreatedBy = LoginUserId(),
                                    CreatedOn = DateTime.Now,
                                    IsDeleted = false,
                                    IsMannualReq = false,
                                    IsApprove = false
                                };
                                entities.Attendance.Add(obj);
                                entities.SaveChanges();
                                TempData["success"] = "record added succesfully";
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                string messgae = ex.Message;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}