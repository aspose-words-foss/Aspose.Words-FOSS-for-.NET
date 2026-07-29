// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2006 by Roman Korchagin

using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;

namespace Aspose.Words
{
    /// <summary>
    /// Encapsulates parameters passed into a document writer.
    /// </summary>
    internal class SaveInfo
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="stream">Can be null when not saving, but running document validator. For example during page layout only.</param>
        /// <param name="fileName">Can be null when saving to a stream or when not saving.</param>
        /// <param name="saveOptions"></param>
        internal SaveInfo(Document doc, Stream stream, string fileName, SaveOptions saveOptions)
        {
            Debug.Assert(doc != null);
            Debug.Assert(saveOptions != null);

            Document = doc;
            Stream = stream;
            FileName = fileName;
            SaveOptions = saveOptions;

            for (int i = 0; i < UsedParaIdArray.Length; i++)
                UsedParaIdArray[i] = new HashSetGeneric<int>();
        }

        /// <summary>
        /// The document being saved.
        /// </summary>
        internal Document Document;

        /// <summary>
        /// The stream where the document is to be written to.
        /// The writer should not close the stream.
        /// </summary>
        internal Stream Stream;

        /// <summary>
        /// The name of the file that is being saved to (if known).
        /// If not saving to a file, this will be null.
        /// Used by some writers, for example HTML writer to derive file names for images
        /// as they are written into the same folder as the main document.
        /// </summary>
        internal string FileName { get; }

        /// <summary>
        /// The format in which the document is to be saved.
        /// </summary>
        internal SaveFormat SaveFormat
        {
            get { return SaveOptions.SaveFormat; }
        }

        /// <summary>
        /// The options that control how the document is to be saved.
        /// </summary>
        internal SaveOptions SaveOptions;

        /// <summary>
        /// Gets a boolean value indicating either a document is saving
        /// into one of the legacy formats (DOC, DOT, RTF or WML).
        /// </summary>
        internal bool IsLegacyFormat
        {
            get
            {
                return IsDocFormat ||
                        (SaveFormat == SaveFormat.WordML) ||
                        (SaveFormat == SaveFormat.Rtf);
            }
        }

        /// <summary>
        /// All OLE objects including embedded, linked and controls are collected here.
        /// Needed for writing to the binary DOC object pool.
        /// Key is string, value is MemoryStorage.
        /// </summary>
        internal MemoryStorage AllOleObjects = new MemoryStorage();

        /// <summary>
        /// Contains only embedded OLE objects (but not controls).
        /// This is needed for writing WordML.
        /// Key is string, value is MemoryStorage.
        /// </summary>
        internal MemoryStorage EmbeddedOleObjects = new MemoryStorage();

        /// <summary>
        /// Returns true if the document has at least one footnote.
        /// </summary>
        internal bool HasFootnotes;

        /// <summary>
        /// Returns true if the document has at least one endnote.
        /// </summary>
        internal bool HasEndnotes;

        /// <summary>
        /// Returns true if the document has at least one OLE control.
        /// </summary>
        internal bool HasOleControls;

        /// <summary>
        /// Returns true if the document has at least one OLE object.
        /// </summary>
        internal bool HasEmbeddedOleObjects
        {
            get { return (EmbeddedOleObjects.Count > 0); }
        }

        /// <summary>
        /// All XML namespaces used by custom XML markup and smart tags in the document are collected here.
        /// The key is a namespace. The value is an integer id.
        /// </summary>
        internal readonly SortedStringListGeneric<int> XmlNamespaces = new SortedStringListGeneric<int>();

        /// <summary>
        /// Parallel to <see cref="XmlNamespaces"/> list of boolean values indicating that origin of Uri is SmartTag.
        /// The key is namespace id.
        /// </summary>
        internal readonly SortedIntegerListGeneric<bool> IsSmartTagOriginated = new SortedIntegerListGeneric<bool>();

        /// <summary>
        /// All XML namespaces used by custom XML markup that need to be written as attached schemas are collected here.
        /// The key and the value are both the namespace uri.
        /// </summary>
        internal SortedStringListGeneric<string> AttachedSchemas = new SortedStringListGeneric<string>();

        /// <summary>
        /// All unique full smart tag names are collected here.
        /// The key is a uri+name string. The value is a <see cref="Markup.SmartTag"/> object.
        /// </summary>
        internal SortedStringListGeneric<SmartTag> SmartTagTypes = new SortedStringListGeneric<SmartTag>();

