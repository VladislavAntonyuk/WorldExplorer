namespace WorldExplorer.Common.Infrastructure.Serialization;

using System.Text.Json;

public static class SerializerSettings
{
	public static readonly JsonSerializerOptions Instance = new(JsonSerializerDefaults.Web);
}