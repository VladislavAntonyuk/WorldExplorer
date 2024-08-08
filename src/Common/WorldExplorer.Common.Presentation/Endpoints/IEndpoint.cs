using Microsoft.AspNetCore.Routing;

namespace WorldExplorer.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
