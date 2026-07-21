// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2006 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Lists;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Stores data during import of elements from one document into another.
    /// </summary>
    internal class ImportContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ImportContext(StyleCollection srcStyles, StyleCollection dstStyles)
            : this(srcStyles.Document, dstStyles.Document, ImportFormatMode.KeepSourceFormatting)
        {
        }

        /// <summary>
        /// Creates ImportContext object with a specified ImportFormatMode.
        /// </summary>
        internal ImportContext(DocumentBase srcDoc, DocumentBase dstDoc, ImportFormatMode importFormatMode)
            : this (srcDoc, dstDoc, importFormatMode, null)
        {
        }

        /// <summary>
        /// Creates ImportContext object with a specified ImportFormatMode and ImportFormatOptions.
        /// </summary>
        internal ImportContext(DocumentBase srcDoc, DocumentBase dstDoc, ImportFormatMode importFormatMode,
            ImportFormatOptions importFormatOptions)
            : this (srcDoc, dstDoc, importFormatMode, importFormatOptions, null)
        {
        }

        internal ImportContext(DocumentBase srcDoc, DocumentBase dstDoc, ImportFormatMode importFormatMode,
            ImportFormatOptions importFormatOptions, IntToIntBidirectionalMap importedIstds)
        {
            Debug.Assert(srcDoc != null);
            Debug.Assert(dstDoc != null);

            mSrcDoc = srcDoc;
            mDstDoc = dstDoc;
            mImportFormatMode = importFormatMode;

            mImportFormatOptions = importFormatOptions;
            if (mImportFormatOptions == null)
            {
                mImportFormatOptions = new ImportFormatOptions
                {
                    // WORDSNET-24169 Use "UseExistingLists" by default to avoid creating new lists and list definitions.
                    UseExistingLists = true
                };
            }

            mImportedIstds = (importedIstds != null) ? importedIstds : new IntToIntBidirectionalMap();

            // It seems, Word relies on the original state of themes,
            // so it should be called here before any changes to themes were made.

            // AM. I think we should not compensate theme difference if node is imported from glossary.
            if (srcDoc is Document || dstDoc is Document)
                CalculateThemeDifference(srcDoc.GetThemeInternal(), dstDoc.GetThemeInternal());

            mIsThemeColorsDifferent = !Theme.ThemeColorsEquals(srcDoc.GetThemeInternal(), dstDoc.GetThemeInternal());
        }

        /// <summary>
        /// Applies theme difference if needed.
        /// </summary>
        internal void ApplyThemeDifference(RunPr runPr)
        {
            if (IsThemeFontsDifferent)
            {
                foreach (int key in RunPr.FontNameAttributes)
                {
                    if (runPr.Contains(key))
                    {
                        ComplexFontName officeFont = (ComplexFontName)runPr[key];

                        if (officeFont.IsThemeFont && mThemeDifference.ContainsKey(officeFont.ThemeFontCore))
                            runPr.SetAttr(key, ComplexFontName.FromName(mThemeDifference[officeFont.ThemeFontCore]));
                    }
                }
            }

            if (IsThemeColorsDifferent)
            {
                // In common both Color and ThemeColor are present, so we may need to just delete ThemeColor.
                // But it will be incorrect if only ThemeColor specified. We need to correctly resolve this color.

                if (!runPr.Contains(FontAttr.Color))
                    ThemeColorUpdater.Update(runPr, DstDoc.GetThemeInternal());

                runPr.Remove(FontAttr.ThemeColor);
            }
        }

        /// <summary>
        /// Returns true if style was already imported in this context.
        /// </summary>
        internal bool IsImported(Style style)
        {
            return (ImportedIstds[style.Istd] != int.MinValue);            
        }

        /// <summary>
        /// Returns true if the specified <paramref name="listDefId"/> can be reused from the destination collection.
        /// </summary>
        internal bool CanReuseListDefId(int listDefId)
        {
            // WORDSNET-17534 The new ImportFormatOptions.KeepSourceNumbering option is introduced.
            return (!mImportFormatOptions.KeepSourceNumbering && (mNonReusableListDefId != listDefId));
        }

        /// <summary>
        /// Sets list definition identifier that cannot be reused.
        /// </summary>
        internal void SetNonReusableListDefId(long nonReusableListDefId)
        {
             mNonReusableListDefId = nonReusableListDefId;
        }

        /// <summary>
        /// Calculates difference between document themes which has to be applied during import.
        /// </summary>
        private void CalculateThemeDifference(Theme srcTheme, Theme dstTheme)
        {
            // The same document, nothing to do.
            if (ReferenceEquals(srcTheme, dstTheme))
                return;

            foreach (ThemeFontCore themeFont in RunPr.ThemeFontAttrs)
            {
                ComplexFontName officeFontA = ComplexFontName.FromTheme(themeFont);
                ComplexFontName officeFontB = ComplexFontName.FromTheme(themeFont);

                // Fonts are different, save resolved value.
                if (officeFontA.Resolve(srcTheme) != officeFontB.Resolve(dstTheme))
                    mThemeDifference[themeFont] = officeFontA.Resolve(srcTheme);
            }
        }

        /// <summary>
        /// The document from which the nodes are being imported.
        /// </summary>
        internal DocumentBase SrcDoc
        {
            get { return mSrcDoc; }
        }

        /// <summary>
        /// The document into which the nodes are being imported.
        /// </summary>
        internal DocumentBase DstDoc
        {
            get { return mDstDoc; }
        }

        internal StyleCollection SrcStyles
        {
            get { return mSrcDoc.Styles; }
        }

        internal StyleCollection DstStyles
        {
            get { return mDstDoc.Styles; }
        }

        internal ListCollection SrcLists
        {
            get { return mSrcDoc.Lists; }
        }

        internal ListCollection DstLists
        {
            get { return mDstDoc.Lists; }
        }

        /// <summary>
        /// Specifies how the current import operation behaves.
        /// </summary>
        internal ImportFormatMode ImportFormatMode
        {
            get { return mImportFormatMode; }
            set { mImportFormatMode = value; }
        }

        /// <summary>
        /// Cache that maps style istds of the source document into istds in the destination document.
        /// </summary>
        internal IntToIntBidirectionalMap ImportedIstds
        {
            get { return mImportedIstds; }
        }

        /// <summary>
        /// Cache that maps list id valid in the source document into valid list id in the destination document.
        /// </summary>
        internal IntToIntDictionary ImportedListIds
        {
            get { return mImportedListIds; }
        }

        /// <summary>
        /// Cache that maps list definition id valid in the source document into valid list definition id in the destination document.
        /// </summary>
        internal IntToIntDictionary ImportedListDefIds
        {
            get { return mImportedListDefIds; }
        }

        /// <summary>
        /// Key is a picture bullet id in the source document, value is a picture bullet id in the destination document.
        /// </summary>
        internal IntToIntDictionary ImportedPictureBulletIds
        {
            get { return mImportedPictureBulletIds; }
        }

        /// <summary>
        /// Key is a comment id in the source document. Value is a comment id in the destination document.
        /// </summary>
        internal IntToIntDictionary ImportedCommentIds
        {
            get { return mImportedCommentIds; }
        }

        /// <summary>
        /// Key is SDT Id in the source document. Value is SDT Id in the destination document.
        /// </summary>
        internal IntToIntDictionary ImportedSdtIds
        {
            get { return mImportedSdtIds; }
        }

        /// <summary>
        /// Indicates that fonts defined by theme are different in source and destination document.
        /// </summary>
        internal bool IsThemeFontsDifferent
        {
            get { return mThemeDifference.Count > 0; }
        }

        /// <summary>
        /// Indicates that colors defined by theme are different in source and destination document.
        /// </summary>
        internal bool IsThemeColorsDifferent
        {
            get { return mIsThemeColorsDifferent; }
        }

        /// <summary>
        /// The topmost importing node.
        /// </summary>
        internal Node TopmostNode
        {
            get { return mTopmostNode; }
            set { mTopmostNode = value; }
        }

        /// <summary>
        /// Specifies additional import options.
        /// </summary>
        internal ImportFormatOptions ImportFormatOptions
        {
            get { return mImportFormatOptions; }
        }

        private readonly DocumentBase mSrcDoc;
        private readonly DocumentBase mDstDoc;

        private ImportFormatMode mImportFormatMode;
        private readonly ImportFormatOptions mImportFormatOptions;

        private readonly IntToIntBidirectionalMap mImportedIstds;
        private readonly IntToIntDictionary mImportedListIds = new IntToIntDictionary();
        private readonly IntToIntDictionary mImportedListDefIds = new IntToIntDictionary();
        private readonly IntToIntDictionary mImportedPictureBulletIds = new IntToIntDictionary();
        private readonly IntToIntDictionary mImportedCommentIds = new IntToIntDictionary();
        private readonly IntToIntDictionary mImportedSdtIds = new IntToIntDictionary();

        private readonly bool mIsThemeColorsDifferent;
        
        private long mNonReusableListDefId = long.MaxValue;

        private Node mTopmostNode;

        private readonly SortedList<ThemeFontCore, string> mThemeDifference = new SortedList<ThemeFontCore, string>();
    }
}
