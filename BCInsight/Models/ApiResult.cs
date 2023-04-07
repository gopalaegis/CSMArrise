namespace BCInsight.Models
{
    public class ApiResult
    {
        public ApiResult()
        {
            ReturnCode = 0;
            Message = string.Empty;
        }

        public bool Response { get; set; }

        public int ReturnCode { get; set; }

        public string Message { get; set; }

        public dynamic Result { get; set; }
    }
}