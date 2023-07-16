﻿using System.Linq;
using System.Windows;
using System.Windows.Media;
using Shared.Interfaces;
using Wpf.Ui.Appearance;

namespace uWidgets.Providers;

public class WpfUiThemeProvider : IThemeProvider
{
    private readonly IAppSettingsProvider appSettingsProvider;

    private static readonly SystemThemeType[] DarkThemes =
    {
        SystemThemeType.Glow,
        SystemThemeType.Dark,
        SystemThemeType.CapturedMotion
    };
    
    public WpfUiThemeProvider(IAppSettingsProvider appSettingsProvider)
    {
        Application.Current.MainWindow = new Window();
        
        this.appSettingsProvider = appSettingsProvider;
        this.appSettingsProvider.Updated += (_, _) => Apply();
    }
    
    public void Apply()
    {
        Accent.Apply(GetAccentColor());
        Theme.Apply(GetThemeType(), BackgroundType.Acrylic);
    }

    private ThemeType GetThemeType()
    {
        var darkTheme = appSettingsProvider.Get().Appearance.UseSystemTheme
            ? DarkThemes.Contains(Theme.GetSystemTheme())
            : appSettingsProvider.Get().Appearance.DarkTheme;

        return darkTheme
            ? ThemeType.Dark
            : ThemeType.Light;
    }

    private Color GetAccentColor()
    {
        if (appSettingsProvider.Get().Appearance.UseSystemAccentColor)
            return Accent.GetColorizationColor();
        
        return (Color) ColorConverter.ConvertFromString(appSettingsProvider.Get().Appearance.AccentColor);
    }
}