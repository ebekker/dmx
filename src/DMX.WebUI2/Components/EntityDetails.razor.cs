// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace DMX.WebUI2.Components;

public partial class EntityDetails
{
    [Parameter]
    public DmxEntity Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog? Dialog { get; set; }
}
