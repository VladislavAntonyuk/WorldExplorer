using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using NetTopologySuite.Geometries;
using Places.Application.Places.GetPlace;
using Places.Domain.Places;
using Shared.Models;

public sealed record GetPlaceDetailsQuery(Guid PlaceId) : IQuery<PlaceResponse>;
