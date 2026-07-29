

using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.Messages;
using System.Collections.Concurrent;

namespace FinTrack.API.TestMocks.Messaging
{
    public class MessagePublisherMock : IMessagePublisher
    {
        private readonly ConcurrentDictionary<Guid, string> _publishedCodes = new();
        public Task PublishAsync<T>(string subject, T message, CancellationToken cancellationToken = default) where T : struct
        {
            switch (message)
            {
                case EmailVerificationCodeMessage emailMsg:
                    _publishedCodes[emailMsg.UserId] = emailMsg.TotpCode;
                    break;
                case LoginVerificationCodeMessage loginMsg:
                    _publishedCodes[loginMsg.UserId] = loginMsg.TotpCode;
                    break;
            }

            return Task.CompletedTask;
        }

        public string GetCode(Guid userId)
        {
            return _publishedCodes.TryGetValue(userId, out var code) ? code : "";
        }

        public void Reset()
        {
            _publishedCodes.Clear();
        }
    }
}
