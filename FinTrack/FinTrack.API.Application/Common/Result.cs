

namespace FinTrack.API.Application.Common
{
    public class Result : ResultBase
    {

        protected Result(bool isSuccess,
                         string statusMessage,
                         string? errorMessage = null) : base(isSuccess, statusMessage, errorMessage)
        {

        }
        

        public static Result Ok(string statusMessage) => new Result(true, statusMessage);
        public static Result Fail(string statusMessage, string errorMessage) => new Result(false, statusMessage, errorMessage);
    }
}
