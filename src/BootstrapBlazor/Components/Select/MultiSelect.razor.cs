﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.Localization;
using System.Collections;

namespace BootstrapBlazor.Components;

/// <summary>
/// MultiSelect 组件
/// </summary>
[JSModuleAutoLoader("multi-select", JSObjectReference = true)]
public partial class MultiSelect<TValue>
{
    [NotNull]
    private List<SelectedItem>? DataSource { get; set; }

    private IEnumerable<SelectedItem> SelectedItems => DataSource.Where(i => i.Active);

    private static string? ClassString => CssBuilder.Default("select dropdown multi-select")
        .Build();

    private string? ToggleClassString => CssBuilder.Default("dropdown-toggle")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled)
        .AddClass("disabled", IsDisabled)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private string? GetItemClassString(SelectedItem item) => CssBuilder.Default("dropdown-item")
        .AddClass("active", GetCheckedState(item))
        .Build();

    private string? PlaceHolderClassString => CssBuilder.Default("multi-select-ph")
        .AddClass("d-none", SelectedItems.Any())
        .Build();

    /// <summary>
    /// 获得/设置 组件 PlaceHolder 文字 默认为 点击进行多选 ...
    /// </summary>
    [Parameter]
    [NotNull]
    public string? PlaceHolder { get; set; }

    /// <summary>
    /// 获得/设置 是否显示关闭按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示功能按钮 默认为 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; }

    /// <summary>
    /// 获得/设置 是否显示默认功能按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowDefaultButtons { get; set; } = true;

    /// <summary>
    /// 获得/设置 扩展按钮模板
    /// </summary>
    [Parameter]
    public RenderFragment? ButtonTemplate { get; set; }

    /// <summary>
    /// 获得/设置 搜索文本发生变化时回调此方法
    /// </summary>
    [Parameter]
    public Func<string, IEnumerable<SelectedItem>>? OnSearchTextChanged { get; set; }

    /// <summary>
    /// 获得/设置 选中项集合发生改变时回调委托方法
    /// </summary>
    [Parameter]
    public Func<IEnumerable<SelectedItem>, Task>? OnSelectedItemsChanged { get; set; }

    /// <summary>
    /// 获得/设置 全选按钮显示文本
    /// </summary>
    [Parameter]
    [NotNull]
    public string? SelectAllText { get; set; }

    /// <summary>
    /// 获得/设置 全选按钮显示文本
    /// </summary>
    [Parameter]
    [NotNull]
    public string? ReverseSelectText { get; set; }

    /// <summary>
    /// 获得/设置 全选按钮显示文本
    /// </summary>
    [Parameter]
    [NotNull]
    public string? ClearText { get; set; }

    /// <summary>
    /// 获得/设置 选项最大数 默认为 0 不限制
    /// </summary>
    [Parameter]
    public int Max { get; set; }

    /// <summary>
    /// 获得/设置 设置最大值时错误消息文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? MaxErrorMessage { get; set; }

    /// <summary>
    /// 获得/设置 选项最小数 默认为 0 不限制
    /// </summary>
    [Parameter]
    public int Min { get; set; }

    /// <summary>
    /// 获得/设置 设置最小值时错误消息文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? MinErrorMessage { get; set; }

    /// <summary>
    /// 获得/设置 设置清除图标 默认 fa-solid fa-xmark
    /// </summary>
    [Parameter]
    [NotNull]
    public string? ClearIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<MultiSelect<TValue>>? Localizer { get; set; }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PlaceHolder ??= Localizer[nameof(PlaceHolder)];
        SelectAllText ??= Localizer[nameof(SelectAllText)];
        ReverseSelectText ??= Localizer[nameof(ReverseSelectText)];
        ClearText ??= Localizer[nameof(ClearText)];
        MinErrorMessage ??= Localizer[nameof(MinErrorMessage)];
        MaxErrorMessage ??= Localizer[nameof(MaxErrorMessage)];

        ClearIcon ??= "fa-solid fa-xmark";

        ResetItems();

        OnSearchTextChanged ??= text => Items.Where(i => i.Text.Contains(text, StringComparison.OrdinalIgnoreCase));

        ResetRules();
    }

    /// <summary>
    /// OnParametersSetAsync 方法
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // 通过 Value 对集合进行赋值
        var list = CurrentValueAsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in Items)
        {
            item.Active = list.Any(i => i.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override async Task ModuleInitAsync()
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync($"{ModuleName}.init", Id, nameof(ToggleRow));
        }
    }

    /// <summary>
    /// FormatValueAsString 方法
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override string? FormatValueAsString(TValue value) => value == null
        ? null
        : Utility.ConvertValueToString(value);

    /// <summary>
    /// 切换当前选项方法
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task ToggleRow(string val)
    {
        if (!IsDisabled)
        {
            var d = DataSource.FirstOrDefault(i => i.Value == val);
            if (d != null)
            {
                d.Active = !d.Active;
            }

            // 更新选中值
            await SetValue();

            await TriggerSelectedItemChanged();

            StateHasChanged();
        }
    }

    private string? GetValueString(SelectedItem item) => IsPopover ? item.Value : null;

    private async Task TriggerSelectedItemChanged()
    {
        if (OnSelectedItemsChanged != null)
        {
            await OnSelectedItemsChanged.Invoke(SelectedItems);
        }
    }

    private int _min;
    private int _max;
    private void ResetRules()
    {
        if (Max != _max)
        {
            _max = Max;
            Rules.RemoveAll(v => v is MaxValidator);

            if (Max > 0)
            {
                Rules.Add(new MaxValidator() { Value = Max, ErrorMessage = MaxErrorMessage });
            }
        }

        if (Min != _min)
        {
            _min = Min;
            Rules.RemoveAll(v => v is MinValidator);

            if (Min > 0)
            {
                Rules.Add(new MinValidator() { Value = Min, ErrorMessage = MinErrorMessage });
            }
        }
    }

    private async Task SetValue()
    {
        var typeValue = NullableUnderlyingType ?? typeof(TValue);
        if (typeValue == typeof(string))
        {
            CurrentValueAsString = string.Join(",", SelectedItems.Select(i => i.Value));
        }
        else if (typeValue.IsGenericType || typeValue.IsArray)
        {
            var t = typeValue.IsGenericType ? typeValue.GenericTypeArguments[0] : typeValue.GetElementType()!;
            var listType = typeof(List<>).MakeGenericType(t);
            var instance = (IList)Activator.CreateInstance(listType, SelectedItems.Count())!;

            foreach (var item in SelectedItems)
            {
                if (item.Value.TryConvertTo(t, out var val))
                {
                    instance.Add(val);
                }
            }
            CurrentValue = (TValue)(typeValue.IsGenericType ? instance : listType.GetMethod("ToArray")!.Invoke(instance, null)!);
        }

        if (ValidateForm == null && (Min > 0 || Max > 0))
        {
            var validationContext = new ValidationContext(Value!) { MemberName = FieldIdentifier?.FieldName };
            var validationResults = new List<ValidationResult>();

            await ValidatePropertyAsync(CurrentValue, validationContext, validationResults);
            ToggleMessage(validationResults, true);
        }
    }

    private async Task Clear()
    {
        foreach (var item in Items)
        {
            item.Active = false;
        }

        await SetValue();

        await TriggerSelectedItemChanged();
    }

    private async Task SelectAll()
    {
        foreach (var item in GetData())
        {
            item.Active = true;
        }

        await SetValue();

        await TriggerSelectedItemChanged();
    }

    private async Task InvertSelect()
    {
        foreach (var item in GetData())
        {
            item.Active = !item.Active;
        }

        await SetValue();

        await TriggerSelectedItemChanged();
    }

    private bool GetCheckedState(SelectedItem item) => SelectedItems.Any(i => i.Value == item.Value);

    private bool CheckCanTrigger(SelectedItem item)
    {
        var ret = true;
        if (Max > 0)
        {
            ret = SelectedItems.Count() < Max || GetCheckedState(item);
        }
        return ret;
    }

    private bool CheckCanSelect(SelectedItem item)
    {
        var ret = GetCheckedState(item);
        if (!ret)
        {
            ret = CheckCanTrigger(item);
        }
        return !ret;
    }

    private IEnumerable<SelectedItem> GetData()
    {
        var data = Items;
        if (ShowSearch && !string.IsNullOrEmpty(SearchText) && OnSearchTextChanged != null)
        {
            data = OnSearchTextChanged(SearchText).ToList();
        }
        return data;
    }

    /// <summary>
    /// 客户端检查完成时调用此方法
    /// </summary>
    /// <param name="valid"></param>
    protected override void OnValidate(bool? valid)
    {
        if (valid != null)
        {
            Color = valid.Value ? Color.Success : Color.Danger;
        }
    }

    private void ResetItems()
    {
        if (Items == null)
        {
            // 判断 IEnumerable<T> 泛型 T 是否为 Enum
            // 特别注意 string 是 IEnumerable 的实例
            var type = typeof(TValue);
            Type? innerType;
            if (type.IsGenericType && type.IsAssignableTo(typeof(IEnumerable)))
            {
                innerType = type.GetGenericArguments()[0];
            }
            else
            {
                innerType = NullableUnderlyingType ?? type;
            }
            if (innerType.IsEnum)
            {
                Items = innerType.ToSelectList();
            }
            else
            {
                Items = Enumerable.Empty<SelectedItem>();
            }
        }

        DataSource = Items.ToList();
    }
}
