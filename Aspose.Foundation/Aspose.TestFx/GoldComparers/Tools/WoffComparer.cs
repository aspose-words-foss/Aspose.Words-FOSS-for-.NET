// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx.GoldComparers
{
    internal class WoffComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            string woffFileName = comparerParams.FileNameOut;

            lock (gIsWoffLockObject)
            {
                string exeName = gWoffValidator.GetExe();
                string output;
                string tempDir = TestEnvironment.GetLocalAppDataTmp();
                int exitCode = TestUtilPal.ExecuteProcess(exeName, string.Format(gWoffValidator.GetCmdLine1(), woffFileName), tempDir, out output);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException(String.Format(
                        "Woff file {1} does not pass validation: \n{0}",
                        output, woffFileName));
                }
            }
        }

        private static readonly object gIsWoffLockObject = new object();
        private static readonly Tool gWoffValidator = new Tool("WoffValidator\\validate.exe", new string[] { "\"{0}\"" });
    }
}
