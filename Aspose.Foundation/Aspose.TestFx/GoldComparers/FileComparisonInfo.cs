// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2007 by Vladimir Averkin
using System.IO;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Stores information about results of 'gold' import/export test of a disk file.
    /// </summary>
    public class FileComparisonInfo : ComparisonInfo
    {
        public FileComparisonInfo(string filenameSrc, string filenameOut, string filenameGold, string filenameMS)
        {
            mFilenameSrc = filenameSrc;
            mFilenameOut = filenameOut;
            mFilenameGold = filenameGold;
            mFilenameMS = filenameMS;
        }

        public FileComparisonInfo(string filenameSrc, string filenameOut, string filenameGold, string filenameOriginalGold, string filenameMS)
            : this(filenameSrc, filenameOut, filenameGold, filenameMS)
        {
            mFilenameOriginalGold = filenameOriginalGold;
        }

        /// <summary>
        /// You must call this to update the comparison info status.
        /// </summary>
        public virtual void UpdateStatus()
        {
            ResetStatus();
            if (File.Exists(mFilenameOut))
            {
                mStatusOut = ComparisonStatus.Ok;

                if (File.Exists(mFilenameGold))
                    mStatusGold = TestFxUtil.CompareFiles(mFilenameOut, mFilenameGold) ? ComparisonStatus.Ok : ComparisonStatus.Different;
                else
                    mStatusGold = ComparisonStatus.Missing;

                if (mFilenameOriginalGold != null && File.Exists(mFilenameOriginalGold))
                    mStatusOriginalGold = TestFxUtil.CompareFiles(mFilenameOut, mFilenameOriginalGold) ? ComparisonStatus.Ok : ComparisonStatus.Different;
                else
                    mStatusOriginalGold = ComparisonStatus.Missing;

                if (File.Exists(mFilenameMS))
                    mStatusMS = TestFxUtil.CompareFiles(mFilenameOut, mFilenameMS) ? ComparisonStatus.Ok : ComparisonStatus.Different;
                else
                    mStatusMS = ComparisonStatus.Missing;
            }
            else
            {
                mStatusOut = ComparisonStatus.Missing;
                mStatusGold = (File.Exists(mFilenameGold)) ? ComparisonStatus.Different : ComparisonStatus.Missing;
                mStatusOriginalGold = (mFilenameOriginalGold != null && File.Exists(mFilenameOriginalGold)) ? ComparisonStatus.Different : ComparisonStatus.Missing;
                mStatusMS = (File.Exists(mFilenameMS)) ? ComparisonStatus.Different : ComparisonStatus.Missing;
            }
        }

        public string FilenameSrc
        {
            get { return mFilenameSrc; }
        }

        public string FilenameOut
        {
            get { return mFilenameOut; }
        }

        public string FilenameGold
        {
            get { return mFilenameGold; }
        }

        public string FilenameOriginalGold
        {
            get { return mFilenameOriginalGold; }
        }

        public string FilenameMS
        {
            get { return mFilenameMS; }
        }

        private readonly string mFilenameSrc;
        private readonly string mFilenameOut;
        private readonly string mFilenameGold;
        private readonly string mFilenameOriginalGold;
        private readonly string mFilenameMS;
    }
}
