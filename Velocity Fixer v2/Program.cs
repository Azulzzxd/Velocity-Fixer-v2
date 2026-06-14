using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.IO.Compression;
using System.Net.Http;

class Program
{

    static void DrawWatermark()
    {
        ConsoleColor color = ConsoleColor.Cyan;
        Console.ForegroundColor = color;

        string ascii = @"
 ___      ___ _______   ___       ________  ________  ___  _________    ___    ___ 
|\  \    /  /|\  ___ \ |\  \     |\   __  \|\   ____\|\  \|\___   ___\ |\  \  /  /|
\ \  \  /  / | \   __/|\ \  \    \ \  \|\  \ \  \___|\ \  \|___ \  \_| \ \  \/  / /
 \ \  \/  / / \ \  \_|/_\ \  \    \ \  \\\  \ \  \    \ \  \   \ \  \   \ \    / / 
  \ \    / /   \ \  \_|\ \ \  \____\ \  \\\  \ \  \____\ \  \   \ \  \   \/  /  /  
   \ \__/ /     \ \_______\ \_______\ \_______\ \_______\ \__\   \ \__\__/  / /    
    \|__|/       \|_______|\|_______|\|_______|\|_______|\|__|    \|__|\___/ /     
                                                                      \|___|/      
";

        string[] lines = ascii.Split('\n');
        int width = Console.WindowWidth;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine();
                continue;
            }

