// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/08/2007 by Vladimir Averkin

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Base class to store information about the results of 'gold' import/export test.
    /// In 'gold' tests, 'out' files, produced by Aspose.Words, are compared with the files 
    /// produced during previous tests and saved as 'gold'.
    /// Files can be also compared against 'ms' files, produced by MS Word.
    /// </summary>
    public abstract class ComparisonInfo
    {
        /// <summary>
        /// Status of the 'out' file or package part.
        /// 'Ok', if file is present and 'Missing' if file is missing.
        /// </summary>
        public ComparisonStatus OutStatus
        {
            get { return mStatusOut; }
        }

        /// <summary>
        /// Status of the 'gold' file.
        /// 'Ok', if file is present, 'Missing' if file is missing, and 'Different' if file is different from 'out' file.
        /// </summary>
        public ComparisonStatus GoldStatus
        {
            get { return mStatusGold; }
        }

        /// <summary>
        /// Status of the 'original gold' file. When JavaGold file is exist, .Net gold is stored as 'Original Gold'.
        /// 'Ok', if file is present, 'Missing' if file is missing, and 'Different' if file is different from 'out' file.
        /// </summary>
        public ComparisonStatus OriginalGoldStatus
        {
            get { return mStatusOriginalGold; }
        }

        /// <summary>
        /// Status of the 'ms' file.
        /// 'Ok', if file is present, 'Missing' if file is missing, and 'Different' if file is different from 'out' file.
        /// </summary>
        public ComparisonStatus MSStatus
        {
            get { return mStatusMS; }
        }

        public void ResetStatus()
        {
            mStatusOut = ComparisonStatus.Different;
            mStatusGold = ComparisonStatus.Different;
            mStatusOriginalGold = ComparisonStatus.Different;
            mStatusMS = ComparisonStatus.Different;
        }

        protected ComparisonStatus mStatusOut;
        protected ComparisonStatus mStatusGold;
        protected ComparisonStatus mStatusOriginalGold;
        protected ComparisonStatus mStatusMS;

        public static bool IsXml(string fileName)
        {
            return
#if CPLUSPLUS
                fileName.EndsWith(".html") ||
#endif
                fileName.EndsWith(".ncx") ||
                fileName.EndsWith(".opf") ||
                fileName.EndsWith(".svg") ||
                fileName.EndsWith(".xml") ||
                fileName.EndsWith(".xaml") ||
                fileName.EndsWith(".wml") ||
                fileName.EndsWith(".wordml") ||
                fileName.EndsWith(".fopc") ||
                fileName.EndsWith(".rels") ||
                fileName.EndsWith(".fpage") ||
                fileName.EndsWith(".fdoc") ||
                fileName.EndsWith(".fdseq");
        }

        public static bool IsPng(string fileName)
        {
            return fileName.EndsWith(".png");
        }
        
    }
}