        /// <summary>
        /// Contains bookmark names. This is used in HTML export.
        /// </summary>
        internal ISetGeneric<string> BookmarkNamesSet;

        /// <summary>
        /// Contains all shapes of the document.
        /// The key is integer shape id. The value is a shape node.
        /// </summary>
        internal IntToObjDictionary<ShapeBase> Shapes = new IntToObjDictionary<ShapeBase>();

        /// <summary>
        /// Contains ids of shapes that are linked in textbox chains.
        /// </summary>
        internal HashSetGeneric<int> LinkedShapeIds = new HashSetGeneric<int>();

        /// <summary>
        /// Indicates that document contains HTML related information references.
        /// </summary>
        internal bool HasHtmlBlockReferences;

        /// <summary>
        /// Indicates that linked textboxes have conflicting names. This means that textboxes MUST be written linked by ShapeId.
        /// </summary>
        internal bool LinkedShapeNameConflict;

        /// <summary>
        /// Returns format-specific z-order value for shape.
        /// </summary>
        internal int GetShapeZIndex(ShapeBase shape)
        {
            int zIndex = mZIndexTable[shape];
            return ObjToIntDictionary<ShapeBase>.IsNullSubstitute(zIndex) ? 0 : zIndex;
        }

        /// <summary>
        /// Top level shapes are added to a list during document validation because several exporters
        /// need to calculate z-index values for them that are different from ZOrder in the model.
        /// </summary>
        internal void AddToZOrderList(Node shape, StoryTypeStack storyTypeStack)
        {
            if ((shape.ParentNode == null) || (shape.ParentNode.NodeType != NodeType.GroupShape))
                GetZOrderList(storyTypeStack).Add((ShapeBase)shape);
        }

        private List<ShapeBase> GetZOrderList(StoryTypeStack storyTypeStack)
        {
            return (storyTypeStack.IsInFooter)
                ? mZOrderListFooter
                : (storyTypeStack.IsInHeader) ? mZOrderListHeader : mZOrderListMain;
        }

        /// <summary>
        /// Builds a sequence of z-indexes suitable for exporting to WordML. May be used in other formats.
        /// Shapes above text indexes are 1-based. Shapes below text have negative values up to -1.
        /// </summary>
        internal void MakeZIndexVmlStyle()
        {
            MakeZIndexVmlStyle(mZOrderListMain, 1, -1);
            MakeZIndexVmlStyle(Union(mZOrderListHeader, mZOrderListFooter), 1, -1);
        }

        private void MakeZIndexVmlStyle(List<ShapeBase> zOrderList, int minAboveIndex, int maxBelowIndex)
        {
            if (zOrderList.Count == 0)
                return;

            ShapeZOrderSorter<ShapeBase>.Sort(zOrderList);

            // First pass, we number shapes above text starting from minAboveIndex to (minAboveIndex + NumberOfAboveText).
            int zIndex = minAboveIndex;
            int belowTextShapeCount = 0;
            foreach (ShapeBase shape in zOrderList)
            {
                if (shape.IsInline)
                    continue;

                if (!shape.BehindText)
                    mZIndexTable[shape] = zIndex++;
                else
                    belowTextShapeCount++;
            }

            // Second pass, we number shapes below text starting from (maxBelowIndex - NumberOfBelowText + 1) to -1.
            zIndex = maxBelowIndex - belowTextShapeCount + 1;
            foreach (ShapeBase shape in zOrderList)
            {
                if (shape.IsInline)
                    continue;

                if (shape.BehindText)
                    mZIndexTable[shape] = zIndex++;
            }
        }

        private void MakeZIndexRtfStyle(List<ShapeBase> zOrderList)
        {
            if (zOrderList.Count == 0)
                return;

            ShapeZOrderSorter<ShapeBase>.Sort(zOrderList);

            int zIndex = 0;
            foreach (ShapeBase shape in zOrderList)
            {
                // Pseudoinline shapes in RTF seem to have a z-index, so let them through even if they are inline.
                if (shape.IsInline && !shape.IsPseudoInline)
                    continue;

                mZIndexTable[shape] = zIndex++;
            }
        }

