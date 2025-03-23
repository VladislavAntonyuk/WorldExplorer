namespace WorldExplorer.Modules.Users.ArchitectureTests.Abstractions;

using NetArchTest.Rules;
using Shouldly;

internal static class TestResultExtensions
{
	internal static void ShouldBeSuccessful(this TestResult testResult)
	{
		testResult.FailingTypes?.ShouldBeEmpty();
	}
}