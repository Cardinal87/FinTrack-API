

namespace FinTrack.API.Application.Common
{
    public class ValueResult<T> : ResultBase
    {
        public T? Value { get; }

        private ValueResult(T? value,
                            bool isSuccess,
                            string statusMessage) : base(isSuccess, statusMessage)
        {
            Value = value;
        }

        public static ValueResult<T> Ok(T value, string statusMessage) => new ValueResult<T>(value, true, statusMessage);

        public static ValueResult<T> Fail(string statusMessage, Exception exception) => new ValueResult<T>(default, false, statusMessage);
    }
}
