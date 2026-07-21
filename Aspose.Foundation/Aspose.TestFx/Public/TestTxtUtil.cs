// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using Aspose.TestFx.GoldComparers;
using Aspose.TestFx.GoldComparers.Factory;

namespace Aspose.TestFx
{
    public static class TestTxtUtil
    {
        public static void VerifyFile(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS,
            string fileNameSource)
        {
            ComparerParams comparerParams = new ComparerParams();
            comparerParams.Title = title;
            comparerParams.FileNameOut = fileNameOut;
            comparerParams.FileNameGold = fileNameGold;
            comparerParams.FileNameMs = fileNameMS;
            comparerParams.FileNameSrc = fileNameSource;
            ComparerFactoryProducer.Factory.TextComparer.Execute(comparerParams);

        }

        public static void VerifyFile(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS)
        {
            ComparerParams comparerParams = new ComparerParams();
            comparerParams.Title = title;
            comparerParams.FileNameOut = fileNameOut;
            comparerParams.FileNameGold = fileNameGold;
            comparerParams.FileNameMs = fileNameMS;
            ComparerFactoryProducer.Factory.TextComparer.Execute(comparerParams);
        }
    }
}
