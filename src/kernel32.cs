using System;
using System.Runtime.InteropServices;

namespace KKW557.WorldBox.NativeLibrary;

internal static class kernel32
{
    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lpLibFileName);
}