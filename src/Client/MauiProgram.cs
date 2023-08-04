namespace Client;

using System.Reflection;
using Camera.MAUI;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;
using Services.API;
using Services.Auth;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;
using ViewModels;
using Views;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		var config = GetConfiguration();
		builder.Configuration.AddConfiguration(config);
		var keysSettings = builder.Configuration.GetRequiredSection("Keys").Get<KeysSettings>();
		var apiSettings = builder.Configuration.GetRequiredSection("API").Get<ApiSettings>();
		ArgumentNullException.ThrowIfNull(apiSettings);
		ArgumentNullException.ThrowIfNull(keysSettings);

		builder.UseMauiApp<App>()
		       .UseMauiCommunityToolkitMaps(keysSettings.WindowsMaps)
			   .UseMauiCameraView()
			   .ConfigureFonts(fonts =>
		       {
			       fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
			       fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FABrands");
			       fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FARegular");
		       })
		       .ConfigureMauiHandlers(handlers =>
		       {
			       handlers.AddHandler<Shell, CustomShellHandler>();
#if ANDROID || IOS
			       handlers.AddHandler<ArView, ArViewHandler>();
#endif
		       });

		builder.UseMauiCommunityToolkit(x =>
		{
#if !DEBUG
			x.SetShouldSuppressExceptionsInConverters(true);
			x.SetShouldSuppressExceptionsInBehaviors(true);
			x.SetShouldSuppressExceptionsInAnimations(true);
#endif
		});

		SyncfusionLicenseProvider.RegisterLicense(keysSettings.Syncfusion);
		builder.ConfigureSyncfusionCore();

		builder.Services.AddSingleton<INavigationService, NavigationService>();
#if DEBUG
		builder.Services.AddSingleton<IAuthService, MockAuthService>();
#else
		builder.Services.AddSingleton<IAuthService, AuthService>();
#endif
		builder.Services.AddSingleton<IArService, ArService>();
		builder.Services.AddSingleton<IDialogService, DialogService>();

		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddScoped(_ => DeviceInfo.Current);
		builder.Services.AddScoped(_ => DeviceDisplay.Current);
		builder.Services.AddScoped(_ => Share.Default);
		builder.Services.AddScoped(_ => Launcher.Default);
		builder.Services.AddScoped<IGeolocator, GeolocatorImplementation>();
		builder.Services.AddTransient<AuthHeaderHandler>();
		builder.Services.AddApi<IPlacesApi>(apiSettings.Places);
		builder.Services.AddApi<IUsersApi>(apiSettings.Users);

		builder.Services.AddScoped<ProfilePage, ProfileViewModel>();
		builder.Services.AddScoped<PlaceDetailsView, PlaceDetailsViewModel>();
		builder.Services.AddScoped<LoginPage, LoginViewModel>();
		builder.Services.AddScoped<ExplorerPage, ExplorerViewModel>();
		builder.Services.AddScoped<LoadingPage, LoadingViewModel>();
		builder.Services.AddScoped<ErrorPage, ErrorViewModel>();
		builder.Services.AddTransientWithShellRoute<ArPage, ArViewModel>($"//home/{nameof(ArPage)}");
		builder.Services.AddTransientWithShellRoute<CameraPage, CameraViewModel>($"//home/{nameof(CameraPage)}");
		builder.Services.AddSingleton<ShellViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static IConfiguration GetConfiguration()
	{
		var builder = new ConfigurationBuilder();
		builder.AddConfiguration("Client.appsettings.json");
#if DEBUG
		builder.AddConfiguration("Client.appsettings.Development.json");
#endif
		return builder.Build();
	}

	private static void AddConfiguration(this IConfigurationBuilder configurationBuilder, string resourceName)
	{
		var builder = new ConfigurationBuilder();
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream != null)
		{
			builder.AddJsonStream(stream);
		}

		configurationBuilder.AddConfiguration(builder.Build());
	}
}