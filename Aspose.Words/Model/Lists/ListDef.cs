// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2005 by Roman Korchagin

using System;
using Aspose.Collections.Generic;
using Aspose.Words.Revisions;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// A definition of a list. These definitions will not be directly used in the document.
    /// Instead, they will be referenced by list elements.
    /// </summary>
    internal class ListDef : IComparable<ListDef>
    {
        /// <summary>
        /// Ctor. Used when loading a document from RTF.
        /// Note: Found only in RtfListTableHandler.cs. Maybe it's good to refactor to using only one ctor.
        /// </summary>
        /// <param name="document">Need to know the parent document because list definition is valid in a context of a document.</param>
        internal ListDef(DocumentBase document)
        {
            mDocument = document;
            mLevels = new ListLevelCollection();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="document">Need to know the parent document because list definition is valid in a context of a document.</param>
        /// <param name="listDefId">Once assigned, list definition id should never change, so assign at ctor.</param>
        /// <param name="listType">Once assigned, list type should never change.</param>
        /// <param name="templateCode">We do not change template code after construction too.</param>
        internal ListDef(DocumentBase document, int listDefId, ListType listType, int templateCode)
        {
            mDocument = document;
            mListDefId = listDefId;
            mListType = listType;
            mTemplateCode = templateCode;
            mLevels = new ListLevelCollection(document, listType);
        }

        [JavaAttributes.JavaDelete]
        public int CompareTo(object obj)
        {
            return mListDefId.CompareTo(((ListDef)obj).ListDefId);
        }

        public int CompareTo(ListDef other)
        {
            return mListDefId.CompareTo(other.ListDefId);
        }

        /// <summary>
        /// Makes a deep copy of the list definition.
        /// Note that the list style and paragraph styles referenced by this object
        /// are not copied.
        /// </summary>
        /// <param name="document">Destination document, required to make a valid clone.</param>
        /// <param name="listDefId">New list definition id, required to make a valid clone.</param>
        internal virtual ListDef Clone(DocumentBase document, int listDefId)
        {
            ListDef lhs = (ListDef)MemberwiseClone();
            lhs.mDocument = document;
            lhs.mListDefId = listDefId;
            lhs.mLevels = mLevels.Clone(document);
            return lhs;
        }

        /// <summary>
        /// Creates collection of blank levels of the desired list type.
        /// </summary>
        internal void PurgeLevels()
        {
            mLevels = new ListLevelCollection(mDocument, mListType);
        }

        /// <summary>
        /// Compares with the specified ListDef.
        /// </summary>
        public bool Equals(ListDef listDef)
        {
            return EqualsCore(listDef, new HashSetGeneric<Pair>());
        }

        /// <summary>
        /// Compares with the specified ListDef.
        /// <param name="listDef">ListDef that will be compared with this ListDef.</param>
        /// <param name="alreadyComparedLinkedStyles">HashSet for collecting already compared styles to avoid dead loops.</param>
        /// </summary>
        internal bool EqualsCore(ListDef listDef, HashSetGeneric<Pair> alreadyComparedLinkedStyles)
        {
            if (listDef == null)
                return false;

            if (ListType != listDef.ListType)
                return false;

            if (Levels.Count != listDef.Levels.Count)
                return false;

            if (IsListStyleDefinition != listDef.IsListStyleDefinition)
                return false;

            if (IsListStyleReference != listDef.IsListStyleReference)
                return false;

            Style style = Style;
            if ((style != null) && (!style.Equals(listDef.Style, alreadyComparedLinkedStyles)))
                return false;

            for (int i = 0; i < Levels.Count; i++)
                if (!Levels[i].EqualsCore(listDef.Levels[i], alreadyComparedLinkedStyles))
                    return false;

            return true;
        }

        /// <summary>
        /// Calculates hash code for this object.
        /// </summary>
        /// <dev>
        /// To be compatible with the <see cref="Equals(ListDef)"/> method, only properties that affect visual
        /// representation of the list should be included into the calculation. List definition ID, name and similar
        /// properties are ignored.
        /// </dev>
        public override int GetHashCode()
        {
            int hashCode = ListType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsListStyleDefinition.GetHashCode();
            hashCode = (hashCode * 397) ^ IsListStyleReference.GetHashCode();

            foreach (ListLevel level in Levels)
                hashCode = (hashCode * 397) ^ level.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// Used when loading a document from RTF and DOCX (if nsid is omitted in list definition).
        /// Don't use this method if list is already added to list collection.
        /// </summary>
        /// <param name="listDefId"></param>
        internal void SetListDefId(int listDefId)
        {
            mListDefId = listDefId;
        }

        /// <summary>
        /// Used when loading a document from RTF, ODT.
        /// </summary>
        /// <param name="listType"></param>
        internal void SetListType(ListType listType)
        {
            mListType = listType;
        }

        /// <summary>
        /// Used when loading a document from RTF.
        /// </summary>
        /// <param name="templateCode"></param>
        internal void SetTemplateCode(int templateCode)
        {
            mTemplateCode = templateCode;
        }

        /// <summary>
        /// Finds the terminal list definition in the chain of referencing.
        /// When ListDef is a reference the list levels should be accessed through
        /// the referenced ListDef. WORDSNET-13530.
        /// Maybe we can also refactor <see cref="IsListStyleDefinition" /> and <see cref="IsListStyleReference" />
        /// using this method but in the current implementation they both require Style != null.
        /// </summary>
        internal ListDef GetTerminalListDef()
        {
            // WORDSNET-17658 Restored recursion.
            if (IsListStyleReference)
                return Style.List.ListDef.GetTerminalListDef();

            // WORDSNET-20030 Return NoList when ListLevels is empty.
            return (Levels.Count > 0)
                ? this
                : mDocument.Styles.GetByIstd(StyleIndex.NoList, false).List.ListDef;
        }

        /// <summary>
        /// Returns <see cref="ParagraphNumberRevision"/> for a specified list level.
        /// </summary>
        internal ParagraphNumberRevision CreateNumberRevision(int listLevel, EditSession session)
        {
            Debug.Assert((listLevel >= 0) && (listLevel < ListLevel.MaxLevels));

            if (listLevel >= Levels.Count)
                return null;

            ParagraphNumberRevision revision = new ParagraphNumberRevision();
            revision.Author = session.Author;
            revision.DateTime = session.DateTime;

            revision.WasNumbered = true;
            revision.NumberFormat = mLevels[listLevel].NumberFormat;

            for (int i = 0; i <= listLevel; i++)
            {
                ListLevel level = mLevels[i];
                revision.NumberStyles[i] = level.NumberStyle;
                revision.NumberValues[i] = level.StartAt;
            }

            return revision;
        }

        /// <summary>
        /// Unique List definition ID.
        ///
        /// In a DOC file, this is actually lsid. In WordML this is listDef.lsid
        /// ListDefId in WordML is something different.
        /// </summary>
        internal int ListDefId
        {
            get { return mListDefId; }
        }

        /// <summary>
        /// Specifies the list type.
        /// </summary>
        internal ListType ListType
        {
            get { return mListType; }
        }

        /// <summary>
        /// This seems to be random in WordML.
        /// </summary>
        internal int TemplateCode
        {
            get { return mTemplateCode; }
        }

        /// <summary>
        /// Represents the list's name. I think it is related to the name that can be used in LISTNUM fields.
        /// </summary>
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Indicates that list should restart at each section.
        /// </summary>
        /// <remarks>
        /// AM. This option is supported only RTF, DOC and DOCX document formats.
        /// </remarks>
        internal bool IsRestartAtEachSection
        {
            get { return mIsRestartAtEachSection; }
            set { mIsRestartAtEachSection = value; }
        }

        /// <summary>
        /// Defines levels in this list. This provides direct access to "raw" data defined for list definition.
        /// You normally want to use this when loading/saving to MS Word formats, but for other purposes,
        /// such as rendering see <see cref="GetTerminalListDef"/>.
        /// </summary>
        internal ListLevelCollection Levels
        {
            get { return mLevels; }
        }

        /// <summary>
        /// Returns true if this list definition is a definition of a list style.
        /// </summary>
        internal bool IsListStyleDefinition
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                // This object is a list style definition when the list style references this list definition.
                Style style = this.Style;
                return (style != null) && (style.List.ListDef == this);
            }
        }

        /// <summary>
        /// Returns true if this list definition is a reference to a list style.
        /// </summary>
        internal bool IsListStyleReference
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                // This object is a list style reference when the list style references another list definition.
                Style style = this.Style;
                return (style != null) && (style.List.ListDef != this);
            }
        }

        /// <summary>
        /// The istd of the list style that this list defines or references.
        /// Corresponds to the w:styleLink and w:listStyleLink in WordML.
        /// </summary>
        internal int ListStyleIstd
        {
            get { return mListStyleIstd; }
            set
            {
                mListStyleIstd = value;

                // WORDSNET-1098 Version 3.7 creates documents that crash Word2002.
                // We should never write StyleIndex.Nil as this crashes MS Word 2002.
                if (mListStyleIstd == StyleIndex.Nil)
                    mListStyleIstd = StyleIndex.NoList;
            }
        }

        /// <summary>
        /// Gets the list style that this list defines or references.
        /// If the list does not define or reference a list style this property returns null.
        /// </summary>
        internal Style Style
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if ((mListStyleIstd == StyleIndex.Nil) || (mListStyleIstd == StyleIndex.NoList))
                    return null;

                Style style = mDocument.Styles.GetByIstd(mListStyleIstd, true);
                if ((style != null) && (style.Type == StyleType.List))
                    return style;
                else
                    return null;
            }
        }

        /// <summary>
        /// Resolves list levels for an empty ListStyleReference listDef, otherwise gets own list levels.
        /// </summary>
        internal ListLevelCollection GetListLevels()
        {
            return IsListStyleReference && (Levels.Count == 0)
                ? GetTerminalListDef().Levels
                : Levels;
        }

        /// <summary>
        /// Returns the owner document.
        /// </summary>
        internal DocumentBase Document
        {
            get { return mDocument; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDocument;
        private int mListDefId;
        /// <summary>
        /// I think this is a correct default according to the RTF spec.
        /// </summary>
        private ListType mListType = ListType.MultiLevel;
        private int mTemplateCode;
        private string mName;
        private ListLevelCollection mLevels;
        private int mListStyleIstd = StyleIndex.NoList;
        /// <summary>
        /// Default value is <c>false</c>.
        /// </summary>
        private bool mIsRestartAtEachSection;

    }
}
