// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/06/2010 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using Aspose.Collections;
using Aspose.IO;
using Aspose.JavaAttributes;
using Microsoft.Win32;

namespace Aspose.Common
{
    /// <summary>
    /// This class is to be ported manually to Java.
    /// </summary>
    [JavaManual("Platform abstraction for system utilities. Manual porting by design.")]
    public static class SystemPal
    {
        /// <summary>
        /// <para>Sets Test flag. This flag is used to make behavior of some methods deterministic for test purposes.
        /// Call this method before running any tests which can be connected to methods of this class.</para>
        /// <para>For now, it's used by <see cref="SetCulture(string)"/> only
        /// to ensure CultureInfo ignores user overrides in Windows Control Panel.</para>
        /// </summary>
        public static void SetTestMode(bool testMode)
        {
            gTestMode = testMode;
        }

        /// <summary>
        /// Returns a resource stream in the specified assembly or throws if the resource cannot be found.
        /// </summary>
        /// <param name="fullResourceName">Full resource name.</param>
        /// <param name="assemblyType">A Type (class) that belongs to Assembly where the resource is located.</param>
        public static Stream FetchResourceStream(string fullResourceName, Type assemblyType)
        {
            // We can't use Assembly.GetCallingAssembly() because of code optimization.
            Stream stream = assemblyType.Assembly.GetManifestResourceStream(fullResourceName);
            if (stream == null)
                throw new InvalidOperationException(string.Format("Cannot find resource '{0}'.", fullResourceName));
            return stream;
        }

        /// <summary>
        /// Returns a resource stream in Aspose.Foundation.dll assembly or throws if the resource cannot be found.
        /// Note: after merging the Aspose.Foundation.dll assembly becomes Aspose.Words.dll.
        /// </summary>
        /// <param name="fullResourceName">Full resource name.</param>
        internal static Stream FetchResourceStream(string fullResourceName)
        {
            return FetchResourceStream(fullResourceName, typeof(ArrayUtil));
        }

        /// <summary>
        /// Opens a stream from a URI or from a path. If it is a URI with a scheme,
        /// then the content is copied into a memory stream and the memory stream is returned.
        /// Uses default timeout 100000 milliseconds (100 seconds).
        /// </summary>
        public static Stream OpenStreamFromHref(string href)
        {
            const int defaultTimeout = 100000;
            return OpenStreamFromHref(href, defaultTimeout);
        }

        /// <summary>
        /// Opens a stream from a URI or from a path. If it is a URI with a scheme,
        /// then the content is copied into a memory stream and the memory stream is returned.
        /// </summary>
        /// <param name="href">Request URI.</param>
        /// <param name="timeout">Web request timeout, measured in milliseconds.</param>
        public static Stream OpenStreamFromHref(string href, int timeout)
        {
            return UriUtil.IsHrefWithScheme(href)
                    ? WebRequestHelper.OpenStreamFromUri(href, timeout)
                    : File.OpenRead(href);
        }

        /// <summary>
        /// Executes web request with specified request data.
        /// </summary>
        public static MemoryStream ExecuteWebRequest(string requestUri, string username, string password)
        {
            WebRequest webRequest;
            try
            {
                webRequest = WebRequest.Create(requestUri);
            }
            catch (NotSupportedException)
            {
                return null;
            }

            webRequest.Credentials = new NetworkCredential(username, password);

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                MemoryStream memoryStream = new MemoryStream();
                StreamUtil.CopyStream(webResponse.GetResponseStream(), memoryStream);
                webResponse.Close();
                return memoryStream;
            }
        }

