using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCInsight.Models;
using BCInsight.DAL;
using BCInsight.BAL.Repository;

namespace BCInsight.Controllers
{
    public class DepartmentController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDepartment _department;

        admin_csmariseEntities context = new admin_csmariseEntities();

        public DepartmentController(IDepartment department)
        {
            _department = department;
        }
        // GET: Department
        public ActionResult Index()
        {
            //List<DepartmentViewModel> deptlist = new List<DepartmentViewModel>();
            var deptlist = new List<DepartmentViewModel>();
            try
            {
                deptlist = (from d in context.Department
                            join ud in context.UserDepartment on d.Id equals ud.DepartmentId into Employees
                            where d.IsDeleted == false
                            select new DepartmentViewModel()
                            {
                                Id = d.Id,
                                Name = d.Name,
                                Remarks = d.Remarks,
                                CreatedBy = d.CreatedBy,
                                CreatedOn = d.CreatedOn,
                                ModifiedBy = d.ModifiedBy,
                                ModifiedOn = d.ModifiedOn,
                                DeletedBy = d.DeletedBy,
                                DeletedOn = d.DeletedOn,
                                Employees = Employees.Count()
                            }).OrderByDescending(x => x.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(deptlist);
        }
        public ActionResult AddDepartment(int? Id)
        {
            DepartmentViewModel deptmodel = new DepartmentViewModel();
            try
            {
                if (Id == null || Id <= 0)
                {
                    return View(deptmodel);
                }
                var data = _department.FindBy(x => x.Id == Id).FirstOrDefault();
                if (data == null)
                {
                    TempData["errorMessage"] = "Record not found. please try again";
                    return RedirectToAction("Index");
                }
                deptmodel.Id = data.Id;
                deptmodel.Name = data.Name;
                deptmodel.Remarks = data.Remarks;
                //deptmodel.StartTime = data.StartTIme;
                //deptmodel.EndTime = data.EndTIme;
                deptmodel.CreatedBy = data.CreatedBy;
                deptmodel.CreatedOn = data.CreatedOn;
                deptmodel.ModifiedBy = data.ModifiedBy;
                deptmodel.ModifiedOn = data.ModifiedOn;
                deptmodel.DeletedBy = data.DeletedBy;
                deptmodel.DeletedOn = data.DeletedOn;
                deptmodel.IsDeleted = false;

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(deptmodel);
        }
        [HttpPost]
        public ActionResult AddDepartment(DepartmentViewModel model)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, string.Empty);
            try
            {
                using (var context = new admin_csmariseEntities())
                {

                    Department dept = _department.FindBy(x => x.Id == model.Id).FirstOrDefault() ?? new Department();

                    var existDept = context.Department.Where(x => x.Name.Replace(" ", "").ToLower() == model.Name.Replace(" ", "").ToLower() && x.Id != model.Id).FirstOrDefault();
                    if (existDept != null)
                    {
                        tuple = new Tuple<bool, string>(false, "Department name is already exits");
                        return Json(tuple, JsonRequestBehavior.AllowGet);

                    }
                    if (model.Id > 0)
                    {
                        var deptdata = context.Department.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefault();
                        if (deptdata == null)
                        {
                            tuple = new Tuple<bool, string>(false, "Record not found.");
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }

                        dept.Name = model.Name;
                        dept.Remarks = model.Remarks;
                        dept.IsDeleted = false;
                        dept.ModifiedBy = LoginUserId();
                        dept.ModifiedOn = DateTime.Now;
                        _department.Edit(dept);
                        _department.Save();
                        TempData["success"] = "Record updated successfully";
                    }
                    else
                    {
                        dept.Name = model.Name;
                        dept.Remarks = model.Remarks;
                        dept.CreatedBy = LoginUserId();
                        dept.CreatedOn = DateTime.Now;
                        _department.Add(dept);
                        _department.Save();
                        TempData["success"] = "Department Added successfully";

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
        public ActionResult DeleteDepartment(int Id)
        {
            try
            {
                var dept = _department.FindBy(x => x.Id == Id).FirstOrDefault();
                if (dept != null)
                {
                    dept.IsDeleted = true;
                    dept.DeletedBy = LoginUserId();
                    dept.DeletedOn = DateTime.Now;
                    _department.Edit(dept);
                    _department.Save();
                    TempData["success"] = "Record Deleted";
                }
                else
                {
                    TempData["errorMessage"] = "Record not found";
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