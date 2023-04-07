using Antlr.Runtime.Misc;
using BCInsight.BAL.Repository;
using BCInsight.Code;
using BCInsight.DAL;
using BCInsight.Models;
using BCInsight.Web.HelperClass;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using Swashbuckle.Swagger;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Http;
using System.Web.WebPages;
using static BCInsight.Web.HelperClass.EnumErrorCodeHelper;

namespace BCInsight.Controllers.API
{
    public class AriseAPIController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IUser _user;
        IAttendance _attendance;
        IClockInRequest _clockInRequest;
        ILeaveRequest _leaveRequest;
        ITaskDepartment _taskDepartment;
        IDailytaskstatus _dailytaskstatus;
        ITask _task;

        public AriseAPIController(IUser user, IAttendance attendance, IClockInRequest clockInRequest, ILeaveRequest leaveRequest, ITaskDepartment taskDepartment, IDailytaskstatus dailytaskstatus, ITask task)
        {
            _user = user;
            _attendance = attendance;
            _clockInRequest = clockInRequest;
            _leaveRequest = leaveRequest;
            _taskDepartment = taskDepartment;
            _dailytaskstatus = dailytaskstatus;
            _task = task;
        }

        [HttpPost]
        [ActionName("SignIn")]
        public ApiResult SignIn(APIReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                using (var entity = new admin_csmariseEntities())
                {
                    var data = entity.User.Where(x => (x.UserName == model.Username || x.Mobile == model.Username) && x.Password == model.Password && x.IsDeleted == false).FirstOrDefault();
                    if (data != null)
                    {
                        var siteList = new List<object>();
                        var sites = (from su in entity.SiteUser
                                     join s in entity.Site on su.SiteId equals s.Id
                                     where su.UserId == data.Id && su.IsDeleted == false && s.IsDeleted == false
                                     select new
                                     {
                                         s.Id,
                                         s.Name,
                                         s.Latitude,
                                         s.Longitude
                                     }).ToList();

                        if (sites != null)
                        {
                            foreach (var item in sites)
                            {
                                siteList.Add(new
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Latitude = item.Latitude,
                                    Longitude = item.Longitude
                                });
                            }
                        }

                        var deptList = new List<object>();
                        var dept = (from d in entity.Department
                                    join ud in entity.UserDepartment on d.Id equals ud.DepartmentId
                                    where ud.UserId == data.Id
                                    select new
                                    {
                                        d.Id,
                                        d.Name,
                                        d.Remarks
                                    }).ToList();

                        if (dept != null)
                        {
                            foreach (var item in dept)
                            {

                                deptList.Add(new
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Remarks = item.Remarks

                                });
                            }
                        }

                        var _retModel = new
                        {
                            UserId = data.Id,
                            UserName = data.UserName,
                            Name = data.Name,
                            Email = data.Email,
                            Mobile = data.Mobile,
                            Salary = data.Salary,
                            SalaryDuration = data.SalaryDuration,
                            Remarks = data.Remarks,
                            PushNotificationId = data.PushNotificationId,
                            DeviceId = data.DeviceId,
                            ParentId = data.ParentId,
                            Role = data.Role,
                            StartTime = data.StartTime,
                            EndTime = data.EndTime,
                            WeekOffDay = data.WeekOffDay,
                            IsDeleted = data.IsDeleted,
                            CreatedBy = data.CreatedBy,
                            CreatedOn = data.CreatedOn == null ? string.Empty : data.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            ModifiedBy = data.ModifiedBy,
                            ModifiedOn = data.ModifiedOn == null ? string.Empty : data.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            DeletedBy = data.DeletedBy,
                            DeletedOn = data.DeletedOn == null ? string.Empty : data.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            SiteData = siteList,
                            DeptData = deptList
                        };

                        if (_retModel.UserId > 0)
                        {
                            _apiResult.Response = true;
                            _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.LoginSuccess;
                            _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.LoginSuccess);
                            _apiResult.Result = _retModel;
                        }
                        else
                        {
                            _apiResult.Response = false;
                            _apiResult.ReturnCode = (int)EnumErrorCodeHelper.ErrorCodes.LoginFail;
                            _apiResult.Message = ErrorHelpers.GetErrorMessage(EnumErrorCodeHelper.ErrorCodes.LoginFail);
                            _apiResult.Result = null;
                        }
                    }
                    else
                    {
                        _apiResult.Response = false;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.ErrorCodes.LoginFail;
                        _apiResult.Message = ErrorHelpers.GetErrorMessage(EnumErrorCodeHelper.ErrorCodes.LoginFail);
                        _apiResult.Result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("ForgotPassword")]
        public ApiResult ForgotPassword(ForgotPasswordReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }
                var newpassword = Helper.CreateRandomPassword(8);
                User user = _user.FindBy(m => m.Email == model.Email && m.IsDeleted == false).FirstOrDefault();
                if (user != null && user.Id > 0)
                {
                    user.Password = newpassword;
                    _user.Edit(user);
                    _user.Save();

                    string emailSubject = string.Empty, emailMessage = string.Empty;
                    emailMessage = SendEmail.SendTemplateEmail(EmailMessageType.forgotpassword, out emailSubject, user.Name, newpassword, string.Empty);
                    SendEmail.SendMail(emailSubject, emailMessage, user.Email, "");

                    _apiResult.Response = true;
                    _apiResult.ReturnCode = (int)SuccessCodes.MAIL_SEND_SUCCESS;
                    _apiResult.Message = ErrorHelpers.GetSuccessMessages(SuccessCodes.MAIL_SEND_SUCCESS);
                }
                else
                {
                    _apiResult.Response = false;
                    _apiResult.ReturnCode = (int)ErrorCodes.USERNAME_NOTFOUND;
                    _apiResult.Message = ErrorHelpers.GetErrorMessage(ErrorCodes.USERNAME_NOTFOUND);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("UpdateCheckTime")]
        public ApiResult UpdateCheckTime(UpdateCheckTimeReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                DateTime checkTime;
                if (DateTime.TryParse(model.CheckTime.Trim(), out DateTime Temp) == true)
                {
                    checkTime = DateTime.ParseExact(model.CheckTime.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    if (checkTime == null || checkTime > DateTime.Now)
                    {
                        return Utility.InvalidModelMessage("Invalid Check DateTime");
                    }
                }
                else
                {
                    return Utility.InvalidModelMessage("Invalid Check DateTime");
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                var data = _attendance.FindBy(x => x.UserId == model.UserId && x.Date == checkTime.Date && x.IsDeleted == false).FirstOrDefault();
                if (data != null)
                {
                    data.Date = checkTime.Date;
                    if (string.Equals(model.CheckType.Trim().Replace(" ", ""), "checkin", StringComparison.CurrentCultureIgnoreCase))
                        data.ClockIn = checkTime;

                    if (string.Equals(model.CheckType.Trim().Replace(" ", ""), "checkout", StringComparison.CurrentCultureIgnoreCase))
                        data.ClockOut = checkTime;

                    data.Date = checkTime.Date;
                    data.ModifiedOn = DateTime.Now;

                    _attendance.Edit(data);
                    _attendance.Save();
                }
                else
                {
                    data = new Attendance();
                    if (string.Equals(model.CheckType.Trim().Replace(" ", ""), "checkout", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Utility.InvalidModelMessage("Please do Check In first");
                    }
                    if (string.Equals(model.CheckType.Trim().Replace(" ", ""), "checkin", StringComparison.CurrentCultureIgnoreCase))
                        data.ClockIn = checkTime;

                    if (string.Equals(model.CheckType.Trim().Replace(" ", ""), "checkout", StringComparison.CurrentCultureIgnoreCase))
                        data.ClockOut = checkTime;

                    data.UserId = model.UserId;
                    data.Date = checkTime.Date;
                    data.CreatedOn = DateTime.Now;
                    data.IsDeleted = false;

                    _attendance.Add(data);
                    _attendance.Save();
                }

                var _retModel = new
                {
                    Id = data.Id,
                    UserId = data.UserId,
                    Date = data.Date.ToString("yyyy-MM-ddT00:00:00"),
                    ClockIn = data.ClockIn == null ? string.Empty : data.ClockIn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ClockOut = data.ClockOut == null ? string.Empty : data.ClockOut.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Remarks = data.Remarks,
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn == null ? string.Empty : data.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ModifiedBy = data.ModifiedBy,
                    ModifiedOn = data.ModifiedOn == null ? string.Empty : data.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    IsDeleted = data.IsDeleted,
                    DeletedBy = data.DeletedBy,
                    DeletedOn = data.DeletedOn == null ? string.Empty : data.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    CheckType = data.CheckType,
                    Description = data.Description,
                    IsMannualReq = data.IsMannualReq,
                    IsApprove = data.IsApprove,
                    Reason = data.Reason
                };

                _apiResult.Response = true;
                _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.RecordUpdated;
                _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.RecordUpdated);
                _apiResult.Result = _retModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetAttendanceDetails")]
        public ApiResult GetAttendanceDetail(AttendanceDetailReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            List<PresentAbsentModel> presentAbsentDays = new List<PresentAbsentModel>();
            AttendanceDetailModel _retModel = new AttendanceDetailModel();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                using (var entity = new admin_csmariseEntities())
                {
                    DateTime startDate = new DateTime(model.Year, model.Month, 1), endDate = startDate.AddMonths(1).Date.AddSeconds(-1);
                    int crntYear = model.Year, crntMonth = model.Month;

                    double fulldays = 0, halfDays = 0, absentDays = 0;
                    double totalDays = (endDate.Date - startDate.Date).TotalDays + 1;
                    double totalWorkingHours = 0;

                    var monthDates = Enumerable.Range(startDate.Day, endDate.Day).Select(d => new DateTime(crntYear, crntMonth, d).Date).ToList();
                    List<DateTime> presentDays = new List<DateTime>();

                    double totalLeaves = 0;
                    var holidayList = new List<Holiday>();
                    var holidayDates = new List<DateTime>();
                    var deptList = new List<Department>();
                    var userDept = new List<int>();

                    var totalApprovedLeave = 0;
                    List<DateTime> approvedLeaveDates = new List<DateTime>();
                    var userLeaves = new List<LeaveRequest>();

                    double baseSalary = user.Salary == null ? 0 : user.Salary.Value;
                    double perDaySalary = baseSalary / DateTime.DaysInMonth(crntYear, crntMonth);
                    double presentDaysSalary = 0;

                    var attendanceList = entity.Attendance.Where(x => x.UserId == model.UserId && x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false).ToList();

                    if (attendanceList != null && attendanceList.Count > 0)
                    {
                        deptList = entity.Department.Where(x => x.IsDeleted == false).ToList();
                        userDept = entity.UserDepartment.Where(x => x.UserId == model.UserId && x.IsDeleted == false).Select(x => x.DepartmentId).ToList();
                        if (userDept != null && userDept.Count > 0)
                        {
                            foreach (var deptId in userDept)
                            {
                                totalLeaves += deptList.Where(x => x.Id == deptId).Select(x => x.NoofLeave).FirstOrDefault() ?? 0;

                                var holidays = entity.Holiday.Where(x => x.DepartmentId == deptId && x.Date.Year == crntYear && x.Date.Month == crntMonth && x.IsDeleted == false).ToList();
                                foreach (var item in holidays)
                                {
                                    holidayList.Add(item);
                                    holidayDates.Add(item.Date);
                                }
                            }
                        }

                        userLeaves = entity.LeaveRequest.Where(x => x.UserId == model.UserId && x.FromDate >= startDate && x.ToDate <= endDate &&
                                                                (x.ApproveDays != null || x.ApproveDays > 0) && x.IsDeleted == false).ToList();
                        if (userLeaves != null && userLeaves.Count > 0)
                        {
                            totalApprovedLeave = userLeaves.Select(x => x.ApproveDays).Sum() ?? 0;

                            foreach (var item in userLeaves)
                                approvedLeaveDates.Add(item.FromDate);
                        }

                        foreach (var item in monthDates)
                        {
                            if (item >= DateTime.Now.Date)
                                continue;

                            var data = attendanceList.Where(x => x.Date == item).FirstOrDefault();
                            if (data != null)
                            {
                                if (data.ClockIn != null && data.ClockOut != null)
                                {
                                    var hours = Math.Abs((data.ClockOut.Value - data.ClockIn.Value).TotalHours);
                                    if (hours > 0)
                                    {
                                        if (hours > 4)
                                        {
                                            presentAbsentDays.Add(new PresentAbsentModel()
                                            {
                                                type = "fullday",
                                                date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                                message = string.Empty,
                                                deduction = "0"
                                            });
                                            fulldays++;
                                        }
                                        else
                                        {
                                            var msg = "CheckIn: " + data.ClockIn.Value.ToString("yyyy-MM-ddTHH:mm:ss") + " | CheckOut: " + data.ClockOut.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                                            presentAbsentDays.Add(new PresentAbsentModel()
                                            {
                                                type = "halfday",
                                                date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                                message = msg,
                                                deduction = $"{Math.Round((perDaySalary / 2), 2)}"
                                            });
                                            halfDays++;
                                        }
                                        presentDays.Add(item.Date);
                                        totalWorkingHours += hours;
                                    }
                                    else
                                    {
                                        absentDays++;
                                        presentAbsentDays.Add(new PresentAbsentModel()
                                        {
                                            type = "absent",
                                            date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                            message = "Invalid CheckIn or CheckOut Time.",
                                            deduction = $"{Math.Round(perDaySalary, 2)}"
                                        });
                                    }
                                }
                                else
                                {
                                    absentDays++;
                                    presentAbsentDays.Add(new PresentAbsentModel()
                                    {
                                        type = "absent",
                                        date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                        message = "CheckIn or CheckOut Time is null.",
                                        deduction = $"{Math.Round(perDaySalary, 2)}"
                                    });
                                }
                            }
                            else
                            {
                                if (totalApprovedLeave == totalLeaves)
                                {
                                    if (approvedLeaveDates.Contains(item))
                                    {
                                        fulldays++;
                                        presentAbsentDays.Add(new PresentAbsentModel()
                                        {
                                            type = "onleave",
                                            date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                            message = "onleave",
                                            deduction = "0"
                                        });
                                    }
                                    else
                                    {
                                        absentDays++;
                                        presentAbsentDays.Add(new PresentAbsentModel()
                                        {
                                            type = "absent",
                                            date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                            message = "absent without Approval.",
                                            deduction = $"{Math.Round(perDaySalary, 2)}"
                                        });
                                    }
                                }
                                if ((totalApprovedLeave <= totalLeaves) && (totalApprovedLeave != totalLeaves))
                                {
                                    if (approvedLeaveDates.Contains(item))
                                    {
                                        fulldays++;
                                        presentAbsentDays.Add(new PresentAbsentModel()
                                        {
                                            type = "onleave",
                                            date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                            message = "onleave",
                                            deduction = "0"
                                        });
                                    }
                                    else
                                    {
                                        absentDays++;
                                        presentAbsentDays.Add(new PresentAbsentModel()
                                        {
                                            type = "absent",
                                            date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                            message = "absent without Approval.",
                                            deduction = $"{Math.Round(perDaySalary, 2)}"
                                        });
                                    }
                                }
                                if (totalApprovedLeave > totalLeaves)
                                {
                                    absentDays++;
                                    presentAbsentDays.Add(new PresentAbsentModel()
                                    {
                                        type = "absent",
                                        date = item.Date.ToString("yyyy-MM-ddT00:00:00"),
                                        message = "absent without Approval.",
                                        deduction = $"{Math.Round(perDaySalary, 2)}"
                                    });
                                }
                            }
                        }

                        List<AdvancedPayment> adPayment = new List<AdvancedPayment>();
                        double totalAdvncPayment = 0;
                        var advancePayment = entity.AdvancePaymentRequest.Where(x => x.UserId == model.UserId && x.ApprovedOn != null && x.ApprovedAmount != null && x.IsDeleted == false &&
                                                                                    x.ApprovedOn.Value.Year == crntYear &&
                                                                                    x.ApprovedOn.Value.Month == crntMonth &&
                                                                                    x.ApprovedOn.Value.Day <= endDate.Day).OrderByDescending(x => x.CreatedOn).ToList();
                        if (advancePayment != null && advancePayment.Count > 0)
                        {
                            foreach (var item in advancePayment)
                            {
                                totalAdvncPayment += item.ApprovedAmount.Value;
                                adPayment.Add(new AdvancedPayment()
                                {
                                    Date = item.ApprovedOn == null ? string.Empty : item.ApprovedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                                    Amount = $"{Math.Round(item.ApprovedAmount.Value, 2)}"
                                });
                            }
                        }

                        if (absentDays > totalLeaves)
                        {
                            absentDays = absentDays - totalLeaves;
                        }

                        //double totalDeduction = (perDaySalary * absentDays) + totalAdvncPayment;
                        //double baseSalary1 = baseSalary - totalDeduction;

                        double totalHalfDaySalary = halfDays * (perDaySalary / 2);
                        double totalFullDaySalary = fulldays * perDaySalary;

                        presentDaysSalary = (totalFullDaySalary + totalHalfDaySalary);
                        double absentDaysSalary = baseSalary - presentDaysSalary;

                        _retModel.UserId = model.UserId;
                        _retModel.BaseSalary = baseSalary.ToString();
                        _retModel.onHandSalary = Math.Round(baseSalary - absentDaysSalary - totalAdvncPayment, 2).ToString();
                        _retModel.SalaryFrom = startDate.ToString("yyyy-MM-ddT00:00:00");
                        _retModel.SalaryTo = startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-ddT00:00:00");
                        _retModel.totalAdvancedPayment = totalAdvncPayment;
                        _retModel.advancedPayment = adPayment;
                        _retModel.presentAbsentModel = presentAbsentDays.OrderByDescending(o => o.date).ToList();

                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                        _apiResult.Result = _retModel;
                    }
                    else
                    {
                        return Utility.InvalidModelMessage($"User Attendance not found for the date {startDate.ToString("yyyy-MM-ddTHH:mm:ss")} to {endDate.ToString("yyyy-MM-ddTHH:mm:ss")}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetUserAttendanceDetail")]
        public ApiResult GetUserAttendanceDetail(UserIdReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }
                var today = DateTime.Now.Date;
                var attendance = _attendance.FindBy(x => x.UserId == model.UserId && x.Date == today).FirstOrDefault();
                if (attendance != null)
                {
                    var _retModel = new
                    {
                        Id = attendance.Id,
                        UserId = attendance.UserId,
                        Date = attendance.Date.ToString("yyyy-MM-ddT00:00:00"),
                        ClockIn = attendance.ClockIn == null ? string.Empty : attendance.ClockIn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                        ClockOut = attendance.ClockOut == null ? string.Empty : attendance.ClockOut.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Remarks = attendance.Remarks,
                        CreatedBy = attendance.CreatedBy,
                        CreatedOn = attendance.CreatedOn == null ? string.Empty : attendance.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                        ModifiedBy = attendance.ModifiedBy,
                        ModifiedOn = attendance.ModifiedOn == null ? string.Empty : attendance.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                        IsDeleted = attendance.IsDeleted,
                        DeletedBy = attendance.DeletedBy,
                        DeletedOn = attendance.DeletedOn == null ? string.Empty : attendance.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                        CheckType = attendance.CheckType,
                        Description = attendance.Description,
                        IsMannualReq = attendance.IsMannualReq,
                        IsApprove = attendance.IsApprove,
                        Reason = attendance.Reason
                    };
                    _apiResult.Response = true;
                    _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                    _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                    _apiResult.Result = _retModel;
                }
                else
                {
                    return Utility.ResponseSucess();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("AdvancePayRequest")]
        public ApiResult AdvancePayRequest(AdvancePayRequest model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                using (var entity = new admin_csmariseEntities())
                {
                    AdvancePaymentRequest paymentRequest = new AdvancePaymentRequest();
                    paymentRequest.UserId = model.UserId;
                    paymentRequest.RequestAmount = model.Amount;
                    paymentRequest.Reason = model.Reason;
                    paymentRequest.CreatedOn = DateTime.Now;
                    paymentRequest.CreatedBy = model.UserId;
                    paymentRequest.IsApprove = false;
                    paymentRequest.IsDeleted = false;

                    entity.AdvancePaymentRequest.Add(paymentRequest);
                    var save = entity.SaveChanges();
                    if (save > 0)
                    {
                        var _retModel = new
                        {
                            UserId = paymentRequest.UserId,
                            RequestAmount = paymentRequest.RequestAmount,
                            Reason = paymentRequest.Reason,
                            CreatedOn = paymentRequest.CreatedOn == null ? string.Empty : paymentRequest.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            CreatedBy = paymentRequest.CreatedBy,
                            IsApprove = paymentRequest.IsApprove,
                            IsDeleted = paymentRequest.IsDeleted
                        };
                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                        _apiResult.Result = _retModel;
                    }
                    else
                        return Utility.ResponseFail();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetManualAttendenceReasons")]
        public ApiResult GetManualAttendenceReasons(UserIdReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                List<string> reasons = new List<string>()
                {
                    AttendenceReasons.Reason1.ToString(),
                    AttendenceReasons.Reason2.ToString(),
                    AttendenceReasons.Reason3.ToString(),
                    AttendenceReasons.Reason4.ToString(),
                    AttendenceReasons.Reason5.ToString(),
                };

                _apiResult.Response = true;
                _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                _apiResult.Result = reasons;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("ManualAttendenceRequest")]
        public ApiResult ManualAttendenceRequest(ManualAttendenceReqModel model)
        {
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                DateTime checkDate;
                if (DateTime.TryParse(model.Date.Trim(), out DateTime Temp) == true)
                {
                    checkDate = DateTime.ParseExact(model.Date.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).Date;
                    if (checkDate == null || checkDate > DateTime.Now)
                    {
                        return Utility.InvalidModelMessage("Invalid Date");
                    }
                }
                else
                {
                    return Utility.InvalidModelMessage("Invalid Date");
                }

                //var atdnc = _attendance.FindBy(x => x.UserId == model.UserId && x.Date == checkDate && x.IsDeleted == false).FirstOrDefault();
                var request = _clockInRequest.FindBy(x => x.UserId == model.UserId && x.Date == checkDate && x.IsDeleted == false).FirstOrDefault();

                if (request != null)
                    return Utility.InvalidModelMessage($"Attendance request for Date: {checkDate.ToString("yyyy-MM-dd")} already submitted.");

                request = new ClockInRequest();
                request.UserId = model.UserId;
                request.Date = checkDate.Date;
                request.Reason = model.Reason;
                request.Description = model.Description;
                request.Approved = false;
                request.IsDeleted = false;
                request.CreatedOn = DateTime.Now;
                request.CreatedBy = model.UserId;

                if (request.Id > 0)
                    _clockInRequest.Edit(request);
                else
                    _clockInRequest.Add(request);
                _clockInRequest.Save();
                return Utility.ResponseSucess();

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
        }

        [HttpPost]
        [ActionName("GetAdvancedPaymentByUserId")]
        public ApiResult GetUserAdvancedPayment(UserIdReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                using (var entity = new admin_csmariseEntities())
                {
                    var adPay = entity.AdvancePaymentRequest.Where(x => x.UserId == model.UserId).OrderByDescending(x => x.CreatedOn).ToList() ?? new List<AdvancePaymentRequest>();
                    if (adPay.Count > 0)
                    {
                        var _retList = new List<object>();
                        foreach (var item in adPay)
                        {
                            _retList.Add(new
                            {
                                Id = item.Id,
                                UserId = item.UserId,
                                RequestAmount = item.RequestAmount,
                                Reason = item.Reason,
                                ApprovedAmount = item.ApprovedAmount,
                                ApprovedBy = item.ApprovedBy,
                                ApprovedOn = item.ApprovedOn == null ? string.Empty : item.ApprovedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                                Remarks = item.Remarks,
                                CreatedBy = item.CreatedBy,
                                CreatedOn = item.CreatedOn == null ? string.Empty : item.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                                ModifiedBy = item.ModifiedBy,
                                ModifiedOn = item.ModifiedOn == null ? string.Empty : item.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                                IsDeleted = item.IsDeleted,
                                DeletedBy = item.DeletedBy,
                                DeletedOn = item.DeletedOn == null ? string.Empty : item.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                                IsApprove = item.IsApprove
                            });
                        }

                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                        _apiResult.Result = _retList;
                    }
                    else
                    {
                        _apiResult.Response = false;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.ErrorCodes.Fail;
                        _apiResult.Message = "data not found";
                        _apiResult.Result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetUserDetail")]
        public ApiResult GetUserDetail(UserIdReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }
                else
                {
                    using (var entity = new admin_csmariseEntities())
                    {
                        List<SiteViewModel> siteList = new List<SiteViewModel>();
                        var sites = (from su in entity.SiteUser
                                     join s in entity.Site on su.SiteId equals s.Id
                                     where su.UserId == model.UserId && su.IsDeleted == false && s.IsDeleted == false
                                     select new
                                     {
                                         s.Id,
                                         s.Name,
                                         s.Latitude,
                                         s.Longitude
                                     }).ToList();

                        if (sites != null)
                        {
                            foreach (var item in sites)
                            {
                                siteList.Add(new SiteViewModel()
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Latitude = item.Latitude,
                                    Longitude = item.Longitude
                                });
                            }
                        }

                        var deptList = new List<object>();
                        var dept = (from d in entity.Department
                                    join ud in entity.UserDepartment on d.Id equals ud.DepartmentId
                                    where ud.UserId == user.Id
                                    select new
                                    {
                                        d.Id,
                                        d.Name,
                                        d.Remarks
                                    }).ToList();
                        if (dept != null)
                        {
                            foreach (var item in dept)
                            {
                                deptList.Add(new
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Remarks = item.Remarks
                                });
                            }
                        }

                        var userViewdata = new
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            Email = user.Email,
                            Mobile = user.Mobile,
                            Salary = user.Salary,
                            SalaryDuration = user.SalaryDuration,
                            Remarks = user.Remarks,
                            PushNotificationId = user.PushNotificationId,
                            DeviceId = user.DeviceId,
                            ParentId = user.ParentId,
                            Role = user.Role,
                            StartTime = user.StartTime,
                            EndTime = user.EndTime,
                            WeekOffDay = user.WeekOffDay,
                            IsDeleted = user.IsDeleted,
                            CreatedBy = user.CreatedBy,
                            CreatedOn = user.CreatedOn == null ? string.Empty : user.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            ModifiedBy = user.ModifiedBy,
                            ModifiedOn = user.ModifiedOn == null ? string.Empty : user.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            DeletedBy = user.DeletedBy,
                            DeletedOn = user.DeletedOn == null ? string.Empty : user.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                            SiteData = siteList,
                            DeptData = deptList
                        };

                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                        _apiResult.Result = userViewdata;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("LeaveRequest")]
        public ApiResult LeaveRequest(LeaveRequestReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                DateTime fromDate, toDate;
                if (DateTime.TryParse(model.FromDate.Trim(), out fromDate) == true)
                {
                    fromDate = DateTime.ParseExact(model.FromDate.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    if (fromDate == null || fromDate.Date < DateTime.Now.Date)
                    {
                        return Utility.InvalidModelMessage("Invalid Date");
                    }
                }
                else
                {
                    return Utility.InvalidModelMessage("Invalid Date format");
                }

                if (DateTime.TryParse(model.FromDate.Trim(), out toDate) == true)
                {
                    toDate = DateTime.ParseExact(model.ToDate.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    if (toDate == null || toDate.Date < fromDate.Date)
                    {
                        return Utility.InvalidModelMessage("ToDate must be grater or equals to FromDate");
                    }
                }
                else
                {
                    return Utility.InvalidModelMessage("Invalid Date format");
                }

                double days = (toDate.Date - fromDate.Date).TotalDays + 1;

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                var leave = _leaveRequest.FindBy(x => x.UserId == model.UserId && x.FromDate == fromDate && x.ToDate == toDate).FirstOrDefault();
                if (leave == null)
                {
                    leave = new LeaveRequest();
                    leave.UserId = model.UserId;
                    leave.FromDate = fromDate;
                    leave.ToDate = toDate;
                    leave.NoOfDays = Convert.ToInt32(days);
                    leave.Reason = model.Reason;
                    leave.CreatedBy = model.UserId;
                    leave.CreatedOn = DateTime.Now;
                    leave.IsDeleted = false;
                    leave.IsApproved = null;

                    _leaveRequest.Add(leave);
                    _leaveRequest.Save();

                    _apiResult = Utility.ResponseSucess();
                }
                else
                {
                    return Utility.InvalidModelMessage("Leave request already submitted");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetUserLeaveRequest")]
        public ApiResult GetUserLeaveRequest(UserLeavAndSalaryeRequestReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                var leaves = new List<LeaveRequest>();
                if (model.Year == 0 && model.Month == 0)
                {
                    leaves = _leaveRequest.FindBy(x => x.UserId == model.UserId).OrderByDescending(x => x.FromDate).ToList();
                }
                else
                {
                    var startDate = new DateTime(model.Year, model.Month, 1, 00, 00, 59);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                    leaves = _leaveRequest.FindBy(x => x.UserId == model.UserId && x.FromDate >= startDate && x.ToDate <= endDate).OrderByDescending(x => x.FromDate).ToList();
                }

                if (leaves != null)
                {
                    List<object> _retList = new List<object>();
                    foreach (var item in leaves)
                    {
                        var status = string.Empty;
                        if (item.IsApproved == null)
                            status = "Pending";
                        if (item.IsApproved != null && item.IsApproved.Value)
                            status = "Approve";
                        if (item.IsApproved != null && !item.IsApproved.Value)
                            status = "Reject";

                        _retList.Add(new
                        {
                            item.UserId,
                            FromDate = item.FromDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            ToDate = item.ToDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            item.Reason,
                            item.NoOfDays,
                            item.ApproveDays,
                            item.Remarks,
                            Status = status
                        });
                    }
                    _apiResult.Response = true;
                    _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                    _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                    _apiResult.Result = _retList;
                }
                else
                    return Utility.ResponseSucess();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetUserSalary")]
        public ApiResult GetUserSalary(UserLeavAndSalaryeRequestReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            object obj = null;
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }
                if (user.StartTime != null)
                    user.StartTime = user.StartTime.Value.Add(TimeSpan.FromMinutes(30));
                if (user.EndTime != null)
                    user.EndTime = user.EndTime.Value.Subtract(TimeSpan.FromMinutes(30));

                var startTime = user.StartTime;
                var endTime = user.EndTime;
                var weekOffDay = user.WeekOffDay;

                int crntYear = model.Year, crntMonth = model.Month;

                var startDate = new DateTime(model.Year, model.Month, 1, 00, 00, 59);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

                double fulldays = 0, halfDays = 0, absentDays = 0;
                double totalDays = (endDate.Date - startDate.Date).TotalDays + 1;
                double totalWorkingHours = 0;

                var monthDates = Enumerable.Range(startDate.Day, endDate.Day).Select(d => new DateTime(crntYear, crntMonth, d).Date).ToList();
                var holidayDates = new List<DateTime>();
                var deptList = new List<Department>();
                var userDept = new List<int>();

                var totalApprovedLeave = 0;
                List<DateTime> approvedLeaveDates = new List<DateTime>();
                var userLeaves = new List<LeaveRequest>();

                double baseSalary = user.Salary == null ? 0 : user.Salary.Value;
                double perDaySalary = Math.Round(baseSalary / DateTime.DaysInMonth(crntYear, crntMonth), 2);

                using (var entity = new admin_csmariseEntities())
                {
                    deptList = entity.Department.Where(x => x.IsDeleted == false).ToList();
                    userDept = entity.UserDepartment.Where(x => x.UserId == model.UserId && x.IsDeleted == false).Select(x => x.DepartmentId).ToList();
                    if (userDept != null && userDept.Count > 0)
                    {
                        foreach (var deptId in userDept)
                        {
                            var holidays = entity.Holiday.Where(x => x.DepartmentId == deptId && x.Date.Year == crntYear && x.Date.Month == crntMonth && x.IsDeleted == false).ToList();
                            foreach (var item in holidays)
                            {
                                holidayDates.Add(item.Date);
                            }
                        }
                    }

                    userLeaves = entity.LeaveRequest.Where(x => x.UserId == model.UserId && x.FromDate >= startDate && x.ToDate <= endDate &&
                                                            (x.ApproveDays != null || x.ApproveDays > 0) && x.IsDeleted == false).ToList();
                    if (userLeaves != null && userLeaves.Count > 0)
                    {
                        totalApprovedLeave = userLeaves.Select(x => x.ApproveDays).Sum() ?? 0;

                        foreach (var item in userLeaves)
                            approvedLeaveDates.Add(item.FromDate.Date);
                    }

                    double totalAdvncPayment = 0;
                    var advancePayment = entity.AdvancePaymentRequest.Where(x => x.UserId == model.UserId && x.ApprovedOn != null && x.ApprovedAmount != null && x.IsDeleted == false &&
                                                                                x.ApprovedOn.Value.Year == crntYear &&
                                                                                x.ApprovedOn.Value.Month == crntMonth &&
                                                                                x.ApprovedOn.Value.Day <= endDate.Day).OrderByDescending(x => x.CreatedOn).ToList();
                    if (advancePayment != null && advancePayment.Count > 0)
                    {
                        foreach (var item in advancePayment)
                        {
                            totalAdvncPayment += item.ApprovedAmount.Value;
                        }
                    }

                    double lateHours = 0, minHours = 8;
                    if (startTime != null && endTime != null)
                    {
                        minHours = (endTime.Value - startTime.Value).TotalHours;
                    }

                    var attendanceList = entity.Attendance.Where(x => x.UserId == model.UserId && x.Date >= startDate.Date && x.Date <= endDate.Date && x.IsDeleted == false).ToList();
                    if (attendanceList != null && attendanceList.Count > 0)
                    {
                        foreach (var item in monthDates)
                        {
                            if (item >= DateTime.Now.Date || item.DayOfWeek.ToString().ToLower() == weekOffDay.ToString().ToLower() || holidayDates.Contains(item))
                            {
                                totalWorkingHours += minHours;
                                continue;
                            }

                            var data = attendanceList.Where(x => x.Date == item).FirstOrDefault();
                            if (data != null)
                            {
                                if (data.ClockIn != null && data.ClockOut != null)
                                {
                                    var hours = Math.Round((data.ClockOut.Value - data.ClockIn.Value).TotalHours, 2);
                                    if (hours > 0)
                                    {
                                        if (hours > minHours)
                                            totalWorkingHours += minHours;
                                        else
                                            totalWorkingHours += hours;

                                        if (startTime != null && data.ClockIn.Value.TimeOfDay > startTime)
                                        {
                                            var timeDiff = (data.ClockIn.Value.TimeOfDay.Subtract(startTime.Value));
                                            if (timeDiff < TimeSpan.FromHours(1))
                                                lateHours = lateHours + 1;
                                            if (timeDiff > TimeSpan.FromHours(1) && timeDiff < TimeSpan.FromHours(2))
                                                lateHours = lateHours + 2;
                                            if (timeDiff > TimeSpan.FromHours(2) && timeDiff < TimeSpan.FromHours(3))
                                                lateHours = lateHours + 3;
                                            if (timeDiff > TimeSpan.FromHours(3) && timeDiff < TimeSpan.FromHours(4))
                                                lateHours = lateHours + 4;
                                            if (timeDiff > TimeSpan.FromHours(4) && timeDiff < TimeSpan.FromHours(5))
                                                lateHours = lateHours + 5;
                                            if (timeDiff > TimeSpan.FromHours(5) && timeDiff < TimeSpan.FromHours(6))
                                                lateHours = lateHours + 6;
                                            if (timeDiff > TimeSpan.FromHours(6) && timeDiff < TimeSpan.FromHours(7))
                                                lateHours = lateHours + 7;
                                            if (timeDiff > TimeSpan.FromHours(7) && timeDiff < TimeSpan.FromHours(8))
                                                lateHours = lateHours + 8;
                                        }

                                        if (endTime != null && data.ClockOut.Value.TimeOfDay < endTime)
                                        {
                                            var timeDiff = (endTime.Value.Subtract(data.ClockOut.Value.TimeOfDay));
                                            if (timeDiff < TimeSpan.FromHours(1))
                                                lateHours = lateHours + 1;
                                            if (timeDiff > TimeSpan.FromHours(1) && timeDiff < TimeSpan.FromHours(2))
                                                lateHours = lateHours + 2;
                                            if (timeDiff > TimeSpan.FromHours(2) && timeDiff < TimeSpan.FromHours(3))
                                                lateHours = lateHours + 3;
                                            if (timeDiff > TimeSpan.FromHours(3) && timeDiff < TimeSpan.FromHours(4))
                                                lateHours = lateHours + 4;
                                            if (timeDiff > TimeSpan.FromHours(4) && timeDiff < TimeSpan.FromHours(5))
                                                lateHours = lateHours + 5;
                                            if (timeDiff > TimeSpan.FromHours(5) && timeDiff < TimeSpan.FromHours(6))
                                                lateHours = lateHours + 6;
                                            if (timeDiff > TimeSpan.FromHours(6) && timeDiff < TimeSpan.FromHours(7))
                                                lateHours = lateHours + 7;
                                            if (timeDiff > TimeSpan.FromHours(7) && timeDiff < TimeSpan.FromHours(8))
                                                lateHours = lateHours + 8;
                                        }
                                    }
                                    else
                                        absentDays++;
                                }
                                else
                                    absentDays++;
                            }
                            else
                                absentDays++;
                        }

                        if (absentDays >= totalApprovedLeave)
                            absentDays = absentDays - totalApprovedLeave;

                        double AbsentDaysDeduction = Math.Round(absentDays * perDaySalary, 2);

                        double perHourSalary = perDaySalary / minHours;
                        double aprvLeaveHours = totalApprovedLeave * minHours;
                        double salaryHours = totalWorkingHours + aprvLeaveHours - lateHours;

                        obj = new
                        {
                            BaseSalary = baseSalary,
                            LeaveDeduction = 0,
                            AdvancePayment = totalAdvncPayment,
                            TotalLateHours = lateHours,
                            LateHoursDeduction = Math.Round(lateHours * perHourSalary, 2),
                            AbsentDaysDeduction = AbsentDaysDeduction,
                            onHandSalary = Math.Round((salaryHours * perHourSalary) - totalAdvncPayment, 2)
                        };

                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                        _apiResult.Result = obj;
                    }
                    else
                    {
                        obj = new
                        {
                            BaseSalary = baseSalary,
                            LeaveDeduction = 0,
                            AdvancePayment = totalAdvncPayment,
                            LateHoursDeduction = 0,
                            AbsentDaysDeduction = 0,
                            onHandSalary = Math.Round(baseSalary - totalAdvncPayment, 2)
                        };

                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = "Attendance not found for requested month";
                        _apiResult.Result = obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("GetTaskByUserAndDate")]
        public ApiResult GetTaskByUserAndDate(UserTaskByDateReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                DateTime date, today = DateTime.Now;
                if (!string.IsNullOrEmpty(model.Date))
                {
                    if (DateTime.TryParse(model.Date.Trim(), out DateTime Temp) == true)
                    {
                        date = DateTime.ParseExact(model.Date.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                        if (date == null || date > DateTime.Now)
                        {
                            return Utility.InvalidModelMessage("Invalid Date");
                        }
                        date = new DateTime(date.Year, date.Month, date.Day, 00, 01, 00);
                    }
                    else
                        date = new DateTime(today.Year, today.Month, today.Day, 00, 01, 00);
                }
                else
                {
                    date = new DateTime(today.Year, today.Month, today.Day, 00, 01, 00);
                }

                var fromDate = date;
                var toDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 23, 59, 59);

                using (var entity = new admin_csmariseEntities())
                {
                    var deptList = new List<object>();
                    var dept = (from d in entity.Department
                                join ud in entity.UserDepartment on d.Id equals ud.DepartmentId
                                where ud.UserId == user.Id
                                select new
                                {
                                    d.Id,
                                    d.Name
                                }).ToList();

                    if (dept != null)
                    {
                        var taskListObj = new List<object>();
                        foreach (var item in dept)
                        {
                            var tasklist = (from td in entity.TaskDepartment
                                            join t in entity.Task on td.TaskId equals t.Id
                                            where td.DepartmentId == item.Id && td.CreatedOn >= fromDate && td.CreatedOn <= toDate
                                            select new
                                            {
                                                TaskId = td.Id,
                                                Title = t.TaskName,
                                                Description = td.Remarks,
                                                IsCompleted = false,
                                                CompletedBy = 0,
                                                CompletedOn = DateTime.MinValue,
                                                Rating = 0,
                                                RatedBy = 0,
                                                RatedOn = DateTime.MinValue,
                                                Comment = string.Empty,
                                                CommentBy = 0,
                                                CommentOn = DateTime.MinValue,
                                                IsDeleted = td.IsDeleted,
                                                CreatedBy = t.CreatedBy,
                                                CreatedOn = t.CreatedOn,
                                                ModifiedBy = td.ModifiedBy,
                                                ModifiedOn = td.ModifiedOn,
                                                DeletedBy = td.DeletedBy,
                                                DeletedOn = td.DeletedOn
                                            }).ToList();

                            if (tasklist != null && tasklist.Count > 0)
                            {
                                foreach (var task in tasklist)
                                {
                                    var taskStatus = entity.DailyTaskStatus.Where(x => x.TaskId == task.TaskId && x.UserId == model.UserId).FirstOrDefault();
                                    if (taskStatus != null)
                                    {
                                        taskListObj.Add(new
                                        {
                                            TaskId = task.TaskId,
                                            Title = task.Title,
                                            Description = task.Description,
                                            IsCompleted = taskStatus.IsCompleted ?? false,
                                            CompletedBy = taskStatus.UserId,
                                            CompletedOn = taskStatus.CreatedOn != null ? taskStatus.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                                            Rating = taskStatus.Rating,
                                            RatedBy = taskStatus.RatingBy,
                                            RatedOn = taskStatus.RatingOn != null ? taskStatus.RatingOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                                            Comment = taskStatus.Comment,
                                            CommentBy = taskStatus.CommentedBy,
                                            CommentOn = taskStatus.CommentedOn != null ? taskStatus.CommentedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                                            IsDeleted = task.IsDeleted,
                                            CreatedBy = task.CreatedBy,
                                            CreatedOn = task.CreatedOn != null ? task.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                                            ModifiedBy = task.ModifiedBy,
                                            ModifiedOn = task.ModifiedOn != null ? task.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                                            DeletedBy = task.DeletedBy,
                                            DeletedOn = task.DeletedOn != null ? task.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty
                                        });
                                    }
                                    else
                                        taskListObj.Add(task);
                                }
                            }

                            deptList.Add(new
                            {
                                DepartmentId = item.Id,
                                Name = item.Name,
                                Tasks = taskListObj
                            });
                        }
                    }

                    var _retModel = new
                    {
                        Departments = deptList
                    };

                    if (_retModel != null)
                    {
                        _apiResult.Response = true;
                        _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                        _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                        _apiResult.Result = _retModel;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }

        [HttpPost]
        [ActionName("SubmitTask")]
        public ApiResult SubmitTask(SubmitTaskReqModel model)
        {
            ApiResult _apiResult = new ApiResult();
            try
            {
                if (!model.GetIsValid())
                {
                    Log.Error(model.ErrorMessage);
                    return Utility.InvalidModelMessage(model.ErrorMessage);
                }

                var user = _user.FindBy(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (user == null)
                {
                    return Utility.InvalidModelMessage("User not found");
                }

                var taskdepartment = _taskDepartment.FindBy(x => x.Id == model.TaskId && x.IsDeleted == false).FirstOrDefault();
                if (taskdepartment == null)
                {
                    return Utility.InvalidModelMessage("Invalid TaskId");
                }

                DateTime date;
                if (!string.IsNullOrEmpty(model.Date))
                {
                    if (DateTime.TryParse(model.Date.Trim(), out DateTime Temp) == true)
                    {
                        date = DateTime.ParseExact(model.Date.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                        if (date == null || date > DateTime.Now)
                        {
                            return Utility.InvalidModelMessage("Invalid Date");
                        }
                        date = date.Date;
                    }
                    else
                        date = DateTime.Now.Date;
                }
                else
                    date = DateTime.Now.Date;

                DailyTaskStatus dailyTask = new DailyTaskStatus();

                var isExist = _dailytaskstatus.FindBy(x => x.TaskId == model.TaskId && x.UserId == model.UserId && x.Date == date && x.IsCompleted == true).FirstOrDefault();
                if (isExist != null)
                {
                    return Utility.InvalidModelMessage("Task already completed");
                }
                else
                {
                    dailyTask.Date = taskdepartment.CreatedOn.Value.Date;
                    dailyTask.TaskId = taskdepartment.Id;
                    dailyTask.UserId = model.UserId;
                    dailyTask.IsCompleted = model.IsCompleted;
                    if (model.IsCompleted == true)
                    {
                        dailyTask.CompletedOn = DateTime.Now;
                    }
                    else
                    {
                        dailyTask.CompletedOn = null;
                    }
                    dailyTask.CreatedBy = model.UserId;
                    dailyTask.CreatedOn = DateTime.Now;
                    dailyTask.IsDeleted = false;

                    _dailytaskstatus.Add(dailyTask);
                    _dailytaskstatus.Save();
                }

                var taskname = _task.FindBy(x => x.Id == taskdepartment.TaskId).Select(x => x.TaskName).FirstOrDefault();
                var _retModel = new
                {
                    TaskId = taskdepartment.Id,
                    Title = taskname,
                    Description = taskdepartment.Remarks,
                    IsCompleted = dailyTask.IsCompleted ?? false,
                    CompletedBy = model.UserId,
                    CompletedOn = dailyTask.CreatedOn != null ? dailyTask.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                    Rating = dailyTask.Rating,
                    RatedBy = dailyTask.RatingBy,
                    RatedOn = dailyTask.RatingOn != null ? dailyTask.RatingOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                    Comment = dailyTask.Comment,
                    CommentBy = dailyTask.CommentedBy,
                    CommentOn = dailyTask.CommentedOn != null ? dailyTask.CommentedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                    IsDeleted = dailyTask.IsDeleted,
                    CreatedBy = dailyTask.CreatedBy,
                    CreatedOn = dailyTask.CreatedOn != null ? dailyTask.CreatedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                    ModifiedBy = dailyTask.ModifiedBy,
                    ModifiedOn = dailyTask.ModifiedOn != null ? dailyTask.ModifiedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty,
                    DeletedBy = dailyTask.DeletedBy,
                    DeletedOn = dailyTask.DeletedOn != null ? dailyTask.DeletedOn.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty

                };

                if (_retModel != null)
                {
                    _apiResult.Response = true;
                    _apiResult.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
                    _apiResult.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
                    _apiResult.Result = _retModel;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Utility.InvalidModelMessage(ex.Message);
            }
            return _apiResult;
        }
    }
}