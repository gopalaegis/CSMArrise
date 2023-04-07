using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCInsight.DAL;
using BCInsight.BAL.Repository;
using BCInsight.Models;
using System.Globalization;

namespace BCInsight.Controllers
{
    public class HolidayController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        admin_csmariseEntities context = new admin_csmariseEntities();
        IHoliday _holiday;

        public HolidayController(IHoliday holiday)
        {
            _holiday = holiday;

        }
        // GET: Holiday
        public ActionResult Index()
        {
            var result = new List<HolidayViewModel>();
            try
            {
                result = (from h in context.Holiday
                          join d in context.Department on h.DepartmentId equals d.Id
                          select new HolidayViewModel()
                          {
                              Id = h.Id,
                              Year = h.Year,
                              Date = h.Date.ToString(),
                              Remarks = h.Remarks,
                              CreatedBy = h.CreatedBy,
                              CreatedOn = h.CreatedOn,
                              ModifiedBy = h.ModifiedBy,
                              ModifiedOn = h.ModifiedOn,
                              IsDeleted = h.IsDeleted,
                              DeletedBy = h.DeletedBy,
                              DeletedOn = h.DeletedOn,
                              DeaprtmentName = d.Name,
                              DepartmentIds = d.Id.ToString()
                          }).OrderByDescending(o => o.DeaprtmentName).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(result);
        }
        public ActionResult AddHoliday(int? Id, string date)
        {
            HolidayViewModel holidaymodel = new HolidayViewModel();
            List<SelectListItem> deptItemList = new List<SelectListItem>();
            try
            {
                var Department = context.Department.Where(x => x.IsDeleted == false).Select(x => new
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
                var dList = _holiday.FindBy(x => x.Date.ToString() == date).Select(x => x.DepartmentId).ToList();
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
                ViewBag.Department = deptItemList.OrderBy(x => x.Text).ToList();

                if (Id == null || Id <= 0)
                {
                    return View(holidaymodel);
                }

                var data = _holiday.FindBy(x => x.Date.ToString() == date).FirstOrDefault();
                if (data == null)
                {
                    TempData["errorMessage"] = "Holiday details not found.";
                    return RedirectToAction("Index");
                }
                string datastring = String.Format("{0:dd/MM/yyyy}", data.Date);
                holidaymodel.Id = data.Id;
                holidaymodel.Year = data.Year;
                //holidaymodel.Date = (data.Date.ToString("dd/MM/yyyy",System.Globalization.CultureInfo.CurrentUICulture));
                holidaymodel.Date = datastring;
                holidaymodel.Remarks = data.Remarks;
                holidaymodel.CreatedBy = data.CreatedBy;
                holidaymodel.CreatedOn = data.CreatedOn;
                holidaymodel.ModifiedBy = data.ModifiedBy;
                holidaymodel.ModifiedOn = data.ModifiedOn;
                holidaymodel.DeletedBy = data.DeletedBy;
                holidaymodel.DeletedOn = data.DeletedOn;
                //holidaymodel.DepartmentIds = data.DepartmentId.ToString();
                holidaymodel.IsDeleted = false;



            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(holidaymodel);
        }
        [HttpPost]
        public ActionResult AddHoliday(HolidayViewModel model)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "somthing went wrong, please try again letter");
            List<int> deptIds = new List<int>();
            List<SelectListItem> deptItemList = new List<SelectListItem>();
            try
            {
                var Department = context.Department.Where(x => x.IsDeleted == false).Select(x => new
                {
                    x.Id,
                    x.Name,
                }).OrderBy(o => o.Name).ToList();

                ViewBag.Department = deptItemList.OrderBy(x => x.Text).ToList();

                if (string.IsNullOrEmpty(model.DepartmentIds))
                {
                    tuple = new Tuple<bool, string>(false, "Please select departemnt.");
                    return Json(tuple, JsonRequestBehavior.AllowGet);
                }
                
                var date = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.Date));
                var Year = date.Year;


                var dept = model.DepartmentIds.Split(',');
                foreach (var item in dept)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                        deptIds.Add(Convert.ToInt32(item));
                };

                using (var context = new admin_csmariseEntities())
                {
                    var Isexit = context.Holiday.Where(x => x.DepartmentId == model.Id && x.Date.ToString() == model.Date).FirstOrDefault();
                    if (Isexit != null)
                    {
                        tuple = new Tuple<bool, string>(false, "Department & Date is already exits");
                        return Json(tuple, JsonRequestBehavior.AllowGet);
                    }

                    if (model.Id > 0)
                    {

                        var holidaydata = context.Holiday.Where(x => x.Id == model.Id).FirstOrDefault();
                        if (holidaydata == null)
                        {
                            tuple = new Tuple<bool, string>(false, "Holiday data not found");
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }                  


                        if (!string.IsNullOrEmpty(model.DepartmentIds))
                        {

                            if (deptIds.Count > 0)
                            {
                                foreach (var d in deptIds)
                                {
                                    Holiday holidaydatas = new Holiday()
                                    {
                                        Id = model.Id,
                                        DepartmentId = d,
                                        Date = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.Date)),
                                        Year = date.Year,
                                        Remarks = model.Remarks,
                                        ModifiedBy = LoginUserId(),
                                        ModifiedOn = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    context.Holiday.Add(holidaydatas);
                                    context.SaveChanges();
                                    TempData["success"] = "Holiday Data Updated";
                                }
                            }

                        }
                    }

                    else
                    {
                        if (!string.IsNullOrEmpty(model.DepartmentIds))
                        {
                            if (deptIds.Count > 0)
                            {
                                foreach (var d in deptIds)
                                {
                                    Holiday holidaydata = new Holiday()
                                    {
                                        Id = model.Id,
                                        DepartmentId = d,
                                        Date = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.Date)),
                                        Year = date.Year,
                                        Remarks = model.Remarks,
                                        CreatedBy = LoginUserId(),
                                        CreatedOn = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    context.Holiday.Add(holidaydata);
                                    context.SaveChanges();
                                    TempData["success"] = "Holiday Data Added";
                                }
                            }
                        }
                    }
                    tuple = new Tuple<bool, string>(true, string.Empty);
                    return Json(tuple, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteHoliday(int Id)
        {
            try
            {
                var holiday = _holiday.FindBy(x => x.Id == Id).FirstOrDefault();
                if (holiday != null)
                {
                    holiday.IsDeleted = true;
                    _holiday.Delete(holiday);
                    _holiday.Save();
                    TempData["success"] = "Holiday Data Deleted";
                }
                else
                {
                    TempData["errorMessage"] = "Holiday record not found.";
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
