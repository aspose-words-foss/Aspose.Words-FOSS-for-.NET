// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/02/2016 by Vyacheslav Durin

using System;
using System.IO;
using System.Text;
using System.Threading;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx
{
    public static class TestEnvironment
    {

        // Paths
        public static string GetTools(string path) { return NormalizePath(gDirTools + path); }
        /// <summary>
        /// This is the root (for example X:) without the slash separator at the end.
        /// </summary>
        public static string GetRawRoot() { return gRootNoSlash; }
        public static string GetDirWordsJava() { return gDirWordsJava; }
        public static string GetJavaProjectName() { return gJavaProject; }
        public static string GetDirAwnet() { return gDirAwnet; }
        public static string GetDirAwnet(string path) { return NormalizePath(gDirAwnet + path); }
        public static string GetDirCsporter() { return gDirCsporter; }
        public static string GetDirAwcpp() { return gDirAwcpp; }
        public static string GetFoundationTestData() { return gDirTestDataFoundation; }
        public static string GetFoundationTestOut() { return gDirTestOutFoundation; }
        public static string GetTestGold() { return gDirTestGold; }
        public static string GetTestGoldLocal() { return gDirTestGoldLocal; }
        public static string GetGoldDirName() { return gGoldDirName; }
        public static string GetTestData() { return gDirTestData; }
        public static string GetTestOut() { return gDirTestOut; }
        public static string GetTestData(string relativePath) { return NormalizePath(gDirTestData + relativePath); }
        public static string GetFoundationTestData(string relativePath) { return NormalizePath(gDirTestDataFoundation + relativePath); }
        public static string GetDirTools() { return gDirTools; }
        public static string GetWinFontsFolder() { return gWinFontsFolder; }

        // Env
        public static string GetUserHome() { return gUserHome; }
        public static string GetUserHome(string path) { return NormalizePath(gUserHome + path); }
        public static string GetLocalAppDataTmp() { return gLocalAppDataTmp; }
        
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        public static string GetRemoteComparerIP() { return gRemoteComparerIP; }
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        public static string GetRemoteComparerPort() { return gRemoteComparerPort; }
        public static string GetSharedOutRoot() { return gSharedOutRoot; }

        // Consts
        private static readonly string gRoot;
        private static readonly string gRootNoSlash;
        private static readonly string gDirWordsJava;
        private static readonly string gJavaProject;
        private static readonly string gDirAwnet;
        private static readonly string gDirCsporter;
        private static readonly string gDirAwcpp;
        private static readonly string gDirTestData;
        private static readonly string gDirTestGold;
        private static readonly string gDirTestGoldLocal;
        private static readonly string gGoldDirName;
        private static readonly string gDirTestOut;
        private static readonly string gDirTestDataFoundation;
        private static readonly string gDirTestOutFoundation;
        private static readonly string gDirTools;
        private static readonly string gRemoteComparerIP;
        private static readonly string gRemoteComparerPort;
        private static readonly string gSharedOutRoot;

        private static readonly string gUserHome;
        private static readonly bool gIsX64;
        
        private static readonly string gLocalAppDataTmp;
        private static readonly string gLocalAppData;
        private const string gWindowsPathSeparator = "\\";
        private const string gUnixPathSeparator = "/";
        private const string gBr = "\n";

        private static readonly string gWinFontsFolder;

        static TestEnvironment()
        {
            gRoot = GetProp("dir.root", GetProjectParentPath());

            // Originally, the exception was thrown inside the GetProjectParentPath method in case if
            // the default AW project root path is not found. So for non-AW projects using this class, 
            // the exception was always thrown and the GetEnv method was not even invoked. But let's 
            // give GetEnv a chance to override the default AW project path, even if the default path 
            // can not be extracted from the executing assembly path.
            if (string.IsNullOrEmpty(gRoot))
                throw new InvalidOperationException("Cannot initialize project parent path.");

            gRootNoSlash = RemoveLastPathSeparator(gRoot);

            gDirWordsJava = GetProp("dir.words-java", gRoot + "words-java\\");
            gJavaProject = GetDirectoryName(gDirWordsJava);


            gDirAwnet = GetProp("dir.awnet", gRoot + "awnet\\");
            gDirCsporter = GetProp("dir.csporter", gRoot + "csporter\\"); 

            gDirAwcpp = GetProp("dir.awcpp", gRoot + "words-cpp\\");

            gDirTestData = GetProp("dir.test.data", gDirAwnet + "TestData\\");
            gDirTestGold = GetProp("dir.test.gold", gDirAwnet + "TestGold\\");
            gDirTestGoldLocal = GetProp("dir.test.gold.local", gRoot + "TestGoldLocal\\");
            gDirTestOut = GetProp("dir.test.out", gDirAwnet + "TestOut\\");

            #if !CPLUSPLUS // Use cpp gold dir name for AWCPP
            gGoldDirName = GetProp("dir.test.gold.name", "TestGoldJava");
            #else
            gGoldDirName = GetProp("dir.test.gold.name", "TestGoldCpp");
            #endif

            gDirTestDataFoundation = GetProp("dir.test.data.foundation", gDirAwnet + "Aspose.Foundation\\TestData\\");
            gDirTestOutFoundation = GetProp("dir.test.out.foundation", gDirAwnet + "Aspose.Foundation\\TestOut\\");

            gDirTools = GetProp("dir.tools", gDirAwnet + "Aspose.Foundation\\Tools\\");
            if (PlatformUtilPal.IsUnixLike())
            {
                gUserHome = GetProp("HOME","");
                if (!gUserHome.EndsWith("/"))
                {
                    gUserHome += '/';
                }
            }
            else
            {
                gUserHome = "C:\\Users\\user\\";                
            }
            gLocalAppData = GetEnv("LocalAppData", gUserHome);
            gLocalAppDataTmp = (gLocalAppData != null) ? Path.Combine(gLocalAppData, "temp") : string.Empty;
            gIsX64 = TestUtilPal.Is64BitWindows();

            if (PlatformUtilPal.IsWindows())
            {
                gWinFontsFolder = SystemPal.GetWindowsFontsFolder();
            }
            else if (PlatformUtilPal.IsMacOS())
            {
                gWinFontsFolder = Path.Combine(gDirAwcpp, "fonts");
            }
            else if (PlatformUtilPal.IsUnixLike())
            {
                gWinFontsFolder = gUserHome + ".fonts";
            }

            string ip =
#if ANDROID
                "192.168.1.64";
#else
                "192.168.0.105";
#endif
            gRemoteComparerIP = GetProp("remote_comparer_ip", ip);

            gRemoteComparerPort = GetProp("remote_comparer_port", "8092");
            gSharedOutRoot = GetProp("shared_out_root");
        }

        /// <summary>
        /// Returns the directory that is the parent of the project directory.
        /// Basically, it returns "X:\" for "X:\awnet\Aspose.Words" on Windows and
        /// "/media/sf_F_DRIVE/" for "/media/sf_F_DRIVE/awnet/Aspose.Words" on Linux.
        /// </summary>
        [JavaThrows(false)]
        private static string GetProjectParentPath()
        {
            string assemblyPath = TestUtilPal.GetExecutingAssemblyPath();
            // AS: Modified this a bit so test classes in Foundation can be used in Express as well.
            // Find the first instance of "aw" or "words" in the path instead.
            int projectNamePos = assemblyPath.IndexOf(TestUtilPal.GetProjectName().Substring(0, 2), StringComparison.Ordinal);
            if (projectNamePos < 0)
            {
                projectNamePos = assemblyPath.IndexOf(TestUtilPal.GetAlternativeProjectName().Substring(0, 5), StringComparison.Ordinal);
            }

            if (projectNamePos < 0)
                return null;

            return assemblyPath.Substring(0, projectNamePos);
        }

        [JavaThrows(false)]
        public static string NormalizePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string sep = PlatformUtilPal.IsUnixLike() ? gUnixPathSeparator : gWindowsPathSeparator;
                path = PlatformUtilPal.IsUnixLike() ?
                        path.Replace(gWindowsPathSeparator, gUnixPathSeparator)
                        : path.Replace(gUnixPathSeparator, gWindowsPathSeparator);

                if (Directory.Exists(path) && !path.EndsWith(sep))
                    path += sep;
            }
            return path;
        }

        private static string GetEnv(string envKey, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(envKey);
            return NormalizePath(GetDefaultIfEmpty(value, defaultValue));
        }

        private static string GetProp(string propName)
        {
            return GetProp(propName, null);
        }

        private static string GetProp(string propName, string defaultValue)
        {
#if JAVA
            String value = System.getProperty(propName);
            if (!msString.isNullOrEmpty(value))
                return normalizePath(getDefaultIfEmpty(value, defaultValue));

#endif
            return GetEnv(propName, defaultValue);
        }

        private static string GetDefaultIfEmpty(string value, string defaultValue)
        {
            return (value != null) ? value : defaultValue;
        }

        private static string RemoveLastPathSeparator(string path)
        {
            StringBuilder str = new StringBuilder(path);
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == '/' || str[i] == '\\')
                    str.Remove(i, 1);
                else
                    break;
            }
            return str.ToString();
        }

        private static string GetDirectoryName(string path)
        {
            path = AddPathSeparatorAtEnd(path);
            return Path.GetFileNameWithoutExtension(Path.GetDirectoryName(path));
        }

        private static string AddPathSeparatorAtEnd(string path)
        {
            char lastChar = path[path.Length - 1];
            if (lastChar != '/' && lastChar != '\\')
                return path + Path.DirectorySeparatorChar;

            return path;
        }

        public static string ToStringData()
        {
            StringBuilder str = new StringBuilder();
            str.Append("gRoot=" + gRoot).Append(gBr);
            str.Append("gRawRoot=" + gRootNoSlash).Append(gBr);
            str.Append("gDirWordsJava=" + gDirWordsJava).Append(gBr);
            str.Append("gDirAwnet=" + gDirAwnet).Append(gBr);
            str.Append("gDirAwcpp=" + gDirAwcpp).Append(gBr);
            str.Append("gDirCsporter=" + gDirCsporter).Append(gBr);
            str.Append("gDirTestData=" + gDirTestData).Append(gBr);
            str.Append("gDirTestGold=" + gDirTestGold).Append(gBr);
            str.Append("gDirTestGoldLocal=" + gDirTestGoldLocal).Append(gBr);
            str.Append("gGoldDirName=" + gGoldDirName).Append(gBr);
            str.Append("gDirTestOut=" + gDirTestOut).Append(gBr);
            str.Append("gDirTestDataFoundation=" + gDirTestDataFoundation).Append(gBr);
            str.Append("gDirTestOutFoundation=" + gDirTestOutFoundation).Append(gBr);

            str.Append("gDirTools=" + gDirTools).Append(gBr);
            str.Append("gUserHome=" + gUserHome).Append(gBr);
            str.Append("gLocalAppData=" + gLocalAppData).Append(gBr);
            str.Append("gLocalAppDataTmp=" + gLocalAppDataTmp).Append(gBr);
            str.Append("gIsX64=" + gIsX64.ToString()).Append(gBr);
            str.Append("Locale=" + Thread.CurrentThread.CurrentCulture.ToString()).Append(gBr);
#if JAVA
            msStringBuilder.append(msStringBuilder.append(str, "TZ=" + CurrentThread.getTimeZone()), G_BR);
#endif
            return str.ToString();
        }

#if PLAIN_JAVA
        public static void main(String[] args) throws Exception { System.out.println(TestEnvironment.toStringData()); }
#endif
    }
}