        /// <summary>
        /// Builds a sequence of z-indexes for top level shapes. Suitable for use in DOCX export.
        ///
        /// RK This code is experimental. I am trying to figure out how MS Word generates zindex in ooxml.
        /// It seems to increase zindex for every shape by 0x400.
        /// For shapes above text the top byte is 0x0f, for shapes below text the top byte is 0xf1
        /// (which results in a negative value). However, for DrawingML images below text it is still 0x0f.
        ///
        /// Note that if I use 1-based values like in WordML, then MS Word fails to place DrawingML ojects properly.
        /// Apart from difference in actual values, the algorithm is exactly the same as for WML.
        /// We can change WML writer to use this code (but will have to accept all new golds),
        /// but we should not change DOCX writer to use WML zindexes.
        /// </summary>
        internal void MakeZIndexDocxStyle()
        {
            MakeZIndexDocxStyle(mZOrderListMain);

            // WORDSNET-21517 Splitting the images in the headers and footers into two independent subgroups.
            // The Z-ordering of the images is generated for each subgroup separately.
            MakeZIndexDocxStyle(mZOrderListHeader);
            MakeZIndexDocxStyle(mZOrderListFooter);
        }

        private void MakeZIndexDocxStyle(List<ShapeBase> zOrderList)
        {
            if (zOrderList.Count == 0)
                return;

            ShapeZOrderSorter<ShapeBase>.Sort(zOrderList);

            // Number shapes in one pass.
            int zOrder = 0;
            foreach (ShapeBase shape in zOrderList)
            {
                if (shape.IsInline)
                    continue;

                mZIndexTable.Add(shape, ZOrderUtil.ZOrderToZIndex(zOrder, shape.BehindText && (shape.MarkupLanguage != ShapeMarkupLanguage.Dml)));
                zOrder++;
            }
        }

        /// <summary>
        /// Builds a sequence of z-indexes for top level shapes. Suitable for use in HTML export.
        /// </summary>
        internal void MakeZIndexHtmlStyle()
        {
            MakeZIndexVmlStyle(mZOrderListMain, 0, -1);
            MakeZIndexVmlStyle(Union(mZOrderListHeader, mZOrderListFooter), -0x10000, -0x10001);
        }

        /// <summary>
        /// Builds a 0-based sequence of z-indexes for top level shapes. Suitable for use in RTF export.
        /// </summary>
        internal void MakeZIndexRtfStyle()
        {
            MakeZIndexRtfStyle(mZOrderListMain);
            MakeZIndexRtfStyle(Union(mZOrderListHeader, mZOrderListFooter));
        }

        /// <summary>
        /// Returns true on saving to Word format before OOXML ISO Transitional (OOXML ECMA-376, DOC, RTF or WML).
        /// </summary>
        internal bool IsPreIsoTransitionalWordFormat()
        {
            return (IsOoxmlFormat && (OoxmlCompliance == OoxmlComplianceCore.Ecma376)) || IsLegacyFormat;
        }

        /// <summary>
        /// Indicates that comment has comment range.
        /// </summary>
        internal bool HasRange(Comment comment)
        {
            return mCommentsWithRange.Contains(comment);
        }

        /// <summary>
        /// Flags comment as having range.
        /// </summary>
        internal void SetHasRange(Comment comment)
        {
            mCommentsWithRange.Add(comment);
        }

        /// <summary>
        /// Returns the original OfficeMath node that the shape was created from.
        /// </summary>
        internal OfficeMath GetOriginalOfficeMath(ShapeBase shapeBase)
        {
            return (mShapeToConvertedOfficeMath != null)
                ? mShapeToConvertedOfficeMath.GetValueOrNull(shapeBase)
                : null;
        }

        /// <summary>
        /// Unions the two specified lists.
        /// </summary>
        private static List<ShapeBase> Union(List<ShapeBase> list1, List<ShapeBase> list2)
        {
            Debug.Assert((list1 != null) && (list2 != null));

            List<ShapeBase> union = new List<ShapeBase>(list1);
            foreach (ShapeBase shape in list2)
                union.Add(shape);
            return union;
        }

        /// <summary>
        /// Returns true if it is Ooxml format.
        /// </summary>
        /// <remarks>
        /// Docx, Docm, Dotx, Dotm, FlatOpc, FlatOpcMacroEnabled, FlatOpcTemplate, FlatOpcTemplateMacroEnabled.
        /// </remarks>
        internal bool IsOoxmlFormat
        {
            get
            {
                return (IsDocxFormat || IsFlatOpcFormat);
            }
        }

