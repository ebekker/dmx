// DMX.
// Copyright (C) Eugene Bekker.

using System.Drawing;
using DPoint = System.Drawing.Point;

namespace DMX.WebUI3;

public class AppState
{
    public DPoint? EntityDetailsPoint { get; set; }
    public Size EntityDetailsSize { get; set; } = new(500, 500);
}
