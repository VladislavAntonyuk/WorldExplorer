namespace WorldExplorer.Modules.Places.Infrastructure.UnitTests;

using System.Text.Json;
using FluentAssertions;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class PointJsonConverterTests
{
	private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
	{
		Converters =
		{
			new PointJsonConverter()
		}
	};

	private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
	{
		Converters =
		{
			new PointNewtonsoftJsonConverter()
		}
	};

	[Fact]
	public void ConvertSystemTextJson()
	{
		var point = new Point(1, 2){SRID = 3};
		var json = JsonSerializer.Serialize(point, jsonSerializerOptions);
		var result = JsonSerializer.Deserialize<Point>(json, jsonSerializerOptions);
		result.Should().Be(point);
	}

	[Fact]
	public void ConvertNewtonsoftJson()
	{
		var point = new Point(1, 2){SRID = 3};
		var json = JsonConvert.SerializeObject(point, jsonSerializerSettings);
		var result = JsonConvert.DeserializeObject<Point>(json, jsonSerializerSettings);
		result.Should().Be(point);
	}
}