        /// <summary>
        /// Returns true if it is Docx, Docm, Dotx, Dotm format.
        /// </summary>
        internal bool IsDocxFormat
        {
            get
            {
                return ((SaveFormat == SaveFormat.Docx) ||
                        (SaveFormat == SaveFormat.Docm) ||
                        (SaveFormat == SaveFormat.Dotx) ||
                        (SaveFormat == SaveFormat.Dotm));
            }
        }

        /// <summary>
        /// Returns true if it is FlatOpc, FlatOpcMacroEnabled, FlatOpcTemplate, FlatOpcTemplateMacroEnabled format.
        /// </summary>
        internal bool IsFlatOpcFormat
        {
            get
            {
                return ((SaveFormat == SaveFormat.FlatOpc) ||
                        (SaveFormat == SaveFormat.FlatOpcMacroEnabled) ||
                        (SaveFormat == SaveFormat.FlatOpcTemplate) ||
                        (SaveFormat == SaveFormat.FlatOpcTemplateMacroEnabled));
            }
        }

        /// <summary>
        /// Returns true if it is Doc or Dot format.
        /// </summary>
        internal bool IsDocFormat
        {
            get
            {
                return ((SaveFormat == SaveFormat.Doc) ||
                        (SaveFormat == SaveFormat.Dot));
            }
        }

        /// <summary>
        /// Specifies the OOXML version for the output document on saving as DOCX formats.
        /// </summary>
        internal OoxmlComplianceCore OoxmlCompliance
        {
            get
            {
                return OoxmlComplianceInfo.GetCompliance(Document.ComplianceInfo, SaveOptions as OoxmlSaveOptions);
            }
        }

        internal IDictionary<ShapeBase, OfficeMath> ShapeToConvertedOfficeMath
        {
            get { return mShapeToConvertedOfficeMath; }
            set { mShapeToConvertedOfficeMath = value; }
        }

        /// <summary>
        /// Indicates either there are any paragraphs with font effects in <see cref="Paragraph.ParagraphBreakRunPr"/>
        /// </summary>
        /// <remarks>
        /// This feature available in LibreOffice only and is supported by AW per WORDSNET-17459
        /// </remarks>
        internal bool HasParagraphGraphicsExtension { get; set; }

        /// <summary>
        /// Array of connector rules for shapes in body.
        /// </summary>
        internal readonly IList<ShapeBase> ConnectorsBody = new List<ShapeBase>();

        /// <summary>
        /// Array of connector rules for shapes in header/footer.
        /// </summary>
        internal readonly IList<ShapeBase> ConnectorsHdr = new List<ShapeBase>();

        /// <summary>
        /// Existing ParaId collected during validation stage grouped by story type.
        /// </summary>
        internal readonly object[] UsedParaIdArray =
            new object[(int)StoryType.EndnoteContinuationSeparator];

        /// <summary>
        /// Existing row ParaId collected during validation stage.
        /// </summary>
        internal readonly HashSetGeneric<int> UsedRowParaIds = new HashSetGeneric<int>();

        /// <summary>
        /// Existing comment DurableId collected during validation stage.
        /// </summary>
        internal readonly HashSetGeneric<int> UsedCommentDurableIds = new HashSetGeneric<int>();

        private readonly HashSetGeneric<Comment> mCommentsWithRange = new HashSetGeneric<Comment>();

        /// <summary>
        /// Table of format specific z-index values for top level shapes.
        /// </summary>
        private readonly ObjToIntDictionary<ShapeBase> mZIndexTable = new ObjToIntDictionary<ShapeBase>();

        private readonly List<ShapeBase> mZOrderListMain = new List<ShapeBase>();
        private readonly List<ShapeBase> mZOrderListHeader = new List<ShapeBase>();
        private readonly List<ShapeBase> mZOrderListFooter = new List<ShapeBase>();

        /// <summary>
        /// Maps original ShapeBase objects to converted OfficeMath objects
        /// when document is saved to format which doesn't support OMML.
        /// Keys are instances of <see cref="ShapeBase"/>.
        /// Values are instances of <see cref="OfficeMath"/>.
        /// </summary>
        private IDictionary<ShapeBase, OfficeMath> mShapeToConvertedOfficeMath;

        /// <summary>
        /// Collection of SDT forced to write ShowingPlaceholder property.
        /// </summary>
        internal HashSetGeneric<StructuredDocumentTag> ForceShowingPlaceholder = new HashSetGeneric<StructuredDocumentTag>();
    }
}
