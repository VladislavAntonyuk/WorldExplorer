namespace Client;

using System.Reflection;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Maui.Handlers;
using Services;
using Services.API;
using Services.Auth;
using Services.Navigation;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using ViewModels;
using Views;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		var config = GetConfiguration();
		builder.Configuration.AddConfiguration(config);
		var urlsSettingsSection = builder.Configuration.GetRequiredSection("Urls");
		builder.Services.Configure<UrlsSettings>(urlsSettingsSection);

		builder.UseMauiApp<App>()
			   .ConfigureSyncfusionToolkit()
			   .UseMauiCommunityToolkitCamera()
			   .UseSkiaSharp()
			   .ConfigureMauiHandlers(handlers =>
			   {
#if ANDROID || IOS
				   handlers.AddHandler<Shell, CustomShellHandler>();
				   handlers.AddHandler<Controls.ArView, ArViewHandler>();
#endif
#if IOS || MACCATALYST
				   handlers.AddHandler<CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
				   handlers.AddHandler<CarouselView, Microsoft.Maui.Controls.Handlers.Items2.CarouselViewHandler2>();
#endif
			   })
			   .ConfigureFonts(fonts =>
			   {
				   fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
				   fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FABrands");
				   fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FARegular");
			   });

		HybridWebViewHandler.Mapper.AppendToMapping("WorldExplorerMap", static
#if WINDOWS
														async
#endif
			                                            (handler, _) =>
		{
#if ANDROID
			handler.PlatformView.SetWebViewClient(new WorldExplorerMapWebViewClient((HybridWebViewHandler)handler));
#elif IOS || MACCATALYST
			handler.PlatformView.AllowsBackForwardNavigationGestures = false;
			handler.PlatformView.AllowsLinkPreview = false;
#elif WINDOWS
			handler.PlatformView.CanGoBack = false;
			handler.PlatformView.CanGoForward = false;
			handler.PlatformView.IsRightTapEnabled = false;
			handler.PlatformView.AllowDrop = false;
			handler.PlatformView.CanDrag = false;
			handler.PlatformView.IsDoubleTapEnabled = false;
			handler.PlatformView.IsHoldingEnabled = false;
			await handler.PlatformView.EnsureCoreWebView2Async();
			handler.PlatformView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
			handler.PlatformView.CoreWebView2.Settings.AreDevToolsEnabled = false;
			handler.PlatformView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
			handler.PlatformView.NavigationStarting += (_, e) =>
			{
				e.Cancel = e.IsUserInitiated && e.Uri != "https://0.0.0.1/";
			};
#endif
		});

		builder.UseMauiCommunityToolkit(
#if !DEBUG
			x =>
		{
			x.SetShouldSuppressExceptionsInConverters(true);
			x.SetShouldSuppressExceptionsInBehaviors(true);
			x.SetShouldSuppressExceptionsInAnimations(true);
		}
#endif
			);

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
		builder.Services.AddApi<IPlacesApi>(s =>
		{
			var urlsSettings = s.GetRequiredService<IOptions<UrlsSettings>>().Value;
			return $"{urlsSettings.Api}/places";
		});
		builder.Services.AddApi<IUsersApi>(s =>
		{
			var urlsSettings = s.GetRequiredService<IOptions<UrlsSettings>>().Value;
			return $"{urlsSettings.Api}/users";
		});
		builder.Services.AddTransient<MicrosoftIdentityUserAuthenticationMessageHandler>();
		builder.Services.AddWorldExplorerTravellersClient()
		        .ConfigureHttpClient(
					(serviceProvider, client) =>
					{
						var urlsSettings = serviceProvider.GetRequiredService<IOptions<UrlsSettings>>().Value;
						client.BaseAddress = new Uri($"{urlsSettings.Api}/graphql");
					},
					clientBuilder =>
					{
						clientBuilder.AddHttpMessageHandler<MicrosoftIdentityUserAuthenticationMessageHandler>();
					});
		
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