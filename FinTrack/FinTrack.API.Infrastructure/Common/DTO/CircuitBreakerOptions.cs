

namespace FinTrack.API.Infrastructure.Common.DTO
{
    public class CircuitBreakerOptions()
    {
        public TimeSpan SamplingDuration { get; set; }
        public int MinimumThroughput { get; set; }
        public double FailureRatio { get; set; }
        public TimeSpan BreakDuration { get; set; }
    }
}
