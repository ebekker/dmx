// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;

namespace DMX.WebUI3.Services;

public class BrowserStorageService
{
    private readonly ILogger<BrowserStorageService> _logger;
    private readonly IJSRuntime _js;

    public BrowserStorageService(ILogger<BrowserStorageService> logger,
        IJSRuntime js)
    {
        _logger = logger;
        _js = js;

        SessionStorage = new BrowserStorage(_logger, js, "window.sessionStorage");
        LocalStorage = new BrowserStorage(_logger, js, "window.localStorage");
    }

    public IBrowserStorage SessionStorage { get; }
    public IBrowserStorage LocalStorage { get; }

    private class BrowserStorage : IBrowserStorage
    {
        private readonly ILogger _logger;
        private readonly IJSRuntime _js;
        private readonly object _root;

        public BrowserStorage(ILogger logger, IJSRuntime js, string root)
        {
            _logger = logger;
            _js = js;
            _root = root;
        }

        public async Task SetItem<T>(string key, T? value)
        {
            await _js.InvokeVoidAsync(
                $"{_root}.setItem",
                key, value);
        }

        public async Task SetItem(string key, string value)
        {
            await _js.InvokeVoidAsync(
                $"{_root}.setItem",
                key, value);
        }

        public async Task<string?> GetItem(string key)
        {
            return await _js.InvokeAsync<string>(
                $"{_root}.getItem",
                key);
        }

        public async Task<T?> GetItem<T>(string key)
        {
            return await _js.InvokeAsync<T?>(
                $"{_root}.getItem",
                key);
        }

        public async Task<string?> Key(int index)
        {
            return await _js.InvokeAsync<string>(
                $"{_root}.key",
                index);
        }

        public async Task RemoveItem(string key, string value)
        {
            await _js.InvokeVoidAsync(
                $"{_root}.removeItem",
                key, value);
        }

        public async Task Clear()
        {
            await _js.InvokeVoidAsync(
                $"{_root}.clear");
        }
    }
}

public interface IBrowserStorage
{
    Task SetItem(string key, string value);
    Task SetItem<T>(string key, T? value);
    Task<string?> GetItem(string key);
    Task<T?> GetItem<T>(string key);
    Task<string?> Key(int index);
    Task RemoveItem(string key, string value);
    Task Clear();
}
