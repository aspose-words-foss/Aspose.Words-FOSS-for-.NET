// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx.GoldComparers
{
    internal class XpsComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            Execute(comparerParams.Title, comparerParams.FileNameOut, comparerParams.FileNameMs,
                TestSettings.RoundXpsPoints ? RoundingXpsPagePartComparer.Instance : null);
        }

        public override void VerifyConformance(ComparerParams comparerParams)
        {
            VerifyConformance(comparerParams.FileNameOut);
        }

        public static void Execute(string testName, string outFileName, string originalFileName, IPackagePartComparer comparer = null)
        {
            VerifyConformance(outFileName);

            string goldFileName = TestFxUtil.BuildGoldFileName(testName, "", ".xps");
            new ZipFileComparer().Execute(
                "EXPORT XPS",
                originalFileName,
                outFileName,
                goldFileName,
                null,
                comparer);
        }

        public static void VerifyConformance(string xpsFileName)
        {
            // RK To properly launch the right isXps.exe I need to know the bitness of the OS, not the bitness of the current process.
            string exeName = gXps.GetExe();

            lock (gIsXpsLockObject)
            {
                string output;
                // WORDSNET-7570 Change TEMP folder for isXps.exe to avoid conflict with TeamCity. We can't use Path.GetTempPath() because it returns TC temp folder.
                string tempDir = TestEnvironment.GetLocalAppDataTmp();
                int exitCode = TestUtilPal.ExecuteProcess(exeName, String.Format(gXps.GetCmdLine1(), xpsFileName), tempDir, out output);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException(String.Format(
                        "XPS file {1} does not conform: \n{0}",
                        output, xpsFileName));
                }
            }
        }

        static XpsComparer()
        {
            gXps = TestUtilPal.Is64BitWindows()
                ? new Tool("xps-tools\\amd64\\isXps.exe", new string[] { "-f=\"{0}\"" })
                : new Tool("xps-tools\\x86\\isXps.exe", new string[] { "-f=\"{0}\"" });
        }

        private static readonly object gIsXpsLockObject = new object();
        private static readonly Tool gXps;
    }
}
