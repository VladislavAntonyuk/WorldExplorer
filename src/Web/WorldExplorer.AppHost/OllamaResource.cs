public sealed class OllamaResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
	private EndpointReference? httpReference;

	public EndpointReference HttpEndpoint =>
		httpReference ??= new(this, Name);

	public ReferenceExpression ConnectionStringExpression =>
		ReferenceExpression.Create(
			$"{HttpEndpoint.Property(EndpointProperty.Scheme)}://{HttpEndpoint.Property(EndpointProperty.Host)}:{HttpEndpoint.Property(EndpointProperty.Port)}/api"
		);
}

public static class OllamaResourceBuilderExtensions
{
	public static IResourceBuilder<OllamaResource> AddOllama(
		this IDistributedApplicationBuilder builder,
		string name,
		int? httpPort = null)
	{
		var resource = new OllamaResource(name);

		return builder.AddResource(resource)
					  .WithImage(OllamaContainerImageTags.Image)
					  .WithImageRegistry(OllamaContainerImageTags.Registry)
					  .WithImageTag(OllamaContainerImageTags.Tag)
					  .WithHttpEndpoint(
						  targetPort: 11434,
						  port: httpPort,
						  name: name)
					  .WithVolume("ollama", "/root/.ollama");
	}
}

internal static class OllamaContainerImageTags
{
	internal const string Registry = "docker.io";

	internal const string Image = "ollama/ollama";

	internal const string Tag = "0.3.8";
}