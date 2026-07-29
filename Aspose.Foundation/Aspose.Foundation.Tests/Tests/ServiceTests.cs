// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2014 by Ivan Lyagin

#if !NETSTANDARD
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests
{
    /// <summary>
    /// Provides set of service tests used to maintain Aspose.Words infrastructure. Tests contained in this class do not test
    /// any functionality but perform several useful tasks instead. It is convenient to treat them as tests to be able to
    /// launch them directly from IDE.
    /// </summary>
    [TestFixture]
    [JavaDelete("Do not need this class on Java.")]
    public class ServiceTests
    {
        /// <summary>
        /// Generates source files by templates using configuration defined at AutoGenConfig.xml.
        /// </summary>
        [Test, Ignore("Not a test.")]
        public void GenerateSourceFilesByTemplates()
        {
            Process process = RunConsoleApp(
                @"X:\awnet\Aspose.Foundation\Tools\Aspose.CsClassTransformer\Aspose.CsClassTransformer.ConsoleApp\bin\Aspose.CsClassTransformer.ConsoleApp.exe",
                @"X:\awnet\AutoGenConfig.xml");

            Debug.Write(process.StandardOutput.ReadToEnd()); // This is to use Resharper's output window.

            process.WaitForExit(); // See remarks here: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput.aspx.
        }

        private static Process RunConsoleApp(string fileName, string arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            return process;
        }

        /// <summary>
        /// Logs all boxing occurances found in Aspose.Words.dll and Aspose.Foundation.dll to .\TestData\Logs\AllBoxings.txt file.
        /// </summary>
        [Test, Ignore("Not a test.")]
        public void WriteAllBoxingsLog()
        {
            WriteBadPatternLog("a", "AllBoxings.txt");
        }

        /// <summary>
        /// Logs boxing-involving-collection occurances found in Aspose.Words.dll and Aspose.Foundation.dll 
        /// to .\TestData\Logs\CollectionBoxings.txt file.
        /// </summary>
        [Test, Ignore("Not a test.")]
        public void WriteCollectionBoxingsLog()
        {
            WriteBadPatternLog("c", "CollectionBoxings.txt");
        }

        /// <summary>
        /// Logs unique boxing-involving-collection occurances found in Aspose.Words.dll and Aspose.Foundation.dll 
        /// to .\TestData\Logs\CollectionBoxings.txt file.
        /// </summary>
        [Test, Ignore("Not a test.")]
        public void WriteUniqueCollectionBoxingsLog()
        {
            WriteBadPatternLog("z", "UniqueCollectionBoxings.txt");
        }

        /// <summary>
        /// Not a test. Just prints current <see cref="TestSettings"/> settings.
        /// Allows to know test settings used by CI Jenkins (See WORDSNET-21914 for details).
        /// </summary>
        [Test]
        public void PrintTestSettings()
        {
            IDictionary<string, TestSetting> settings = TestSettings.GetAllSettings();
            foreach (KeyValuePair<string, TestSetting> item in settings)
                Console.WriteLine(string.Format("{0}={1}", item.Key, item.Value));
            Console.WriteLine(string.Format("Global config file: {0}", TestSettings.IsGlobalConfigFile));
        }

        private static void WriteBadPatternLog(string key, string fileName)
        {
            string arguments = string.Format(
                @"/{0} X:\awnet\Aspose.Words\bin\net2.0\Aspose.Words.dll X:\awnet\Aspose.Words\bin\net2.0\Aspose.Foundation.dll",
                key);

            Process process = RunConsoleApp(
                @"X:\awnet\Aspose.Foundation\Tools\BadPatternFinder\BadPatternFinder\bin\Scc\BadPatternFinder.exe",
                arguments);

            using (FileStream stream = new FileStream(@"X:\awnet\TestData\Logs\" + fileName, FileMode.Create, FileAccess.Write))
                StreamUtil.CopyStream(process.StandardOutput.BaseStream, stream);

            process.WaitForExit(); // See remarks here: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput.aspx.
        }
    }
}
#endif
