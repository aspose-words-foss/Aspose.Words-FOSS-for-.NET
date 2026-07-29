// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2013 by Michael Morozoff

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Aspose.JavaAttributes;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx
{
    /// <summary>
    /// Provides test run wide settings used by the verification UI.
    /// </summary>
    public static class TestSettings
    {
        /// <summary>
        /// RK We need this overload because CPPPORTER does not yet support default parameter values.
        /// </summary>
        public static void Set(string id, object value)
        {
            Set(-1, id, value, null);
        }

        /// <summary>
        /// Sets value of the setting.
        /// This is used to set default values, or change value at runtime.
        /// </summary>
        /// <param name="index">This is used when value is created for the first time.
        /// The index affects placement of value in the test settings UI.
        /// It is not used when key was already used to create a setting</param>
        /// <param name="key">A key of the setting.</param>
        /// <param name="value">A value. This must be of actual type: bool, float, string or string[].
        /// When setting is created this type determines what <see cref="TestSetting.Kind"/> would be.</param>
        /// <param name="attached">All the text associated with the setting. In the configuraton file
        /// this are all comment lines before the key=value.</param>
        private static void Set(int index, string key, object value, params string[] attached)
        {
            lock (gValuesLock)
            {
                ArgumentUtil.CheckHasChars(key, "key");
                Init();

                TestSetting v = gValues.GetValueOrNull(key.ToLower());
                if (v == null)
                {
                    // attached values as stored as-is in file thus we want to add ";" at start
                    for (var i = 0; i < attached.Length; i++)
                    {
                        // null and empty values are stored as empty strings
                        if (attached[i] == null) attached[i] = string.Empty;
                        if (attached[i].Length > 0) attached[i] = ";" + attached[i];
                    }

                    v = TestSetting.Create(index, key, value, attached);
                }
                else
                {
                    v.UpdateValue(value);
                }

                gValues[key.ToLower()] = v;
            }
        }

        /// <summary>
        /// Gets a test setting.
        /// If at the time this method is called the Alt+Shift combination is pressed,
        /// this method displays the dialog box where the user can see and set the test settings.
        /// </summary>
        private static TestSetting Get(string id)
        {
            lock (gValuesLock)
            {
                ArgumentUtil.CheckHasChars(id, "id");
                Init();
#if !NETSTANDARD && !CPLUSPLUS
                TestUtilPal.ShowTestSettingsFormIfNeeded();
#endif
                TestSetting v = gValues.GetValueOrNull(id);
                return v;
            }
        }

        /// <summary>
        /// Returns True if specified settings is set to True.
        /// </summary>
        public static bool Is(string id)
        {
            return Get(id).AsBool;
        }

        /// <summary>
        /// Provides access to all of the test settings.
        /// The key is the setting name, the value is the <see cref="TestSetting"/> object.
        /// </summary>
        public static IDictionary<string, TestSetting> GetAllSettings()
        {
            lock (gValuesLock)
            {
                Init();
                return new Dictionary<string, TestSetting>(gValues);
            }
        }

        /// <summary>
        /// Returns True if comparing xps documents shall use rounding for points.
        /// This is useful for layout tests to ignore minor differencies in layout.
        /// Its much like ignoring minor pixel differencies.
        ///
        /// You should only use this when many tests are failing and you want to check if its's because of rounding issues.
        /// If using this option test passes you a likely safe to accept new gold.
        /// </summary>
        public static bool RoundXpsPoints { get { return Is(B_ROUNDXPS); } }

        /// <summary>
        /// Whether test passes or fails out must be accepted as new gold.
        /// This is used when many golds fail because of the Windows update or other
        /// external factor wher AW code has not changed since all tests passed before.
        /// </summary>
        public static bool AcceptGoldAlways { get { return Get(A_AUTOACCEPT).AsArray[0] == I_AUTOACCEPT_ALWAYS; } }

        /// <summary>
        /// Accept out as new gold when test passed and there was a difference.
        /// This happens when out does not match gold but test passed due to comparer result or other decision.
        /// </summary>
        public static bool AcceptGoldWhenPass { get { return Get(A_AUTOACCEPT).AsArray[0] == I_AUTOACCEPT_PASS; } }

        /// <summary>
        /// Returns image difference labels which shall pass the test
        /// </summary>
        public static string[] Labels
        {
            get { return Get(S_LABELS).AsString.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries); }
            set { Set(S_LABELS, value); }
        }

        public static bool CompareLogical { get { return Is(B_COMPARELOGICAL); } }

        public static bool CompareImages { get { return Get(A_COMPAREIMAGES).AsArray[0] != I_COMPAREIMAGES_DONT; } }

        /// <summary>
        /// True if image difference label learning more is activated.
        /// In this mode when compare fails and difference label does not match
        /// the form will have buttons to manually select difference label.
        /// </summary>
        public static bool Learning { get { return Is(B_LEARN) && PassWhenImagesDifferenceLabelsMatch; } }


        /// <summary>
        /// True if test shall pass when out and gold compared as images are binary equal.
        /// </summary>
        public static bool PassWhenImagesBinaryEqual
        {
            get{ return Get(A_COMPAREIMAGES).AsArray[0] != I_COMPAREIMAGES_DONT; }
        }

        /// <summary>
        /// True if test shall pass when out and gold compared as images have the same pixels.
        /// </summary>
        public static bool PassWhenImagesPixelsMatch
        {
            get
            {
                switch (Get(A_COMPAREIMAGES).AsArray[0])
                {
                    case I_COMPAREIMAGES_DONT:
                    case I_COMPAREIMAGES_PASS_BINARY:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// True if test shall pass when out and gold compared as images have the same pixels.
        /// </summary>
        public static bool PassWhenImagesDifferenceLabelsMatch
        {
            get
            {
                switch (Get(A_COMPAREIMAGES).AsArray[0])
                {
                    case I_COMPAREIMAGES_DONT:
                    case I_COMPAREIMAGES_PASS_BINARY:
                    case I_COMPAREIMAGES_PASS_PIXEL:
                        return false;
                    case I_COMPAREIMAGES_PASS_LABEL:
                    case I_COMPAREIMAGES_PASS_EXPLICIT:
                        // we cannot pass by matching to undefined labels
                        return Labels.Length > 0;
                    default:
                        throw new Exception("PassWhenImagesDifferenceLabelsMatch");
                }
            }
        }

        public static bool PassExplicitLabel
        {
            get
            {
                return Get(A_COMPAREIMAGES).AsArray[0] == I_COMPAREIMAGES_PASS_EXPLICIT && Labels.Length > 0;
            }
        }


        public static bool CloseWordDocument
        {
            get { return Is(B_CLOSEDOC); }
            set { Set(B_CLOSEDOC, value); }
        }

        /// <summary>
        /// Returns True if failed tests shall throw and do not display the compare form.
        /// ScrollLock on overrides behavior and means display the compare form.
        /// </summary>
        public static bool DontShowCompareForm
        {
            get
            {
                if (TestUtilPal.IsScrollLockOn())
                    return false;

                return !Is(B_SHOWCOMPARE);
            }
            set { Set(B_SHOWCOMPARE, value); }
        }

        /// <summary>
        /// Returns True if compare form shall be displayed regardless of whether out and gold are different or not.
        /// </summary>
        public static bool ShowCompareAlways
        {
            get { return Is(B_SHOWCOMPARE_ALWAYS); }
            set { Set(B_SHOWCOMPARE_ALWAYS, value); }
        }

        /// <summary>
        /// Returns True if local golds shall be used instead of versioned golds.
        /// </summary>
        public static bool UseLocalGolds
        {
            get { return Is(B_USELOCALGOLDS); }
        }

        /// <summary>
        /// Returns True if proxy is used to access resources on the web and cached values are returned.
        /// </summary>
        public static bool ProxyWebTests
        {
            get { return Is(B_PROXYWEB); }
        }

        public static bool IgnoreWebTests
        {
            get { return Is(B_NOWEB); }
        }

        /// <summary>
        /// WORDSNET-21914 Enable PDF validation on CI server.
        /// Indicates whether the config is loaded from %USERPROFILE%\Aspose folder (global for all test assemblies)
        /// or from current test assembly output folder (unique for any test assembly).
        /// </summary>
        public static bool IsGlobalConfigFile { get; set; }

#if !JAVA && !CPLUSPLUS
        /// <summary>
        /// Ensures that all web requests are re-routed to http://aspose-web-resources/ uri.
        /// This feature is useful where test host does not have internet connectivity or when web resource has disappeared.
        /// You need to change hosts file so that 'aspose-web-resources' is mapped to '127.0.0.1'
        /// </summary>
        public static void EnableRequestRedirect()
        {
            if (!ProxyWebTests)
                return;

            lock (gValuesLock)
            {
                if (_RedirectEnabled)
                    return;

                // Create delegate which works through real System.Net.HttpRequestCreator creator
                // This allows us to send requests to real servers.
                // I do not know why .net does not allow to get existing implementation of it directly.
                // System.Net.HttpRequestCreator
                Type creatorType = typeof(WebRequest).Assembly.GetType("System.Net.HttpRequestCreator");
                object creatorInstance = Activator.CreateInstance(creatorType);
                _CreateWebRequest = (CreateDelegate)creatorInstance.GetType().GetMethod("Create")
                    .CreateDelegate(typeof(CreateDelegate), creatorInstance);

                // The host here can be anything, the port is important. Make sure to restart directory browser if changing this line.
                // Only one instance of directory browser will run.
                const string LOCAL_WEB_SERVER_URI = "http://aspose-web-resources:5555/";
                const string LOCAL_WEB_SERVER_EXE = @"X:\awnet\Aspose.Foundation\Tools\DirectoryBrowser\bin\Release\net8.0\DirectoryBrowser.exe";

                IWebRequestCreate creator = new RequestCreator(LOCAL_WEB_SERVER_URI, LOCAL_WEB_SERVER_EXE);
                // Both http and https are redirected to http only.
                HttpWebRequest.RegisterPrefix("http://", creator);
                HttpWebRequest.RegisterPrefix("https://", creator);
                _RedirectEnabled = true;
            }
        }

        private delegate WebRequest CreateDelegate(Uri uri);
        private static CreateDelegate _CreateWebRequest;
        private static bool _RedirectEnabled = false;

        private class RequestCreator : IWebRequestCreate
        {
            public RequestCreator(string uri, string exe)
            {
                Debug.Assert(!string.IsNullOrEmpty(uri));
                Debug.Assert(!string.IsNullOrEmpty(exe));

                _Uri = uri;

                // Start local web server to expose the resources
                ProcessStartInfo psi = new ProcessStartInfo(exe);
                psi.Arguments = "/url=" + _Uri + " /root=\"" + ROOT + "\"";
                Process.Start(psi);
                Debug.WriteLine(String.Format("started local web server '{0}' on '{1}'", exe, uri));
            }

            private readonly string _Uri;

            /// <summary>
            /// This is called by framework, specifically WebRequest.Create.
            /// </summary>
            public WebRequest Create(Uri uri)
            {
                try
                {
                    string replacement = null;
                    while (!_Replacements.TryGetValue(uri.AbsoluteUri, out replacement))
                        // This never returns false. Originally I thought that if downloading and caching fails
                        // then I should try direct request. However it causes massive timeouts in tests.
                        // If resource cannot be quickly downloaded then there is no point to let AW make an another attempt.
                        if (!DownloadAndCacheResource(uri))
                            break;
                    if (replacement != null)
                    {
                        string newUri = string.Format("{0}{1}", _Uri, replacement);
                        Debug.WriteLine(string.Format("WebRequestCreator, request for '{0}' was redirected to '{1}'", uri, newUri));
                        uri = new Uri(newUri);
                    }
                    return _CreateWebRequest(uri);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(string.Format("WebRequestCreator, request for '{0}' failed.\r\n{1}", uri, e));
                    throw;
                }
            }

            private bool DownloadAndCacheResource(Uri uri)
            {
                // When multiple tests run they may need to download resources all at once.
                // We need to make sure that requests wait for each other otherwise some
                // can go to the web due to recursion. It is possible that timeouts happen.
                lock (_Lock)
                {
                    // Another thread may have just downloaded the resource
                    if (_Replacements.ContainsKey(uri.AbsoluteUri))
                        return true;

                    Response response = Response.Get(uri);
                    if (!response.Success)
                        throw new Exception(string.Format("response failed with error {1}", response.StatusCode));

                    string extension = string.Empty;
                    string mimeType = response.ContentType.ToLower();
                    switch (mimeType)
                    {
                        case "image/jpeg" : extension = "jpg"; break;
                        case "image/gif" : extension = "gif"; break;
                        case "image/png" : extension = "png"; break;
                        default:
                            // Sometimes response could be success with html or json payload saying it was an error.
                            // We should not cache these apparently. But the error is misleading still.
                            throw new Exception(string.Format("add mapping for '{0}' mimetype to extension", mimeType));
                    }
                    string name = DateTime.UtcNow.Ticks.ToString("X16");
                    string folder = name.Substring(0, name.Length - 10);
                    string folderPath = Path.Combine(ROOT, folder);

                    name = name.Substring(folder.Length) + "." + extension;

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    using (FileStream fs = new FileStream(Path.Combine(folderPath, name), FileMode.CreateNew))
                        response.Stream.CopyTo(fs);

                    string key = uri.AbsoluteUri;
                    string value = string.Format("{0}/{1}", folder, name);
                    File.AppendAllText(LINKS, string.Format("{0} {1}\r\n", value, key));
                    _Replacements.Add(key, value);

                    return true;
                }
            }

            private class Response
            {
                public static Response Get(Uri uri)
                {
                    WebRequest wq = _CreateWebRequest(uri);
                    wq.Timeout = 5000;
                    wq.Method = "GET";
                    // We mapped http(s) to our handler, so we expect only HttpWebResponse here.
                    HttpWebResponse wp = (HttpWebResponse)wq.GetResponse();
                    return new Response { Success = true, ContentType = wp.ContentType, StatusCode = wp.StatusCode, Stream = wp.GetResponseStream() };
                }
                public HttpStatusCode StatusCode { get; private set;}
                public bool Success { get; private set; }
                public Stream Stream { get; private set; }
                public string ContentType { get; private set; }
            }

            private static Dictionary<string, string> LoadReplacements()
            {
                Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                if (File.Exists(LINKS))
                    foreach (string line in File.ReadAllLines(LINKS))
                    {
                        int pos = line.IndexOf(' ');
                        Debug.Assert(pos > 0);
                        string key = line.Substring(pos+1);
                        string value = line.Substring(0, pos);
                        result.Add(key, value);
                    }
                return result;
            }

            private const string ROOT = @"X:\awnet\TestData\WebResources";
            private const string LINKS = ROOT + @"\links.txt";
            private static readonly Dictionary<string, string> _Replacements = LoadReplacements();
            private static object _Lock = new object();
        }
#endif

        /// <summary>
        /// Loads the configuration settings from the file but only if they have not been loaded yet.
        /// </summary>
        private static void Init()
        {
            if (gValues == null)
                LoadConfig();
        }

        /// <summary>
        /// Resets and loads all the configuration settings.
        /// </summary>
        private static void LoadConfig()
        {
            LoadDefaults();

            string configFileName = GetConfigFileName();
            if (!File.Exists(configFileName))
            {
                configFileName = GetGlobalConfigFileName();
                if (!File.Exists(configFileName))
                    return;
                IsGlobalConfigFile = true;
            }

            // I wanted simplest possible format of the file and to not use any 3rd party libraries
            // Simplest format is key=value pair, similar to ini files but even simpler
            // However, I need a few extras:
            // - store description for the value, so that it can be used in UI to describe what this option is
            // - file must support comments, so that it is easy to edit by hand, but these comments are not needed in UI,
            //
            // Originally format was like this:
            // ; at start of line designates comment line
            // key=value is the value stored
            // ; right above the key=value is description used in UI, any further is just comment
            // all values are bools, but can be parsed from y/yes/true/1/n/no/false/0
            // in the UI all the values were sorted by key
            //
            // Now the new version has this on top of the above:
            // - values are not sorted in the UI, they appear in the file order,
            // - when file is read/written the layout is preserved, values are updates in their respective lines,
            // - blank lines designate a divider in the UI, so that controls can be grouped
            // - values are read as strings and not parsed, instead methods AsBool, AsString, AsList, etc
            // - where same key appears multiple times in the file it is considered as array of values
            // - where value is array the UI will default to first value found in file, and values are not sorted
            // - where in UI list value changes, it will swap lines in file with the new value selected

            // Read config file if present. It may replace defaults.
            using (StreamReader sr = File.OpenText(configFileName))
            {
                string line = string.Empty;
                int index = 0;
                List<string> attached = new List<string>();
                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null)
                        break;

                    line = line.Trim();

                    int p = line.IndexOf('=');

                    // lines not in key=value format, or where key is empty are ignored
                    if (p <= 0)
                    {
                        attached.Add(line);
                        continue;
                    }

                    string key = line.Substring(0, p);
                    string val = line.Substring(p + 1).Trim();

                    try
                    {
                        TestSetting v = TestSetting.Load(index++, key, val, attached.ToArray());

                        // "Autoaccept always" shall not be read from config, use default value instead
                        if (v.Key != A_AUTOACCEPT || v.AsArray[0] != I_AUTOACCEPT_ALWAYS)
                            gValues[key] = v;       // Value of the last matching key wins
                    }
                    catch
                    {
                        // Discard invalid value in file, use default instead
                    }
                    finally
                    {
                        attached.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the configuration settings to a file.
        /// </summary>
        [JavaDelete("generic array creation not work in java")]
        public static void SaveConfig()
        {
            if (gValues == null)
                return;

            string configFileName = (IsGlobalConfigFile)
                ? GetGlobalConfigFileName()
                : GetConfigFileName();
            try
            {
                using (StreamWriter sw = new StreamWriter(configFileName, false))
                {
                    KeyValuePair<string, TestSetting>[] entries = new KeyValuePair<string, TestSetting>[gValues.Count];
                    (gValues as ICollection<KeyValuePair<string, TestSetting>>).CopyTo(entries, 0);
                    Array.Sort(entries, new TestSettingIndexComparer());

                    foreach (KeyValuePair<string, TestSetting> entry in entries)
                    {
                        TestSetting item = entry.Value;
                        foreach (var attached in item.Attached)
                            sw.WriteLine(attached);
                        sw.WriteLine(entry.Key + "=" + item.GetSerializedValue());
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format("Failed to save config {0}.", configFileName), e);
            }
        }

        private class TestSettingIndexComparer : IComparer<KeyValuePair<string, TestSetting>>
        {
            public int Compare(KeyValuePair<string, TestSetting> x, KeyValuePair<string, TestSetting> y)
            {
                return x.Value.Index.CompareTo(y.Value.Index);
            }
        }

        private static void LoadDefaults()
        {
            gValues = new Dictionary<string, TestSetting>();

            int index = 0;
            Set(index++, B_NOPDFVALIDATION, true, "Do not validate PDF output");
            Set(index++, B_NOPDF, false, "Do not verify PDF output");
            Set(index++, B_NOGDI, false, "Do not verify GDI output");
            Set(index++, B_NOXPS, false, "Do not verify XPS output");
            Set(index++, B_NOXAML, false, "Do not verify XAML output");
            Set(index++, B_NOSVG, false, "Do not verify SVG output");
            Set(index++, B_NOHTML, false, "Do not verify FixedPage HTML output");
            Set(index++, B_NOPS, false, "Do not verify PostScript output");
            Set(index++, B_NOEPS, false, "Do not verify Encapsulated PostScript output");
            Set(index++, B_NOEMF, false, "Do not verify EMF output");

            Set(index++, B_SHOWCOMPARE,
#if ANDROID
            true,
#else
            false,
#endif
            "", "Show compare dialog when test fails");

            Set(index++, B_SHOWCOMPARE_ALWAYS, false, "Show compare dialog always, even if test passes");

            // TODO We should normally have list here: ignore test, use proxy, neither

            Set(index++, B_NOWEB, false, "", "Ignore tests which access internet");
            Set(index++, B_PROXYWEB, false, "Use proxy to get web resources");
            Set(index++, S_PROXYWEBSERVER, "X:/awnet/Aspose.Foundation/Tools/DirectoryBrowser/bin/Release/net8.0/DirectoryBrowser.exe", "A full path to the local image proxy server");
            Set(index++, S_PROXYWEBURI, "http://aspose-web-resources:5555/", "A full url to the local image proxy server");

            Set(index++, B_USELOCALGOLDS, false, "", "Use local set of gold files like in Java builds");
            Set(index++, B_USESHAREDOUT, false, "Use shared folder to keep out files");
            Set(index++, B_CLOSEDOC, false, "Close test document in Word before running test (under debugger only)");
            Set(index++, B_ROUNDXPS, false, "Round xps points");
            Set(index++, B_ENABLEPROGRESSESTIMATION, false, "Record layout progress in CSV files");

            Set(index++, B_COMPARELOGICAL, false, "", "Compare logical difference");
            Set(index++, A_COMPAREIMAGES, new string[]
                { I_COMPAREIMAGES_DONT, I_COMPAREIMAGES_PASS_BINARY, I_COMPAREIMAGES_PASS_PIXEL, I_COMPAREIMAGES_PASS_LABEL, I_COMPAREIMAGES_PASS_EXPLICIT },
                "Compare image difference");

            Set(index++, B_LEARN, false, "Enable manual image difference labels tagging");
            Set(index++, S_LABELS, "", "Compare image difference label with the following labels");
            Set(index++, A_AUTOACCEPT, new string[]
                { I_AUTOACCEPT_DONT,I_AUTOACCEPT_PASS, I_AUTOACCEPT_ALWAYS },
                null, "Accept out as new gold when different");
        }

        private const string A_AUTOACCEPT = "autoaccept";
        private const string I_AUTOACCEPT_DONT = "Do not accept";
        private const string I_AUTOACCEPT_PASS = "Accept when test passed with a difference";
        private const string I_AUTOACCEPT_ALWAYS = "Accept always, even when test failed";

        private const string B_USELOCALGOLDS = "uselocalgolds";
        private const string B_CLOSEDOC = "closedoc";

        private const string B_ENABLEPROGRESSESTIMATION="enableprogressestimation";

        private const string B_ROUNDXPS = "roundxps";
        private const string B_PROXYWEB = "proxyweb";
        private const string S_PROXYWEBSERVER = "proxyweb-server";
        private const string S_PROXYWEBURI = "proxyweb-uri";
        private const string B_NOWEB = "noweb";
        private const string B_NOEMF = "noemf";
        private const string B_NOEPS = "noeps";
        private const string B_NOPS = "nops";
        private const string B_NOHTML = "nohtml";
        private const string B_NOSVG = "nosvg";
        private const string B_NOXAML = "noxaml";
        private const string B_NOPDFVALIDATION = "nopdfvalidation";
        private const string B_NOPDF = "nopdf";
        private const string B_NOXPS = "noxps";
        private const string B_NOGDI = "nogdi";
        private const string B_USESHAREDOUT = "usesharedout";
        private const string LABEL_ENABLED = "label-enabled";
        private const string LABEL_PASS_MINOR = "label-pass-minor";
        private const string LABEL_PASS_NONE = "label-pass-none";
        private const string LABEL_PASS_NOISE = "label-pass-noise";
        private const string LABEL_PASS_CLIP= "label-pass-clip";

        private const string B_COMPARELOGICAL="comparelogical";

        private const string A_COMPAREIMAGES = "compareimages";
        private const string I_COMPAREIMAGES_DONT = "Do not compare";
        private const string I_COMPAREIMAGES_PASS_BINARY = "Compare and pass if images are binary equal";
        private const string I_COMPAREIMAGES_PASS_PIXEL = "Compare and pass if images have matching pixels";
        private const string I_COMPAREIMAGES_PASS_LABEL = "Compare and pass if images difference label matches";
        private const string I_COMPAREIMAGES_PASS_EXPLICIT = "Compare and pass applying first label to the difference";

        private const string B_LEARN = "learn";
        private const string S_LABELS = "labels";

        private const string B_SHOWCOMPARE = "showbinary";
        private const string B_SHOWCOMPARE_ALWAYS = "showalways";

        private static string GetConfigFileName()
        {
            return TestFxUtil.CorrectPath(Path.Combine(TestUtilPal.GetExecutingAssemblyPath(), TestConfigFileName));
        }

        /// <summary>
        /// Gets global config file name stored in %USERPROFILE% folder.
        /// Currently helps to adjust specific test setting for CI (Jenkins) builds.
        /// </summary>
        private static string GetGlobalConfigFileName()
        {
            string userProfileDir = GetUserProfileDir();
            userProfileDir = Path.Combine(userProfileDir, "Aspose");
            return TestFxUtil.CorrectPath(Path.Combine(userProfileDir, TestConfigFileName));
        }

        /// <summary>
        /// C++ implementation of Environment.GetFolderPath throws NotImplementedException for Environment.SpecialFolder.UserProfile.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        private static string GetUserProfileDir()
        {
#if JAVA
            return System.getProperty("user.home");
#else
#if NET30
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#else
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
#endif
        }

        private static Dictionary<string, TestSetting> gValues;
        private static object gValuesLock = new object();

        private const string TestConfigFileName = "TestSettings.config";
    }
}
