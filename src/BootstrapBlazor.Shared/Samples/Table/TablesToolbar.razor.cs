﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Shared.Samples.Table;

/// <summary>
/// 
/// </summary>
public sealed partial class TablesToolbar
{
    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? LocalizerFoo { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TablesToolbar>? Localizer { get; set; }

    private static IEnumerable<int> PageItemsSource => new int[] { 2, 4, 10, 20 };

    [NotNull]
    private List<Foo>? Items { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items = Foo.GenerateFoo(LocalizerFoo);
    }

    private Task<QueryData<Foo>> OnQueryAsync(QueryPageOptions options)
    {
        // Set the total number of records
        var total = Items.Count;

        // memory paging
        var items = Items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList();

        return Task.FromResult(new QueryData<Foo>()
        {
            Items = items,
            TotalCount = total,
            IsSorted = true,
            IsFiltered = true,
            IsSearch = true
        });
    }

    private Task<QueryData<Foo>> OnSearchQueryAsync(QueryPageOptions options)
    {
        var items = Items.AsEnumerable();
        if (!string.IsNullOrEmpty(options.SearchText))
        {
            // Fuzzy query against SearchText
            items = items.Where(i => (i.Address ?? "").Contains(options.SearchText)
                    || (i.Name ?? "").Contains(options.SearchText));
        }

        // Set the total number of records
        var total = items.Count();

        // memory paging
        items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList();

        return Task.FromResult(new QueryData<Foo>()
        {
            Items = items,
            TotalCount = total,
            IsSorted = true,
            IsFiltered = true,
            IsSearch = true
        });
    }

    private Task<bool> OnDeleteAsync(IEnumerable<Foo> items)
    {
        items.ToList().ForEach(foo => Items.Remove(foo));
        return Task.FromResult(true);
    }

    private async Task DownloadAsync(IEnumerable<Foo> items)
    {
        // Construct pop-up window configuration information and perform pop-up window operations
        var cate = ToastCategory.Information;
        var title = "Custom download example";
        var content = "Please select the data first, then click the download button";
        if (items.Any())
        {
            cate = ToastCategory.Success;
            content = $"start packing selected {items.Count()} data, this window will be closed automatically after completion";
        }

        var option = new ToastOption()
        {
            Category = cate,
            Title = title,
            Content = content,
        };

        // 弹出 Toast
        await ToastService.Show(option);

        // If the download item is selected for package download operation
        if (items.Any())
        {
            // Disable automatic shutdown
            option.IsAutoHide = false;

            // Start a background process for data processing
            // Passing Option used to be used to close the popup after the asynchronous operation
            await MockDownLoadAsync();

            // Close the popup associated with the option
            await option.Close();

            // A pop-up window informs that the download is complete
            await ToastService.Show(new ToastOption()
            {
                Category = ToastCategory.Success,
                Title = "Custom download example",
                Content = "data download complete",
            });
        }
    }

    private static async Task MockDownLoadAsync()
    {
        // It takes 5 seconds to simulate the package download data here
        await Task.Delay(5000);
    }
}
