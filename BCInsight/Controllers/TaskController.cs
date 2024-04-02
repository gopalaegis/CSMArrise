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
        IDailytaskstatus _Dailytaskstatus;

        public TaskController(ITask task, IDepartment department, ITaskDepartment taskDepartment, IDailytaskstatus dailytaskstatus)
        {
            _task = task;
            _department = department;
            _taskDepartment = taskDepartment;
            _Dailytaskstatus = dailytaskstatus;
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
                                    CreatedDate = td.CreatedOn,
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
                                CreatedOn = item.CreatedDate,
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

            if (string.IsNullOrEmpty(model.DeptIds))
            {
                tuple = new Tuple<bool, string>(false, "Please select department");
                return Json(tuple, JsonRequestBehavior.AllowGet);
            }

            List<int> deptIds = new List<int>();
            var dept = model.DeptIds.Split(',');
            foreach (var item in dept)
            {
                if (!string.IsNullOrWhiteSpace(item))
                    deptIds.Add(Convert.ToInt32(item));
            };

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

                            var taskDeptList = entity.TaskDepartment.Where(x => x.TaskId == task.Id).ToList();
                            foreach (var item in taskDeptList)
                            {
                                var dailyTask = entity.DailyTaskStatus.Where(x => x.TaskId == item.Id).ToList();
                                if (!deptIds.Contains(item.DepartmentId))
                                {
                                    entity.TaskDepartment.Remove(item);
                                    var isDelete = entity.SaveChanges();
                                    if (isDelete > 0)
                                    {
                                        foreach (var dTask in dailyTask)
                                        {
                                            entity.DailyTaskStatus.Remove(dTask);
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }

                            var taskDepIds = _taskDepartment.FindBy(x => x.TaskId == task.Id).Select(x => x.DepartmentId).ToList();
                            var taskDeps = taskDeptList.Where(x => x.TaskId == task.Id).ToList();

                            var notExistDeptIds = deptIds.Except(taskDepIds).ToList();
                            if (notExistDeptIds.Any())
                            {
                                foreach (var deptId in notExistDeptIds)
                                {
                                    var taskDept = _taskDepartment.FindBy(x => x.TaskId == -1).FirstOrDefault() ?? new TaskDepartment();
                                    taskDept.TaskId = task.Id;
                                    taskDept.DepartmentId = deptId;
                                    taskDept.Remarks = model.TaskRemarks;
                                    taskDept.CreatedBy = LoginUserId();
                                    taskDept.CreatedOn = DateTime.Now;
                                    taskDept.IsDeleted = false;

                                    if (model.IsForManager)
                                        taskDept.IsForManager = true;
                                    else
                                        taskDept.IsForManager = false;

                                    _taskDepartment.Add(taskDept);
                                    _taskDepartment.Save();
                                }
                            }

                            foreach (var td in taskDeps)
                            {
                                if (deptIds.Contains(td.DepartmentId))
                                {
                                    td.TaskId = task.Id;
                                    td.DepartmentId = td.DepartmentId;
                                    td.Remarks = model.TaskRemarks;
                                    td.ModifiedBy = LoginUserId();
                                    td.ModifiedOn = DateTime.Now;
                                    td.IsDeleted = false;

                                    if (model.IsForManager)
                                        td.IsForManager = true;
                                    else
                                        td.IsForManager = false;

                                    _taskDepartment.Edit(td);
                                    _taskDepartment.Save();
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
                        Task task = new Task()
                        {
                            TaskName = model.TaskName,
                            Remarks = model.TaskRemarks,
                            CreatedBy = LoginUserId(),
                            CreatedOn = DateTime.Now,
                            IsDeleted = false
                        };

                        entity.Task.Add(task);
                        var save = entity.SaveChanges();
                        if (save <= 0)
                        {
                            var msg = "Error occured while creating new Task";
                            tuple = new Tuple<bool, string>(false, msg);
                            return Json(tuple, JsonRequestBehavior.AllowGet);
                        }

                        var taskId = task.Id;
                        if (!string.IsNullOrEmpty(model.DeptIds))
                        {
                            if (deptIds.Count > 0)
                            {
                                foreach (var d in deptIds)
                                {
                                    TaskDepartment taskDept = new TaskDepartment()
                                    {
                                        TaskId = taskId,
                                        Remarks = model.TaskRemarks,
                                        DepartmentId = d,
                                        CreatedBy = LoginUserId(),
                                        CreatedOn = DateTime.Now,
                                        IsDeleted = false
                                    };

                                    if (model.IsForManager)
                                        taskDept.IsForManager = true;
                                    else
                                        taskDept.IsForManager = false;

                                    entity.TaskDepartment.Add(taskDept);
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
