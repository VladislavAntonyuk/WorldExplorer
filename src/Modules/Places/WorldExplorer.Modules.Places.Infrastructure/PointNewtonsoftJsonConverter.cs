namespace WorldExplorer.Modules.Places.Infrastructure;

using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PointNewtonsoftJsonConverter : JsonConverter<Point>
{
	public override Point ReadJson(JsonReader reader,
		Type objectType,
		Point? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer)
	{
		var pointObject = JObject.Load(reader);

		pointObject.TryGetValue("y", StringComparison.OrdinalIgnoreCase, out var latitude);
		pointObject.TryGetValue("x", StringComparison.OrdinalIgnoreCase, out var longitude);
		pointObject.TryGetValue("SRID", StringComparison.OrdinalIgnoreCase, out var srid);
		return new Point(longitude.Value<double>(), latitude.Value<double>())
		{
			SRID = srid.Value<int>()
		};
	}

	public override void WriteJson(JsonWriter writer, Point? value, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("y");
		writer.WriteValue(value.Y);
		writer.WritePropertyName("x");
		writer.WriteValue(value.X);
		writer.WritePropertyName("SRID");
		writer.WriteValue(value.SRID);
		writer.WriteEndObject();
	}
}