﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;

namespace BootstrapBlazor.Shared.Components;

/// <summary>
/// 广告组件
/// </summary>
[JSModuleAutoLoader("./_content/BootstrapBlazor.Shared/modules/wwads.js", ModuleName = "Wwads", Relative = false)]
public partial class Wwads
{
    private string? DebugString { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

#if DEBUG
        DebugString = "true";
#endif
    }
}
