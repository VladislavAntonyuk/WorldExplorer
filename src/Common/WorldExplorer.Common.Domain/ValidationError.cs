namespace WorldExplorer.Common.Domain;

public sealed record ValidationError(Error[] Errors) : Error("General.Validation",
                                                             "One or more validation errors occurred",
                                                             ErrorType.Validation)
{
	public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
