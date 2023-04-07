using BCInsight.Code;
using BCInsight.Models;
using System;
using System.Linq;
using System.Web;
using BCInsight.DAL;

namespace BCInsight.Web.HelperClass
{
    public class Utility
    {
        //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static LoginUser CurrentLoginUser => GetLoggedInUser();

        private static LoginUser GetLoggedInUser()
        {
            try
            {
                Int32 userid = 0;
                HttpCookie myCookie1 = HttpContext.Current.Request.Cookies["admin_id"];
                if (myCookie1 != null && !string.IsNullOrEmpty(myCookie1.Value))
                    userid = Convert.ToInt32(myCookie1.Value);

                if (userid <= 0)
                    userid = HttpContext.Current.Session["admin_id"] != null ? Convert.ToInt32(HttpContext.Current.Session["admin_id"]) : 0;

                using (admin_csmariseEntities context = new admin_csmariseEntities())
                {
                    dynamic user = null;

                    if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        int uid = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                        user = context.User.FirstOrDefault(m => m.Id == uid);
                    }
                    if (user == null || user.Id == 0)
                    {
                        user = context.User.FirstOrDefault(m => m.Id == userid);
                        if (user == null || user.Id == 0)
                            return null;
                    }

                    var loginUser = new LoginUser
                    {
                        LoginId = Convert.ToString(user.Id),
                        Name = user.Name,
                        UserType = user.Role,
                        IsLogin = true
                    };

                    return loginUser;
                }
            }
            catch (Exception e)
            {
                //Log.Error("Error from BCInsight Utility User : " + e); Log.Error("Error from BCInsight Utility User : " + e.Message);

            }
            return null;
        }

        public static ApiResult InvalidModelMessage(string msg)
        {
            ApiResult result = new ApiResult();
            result.Response = false;
            result.ReturnCode = (int)EnumErrorCodeHelper.ErrorCodes.InvalidModel;
            result.Message = msg;
            result.Result = null;
            return result;
        }

        public static ApiResult ResponseFail()
        {
            ApiResult result = new ApiResult();
            result.Response = false;
            result.ReturnCode = (int)EnumErrorCodeHelper.ErrorCodes.Fail;
            result.Message = ErrorHelpers.GetErrorMessage(EnumErrorCodeHelper.ErrorCodes.Fail); ;
            result.Result = null;
            return result;
        }

        public static ApiResult ResponseSucess()
        {
            ApiResult result = new ApiResult();
            result.Response = true;
            result.ReturnCode = (int)EnumErrorCodeHelper.SuccessCodes.Success;
            result.Message = ErrorHelpers.GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes.Success);
            result.Result = null;
            return result;
        }
    }
}