

namespace FinTrack.API.Infrastructure.Common.DTO
{
    public readonly record struct CircuitBreakerOptions(
            TimeSpan SamplingDuration,
            int MinimumThroughput,
            double FailureRatio,
            TimeSpan BreakDuration
        );
}
