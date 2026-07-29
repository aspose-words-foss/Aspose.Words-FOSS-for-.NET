// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

using System;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Collection of attribute values that can have formatting revision.
    /// </summary>
    internal abstract class WordAttrCollection : AttrCollection
    {
        /// <summary>
        /// Specifies modes to get attribute value.
        /// </summary>
        /// <remarks>
        /// AM. This enum is just temporary solution, I was forced to make it by following reasons:
        /// - I cannot use RevisionsView as it is going to be public later.
        ///
        /// - There is some inconsistency with expand flags. For example we can set both AfterChanges and Revised flags but
        ///   this has no meaning at all.
        ///
        /// - More inconsistency is that for example RunPrExpandFlags.DocumentDefaults also has no meaning at this level.
        ///
        /// - Most important is that we have single method to get attribute value but
        ///   have different flags for run and paragraph formatting: RunPrExpandFlags and ParaPrExpandFlags.
        ///   That means that we have to convert both flag types into one type anyway.
        ///
        /// It seems that we need to reconsider expand flags approach and make some rework to avoid inconsistency.
        /// </remarks>
        protected enum ValueVersion
        {
            /// <summary>
            /// Gets original attribute value.
            /// </summary>
            Original,

            /// <summary>
            /// Gets final attribute value.
            /// </summary>
            Final,

            /// <summary>
            /// Gets raw after changes attribute value. Has meaning during DOCX import only.
            /// </summary>
            AfterChanges
        }

        internal object FetchAttr(int key, RevisionsView revisionsView)
        {
            return GetAttr(key, ToValueVersion(revisionsView), true);
        }

        internal object GetDirectAttr(int key, RevisionsView revisionsView)
        {
            return GetAttr(key, ToValueVersion(revisionsView), false);
        }

        protected object GetAttr(int key, ValueVersion valueVersion, bool globalDefaults)
        {
            object value = GetDirectAttrValueVersion(key, valueVersion);

            if (!globalDefaults)
                return value;

            return (value != null) ? value : GetDefaults()[key];
        }

        private static ValueVersion ToValueVersion(RevisionsView view)
        {
            return (view == RevisionsView.Final) ? ValueVersion.Final : ValueVersion.Original;
        }

        private object GetDirectAttrValueVersion(int key, ValueVersion valueVersion)
        {
            // WORDSNET-28929 Empty format revision might be Istd revision.
            if ((valueVersion == ValueVersion.AfterChanges) && FormatRevision != null)
                return FormatRevision.RevPr[key];

            if ((valueVersion == ValueVersion.Final) && HasFormatRevision)
            {
                WordAttrCollection revPr = FormatRevision.RevPr;

                // This might be calculated differently for complex attributes. Let's postpone for a while.
                object value = revPr[key];
                if (value != null)
                    return value;

                // Format revision can contain Istd attribute.
                // In this case we need to "reset" original formatting i.e we should get formatting from revision only so return null immediately.
                if (revPr.Contains(ParaAttr.Istd) || revPr.Contains(FontAttr.Istd))
                    return null;
            }

            return this[key];
        }

        /// <summary>
        /// Sets the specified value in the desired collection depending on revisionsView parameter.
        /// </summary>
        /// <remarks>
        /// The method does not replicate MS Word functionality for reserving formatting attributes in the change tracking mode.
        /// The method does not move attributes from one collection to another. It also does not create the format revision if its value is null.
        /// The method assumes that one of the two points is true:
        /// 1. The current collection contains the attributes before the change, Original state. FormatRevision.RevPr contains the delta which
        ///    is expands through AcceptFormatRevision() brings this collection to Final state.
        /// 2. The current collection contains the actual attributes, and Original and Final states are identical.
        ///    FormatRevision.RevPr is strictly equal to null.
        /// </remarks>
        internal void SetAttr(int key, object value, RevisionsView revisionsView)
        {
            if ((FormatRevision != null) && (revisionsView == RevisionsView.Final))
                FormatRevision.RevPr.SetAttr(key, value);
            else
                SetAttr(key, value);
        }

        /// <summary>
        /// Removes the attribute from the desired attribute collection based on revisionsView parameter.
        /// </summary>
        /// <remarks>
        /// See the remarks to SetAttr(key, value, revisionsView).
        /// </remarks>
        internal void Remove(int key, RevisionsView revisionsView)
        {
            if ((FormatRevision != null) && (revisionsView == RevisionsView.Final))
                FormatRevision.RevPr.Remove(key);
            else
                Remove(key);
        }

        /// <summary>
        /// Rejects formatting revisions on this attribute collection.
        /// </summary>
        internal void RejectFormatRevision()
        {
            Remove(RevisionAttr.FormatRevision);
        }

        /// <summary>
        /// Accepts formatting revisions on this attribute collection.
        /// </summary>
        internal virtual void AcceptFormatRevision()
        {
            // The default implementation does not perform any special handling when the style changes in the revision.
            AcceptFormatRevisionCore(-1, null);
        }

        /// <summary>
        /// Accepts formatting revisions on this attribute collection.
        ///
        /// Accepting a revision means replacing current attributes with the attributes
        /// from the revision collection and deleting the revision collection.
        ///
        /// If the style was applied during a formatting revision it looks like
        /// we should clear all existing attributes except special things (number, insert and delete revisions).
        /// The parameters allow for this to be done for different collection types.
        /// </summary>
        /// <param name="istdKey">They key that is used for the style attribute. -1 if not used.</param>
        /// <param name="keysToPreserve">Array of keys to preserve, null if none.</param>
        protected void AcceptFormatRevisionCore(int istdKey, int[] keysToPreserve)
        {
            FormatRevision revision = FormatRevision;
            if (revision != null)
            {
                WordAttrCollection revPr = revision.RevPr;

                // WORDSNET-3081 If the style was applied during a formatting revision it looks like
                // we should clear all existing attributes. Exclude special things like numbering revision.
                if ((istdKey >= 0) && (revPr.GetDirectAttr(istdKey) != null) && (keysToPreserve != null))
                    Clear(keysToPreserve);

                // We should expand attributes rather than just copy in order to correctly
                // expand complex attributes.
                Remove(RevisionAttr.FormatRevision);
                revPr.ExpandTo(this);
            }

            Remove(RevisionAttr.FormatRevision);
        }

        /// <summary>
        /// Clears collection except attributes with given keys.
        /// </summary>
        internal void Clear(int[] excludeKeys)
        {
            if ((excludeKeys == null) || (excludeKeys.Length == 0))
            {
                // Clear entire collection.
                Clear();
            }
            else
            {
                // Clear exclude given keys.
                int i = 0;
                while (i < Count)
                    if (Array.IndexOf(excludeKeys, GetKey(i)) < 0)
                        RemoveAt(i);
                    else
                        i++;
            }
        }

        /// <summary>
        /// Gets direct complex attribute if exist otherwise sets cloned default and returns it.
        /// </summary>
        /// <remarks>
        /// We need the instance of complex attribute to set its part.
        /// </remarks>
        protected object GetOrCreateComplexAttr(int key)
        {
            object directValue = this[key];

            if (directValue != null)
                return directValue;

            object defaultValue = GetDefaults()[key];

            object clonedValue = ((IComplexAttr)defaultValue).DeepCloneComplexAttr();
            SetAttr(key, clonedValue);

            return clonedValue;
        }

        /// <summary>
        /// Returns true if there is not attribute in the formatting revision, attributes are collapsed.
        /// </summary>
        /// <remarks>
        /// Note: MS Word does not show revisions without changes for accept/reject. However, table revisions for rows and
        /// cells in both cases with or without changes MS Word show for accept/reject as single revision on table level.
        /// </remarks>
        internal bool HasEmptyFormatRevision
        {
            get { return (FormatRevision != null) && !HasFormatRevision; }
        }

        /// <summary>
        /// Returns true if there is at least one attribute in the formatting revision.
        /// </summary>
        internal bool HasFormatRevision
        {
            get { return (FormatRevision != null) && (FormatRevision.RevPr.Count > 0); }
        }

        /// <summary>
        /// Returns true if there is delete revision.
        /// </summary>
        internal bool HasDeleteRevision
        {
            get { return DeleteRevision != null; }
        }

        /// <summary>
        /// Returns true if there is insert revision.
        /// </summary>
        internal bool HasInsertRevision
        {
            get { return InsertRevision != null; }
        }

        /// <summary>
        /// Returns true if the collection has format, insert or delete revision.
        /// </summary>
        internal bool HasRevisions
        {
            get { return (HasFormatRevision || HasInsertRevision || HasDeleteRevision || HasMoveFromRevision || HasMoveToRevision); }
        }

        /// <summary>
        /// Returns true if there is moveFrom revision.
        /// </summary>
        internal bool HasMoveFromRevision
        {
            get { return MoveFromRevision != null; }
        }

        /// <summary>
        /// Returns true if there is moveTo revision.
        /// </summary>
        internal bool HasMoveToRevision
        {
            get { return MoveToRevision != null; }
        }

        /// <summary>
        /// Gets the formatting revision for this collection.
        /// Note this can be null. But even when not null, it can have zero attributes
        /// which still means there is no formatting revision. So use <see cref="HasFormatRevision"/>
        /// to reliably detect a revision.
        /// </summary>
        internal FormatRevision FormatRevision
        {
            get { return (FormatRevision)GetDirectAttr(RevisionAttr.FormatRevision); }
            set { SetAttr(RevisionAttr.FormatRevision, value); }
        }

        /// <summary>
        /// Gets the delete revision for this collection.
        /// Note this can be null. If null does not have delete revision.
        /// </summary>
        internal EditRevision DeleteRevision
        {
            get { return (EditRevision)GetDirectAttr(RevisionAttr.DeleteRevision); }
            set { SetAttr(RevisionAttr.DeleteRevision, value); }
        }

        /// <summary>
        /// Gets the insert revision for this collection.
        /// Note this can be null. If null does not have insert revision.
        /// </summary>
        internal EditRevision InsertRevision
        {
            get { return (EditRevision)GetDirectAttr(RevisionAttr.InsertRevision); }
            set { SetAttr(RevisionAttr.InsertRevision, value); }
        }

        /// <summary>
        /// Gets the MoveFrom revision for this collection.
        /// Note this can be null. If null does not have MoveFrom revision.
        /// </summary>
        internal MoveRevision MoveFromRevision
        {
            get { return (MoveRevision)GetDirectAttr(RevisionAttr.MoveFromRevision); }
            set { SetAttr(RevisionAttr.MoveFromRevision, value); }
        }

        /// <summary>
        /// Gets the MoveTo revision for this collection.
        /// Note this can be null. If null does not have MoveTo revision.
        /// </summary>
        internal MoveRevision MoveToRevision
        {
            get { return (MoveRevision)GetDirectAttr(RevisionAttr.MoveToRevision); }
            set { SetAttr(RevisionAttr.MoveToRevision, value); }
        }

        /// <summary>
        /// Used to save direct formatting of table nodes before TableFormattingExpander expands table styles.
        /// </summary>
        internal WordAttrCollection SysDirectAttrs
        {
            // Sometimes table is not formatted by TableFormattingExpander (pre-Word2002 documents,
            // invalid table style etc). In order to simplify writer this collection can be returned as both
            // resolved and direct attrs if direct attrs was not saved.
            get { return (Contains(SysDirectAttrsBackup) ? ((AttrCollectionBackup)this[SysDirectAttrsBackup]).Pr : this); }
            set { SetAttr(SysDirectAttrsBackup, new AttrCollectionBackup(value)); }
        }

        /// <summary>
        /// Internal attribute.
        /// Used to "backup" direct attributes of table nodes before TableFormattingExpander expands table styles.
        /// Exists only while document is being saved.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int SysDirectAttrsBackup = 9999;
    }
}
