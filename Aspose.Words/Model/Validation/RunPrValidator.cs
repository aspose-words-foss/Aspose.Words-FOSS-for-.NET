// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2009 by Roman Korchagin
using Aspose.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fonts;
using Aspose.Words.Themes;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// <para>Visits all nodes of a document and can perform the following tasks:</para>
    /// <para>1. Collect names of fonts used throughout the document. It helps to <see cref="DocumentValidator"/>
    /// to update <see cref="FontInfoCollection"/> of whole document.</para>
    /// <para>2. Expand theme font names into normal formatting.</para>
    /// </summary>
    /// <remarks>
    /// This is in a separate document visitor to declutter the main document validator. But it requires
    /// an extra loop through a document. If that proves slow we should hook this class to the document validator methods.
    /// </remarks>
    internal class RunPrValidator : RunPrCollectorBase
    {
        /// <summary>
        /// <para>Visits all nodes of a document and can collect of used font names and update theme colors.</para>
        /// </summary>
        /// <param name="doc">Visited document.</param>
        /// <param name="usedFontNames">Found font names are added here. If it's <c>null</c>, font names shall not be collected.</param>
        internal void Execute(DocumentBase doc, ISetGeneric<string> usedFontNames)
        {
            mTheme = doc.GetThemeInternal();
            mUsedFontNames = usedFontNames;

            doc.Accept(this);

            if (mUsedFontNames != null)
                AddDefaultsToUsedFontNames();
        }

        protected override void DoHandleRunPr(RunPr runPr)
        {
            if (runPr == null)
                return;

            if (mUsedFontNames != null)
            {
                CollectFontNames(runPr);

                // WORDSNET-9373 Collect font names also inside format revisions.
                if (runPr.HasFormatRevision)
                    CollectFontNames((RunPr)runPr.FormatRevision.RevPr);
            }

            ThemeColorUpdater.Update(runPr, mTheme);
        }

        /// <summary>
        /// <para>Marks theme fonts as used. Some of them can be unused actually but we add unused style fonts
        /// to FontInfos, so it's consistent to add theme fonts as used too. MS Word does it too.</para>
        /// <para>Also adds Times New Roman. It looks like MS Word always does it.</para>
        /// </summary>
        private void AddDefaultsToUsedFontNames()
        {
            Debug.Assert(mUsedFontNames != null);

            if (mTheme != null)
            {
                foreach (ThemeFontCore themeFont in RunPr.ThemeFontAttrs)
                {
                    string fontName = ((IThemeProvider)mTheme).GetFontName(themeFont);
                    if (StringUtil.HasChars(fontName))
                        mUsedFontNames.Add(fontName);
                }
            }

            mUsedFontNames.Add(RunPr.DefaultNameAscii);

            // AS It's good to add them too but currently they are the same as NameAscii and they will unlikely be changed.
            Debug.Assert(object.Equals(RunPr.FetchDefaultAttr(FontAttr.NameBi), RunPr.FetchDefaultAttr(FontAttr.NameAscii)));
            Debug.Assert(object.Equals(RunPr.FetchDefaultAttr(FontAttr.NameFarEast), RunPr.FetchDefaultAttr(FontAttr.NameAscii)));
            Debug.Assert(object.Equals(RunPr.FetchDefaultAttr(FontAttr.NameOther), RunPr.FetchDefaultAttr(FontAttr.NameAscii)));
        }

        private void CollectFontNames(RunPr runPr)
        {
            Debug.Assert(mUsedFontNames != null);

            foreach (int fontAttr in RunPr.FontNameAttributes)
            {
                ComplexFontName fontName = (ComplexFontName)runPr.GetDirectAttr(fontAttr);
                if (fontName != null)
                    mUsedFontNames.Add(ComplexFontName.Resolve(fontName, mTheme));
            }
        }

        private Theme mTheme;

        private ISetGeneric<string> mUsedFontNames;

    }
}
