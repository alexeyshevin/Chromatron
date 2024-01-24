﻿// Copyright © 2024 Greeana LLC. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

namespace Chromatron.NativeHosts.WinHost.WinBase;

public abstract partial class WinHostBase
{
    /// <summary>
    /// Place holder method to register hot keys.
    /// </summary>
    /// <param name="hwnd">The window handle.</param>
    protected virtual void RegisterHotKeys(IntPtr hwnd)
    {
    }
}