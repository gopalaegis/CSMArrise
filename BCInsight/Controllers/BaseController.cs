using BCInsight.Code;
using BCInsight.DAL;
using BCInsight.Web.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BCInsight.Controllers
{
    public class BaseController : Controller
    {

        //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        DAL.admin_csmariseEntities entities = new DAL.admin_csmariseEntities();

        public LoginUser CurrentUser;
        public BaseController()
        {
            CurrentUser = Utility.CurrentLoginUser;

        }
        public int LoginUserId()
        {
            var authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                string UserName = authTicket.Name;
                List<string> s = new List<string>(UserName.Split(new string[] { "|" }, StringSplitOptions.None));
                return Convert.ToInt32(s.FirstOrDefault());
            }
            return 0;
        }

        public void SetNotification(string message, NotificationEnum notifyType, bool autoHideNotification = true)
        {
            this.TempData["Notification"] = message;
            this.TempData["NotificationAutoHide"] = (autoHideNotification) ? "true" : "false";

            switch (notifyType)
            {
                case NotificationEnum.Success:
                    this.TempData["NotificationCSS"] = "notificationbox notification-success";
                    break;
                case NotificationEnum.Error:
                    this.TempData["NotificationCSS"] = "notificationbox notification-danger";
                    break;
                case NotificationEnum.Warning:
                    this.TempData["NotificationCSS"] = "notificationbox notification-warning";
                    break;
                case NotificationEnum.Info:
                    this.TempData["NotificationCSS"] = "notificationbox notification-info";
                    break;
            }

        }
    }
}