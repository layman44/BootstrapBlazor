﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using BootstrapBlazor.Shared;

namespace UnitTest.Extensions
{
    internal static class FooExtensions
    {
        public static object GenerateValueExpression(this Foo model) => Utility.GenerateValueExpression(model, nameof(Foo.Name), typeof(string));
    }
}
