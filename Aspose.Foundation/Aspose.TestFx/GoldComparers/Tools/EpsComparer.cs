// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/05/2023 by Denis Panov

using System;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx.GoldComparers
{
    internal class EpsComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            string srcFileName = comparerParams.FileNameSrc;
            string outFileName = comparerParams.FileNameOut;
            string goldFileName = comparerParams.FileNameGold;;

            RasterizePostScript(srcFileName, outFileName);
            ImageFileComparer.Execute(outFileName, goldFileName, null);
        }


        private static void RasterizePostScript(string epsFileName, string outFileName)
        {
            string exeName = gGswin.GetExe();

            string cmdLine =
                String.Format(
                    gGswin.GetCmdLine2(),
                    outFileName,
                    epsFileName);

            lock (gIsEpsLockObject)
            {
                string output;
                int exitCode = TestUtilPal.ExecuteProcess(exeName, cmdLine, out output);

                if (exitCode != 0)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            "gswin64c.exe failed, exit code: {0}. \r\nFile: {1} \r\nMessage: {2}", exitCode, epsFileName, output));
                }
            }
        }

        public override void VerifyConformance(ComparerParams comparerParams)
        {
            string epsFileName = comparerParams.FileNameOut;
            string exeName = gGswin.GetExe();

            lock (gIsEpsLockObject)
            {
                string output;
                int exitCode = TestUtilPal.ExecuteProcess(exeName, String.Format(gGswin.GetCmdLine1(), epsFileName), out output);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException(String.Format(
                        "Encapsulated PostScript file {0} is not valid: \n{1}",
                        epsFileName, output));
                }
            }
        }

        static EpsComparer()
        {
            gGswin = TestUtilPal.Is64BitWindows()
                ? new Tool("PostScript-tools\\gs64\\gswin64c.exe",
                    new string[]
                    {
                        "-sDEVICE=nullpage -dNOPAUSE -dBATCH \"{0}\"",
                        "-dSAFER -dBATCH -dNOPAUSE -dEPSCrop -sDEVICE=png16m -dGraphicsAlphaBits=4 -r96 -sOutputFile=\"{0}\" \"{1}\""
                    })
                : new Tool("PostScript-tools\\gs32\\gswin32c.exe",
                    new string[]
                    {
                        "-sDEVICE=nullpage -dNOPAUSE -dBATCH \"{0}\"",
                        "-dSAFER -dBATCH -dNOPAUSE -dEPSCrop -sDEVICE=png16m -dGraphicsAlphaBits=4 -r96 -sOutputFile=\"{0}\" \"{1}\""
                    });
        }

        private static readonly object gIsEpsLockObject = new object();
        private static readonly Tool gGswin;
    }
}
