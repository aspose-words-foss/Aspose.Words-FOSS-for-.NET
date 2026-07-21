// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2026 by Edward Voronov

using Aspose.TestFx.GoldComparers;
using Aspose.TestFx.GoldComparers.Factory;

namespace Aspose.TestFx
{
    public static class TestImageUtil
    {
        public static void VerifyFile(string fileNameOut, string fileNameGold, string fileNameSource)
        {
            ComparerParams comparerParams = new ComparerParams();
            comparerParams.FileNameOut = fileNameOut;
            comparerParams.FileNameGold = fileNameGold;
            comparerParams.FileNameSrc = fileNameSource;
            ComparerFactoryProducer.Factory.ImageComparer.Execute(comparerParams);
        }
    }
}
