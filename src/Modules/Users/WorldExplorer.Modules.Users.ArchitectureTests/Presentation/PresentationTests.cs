namespace WorldExplorer.Modules.Users.ArchitectureTests.Presentation;

using Abstractions;
using Common.Application.EventBus;
using NetArchTest.Rules;
using Xunit;

public class PresentationTests : BaseTest
{
	[Fact]
	public void IntegrationEventHandler_Should_NotBePublic()
	{
		Types.InAssembly(PresentationAssembly)
		     .That()
		     .ImplementInterface(typeof(IIntegrationEventHandler<>))
		     .Or()
		     .Inherit(typeof(IntegrationEventHandler<>))
		     .Should()
		     .NotBePublic()
		     .GetResult()
		     .ShouldBeSuccessful();
	}

	[Fact]
	public void IntegrationEventHandler_Should_BeSealed()
	{
		Types.InAssembly(PresentationAssembly)
		     .That()
		     .ImplementInterface(typeof(IIntegrationEventHandler<>))
		     .Or()
		     .Inherit(typeof(IntegrationEventHandler<>))
		     .Should()
		     .BeSealed()
		     .GetResult()
		     .ShouldBeSuccessful();
	}

	[Fact]
	public void IntegrationEventHandler_ShouldHave_NameEndingWith_DomainEventHandler()
	{
		Types.InAssembly(PresentationAssembly)
		     .That()
		     .ImplementInterface(typeof(IIntegrationEventHandler<>))
		     .Or()
		     .Inherit(typeof(IntegrationEventHandler<>))
		     .Should()
		     .HaveNameEndingWith("IntegrationEventHandler")
		     .GetResult()
		     .ShouldBeSuccessful();
	}
}