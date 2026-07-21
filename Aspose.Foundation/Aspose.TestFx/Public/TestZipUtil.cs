// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using Aspose.TestFx.GoldComparers;
using Aspose.TestFx.GoldComparers.Factory;

namespace Aspose.TestFx
{
    public static class TestZipUtil
    {
        public static void VerifyFile(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMs)
        {
            ComparerParams comparerParams = new ComparerParams();
            comparerParams.Title = title;
            comparerParams.FileNameSrc = fileNameSrc;
            comparerParams.FileNameOut = fileNameOut;
            comparerParams.FileNameGold = fileNameGold;
            comparerParams.FileNameMs = fileNameMs;
            ComparerFactoryProducer.Factory.ZipComparer.Execute(comparerParams);
        }
    }
}
