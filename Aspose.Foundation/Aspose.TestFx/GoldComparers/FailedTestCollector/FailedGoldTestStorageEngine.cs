// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/01/2013 by Alexey Butalov

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Aspose.JavaAttributes;
using Aspose.TestFx.GoldComparers.Factory;

namespace Aspose.TestFx.GoldComparers.FailedTestCollector
{
    /// <summary>
    /// Helps to process failed gold tests.
    /// </summary>
    /// <remarks>
    /// We need the gold tests postprocessing feature for CI (Jenkins). Please see WORDSNET-7572
    /// It's a part of the mechanism which allows CI users to see CI gold test problems.
    /// Note: This class is also used in Aspose.Foundation\Tools\GoldDiffViewer\GoldDiffViewer.csproj project.
    /// </remarks>
    [JavaManual]
    public class FailedGoldTestStorageEngine
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="problemTestsBaseDir">Directory where problem test gold files are stored.</param>
        /// <param name="configFileName">Configuration file name.</param>
        public FailedGoldTestStorageEngine(string problemTestsBaseDir, string configFileName)
        {
            Debug.Assert(StringUtil.HasChars(problemTestsBaseDir));
            Debug.Assert(StringUtil.HasChars(configFileName));
            mProblemTestsBaseDir = problemTestsBaseDir;
            mConfigFileName = configFileName;
        }

        /// <summary>
        /// Ctor. Uses default configuration file name.
        /// </summary>
        /// <param name="problemTestsBaseDir">Directory where problem test gold files are stored.</param>
        public FailedGoldTestStorageEngine(string problemTestsBaseDir)
            : this(problemTestsBaseDir, gDefaultConfigFileName)
        {
        }

        /// <summary>
        /// Reads information about failed gold tests.
        /// </summary>
        /// <returns>Array of <see cref="FailedGoldTestInfo" /></returns>
        public FailedGoldTestInfo[] ReadFailedTests()
        {
            if (!File.Exists(ConfigFileFullName))
                throw new InvalidOperationException(string.Format("Configuration file '{0}' not found!", ConfigFileFullName));

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigFileFullName);
            if (xmlDoc.DocumentElement == null)
                return new FailedGoldTestInfo[0];

            XmlElement testRoot = xmlDoc.DocumentElement;
            List<FailedGoldTestInfo> failedTests = new List<FailedGoldTestInfo>();
            foreach (XmlNode testInfoNode in testRoot.ChildNodes)
            {
                if (!string.Equals(testInfoNode.Name, gTestElement, StringComparison.OrdinalIgnoreCase))
                    continue;

                FailedGoldTestInfo testInfo = new FailedGoldTestInfo();

                XmlAttribute testNameAttr = null;
                XmlAttribute testFileTypeAttr = null;
                if (testInfoNode.Attributes != null)
                {
                    XmlAttribute titleAttr = testInfoNode.Attributes[gTestTitleAttr];
                    if (titleAttr != null)
                        testInfo.Title = titleAttr.Value;
                    testNameAttr = testInfoNode.Attributes[gTestNameAttr];
                    if (testNameAttr != null)
                        testInfo.TestName = testNameAttr.Value;
                    testFileTypeAttr = testInfoNode.Attributes[gTestFileTypeAttr];
                    if (testFileTypeAttr != null)
                        testInfo.GoldFileType = (GoldTestFileType)Enum.Parse(typeof(GoldTestFileType), testFileTypeAttr.Value, true);
                }

                if ((testNameAttr == null) || (testFileTypeAttr == null))
                {
                    Debug.Assert(false);
                    continue;
                }

                foreach (XmlNode node in testInfoNode.ChildNodes)
                    NodeToTestInfo(node, testInfo);

                failedTests.Add(testInfo);
            }

