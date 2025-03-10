﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Components;

/// <summary>
/// 
/// </summary>
public partial class PopConfirmButton
{
    private string? ClassString => CssBuilder.Default()
        .AddClass("disabled", IsDisabled)
        .AddClass(InternalClassName, IsLink)
        .AddClass(ClassName, !IsLink)
        .Build();

    private string? InternalClassName => CssBuilder.Default()
        .AddClass($"link-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// 获得/设置 按钮颜色
    /// </summary>
    [Parameter]
    public override Color Color { get; set; } = Color.None;

    [Inject]
    [NotNull]
    private IStringLocalizer<PopConfirmButton>? Localizer { get; set; }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];
        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        Content ??= Localizer[nameof(Content)];
    }

    /// <summary>
    /// 显示确认弹窗方法
    /// </summary>
    private async Task Show()
    {
        // 回调消费者逻辑 判断是否需要弹出确认框
        if (await OnBeforeClick())
        {
            if (Module != null)
            {
                await Module.InvokeVoidAsync($"{ModuleName}.execute", Id, "showConfirm");
            }
        }
    }

    /// <summary>
    /// 确认回调方法
    /// </summary>
    /// <returns></returns>
    private async Task Confirm()
    {
        if (IsAsync)
        {
            IsDisabled = true;
            ButtonIcon = LoadingIcon;
            StateHasChanged();
            await Task.Run(() => InvokeAsync(OnConfirm));

            if (ButtonType == ButtonType.Submit)
            {
                await TrySubmit();
            }
            else
            {
                IsDisabled = false;
                ButtonIcon = Icon;
                StateHasChanged();
            }
        }
        else
        {
            await OnConfirm();
            if (ButtonType == ButtonType.Submit)
            {
                await TrySubmit();
            }
        }
    }

    private async Task TrySubmit()
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync($"{ModuleName}.execute", Id, "submit");
        }
    }
}
