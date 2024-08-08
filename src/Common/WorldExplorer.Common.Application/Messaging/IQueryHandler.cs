using WorldExplorer.Common.Domain;
using MediatR;

namespace WorldExplorer.Common.Application.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
