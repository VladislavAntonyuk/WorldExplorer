namespace Client.Services;

using Android.App;
using Google.AR.Core;

public class ArService : IArService
{
	public bool IsSupported()
	{
		return ArCoreApk.Instance.CheckAvailability(Application.Context).IsSupported;
	}
}