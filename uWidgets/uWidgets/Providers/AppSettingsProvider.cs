﻿using Shared.Interfaces;
using Shared.Models;
using Shared.Services;
using uWidgets.Services;

namespace uWidgets.Providers;

public class AppSettingsProvider : IAppSettingsProvider
{
    private readonly JsonFileParser<AppSettings> jsonFileParser = new(PathBuilder.AppSettingsFile);
    
    public AppSettings Get() => jsonFileParser.Read();

    public void Update(AppSettings newData) => jsonFileParser.Write(newData);
}