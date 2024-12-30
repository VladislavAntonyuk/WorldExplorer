namespace WorldExplorer.Modules.Places.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

public class PointJsonConverter : JsonConverter<Point>
{
	public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var pointObject = JsonDocument.ParseValue(ref reader);
		pointObject.RootElement.TryGetProperty("y", out var latitude);
		pointObject.RootElement.TryGetProperty("x", out var longitude);
		pointObject.RootElement.TryGetProperty("SRID", out var srid);
		var point = new Point(longitude.GetDouble(), latitude.GetDouble())
		{
			SRID = srid.GetInt32()
		};
		return point;
	}

	public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteNumber("y", value.Y);
		writer.WriteNumber("x", value.X);
		writer.WriteNumber("SRID", value.SRID);
		writer.WriteEndObject();
	}
}