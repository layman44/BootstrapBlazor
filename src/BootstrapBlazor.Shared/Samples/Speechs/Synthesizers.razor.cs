﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Shared.Samples;

/// <summary>
/// 
/// </summary>
public partial class Synthesizers
{
    [Inject]
    [NotNull]
    private SynthesizerService? SynthesizerService { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Synthesizers>? Localizer { get; set; }

    private bool Start { get; set; }

    private string? InputText { get; set; }

    private string ButtonText { get; set; } = "开始合成";

    private async Task OnStart()
    {
        if (ButtonText == "开始合成")
        {
            if (!string.IsNullOrEmpty(InputText))
            {
                await SynthesizerService.SynthesizerOnceAsync(InputText, Synthesizer);
            }
        }
        else
        {
            await SynthesizerService.CloseAsync(Synthesizer);
        }
    }

    private Task Synthesizer(SynthesizerStatus status)
    {
        if (status == SynthesizerStatus.Synthesizer)
        {
            Start = true;
            ButtonText = "停止合成";
        }
        else
        {
            Start = false;
            ButtonText = "开始合成";
        }
        StateHasChanged();
        return Task.CompletedTask;
    }
}
