# WorldBox NativeLibrary Mod

**NativeLibrary** is a dependency mod for [WorldBox](https://www.superworldbox.com/) [NeoModLoader](https://github.com/WorldBoxOpenMods/ModLoader).
It provides automated loading of managed DLLs and native libraries within the modding ecosystem.
This mod simplifies the integration of external libraries by handling the loading process at runtime,
eliminating the need for explicit calls to `LoadLibrary` or `Assembly.Load`.

Please note that due to the complexities involved in performing **shading** or **relocation** operations at runtime,
**NativeLibrary** does **not offer any mechanisms for dependency isolation**.
For native libraries, users may achieve isolation by manually renaming binary files.
However, this approach is not possible for managed DLLs,
and potential conflicts must be managed externally.

## Features

* **Automated Libraries Loading**: Automatically loads both **managed DLLs** and **native libraries** without requiring manual intervention in code.
* **Dependency Integration**: Acts as a foundational mod that other mods can depend on for library management.
* **No Isolation Support**: Explicitly does not provide **shading** or **relocation** to isolate dependencies, prioritizing simplicity and runtime efficiency.

## Usage

1. Include the GUID of `NativeLibrary` in the dependencies list of your `mod.json` file. For example:
   
   ```json5
   {
   "Dependencies": ["NATIVELIBRARY"],
   // or
   "OptionalDependencies": ["NATIVELIBRARY"]
   }
   ```

2. Place managed DLLs directly in the `NativeLibrary` folder.
   
   For native libraries, place them in architecture-specific subfolders under `NativeLibrary` (supported: `x86_64` and `arm64`, based on **WorldBox** target platform).
   
   The mod automatically loads the correct binary (`.dll` on **Windows**, `.so` on **Linux**, `.dylib` on **macOS**) for the operating system.
   
   For details on native library loading, see [Native library loading](https://learn.microsoft.com/dotnet/standard/native-interop/native-library-loading).
   
   Example structure:
   
   ```
   NativeLibrary/
   ├── Managed.dll
   ├── x86_64/
   │   ├── native.dll
   │   ├── libnative.so
   │   └── libnative.dylib
   └── arm64/
       └── libnative.dylib
   ```

3. For native libraries, you still need to use `DllImport`
   
   ```csharp
   [DllImport("example")]
   public static extern long foo(); 
   ```

## License

This project is licensed under the [MIT License](LICENSE) © 2025 557.
