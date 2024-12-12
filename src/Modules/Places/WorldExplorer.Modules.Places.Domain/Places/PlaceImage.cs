namespace WorldExplorer.Modules.Places.Domain.Places;

using Common.Domain;

public class PlaceImage : Entity
{
	public required Guid Id { get; set; }

	public required string Source { get; set; }

	public Guid PlaceId { get; set; }
}