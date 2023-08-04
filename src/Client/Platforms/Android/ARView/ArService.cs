namespace Client;

using Android.App;
using Google.AR.Core;
using Services;

public class ArService : IArService
{
	public bool IsSupported()
	{
		return ArCoreApk.Instance.CheckAvailability(Application.Context).IsSupported;
	}
}