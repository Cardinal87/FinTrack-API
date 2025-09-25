
namespace FinTrack.API.Application.Common
{
    public abstract class ResultBase
    {
        public bool IsSuccess { get; }
        public string StatusMessage { get; }
        public string? ErrorMessage { get; }

        protected ResultBase(bool isSuccess, string statusMessage, string? errorMessage)
        {
            IsSuccess = isSuccess;
            StatusMessage = statusMessage;
            ErrorMessage = errorMessage;
        }
    }
}
