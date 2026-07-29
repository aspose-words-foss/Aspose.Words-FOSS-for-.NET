// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2005 by Roman Korchagin

using System;
using Aspose.Collections.Generic;
using Aspose.Words.Saving;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Represents formatting of a list.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-lists/">Working with Lists</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A list in a Microsoft Word document is a set of list formatting properties.
    /// Each list can have up to 9 levels and formatting properties, such as number style, start value,
    /// indent, tab position etc are defined separately for each level.</p>
    ///
    /// <p>A <see cref="List"/> object always belongs to the <see cref="ListCollection"/> collection.</p>
    ///
    /// <p>To create a new list, use the Add methods of the <see cref="ListCollection"/> collection.</p>
    ///
    /// <p>To modify formatting of a list, use <see cref="ListLevel"/> objects found in
    /// the <see cref="ListLevels"/> collection.</p>
    ///
    /// <p>To apply or remove list formatting from a paragraph, use <see cref="ListFormat"/>.</p>
    ///
    /// <seealso cref="ListCollection"/>
    /// <seealso cref="ListLevel"/>
    /// <seealso cref="ListFormat"/>
    /// </remarks>
    // TODO: When overriding Equals, you must also override GetHashCode to guarantee correct behavior with hashtables.
    public class List : IComparable<List>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="document">Need to know the parent document because a list is only valid in a context of a document.</param>
        /// <param name="listId">List id once assigned is never changed later, so assign at ctor.</param>
        internal List(DocumentBase document, int listId)
        {
            mDocument = document;
            mListId = listId;
        }

        /// <summary>
        /// Ctor to create a list used to maintain LISTNUM or AUTONUM field functionality.
        /// </summary>
        internal List(ListDef listDef, int listId)
        {
            mListDefCache = listDef;
            mDocument = listDef.Document;
            mListDefId = listDef.ListDefId;
            mListId = listId;
        }

        /// <summary>
        /// Creates a clone of the list with a new listId.
        /// The new list still refers to the original list definition.
        /// </summary>
        internal List Clone(DocumentBase document, int listId)
        {
            List lhs = (List)MemberwiseClone();

            lhs.mDocument = document;
            lhs.mListId = listId;
            lhs.mListDefCache = null;

            lhs.mOverrides = new ListLevelOverrideCollection();
            foreach (ListLevelOverride item in mOverrides)
                lhs.mOverrides.Add(item.Clone(document));

            return lhs;
        }

        /// <summary>
        /// Compares with the specified list.
        /// </summary>
        public bool Equals(List list)
        {
            return EqualsCore(list, new HashSetGeneric<Pair>());
        }

        /// <summary>
        /// Determines whether the specified object is equal in value to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (typeof(List) != obj.GetType())
                return false;

            return Equals((List)obj);
        }

        /// <summary>
        /// Calculates hash code for this list object.
        /// </summary>
        /// <dev>
        /// To be compatible with the <see cref="Equals(object)"/> method, only properties that affect visual
        /// representation of the list should be included into the calculation. List ID and similar properties
        /// are ignored.
        /// </dev>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            int hashCode = ListDef.GetHashCode();
            hashCode = (hashCode * 397) ^ Overrides.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Core method for comparing with the specified list.
        /// </summary>
        /// <param name="list">List that will be compared with this list.</param>
        /// <param name="alreadyComparedLinkedStyles">HashSet for collecting already compared styles to avoid dead loops.</param>
        internal bool EqualsCore(List list, HashSetGeneric<Pair> alreadyComparedLinkedStyles)
        {
            if (list == null)
                return false;

            if (!ListDef.EqualsCore(list.ListDef, alreadyComparedLinkedStyles))
                return false;

            if (!Overrides.EqualsCore(list.Overrides, alreadyComparedLinkedStyles))
                return false;

            return true;
        }

        /// <summary>
        /// Don't use this method if list is already added to list collection.
        /// </summary>
        internal void SetListId(int listId)
        {
            mListId = listId;
        }

        internal ListLevelOverrideCollection Overrides
        {
            get { return mOverrides; }
        }

        /// <summary>
        /// Gets the unique identifier of the list.
        /// </summary>
        /// <remarks>
        /// <p>You do not normally need to use this property. But if you use it, you normally do so
        /// in conjunction with the <see cref="ListCollection.GetListByListId"/> method to find a
        /// list by its identifier.</p>
        /// </remarks>
        public int ListId
        {
            get { return mListId; }
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <remarks>
        /// <p>A list always has a parent document and is valid only in the context of that document.</p>
        /// </remarks>
        public DocumentBase Document
        {
            get { return mDocument; }
        }

        /// <summary>
        /// Returns <c>true</c> when the list contains 9 levels; <c>false</c> when 1 level.
        /// </summary>
        /// <remarks>
        /// <p>The lists that you create with Aspose.Words are always multi-level lists and contain 9 levels.</p>
        ///
        /// <p>Microsoft Word 2003 and later always create multi-level lists with 9 levels.
        /// But in some documents, created with earlier versions of Microsoft Word you might encounter
        /// lists that have 1 level only.</p>
        /// </remarks>
        public bool IsMultiLevel
        {
            get { return (ListDef.ListType != ListType.SingleLevel); }
        }

        /// <summary>
        /// Gets the collection of list levels for this list.
        /// </summary>
        /// <remarks>
        /// <p>Use this property to access and modify formatting individual to each level of the list.</p>
        /// </remarks>
        /// <dev>
        /// RK This might be a bit nasty because it provides access to the terminal list level definitions,
        /// e.g. it could return the list level definitions from the list style that is referenced by this list.
        /// So if the user modifies the formatting of a list level he could actually be modifying the list style.
        /// But let's leave this for now because this property is mostly accessed to actually get the terminal
        /// formatting of list levels.
        /// </dev>
        public ListLevelCollection ListLevels
        {
            get { return ListDef.GetTerminalListDef().Levels; }
        }

        /// <summary>
        /// This never returns null. If a list points to a non-existant list definition,
        /// a new default list definition is created automatically on first access.
        /// </summary>
        internal ListDef ListDef
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mListDefCache == null)
                    mListDefCache = mDocument.Lists.FetchListDefByListDefId(mListDefId);

                return mListDefCache;
            }
        }

        /// <summary>
        /// Clears cache of ListDef.
        /// </summary>
        internal void ClearListDefCache()
        {
            mListDefCache = null;
        }

        /// <summary>
        /// Specifies whether list should be restarted at each section.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <p>This option is supported only in RTF, DOC and DOCX document formats.</p>
        /// <p>This option will be written to DOCX only if <see cref="OoxmlCompliance"/> is higher then <see cref="OoxmlCompliance.Ecma376_2006"/>.</p>
        /// </remarks>
        public bool IsRestartAtEachSection
        {
            get { return ListDef.IsRestartAtEachSection; }
            set
            {
                ListDef.IsRestartAtEachSection = value;
                if (value)
                    OoxmlComplianceInfo.MarkAsHasDocxExtensionsOf(Document.FetchDocument(), MsWordVersionCore.Word2013);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this list is a definition of a list style.
        /// </summary>
        /// <remarks>
        /// <p>When this property is <c>true</c>, the <see cref="Style"/> property returns the list style that
        /// this list defines.</p>
        ///
        /// <p>By modifying properties of a list that defines a list style, you modify the properties
        /// of the list style.</p>
        ///
        /// <p>A list that is a definition of a list style cannot be applied directly to paragraphs
        /// to make them numbered.</p>
        ///
        /// <seealso cref="List.Style"/>
        /// <seealso cref="IsListStyleReference"/>
        /// </remarks>
        public bool IsListStyleDefinition
        {
            get { return ListDef.IsListStyleDefinition; }
        }

        /// <summary>
        /// Returns <c>true</c> if this list is a reference to a list style.
        /// </summary>
        /// <remarks>
        /// <p>Note, modifying properties of a list that is a reference to list style has no effect.
        /// The list formatting specified in the list style itself always takes precedence.</p>
        ///
        /// <seealso cref="Style"/>
        /// <seealso cref="IsListStyleDefinition"/>
        /// </remarks>
        public bool IsListStyleReference
        {
            get { return ListDef.IsListStyleReference; }
        }

        /// <summary>
        /// Gets the list style that this list references or defines.
        /// </summary>
        /// <remarks>
        /// <p>If this list is not associated with a list style, the property will return <c>null</c>.</p>
        ///
        /// <p>A list could be a reference to a list style, in this case <see cref="IsListStyleReference"/>
        /// will be <c>true</c>.</p>
        ///
        /// <p>A list could be a definition of a list style, in this case <see cref="IsListStyleDefinition"/>
        /// will be <c>true</c>. Such a list cannot be applied to paragraphs in the document directly.</p>
        /// </remarks>
        public Style Style
        {
            get
            {
                // In the model, ListDef does not refer to the Nil style, it can refer to the NoList style.
                // But for the public interface it is better if we return null in this case.
                if (ListDef.ListStyleIstd != StyleIndex.NoList)
                    return ListDef.Style;
                else
                    return null;
            }
        }

        /// <summary>
        /// The identifier of the list definition this list refers to.
        /// Be careful to set this only to list definition identifiers that are valid within this document.
        /// </summary>
        internal int ListDefId
        {
            get { return mListDefId; }
            set { mListDefId = value; }
        }

        /// <summary>
        /// Gets or sets a durable identifier of the list.
        /// </summary>
        /// <dev>
        /// [MS-DOCX] 2.8.2.1 durableId attribute.
        /// </dev>
        internal int DurableId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns override object if present for the specified list level.
        /// If there is no override for the specified list level, returns null.
        /// </summary>
        internal ListLevelOverride GetOverride(int level)
        {
            foreach (ListLevelOverride levelOverride in Overrides)
            {
                if (levelOverride.ListLevel.LevelNumber == level)
                    return levelOverride;
            }
            return null;
        }

        /// <summary>
        /// Returns the "start at" override object if present for the specified list level.
        /// If there is no override for the specified list level, returns null.
        /// </summary>
        private ListLevelOverride GetStartAtOverride(int level)
        {
            foreach (ListLevelOverride levelOverride in Overrides)
            {
                if ((levelOverride.ListLevel.LevelNumber == level) && levelOverride.IsStartAt)
                    return levelOverride;
            }
            return null;
        }

        /// <summary>
        /// Returns the "formatting" override object if present for the specified list level.
        /// If there is no override for the specified list level, returns null.
        /// </summary>
        internal ListLevelOverride GetFormattingOverride(int level)
        {
            // WORDSNET-17717 When ListDef.ListType is singleLevel, get 0-th levelOverride.ListLevel, regardless the requested level.
            int overrideLvlNumber = IsMultiLevel ? level : 0;

            foreach (ListLevelOverride levelOverride in Overrides)
            {
                if ((levelOverride.ListLevel.LevelNumber == overrideLvlNumber) && levelOverride.IsFormatting)
                    return levelOverride;
            }
            return null;
        }

        internal bool IsStartAtOverridden(int level)
        {
            return (GetStartAtOverride(level) != null);
        }

        /// <summary>
        /// Determines the start number according to list level overrides and list definition.
        /// </summary>
        internal int GetStartAtOverrideAware(int level)
        {
            ListLevelOverride over = GetStartAtOverride(level);
            if (over != null)
                return over.StartAtReal;

            // WORDSNET-15226, 17658. We must rely on TerminalListDef Levels in a common situation.
            // But in the case, when StyleReference ListDef has its own ListLevels, we must rely on
            // the own ListLevels.
            ListLevelCollection listLevels = ListDef.GetListLevels();

            // WORDSNET-21255 Rely on TerminalListDef levels when we have no list levels.
            if (listLevels.Count == 0)
                listLevels = ListDef.GetTerminalListDef().Levels;

            return listLevels.FetchListLevel(level).StartAt;
        }

        internal ListLevel GetListLevelOverrideAware(int level)
        {
            ListLevelOverride over = GetFormattingOverride(level);
            return (over != null) ? over.ListLevel : ListLevels.FetchListLevel(level);
        }

        /// <summary>
        /// Returns <c>true</c> if expected List are same actual List.
        /// </summary>
        /// <remarks>
        /// AM. We already have new List.Equals(List) implementation.
        /// I tried to switch to new method but failed to do this as it's something wrong
        /// with copying list referenced styles.
        /// </remarks>
        internal static bool AreSameLegacyLists(List expected, List actual)
        {
            bool equalStyle = true;
            if (expected.ListLevels.Count != actual.ListLevels.Count)
                return false;
            for (int i = 0; i < expected.ListLevels.Count; i++)
            {
                if(!ListLevel.AreSameLegacyListLevels(expected.ListLevels[i], actual.ListLevels[i]))
                {
                    equalStyle = false;
                    break;
                }
            }
            return equalStyle;
        }

        /// <summary>
        /// Compares the specified object to the current object.
        /// </summary>
        [JavaAttributes.JavaDelete]
        public int CompareTo(object obj)
        {
            return ListId.CompareTo(((List)obj).ListId);
        }

        /// <summary>
        /// Compares the specified list to the current list.
        /// </summary>
        public int CompareTo(List other)
        {
            return ListId.CompareTo(other.ListId);
        }

        /// <summary>
        /// Returns true if the current list and the given list are created from the same template.
        /// </summary>
        public bool HasSameTemplate(List other)
        {
            return (ListDefId == other.ListDefId);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDocument;
        private int mListId;
        private int mListDefId;
        /// <summary>
        /// Cached pointer to list definition for speed.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private ListDef mListDefCache;
        private ListLevelOverrideCollection mOverrides = new ListLevelOverrideCollection();
    }
}
