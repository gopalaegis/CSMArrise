namespace BCInsight.Web.HelperClass
{
    public class EnumErrorCodeHelper
    {
        public enum SuccessCodes
        {
            Success = 0,
            RegisterSuccess = 1,
            VerifySuccess = 2,
            ResendSuccess = 3,
            LoginSuccess = 4,
            MAIL_SEND_SUCCESS = 5,
            PODCAST_ADDED_SUCCESS = 6,
            USER_VARIFICATION_SUCCESS = 7,
            UserNotActive = 8,
            RecordUpdated = 9
        }

        public enum ErrorCodes
        {
            Fail = 1000,
            InvalidModel = 1001,
            InvalidDeviceRequest = 1002,
            LoginFail = 1003,
            alreadyRegister = 1004,
            USERNAME_NOTFOUND = 1005,
            ENTER_VALID_OTP = 1006,
            PODCAST_NOTFOUND = 1007,
            EPISODE_NOTFOUND = 1008,
            RecordNotFound = 1009
        }
    }
}