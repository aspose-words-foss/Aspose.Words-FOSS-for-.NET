// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2019 by Alexander Zhiltsov

using Aspose.Collections;
using Aspose.Words.Lists;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Allows normalizing indexes of style definition (Istd) that are stored in the <see cref="Style.Istd"/> properties.
    /// </summary>
    /// <remarks>
    /// After deleting the styles, unused Istds are formed, which, combined with the restriction of the maximum
    /// value of Istd (4094), reduces the possible number of styles that can be added to the style collection.
    /// So, Istd normalization is needed at this case.
    /// </remarks>
    internal class StyleIstdNormalizer : IstdVisitor
    {
        /// <summary>
        /// Cannot create instances of this class from outside.
        /// </summary>
        private StyleIstdNormalizer()
        {
        }

        /// <summary>
        /// Executes normalization of indexes of style definition for the specified document.
        /// </summary>
        internal static void Execute(DocumentBase document)
        {
            StyleIstdNormalizer normalizer = new StyleIstdNormalizer();
            normalizer.ExecuteCore(document);
        }

        protected override void OnRunPr(RunPr runPr)
        {
            if (runPr.Contains(FontAttr.Istd))
                runPr.Istd = GetNewIstd(runPr.Istd);
        }

        protected override void OnParaPr(ParaPr paraPr)
        {
            if (paraPr.Contains(ParaAttr.Istd))
                paraPr.Istd = GetNewIstd(paraPr.Istd);
        }

        protected override void OnRowPr(TablePr tablePr)
        {
            if (tablePr.Contains(TableAttr.Istd))
                tablePr.Istd = GetNewIstd(tablePr.Istd);
        }

        protected override void OnDocPr(DocPr docPr)
        {
            docPr.ClickTypeParaStyleIstd = GetNewIstd(docPr.ClickTypeParaStyleIstd);
            docPr.DefaultTableStyleIstd = GetNewIstd(docPr.DefaultTableStyleIstd);
        }

        /// <summary>
        /// Performs normalization of Istds for the specified document.
        /// </summary>
        private void ExecuteCore(DocumentBase document)
        {
            mDocument = document;

            GenerateIstdMap();

            // Visit all document content and update style Istd in node properties.
            Run(document);

            UpdateListStyleIstds();
            UpdateStyles();
        }

        /// <summary>
        /// Generates new Istds and a map from an old Istd to a new Istd.
        /// </summary>
        private void GenerateIstdMap()
        {
            // Let's keep order of Istds, it is used on writing styles.
            Style[] styles = new Style[mDocument.Styles.MaxIstd + 1];
            foreach (Style style in mDocument.Styles)
                styles[style.Istd] = style;

            foreach (Style style in styles)
            {
                if ((style != null) && (style.Istd > StyleCollection.MaxFixedIstd))
                {
                    mIstdMap[style.Istd] = mNextIstd;
                    mNextIstd++;
                }
            }
        }

        /// <summary>
        /// Updates Istds of styles, also including base, linked and next style Istds.
        /// </summary>
        private void UpdateStyles()
        {
            foreach (Style style in mDocument.Styles)
            {
                // Need to get all depending style Istd before assigning style Istd, since depending style Istds may
                // be reset on the assignment.
                int linkedStyleNewIstd = GetNewIstd(style.LinkedIstd);
                int baseStyleNewIstd = GetNewIstd(style.BasedOnIstd);
                // Next style may be not preserved during document cleanup: need to reset it, otherwise it may become to
                // refer another style after normalization.
                int nextStyleIstd = (mDocument.Styles.GetByIstd(style.NextIstd, false) != null)
                    ? GetNewIstd(style.NextIstd)
                    : StyleIndex.Nil;

                if (style.Istd > StyleCollection.MaxFixedIstd)
                    style.SetIstd(GetNewIstd(style.Istd), false);

                style.LinkedIstd = linkedStyleNewIstd;
                style.BasedOnIstd = baseStyleNewIstd;
                style.NextIstd = nextStyleIstd;
            }

            mDocument.Styles.UpdateIstdMap();
        }

        /// <summary>
        /// Updates list definition style Istds.
        /// </summary>
        private void UpdateListStyleIstds()
        {
            foreach (ListDef listDef in mDocument.Lists.ListDefs)
            {
                listDef.ListStyleIstd = GetNewIstd(listDef.ListStyleIstd);

                foreach (ListLevel level in listDef.Levels)
                {
                    // If a level is not used, its style is removed by document cleaner: need update it, otherwise it
                    // may become referred to another style after normalization.
                    level.ParaStyleIstd = (mDocument.Styles.GetByIstd(level.ParaStyleIstd, false) != null)
                        ? GetNewIstd(level.ParaStyleIstd)
                        : StyleIndex.Nil;
                }
            }
        }

        /// <summary>
        /// Returns a new Istd for the specified old Istd.
        /// </summary>
        /// <remarks>
        /// The property returns a negative value if the specified old Istd is an Istd of a fixed style.
        /// </remarks>
        internal int GetNewIstd(int oldIstd)
        {
            // Need return a negative value for a fixed style to not set fixed style explicitly when a style property
            // returns a default value.
            if (oldIstd <= StyleCollection.MaxFixedIstd)
                return oldIstd;

            int newIstd = mIstdMap[oldIstd];

            // If it is an Istd that didn't exist, we need to return an Istd that does not exist too.
            if (newIstd < 0)
                newIstd = (oldIstd < mNextIstd) ? mNextIstd : oldIstd;

            return newIstd;
        }

        private DocumentBase mDocument;
        private readonly IntToIntDictionary mIstdMap = new IntToIntDictionary();

        /// <summary>
        /// A new Istd for the next old Istd.
        /// </summary>
        private int mNextIstd = StyleCollection.MaxFixedIstd + 1;
    }
}
