﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using BootstrapBlazor.Shared.Common;

namespace BootstrapBlazor.Shared.Samples;

/// <summary>
/// 
/// </summary>
public partial class Lights : IDisposable
{
    private Color Color { get; set; } = Color.Primary;

    private CancellationTokenSource UpdateColorTokenSource { get; } = new CancellationTokenSource();

    /// <summary>
    /// OnAfterRender
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await Task.Delay(2000, UpdateColorTokenSource.Token);
            Color = Color switch
            {
                Color.Primary => Color.Success,
                Color.Success => Color.Info,
                Color.Info => Color.Warning,
                Color.Warning => Color.Danger,
                Color.Danger => Color.Secondary,
                _ => Color.Primary
            };
            StateHasChanged();
        }
        catch (TaskCanceledException)
        {

        }
    }

    private static IEnumerable<AttributeItem> GetAttributes()
    {
        return new AttributeItem[]
        {
            new AttributeItem() {
                Name = "Color",
                Description = "Color",
                Type = "Color",
                ValueList = "None / Active / Primary / Secondary / Success / Danger / Warning / Info / Light / Dark / Link",
                DefaultValue = "Success"
            },
            new AttributeItem() {
                Name = "IsFlash",
                Description = "Is it flashing",
                Type = "boolean",
                ValueList = " — ",
                DefaultValue = "false"
            },
            new AttributeItem() {
                Name = "Title",
                Description = "Indicator Tooltip Display text",
                Type = "string",
                ValueList = " — ",
                DefaultValue = " — "
            }
        };
    }

    /// <summary>
    /// Dispose
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            UpdateColorTokenSource.Cancel();
            UpdateColorTokenSource.Dispose();
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
