namespace WebApp.Shared;

using global::Shared.Enums;
using global::Shared.Extensions;

public sealed partial class LanguageControl : WorldExplorerBaseComponent, IDisposable
{
	private Language currentLang;

	protected override Task OnInitializedAsync()
	{
		I18NText.ChangeLanguage += I18NText_ChangeLanguage;
		return base.OnInitializedAsync();
	}

	private void I18NText_ChangeLanguage(object? sender, Toolbelt.Blazor.I18nText.I18nTextChangeLanguageEventArgs e)
	{
		currentLang = e.LanguageCode.GetValueFromDescription<Language>();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
		{
			await I18NText.SetCurrentLanguageAsync(currentLang.GetDescription());
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	public void Dispose()
	{
		I18NText.ChangeLanguage -= I18NText_ChangeLanguage;
	}
}