﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using BootstrapBlazor.Shared.Common;
using BootstrapBlazor.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Shared.Samples;

/// <summary>
/// 
/// </summary>
public sealed partial class TreeViews
{
    [NotNull]
    private BlockLogger? Trace { get; set; }

    [NotNull]
    private BlockLogger? TraceChecked { get; set; }

    [NotNull]
    private BlockLogger? TraceCheckedItems { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? LocalizerFoo { get; set; }

    private Foo Model => Foo.Generate(LocalizerFoo);

    private List<TreeViewItem<TreeFoo>> Items { get; set; } = TreeFoo.GetTreeItems();

    private List<TreeViewItem<TreeFoo>> CheckedItems { get; set; } = GetCheckedItems();

    private List<TreeViewItem<TreeFoo>> ExpandItems { get; set; } = GetExpandItems();

    private List<TreeViewItem<TreeFoo>>? AsyncItems { get; set; }

    [NotNull]
    private List<TreeViewItem<TreeFoo>>? SelectedItemsDataSource { get; set; }

    private bool AutoCheckChildren { get; set; }

    private bool AutoCheckParent { get; set; }

    private List<TreeViewItem<TreeFoo>> SelectedItems { get; set; } = new();

    private List<SelectedItem> ResetItems { get; } = new List<SelectedItem>()
    {
        new("True", "Reset"),
        new("False", "Keep")
    };

    /// <summary>
    /// OnInitializedAsync 方法
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await OnLoadAsyncItems();

        SelectedItemsDataSource = TreeFoo.GetTreeItems();
    }

    private void OnRefresh()
    {
        CheckedItems = GetCheckedItems();
    }

    private bool IsReset { get; set; }

    private async Task OnLoadAsyncItems()
    {
        AsyncItems = null;
        await Task.Delay(2000);
        AsyncItems = TreeFoo.GetTreeItems();
        AsyncItems[2].Text = "延时加载";
        AsyncItems[2].HasChildren = true;
    }