            int padding = Math.Max(0, (width - line.Length) / 2);
            Console.WriteLine(new string(' ', padding) + line);
        }

        Console.WriteLine();

        string text = $"Velocity Fixer v2   |   made by Azul   |   {DateTime.Now:HH:mm:ss}";
        int textPadding = Math.Max(0, (width - text.Length) / 2);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string(' ', textPadding) + text);
        Console.WriteLine(new string(' ', textPadding) + new string('─', text.Length));

        Console.ResetColor();
    }

    static async Task Main()
    {
        Console.SetWindowSize(86, 50);
        Console.SetBufferSize(86, 2000);

        Console.Title = "Velocity Fixer - rebuilt by Azul";
        while (true)
        {
            DrawMenu();

            Console.Write("Choose your option: ");
            string choice = (Console.ReadLine() ?? "").Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(choice))
                continue;

            if (choice == "Q")
                return;

            switch (choice)
            {
                case "A":
                    Console.Clear();
                    await FixMonacoAsync();
                    ReturnToMenu();
                    break;

                case "B":
                    Console.Clear();
                    await InstallWebView2Async();
                    ReturnToMenu();
                    break;

                case "C":
                    Console.Clear();
                    await InstallCloudflareWarpAsync();
                    ReturnToMenu();
                    break;

                case "D":
                    Console.Clear();
                    await InstallBetaUiAsync();
                    ReturnToMenu();
                    break;

                case "E":
                    Console.Clear();
                    ClearSettings();
                    ReturnToMenu();
                    break;

                case "F":
                    Console.Clear();
                    ClearTabHistory();
                    ReturnToMenu();
                    break;

                case "H":
                    Console.Clear();
                    await InstallVCRuntimeAsync();
                    ReturnToMenu();
                    break;

                case "I":
                    Console.Clear();
                    Console.WriteLine(IsFishstrapInstalled()
                        ? "Fishstrap installed."
                        : "Fishstrap not installed.");

                    await InstallFishstrapAsync();
                    ReturnToMenu();
                    break;

                case "G":
                    Console.Clear();
                    Console.WriteLine(IsDotNet10Installed()
                        ? ".NET 10 runtime is installed."
                        : ".NET 10 runtime is missing.");

                    await InstallDotNet10Async();
                    ReturnToMenu();
                    break;

                case "Q":
                    return;

                case "R":
                    Console.Clear();
                    DrawMenu();
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    ReturnToMenu();
                    break;
            }
        }

        static void DrawMenu()
        {
            Console.Clear();
            DrawWatermark();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Legend:");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [!] Required for base functionality");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [/] Recommended / only if you have the issue");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  [*] Optional feature");

            Console.WriteLine();

            DrawSection("USER INTERFACE", new[]
            {
        ("[!] A. Fix Monaco file not found",
         "Installs Monaco Editor for all users",
         IsMonacoInstalled()),

        ("[!] B. Fix no editor appearing",
         "Downloads WebView2",
         IsWebView2Installed()),

        ("[/] C. Fix Ui crashing on launch",
         "Downloads Cloudflare WARP - prevents ISP blocking Velocity.",
         IsCloudflareWarpInstalled()),

        ("[*] D. Download V2 Beta UI",
         "Downloads the discontinued v2 Ui",
         (bool?)null),

        ("[*] E. Reset Velocity settings",
         "Clears config.json",
         AreSettingsPresent()),

        ("[*] F. Clear tab history",
         "Clears tabs.json",
         HasTabHistory()),
        ("[!] G. .NET 10 runtime",
         "Needed to run the Velocity UI!",
         IsDotNet10Installed())
    });

            DrawSection("EXECUTOR FIXES", new (string name, string desc, bool? status)[]
            {
        ("[!] H. Visual C++ Runtime",
         "Required for the injector and other services.",
         IsVCRuntimeInstalled()),

        ("[/] I. Fishstrap",
         "Usually puts you on the correct Roblox version",
         IsFishstrapInstalled())
            });

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Q. Quit");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("R. Refresh");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void DrawSection(string title, (string name, string desc, bool? status)[] items)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"┌────────────── {title} ──────────────┐");

            foreach (var item in items)
            {
                Console.Write("│ ");

                string icon = item.name.Length >= 3 ? item.name.Substring(0, 3) : "";
                string text = item.name.Length > 3 ? item.name.Substring(3) : item.name;

                switch (icon)
                {
                    case "[!]":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case "[/]":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case "[*]":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.Write(icon);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(text.PadRight(52));

                if (item.status == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[Installed]".PadLeft(18));
                }
                else if (item.status == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[Not installed]".PadLeft(18));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("[Unknown]".PadLeft(18));
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" │");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"│    {item.desc.PadRight(70)} │");
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("└──────────────────────────────────────┘");
            Console.WriteLine();
        }

        static void ReturnToMenu()
        {
            WaitForEnter();
        }


        static bool IsMonacoInstalled()
        {
            try
            {
                foreach (string userDir in Directory.GetDirectories(@"C:\Users"))
                {
                    string basePath = Path.Combine(
                        userDir,
                        "AppData",
                        "Local",
                        "Velocity Ui",
                        "Monaco Editor");

                    if (!Directory.Exists(basePath))
                        continue;

                    string indexFile = Path.Combine(basePath, "Index.html");
                    string vsFolder = Path.Combine(basePath, "vs");

                    if (!File.Exists(indexFile))
                        continue;

                    if (!Directory.Exists(vsFolder))
                        continue;

                    if (!Directory.EnumerateFileSystemEntries(vsFolder).Any())
                        continue;

                    return true;
                }
            }
            catch { }

            return false;
        }

        static bool AreSettingsPresent()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Velocity Ui",
                "config.json");

            return File.Exists(path);
        }

        static bool HasTabHistory()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Velocity Ui",
                "tabs.json");

            return File.Exists(path);
        }
    
        static bool IsFishstrapInstalled()
        {
            try
            {
                string[] uninstallRoots =
                {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    foreach (var root in uninstallRoots)
                    {
                        using var key = baseKey.OpenSubKey(root);
                        if (key != null)
                        {
                            foreach (var subKeyName in key.GetSubKeyNames())
                            {
                                using var subKey = key.OpenSubKey(subKeyName);
                                var name = subKey?.GetValue("DisplayName")?.ToString();

                                if (!string.IsNullOrWhiteSpace(name) &&
                                    name.Contains("Fishstrap", StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                string fishstrapFolder = Path.Combine(localAppData, "Fishstrap");
                if (Directory.Exists(fishstrapFolder))
                    return true;

                var possibleFolders = Directory.GetDirectories(localAppData)
                    .Where(d => Path.GetFileName(d)
                        .Contains("fish", StringComparison.OrdinalIgnoreCase));

                if (possibleFolders.Any())
                    return true;

                string exePath = Path.Combine(fishstrapFolder, "Fishstrap.exe");
                if (File.Exists(exePath))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        static bool IsCloudflareWarpInstalled()
        {
            try
            {
                bool foundInUninstall = false;

                string[] uninstallKeys =
                {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

                foreach (var path in uninstallKeys)
                {
                    using var key = Registry.LocalMachine.OpenSubKey(path);
                    if (key == null) continue;

                    foreach (var sub in key.GetSubKeyNames())
                    {
                        using var sk = key.OpenSubKey(sub);
                        var name = sk?.GetValue("DisplayName")?.ToString();

                        if (!string.IsNullOrWhiteSpace(name) &&
                            name.Equals("Cloudflare WARP", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                        if (!string.IsNullOrWhiteSpace(name) &&
                            name.Contains("Cloudflare WARP", StringComparison.OrdinalIgnoreCase))
                        {
                            foundInUninstall = true;
                        }
                    }
                }

                if (foundInUninstall)
                    return true;

                string exePath = @"C:\Program Files\Cloudflare\Cloudflare WARP\Cloudflare WARP.exe";
                if (File.Exists(exePath))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        static bool IsWebView2Installed()
        {
            string[] executables =
            {
        @"C:\Program Files (x86)\Microsoft\EdgeWebView\Application",
        @"C:\Program Files\Microsoft\EdgeWebView\Application"
    };

            foreach (string root in executables)
            {
                if (!Directory.Exists(root))
                    continue;

                foreach (string versionDir in Directory.GetDirectories(root))
                {
                    if (File.Exists(Path.Combine(versionDir, "msedgewebview2.exe")))
                        return true;
                }
            }

            return false;
        }

        static string? FindDotNet()
        {
            string[] locations =
            {
        @"C:\Program Files\dotnet\dotnet.exe",
        @"C:\Program Files (x86)\dotnet\dotnet.exe"
    };

            return locations.FirstOrDefault(File.Exists);
        }

        static bool IsDotNet10Installed()
        {
            try
            {
                string? dotnet = FindDotNet();

                if (dotnet == null)
                    return false;

                var psi = new ProcessStartInfo
                {
                    FileName = dotnet,
                    Arguments = "--list-runtimes",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);

                string output = process?.StandardOutput.ReadToEnd() ?? "";

                process?.WaitForExit();

                return output.Contains("Microsoft.NETCore.App 10.");
            }
            catch
            {
                return false;
            }
        }

        static bool IsVCRuntimeInstalled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\X64");

                return Convert.ToInt32(key?.GetValue("Installed", 0)) == 1;
            }
            catch { return false; }
        }

        static bool ConfirmReinstall(string productName)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{productName} is already installed.");
            Console.WriteLine();
            Console.WriteLine("Press ENTER to go back.");
            Console.WriteLine("Press Y to continue anyway.");
            Console.ResetColor();

            var key = Console.ReadKey(true);

            return key.Key == ConsoleKey.Y;
        }


        static void ClearSettings()
        {
            try
            {
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Velocity Ui",
                    "config.json");

                if (!File.Exists(configPath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Settings file not found.");
                    Console.ResetColor();
                    return;
                }

                File.WriteAllText(configPath, "{}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Settings reset.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void ClearTabHistory()
        {
            try
            {
                string tabsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Velocity Ui",
                    "tabs.json");

                if (!File.Exists(tabsPath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Tab history file not found.");
                    Console.ResetColor();
                    return;
                }

                File.WriteAllText(tabsPath, "[]");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Tab history cleared.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed: {ex.Message}");
                Console.ResetColor();
            }
        }


        static async Task DownloadFileAsync(string url, string path)
        {
            using HttpClient client = new();

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            byte[] buffer = new byte[8192];
            int read;
            long total = 0;
            long? length = response.Content.Headers.ContentLength;

            Console.CursorVisible = false;

            while ((read = await stream.ReadAsync(buffer)) > 0)
            {
                await file.WriteAsync(buffer.AsMemory(0, read));
                total += read;

                if (length.HasValue)
                {
                    double p = (double)total / length.Value;
                    int bar = (int)(p * 30);

                    Console.Write($"\r[{new string('█', bar)}{new string('░', 30 - bar)}] {p * 100:0.0}%".PadRight(Console.WindowWidth - 1));
                }
                else
                {
                    Console.Write($"\rDownloaded {total / 1024} KB".PadRight(Console.WindowWidth - 1));
                }
            }

            Console.WriteLine();
            Console.CursorVisible = true;
        }

        static async Task InstallVCRuntimeAsync()
        {
            if (IsVCRuntimeInstalled())
            {
                Console.Clear();
                if (!ConfirmReinstall("Visual C++ Runtime"))
                    return;
            }

            try
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Directory.CreateDirectory(folder);

                string exePath = Path.Combine(folder, "vc_redist.x64.exe");
                const string url = "https://aka.ms/vc14/vc_redist.x64.exe";

                Console.Clear();
                Console.WriteLine("Downloading Visual C++ Redistributable...\n");

                await DownloadFileAsync(url, exePath);

                Console.WriteLine("\nRunning installer...");

                var process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = "/install /quiet /norestart";
                process.StartInfo.UseShellExecute = true;
                process.Start();
                process.WaitForExit();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nVC++ Redistributable installed.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFailed: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static async Task InstallWebView2Async()
        {
            if (IsWebView2Installed())
            {
                Console.Clear();
                if (!ConfirmReinstall("WebView2 Runtime"))
                    return;
            }

            try
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Directory.CreateDirectory(folder);

                string exePath = Path.Combine(folder, "webview2.exe");
                const string url = "https://go.microsoft.com/fwlink/p/?LinkId=2124701";

                Console.Clear();
                Console.WriteLine("Downloading WebView2 Runtime...\n");

                await DownloadFileAsync(url, exePath);

                Console.WriteLine("\nRunning installer...");

                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = "",
                    UseShellExecute = true
                });

                Console.WriteLine("WebView2 installer launched.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed: {ex.Message}");
                Console.ResetColor();
            }
        }

        static async Task InstallDotNet10Async()
        {
            try
            {
                const string url = "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/10.0.9/windowsdesktop-runtime-10.0.9-win-x64.exe";

                string tempFile = Path.Combine(Path.GetTempPath(), "windowsdesktop-runtime-10.0.9-win-x64;

                Console.Clear();
                Console.WriteLine("Downloading .NET 10 SDK...\n");

                await DownloadFileAsync(url, tempFile);

                Console.WriteLine("\nLaunching installer...\n");

                Process.Start(new ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed: {ex.Message}");
                Console.ResetColor();
            }
        }

        static async Task InstallFishstrapAsync()
        {
            if (IsFishstrapInstalled())
            {
                Console.Clear();
                if (!ConfirmReinstall("Fishstrap"))
                    return;
            }

            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            Directory.CreateDirectory(folder);

            string exePath = Path.Combine(folder, "Fishstrap.exe");
            const string url = "https://github.com/fishstrap/fishstrap/releases/latest/download/Fishstrap.exe";

            Console.Clear();
            Console.WriteLine("Downloading Fishstrap...\n");

            await DownloadFileAsync(url, exePath);

            await Task.Delay(500);

            int retries = 10;
            while (IsFileLocked(exePath) && retries-- > 0)
                await Task.Delay(300);

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            });
        }

        static bool IsFileLocked(string path)
        {
            try
            {
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                return false;
            }
            catch
            {
                return true;
            }
        }

        static async Task InstallBetaUiAsync()
        {
            try
            {
                string baseFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    "Velocity Beta interface"
                );

                Directory.CreateDirectory(baseFolder);

                string zipPath = Path.Combine(baseFolder, "beta.zip");

                const string url =
                    "https://github.com/Azulzzxd/Velocity-User-Interface/releases/download/Release_x64/Velocity.Compiled.zip";

                using HttpClient client = new();

                Console.Clear();
                Console.WriteLine("Downloading Beta UI...\n");

                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var file = new FileStream(zipPath, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[8192];
                int read;

                long totalRead = 0;
                long? totalSize = response.Content.Headers.ContentLength;

                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await file.WriteAsync(buffer.AsMemory(0, read));
                    totalRead += read;

                    if (totalSize.HasValue)
                    {
                        double percent = (double)totalRead / totalSize.Value * 100;
                        int bar = (int)(percent / 100 * 30);

                        Console.CursorLeft = 0;
                        Console.Write(
                            $"[{new string('█', bar)}{new string('░', 30 - bar)}] {percent:0.0}%"
                        );
                    }
                }

                file.Close();

                Console.WriteLine("\n\nExtracting...");

                ZipFile.ExtractToDirectory(zipPath, baseFolder, true);

                File.Delete(zipPath);

                Console.WriteLine("Opening folder...");

                Process.Start(new ProcessStartInfo
                {
                    FileName = baseFolder,
                    UseShellExecute = true,
                    Verb = "open"
                });

                Console.WriteLine("Beta UI installed.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed: {ex.Message}");
                Console.ResetColor();
            }
        }

        static async Task FixMonacoAsync()
        {
            if (IsMonacoInstalled())
            {
                Console.Clear();

                if (!ConfirmReinstall("Monaco Editor"))
                    return;
            }

            try
            {
                const string url =
                    "https://github.com/Azulzzxd/Velocity-User-Interface/raw/refs/heads/main/VelocityLite/Monaco%20Editor.zip";

                string tempZip = Path.Combine(
                    Path.GetTempPath(),
                    "velocity_monaco.zip");

                Console.Clear();
                Console.WriteLine("Downloading Monaco Editor...\n");

                await DownloadFileAsync(url, tempZip);

                Console.WriteLine("\nInstalling for all users...\n");

                HashSet<string> ignoredUsers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Public",
            "Default",
            "Default User",
            "All Users"
        };

                int installedCount = 0;

                foreach (string userDir in Directory.GetDirectories(@"C:\Users"))
                {
                    try
                    {
                        string userName = Path.GetFileName(userDir);

                        if (ignoredUsers.Contains(userName))
                            continue;

                        string velocityPath = Path.Combine(
                            userDir,
                            "AppData",
                            "Local",
                            "Velocity Ui",
                            "Monaco Editor");

                        Directory.CreateDirectory(velocityPath);

                        ZipFile.ExtractToDirectory(
                            tempZip,
                            velocityPath,
                            true);

                        installedCount++;

                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Installed for: {userName}");
                        Console.ResetColor();
                    }
                    catch
                    {
                    }
                }

                if (File.Exists(tempZip))
                    File.Delete(tempZip);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nMonaco installed for {installedCount} user(s).");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFailed: {ex.Message}");
                Console.ResetColor();
            }
        }

        static async Task InstallCloudflareWarpAsync()
        {
            if (IsCloudflareWarpInstalled())
            {
                Console.Clear();

                if (!ConfirmReinstall("Cloudflare WARP"))
                    return;
            }
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads");

                Directory.CreateDirectory(folder);

                const string url = "https://1111-releases.cloudflareclient.com/win/latest";

                using HttpClient client = new();

                Console.Clear();
                Console.WriteLine("Downloading Cloudflare WARP...\n");

                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                string fileName = response.Content.Headers.ContentDisposition?.FileNameStar?.Trim('"')
                    ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                    ?? "Cloudflare_WARP.msi";

                string msiPath = Path.Combine(folder, fileName);

                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var file = new FileStream(msiPath, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[8192];
                int read;
                long total = 0;
                long? length = response.Content.Headers.ContentLength;

                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await file.WriteAsync(buffer.AsMemory(0, read));
                    total += read;

                    if (length.HasValue)
                    {
                        double p = (double)total / length.Value * 100;
                        int bar = (int)(p / 100 * 30);

                        Console.CursorLeft = 0;
                        Console.Write($"[{new string('█', bar)}{new string('░', 30 - bar)}] {p:0.0}%");
                    }
                }

                file.Close();

                string? latestWarp = Directory
                    .GetFiles(folder, "Cloudflare_WARP*.msi")
                    .OrderByDescending(File.GetLastWriteTime)
                    .FirstOrDefault();

                if (latestWarp == null)
                    throw new FileNotFoundException("Could not locate Cloudflare WARP installer.");

                Console.WriteLine("\n\nRunning Cloudflare WARP installer...");

                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = $"/i \"{latestWarp}\"",
                    UseShellExecute = true
                });

                if (process != null)
                    process.WaitForExit();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nCloudflare WARP installed.");
                Console.ForegroundColor = ConsoleColor.White;

                string velocityPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "VelocityUi",
                    "Velocity.exe");

                if (File.Exists(velocityPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = velocityPath,
                        UseShellExecute = true
                    });

                    Console.WriteLine("Velocity launched.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFailed: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void WaitForEnter(string message = "Press ENTER to go back...")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine(message);

            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            Console.ResetColor();
        }
    }
}
