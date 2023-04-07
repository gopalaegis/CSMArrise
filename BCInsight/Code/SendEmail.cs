using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace BCInsight.Code
{
    public class SendEmail
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool SendMail(string subject, string message, string to, string fileAttachment)
        {
            try
            {
                bool IsEmailEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["IsEmailEnabled"]);
                if (IsEmailEnabled == true)
                {
                    MailMessage mail = new MailMessage();
                    if (to.Contains(','))
                    {
                        var list = to.Split(',');
                        foreach (var item in list)
                        {
                            mail.To.Add(item);
                        }
                    }
                    else
                    {
                        mail.To.Add(to);
                    }
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"].ToString());
                    mail.Subject = subject;
                    string Body = message;
                    mail.Body = Body;
                    if (fileAttachment != null && fileAttachment != "")
                    {
                        Attachment attachment = new Attachment(fileAttachment);
                        mail.Attachments.Add(attachment);
                    }
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["Host"].ToString();
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential
                    (ConfigurationManager.AppSettings["FromEmail"].ToString(),
                        ConfigurationManager.AppSettings["Emailpassword"].ToString());
                    smtp.EnableSsl = (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]));
                    smtp.Send(mail);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;

        }

        public static string SendTemplateEmail(EmailMessageType emailMessageType, out string emailSubject, params string[] data)
        {
            emailSubject = string.Empty;
            string emailMessage = "";
            StringBuilder sb = new StringBuilder();
            switch (emailMessageType)
            {
                case EmailMessageType.forgotpassword:

                    emailSubject = "Audiobeez - Request new password Generate";

                    var ResetLink = ConfigurationManager.AppSettings["WebURL"].ToString() + "/Login";

                    sb.Clear();
                    sb.Append("Hello, <b>" + data[0] + "</b> <br/><br/>");
                    sb.Append("Your new password generated successfully. You can login using new password. <br>");
                    sb.Append("Please click on the below link for login <br>");
                    sb.Append("<a href=" + ResetLink + ">" + ResetLink + "</a><br><br>");
                    sb.Append("New password is : <b>" + data[1] + "</b><br/>");
                    sb.Append("-----------<br/>");
                    sb.Append("Warm Regards,<br>Support<br>Audiobeez Team");
                    emailMessage = sb.ToString();
                    break;
                case EmailMessageType.OTPVarification:
                    emailSubject = "Audiobeez - OTP varification";
                    sb.Clear();

                    sb.Append("Hello " + data[0] + ",<br><br>");
                    sb.Append("Your OTP is : " + data[1] + ".<br>");
                    sb.Append("Don't share OTP with anyone.<br><br>");
                    sb.Append("Warm Regards,<br>Support<br>Audiobeez Team");
                    emailMessage = sb.ToString();
                    break;
                    sb.Append("Warm Regards,<br>Support<br>Audiobeez Team");
                    emailMessage = sb.ToString();
                    break;
                default:
                    break;
            }
            return emailMessage;
        }
    }
}