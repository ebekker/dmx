// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3.Components;

public class DetailsResult : IEquatable<DetailsResult>
{
    public static readonly DetailsResult OK = new(nameof(OK));
    public static readonly DetailsResult Cancel = new(nameof(Cancel));
    public static readonly DetailsResult Delete = new(nameof(Delete));
    public static readonly DetailsResult Refresh = new(nameof(Refresh));

    private readonly string _key;

    private DetailsResult(string key)
    {
        _key = key;
    }

    public override string ToString() => $"DetailsResult({_key})";

    public override int GetHashCode() => _key.GetHashCode();

    public override bool Equals(object? obj) =>
        obj is DetailsResult && Equals((DetailsResult)obj);

    public bool Equals(DetailsResult? other) => string.Equals(_key, other?._key);
}
