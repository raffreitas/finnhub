using FinnHub.Shared.Core;

using Flurl.Http;

using Polly;
using Polly.Fallback;
using Polly.Retry;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Shared.Policies;

internal static class PolicyFactory
{
    public static IAsyncPolicy<Result<T>> CreateDefaultPolicy<T>()
    {
        return new ResiliencePipelineBuilder<Result<T>>()
            .AddRetry(new RetryStrategyOptions<Result<T>>
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                ShouldHandle = new PredicateBuilder<Result<T>>()
                    .Handle<FlurlHttpException>(IsTransient),
            })
            .AddFallback(new FallbackStrategyOptions<Result<T>>
            {
                ShouldHandle = new PredicateBuilder<Result<T>>()
                    .Handle<FlurlHttpException>(IsTransient),
                FallbackAction = async args =>
                {
                    var exception = args.Outcome.Exception as FlurlHttpException;
                    var errorMessage = exception?.Message ?? "Unknown error.";
                    var result = Result.Failure<T>(Error.Failure("Integration.Error", errorMessage));
                    return await Outcome.FromResultAsValueTask(result);
                }
            })
            .Build()
            .AsAsyncPolicy();
    }

    private static bool IsTransient(FlurlHttpException ex)
        => ex.Call?.Response == null
            || ex.Call.Response.StatusCode >= 500
            || ex.InnerException is TimeoutException;
}
