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
    public class LeaveRequestController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ILeaveRequest _leaveRequest;
        IUser _user;

        public LeaveRequestController(IUser user, ILeaveRequest leaveRequest)
        {
            _user = user;
            _leaveRequest = leaveRequest;
        }

        public ActionResult Index()
        {
            List<LeaveRequestViewModel> _retList = new List<LeaveRequestViewModel>();
            try
            {
                using (var entity = new admin_csmariseEntities())
                {
                    var dataList = (from l in entity.LeaveRequest
                                    join u in entity.User on l.UserId equals u.Id
                                    where l.IsDeleted == false && u.IsDeleted == false
                                    select new
                                    {
                                        l.Id,
                                        l.UserId,
                                        l.FromDate,
                                        l.ToDate,
                                        l.NoOfDays,
                                        l.Reason,
                                        l.ApproveDays,
                                        l.ApprovedBy,
                                        l.IsApproved,
                                        l.Remarks,
                                        l.CreatedBy,
                                        l.CreatedOn,
                                        l.ModifiedBy,
                                        l.ModifiedOn,
                                        UserName = u.Name
                                    }).ToList();

                    if (dataList != null && dataList.Count > 0)
                    {
                        foreach (var item in dataList)
                        {
                            _retList.Add(new LeaveRequestViewModel()
                            {
                                Id = item.Id,
                                UserId = item.UserId,
                                UserName = item.UserName,
                                FromDate = item.FromDate,
                                ToDate = item.ToDate,
                                NoOfDays = item.NoOfDays,
                                Reason = item.Reason,
                                ApproveDays = item.ApproveDays,
                                ApprovedBy = item.ApprovedBy,
                                IsApproved = item.IsApproved,
                                Remarks = item.Remarks,
                                CreatedBy = item.CreatedBy,
                                CreatedOn = item.CreatedOn,
                                ModifiedBy = item.ModifiedBy,
                                ModifiedOn = item.ModifiedOn
                            });
                        }
                        _retList.OrderByDescending(x => x.CreatedOn).ToList();
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

        public ActionResult Approve(int Id, int approveDays, string Remarks)
        {
            try
            {
                var leave = _leaveRequest.FindBy(x => x.Id == Id).FirstOrDefault();
                if (leave != null)
                {
                    leave.ApproveDays = approveDays;
                    leave.Remarks = Remarks;
                    leave.ApprovedBy = LoginUserId();
                    leave.ModifiedBy = LoginUserId();
                    leave.ModifiedOn = DateTime.Now;
                    leave.IsApproved = true;

                    _leaveRequest.Edit(leave);
                    _leaveRequest.Save();
                }
                else
                    TempData["errorMessage"] = "Record not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Reject(int Id, string Remarks)
        {
            try
            {
                var leave = _leaveRequest.FindBy(x => x.Id == Id).FirstOrDefault();
                if (leave != null)
                {
                    leave.Remarks = Remarks;
                    leave.ModifiedBy = LoginUserId();
                    leave.ModifiedOn = DateTime.Now;
                    leave.IsApproved = false;

                    _leaveRequest.Edit(leave);
                    _leaveRequest.Save();
                }
                else
                    TempData["errorMessage"] = "Record not found.";
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