// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/08/2012 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.MarkupLanguage.Writer
{
    /// <summary>
    /// Collect new subsidiary content parts are generated during export. 
    /// After export will contain the array of <see cref="SubsidiaryContentPart" /> objects.
    /// </summary>
    internal class SubsidiaryContentPartCollector
    {
        internal SubsidiaryContentPartCollector()
        {
            mParts = new List<SubsidiaryContentPart>();
        }

        /// <summary>
        /// Adds new item to the list of subsidiary content parts.
        /// </summary>
        internal void AddPart(SubsidiaryContentPart part)
        {
            mParts.Add(part);
        }

        internal IList<SubsidiaryContentPart> Parts
        {
            get { return mParts; }
        }

        private readonly List<SubsidiaryContentPart> mParts;
    }
}
