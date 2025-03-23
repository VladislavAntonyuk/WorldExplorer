namespace WorldExplorer.Modules.Places.Infrastructure.UnitTests;

using System.Text.Json;
using NetTopologySuite.Geometries;
using Shouldly;
using Xunit;

public class PointJsonConverterTests
{
	private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
	{
		Converters =
		{
			new PointJsonConverter()
		}
	};

	[Fact]
	public void ConvertSystemTextJson()
	{
		var point = new Point(1, 2) { SRID = 3 };
		var json = JsonSerializer.Serialize(point, jsonSerializerOptions);
		var result = JsonSerializer.Deserialize<Point>(json, jsonSerializerOptions);
		result.ShouldBe(point);
	}
}
