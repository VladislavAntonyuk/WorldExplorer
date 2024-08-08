namespace WorldExplorer.Web.Components;

using Common.Infrastructure;
using Modules.Users.Application.Abstractions.Identity;
using Toolbelt.Blazor.I18nText;

public sealed partial class LanguageControl : WorldExplorerBaseComponent, IDisposable
{
	private Language currentLang;

	protected override Task OnInitializedAsync()
	{
		I18NText.ChangeLanguage += I18NText_ChangeLanguage;
		return base.OnInitializedAsync();
	}

	private void I18NText_ChangeLanguage(object? sender, I18nTextChangeLanguageEventArgs e)
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