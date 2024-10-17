namespace WorldExplorer.Common.Infrastructure.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public static class SerializerSettings
{
	public static readonly JsonSerializerOptions Instance = new(JsonSerializerDefaults.Web)
	{
		AllowOutOfOrderMetadataProperties = true,
		ReferenceHandler = ReferenceHandler.Preserve,
		NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
	};

	public static readonly JsonSerializerSettings JsonSerializerSettingsInstance = new()
	{
		TypeNameHandling = TypeNameHandling.All,
		MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore
	};
}