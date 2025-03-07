﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Chromatron.NativeHosts.WinHost;

public partial class Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public RECT(Rectangle r)
        {
            left = r.Left;
            top = r.Top;
            right = r.Right;
            bottom = r.Bottom;
        }

        public bool AreEqual(RECT r)
        {
            if ((left == r.left) &&
                (top == r.top) &&
                (right == r.right) &&
                (bottom == r.bottom))
            {
                return true;
            }

            return false;
        }

        public static implicit operator Rectangle(RECT r)
            => Rectangle.FromLTRB(r.left, r.top, r.right, r.bottom);

        public static implicit operator RECT(Rectangle r)
            => new RECT(r);

        public int X => left;

        public int Y => top;

        public int Width
            => right - left;

        public int Height
            => bottom - top;

        public Size Size
            => new Size(Width, Height);
    }
}