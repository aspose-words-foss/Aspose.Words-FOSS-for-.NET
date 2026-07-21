// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2020 by Ilya Navrotskiy

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.HtmlCommon;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown ImageDescription block.
    /// </summary>
    internal class ImageDescriptionBlock : ReferenceBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return false;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            Debug.Assert(IsOpened);

            Add(block);
            return true;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            // Normally, there cannot be ImageDescription without corresponding LinkDestination.
            Debug.Assert(LinkDestinationBlock != null);

            // FOSS Storing an SVG image the way OOXML does requires a rasterized PNG fallback,
            // which depends on the removed rendering subsystem. Rather than throw while trying to
            // rasterize on load, skip SVG images entirely so they are not added to the model.
            // Full SVG support for Markdown is left as a later feature.
            if (LinkDestinationBlock.IsSvgDataUrl)
                return;

            Document doc = context.Builder.Document;
            Shape shape = new Shape(doc, ShapeType.Image);
            shape.WrapType = WrapType.Inline;
            shape.SetShapeAttrInternal(ShapeAttr.ShapeDescription, Text);
            shape.ImageData.SourceFullName = LinkDestinationBlock.Uri;
            if (LinkDestinationBlock.Title != "")
                shape.SetShapeAttrInternal(ShapeAttr.ImageTitle, LinkDestinationBlock.Title);
            shape.Name = DefinitionLabel;

            // WORDSNET-21685 Update shape size from the actual image bytes.
            HtmlResourceLoader resourceLoader = new HtmlResourceLoader(doc.ResourceLoadingCallback, doc.WarningCallback);
            resourceLoader.UpdateShape(shape);

            context.Builder.InsertNode(shape);
        }

        /// <summary>
        /// Returns true, if specified delimiters can constitute a valid visible text of the link.
        /// </summary>
        internal static bool IsValid(ImageDescriptionOpeningDelimiter opening, Delimiter closing)
        {
            Debug.Assert((opening != null) && (closing != null));
            Debug.Assert(opening.IsBefore(closing));

            LinkTextOpeningDelimiter linkTextOpening = opening.LinkTextOpening;
            if (linkTextOpening == null)
                return false;

            return LinkTextBlock.IsValid(linkTextOpening, closing);
        }

        /// <summary>
        /// A type of the block.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.ImageDescription; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Opening string delimiter.
        /// </summary>
        protected override string OpeningStringDelimiter
        {
            get { return "!["; }
        }

        /// <summary>
        /// The Opening delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const string OpeningDelimiter = "![";

        /// <summary>
        /// The Closing delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const string ClosingDelimiter = "]";
    }
}
