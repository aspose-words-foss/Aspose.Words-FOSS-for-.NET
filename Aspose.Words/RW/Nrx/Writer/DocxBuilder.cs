// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/05/2007 by Vladimir Averkin

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.OpcPackaging;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Writer
{
    internal class DocxBuilder : NrxXmlBuilder
    {
        /// <summary>
        /// Creates DOCX builder, which writes in UTF8 encoding. MS Word writes in UTF8 too.
        /// </summary>
        /// <remarks>
        /// WORDSNET-12920 MS Word versions 2007-2019 never writes BOM to DOCX parts.
        /// AW writes BOM because we are using the static UTF8 property on the Encoding class.
        /// When the GetPreamble method is called on the instance of the Encoding class returned by the UTF8 property,
        /// it returns the byte order mark (the byte array of three characters) and is written to the stream before any other content
        /// is written to the stream (assuming a new stream).
        /// We can avoid this by creating the instance of the UTF8Encoding(false) class yourself, like below.
        /// </remarks>
        internal DocxBuilder(OpcPackagePart part, bool isPrettyFormat, OoxmlComplianceCore compliance,
            MsWordVersionCore extensionVersion, IWarningCallback warningCallback) :
            base(part.Stream, new UTF8Encoding(false), isPrettyFormat, (compliance == OoxmlComplianceCore.Ecma376))
        {
            mPart = part;
            mCompliance = compliance;
            mMsWordExtensionVersion = extensionVersion;
            mDocxNamespaces = new DocxNamespaces(mCompliance == OoxmlComplianceCore.IsoStrict);
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// Writes the start of a document with the 9 standard OOXML namespaces and prefixes.
        /// </summary>
        internal void StartDocumentWithStandardNamespaces(string rootName)
        {
            StartDocument(rootName);

            if (IsIso29500Up)
            {
                WriteAttributeString("xmlns:wpc", mDocxNamespaces.WordprocessingCanvas);
                WriteWord2016SpecificNs();
                WriteMarkupCompatibilityNs();
            }
            else
            {
                WriteAttributeString("xmlns:ve", mDocxNamespaces.MarkupCompatibility);
            }

            WriteAttributeString("xmlns:o", NrxNamespaces.Office);
            WriteAttributeString("xmlns:r", mDocxNamespaces.Relationships);
            WriteAttributeString("xmlns:m", mDocxNamespaces.Math);
            WriteAttributeString("xmlns:v", NrxNamespaces.Vml);

            if (IsIso29500Up)
                WriteAttributeString("xmlns:wp14", mDocxNamespaces.DrawingMLIso29500);

            WriteAttributeString("xmlns:wp", mDocxNamespaces.DrawingML);
            WriteAttributeString("xmlns:w10", NrxNamespaces.Word);
            WriteAttributeString("xmlns:w", mDocxNamespaces.Main);

            if (IsIso29500Up)
            {
                WriteWord2012SpecificNs();
            }

            WriteAttributeString("xmlns:wne", mDocxNamespaces.WmlBeta);

            if (IsIso29500Up)
            {
                WriteW16Ns();

                WriteAttributeString("xmlns:wpg", mDocxNamespaces.WordprocessingGroup);
                WriteAttributeString("xmlns:wpi", mDocxNamespaces.WordprocessingInk);
                WriteAttributeString("xmlns:wps", mDocxNamespaces.WordrocessingShape);
                WriteIgnorableNs(true);
            }
        }

        /// <summary>
        /// Writes Word Markup extension namespaces introduced in Word 2016 and Word 2019.
        /// </summary>
        private void WriteW16Ns()
        {
            if (mMsWordExtensionVersion >= MsWordVersionCore.Word2016)
            {
                WriteAttributeString("xmlns:w16cid", mDocxNamespaces.W16Cid);
                WriteAttributeString("xmlns:w16se", mDocxNamespaces.W16Symex);
            }

            if (mMsWordExtensionVersion >= MsWordVersionCore.Word2019)
            {
                WriteAttributeString("xmlns:w16", mDocxNamespaces.W16Markup);
                WriteAttributeString("xmlns:w16cex", mDocxNamespaces.W16Cex);
                WriteAttributeString("xmlns:w16sdtdh", mDocxNamespaces.W16Sdtdh);
            }
        }

        /// <summary>
        /// Writes main element w:fonts of fontTable.xml, takes OOXML compliance into the account to properly write namespaces.
        /// </summary>
        /// <remarks>Keeps Namespace order the same way MS Word does.</remarks>
        internal void StartFontTableDocumentPart()
        {
#if !OPTIMIZED
            Debug.AssertCallingClass("DocxFontTableWriter");
#endif

            StartDocument("w:fonts");
            WriteStandardNamespaces();
        }

        internal void StartExtendedPropertiesDocumentPart()
        {
#if !OPTIMIZED
            Debug.AssertCallingClass("DocxExtentedPropertiesWriter");
#endif
            StartDocument("Properties");

            if (IsIso29500Strict)
            {
                WriteAttributeString("xmlns",
                    "http://purl.oclc.org/ooxml/officeDocument/extendedProperties");
                WriteAttributeString("xmlns:vt",
                    "http://purl.oclc.org/ooxml/officeDocument/docPropsVTypes");
            }
            else
            {
                WriteAttributeString("xmlns",
                    "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
                WriteAttributeString("xmlns:vt",
                    "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            }
        }

        internal void StartCustomPropertiesDocumentPart()
        {
#if !OPTIMIZED
            Debug.AssertCallingClass("DocxCustomPropertiesWriter");
#endif
            StartDocument("Properties");

            if (IsIso29500Strict)
            {
                WriteAttributeString("xmlns",
                    "http://purl.oclc.org/ooxml/officeDocument/customProperties");
                WriteAttributeString("xmlns:vt",
                    "http://purl.oclc.org/ooxml/officeDocument/docPropsVTypes");
            }
            else
            {
                WriteAttributeString("xmlns",
                    "http://schemas.openxmlformats.org/officeDocument/2006/custom-properties");
                WriteAttributeString("xmlns:vt",
                    "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            }
        }

        /// <summary>
        /// Writes main element w:settings of settings.xml, takes OOXML compliance into the account to properly write namespaces.
        /// </summary>
        /// <remarks>Keeps Namespace order the same way MS Word does.</remarks>
        internal void StartSettingsDocumentPart()
        {
#if !OPTIMIZED
            Debug.AssertCallingClass("DocxSettingsWriter");
#endif

            StartDocument("w:settings");

            if (IsIso29500Up)
                WriteMarkupCompatibilityNs();

            WriteAttributeString("xmlns:o", NrxNamespaces.Office);
            WriteAttributeString("xmlns:r", mDocxNamespaces.Relationships);
            WriteAttributeString("xmlns:m", mDocxNamespaces.Math);
            WriteAttributeString("xmlns:v", NrxNamespaces.Vml);
            WriteAttributeString("xmlns:w10", NrxNamespaces.Word);
            WriteAttributeString("xmlns:w", mDocxNamespaces.Main);

            if (IsIso29500Up)
            {
                WriteWord2012SpecificNs();
                WriteW16Ns();
            }

            WriteAttributeString("xmlns:sl", mDocxNamespaces.SchemaLibrary);

            if (IsIso29500Up)
                WriteIgnorableNs(false);
        }

        /// <summary>
        /// Writes main element w:webSettings of webSettings.xml, takes OOXML compliance into the account to properly write namespaces.
        /// </summary>
        /// <remarks>Keeps Namespace order the same way MS Word does.</remarks>
        internal void StartWebSettingsDocumentPart()
        {
#if !OPTIMIZED
            Debug.AssertCallingClass("DocxWebSettingsWriter");
#endif
            StartDocument("w:webSettings");
            WriteStandardNamespaces();
        }

        /// <summary>
        /// Writes main element w:styles of styles.xml, takes OOXML compliance into the account to properly write namespaces.
        /// </summary>
        /// <remarks>Keeps Namespace order the same way MS Word does.</remarks>
        internal void StartStylesDocumentPart()
        {
#if !OPTIMIZED
            Debug.AssertCallingClass("DocxStylesWriter");
#endif
            StartDocument("w:styles");
            WriteStandardNamespaces();
        }

        #region Namespace Writing Routines

        private void WriteStandardNamespaces()
        {
            if (IsIso29500Up)
                WriteMarkupCompatibilityNs();

            WriteAttributeString("xmlns:r", mDocxNamespaces.Relationships);
            WriteAttributeString("xmlns:w", mDocxNamespaces.Main);

            if (IsIso29500Up)
            {
                WriteWord2012SpecificNs();
                WriteW16Ns();
                WriteIgnorableNs(false);
            }
        }

        private void WriteMarkupCompatibilityNs()
        {
            WriteAttributeString("xmlns:mc", mDocxNamespaces.MarkupCompatibility);
        }

        private void WriteWord2012SpecificNs()
        {
            WriteAttributeString("xmlns:w14", mDocxNamespaces.W14Markup);
            WriteAttributeString("xmlns:w15", mDocxNamespaces.W15Markup);
        }

        /// <summary>
        /// Writes specific namespaces that are written since 2016 version of MS Word.
        /// </summary>
        private void WriteWord2016SpecificNs()
        {
            WriteAttributeString("xmlns:cx", mDocxNamespaces.ChartEx);
            WriteAttributeString("xmlns:cx1", mDocxNamespaces.ChartEx1);
            WriteAttributeString("xmlns:cx2", mDocxNamespaces.ChartEx2);
            WriteAttributeString("xmlns:cx3", mDocxNamespaces.ChartEx3);
            WriteAttributeString("xmlns:cx4", mDocxNamespaces.ChartEx4);
            WriteAttributeString("xmlns:cx5", mDocxNamespaces.ChartEx5);
        }

        private void WriteIgnorableNs(bool writeDrawingNs)
        {
            string prefixes =
                "w14 w15" +
                ((writeDrawingNs) ? " wp14" : "") +
                ((mMsWordExtensionVersion >= MsWordVersionCore.Word2016) ? " w16se w16cid" : "") +
                ((mMsWordExtensionVersion >= MsWordVersionCore.Word2019) ? " w16 w16cex w16sdtdh" : "");

            WriteAttributeString("mc:Ignorable", prefixes);
        }

        #endregion

        /// <summary>
        /// Writes border element.
        /// </summary>
        /// <param name="elementName">Name of the border element.</param>
        /// <param name="border">Border value.</param>
        protected override void WriteBorderCore(string elementName, Border border)
        {
            WriteElementWithAttributes(
                elementName,
                "w:val", DocxEnum.LineStyleToDocx(border.LineStyle),
                "w:sz", border.RawLineWidth,
                "w:space", border.RawDistanceFromText,
                "w:color", border.ColorInternal,
                "w:themeColor", border.ThemeColorInternal,
                "w:themeShade", border.ThemeShade,
                "w:themeTint", border.ThemeTint,
                "w:shadow", DontWriteOff(border.Shadow),
                "w:frame", DontWriteOff(border.Frame));
        }

        /// <summary>
        /// Writes w:shd element.
        /// </summary>
        /// <param name="shading">Shading value.</param>
        internal override void WriteShd(Shading shading)
        {
            // Example:
            //     <w:shd w:val="pct-12" w:color="auto" w:fill="0B1621" wx:bgcolor="0B1520" />

            if (shading == null || shading.IsInherited)
                return;

            WriteElementWithAttributes(
                "w:shd",
                "w:val", StyleConvertUtil.TextureIndexToXml(shading.Texture, true),
                "w:color", shading.ForegroundPatternColorInternal,
                "w:fill", shading.BackgroundPatternColorInternal,
                "w:themeColor", shading.ThemeColor,
                "w:themeShade", shading.ThemeShade,
                "w:themeTint", shading.ThemeTint,
                "w:themeFill", shading.ThemeFill,
                "w:themeFillShade", shading.ThemeFillShade,
                "w:themeFillTint", shading.ThemeFillTint);
        }

        /// <summary>
        /// Writes element with the specified name and "w:val" attribute with "0" value if specified boolean value is false
        /// and "1" value if specified boolean value is true.
        /// </summary>
        internal override void WriteBoolValExplicit(string elementName, bool value)
        {
            WriteVal(elementName, value ? "1" : "0");
        }

        /// <summary>
        /// Writes element with the specified name and "w:val" attribute with "0" value if specified boolean value is false.
        /// If true, the empty element with the specified name is written.
        /// </summary>
        internal override void WriteVal(string elementName, bool value)
        {
            if (value)
                WriteEmptyElement(elementName); // do not write "1", "1" is default
            else
                WriteVal(elementName, "0");
        }

        /// <summary>
        /// RK Override to write "1/0" instead of "on/off".
        /// </summary>
        internal override void WriteAttribute(string attributeName, bool value)
        {
            WriteAttributeString(attributeName, value ? "1" : "0");
        }

        internal override void WriteAttributeIfTrue(string attributeName, bool value)
        {
            if (value)
                WriteAttributeString(attributeName, "1");
        }

        /// <summary>
        /// Writes format revision element start.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        internal override void WriteRevisionStart(FormatRevision revision, string elementName, int id)
        {
            WriteRevisionStartCore(elementName, id, revision.Author, revision.DateTime);
        }

        /// <summary>
        /// Writes edit revision element start.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        internal override void WriteRevisionStart(EditRevision revision, int id)
        {
            string elemName = (revision.Type == EditRevisionType.Insertion) ? "w:ins" : "w:del";
            WriteRevisionStartCore(elemName, id, revision.Author, revision.DateTime);
        }

        /// <summary>
        /// Writes move revision element start.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        internal override void WriteRevisionStart(MoveRevision revision, int id)
        {
            string elemName = (revision.Type == MoveRevisionType.MoveTo) ? "w:moveTo" : "w:moveFrom";
            WriteRevisionStartCore(elemName, id, revision.Author, revision.DateTime);
        }

        /// <summary>
        /// Writes revision element end. Provided for symmetry with StartRevision.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        internal override void WriteRevisionEnd()
        {
            EndElement();
        }

        /// <summary>
        /// Writes a numbering revision.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        internal override void WriteRevision(ParagraphNumberRevision revision, int id)
        {
            string elemName = revision.IsInsertion ? "w:ins" : "w:numberingChange";
            // The numberingChange element was removed in ISO Strict.
            if ((elemName == "w:numberingChange") && (OoxmlCompliance == OoxmlComplianceCore.IsoStrict))
            {
                Warn(WarningType.MinorFormattingLoss,
                    string.Format(WarningStrings.NotSupportedByIsoStrict, "numberingChange"));
                return;
            }

            WriteRevisionStartCore(elemName, id, revision.Author, revision.DateTime);

            // WORDSNET-9507 In the DOCX specification mentioned that w:original is optional.
            // But for some reason MS Word cannot open DOCX/WML document with omited w:original.
            // So write it in all cases, even when it is empty string.
            if (revision.IsNumbering)
                WriteAttributeString("w:original", NrxXmlUtil.GetOriginal(revision));

            EndElement(elemName);
        }

        internal override void WriteRevision(FieldNumberRevision revision, int id)
        {
            if (OoxmlCompliance == OoxmlComplianceCore.IsoStrict)
            {
                Warn(
                    WarningType.MinorFormattingLoss,
                    string.Format(WarningStrings.NotSupportedByIsoStrict, "numberingChange"));
                return;
            }

            WriteRevisionStartCore("w:numberingChange", id, revision.Author, revision.DateTime);
            WriteAttributeString("w:original", revision.Original);
            EndElement("w:numberingChange");
        }

        internal override void WriteRevision(EditRevision revision, int id)
        {
            string elemName = (revision.Type == EditRevisionType.Insertion) ? "w:ins" : "w:del";
            WriteRevisionStartElement(elemName, revision, id);
        }

        /// <summary>
        /// Writes move revision element.
        /// </summary>
        internal override void WriteRevision(MoveRevision revision, int id)
        {
            if (revision.Type != MoveRevisionType.None)
            {
                string elemName = (revision.Type == MoveRevisionType.MoveFrom) ? "w:moveFrom" : "w:moveTo";
                WriteRevisionStartElement(elemName, revision, id);
            }
        }

        /// <summary>
        /// Writes cell revision.
        /// </summary>
        /// <remarks>Cell revisions can be only inside OOXML documents.</remarks>
        internal override void WriteCellRevision(EditRevision revision, int id)
        {
            string elemName = (revision.Type == EditRevisionType.Insertion) ? "w:cellIns" : "w:cellDel";
            WriteRevisionStartElement(elemName, revision, id);
        }

        /// <summary>
        /// Writes SDT edit revision start.
        /// </summary>
        internal override void WriteSdtRevisionStart(EditRevision revision, int id)
        {
            string elemName = (revision.Type == EditRevisionType.Insertion) ? "w:customXmlInsRangeStart" : "w:customXmlDelRangeStart";
            WriteRevisionStartElement(elemName, revision, id);
        }

        /// <summary>
        /// Writes SDT move revision start.
        /// </summary>
        internal override void WriteSdtRevisionStart(MoveRevision revision, int id)
        {
            if (revision.Type == MoveRevisionType.None)
                return;

            string elemName = (revision.Type == MoveRevisionType.MoveFrom)
                ? "w:customXmlMoveFromRangeStart"
                : "w:customXmlMoveToRangeStart";
            WriteRevisionStartElement(elemName, revision, id);
        }

        /// <summary>
        /// Writes SDT edit revision end.
        /// </summary>
        internal override void WriteSdtRevisionEnd(EditRevision revision, int id)
        {
            string elemName = (revision.Type == EditRevisionType.Insertion) ? "w:customXmlInsRangeEnd" : "w:customXmlDelRangeEnd";
            WriteRevisionStartCore(elemName, id, "", DateTime.MinValue);
            EndElement(elemName);
        }

        /// <summary>
        /// Writes SDT move revision end.
        /// </summary>
        internal override void WriteSdtRevisionEnd(MoveRevision revision, int id)
        {
            if (revision.Type == MoveRevisionType.None)
                return;

            string elemName = (revision.Type == MoveRevisionType.MoveFrom)
                ? "w:customXmlMoveFromRangeEnd"
                : "w:customXmlMoveToRangeEnd";
            WriteRevisionStartCore(elemName, id, "", DateTime.MinValue);
            EndElement(elemName);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        internal void Warn(WarningType type, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(type, WarningSource.Docx, description));
        }

        /// <summary>
        /// Writes revision start element.
        /// </summary>
        private void WriteRevisionStartElement(string elemName, RevisionBase revision, int id)
        {
            WriteRevisionStartCore(elemName, id, revision.Author, revision.DateTime);
            EndElement(elemName);
        }

        private void WriteRevisionStartCore(string elementName, int id, string author, DateTime dateTime)
        {
            StartElement(elementName);
            WriteAttribute("w:id", id);
            WriteAttribute("w:author", author);
            if (dateTime != DateTime.MinValue)
                WriteAttribute("w:date", FormatterPal.DateTimeToXmlUtc(dateTime));
        }

        internal override bool IsStylesBuilder
        {
            get { return (mPart.ContentType == DocxContentType.Styles); }
        }

        /// <summary>
        /// Writes an element that contains only one attribute "r:id".
        /// </summary>
        internal void WriteRelationshipId(string elemName, string relId)
        {
            StartElement(elemName);
            WriteAttributeString("r:id", relId);
            EndElement();
        }

        /// <summary>
        /// WORDSNET-6471 See "part 1 reference c059575_ISO_IEC_29500-1_2011 §17.18.38".
        /// This simple type specifies that its contents shall contain a color value in RRGGBB hexadecimal format, specified
        /// using six hexadecimal digits. Each of the red, green, and blue color values, from 0-255, is encoded as two
        /// hexadecimal digits.
        /// </summary>
        internal static string GetColorString(DrColor color)
        {
            return string.Format("{0}{1}{2}", FormatterPal.IntToStrX2Lower(color.R),
                FormatterPal.IntToStrX2Lower(color.G),
                FormatterPal.IntToStrX2Lower(color.B));
        }

        internal OpcPackagePart Part
        {
            get { return mPart; }
        }

        /// <summary>
        /// Keeps record of shapetype definitions already written to the document part where this builder writes to.
        /// </summary>
        internal IList<string> ShapeTypesWritten
        {
            get
            {
                if (mShapeTypesWritten == null)
                    mShapeTypesWritten = new List<string>();

                return mShapeTypesWritten;
            }
        }

        /// <summary>
        /// RsidR string for the current paragraph.
        /// Should be refactored to stack to work correctly as paragraphs can be nested.
        /// But since RSIDs implementation in the model is incomplete anyway - I leave it as is for now.
        /// </summary>
        internal string RsidR
        {
            get { return mRsidR; }
            set { mRsidR = value; }
        }

        /// <summary>
        /// Requires builder to adhere to one of <see cref="Saving.OoxmlCompliance"/> standards.
        /// </summary>
        internal OoxmlComplianceCore OoxmlCompliance
        {
            get { return mCompliance; }
        }

        private bool IsIso29500Up
        {
            get { return mCompliance != OoxmlComplianceCore.Ecma376; }
        }

        /// <summary>
        /// Gets a flag indicating that the output document should conforms to ISO/IEC 29500 Strict specification.
        /// </summary>
        private bool IsIso29500Strict
        {
            get { return mCompliance == OoxmlComplianceCore.IsoStrict; }
        }

        private List<string> mShapeTypesWritten;
        private string mRsidR;
        private readonly OpcPackagePart mPart;
        private readonly DocxNamespaces mDocxNamespaces;
        private readonly IWarningCallback mWarningCallback;
        private readonly OoxmlComplianceCore mCompliance;
        private readonly MsWordVersionCore mMsWordExtensionVersion;
    }
}
