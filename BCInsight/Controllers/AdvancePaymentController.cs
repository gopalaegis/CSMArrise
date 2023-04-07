using Antlr.Runtime.Misc;
using BCInsight.BAL.Repository;
using BCInsight.DAL;
using BCInsight.Models;
using BCInsight.Web.HelperClass;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace BCInsight.Controllers
{
    public class AdvancePaymentController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IAdvancePayment _advancePayment;
        public AdvancePaymentController(IAdvancePayment advancePayment)
        {
            _advancePayment = advancePayment;
        }

        public ActionResult Index()
        {
            List<AdvancePaymentViewModel> list = new List<AdvancePaymentViewModel>();
            try
            {
                using (var entity = new admin_csmariseEntities())
                {
                    var data = (from p in entity.AdvancePaymentRequest
                                join u in entity.User on p.UserId equals u.Id
                                where p.IsDeleted == false && u.IsDeleted == false
                                select new
                                {
                                    uId = u.Id,
                                    u.Name,
                                    u.Mobile,
                                    u.Role,
                                    u.Salary,
                                    u.SalaryDuration,
                                    pId = p.Id,
                                    p.RequestAmount,
                                    p.Reason,
                                    p.ApprovedAmount,
                                    p.ApprovedOn,
                                    p.Remarks,
                                    p.CreatedOn,
                                    p.IsApprove
                                }).OrderByDescending(x => x.CreatedOn).ToList();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            list.Add(new AdvancePaymentViewModel()
                            {
                                Id = item.pId,
                                UserId = item.pId,
                                UName = item.Name,
                                UMobile = item.Mobile,
                                URole = item.Role,
                                USalary = item.Salary.Value,
                                USalaryDuration = item.SalaryDuration,
                                RequestAmount = item.RequestAmount,
                                Reason = item.Reason,
                                ApprovedAmount = item.ApprovedAmount,
                                ApprovedOn = item.ApprovedOn,
                                Remarks = item.Remarks,
                                IsApprove = item.IsApprove,
                                CreatedOn = item.CreatedOn
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error occured please contact your administrator.";
                Log.Error(ex);
            }
            return View(list);
        }

        public ActionResult Approve(int Id, double Amount, string Remarks)
        {
            try
            {
                var payment = _advancePayment.FindBy(x => x.Id == Id && x.IsDeleted == false).FirstOrDefault();
                if (payment == null)
                    TempData["errorMessage"] = "Record not found please contact your administrator.";
                else
                {
                    payment.IsApprove = true;
                    payment.ApprovedAmount = Amount;
                    payment.Remarks = Remarks;
                    payment.ApprovedOn = DateTime.Now;
                    payment.ApprovedBy = LoginUserId();
                    payment.ModifiedOn = DateTime.Now;
                    payment.ModifiedBy = LoginUserId();

                    _advancePayment.Edit(payment);
                    _advancePayment.Save();
                }
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
                var payment = _advancePayment.FindBy(x => x.Id == Id && x.IsDeleted == false).FirstOrDefault();
                if (payment == null)
                    TempData["errorMessage"] = "Record not found please contact your administrator.";
                else
                {
                    payment.IsApprove = false;
                    payment.Remarks = Remarks;
                    payment.ModifiedOn = DateTime.Now;
                    payment.ModifiedBy = LoginUserId();

                    _advancePayment.Edit(payment);
                    _advancePayment.Save();
                }
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