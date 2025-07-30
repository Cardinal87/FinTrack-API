
namespace FinTrack.API.Application.Common
{
    public abstract class ResultBase
    {
        public bool IsSuccess { get; }
        public string StatusMessage { get; }

        protected ResultBase(bool isSuccess, string statusMessage)
        {
            IsSuccess = isSuccess;
            StatusMessage = statusMessage;
        }
    }
}
