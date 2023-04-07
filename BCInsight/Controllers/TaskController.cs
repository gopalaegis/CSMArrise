using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCInsight.DAL;
using BCInsight.BAL.Repository;
using BCInsight.Models;
using Microsoft.Practices.ObjectBuilder2;

namespace BCInsight.Controllers
{
    public class TaskController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        admin_csmariseEntities context = new admin_csmariseEntities();
        ITask _task;
        IDepartment _department;
        ITaskDepartment _taskDepartment;

        public TaskController(ITask task, IDepartment department, ITaskDepartment taskDepartment)
        {
            _task = task;
            _department = department;
            _taskDepartment = taskDepartment;
        }

        // GET: Task
        public ActionResult Index()
        {
            var taskList = new List<TaskViewModel>();
            try
            {
                using (var entity = new admin_csmariseEntities())
                {
                    var list = (from td in entity.TaskDepartment
                                join d in entity.Department on td.DepartmentId equals d.Id
                                join t in entity.Task on td.TaskId equals t.Id
                                where td.IsDeleted == false && d.IsDeleted == false
                                select new
                                {
                                    taskdeptId = td.Id,
                                    taskId = td.TaskId,
                                    taskName = t.TaskName,
                                    deptName = d.Name,
                                    deptId = td.DepartmentId,
                                    taskRemarks = td.Remarks,
                                    isformanager = td.IsForManager ?? false,
                                    d.CreatedBy,
                                    d.CreatedOn
                                }).ToList();

                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            taskList.Add(new TaskViewModel()
                            {
                                TaskId = item.taskId,
                                TaskName = item.taskName,
                                TaskRemarks = item.taskRemarks,
                                CreatedOn = item.CreatedOn,
                                CreatedBy = item.CreatedBy,
                                DeptName = item.deptName,
                                TaskDeptId = item.taskdeptId,
                                IsForManager = item.isformanager
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
            return View(taskList);
        }
        public ActionResult AddTask(int? Id)
        {
            TaskViewModel taskmodel = new TaskViewModel();
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

                if (Id != null && Id > 0)
                {
                    var taskdata = _task.FindBy(x => x.Id == Id).FirstOrDefault();
                    taskmodel.Id = taskdata.Id;
                    taskmodel.TaskName = taskdata.TaskName;

                    var taskDept = _taskDepartment.FindBy(x => x.TaskId == Id && x.IsDeleted == false).FirstOrDefault();
                    taskmodel.TaskRemarks = taskDept.Remarks;
                    taskmodel.IsForManager = taskDept.IsForManager.Value;

                    var dList = _taskDepartment.FindBy(x => x.TaskId == Id && x.IsDeleted == false).Select(x => x.DepartmentId).ToList();
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
                ViewBag.department = deptItemList.OrderBy(x => x.Text).ToList();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error occured please contact your administrator.";
                Log.Error(ex);
            }
            return View(taskmodel);
        }
        [HttpPost]
        public ActionResult AddTask(TaskViewModel model)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "somthing went wrong, please try again letter");
            List<int> deptId = new List<int>();
            bool Isformanager;
            try
            {
                using (var entity = new admin_csmariseEntities())
                {
                    if (model.Id > 0)
                    {
                        var task = _task.FindBy(x => x.Id == model.TaskId && x.IsDeleted == false).FirstOrDefault();
                        if (task != null)
                        {
                            task.Id = model.Id;
                            task.TaskName = model.TaskName;
                            task.ModifiedBy = LoginUserId();
                            task.ModifiedOn = DateTime.Now;

                            _task.Edit(task);
                            _task.Save();

                            var taskid = task.Id;

                            if (!string.IsNullOrEmpty(model.DeptIds))
                            {
                                var dept = model.DeptIds.Split(',');
                                foreach (var item in dept)
                                {
                                    if (!string.IsNullOrWhiteSpace(item))
                                        deptId.Add(Convert.ToInt32(item));
                                };

                                var deptlist = entity.TaskDepartment.Where(x => x.TaskId == task.Id).ToList() ?? new List<TaskDepartment>();

                                if (model.IsForManager == true)
                                {
                                    Isformanager = true;
                                }
                                else
                                {
                                    Isformanager = false;
                                }

                                foreach (var deptid in deptlist)
                                {
                                    entity.TaskDepartment.Remove(deptid);
                                    entity.SaveChanges();
                                }

                                if (deptId.Count > 0)
                                {
                                    foreach (var d in deptId)
                                    {
                                        TaskDepartment taskDept = new TaskDepartment()
                                        {
                                            TaskId = task.Id,
                                            DepartmentId = d,
                                            Remarks = model.TaskRemarks,
                                            CreatedBy = LoginUserId(),
                                            CreatedOn = DateTime.Now,
                                            IsDeleted = false,
                                            IsForManager = Isformanager

                                        };
                                        entity.TaskDepartment.Add(taskDept);
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
                            tuple = new Tuple<bool, string>(false, "Task not found");
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        Task usertask = new Task()
                        {
                            TaskName = model.TaskName,
                            Remarks = model.TaskRemarks,
                            CreatedBy = LoginUserId(),
                            CreatedOn = DateTime.Now,
                            IsDeleted = false,

                        };

                        entity.Task.Add(usertask);
                        var save = entity.SaveChanges();
                        if (save <= 0)
                        {
                            var msg = "Error occured while creating new Task";
                            tuple = new Tuple<bool, string>(false, msg);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }

                        var taskId = usertask.Id;
                        if (!string.IsNullOrEmpty(model.DeptIds))
                        {
                            var dept = model.DeptIds.Split(',');
                            foreach (var item in dept)
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                    deptId.Add(Convert.ToInt32(item));
                            };

                            if (model.IsForManager == true)
                            {
                                Isformanager = true;
                            }
                            else
                            {
                                Isformanager = false;
                            }


                            if (deptId.Count > 0)
                            {
                                foreach (var d in deptId)
                                {
                                    TaskDepartment taskDepartment = new TaskDepartment()
                                    {
                                        TaskId = taskId,
                                        Remarks = model.TaskRemarks,
                                        DepartmentId = d,
                                        CreatedBy = LoginUserId(),
                                        CreatedOn = DateTime.Now,
                                        IsDeleted = false,
                                        IsForManager = Isformanager
                                    };
                                    entity.TaskDepartment.Add(taskDepartment);
                                    entity.SaveChanges();
                                }
                            }
                            TempData["success"] = "Task Added successfully";
                            tuple = new Tuple<bool, string>(true, string.Empty);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }
                        tuple = new Tuple<bool, string>(true, string.Empty);
                        return Json(tuple, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteTask(int Id)
        {
            try
            {
                var taskuser = _taskDepartment.FindBy(x => x.Id == Id).ToList();
                if (taskuser != null)
                {
                    foreach (var item in taskuser)
                    {
                        item.IsDeleted = true;
                        item.DeletedBy = LoginUserId();
                        item.DeletedOn = DateTime.Now;
                        item.ModifiedBy = LoginUserId();
                        item.ModifiedOn = DateTime.Now;

                        _taskDepartment.Edit(item);
                        _taskDepartment.Save();
                    }
                    TempData["success"] = "Record Deleted successfully";
                }
                else
                    TempData["errorMessage"] = "Task record not found.";
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




