            return failedTests.ToArray();
        }

        ///<summary>
        /// Saves information about a failed gold test.
        /// </summary>
        internal void SaveFailedTest(FailedGoldTestInfo testInfo)
        {
            const string mutexName = "Global\\AsposeFailedGoldTestStorageEngineSaveMutex";
            const int mutexWaitTimeout = 10000;

            Mutex mutex = new Mutex(false, mutexName);
            if (!mutex.WaitOne(mutexWaitTimeout))
                throw new InvalidOperationException("Timeout waiting for exclusive access to write config file!");

            try
            {
                if (!Directory.Exists(mProblemTestsBaseDir))
                    Directory.CreateDirectory(mProblemTestsBaseDir);

                MethodBase testMethod = FailedGoldTestUtil.GetTestMethod();
                if (testMethod == null)
                {
                    Debug.Assert(false);
                    return;
                }

                string testName = testMethod.Name;
                string testOutputDir = Path.Combine(mProblemTestsBaseDir, testName);
                if (!Directory.Exists(testOutputDir))
                    Directory.CreateDirectory(testOutputDir);

                testInfo.TestName = testName;
                CopyFilesToOutputDir(testInfo, testOutputDir);
                WriteConfigFile(testInfo);
            }
            finally
            {
                mutex.ReleaseMutex();
#if !CPLUSPLUS
                mutex.Close();
#endif
            }
        }

        private void OpenTestFile(string testName, string testFileName)
        {
            string fullFileName = GetFullTestFileName(testName, testFileName);
            ProcessStartInfo psi = new ProcessStartInfo(fullFileName);
            psi.WorkingDirectory = Path.GetDirectoryName(fullFileName);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private static void NodeToTestInfo(XmlNode node, FailedGoldTestInfo testInfo)
        {
            switch (node.Name)
            {
                case gFileNameSrcElement:
                    testInfo.FileNameSrc = ReadFileNameValue(node);
                    break;
                case gFileNameGoldElement:
                    testInfo.FileNameGold = ReadFileNameValue(node);
                    break;
                case gFileNameMSElement:
                    testInfo.FileNameMS = ReadFileNameValue(node);
                    break;
                case gFileNameOutElement:
                    testInfo.FileNameOut = ReadFileNameValue(node);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Copies source test files to an output folder.
        /// </summary>
        private static void CopyFilesToOutputDir(FailedGoldTestInfo testInfo, string testOutputDir)
        {
            List<string> fileList = new List<string>();
            if (StringUtil.HasChars(testInfo.FileNameSrc))
                fileList.Add(testInfo.FileNameSrc);
            fileList.Add(testInfo.FileNameOut);
            fileList.Add(testInfo.FileNameGold);
            if (StringUtil.HasChars(testInfo.FileNameMS))
                fileList.Add(testInfo.FileNameMS);

            foreach (string fullFileName in fileList)
            {
                if (!File.Exists(fullFileName))
                    continue;

                string fileName = Path.GetFileName(fullFileName);
                if (fileName == null)
                {
                    Debug.Fail("Wrong file name: " + fullFileName);
                    continue;
                }

                string destFullFileName = Path.Combine(testOutputDir, fileName);
                File.Copy(fullFileName, destFullFileName, true);
            }
        }

        /// <summary>
        /// Writes fail information to the configuration file.
        /// This configuration file is used to restore the fail on a user computer.
        /// </summary>
        private void WriteConfigFile(FailedGoldTestInfo testInfo)
        {
            XmlDocument xmlDoc = new XmlDocument();

            if (File.Exists(ConfigFileFullName))
            {
                using (TextReader textReader = new StreamReader(ConfigFileFullName))
                {
                    xmlDoc.Load(textReader);
                }
            }

            if (xmlDoc.DocumentElement == null)
                xmlDoc.AppendChild(xmlDoc.CreateElement(gTestRootElement));

            XmlElement testRoot = xmlDoc.CreateElement(gTestElement);
            testRoot.SetAttribute(gTestNameAttr, testInfo.TestName);
            testRoot.SetAttribute(gTestTitleAttr, testInfo.Title);
            testRoot.SetAttribute(gTestFileTypeAttr, testInfo.GoldFileType.ToString());
            xmlDoc.DocumentElement.AppendChild(testRoot);

            if (StringUtil.HasChars(testInfo.FileNameSrc))
                AppendFileNameElement(xmlDoc, testRoot, gFileNameSrcElement, testInfo.FileNameSrc);
            AppendFileNameElement(xmlDoc, testRoot, gFileNameOutElement, testInfo.FileNameOut);
            AppendFileNameElement(xmlDoc, testRoot, gFileNameGoldElement, testInfo.FileNameGold);
            if (StringUtil.HasChars(testInfo.FileNameMS))
                AppendFileNameElement(xmlDoc, testRoot, gFileNameMSElement, testInfo.FileNameMS);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = new string(' ', 4);
            using (XmlWriter xmlWriter = XmlWriter.Create(ConfigFileFullName, settings))
            {
                xmlDoc.Save(xmlWriter);
            }
        }

        private static void AppendFileNameElement(XmlDocument xmlDoc, XmlElement rootElement, string name, string fileName)
        {
            XmlElement xmlElement = xmlDoc.CreateElement(name);
            rootElement.AppendChild(xmlElement);
            XmlText xmlText = xmlDoc.CreateTextNode(Path.GetFileName(fileName));
            xmlElement.AppendChild(xmlText);
        }

        private static string ReadFileNameValue(XmlNode node)
        {
            return (node.ChildNodes.Count > 0) ? node.ChildNodes[0].Value : string.Empty;
        }

        private string GetFullTestFileName(string testName, string testFileName)
        {
            if (string.IsNullOrEmpty(testFileName))
                return testFileName;

            string path = Path.Combine(mProblemTestsBaseDir, testName);
            return Path.Combine(path, testFileName);
        }

        /// <summary>
        /// Configuration file name with path.
        /// </summary>
        private string ConfigFileFullName
        {
            get { return Path.Combine(mProblemTestsBaseDir, mConfigFileName); }
        }

        /// <summary>
        /// Configuration file name without path.
        /// </summary>
        private readonly string mConfigFileName;
        /// <summary>
        /// Path to the base directory where problem test gold files are stored.
        /// </summary>
        private readonly string mProblemTestsBaseDir;

        private const string gDefaultConfigFileName = "config.testlist";
        private const string gTestRootElement = "testlist";
        private const string gTestElement = "test";
        private const string gTestNameAttr = "name";
        private const string gTestTitleAttr = "title";
        private const string gTestFileTypeAttr = "filestype";
        private const string gFileNameSrcElement = "fileNameSrc";
        private const string gFileNameOutElement = "fileNameOut";
        private const string gFileNameGoldElement = "fileNameGold";
        private const string gFileNameMSElement = "fileNameMS";
        private const string gFilesToShowElementItem = "file";
    }
}
