namespace Client;

using Services;

public class ArService : IArService
{
	public bool IsSupported()
	{
		return true;
	}
}