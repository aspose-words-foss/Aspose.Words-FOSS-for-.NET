// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/01/2013 by Alexey Butalov

namespace Aspose.TestFx.GoldComparers.FailedTestCollector
{
    ///<summary>
    /// Contains information about failed gold tests.
    /// </summary>
    public class FailedGoldTestInfo
    {
        public FailedGoldTestInfo()
        {
            mGoldFileType = GoldTestFileType.Unknown;
            mTestName = string.Empty;
            mTitle = string.Empty;
            mFileNameSrc = string.Empty;
            mFileNameOut = string.Empty;
            mFileNameGold = string.Empty;
            mFileNameOriginalGold = string.Empty;
            mFileNameMS = string.Empty;
            mId = string.Empty;
        }

        public GoldTestFileType GoldFileType
        {
            get { return mGoldFileType; }
            set { mGoldFileType = value; }
        }

        public string TestName
        {
            get { return mTestName; }
            set { mTestName = value; }
        }

        public string FileNameMS
        {
            get { return mFileNameMS; }
            set { mFileNameMS = value; }
        }

        public string FileNameGold
        {
            get { return mFileNameGold; }
            set { mFileNameGold = value; }
        }

        /// <summary>
        /// When JavaGold file is exist, .Net gold is stored as 'Original Gold'.
        /// </summary>
        public string FileNameOriginalGold
        {
            get { return mFileNameOriginalGold; }
            set { mFileNameOriginalGold = value; }
        }

        public string FileNameOut
        {
            get { return mFileNameOut; }
            set { mFileNameOut = value; }
        }

        public string FileNameSrc
        {
            get { return mFileNameSrc; }
            set { mFileNameSrc = value; }
        }

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        public string Id
        {
            get { return mId; }
            set { mId = value; }
        }

        private string mTitle;
        private string mFileNameSrc;
        private string mFileNameOut;
        private string mFileNameGold;
        private string mFileNameOriginalGold;
        private string mFileNameMS;
        private string mTestName;
        private string mId;
        private GoldTestFileType mGoldFileType;
    }
}