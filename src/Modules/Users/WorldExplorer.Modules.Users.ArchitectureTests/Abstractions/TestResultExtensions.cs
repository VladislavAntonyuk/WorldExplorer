namespace WorldExplorer.Modules.Users.ArchitectureTests.Abstractions;

using FluentAssertions;
using NetArchTest.Rules;

internal static class TestResultExtensions
{
	internal static void ShouldBeSuccessful(this TestResult testResult)
	{
		testResult.FailingTypes?.Should().BeEmpty();
	}
}