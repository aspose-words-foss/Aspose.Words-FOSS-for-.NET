// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

namespace Aspose.TestFx.GoldComparers
{
    internal class EmfComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            string testName = comparerParams.Title;
            string outFileName = comparerParams.FileNameOut;
            string originalImageFileName = comparerParams.FileNameSrc;

            string goldFileName = TestFxUtil.BuildGoldFileName(testName, "", ".emf");
            ImageFileComparer.Execute(outFileName, goldFileName, originalImageFileName);
        }
    }
}
