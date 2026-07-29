// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2017 by Edward Voronov

using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Collects imported styles istds from particular sources.
    /// </summary>
    internal class ImportedStylesIstdsCollector
    {
        internal IntToIntBidirectionalMap GetImportedStylesIstds(string sourceFileName)
        {
            IntToIntBidirectionalMap importedIstds = mCache[sourceFileName];
            if (importedIstds == null)
            {
                importedIstds = new IntToIntBidirectionalMap();
                mCache[sourceFileName] = importedIstds;
            }

            return importedIstds;
        }

        private readonly StringToObjDictionary<IntToIntBidirectionalMap> mCache = new StringToObjDictionary<IntToIntBidirectionalMap>();
    }
}