// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers
{
    public abstract class AbstractGoldComparer
    {
        [JavaThrows(true)]
        public virtual void Execute(ComparerParams comparerParams) { }

        // Only FailedGoldTestStorageEngine uses this method
        [JavaThrows(true)]
        public virtual bool ExecuteForm(ComparerParams comparerParams) { return false; }

        [JavaThrows(true)]
        public virtual void VerifyConformance(ComparerParams comparerParams) { }

        /// <summary>
        /// When out does not match gold we can ananlyze further and pass the test if conditions are right.
        /// As part of this we can also automatically accept out as new gold.
        ///
        /// If we return False then typically the caller will show compare form.
        /// </summary>
        /// <returns>True if test shall pass, False otherwise.</returns>
        public static bool TryPassAutomatically(string fileNameOut, string fileNameGold)
        {
            bool acceptAlways = TestSettings.AcceptGoldAlways;
            bool passed = false;

            // If we accept any outcome then there is no point in computing that outcome
            // We need to want to compare images in order to want to compute and/or compare labels.
            if (!acceptAlways && TestSettings.CompareImages)
#if ANDROID
                passed = true; // hack for easy testing with android VM
#else
                passed = ImageFileComparer.CompareDocumentsAsImages(fileNameOut, fileNameGold);
#endif

            bool acceptIfPassed = TestSettings.AcceptGoldWhenPass;
            if (acceptAlways || (acceptIfPassed && passed))
            {
                TestFxUtil.AcceptNewGold(fileNameOut, fileNameGold);
                Console.WriteLine(string.Format("Test {0}, accepted new gold '{1}'", passed ? "passed" : "failed", fileNameGold));

                // even if test failed we accepted new gold which means now we treat test as passed
                return true;
            }

            // We do not have to accept new gold when test passed with a difference
            return passed;
        }

        public enum DocumentType
        {
            Img = 0,
            Pdf = 1,
            Xps = 2,
            Ps = 4,
            Emf = 5,
            Txt = 6,
            Woff = 7,
            Zip = 8,
            Mhtml = 9,
            Pcl = 10,
            Eps = 11,
            Mobi = 12,
            Azw3 = 13
        }

        /// <summary>
        /// This class represents Test Tool. 
        /// It keeps the path to the .exe file and keeps command line arguments.
        /// </summary>
        public class Tool
        {
            public Tool(string toolExe) : this(toolExe, null)
            {
            }

            public Tool(string toolExe, params string[] cmd)
            {
                mToolExe = TestEnvironment.GetDirTools() + TestEnvironment.NormalizePath(toolExe);
                mCmdLine = cmd;
            }

            private readonly string[] mCmdLine;
            private readonly string mToolExe;

            public string GetExe() { return mToolExe; }
            public string GetCmdLine1() { return mCmdLine[0]; }
            public string GetCmdLine2() { return mCmdLine[1]; }

            public string[] CmdLine
            {
                [CodePorting.Translator.Cs2Cpp.CppConstMethod]
                get { return mCmdLine; }
            }

            public override string ToString()
            {
                StringBuilder str = new StringBuilder();
                str.Append(mToolExe);
                str.Append(": ");
                foreach (string line in CmdLine)
                    str.Append(line).Append(" | ");

                return str.ToString();
            }
        }
    }
}
