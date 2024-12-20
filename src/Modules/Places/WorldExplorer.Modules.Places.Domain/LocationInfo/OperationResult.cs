﻿namespace WorldExplorer.Modules.Places.Domain.LocationInfo;

public class OperationResult<T> where T : new()
{
	public T Result { get; init; } = new();

	public StatusCode StatusCode { get; init; }
}