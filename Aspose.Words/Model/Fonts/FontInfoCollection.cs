// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2009 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Settings;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Represents a collection of fonts used in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fonts/">Working with Fonts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Items are <see cref="FontInfo"/> objects.</para>
    /// 
    /// <para>You do not create instances of this class directly. 
    /// Use the <see cref="DocumentBase.FontInfos"/> property to access the collection of fonts 
    /// defined in the document.</para>
    /// 
    /// <seealso cref="FontInfo"/>
    /// <seealso cref="DocumentBase.FontInfos"/>
    /// </remarks>
    public class FontInfoCollection : IEnumerable<FontInfo>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <overloads>Provides access to the collection items.</overloads>
        /// <summary>
        /// Gets a font with the specified name.
        /// </summary>
        /// <param name="name">Case-insensitive name of the font to locate.</param>
        public FontInfo this[string name]
        {
            get
            {
                int fontIndex = mFontNameToItemIndex[name];
                return StringToIntDictionary.IsNullSubstitute(fontIndex) ? null : this[fontIndex];
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Gets a font with the specified name.
        /// </summary>
        public FontInfo GetByName(string name)
        {
            return this[name];
        }
#endif

        /// <summary>
        /// Gets a font at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the font.</param>
        public FontInfo this[int index]
        {
            get { return mItems[index]; }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<FontInfo> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether the collection contains a font with the given name.
        /// </summary>
        /// <param name="name">Case-insensitive name of the font to locate.</param>
        /// <returns><c>true</c> if the item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return mFontNameToItemIndex.ContainsKey(name);
        }

        /// <summary>
        /// Specifies whether or not to embed TrueType fonts in a document when it is saved.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>Embedding TrueType fonts allows others to view the document with the same fonts that were used to create it,
        /// but may substantially increase the document size.</para>
        /// <para>This option works for DOC, DOCX and RTF formats only.</para>
        /// </remarks>
        public bool EmbedTrueTypeFonts
        {
            get { return mEmbedTrueTypeFonts; }
            set { mEmbedTrueTypeFonts = value; }
        }

        /// <summary>
        /// <para>Specifies whether or not to embed System fonts into the document.
        /// Default value for this property is <c>false</c>.</para>
        /// <para>This option works only when <see cref="EmbedTrueTypeFonts"/> option is set to <c>true</c>.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting this property to <c>true</c> is useful if the user is on an East Asian system
        /// and wants to create a document that is readable by others who do not have fonts for that
        /// language on their system. For example, a user on a Japanese system could choose to embed the
        /// fonts in a document so that the Japanese document would be readable on all systems.
        /// </para>
        /// <para>This option works for DOC, DOCX and RTF formats only.</para>
        /// </remarks>
        public bool EmbedSystemFonts
        {
            get { return mEmbedSystemFonts; }
            set { mEmbedSystemFonts = value; }
        }

        /// <summary>
        /// <para>Specifies whether or not to save a subset of the embedded TrueType fonts with the document.
        /// Default value for this property is <c>false</c>.</para>
        /// <para>This option works only when <see cref="EmbedTrueTypeFonts"/> property is set to <c>true</c>.</para>
        /// </summary>
        /// <remarks>
        /// This option works for DOC, DOCX and RTF formats only.
        /// </remarks>
        public bool SaveSubsetFonts
        {
            get { return mSaveSubsetFonts; }
            set { mSaveSubsetFonts = value; }
        }

        /// <summary>
        /// Don't need public ctor.
        /// </summary>
        internal FontInfoCollection()
        {
            Clear();
        }

        /// <summary>
        /// <para>Returns a font code (index of the font) for the given font name (case insensitive).</para>
        /// <para>Searches through fonts by their main name, creates a font info if does not exist.</para>
        /// </summary>
        internal int NameToCode(string name)
        {
            int fontCode = mFontNameToItemIndex[name];

            if (StringToIntDictionary.IsNullSubstitute(fontCode))
                return Merge(new FontInfo(name));

            return fontCode;
        }

        /// <summary>
        /// Returns font name for the specified code.
        /// Handles invalid requests safely by limiting the font code to valid range. 
        /// </summary>
        internal string CodeToName(int fontCode)
        {
            // RK If we have an empty font table, we should create a default font.
            if (mItems.Count == 0)
                Merge(new FontInfo("Times New Roman"));

            // RK Make the font code valid index.
            if ((fontCode < 0) || (fontCode >= mExternalCodeToFontName.Count))
                fontCode = 0;

            return mExternalCodeToFontName[fontCode];
        }

        /// <summary>
        /// <para>Adds clone of <see cref="FontInfo"/> to the collection if there is no <see cref="FontInfo"/>
        /// with the same name or merges information from new <see cref="FontInfo"/> to existing one.</para>
        /// </summary>
        /// <param name="fontInfo"><see cref="FontInfo"/> added to the collection. It's cloned or merged to one
        /// of existing <see cref="FontInfo"/>s.</param>
        /// <returns>Index of new or existing <see cref="FontInfo"/> in the collection.</returns>
        internal int Merge(FontInfo fontInfo)
        {
            int fontIndex = 0;

            if (Contains(fontInfo.Name))
            {
                fontIndex = mFontNameToItemIndex[fontInfo.Name];
                mItems[fontIndex].Merge(fontInfo);
            }
            else if (StringUtil.HasChars(fontInfo.Name))
            {
                mItems.Add(fontInfo.Clone());
                fontIndex = mItems.Count - 1;
                mFontNameToItemIndex[fontInfo.Name] = fontIndex;
            }

            // AS Only font Name is needed during reading of DOC.
            // It's better to remove CodeToName method and mExternalCodeToFontName to DocFontCodeResolver
            // or combine it with RtfFontCodeResolver.
            // mExternalCodeToFontName can contain several records with the same font name.
            mExternalCodeToFontName.Add(fontInfo.Name);

            foreach (string fontAltName in fontInfo.GetAltNameList())
            {
                if (!mFontAltNameToItemIndex.ContainsKey(fontAltName))
                    mFontAltNameToItemIndex[fontAltName] = fontIndex;
            }

            return fontIndex;
        }

        /// <summary>
        /// Calls <see cref="Merge(FontInfo)"/> for all <see cref="FontInfo"/>s of source collection.
        /// </summary>
        /// <param name="srcFontInfoCollection">Can not be <c>null</c>.</param>
        internal void Merge(FontInfoCollection srcFontInfoCollection)
        {
            foreach (FontInfo srcFontInfo in srcFontInfoCollection)
                Merge(srcFontInfo);
        }

        /// <summary>
        /// <para>Update collection of fonts stored in the document according to their usage in used font names.
        /// The following operations are performed:</para>
        /// <list type="">
        /// <item>Filters out all <see cref="FontInfo"/>s which are not in <paramref name="usedFontNames"/> or have duplicated names.</item>
        /// <item>Adds new <see cref="FontInfo"/>s from <paramref name="usedFontNames"/> if they are absent in the collection.</item>
        /// </list>
        /// </summary>
        internal void UpdateToUsedFonts(ISetGeneric<string> usedFontNames)
        {
            List<string> newFontNames = new List<string>();
            SortedList<int, int> validFontIndexes = new SortedList<int, int>();

            FindNewAndUsedExistingFonts(usedFontNames, newFontNames, validFontIndexes);

            RemoveUnusedFonts(validFontIndexes);
            AddNewFonts(newFontNames);
        }

        /// <summary>
        /// Makes a deep copy of the collection.
        /// </summary>
        internal FontInfoCollection Clone()
        {
            FontInfoCollection lhs = new FontInfoCollection();

            lhs.UpdateEmbedFontOptions(this);
            
            lhs.Merge(this);

            return lhs;
        }

        /// <summary>
        /// Clones without fonts embedding.
        /// </summary>
        internal FontInfoCollection CloneWithoutEmbeddedFonts()
        {
            FontInfoCollection lhs = Clone();
            lhs.RemoveEmbeddedFonts();
            
            return lhs;
        }

        /// <summary>
        /// Removes embedded fonts.
        /// </summary>
        internal void RemoveEmbeddedFonts()
        {
            foreach (FontInfo fontInfo in this)
                fontInfo.RemoveEmbeddedFonts();
        }

        /// <summary>
        /// Returns true if this font info collection has embedded fonts.
        /// </summary>
        internal bool HasEmbeddedFonts()
        {
            foreach (FontInfo fontInfo in this)
                if (fontInfo.HasEmbeddedFonts)
                    return true;

            return false;
        }

        /// <summary>
        /// Removes from <see cref="mItems"/> all fonts except having theirs indexes
        /// in <paramref name="validFontIndexes"/> collection.
        /// </summary>
        internal void RemoveUnusedFonts(SortedList<int, int> validFontIndexes)
        {
            List<FontInfo> itemsOriginal = mItems;
            Clear();
            Debug.Assert(itemsOriginal != mItems);

            foreach (int fontIndex in validFontIndexes.Keys)
                Merge(itemsOriginal[fontIndex]);
        }
        
        /// <summary>
        /// Sets all font embedding options equal to embedding options in specified <paramref name="docPr"/>.
        /// </summary>
        internal void UpdateEmbedFontOptions(DocPr docPr)
        {
            mEmbedTrueTypeFonts = docPr.EmbedTrueTypeFonts;
            mEmbedSystemFonts = docPr.EmbedSystemFonts;
            mSaveSubsetFonts = docPr.SaveSubsetFonts;
        }

        /// <summary>
        /// Sets all font embedding options equal to embedding options in specified <paramref name="fontInfos"/>.
        /// </summary>
        private void UpdateEmbedFontOptions(FontInfoCollection fontInfos)
        {
            mEmbedTrueTypeFonts = fontInfos.EmbedTrueTypeFonts;
            mEmbedSystemFonts = fontInfos.EmbedSystemFonts;
            mSaveSubsetFonts = fontInfos.SaveSubsetFonts;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        private void Clear()
        {
            mItems = new List<FontInfo>();
            mExternalCodeToFontName = new List<string>();
            mFontNameToItemIndex = new StringToIntDictionary(false);
            mFontAltNameToItemIndex = new StringToIntDictionary(false);
        }

        /// <summary>
        /// <para>Searches <paramref name="usedFontNames"/> (fonts used in the document) among fonts in the collection and fills two collections:
        /// <paramref name="newFontNames"/> and <paramref name="validFontIndexes"/>.</para>
        /// </summary>
        /// <param name="usedFontNames">Input collection of fonts used in the document.</param>
        /// <param name="newFontNames">Output collection of font names that will be added but not existing
        /// in the collection yet. Must be empty at input.</param>
        /// <param name="validFontIndexes">Output collection of indexes in <see cref="mItems"/> of fonts used in the document
        /// as <paramref name="usedFontNames"/> states. Must be empty at input.</param>
        private void FindNewAndUsedExistingFonts(ISetGeneric<string> usedFontNames, 
            List<string> newFontNames,
            SortedList<int, int> validFontIndexes)
        {
            // RK Have to sort font names first because otherwise they will affect golds on Java.
            List<string> sortedFontNames = new List<string>();
            foreach (string fontName in usedFontNames)
                sortedFontNames.Add(fontName);
            sortedFontNames.Sort();

            foreach (string fontName in sortedFontNames)
            {
                int fontIndex = mFontNameToItemIndex[fontName];
                if (StringToIntDictionary.IsNullSubstitute(fontIndex))
                    fontIndex = mFontAltNameToItemIndex[fontName];

                if (StringToIntDictionary.IsNullSubstitute(fontIndex))
                {
                    newFontNames.Add(fontName);
                }
                else if (!validFontIndexes.ContainsKey(fontIndex))
                {
                    validFontIndexes.Add(fontIndex, 0);
                }
            }
        }

        /// <summary>
        /// Adds to the collection new <see cref="FontInfo"/>s by their names collected in <paramref name="newFontNames"/>.
        /// </summary>
        /// <remarks>
        /// It's more correct to collect new fonts information from installed fonts
        /// but currently the function just add stubs for new fonts.
        /// </remarks>
        private void AddNewFonts(List<string> newFontNames)
        {
            foreach (string fontName in newFontNames)
                Merge(new FontInfo(fontName));
        }

        /// <summary>
        /// Contains FontInfo objects in the order they appear in the original document.
        /// </summary>
        private List<FontInfo> mItems;
        /// <summary>
        /// Key is case insensitive font name. Value is integer font code (index) in <see cref="mItems"/>.
        /// </summary>
        private StringToIntDictionary mFontNameToItemIndex;
        /// <summary>
        /// Keys is external index used in <see cref="CodeToName"/>. Value is index in <see cref="mItems"/>
        /// </summary>
        private List<string> mExternalCodeToFontName;
        /// <summary>
        /// <para>Key is case insensitive font name among all alternative names of font of the collection.</para>
        /// <para>Value is integer font code (index) in the font info collection.</para>
        /// </summary>
        private StringToIntDictionary mFontAltNameToItemIndex;

        private bool mEmbedTrueTypeFonts;
        private bool mEmbedSystemFonts;
        private bool mSaveSubsetFonts;
    }
}
