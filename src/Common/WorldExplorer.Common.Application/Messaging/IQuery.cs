using WorldExplorer.Common.Domain;
using MediatR;

namespace WorldExplorer.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
