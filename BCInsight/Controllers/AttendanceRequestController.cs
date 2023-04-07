using Antlr.Runtime.Misc;
using BCInsight.BAL.Repository;
using BCInsight.Code;
using BCInsight.DAL;
using BCInsight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCInsight.Controllers
{
    public class AttendanceRequestController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IAttendance _attendance;
        IClockInRequest _clockInRequest;
        public AttendanceRequestController(IAttendance attendance, IClockInRequest clockInRequest)
        {
            _attendance = attendance;
            _clockInRequest = clockInRequest;
        }

        public ActionResult Index()
        {
            List<ClockInRequestViewModel> viewModelList = new List<ClockInRequestViewModel>();
            try
            {
                using (var entity = new admin_csmariseEntities())
                {
                    var list = (from c in entity.ClockInRequest
                                join u in entity.User on c.UserId equals u.Id
                                join a in entity.Attendance on c.UserId equals a.Id
                                where c.IsDeleted == false && u.IsDeleted == false
                                select new
                                {
                                    UserId = u.Id,
                                    UName = u.Name,
                                    UMobile = u.Mobile,
                                    c.Id,
                                    c.Date,
                                    c.Reason,
                                    c.Description,
                                    c.Approved,
                                    c.ApprovedBy,
                                    c.Remarks,
                                    c.CreatedOn,
                                    a.ClockIn,
                                    a.ClockOut
                                }).OrderByDescending(x => x.CreatedOn).ToList();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var atdnc = _attendance.FindBy(x => x.UserId == item.UserId && x.Date == item.Date).FirstOrDefault();
                            viewModelList.Add(new ClockInRequestViewModel
                            {
                                UserName = item.UName,
                                UserMobile = item.UMobile,
                                Id = item.Id,
                                Date = item.Date,
                                ClockIn = atdnc != null ? atdnc.ClockIn : null,
                                ClockOut = atdnc != null ? atdnc.ClockOut : null,
                                Remarks = item.Remarks,
                                Description = item.Description,
                                Reason = item.Reason,
                                CreatedOn = item.CreatedOn,
                                Approved = item.Approved != null ? item.Approved.Value : false
                            });
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(viewModelList);
        }

        public ActionResult Approve(int id, string Remarks, string Type)
        {
            DateTime? clockIn = null;
            DateTime? clockOut = null;
            try
            {
                var req = _clockInRequest.FindBy(x => x.Id == id).FirstOrDefault();
                if (req != null)
                {
                    req.Approved = true;
                    req.Remarks = Remarks;
                    req.ApprovedBy = LoginUserId();
                    req.ModifiedBy = LoginUserId();
                    req.ModifiedOn = DateTime.Now;

                    var date_only = req.Date;
                    clockIn = new DateTime(req.Date.Year, req.Date.Month, req.Date.Day, 10, 00, 00);
                    clockOut = new DateTime(req.Date.Year, req.Date.Month, req.Date.Day, 20, 00, 00);

                    var data = _attendance.FindBy(x => x.UserId == req.UserId && x.Date == date_only).FirstOrDefault();
                    if (data != null)
                    {
                        if (Type.Trim().Replace(" ", "").ToLower() == "both")
                        {
                            data.ClockIn = clockIn;
                            data.ClockOut = clockOut;
                        }
                        else
                        {
                            if (Type.Trim().Replace(" ", "").ToLower() == "checkin")
                            {
                                data.ClockIn = clockIn;
                                data.ClockOut = data.ClockOut;
                            }
                            if (Type.Trim().Replace(" ", "").ToLower() == "checkout")
                            {
                                if (data.ClockIn == null)
                                {
                                    TempData["errorMessage"] = "Do CheckIn first";
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    data.ClockIn = data.ClockIn;
                                    data.ClockOut = clockOut;
                                }
                            }
                        }

                        data.UserId = req.UserId;
                        data.Date = date_only;
                        data.Remarks = Remarks;
                        data.ModifiedOn = DateTime.Now;
                        data.ModifiedBy = LoginUserId();

                        _attendance.Edit(data);
                        _attendance.Save();
                    }
                    else
                    {
                        data = new Attendance();

                        if (Type.Trim().Replace(" ", "").ToLower() == "both")
                        {
                            data.ClockIn = clockIn;
                            data.ClockOut = clockOut;
                        }
                        else
                        {
                            if (Type.Trim().Replace(" ", "").ToLower() == "checkin")
                            {
                                data.ClockIn = clockIn;
                                data.ClockOut = data.ClockOut;
                            }
                            if (Type.Trim().Replace(" ", "").ToLower() == "checkout")
                            {
                                if (data.ClockIn == null)
                                {
                                    TempData["errorMessage"] = "Do CheckIn first";
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    data.ClockIn = data.ClockIn;
                                    data.ClockOut = clockOut;
                                }
                            }
                        }

                        data.UserId = req.UserId;
                        data.Date = date_only;
                        data.Remarks = Remarks;
                        data.CreatedOn = DateTime.Now;
                        data.CreatedBy = LoginUserId();
                        data.IsDeleted = false;

                        _attendance.Add(data);
                        _attendance.Save();
                    }

                    _clockInRequest.Edit(req);
                    _clockInRequest.Save();
                }
                else
                    TempData["errorMessage"] = "Record not found please contact your administrator.";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Reject(int id, string Remarks)
        {
            try
            {
                var data = _clockInRequest.FindBy(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.Approved = false;
                    data.Remarks = Remarks;
                    data.ModifiedBy = LoginUserId();
                    data.ModifiedOn = DateTime.Now;

                    _clockInRequest.Edit(data);
                    _clockInRequest.Save();
                }
                else
                    TempData["errorMessage"] = "Record not found please contact your administrator.";
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