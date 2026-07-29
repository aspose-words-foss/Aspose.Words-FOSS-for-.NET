// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/09/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Represents a container for the footnote/endnote separator and continuation content of a document.
    /// </summary>
    /// <remarks>
    /// <p><see cref="FootnoteSeparator"/> can contain <see cref="Paragraph"/> and <see cref="Tables.Table">Table</see> child nodes.</p>
    ///
    /// <p>There can only be one <see cref="FootnoteSeparator"/> of each <see cref="FootnoteSeparatorType"/> in a document.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public class FootnoteSeparator : Story
    {
        /// <summary>
        /// Creates a new footnote/endnote separator of the specified type.
        /// </summary>
        internal FootnoteSeparator(DocumentBase doc, FootnoteSeparatorType separatorType)
            : base(doc, WordUtil.FootnoteSeparatorTypeToStoryType(separatorType))
        {
            SeparatorType = separatorType;
        }

        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptChildren(visitor);
        }

        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return VisitorAction.Continue;
        }

        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return VisitorAction.Continue;
        }

        internal override bool CanInsert(Node newChild)
        {
            return true;
        }

        public override NodeType NodeType
        {
            get { return NodeType.System; }
        }

        public FootnoteSeparatorType SeparatorType { get; }

        /// <summary>
        /// Indicates that separator has default formatting.
        /// </summary>
        /// <remarks>
        /// Main purpose of this property is to reduce number of golds affected.
        /// </remarks>
        internal bool IsDefault
        {
            get
            {
                // Default footnote/endnote separator seems to be single paragraph.
                if (GetChildNodes(NodeType.Any, false).Count > 1)
                    return false;

                Paragraph para = GetChildNodes(NodeType.Any, false)[0] as Paragraph;
                if (para == null)
                    return false;

                ParaPr paraPr = para.ParaPr.Clone();
                // WORDSNET-24128 Use final formatting.
                paraPr.AcceptFormatRevision();

                // Remove inherited attributes.
                for (int i = 0; i < paraPr.Count; /* */)
                {
                    int key = paraPr.GetKey(i);
                    object inheritedValue = ((IParaAttrSource)para).FetchInheritedParaAttr(key);
                    object thisValue = paraPr[key];

                    if (Equals(thisValue, inheritedValue))
                        paraPr.RemoveAt(i);
                    else
                        i++;
                }

                ParaPr testPr = new ParaPr();
                testPr.SetAttr(ParaAttr.Istd, 0);
                testPr.SetAttr(ParaAttr.SpaceAfter, 0);
                testPr.SetAttr(ParaAttr.LineSpacing, new LineSpacing(240, LineSpacingRule.Multiple));

                // Ignore system and rsid attributes.
                paraPr.RemoveEquals(testPr);
                paraPr.Remove(ParaAttr.RsidP);
                paraPr.Remove(ParaAttr.Sys_Alignment97);
                paraPr.Remove(ParaAttr.Sys_LeftIndent97);
                paraPr.Remove(ParaAttr.Sys_RightIndent97);

                if (paraPr.Count > 0)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Returns true, if this <see cref="FootnoteSeparator"/> equals to a specified one.
        /// </summary>
        internal bool Equals(FootnoteSeparator other)
        {
            if (other == null)
                return false;

            if (SeparatorType != other.SeparatorType)
                return false;

            NodeCollection paragraphs = GetChildNodes(NodeType.Paragraph, false);
            NodeCollection otherParagraphs = other.GetChildNodes(NodeType.Paragraph, false);

            if (paragraphs.Count != otherParagraphs.Count)
                return false;

            for (int i = 0; i < paragraphs.Count; i++)
            {
                Paragraph paragraph = (Paragraph)paragraphs[i];
                Paragraph otherParagraph = (Paragraph)otherParagraphs[i];

                if (!paragraph.ParaPr.Equals(otherParagraph.ParaPr, gParaPrIgnorableKeys))
                    return false;

                if (!paragraph.ParagraphBreakRunPr.Equals(otherParagraph.ParagraphBreakRunPr, gRunPrIgnorableKeys))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// These keys will be ignored in properties comparison.
        /// </summary>
        private static readonly int[] gParaPrIgnorableKeys = new int[]
        {
            ParaAttr.RsidP, ParaAttr.Sys_Alignment97, ParaAttr.Sys_LeftIndent97, ParaAttr.Sys_RightIndent97,
            ParaAttr.Sys_FirstLineIndent97
        };

        /// <summary>
        /// These keys will be ignored in properties comparison.
        /// </summary>
        private static readonly int[] gRunPrIgnorableKeys = new int[]
        {
            FontAttr.RsidRPr, FontAttr.RsidR, ParaAttr.RsidP
        };
    }
}
