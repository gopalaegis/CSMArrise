using Antlr.Runtime.Misc;
using BCInsight.DAL;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.VariantTypes;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace BCInsight.Models
{
    public class APIReqModel
    {
        public APIReqModel()
        {

        }
        public bool GetIsValid()
        {
            List<string> lstErrorList = new List<string>();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                if (string.IsNullOrEmpty(Username))
                    lstErrorList.Add("Username is Required");

                if (string.IsNullOrEmpty(Password))
                    lstErrorList.Add("Password Name is Required");

            }
            if (lstErrorList.Any())
            {
                ErrorMessage = string.Join(",", lstErrorList);
                return false;
            }
            return true;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ForgotPasswordReqModel
    {
        public ForgotPasswordReqModel()
        {

        }
        public bool GetIsValid()
        {
            List<string> lstErrorList = new List<string>();
            if (string.IsNullOrEmpty(Email))
            {
                if (string.IsNullOrEmpty(Email))
                    lstErrorList.Add("Email is Required");
            }
            if (lstErrorList.Any())
            {
                ErrorMessage = string.Join(",", lstErrorList);
                return false;
            }
            return true;
        }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UpdateCheckTimeReqModel
    {
        public UpdateCheckTimeReqModel() { }
        public bool GetIsValid()
        {
            List<string> lstErrorList = new List<string>();
            if (UserId <= 0)
                lstErrorList.Add("Invalid UserId");
            if (string.IsNullOrEmpty(CheckType))
                lstErrorList.Add("CheckType is required");
            if (string.IsNullOrEmpty(CheckTime))
                lstErrorList.Add("CheckTime is required");
            if (lstErrorList.Any())
            {
                ErrorMessage = string.Join(",", lstErrorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public string CheckType { get; set; }
        public string CheckTime { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UserIdReqModel
    {
        public UserIdReqModel() { }
        public bool GetIsValid()
        {
            List<string> errorMessage = new List<string>();
            if (UserId <= 0)
                errorMessage.Add("Invalid UserId");
            if (errorMessage.Count > 0)
            {
                ErrorMessage = string.Join(",", errorMessage);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AttendanceDetailReqModel
    {
        public AttendanceDetailReqModel()
        {

        }
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            if (Year <= 0)
                errorList.Add("Invalid Year");
            if (Month <= 0)
                errorList.Add("Invalid Month");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PresentAbsentModel
    {
        public string type { get; set; }
        public string date { get; set; }
        public string deduction { get; set; }
        public string message { get; set; }
    }

    public class AttendanceDetailModel
    {
        public int UserId { get; set; }
        public string BaseSalary { get; set; }
        public string onHandSalary { get; set; }
        public string SalaryFrom { get; set; }
        public string SalaryTo { get; set; }
        public double totalAdvancedPayment { get; set; }
        public List<AdvancedPayment> advancedPayment { get; set; }
        public List<PresentAbsentModel> presentAbsentModel { get; set; }
    }

    public class AdvancedPayment
    {
        public string Date { get; set; }
        public string Amount { get; set; }
    }

    public class UserAttendanceDetails
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public double? Salary { get; set; }
        public string SalaryDuration { get; set; }

        public string Date { get; set; }
        public string CkeckIn { get; set; }
        public string CheckOut { get; set; }
        public string Remarks { get; set; }
    }

    public class AdvancePayRequest
    {
        public AdvancePayRequest()
        {

        }
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            if (Amount <= 0 || string.IsNullOrEmpty(Amount.ToString().Trim()))
                errorList.Add("Invalid Amount");
            if (string.IsNullOrEmpty(Reason))
                errorList.Add("Reason is required");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public double Amount { get; set; }
        public string Reason { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ManualAttendenceReqModel
    {
        public ManualAttendenceReqModel() { }
        public bool GetIsValid()
        {
            List<string> errorMessage = new List<string>();
            if (UserId <= 0)
                errorMessage.Add("Invalid UserId");
            if (string.IsNullOrEmpty(Date))
                errorMessage.Add("Date is required");
            if (string.IsNullOrEmpty(Reason))
                errorMessage.Add("Reason is required");
            //if (string.IsNullOrEmpty(Description))
            //    errorMessage.Add("Description is required");

            if (errorMessage.Count > 0)
            {
                ErrorMessage = string.Join(",", errorMessage);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class LeaveRequestReqModel
    {
        public LeaveRequestReqModel() { }
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            if (string.IsNullOrEmpty(FromDate.Trim()))
                errorList.Add("Invalid Date Format");
            if (string.IsNullOrEmpty(ToDate.Trim()))
                errorList.Add("Invalid Date Format");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Reason { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UserLeavAndSalaryeRequestReqModel
    {
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            if (Year < 0)
                errorList.Add("Invalid Year");
            if (Month < 0)
                errorList.Add("Invalid Month");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UserTaskByDateReqModel
    {
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            //if (string.IsNullOrEmpty(Date))
            //    errorList.Add("Invalid Date");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public string Date { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SubmitTaskReqModel
    {
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            if (DepartmentId <= 0)
                errorList.Add("Invalid DepartmentId");
            if (TaskId <= 0)
                errorList.Add("Invalid TaskId");
            //if (string.IsNullOrEmpty(Date))
            //    errorList.Add("Invalid Date");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int TaskId { get; set; }

        public string Date { get; set; }
        public bool IsCompleted { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TaskRatingReqModel
    {
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");
            if (DepartmentId <= 0)
                errorList.Add("Invalid DepartmentId");
            if (TaskId <= 0)
                errorList.Add("Invalid TaskId");
            if (Rating <= 0)
                errorList.Add("Invalid Rating");
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public int DepartmentId { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class GetSubCoordinatorTask
    {
        public bool GetIsValid()
        {
            List<string> errorList = new List<string>();
            if (UserId <= 0)
                errorList.Add("Invalid UserId");           
           
            if (errorList.Any())
            {
                ErrorMessage = string.Join(",", errorList);
                return false;
            }
            return true;
        }
        public int UserId { get; set; }
        public string Date { get; set; }
        public string ErrorMessage { get; set; }
    }
}