    private static List<TreeViewItem<TreeFoo>> GetCheckedItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[1].IsActive = true;
        ret[1].Items[1].CheckedState = CheckboxState.Checked;
        return ret;
    }

    private static List<TreeViewItem<TreeFoo>> GetExpandItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[1].IsExpand = true;
        return ret;
    }

    private List<TreeViewItem<TreeFoo>> DisabledItems { get; set; } = GetDisabledItems();

    private static List<TreeViewItem<TreeFoo>> GetDisabledItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[1].Items[1].IsDisabled = true;
        return ret;
    }

    private static List<TreeViewItem<TreeFoo>> GetAccordionItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[1].Items[0].HasChildren = true;
        return ret;
    }

    private static List<TreeViewItem<TreeFoo>> GetIconItems() => TreeFoo.GetTreeItems();

    private static List<TreeViewItem<TreeFoo>> GetLazyItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[1].Items[0].IsExpand = true;
        ret[2].Text = "懒加载延时";
        ret[2].HasChildren = true;
        return ret;
    }

    private static List<TreeViewItem<TreeFoo>> GetTemplateItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[0].Template = foo => BootstrapDynamicComponent.CreateComponent<CustomerTreeItem>(new Dictionary<string, object?>()
        {
            [nameof(CustomerTreeItem.Foo)] = foo
        }).Render();
        return ret;
    }

    private static List<TreeViewItem<TreeFoo>> GetColorItems()
    {
        var ret = TreeFoo.GetTreeItems();
        ret[0].CssClass = "text-primary";
        ret[1].CssClass = "text-success";
        ret[2].CssClass = "text-danger";
        return ret;
    }

    private class CustomerTreeItem : ComponentBase
    {
        [Inject]
        [NotNull]
        private ToastService? ToastService { get; set; }

        [Parameter]
        [NotNull]
        public TreeFoo? Foo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(3, "span");
            builder.AddAttribute(4, "class", "me-3");
            builder.AddContent(5, Foo.Text);
            builder.CloseElement();

            builder.OpenComponent<Button>(0);
            builder.AddAttribute(1, nameof(Button.Icon), "fa-solid fa-font-awesome");
            builder.AddAttribute(2, nameof(Button.Text), "Click");
            builder.AddAttribute(3, nameof(Button.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, e =>
            {
                ToastService.Warning("自定义 TreeItem", "测试 TreeItem 按钮点击事件");
            }));
            builder.CloseComponent();
        }
    }

    private Task OnTreeItemClick(TreeViewItem<TreeFoo> item)
    {
        Trace.Log($"TreeItem: {item.Text} clicked");
        return Task.CompletedTask;
    }

    private Task OnTreeItemChecked(TreeViewItem<TreeFoo> item)
    {
        var state = item.CheckedState == CheckboxState.Checked ? "选中" : "未选中";
        TraceChecked.Log($"TreeItem: {item.Text} {state}");
        return Task.CompletedTask;
    }

    private static async Task<IEnumerable<TreeViewItem<TreeFoo>>> OnExpandNodeAsync(TreeViewItem<TreeFoo> node)
    {
        await Task.Delay(800);
        var item = node.Value;
        return new TreeViewItem<TreeFoo>[]
        {
            new TreeViewItem<TreeFoo>(new TreeFoo() { Id = $"{item.Id}-101", ParentId = item.Id })
            {
                Text = "懒加载子节点1",
                HasChildren = true
            },
            new TreeViewItem<TreeFoo>(new TreeFoo(){ Id = $"{item.Id}-102", ParentId = item.Id })
            {
                Text = "懒加载子节点2"
            }
        };
    }

    private Task OnTreeItemChecked(List<TreeViewItem<TreeFoo>> items)
    {
        TraceCheckedItems.Log($"当前共选中{items.Count}项");
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获得属性方法
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<AttributeItem> GetAttributes() => new AttributeItem[]
    {
        new AttributeItem() {
            Name = "Items",
            Description = "menu data set",
            Type = "IEnumerable<TreeItem>",
            ValueList = " — ",
            DefaultValue = "new List<TreeItem>(20)"
        },
        new AttributeItem() {
            Name = "ClickToggleNode",
            Description = "Whether to expand or contract children when a node is clicked",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = "ShowCheckbox",
            Description = "Whether to display CheckBox",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = "ShowIcon",
            Description = "Whether to display Icon",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = "ShowSkeleton",
            Description = "Whether to display the loading skeleton screen",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = nameof(TreeView<string>.OnTreeItemClick),
            Description = "Callback delegate when tree control node is clicked",
            Type = "Func<TreeItem, Task>",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new AttributeItem() {
            Name = nameof(TreeView<string>.OnTreeItemChecked),
            Description = "Callback delegate when tree control node is selected",
            Type = "Func<TreeItem, Task>",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new AttributeItem() {
            Name = nameof(TreeView<string>.OnExpandNodeAsync),
            Description = "Tree control node expand callback delegate",
            Type = "Func<TreeItem, Task>",
            ValueList = " — ",
            DefaultValue = " — "
        }
    };

    private static IEnumerable<AttributeItem> GetTreeItemAttributes() => new AttributeItem[]
    {
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.Items),
            Description = "Child node data source",
            Type = "List<TreeItem<TItem>>",
            ValueList = " — ",
            DefaultValue = "new ()"
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.Text),
            Description = "Display text",
            Type = "string",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.Icon),
            Description = "Show icon",
            Type = "string",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.CssClass),
            Description = "Node custom style",
            Type = "string",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.CheckedState),
            Description = "Is selected",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.IsDisabled),
            Description = "Is disabled",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.IsExpand),
            Description = "Whether to expand",
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "true"
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.HasChildren),
            Description = "Whether there are child nodes",
            Type = "bool",
            ValueList = " true|false ",
            DefaultValue = " false "
        },
        new AttributeItem() {
            Name = nameof(TreeViewItem<TreeFoo>.ShowLoading),
            Description = "Whether to show child node loading animation",
            Type = "bool",
            ValueList = " true|false ",
            DefaultValue = " false "
        },
        new AttributeItem()
        {
            Name = nameof(TreeViewItem<TreeFoo>.Template),
            Description = "Child node template",
            Type = nameof(RenderFragment),
            ValueList = " — ",
            DefaultValue = " — "
        }
    };
}
