// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/12/2016 by Alexey Butalov

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// Provides methods to detect the current platform, operating system etc.
    /// </summary>
    [JavaManual("Platform abstraction for system utilities. Manual porting by design.")]
    public static class PlatformUtilPal
    {
        /// <summary>
        /// Returns the operating system type in a way that is suitable for our purposes.
        /// </summary>
        public static Platform GetPlatform()
        {
#if NETSTANDARD // 
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return Platform.Windows;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                return Platform.MacOS;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                return Platform.Unix;
#endif
            PlatformID pid = Environment.OSVersion.Platform;
            bool isWindows = (pid == PlatformID.Win32NT) || (pid == PlatformID.Win32S) || (pid == PlatformID.Win32Windows) || (pid == PlatformID.WinCE);

            if (isWindows)
            {
                return Platform.Windows;
            }
            else
            {
                // AS: This looks like it could be a more standard way to for checking platform - taken from the Mono FAQ page.
                // VD: Mac Sierra 10.12 + mono 5.12 returns p = 4
                // Also Notice that as of Mono 2.2 the version returned on MacOS X is still 4 for legacy reasons, 
                int p = (int)Environment.OSVersion.Platform;
                if (p == 4)
                {
                    if (Directory.Exists("/Library") && Directory.Exists("/Applications"))
                        return Platform.MacOS;

                    return Platform.Unix;
                }
                return ((p == 6) || (p == 128)) ? Platform.Unix : Platform.MacOS;
            }
        }

        public static bool IsWindows()
        {
            return GetPlatform() == Platform.Windows;
        }

        public static bool IsLinux()
        {
            return GetPlatform() == Platform.Unix;
        }

        public static bool IsAndroid()
        {
            return GetPlatform() == Platform.Android;
        }

        public static bool IsMacOS()
        {
            return GetPlatform() == Platform.MacOS;
        }

        /// <summary>
        /// Returns true for Unix-like platforms: Linux, MacOs, Android.
        /// </summary>
        public static bool IsUnixLike()
        {
            Platform platform = GetPlatform();
            return platform == Platform.Unix || platform == Platform.MacOS || platform == Platform.Android || platform == Platform.iOS;
        }

        /// <summary>
        /// Shows if the operating is Windows and is equal or greater that Windows 7.
        /// </summary>
        public static bool IsOsEqualOrGreaterThenWindows7()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //+---------------------------------------------------------------------------------------------------------------------------------------------------+
            //|           |   Windows    |   Windows    |   Windows    |Windows NT| Windows | Windows | Windows | Windows | Windows | Windows | Windows | Windows |
            //|           |     95       |      98      |     Me       |    4.0   |  2000   |   XP    |  2003   |  Vista  |  2008   |    7    | 2008 R2 |    8    |
            //+---------------------------------------------------------------------------------------------------------------------------------------------------+
            //|PlatformID | Win32Windows | Win32Windows | Win32Windows | Win32NT  | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT |
            //+---------------------------------------------------------------------------------------------------------------------------------------------------+
            //|Major      |              |              |              |          |         |         |         |         |         |         |         |         |
            //| version   |      4       |      4       |      4       |    4     |    5    |    5    |    5    |    6    |    6    |    6    |    6    |    6    |
            //+---------------------------------------------------------------------------------------------------------------------------------------------------+
            //|Minor      |              |              |              |          |         |         |         |         |         |         |         |         |
            //| version   |      0       |     10       |     90       |    0     |    0    |    1    |    2    |    0    |    0    |    1    |    1    |    2    |
            //+---------------------------------------------------------------------------------------------------------------------------------------------------+

            if (os.Platform == PlatformID.Win32NT)
                if (vs.Major == 6)
                    return vs.Minor > 0;
                else if (vs.Major > 6)
                    return true;
            return false;
        }

        /// <summary>
        /// Shows if the operating is Windows and is greater that Windows 7.
        /// </summary>
        public static bool IsOsGreaterThenWindows7()
        {
            OperatingSystem os = Environment.OSVersion;
            Version vs = os.Version;

            if (os.Platform == PlatformID.Win32NT)
                if (vs.Major == 6)
                    return vs.Minor > 1;
                else if (vs.Major > 6)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true, if the current runtime is Mono.
        /// </summary>
        public static bool IsRunningOnMono()
        {
            // CPLUSPLUS doesn't support reflection
#if !CPLUSPLUS
            return Type.GetType("Mono.Runtime") != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// Runs process and returns process output.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("RunProcess is not implemented for C++ (cross-platform way)")]
        public static ProcessExecutionResult RunProcess(string fileName, params string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = fileName;
            
            if (args != null && args.Length > 0)
            {
                // Surround arguments which have spaces with double quotes.
                StringBuilder parameters = new StringBuilder();
                foreach (string s in args)
                    parameters.AppendFormat("{0}{1}{0} ", s.Contains(" ") ? "\"" : "", s);
                
                parameters.Length--;
                
                startInfo.Arguments = parameters.ToString();
            }

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    int exitCode = process.ExitCode;

                    return new ProcessExecutionResult(output, exitCode == 0, errors, null);
                }
            }
            catch (InvalidOperationException e)
            {
                return new ProcessExecutionResult(null, false, null, e);
            }
            catch (Win32Exception e)
            {
                return new ProcessExecutionResult(null, false, null, e);
            }
        }
    }
}
