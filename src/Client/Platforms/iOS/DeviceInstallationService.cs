namespace Client;

using System.Threading.Tasks;

public static partial class DeviceInstallationService
{
	private static Task<DeviceInstallation?> GetDeviceInstallation()
	{
		return Task.FromResult<DeviceInstallation?>(null);
	}
}