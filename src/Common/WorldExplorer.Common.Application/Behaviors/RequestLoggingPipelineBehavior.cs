using System.Diagnostics;
using WorldExplorer.Common.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace WorldExplorer.Common.Application.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string moduleName = GetModuleName(typeof(TRequest).FullName!);
        string requestName = typeof(TRequest).Name;

        Activity.Current?.SetTag("request.module", moduleName);
        Activity.Current?.SetTag("request.name", requestName);

        using (logger.BeginScope("Module {0}", moduleName))
        {
            logger.LogInformation("Processing request {RequestName}", requestName);

            TResponse result = await next();

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed request {RequestName}", requestName);
            }
            else
            {
                using (logger.BeginScope("Error {0}", result.Error))
                {
                    logger.LogError("Completed request {RequestName} with error", requestName);
                }
            }

            return result;
        }
    }

    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}
