// DMX.
// Copyright (C) Eugene Bekker.

using DMX.WebUI3.Services;

namespace DMX.WebUI3;

public class AppServices
{
    public AppServices(
        EventsManager events,
        ModelManager model,
        BrowserStorageService browserStorage,
        PrefsService prefs)
    {
        Events = events;
        Model = model;
        BrowserStorage = browserStorage;
        Prefs = prefs;
    }

    public EventsManager Events { get; }

    public ModelManager Model { get; }

    public BrowserStorageService BrowserStorage { get; }

    public PrefsService Prefs { get; }
}
