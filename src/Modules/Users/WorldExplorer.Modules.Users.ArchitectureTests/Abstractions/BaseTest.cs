namespace WorldExplorer.Modules.Users.ArchitectureTests.Abstractions;

using System.Reflection;
using Infrastructure;
using Users.Application;
using Users.Domain.Users;

public abstract class BaseTest
{
	protected static readonly Assembly ApplicationAssembly = typeof(AssemblyReference).Assembly;

	protected static readonly Assembly DomainAssembly = typeof(User).Assembly;

	protected static readonly Assembly InfrastructureAssembly = typeof(UsersModule).Assembly;

	protected static readonly Assembly PresentationAssembly = typeof(Users.Presentation.AssemblyReference).Assembly;
}