﻿namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using GetPlace;
using WorldExplorer.Common.Application.Messaging;

public sealed record GetPlacesQuery : IQuery<List<PlaceResponse>>;