

namespace FinTrack.API.Infrastructure.Messaging
{
    public class MessageStreamOptions
    {
        public string Name { get; set; } = null!;
        public string[] Subjects { get; set; } = null!;
        public TimeSpan MaxAge { get; set; }
        public int MaxBytes { get; set; }
    }
}
