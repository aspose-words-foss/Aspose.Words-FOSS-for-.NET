// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/08/2007 by Vladimir Averkin

using System;
using System.IO;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Stores information about results of 'gold' import/export test for the package part.
    /// </summary>
    public class PackagePartComparisonInfo : ComparisonInfo
    {
        internal PackagePartComparisonInfo(string name)
        {
            mName = name;
        }

        /// <summary>
        /// You must call this to update the comparison info status.
        /// </summary>
        public void UpdateStatus()
        {
            UpdateStatus(null);
        }

        /// <summary>
        /// Call this to update the comparison info status if using a custom package part comparer.
        /// </summary>
        public void UpdateStatus(IPackagePartComparer comparer)
        {
            ResetStatus();
            if (mBufferOut != null)
            {
                mStatusOut = ComparisonStatus.Ok;

                mStatusGold = CompareBuffers(comparer, mName, mBufferOut, mBufferGold, false);
                mStatusOriginalGold = CompareBuffers(comparer, mName, mBufferOut, mBufferOriginalGold, false);

                // It is nice to use XmlDiff when comparing OUT and MS - it ignores things like "UTF-8" vs "utf-8" 
                // and thus shows more "green" in the compare dialog box.
                mStatusMS = CompareBuffers(comparer, mName, mBufferOut, mBufferMS, true);
                mStatusMS = ComparisonStatus.Ok;
            }
            else
            {
                mStatusOut = ComparisonStatus.Missing;
                mStatusGold = GetStatusForMissingOut(mBufferGold);
                mStatusOriginalGold = GetStatusForMissingOut(mBufferOriginalGold);
                mStatusMS = GetStatusForMissingOut(mBufferMS);
            }
        }

        private static ComparisonStatus CompareBuffers(
            IPackagePartComparer comparer,
            string partName,
            byte[] buffer1,
            byte[] buffer2,
            bool isPreferXmlDiff)
        {
            if (buffer2 == null)
                return ComparisonStatus.Missing;

            if (comparer == null)
            {
                if (IsPng(partName))
                {
                    try
                    {
                        MemoryStream stream1 = new MemoryStream(buffer1);
                        MemoryStream stream2 = new MemoryStream(buffer2);
                        // TODO The CompareImageFiles converts streams to buffers again and compare buffers like we do below.
                        // I do not get the point of special handling of images then.
                        ImageFileComparer.CompareImageFiles(stream1, stream2, null, null);
                        return ComparisonStatus.Ok;
                    }
                    catch (Exception)
                    {
                        return ComparisonStatus.Different;
                    }
                }
                comparer = DefaultPackagePartComparer.Instance;
            }

            return comparer.CompareBuffers(partName, buffer1, buffer2, isPreferXmlDiff)
                ? ComparisonStatus.Ok
                : ComparisonStatus.Different;
        }

        private static ComparisonStatus GetStatusForMissingOut(byte[] buffer)
        {
            return (buffer != null) ? ComparisonStatus.Different : ComparisonStatus.Missing;
        }

        public string Name
        {
            get { return mName; }
        }

        public byte[] BufferOut
        {
            get { return mBufferOut; }
            set { mBufferOut = value; }
        }

        public byte[] BufferGold
        {
            get { return mBufferGold; }
            set { mBufferGold = value; }
        }

        public byte[] BufferOriginalGold
        {
            get { return mBufferOriginalGold; }
            set { mBufferOriginalGold = value; }
        }

        public byte[] BufferMS
        {
            get { return mBufferMS; }
            set { mBufferMS = value; }
        }

        private readonly string mName;
        private byte[] mBufferOut;
        private byte[] mBufferGold;
        private byte[] mBufferOriginalGold;
        private byte[] mBufferMS;
    }
}
