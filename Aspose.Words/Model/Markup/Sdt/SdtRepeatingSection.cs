// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2010 by Andrey Noskov

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Collections.Generic;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Word Extensions to the Office Open XML.
    /// Office2013 SDT type which specifies the properties of a structured document tag (as specified in [ISO/IEC29500-1:2011]
    /// section 17.5.2) in the form of a repeated section.
    /// </summary>
    internal class SdtRepeatingSection : SdtControlProperties
    {
        /// <summary>
        /// Provides data and methods needed by SDT repeating sections update.
        /// </summary>
        private class SdtRepeatingSectionContext
        {
            private readonly HashSetGeneric<SdtRepeatingSection> mUpdatedSections = new HashSetGeneric<SdtRepeatingSection>();

            internal SdtRepeatingSectionContext(string rootXPath)
            {
                RootXPath = rootXPath;
            }

            internal HashSetGeneric<SdtRepeatingSection> UpdatedSections
            {
                get { return mUpdatedSections; }
            }

            internal int CurrentDataItemIndex { get; set; }

            /// <summary>
            /// Used for building child XPaths for repeatingSectionItems.
            /// </summary>
            internal string RootXPath { get; }
        }

        internal override SdtType Type
        {
            get { return SdtType.RepeatingSection; }
        }

        /// <summary>
        /// An optional element that specifies the display name of the repeated section.
        /// </summary>
        internal string SectionTitle
        {
            get { return mSectionTitle; }
            set { mSectionTitle = value; }
        }

        /// <summary>
        /// An optional element that specifies whether to allow the insertion of new or deletion of old repeatingSectionItems
        /// contained within the structured document tag except when needed to maintain the numerical relation between
        /// the number of elements matched through data binding and the number of repeatingSectionItems contained.
        /// </summary>
        internal bool DoNotAllowInsertDeleteSection
        {
            get { return mDoNotAllowInsertDeleteSection; }
            set { mDoNotAllowInsertDeleteSection = value; }
        }

        /// <summary>
        /// Update content of the repeatingSection element that specifies that the parent structured document tag
        /// is a container for repeated section items.
        /// </summary>
        /// <dev>AM. Still looking for better place for this code.</dev>
        internal static bool UpdateRepeatingSection(StructuredDocumentTag sdt)
        {
            if (sdt.XmlMapping.IsEmpty)
                return true;

            if (!sdt.XmlMapping.IsValid)
                return false;

            ExtractRepeatingSectionItems(sdt);

            SdtRepeatingSectionContext context = new SdtRepeatingSectionContext(sdt.XmlMapping.XPath);
            if (sdt.IsShowingPlaceholderText && (sdt.Placeholder == null))
                sdt.IsShowingPlaceholderText = false;

            return UpdateRepeatingSection(sdt, context);
        }

        /// <summary>
        /// Update content of the repeatingSection.
        /// </summary>
        private static bool UpdateRepeatingSection(StructuredDocumentTag sdt, SdtRepeatingSectionContext context)
        {
            SdtRepeatingSection repeatingSection = (SdtRepeatingSection)sdt.ControlProperties;

            if (context.UpdatedSections.Contains(repeatingSection))
                return true;

            NodeCollection childSdtItems = sdt.GetChildNodes(NodeType.StructuredDocumentTag, false);

            foreach (StructuredDocumentTag childSdtItem in childSdtItems)
            {
                if (childSdtItem.IsRepeatingSectionItem)
                    UpdateRepeatingSectionItem(childSdtItem, context);
                else
                    childSdtItem.XmlMapping.UpdateContent();
            }

            // Set flag which indicates that this RepeatingSection is updated.
            // We need this to avoid updating the same RepeatingSection more then one time, in case of nested RepeatingSection.
            context.UpdatedSections.Add(repeatingSection);

            return true;
        }

        /// <summary>
        /// Update content of the repeatingSectionItem that must be contained within a repeatingSection,
        /// and also can contain repeatingSection.
        /// </summary>
        private static void UpdateRepeatingSectionItem(StructuredDocumentTag sdt, SdtRepeatingSectionContext context)
        {
            // The repeatingSectionItem must be either Block-Level, Row-Level, or Cell-Level.
            switch (sdt.Level)
            {
                case MarkupLevel.Row:
                case MarkupLevel.Block:
                {
                    FillWithData(sdt, context);
                    break;
                }
                case MarkupLevel.Cell:
                {
                    // Cannot create such document in MS Word.
                    break;
                }
                default:
                    throw new InvalidOperationException("Unexpected SDT level.");
            }

            context.CurrentDataItemIndex++;
        }

        /// <summary>
        /// Extracts recursively all repeating section items for the specified <paramref name="repeatingSection"/>.
        /// </summary>
        private static void ExtractRepeatingSectionItems(StructuredDocumentTag repeatingSection)
        {
            // There can be some other types of SDTs inside repeating section,
            // than repeating items. See for example TestSdt.TestJira8888().
            // So, we should calculate repeating items actual count.
            int dataItemCount = !repeatingSection.XmlMapping.IsEmpty
                ? repeatingSection.XmlMapping.MappedNodesCount
                : 0;

            // WORDSNET-16155 AW must not remove repeating sections, when document contains invalid XPath in SDT.
            // Don't analyze nested sections, at this case order of sections can be wrong (see TestJira16155).
            if (dataItemCount == 0)
                return;

            CompositeNode parentNode = repeatingSection;

            // WORDSNET-24905 RepeatingSectionItem is not immediate child of RepeatingSection.
            if ((repeatingSection.FirstChild != null) && (repeatingSection.FirstChild.NodeType == NodeType.Table))
                parentNode = ((CompositeNode)repeatingSection.FirstChild);

            NodeCollection repeatingSectionChildren = parentNode.GetChildNodes(NodeType.StructuredDocumentTag, false);

            List<StructuredDocumentTag> itemsToRemove = new List<StructuredDocumentTag>();
            StructuredDocumentTag templateItem = null;

            // Go throughout the all children SDT to find nested repeating sections to update them recursively.
            int repeatingItemsCount = 0;
            for (int i = 0; i < repeatingSectionChildren.Count; i++)
            {
                StructuredDocumentTag repeatingSectionChild = (StructuredDocumentTag)repeatingSectionChildren[i];
                NodeCollection childSdts = repeatingSectionChild.GetChildNodes(NodeType.StructuredDocumentTag, false);

                for (int j = 0; j < childSdts.Count; j++)
                {
                    StructuredDocumentTag childSdt = (StructuredDocumentTag)childSdts[j];

                    // MS Word moves a repeating section with data binding inside the first repeating item up to
                    // the level of a parent repeating section at some cases. See TestSdt.TestNestedRepeatingSection().
                    if (repeatingSectionChild.IsRepeatingSectionItem && childSdt.IsRepeatingSection &&
                        (parentNode.NodeType == NodeType.StructuredDocumentTag))
                        MoveNestedRepeatingSectionIfNeeded(childSdt, (StructuredDocumentTag)parentNode);
                }

                // Count repeating section items and remember last of them
                // to use as the template for all missed repeating items.
                if (repeatingSectionChild.IsRepeatingSectionItem)
                {
                    StructuredDocumentTag repeatingItem = repeatingSectionChild;

                    if (repeatingItem.HasChildNodes && (repeatingItemsCount < dataItemCount))
                    {
                        repeatingItemsCount++;
                        templateItem = repeatingSectionChild;

                        UpdateNestedXPath(repeatingSection, repeatingItem, i);
                    }
                    else
                    {
                        if (!itemsToRemove.Contains(repeatingItem))
                            itemsToRemove.Add(repeatingItem);
                    }
                }
            }

            // Remove superfluous repeating section items.
            foreach (StructuredDocumentTag item in itemsToRemove)
                item.Remove();

            if (templateItem == null)
                return;

            // Create missed repeating items.
            for (int i = repeatingItemsCount; i < dataItemCount; i++)
            {
                StructuredDocumentTag newRepeatingItem = (StructuredDocumentTag)templateItem.Clone(true);

                UpdateNestedXPath(repeatingSection, newRepeatingItem, i);

                parentNode.InsertAfter(newRepeatingItem, templateItem);
            }
        }

        /// <summary>
        /// Ensures child SDT nodes have absolute mapping XPath.
        /// </summary>
        private static void UpdateNestedXPath(StructuredDocumentTag rs, StructuredDocumentTag rsItem, int index)
        {
            foreach (StructuredDocumentTag childTag in rsItem.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                if (childTag.XmlMapping.IsMapped)
                {
                    string parentPath = rs.XmlMapping.XPath;
                    string childPath = childTag.XmlMapping.XPath;

                    if (!IsRelativeXPath(childPath))
                    {
                        // Try to convert absolute XPath to relative.
                        if (childPath.StartsWith(parentPath, StringComparison.InvariantCulture))
                        {
                            childPath = childPath.Remove(0, parentPath.Length);
                            childPath = gIndexRegex.Replace(childPath, @"/", 1);
                        }

                    }

                    if (IsRelativeXPath(childPath))
                    {
                        string newPath = string.Format(@"{0}[{1}]/{2}", parentPath, index + 1, childPath.TrimStart('/'));

                        childTag.XmlMapping.SetXPath(newPath);
                    }
                }
            }
        }

        /// <summary>
        /// Checks that given XPath is relative.
        /// </summary>
        private static bool IsRelativeXPath(string xPath)
        {
            return xPath.StartsWith(@"//", StringComparison.Ordinal);
        }

        /// <summary>
        /// Moves the specified nested repeating section at some cases to be direct child of the parent repeating
        /// section like MS Word does. Such behaviour of MS Word looks as annoying but we have to mimic it.
        /// </summary>
        private static void MoveNestedRepeatingSectionIfNeeded(StructuredDocumentTag childRepeatingSection,
            StructuredDocumentTag parentRepeatingSection)
        {
            if (childRepeatingSection.XmlMapping.IsEmpty || parentRepeatingSection.XmlMapping.IsEmpty)
                return;

            int childRecordCount = childRepeatingSection.XmlMapping.MappedNodesCount;
            int childRepeatingItemCount = CalcChildRepeatingSectionItems(childRepeatingSection);

            if ((childRecordCount == 0) || (childRepeatingItemCount >= childRecordCount))
                return;

            int parentRepeatingItemCount = CalcChildRepeatingSectionItems(parentRepeatingSection);
            if (parentRepeatingItemCount == 1)
            {
                // If childRepeatingItemCount > 1 here, MS Word exchanges parent and child repeating sections.
                // Postponed for now (WORDSNET-18023).
                return;
            }

            parentRepeatingSection.InsertBefore(childRepeatingSection, childRepeatingSection.ParentNode);
        }

        /// <summary>
        /// Calculates number of child repeating section items related to the specified repeating section.
        /// </summary>
        private static int CalcChildRepeatingSectionItems(StructuredDocumentTag repeatingSection)
        {
            int count = 0;

            foreach (StructuredDocumentTag sdt in repeatingSection.GetChildNodes(NodeType.StructuredDocumentTag, false))
            {
                if (sdt.IsRepeatingSectionItem)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Fill repeated item with data using RootXPath as a start key.
        /// </summary>
        private static void FillWithData(StructuredDocumentTag sdt, SdtRepeatingSectionContext context)
        {
            NodeCollection childTags = sdt.GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (StructuredDocumentTag childTag in childTags)
                FillWithDataCore(sdt, childTag, context);
        }

        /// <summary>
        /// Core method to fill sdt with data.
        /// </summary>
        private static void FillWithDataCore(StructuredDocumentTag parentSdt, StructuredDocumentTag sdt, SdtRepeatingSectionContext context)
        {
            string rootXPath = context.RootXPath;
            Regex regex = new Regex(Regex.Escape(rootXPath) + @"\[(\d+)\].*", RegexOptions.IgnoreCase);

            if (!sdt.XmlMapping.IsEmpty && sdt.XmlMapping.IsValid)
            {
                string childXPath = sdt.XmlMapping.XPath;
                string newDataIndex = (context.CurrentDataItemIndex + 1).ToString();

                // Update XPath using rootXPath as 'key'.
                Match match = regex.Match(childXPath);
                if (match.Success)
                {
                    sdt.XmlMapping.SetXPath(ReplaceIndex(match, newDataIndex));

                    // Recursively update SDT data bound content.
                    parentSdt.XmlMapping.UpdateContent();
                }
                else
                {
                    // WORDSNET-12206 If XPath has no node index, add it
                    if (childXPath.Length >= rootXPath.Length &&
                        StringUtil.EqualsIgnoreCase(childXPath.Substring(0, rootXPath.Length), rootXPath) &&
                        (childXPath.Length == rootXPath.Length || childXPath[rootXPath.Length] == '/'))
                    {
                        sdt.XmlMapping.SetXPath(childXPath.Insert(rootXPath.Length, "[" + newDataIndex + "]"));

                        // Recursively update SDT data bound content.
                        parentSdt.XmlMapping.UpdateContent();
                    }
                }
            }
        }

        private static string ReplaceIndex(Match m, string newIndex)
        {
            string capture = m.Value;
            capture = capture.Remove(m.Groups[1].Index, m.Groups[1].Length);
            capture = capture.Insert(m.Groups[1].Index, newIndex);
            return capture;
        }

        private string mSectionTitle;
        private bool mDoNotAllowInsertDeleteSection;

        private static readonly Regex gIndexRegex = new Regex(@"^\[\d+\]");
    }
}
