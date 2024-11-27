namespace WorldExplorer.Common.Infrastructure.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

public static class SerializerSettings
{
	public static readonly JsonSerializerSettings JsonSerializerSettingsInstance = new()
	{
		TypeNameHandling = TypeNameHandling.All,
		MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore
	};

	public static readonly JsonSerializerOptions Instance = new(JsonSerializerDefaults.Web)
	{
		AllowOutOfOrderMetadataProperties = true,
		ReferenceHandler = ReferenceHandler.Preserve,
		NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
	};

	public static void ConfigureJsonSerializerSettingsInstance(IList<JsonConverter> converters) 
	{
		JsonSerializerSettingsInstance.Converters = converters;
	}

	public static void ConfigureJsonSerializerOptionsInstance(IList<System.Text.Json.Serialization.JsonConverter> converters) 
	{
		foreach (var converter in converters)
		{
			Instance.Converters.Add(converter);
		}
	}
}