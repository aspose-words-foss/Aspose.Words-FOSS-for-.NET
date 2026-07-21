// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2015 by Alexander Zhiltsov

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Collects all warnings and asserts if number of warnings is more than MaxCount.
    /// </summary>
    public class TestMaxWarningCountCallback : TestWarningCallback
    {
        public TestMaxWarningCountCallback(int maxWarningCount)
        {
            mMaxCount = maxWarningCount;
        }

        public int MaxCount
        {
            get { return mMaxCount; }
        }

        protected override void WarningAdded()
        {
            if (MaxCount >= 0 && Count > MaxCount)
                Assert.Fail(string.Format("Too many warnings (expected {0}).", MaxCount));
        }

        private int mMaxCount;
    }
}
