namespace Client;

using System.Reflection;
using CommunityToolkit.Maui;
using Controls.WorldExplorerMap;
using Microsoft.Extensions.Configuration;
using Services;
using Services.API;
using Services.Auth;
using Services.Navigation;
using SimpleRatingControlMaui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using ViewModels;
using Views;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
#if DEBUG && WINDOWS
		builder.AddAppDefaults();
#endif
		var config = GetConfiguration();
		builder.Configuration.AddConfiguration(config);
		var apiSettings = builder.Configuration.GetRequiredSection("API").Get<ApiSettings>();
		ArgumentNullException.ThrowIfNull(apiSettings);

		builder.UseMauiApp<App>()
		       .ConfigureSyncfusionToolkit()
			   .UseMauiCommunityToolkitCamera()
			   .UseSkiaSharp()
			   .UseSimpleRatingControl()
			   .ConfigureFonts(fonts =>
			   {
				   fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
				   fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FABrands");
				   fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FARegular");
			   })
			   .ConfigureMauiHandlers(handlers =>
			   {
#if ANDROID || IOS
				   handlers.AddHandler<Shell, CustomShellHandler>();
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

		builder.Services.AddSingleton<INavigationService, NavigationService>();

#if IOS || MACCATALYST
		builder.Services.AddSingleton<IAuthService, AppleAuthService>();
#else
		builder.Services.AddSingleton<IAuthService, AuthService>();
#endif
		builder.Services.Configure<AzureB2CConfiguration>(configuration => builder.Configuration.GetRequiredSection("AzureAdB2C").Bind(configuration));
		builder.Services.AddSingleton<IArService, ArService>();
		builder.Services.AddSingleton<IDialogService, DialogService>();

		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton(DeviceInfo.Current);
		builder.Services.AddSingleton(DeviceDisplay.Current);
		builder.Services.AddSingleton(Share.Default);
		builder.Services.AddSingleton(Launcher.Default);
		builder.Services.AddSingleton(Geolocation.Default);
		builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
		builder.Services.AddApi<IPlacesApi>(apiSettings.Places);
		builder.Services.AddApi<IUsersApi>(apiSettings.Users);

		builder.Services.AddSingleton<AboutPage, AboutViewModel>();
		builder.Services.AddSingletonWithShellRoute<LoginPage, LoginViewModel>($"//{nameof(LoginPage)}");
		builder.Services.AddSingletonWithShellRoute<ProfilePage, ProfileViewModel>($"//home/{nameof(ProfilePage)}");
		builder.Services.AddTransientWithShellRoute<PlaceDetailsView, PlaceDetailsViewModel>($"//home/{nameof(PlaceDetailsView)}");
		builder.Services.AddSingletonWithShellRoute<ExplorerPage, ExplorerViewModel>($"//home/{nameof(ExplorerPage)}");
		builder.Services.AddSingletonWithShellRoute<LoadingPage, LoadingViewModel>($"//{nameof(LoadingPage)}");
		builder.Services.AddSingletonWithShellRoute<ErrorPage, ErrorViewModel>($"//{nameof(ErrorViewModel)}");
		builder.Services.AddTransientWithShellRoute<ArPage, ArViewModel>($"//home/{nameof(ArPage)}");
		builder.Services.AddTransientWithShellRoute<CameraPage, CameraViewModel>($"//home/{nameof(CameraPage)}");
		builder.Services.AddSingleton<MainViewModel>();
		return builder.Build();
	}

	private static IConfiguration GetConfiguration()
	{
		var builder = new ConfigurationBuilder();
		builder.AddConfiguration("Client.appsettings.json");
#if DEBUG
		builder.AddConfiguration("Client.appsettings.Development.json");
#if WINDOWS
		builder.AddInMemoryCollection(AspireAppSettings.Settings);
#endif
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