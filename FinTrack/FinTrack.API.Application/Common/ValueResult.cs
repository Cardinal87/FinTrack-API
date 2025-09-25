

namespace FinTrack.API.Application.Common
{
    public class ValueResult<T> : ResultBase
    {
        public T? Value { get; }

        private ValueResult(T? value,
                            bool isSuccess,
                            string statusMessage,
                            string? errorMessage = null) : base(isSuccess, statusMessage, errorMessage)
        {
            Value = value;
        }

        public static ValueResult<T> Ok(T value, string statusMessage) => new ValueResult<T>(value, true, statusMessage);

        public static ValueResult<T> Fail(string statusMessage, string errorMessage) => new ValueResult<T>(default, false, statusMessage, errorMessage);
    }
}
