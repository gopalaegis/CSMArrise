using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCInsight.Models
{
    public class OtpVarificationReqModel
    {
        public bool GetIsValid()
        {
            List<string> lstErrorList = new List<string>();
            if (string.IsNullOrEmpty(OTP) || string.IsNullOrEmpty(Username))
            {
                if (string.IsNullOrEmpty(OTP))
                    lstErrorList.Add("OTP is Required");

                if (string.IsNullOrEmpty(Username))
                    lstErrorList.Add("Username is Required");
            }
            if (lstErrorList.Any())
            {
                ErrorMessage = string.Join(",", lstErrorList);
                return false;
            }
            return true;
        }

        public string OTP { get; set; }

        public string Username { get; set; }

        public string ErrorMessage { get; set; }

    }
}