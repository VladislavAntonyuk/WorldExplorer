namespace WorldExplorer.Modules.Users.ArchitectureTests.Domain;

using System.Reflection;
using Abstractions;
using Common.Domain;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

public class DomainTests : BaseTest
{
	[Fact]
	public void DomainEvents_Should_BeSealed()
	{
		Types.InAssembly(DomainAssembly)
		     .That()
		     .ImplementInterface(typeof(IDomainEvent))
		     .Or()
		     .Inherit(typeof(DomainEvent))
		     .Should()
		     .BeSealed()
		     .GetResult()
		     .ShouldBeSuccessful();
	}

	[Fact]
	public void DomainEvent_ShouldHave_DomainEventPostfix()
	{
		Types.InAssembly(DomainAssembly)
		     .That()
		     .ImplementInterface(typeof(IDomainEvent))
		     .Or()
		     .Inherit(typeof(DomainEvent))
		     .Should()
		     .HaveNameEndingWith("DomainEvent")
		     .GetResult()
		     .ShouldBeSuccessful();
	}

	[Fact]
	public void Entities_ShouldHave_PrivateParameterlessConstructor()
	{
		IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly).That().Inherit(typeof(Entity)).GetTypes();

		var failingTypes = new List<Type>();
		foreach (var entityType in entityTypes)
		{
			var constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

			if (!constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
			{
				failingTypes.Add(entityType);
			}
		}

		failingTypes.ShouldBeEmpty();
	}

	[Fact]
	public void Entities_ShouldOnlyHave_PrivateConstructors()
	{
		IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly).That().Inherit(typeof(Entity)).GetTypes();

		var failingTypes = new List<Type>();
		foreach (var entityType in entityTypes)
		{
			var constructors = entityType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

			if (constructors.Any())
			{
				failingTypes.Add(entityType);
			}
		}

		failingTypes.ShouldBeEmpty();
	}
}