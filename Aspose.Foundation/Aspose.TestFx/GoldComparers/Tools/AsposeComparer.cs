// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx.GoldComparers
{
    internal static class AsposeComparer
    {
        public static Comparables Convert(
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            IToComparableConverter toComparableConverter)
        {
            string[] comparableOuts = toComparableConverter.Convert(fileNameOut);
            string[] comparableGolds = toComparableConverter.Convert(fileNameGold);
            string[] comparableOriginalGolds = null;
            if (fileNameOriginalGold != null)
                comparableOriginalGolds = toComparableConverter.Convert(fileNameOriginalGold);

            Comparables comparables = new Comparables(comparableOuts, comparableGolds, comparableOriginalGolds);
            return comparables;
        }

        private static ComparerFormResult ExecuteTestUI(
           string testName,
           string fileNameOut,
           string fileNameGold,
           string fileNameOriginalGold,
           string fileNameSrc,
           string fileNameMS,
           string message,
           ComparableFileType comparableFileType,
           string comparableOut,
           string comparableGold,
           string comparableOriginalGold,
           bool topmost = false,
           bool restorePosition = false)
        {
            if (fileNameOriginalGold == fileNameGold)
                fileNameOriginalGold = null;
            if (fileNameOriginalGold == null)
                comparableOriginalGold = null;

            string cmdLine;
            string difflabels = TestSettings.Learning ? string.Join(",", TestSettings.Labels) : string.Empty;
            switch (comparableFileType)
            {
                case ComparableFileType.Image:
                    cmdLine = string.Format(CmdCompareImage, comparableOut, comparableGold, comparableOriginalGold, fileNameSrc, message, difflabels);
                    break;
                case ComparableFileType.Text:
                    string filesToShow = (string.IsNullOrEmpty(fileNameOriginalGold))
                                             ? string.Format("{0},{1}", fileNameOut, fileNameGold)
                                             : string.Format("{0},{1},{2}", fileNameOut, fileNameGold, fileNameOriginalGold);
                    cmdLine = string.Format(CmdCompareText, comparableOut, comparableGold, comparableOriginalGold, fileNameSrc, testName,
                        filesToShow);
                    break;
                case ComparableFileType.Zip:
                    cmdLine = string.Format(CmdCompareZip, fileNameSrc, comparableOut, comparableGold, comparableOriginalGold, fileNameMS, testName, difflabels);
                    break;
                case ComparableFileType.Mobi:
                    cmdLine = string.Format(CmdCompareMobi, fileNameSrc, comparableOut, comparableGold, comparableOriginalGold, fileNameMS, testName);
                    break;
                case ComparableFileType.Azw3:
                    cmdLine = string.Format(CmdCompareAzw3, fileNameSrc, comparableOut, comparableGold, comparableOriginalGold, fileNameMS, testName);
                    break;
                default:
                    throw new ArgumentException("Unexpected comparable file type.");
            }
            if (topmost)
                cmdLine += " /topmost:";
            if (restorePosition)
                cmdLine += " /restoreposition:";
            lock (gSyncObj)
            {
                return (ComparerFormResult)TestUtilPal.ExecuteProcess(gAsposeTestUI.GetExe(), cmdLine);
            }
        }

        private static ComparerFormResult ExecuteTestUI(
            string testName,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS,
            string message,
            ComparableFileType comparableFileType,
            bool topMost = false,
            bool restorePostion = false)
        {
            return ExecuteTestUI(testName, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, fileNameMS,
                message, comparableFileType, fileNameOut, fileNameGold, fileNameOriginalGold, topMost, restorePostion);
        }

        public static ComparerFormResult ExecuteImageTestUI(string fileNameOut, string fileNameGold, string fileNameOriginalGold, string fileNameSrc, string message)
        {
            return ExecuteTestUI(null, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, null, message,
                ComparableFileType.Image);
        }

        public static ComparerFormResult ExecuteTextTestUI(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS,
            bool topmost,
            bool restorePosition)
        {
            return ExecuteTestUI(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc,
                fileNameMS, null, ComparableFileType.Text, topmost, restorePosition);
        }

        public static ComparerFormResult ExecuteZipTestUI(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS)
        {
            return ExecuteTestUI(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, fileNameMS, null,
                ComparableFileType.Zip);
        }

        public static ComparerFormResult ExecuteMobiTestUI(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS)
        {
            return ExecuteTestUI(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, fileNameMS, null,
                ComparableFileType.Mobi);
        }

        public static ComparerFormResult ExecuteAzw3TestUI(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS)
        {
            return ExecuteTestUI(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, fileNameMS, null,
                ComparableFileType.Azw3);
        }

        public static ComparerFormResult ExecuteComparableTestUI(
            string testName,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string message,
            ComparableFileType comparableFileType,
            string comparableOut,
            string comparableGold,
            string comparableOriginalGold)
        {
            return ExecuteTestUI(testName, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, null, message,
                comparableFileType, comparableOut, comparableGold, comparableOriginalGold);
        }

        private const string CmdCompareImage = "/task:CompareImage /out:\"{0}\" /gold:\"{1}\" /originalGold:\"{2}\" /src:\"{3}\" /message:\"{4}\" /difflabels:\"{5}\"";
        private const string CmdCompareText = "/task:CompareText /out:\"{0}\" /gold:\"{1}\" /originalGold:\"{2}\" /src:\"{3}\" /title:\"{4}\" /filesToShow:\"{5}\"";
        private const string CmdCompareZip = "/task:CompareZip /src:\"{0}\" /out:\"{1}\" /gold:\"{2}\" /originalGold:\"{3}\" /ms:\"{4}\" /title:\"{5}\" /difflabels:\"{6}\"";
        private const string CmdCompareMobi = "/task:CompareMobi /src:\"{0}\" /out:\"{1}\" /gold:\"{2}\" /originalGold:\"{3}\" /ms:\"{4}\" /title:\"{5}\"";
        private const string CmdCompareAzw3 = "/task:CompareAzw3 /src:\"{0}\" /out:\"{1}\" /gold:\"{2}\" /originalGold:\"{3}\" /ms:\"{4}\" /title:\"{5}\"";

        private static readonly AbstractGoldComparer.Tool gAsposeTestUI = new AbstractGoldComparer.Tool("Aspose.TestUI\\bin\\Aspose.TestUI.exe");
        private static readonly object gSyncObj = new object();
    }
}
