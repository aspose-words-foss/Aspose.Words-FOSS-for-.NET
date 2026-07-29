// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Saving;

namespace Aspose.Words.Tests.Gold.Txt
{
    /// <summary>
    /// Base class for export to TXT tests.
    /// </summary>
    public class TestGoldTxtBase : TestGoldBase
    {
        /// <summary>
        /// See base class for more info.
        /// </summary>
        internal static void VerifyExport(string relSrcFileName)
        {
            relSrcFileName = Aspose.TestFx.TestFxUtil.CorrectPath(relSrcFileName);
            VerifyExport(Open(relSrcFileName), relSrcFileName, null);
        }

        /// <summary>
        /// See base class for more info.
        /// </summary>
        internal static void VerifyExport(Document doc, string relSrcFileName, TxtSaveOptions saveOptions)
        {
            VerifyExport(doc, relSrcFileName, "", saveOptions);
        }

        /// <summary>
        /// Exports to TXT format and verifies against gold file.
        /// </summary>
        /// <remarks>
        /// See base class for more info.
        /// </remarks>
        internal static void VerifyExport(Document doc, string relSrcFileName, string suffix, TxtSaveOptions saveOptions)
        {
            relSrcFileName = Aspose.TestFx.TestFxUtil.CorrectPath(relSrcFileName);

            if (saveOptions == null)
                saveOptions = new TxtSaveOptions();

            VerifyTextGold(doc, relSrcFileName, suffix, GoldLevel.ExportOnly, saveOptions);
        }
    }
}
