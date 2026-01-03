using System;
using System.Runtime.InteropServices;

namespace KKW557.WorldBox.NativeLibrary;

internal static class dl
{
    public const int RTLD_NOW = 2;
    
    [DllImport("dl")]
    public static extern IntPtr dlopen(string path, int flags);
}