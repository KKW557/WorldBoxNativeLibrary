using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NeoModLoader.api;
using NeoModLoader.constants;
using Newtonsoft.Json;
using UnityEngine;

namespace KKW557.WorldBox.NativeLibrary;

public class Mod : BasicMod<Mod>
{
    public const string Name = "NativeLibrary";
    public const string DllPattern = "*.dll";
    public const string SoPattern = "*.so";
    public const string DylibPattern = "*.dylib";
    public const string Apple = "Apple";
    public const string x86_64 = "x86_64";
    public const string arm64 = "arm64";

    protected override void OnModLoad()
    {
        LogInfo("Author: " + GetDeclaration().Author);
        LogInfo("Version: " + GetDeclaration().Version);

        LoadLibraries();
    }

    private static void LoadLibraries()
    {
        var managedLoaded = 0;
        var managedFailed = 0;
        var nativeLoaded = 0;
        var nativeFailed = 0;

        foreach (var dir in Directory.GetDirectories(Paths.ModsPath)
                     .Concat(Directory.GetDirectories(Paths.CommonModsWorkshopPath)))
        {
            var file = Path.Combine(dir, Paths.ModDeclarationFileName);
            if (!File.Exists(file)) continue;

            var mod = JsonConvert.DeserializeObject<ModDeclare>(File.ReadAllText(file));
            if (mod == null) continue;
            
            var incompatibleWith = mod.IncompatibleWith ?? Array.Empty<string>();
            if (incompatibleWith.Contains(Instance.GetDeclaration().UID)) continue;

            var dependencies = mod.Dependencies ?? Array.Empty<string>();
            var optionalDependencies = mod.OptionalDependencies ?? Array.Empty<string>();
            if (!dependencies.Contains(Instance.GetDeclaration().UID) &&
                !optionalDependencies.Contains(Instance.GetDeclaration().UID)) continue;

            var lib = Path.Combine(dir, Name);
            if (!Directory.Exists(lib)) continue;

            LoadManaged(lib, ref managedLoaded, ref managedFailed);

            LoadNative(lib, ref nativeLoaded, ref nativeFailed);
        }

        LogInfo($"Managed loaded {managedLoaded}, failed {managedFailed}");
        LogInfo($"Native loaded {nativeLoaded}, failed {nativeFailed}");
    }

    private static void LoadManaged(string path, ref int loaded, ref int failed)
    {
        foreach (var dll in Directory.GetFiles(path, DllPattern))
        {
            try
            {
                Assembly.LoadFrom(dll);
                loaded++;
            }
            catch (Exception)
            {
                failed++;
            }
        }
    }

    private static void LoadNative(string path, ref int loaded, ref int failed)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                var wTarget = Path.Combine(path, x86_64);
                if (!Directory.Exists(wTarget)) return;

                foreach (var file in Directory.GetFiles(wTarget, DllPattern))
                {
                    if (kernel32.LoadLibrary(file) == IntPtr.Zero) failed++;
                    else loaded++;
                }

                return;
            case RuntimePlatform.LinuxPlayer:
                var lTarget = Path.Combine(path, x86_64);
                if (!Directory.Exists(lTarget)) return;

                foreach (var file in Directory.GetFiles(lTarget, SoPattern))
                {
                    if (dl.dlopen(file, dl.RTLD_NOW) == IntPtr.Zero) failed++;
                    else loaded++;
                }

                return;
            case RuntimePlatform.OSXPlayer:
                var xTarget = Path.Combine(path, SystemInfo.processorType.Contains(Apple) ? arm64 : x86_64);
                if (!Directory.Exists(xTarget)) return;

                foreach (var file in Directory.GetFiles(xTarget, DylibPattern))
                {
                    if (dl.dlopen(file, dl.RTLD_NOW) == IntPtr.Zero) failed++;
                    else loaded++;
                }

                return;
        }
    }
}