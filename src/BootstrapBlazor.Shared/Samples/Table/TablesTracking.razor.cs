﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Shared.Samples.Table;

/// <summary>
/// Tracking 模式示例代码
/// </summary>
public partial class TablesTracking
{
    [NotNull]
    private IEnumerable<Foo>? Items { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? LocalizerFoo { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TablesTracking>? Localizer { get; set; }

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items = Foo.GenerateFoo(LocalizerFoo, 4);
    }
}
