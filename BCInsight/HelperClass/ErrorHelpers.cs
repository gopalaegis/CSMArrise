namespace BCInsight.Web.HelperClass
{
    public class ErrorHelpers
    {
        static string Msg = string.Empty;
        public static string GetSuccessMessages(EnumErrorCodeHelper.SuccessCodes apiSuccessCodes)
        {
            switch (apiSuccessCodes)
            {
                case EnumErrorCodeHelper.SuccessCodes.Success:
                    Msg = "Success";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.RegisterSuccess:
                    Msg = "Register Success";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.VerifySuccess:
                    Msg = "Verification Success";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.ResendSuccess:
                    Msg = "Resend Success";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.LoginSuccess:
                    Msg = "Login Success";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.MAIL_SEND_SUCCESS:
                    Msg = "Mail send successfully.";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.USER_VARIFICATION_SUCCESS:
                    Msg = "User varification successfully.";
                    break;
                case EnumErrorCodeHelper.SuccessCodes.UserNotActive:
                    Msg = "User not activated.";
                    break;
            }

            return Msg;
        }

        public static string GetErrorMessage(EnumErrorCodeHelper.ErrorCodes errorCode)
        {
            switch (errorCode)
            {
                case EnumErrorCodeHelper.ErrorCodes.Fail:
                    Msg = "Fail";
                    break;
                case EnumErrorCodeHelper.ErrorCodes.InvalidModel:
                    Msg = "User Not Found Or Otp Expired";
                    break;

                case EnumErrorCodeHelper.ErrorCodes.InvalidDeviceRequest:
                    Msg = "Request From Other Device Or Platform";
                    break;
                case EnumErrorCodeHelper.ErrorCodes.LoginFail:
                    Msg = "Enter valid username and password";
                    break;
                case EnumErrorCodeHelper.ErrorCodes.alreadyRegister:
                    Msg = "Enter different Email or mobile number";
                    break;
                case EnumErrorCodeHelper.ErrorCodes.USERNAME_NOTFOUND:
                    Msg = "User Not Found!";
                    break;
                case EnumErrorCodeHelper.ErrorCodes.ENTER_VALID_OTP:
                    Msg = "Enter valid OTP.";
                    break;
                case EnumErrorCodeHelper.ErrorCodes.PODCAST_NOTFOUND:
                    Msg = "Podcast not found.";
                    break;
            }
            return Msg;
        }
    }
}