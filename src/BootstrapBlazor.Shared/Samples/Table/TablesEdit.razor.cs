﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using BootstrapBlazor.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace BootstrapBlazor.Shared.Samples.Table;

/// <summary>
/// 表单编辑功能示例
/// </summary>
public partial class TablesEdit
{
    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? LocalizerFoo { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TablesEdit>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<WebsiteOptions>? WebsiteOption { get; set; }

    private static IEnumerable<int> PageItemsSource => new int[] { 4, 10, 20 };

    [NotNull]
    private IEnumerable<SelectedItem>? Hobbys { get; set; }

    [NotNull]
    private List<Foo>? Items { get; set; }

    [NotNull]
    private IEnumerable<Foo>? BindItems { get; set; }

    private InsertRowMode InsertMode { get; set; } = InsertRowMode.Last;

    private string DataServiceUrl => $"{WebsiteOption.CurrentValue.BootstrapBlazorLink}/wikis/Table%20%E7%BB%84%E4%BB%B6%E6%95%B0%E6%8D%AE%E6%9C%8D%E5%8A%A1%E4%BB%8B%E7%BB%8D?sort_id=3207977";

    private string? PlaceHolderString { get; set; }

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Hobbys = Foo.GenerateHobbys(LocalizerFoo);
        Items = Foo.GenerateFoo(LocalizerFoo);

        BindItems = Foo.GenerateFoo(LocalizerFoo).Take(5).ToList();

        CustomerDataService = new FooDataService<Foo>(LocalizerFoo);

        PlaceHolderString ??= Localizer["P4"];
    }

    private static Task<Foo> OnAddAsync() => Task.FromResult(new Foo() { DateTime = DateTime.Now, Address = $"Custom address  {DateTime.Now.Second}" });

    private Task<bool> OnSaveAsync(Foo item, ItemChangedType changedType)
    {
        if (changedType == ItemChangedType.Add)
        {
            item.Id = Items.Max(i => i.Id) + 1;
            Items.Add(item);
        }
        else
        {
            var oldItem = Items.FirstOrDefault(i => i.Id == item.Id);
            if (oldItem != null)
            {
                oldItem.Name = item.Name;
                oldItem.Address = item.Address;
                oldItem.DateTime = item.DateTime;
                oldItem.Count = item.Count;
                oldItem.Complete = item.Complete;
                oldItem.Education = item.Education;
            }
        }
        return Task.FromResult(true);
    }

    private Task<bool> OnDeleteAsync(IEnumerable<Foo> items)
    {
        items.ToList().ForEach(i => Items.Remove(i));
        return Task.FromResult(true);
    }

    private Task<QueryData<Foo>> OnQueryAsync(QueryPageOptions options)
    {
        IEnumerable<Foo> items = Items;

        // 过滤
        var isFiltered = false;
        if (options.Filters.Any())
        {
            items = items.Where(options.Filters.GetFilterFunc<Foo>());
            isFiltered = true;
        }

        // 排序
        var isSorted = false;
        if (!string.IsNullOrEmpty(options.SortName))
        {
            var invoker = Foo.GetNameSortFunc();
            items = invoker(items, options.SortName, options.SortOrder);
            isSorted = true;
        }

        // 设置记录总数
        var total = items.Count();

        // 内存分页
        items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList();

        return Task.FromResult(new QueryData<Foo>()
        {
            Items = items,
            TotalCount = total,
            IsSorted = isSorted,
            IsFiltered = isFiltered,
            IsSearch = true
        });
    }

    [NotNull]
    private IDataService<Foo>? CustomerDataService { get; set; }

    private class FooDataService<TModel> : TableDemoDataService<TModel> where TModel : class, new()
    {
        public FooDataService(IStringLocalizer<TModel> localizer) : base(localizer)
        {

        }
    }
}
