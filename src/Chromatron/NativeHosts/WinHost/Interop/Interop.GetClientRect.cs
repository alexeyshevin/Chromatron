﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Chromatron.NativeHosts.WinHost;

public static partial class Interop
{
    public static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        internal static extern BOOL GetClientRect(IntPtr hWnd, ref RECT lpRect);

        public static BOOL GetClientRect(HandleRef hWnd, ref RECT lpRect)
        {
            BOOL result = GetClientRect(hWnd.Handle, ref lpRect);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}