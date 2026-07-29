// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/14/2013 by Alexey Noskov

namespace Aspose.TestFx.GoldComparers
{
    public class Comparables
    {
        public Comparables(string[] comparableOuts, string[] comparableGolds, string[] comparableOriginalGolds)
        {
            mComparableOuts = comparableOuts;
            mComparableGolds = comparableGolds;
            mComparableOriginalGolds = comparableOriginalGolds;
        }

        public string[] ComparableOuts
        {
            get { return mComparableOuts; }
        }

        public string[] ComparableGolds
        {
            get { return mComparableGolds; }
        }

        public string[] ComparableOriginalGolds
        {
            get { return mComparableOriginalGolds; }
        }

        private readonly string[] mComparableOuts;
        private readonly string[] mComparableGolds;
        private readonly string[] mComparableOriginalGolds;
    }
}