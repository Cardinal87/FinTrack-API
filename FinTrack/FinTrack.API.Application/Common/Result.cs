

namespace FinTrack.API.Application.Common
{
    public class Result : ResultBase
    {

        protected Result(bool isSuccess,
                         string statusMessage) : base(isSuccess, statusMessage)
        {

        }
        

        public static Result Ok(string statusMessage) => new Result(true, statusMessage);
        public static Result Fail(string statusMessage) => new Result(false, statusMessage);
    }
}