        /// <summary>
        /// Gets all font file names registered in Windows and stores them in the specified hashtable.
        /// The key and the value is the full file name of the font.
        /// <param name="fileNames">Dictionary to store filenames.</param>
        /// </summary>
        public static void GetFontFileNamesFromRegistry(StringToObjDictionary<string> fileNames)
        {
            try
            {
                string windowsFontsFolder = GetWindowsFontsFolder();

                // WORDSNET-4912 Don't load fonts from registry if we don't have access to windows fonts folder.
                if (!StringUtil.HasChars(windowsFontsFolder))
                    return;

                using (RegistryKey fontsKey =
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false))
                {
                    GetFontFilesForKey(fileNames, fontsKey, windowsFontsFolder);
                }

                // WORDSNET-19531 On Win10 MW also uses current user registry for fonts.
                using (RegistryKey fontsKey =
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false))
                {
                    GetFontFilesForKey(fileNames, fontsKey, windowsFontsFolder);
                }
            }
            catch
            {
                // Silence the exceptions. We don't want to crash when accessing font files.
            }
        }

        private static void GetFontFilesForKey(StringToObjDictionary<string> fileNames, RegistryKey fontsKey,
            string windowsFontsFolder)
        {
            if (fontsKey == null)
                return;


            string[] fontNames = fontsKey.GetValueNames();
            foreach (string fontName in fontNames)
            {
                try
                {
                    string fileName = (string)fontsKey.GetValue(fontName);
                    if (!Path.IsPathRooted(fileName))
                    {
                        fileName = Path.Combine(windowsFontsFolder, fileName);
                    }

                    fileNames[fileName] = fileName;
                }
                catch (ArgumentException)
                {
                    // WORDSNET-5564 If registry entry contains illegal characters then Path methods could throw
                    // exception. Ignore such entries.
                }
            }

        }

        public static string GetWindowsFontsFolder()
        {
            try
            {
                return Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "Fonts");
            }
            catch (SecurityException)
            {
                // WORDSNET-4912.
                // Consider there is no system fonts folder if access to environment variables is denied.
                return string.Empty;
            }
        }

        /// <summary>
        /// This method for java. Probably doesn't needed in .Net. Doesn't tested under .Net.
        /// </summary>
        public static string[] GetLinuxFontFolders()
        {
            return new string[]
            {
                GetLinuxUserFontFolder(), // user's local fonts' directory ~/.fonts
                "/usr/share/fonts", // Ubuntu, openSUSE
                "/usr/local/share/fonts", // Fedora?
                "/usr/X11R6/lib/X11/fonts" // RHEL?
            };
        }

        private static string GetLinuxUserFontFolder()
        {
            string userHome = Environment.GetEnvironmentVariable("HOME");
            return StringUtil.HasChars(userHome) ? Path.Combine(userHome, ".fonts") : "";
        }

        public static string[] GetMacOSFontsFolder()
        {
            return new string[]
            {
                GetMacOSUserFontFolder(),
                "/Library/Fonts",
                "/System/Library/Fonts/"
            };
        }

        public static string[] GetIosFontsFolder()
        {
            // https://support.apple.com/en-us/HT201722
            return new string[]
            {
                GetIosUserFontFolder(),
                "/Library/Fonts",
                "/System/Library/Fonts/",
                "/System Folder/Fonts/",
                "/Network/Library/Fonts/",
            };
        }

        private static string GetMacOSUserFontFolder()
        {
            string userHome = Environment.GetEnvironmentVariable("HOME");
            return StringUtil.HasChars(userHome) ? Path.Combine(Path.Combine(userHome, "Library"), "Fonts") : "";
        }

        private static string GetIosUserFontFolder()
        {
            string userHome = Environment.GetEnvironmentVariable("user.home");
            return StringUtil.HasChars(userHome) ? Path.Combine(Path.Combine(userHome, "Library"), "Fonts") : "";
        }

        /// <summary>
        /// This just returns some tick count in milliseconds. It can be current time or the time since the system started.
        /// </summary>
        public static int GetTickCount()
        {
            return Environment.TickCount;
        }

        /// <summary>
        /// Gets specific culture by it name.
        /// </summary>
        public static CultureInfo GetCulture(string culture)
        {
            return new CultureInfo(culture, !gTestMode);
        }

        /// <summary>
        /// Gets specific culture by it id.
        /// </summary>
        public static CultureInfo GetCulture(int lcid)
        {
            return new CultureInfo(lcid, !gTestMode);
        }

        /// <summary>
        /// Gets specific culture by it name.
        /// </summary>
        public static CultureInfo TryGetCulture(string cultureName)
        {
            try
            {
                return GetCulture(cultureName);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets specific culture by it id.
        /// </summary>
        public static CultureInfo TryGetCulture(int lcid)
        {
            try
            {
                return GetCulture(lcid);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the culture name of the current thread culture in the "language2-country2" format.
        /// </summary>
        public static string GetCurrentCultureName()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        /// <summary>
        /// Gets the current thread culture.
        /// </summary>
        public static CultureInfo GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        [JavaDelete("No analog in Java.")]
        public static CultureInfo GetCurrentUICulture()
        {
            return Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Gets the culture name of the current thread culture in the ISO 639-1 two-letter code format.
        /// </summary>
        public static string GetCurrentCultureTwoLetterName()
        {
            return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        }

        /// <summary>
        /// Gets the system default LCID from Windows registry. Returns 0 if the value cannot be obtained.
        /// </summary>
        public static int GetSystemDefaultLcid()
        {
            if (gSystemLcid != 0)
                return gSystemLcid;

            try
            {
                using (RegistryKey fontsKey =
                    Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Nls\Language", false))
                {
                    if (fontsKey == null)
                        return 0;

                    string value = fontsKey.GetValue("Default").ToString();
                    fontsKey.Close();
                    gSystemLcid = FormatterPal.ParseHex(value);

                    return gSystemLcid;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Sets the culture for the current thread e.g. "ru-RU".
        /// </summary>
        public static CultureInfo SetCulture(string culture)
        {
            CultureInfo cultureInfo = GetCulture(culture);
            return SetCulture(cultureInfo);
        }

        public static CultureInfo SetCulture(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            return cultureInfo;
        }

        [JavaDelete("No analog in Java.")]
        public static CultureInfo SetUICulture(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            return cultureInfo;
        }

        /// <summary>
        /// Sets the "standard development" culture, which is "en-nz".
        /// </summary>
        /// <remarks>
        /// For test purposes only.
        /// </remarks>
        public static void SetStandardCulture()
        {
            SetCulture(gStandardCulture);
        }

        [JavaDelete("No analog in Java.")]
        public static void SetStandardUICulture()
        {
            SetUICulture(gStandardCulture);
        }

        /// <summary>
        /// Saves currect culture to a ThreadStatic field. Use <see cref="RestoreCulture"/>to restore the saved culture.
        /// </summary>
        public static void SaveCulture()
        {
            if (gOldCultures == null)
                gOldCultures = new Stack<CultureInfo>();
            gOldCultures.Push(GetCurrentCulture());
        }

        /// <summary>
        /// Saves currect UI culture to a ThreadStatic field. Use <see cref="RestoreUICulture"/>to restore the saved culture.
        /// </summary>
        [JavaDelete("No analog in Java.")]
        public static void SaveUICulture()
        {
            if (gOldUICultures == null)
                gOldUICultures = new Stack<CultureInfo>();
            gOldUICultures.Push(GetCurrentUICulture());
        }

        /// <summary>
        /// Restores the culture that was saved with <see cref="SaveCulture"/>.
        /// </summary>
        public static void RestoreCulture()
        {
            Debug.Assert(gOldCultures.Count != 0);
            SetCulture(gOldCultures.Pop());
        }

        /// <summary>
        /// Restores the UI culture that was saved with <see cref="SaveUICulture"/>.
        /// </summary>
        [JavaDelete("No analog in Java.")]
        public static void RestoreUICulture()
        {
            Debug.Assert(gOldUICultures.Count != 0);
            SetUICulture(gOldUICultures.Pop());
        }

        [ThreadStatic]
        private static Stack<CultureInfo> gOldCultures;
        [ThreadStatic]
        private static Stack<CultureInfo> gOldUICultures;
        /// <summary>
        /// This is the most "human" culture. It uses numbers like 123.45 (decimal dot) and dates like dd/mm/yyyy.
        /// </summary>
        private static readonly CultureInfo gStandardCulture = new CultureInfo("en-nz", false);
        /// <summary>
        /// LCID cached value from Windows Registry.
        /// </summary>
        private static int gSystemLcid;

        public static string[] GetAndroidFontFolders()
        {
            return new string[]
            {
                "/system/fonts",
                "/system/font",
                "/data/fonts",
            };
        }


        private static bool gTestMode = false;
    }
}
