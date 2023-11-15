namespace Client;

using System.Reflection;
using Camera.MAUI;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using Microsoft.Extensions.Configuration;
using Services;
using Services.API;
using Services.Auth;
using Services.Navigation;
using SkiaSharp.Views.Maui.Controls.Hosting;
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
			   .UseSkiaSharp()
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
				   handlers.AddHandler<Controls.ArView, ArViewHandler>();
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

#if IOS || MACCATALYST
		builder.Services.AddSingleton<IAuthService, MockAuthService>();
#else
		builder.Services.AddSingleton<IAuthService, AuthService>();
#endif
		builder.Services.Configure<AzureB2CConfiguration>(configuration => builder.Configuration.GetRequiredSection("AzureAdB2C").Bind(configuration));
		builder.Services.AddSingleton<IArService, ArService>();
		builder.Services.AddSingleton<IDialogService, DialogService>();

		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton(_ => DeviceInfo.Current);
		builder.Services.AddSingleton(_ => DeviceDisplay.Current);
		builder.Services.AddSingleton(_ => Share.Default);
		builder.Services.AddSingleton(_ => Launcher.Default);
		builder.Services.AddSingleton<IGeolocator, Services.GeolocatorImplementation>();
		builder.Services.AddTransient<AuthHeaderHandler>();
		builder.Services.AddApi<IPlacesApi>(apiSettings.Places);
		builder.Services.AddApi<IUsersApi>(apiSettings.Users);

		builder.Services.AddSingleton<ProfilePage, ProfileViewModel>();
		builder.Services.AddSingleton<PlaceDetailsView, PlaceDetailsViewModel>();
		builder.Services.AddSingleton<LoginPage, LoginViewModel>();
		builder.Services.AddSingleton<ExplorerPage, ExplorerViewModel>();
		builder.Services.AddSingleton<LoadingPage, LoadingViewModel>();
		builder.Services.AddSingleton<ErrorPage, ErrorViewModel>();
		builder.Services.AddTransientWithShellRoute<ArPage, ArViewModel>($"//home/{nameof(ArPage)}");
		builder.Services.AddTransientWithShellRoute<CameraPage, CameraViewModel>($"//home/{nameof(CameraPage)}");
		builder.Services.AddSingleton<ShellViewModel>();

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