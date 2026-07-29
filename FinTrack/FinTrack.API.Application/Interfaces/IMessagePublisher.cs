

namespace FinTrack.API.Application.Interfaces
{
    public interface IMessagePublisher
    {

        Task PublishAsync<T> (string subject,  T message, CancellationToken cancellationToken = default) where T: struct;
    }
}
