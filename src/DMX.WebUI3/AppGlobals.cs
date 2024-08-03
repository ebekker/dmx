// DMX.
// Copyright (C) Eugene Bekker.

using System.Text.Json;

namespace DMX.WebUI3;

internal static class AppGlobals
{
    internal static readonly JsonSerializerOptions WriteIndentedOptions = new()
    {
        WriteIndented = true
    };
}
