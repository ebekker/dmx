// DMX.
// Copyright (C) Eugene Bekker.

using System.Text.Json;

namespace DMX.WebUI3.Services;

public class PrefsService
{
    private readonly ILogger<PrefsService> _logger;
    private readonly BrowserStorageService _storage;

    public PrefsService(ILogger<PrefsService> logger,
        BrowserStorageService storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public async Task Save<TPrefs>(string key, TPrefs prefs)
    {
        var json = JsonSerializer.Serialize(prefs);
        await _storage.LocalStorage.SetItem(key, json);
    }

    public async Task<TPrefs?> Load<TPrefs>(string key)
        where TPrefs : new()
    {
        var json = await _storage.LocalStorage.GetItem(key);
        if (json == null)
            return default;
        var prefs = JsonSerializer.Deserialize<TPrefs>(json);
        return prefs;
    }

    public async Task Save(Guid id, PerspectivePrefs prefs)
    {
        var key = $"prsp_{id}";
        await Save(key, prefs);
    }

    public async Task<PerspectivePrefs> LoadPerspective(Guid id)
    {
        var key = $"prsp_{id}";
        var prefs = await Load<PerspectivePrefs>(key);
        if (prefs == null)
        {
            prefs = new();
            await Save(key, prefs);
        }
        return prefs;
    }

    public class PerspectivePrefs
    {
        public double PanX { get; set; } = 0.0;
        public double PanY { get; set; } = 0.0;
        public double Zoom { get; set; } = 1.0;
        public bool OriginMarkerEnabled { get; set; } = true;
    }
}
