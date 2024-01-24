﻿// Copyright © 2024 Greeana LLC. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

namespace Chromatron.NativeHosts.Helpers;

/// <summary>
/// Sets application thread apartment.
/// </summary>
/// <remarks>
/// https://github.com/dotnet/winforms/issues/5071
/// </remarks>
public static class ThreadApt
{
    /// <summary>
    /// Set application to Single Thread Apartment (STA).
    /// </summary>
    public static void STA()
    {
        // Set STAThread 
#pragma warning disable CA1416 // Validate platform compatibility
        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416 // Validate platform compatibility

    }
}
