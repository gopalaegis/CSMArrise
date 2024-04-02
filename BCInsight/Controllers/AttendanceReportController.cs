using BCInsight.BAL.Repository;
using BCInsight.DAL;
using BCInsight.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCInsight.Controllers
{
    public class AttendanceReportController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IAttendance _attendance;
        admin_csmariseEntities db = new admin_csmariseEntities();
        public AttendanceReportController(IAttendance attendance)
        {
            _attendance = attendance;
        }
        public ActionResult Index()
        {
            try
            {
                ViewBag.YearList = _attendance.GetAll().Select(x => new SelectListItem()
                {
                    Value = x.Date.Year.ToString(),
                    Text = x.Date.Year.ToString()
                }).Distinct();
                ViewBag.MonthList = _attendance.GetAll().Select(x => new SelectListItem()
                {
                    Value = x.Date.Month.ToString(),
                    Text = x.Date.Month.ToString()
                }).Distinct();
                //var Year = _attendance.GetAll().Select(x => x.Date.Year).Distinct().ToList();

                //var Month = _attendance.GetAll().Select(x => x.Date.Month).Distinct().ToList();

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return View();
        }

        public ActionResult FetchDailyData(string reportdate)
        {
            AttendanceViewModel av = new AttendanceViewModel();
            List<AttendanceViewModel> lstav = new List<AttendanceViewModel>();
            DateTime dtreportdate = DateTime.Now;
            //Log.Error("before convert date is : " + reportdate);
            try
            {
                try
                {
                    dtreportdate = DateTime.ParseExact(reportdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    dtreportdate = Convert.ToDateTime(reportdate);
                }
                //Log.Error("After convert date is : " + reportdate);
                //var lstdata = _attendance.FindBy(x => x.Date == dtreportdate).ToList();
                //foreach (var item in lstdata)
                //{
                //    av.Date = item.Date;
                //    av.ClockIn = item.ClockIn;
                //    av.UserName = item.
                //}
                using (var entity = new admin_csmariseEntities())
                {
                    var dataList = (from l in entity.Attendance
                                    join u in entity.User on l.UserId equals u.Id
                                    where l.Date.Day == dtreportdate.Day && l.Date.Month == dtreportdate.Month && l.Date.Year == dtreportdate.Year
                                    select new
                                    {
                                        l.Date,
                                        l.UserId,
                                        l.ClockIn,
                                        UserName = u.Name
                                    }).ToList();

                    if (dataList != null && dataList.Count > 0)
                    {
                        foreach (var item in dataList)
                        {
                            lstav.Add(new AttendanceViewModel()
                            {
                                Date = item.Date,
                                ClockIn = item.ClockIn,
                                UserName = item.UserName
                            });
                        }
                        lstav.OrderByDescending(x => x.Date).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return PartialView("~/Views/AttendanceReport/_PartialDailyReport.cshtml", lstav);
        }

        public ActionResult FetchMonthlyData(int Month, int Year)
        {
            AttendanceViewModel av = new AttendanceViewModel();
            List<AttendanceViewModel> lstav = new List<AttendanceViewModel>();
            //var lstdata = _attendance.FindBy(x => x.Date == dtreportdate).ToList();
            //foreach (var item in lstdata)
            //{
            //    av.Date = item.Date;
            //    av.ClockIn = item.ClockIn;
            //    av.UserName = item.
            //}
            try
            {
                using (var entity = new admin_csmariseEntities())
                {
                    var dataList = (from l in entity.Attendance
                                    join u in entity.User on l.UserId equals u.Id
                                    where l.Date.Year == Year && l.Date.Month == Month
                                    select new
                                    {
                                        l.Date,
                                        l.UserId,
                                        l.ClockIn,
                                        UserName = u.Name
                                    }).ToList();

                    if (dataList != null && dataList.Count > 0)
                    {
                        foreach (var item in dataList)
                        {
                            lstav.Add(new AttendanceViewModel()
                            {
                                Date = item.Date,
                                ClockIn = item.ClockIn,
                                UserName = item.UserName,
                                DaysInMonth = DateTime.DaysInMonth(Year, Month)
                            });
                        }
                        lstav.OrderByDescending(x => x.Date).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return PartialView("~/Views/AttendanceReport/_PartialMonthlyReport.cshtml", lstav);
        }
    }
}