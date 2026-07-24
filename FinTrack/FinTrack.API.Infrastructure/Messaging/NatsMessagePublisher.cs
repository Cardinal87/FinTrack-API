

using FinTrack.API.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace FinTrack.API.Infrastructure.Messaging
{
    public class NatsMessagePublisher : IMessagePublisher
    {
        private readonly ILogger<NatsMessagePublisher> _logger;
        private readonly INatsJSContext _context;
        private readonly MessageStreamOptions _streamOptions;
        private bool isStreamInitialized;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public NatsMessagePublisher(ILogger<NatsMessagePublisher> logger,  INatsJSContext context, IOptions<MessageStreamOptions> streamOptions)
        {
            _logger = logger;
            _context = context;
            _streamOptions = streamOptions.Value;
        }

        public async Task PublishAsync<T>(string subject, T message, CancellationToken cancellationToken = default) where T : class
        {
            await InitializeStream(cancellationToken);

            var ack = await _context.PublishAsync(subject, message, cancellationToken:  cancellationToken);
            ack.EnsureSuccess();

            _logger.LogDebug("Message for subject {Subject} published", subject);
        }

        public async Task InitializeStream(CancellationToken ct)
        {
            if (isStreamInitialized) return;

            await _semaphore.WaitAsync(ct);
            try
            {
                if (isStreamInitialized) return;
                var stream = new StreamConfig
                {
                    Name = _streamOptions.Name,
                    Subjects = _streamOptions.Subjects,
                    MaxAge = _streamOptions.MaxAge,
                    MaxBytes = _streamOptions.MaxBytes,
                    Storage = StreamConfigStorage.File,
                    Retention = StreamConfigRetention.Workqueue
                };
                await _context.CreateStreamAsync(stream, ct);
                isStreamInitialized = true;

                _logger.LogDebug("Stream initialized");
            }
            finally
            {
                _semaphore.Release();
            }

        }
    }
}
