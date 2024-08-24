namespace WorldExplorer.Modules.Places.Application.Abstractions.Data;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}