// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/04/2012 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Bidi;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.Images;
using Aspose.IO;
using Aspose.Ss;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Formatting.Intern;
using Aspose.Words.Forms2;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Properties;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Celler;
using Aspose.Words.RW.Factories;
using Aspose.Words.RW.Ole.Ole2;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Words.Themes;
using Aspose.Words.Vba;

namespace Aspose.Words.Tests
{
    internal class MsoEnvelope
    {
        internal enum FlagStatusType
        {
            NotFlagged = 0x00,
            Flagged = 0x01,
            Completed = 0x02
        }

        internal enum SensitivityType
        {
            Normal = 0x00,
            Personal = 0x01,
            Private = 0x02,
            Confidential = 0x03
        }

        private enum PropTagType
        {
            Long = 0x0003,
            Null = 0x0001,
            Boolean = 0x000B,
            Systime = 0x0040,
            Error = 0x000A,
            String8 = 0x001E,
            Unicode = 0x001F,
            Binary = 0x0102,
            MvString8 = 0x101E,
            MvBinary = 0x1102
        }

        internal enum MsoEnvSecurityFlags
        {
            None = 0x00,
            Signed = 0x01,
            Encrypted = 0x02
        }

        internal enum ImportanceType
        {
            Low = 0x00,
            Normal = 0x01,
            High = 0x02
        }

        internal MsoEnvelope(BinaryReader reader)
        {
            Guid clsid = new Guid(reader.ReadBytes(16));
            int ver = reader.ReadInt32();

            LastSentTime = ReadDate(reader);
            FlagStatus = (FlagStatusType)reader.ReadInt32();
            ReplyTime = ReadDate(reader);

            Request = ReadString(reader);
            int sentRepresentingEntryIdSize = reader.ReadInt32();
            SentRepresentingEntryId = reader.ReadBytes(sentRepresentingEntryIdSize);
            SentRepresentingName = ReadString(reader);
            InetAcctStamp = ReadString(reader);
            InetAcctName = ReadString(reader);

            ExpireTime = ReadDate(reader);
            DeferredDeliveryTime = ReadDate(reader);
            DeleteAfterSubmit = reader.ReadInt32() != 0;
            SecurityFlags = (MsoEnvSecurityFlags)reader.ReadInt32();
            OriginatorDeliveryReportRequested = reader.ReadInt32() != 0;
            ReadReceiptRequested = reader.ReadInt32() != 0;

            Categories = ReadString(reader);
            Sensitivity = (SensitivityType)reader.ReadInt32();
            Importance = (ImportanceType)reader.ReadInt32();

            Subject = ReadString(reader);
            VotingOptions = ReadString(reader);

            // ReplyRecipients
            ReplyRecipients = ReadEnvRecipientCollection(reader);

            // ContactLinkRecipients
            ContactLinkRecipients = ReadEnvRecipientCollection(reader);

            // Recipients
            Recipients = ReadEnvRecipientCollection(reader);
        }

        private static Pair[][] ReadEnvRecipientCollection(BinaryReader reader)
        {
            // EnvRecipientCollection
            uint recipientCollTag = reader.ReadUInt32();
            Debug.Assert(0xDCCA0123 == recipientCollTag);
            int recipientCollVer = reader.ReadInt32();
            Debug.Assert(1 == recipientCollVer);

            int count = reader.ReadInt32();
            if (count == 0)
                return null;

            Pair[][] mainList = new Pair[count][];

            for (int i = 0; i < count; i++)
            {
                // EnvRecipientProperties
                int count2 = reader.ReadInt32();
                // Read ignored value.
                reader.ReadInt32();

                Pair[] childList = new Pair[count2];
                mainList[i] = childList;

                // EnvRecipientProperty
                for (int j = 0; j < count2; j++)
                {
                    PropTagType propTag = (PropTagType)reader.ReadInt16();
                    // Read ignored value.
                    reader.ReadInt16();

                    object value;
                    switch (propTag)
                    {
                        case PropTagType.Long:
                        case PropTagType.Error:
                            value = reader.ReadInt32();
                            break;

                        case PropTagType.Boolean:
                            value = reader.ReadInt16() != 0;
                            break;

                        case PropTagType.Unicode:
                            {
                                int size = reader.ReadInt16();
                                byte[] unicode = reader.ReadBytes(size);
                                value = System.Text.Encoding.Unicode.GetString(unicode);
                                break;
                            }

                        case PropTagType.Binary:
                            {
                                int size = reader.ReadInt16();
                                value = reader.ReadBytes(size);
                                break;
                            }

                        default:
                            throw new InvalidOperationException();
                    }

                    childList[j] = new Pair(propTag, value);
                }
            }

            return mainList;
        }

        private static DateTime ReadDate(BinaryReader reader)
        {
            const int undefinedTime = 0x5AE980E0;

            int since = reader.ReadInt32();

            if (since == undefinedTime)
                return DateTime.MinValue;

            DateTime dateTime = new DateTime(1601, 01, 01);
            return dateTime.AddMinutes(since);
        }

        private static string ReadString(BinaryReader reader)
        {
            short size = reader.ReadInt16();
            byte[] bytes = reader.ReadBytes(size * 2);

            return Encoding.Unicode.GetString(bytes);
        }

        internal string Subject;
        internal string Request;
        internal FlagStatusType FlagStatus;

        internal DateTime LastSentTime;
        internal DateTime ReplyTime;
        internal DateTime ExpireTime;
        internal DateTime DeferredDeliveryTime;

        internal SensitivityType Sensitivity;
        internal ImportanceType Importance;
        internal MsoEnvSecurityFlags SecurityFlags;
        internal string VotingOptions;

        internal byte[] SentRepresentingEntryId;

        internal string SentRepresentingName;
        internal string InetAcctStamp;
        internal string InetAcctName;

        internal bool DeleteAfterSubmit;

        internal bool OriginatorDeliveryReportRequested;
        internal bool ReadReceiptRequested;

        internal string Categories;


        internal Pair[][] ReplyRecipients;
        internal Pair[][] ContactLinkRecipients;
        internal Pair[][] Recipients;
    }

    /// <summary>
    /// Log unique warnings.
    /// </summary>
    internal class UniqueWarningCollection : IWarningCallback, IEnumerable<WarningInfo>
    {
        void IWarningCallback.Warning(WarningInfo info)
        {
            foreach(WarningInfo item in mItems)
            {
                if ((item.WarningType == info.WarningType) &&
                    (item.Source == info.Source) &&
                    (item.Description == info.Description))
                    return;
            }

            mItems.Add(info);
        }

        public IEnumerator<WarningInfo> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        private readonly List<WarningInfo> mItems = new List<WarningInfo>();
    }


    /// <summary>
    /// Utility class outputs whole document model into file.
    /// </summary>
    /// <remarks>
    /// AM. This is very draft but still useful for me.
    /// Model dump shows for example how AW/Word converts document upon re-saving and so on.
    /// I'd like this test utility integrated.
    /// </remarks>
    internal class ModelDump
    {
        private ModelDump()
        {
            // No ctor.
        }

        /// <summary>
        /// Saves document model into specified file.
        /// </summary>
        internal static void Save(string fileName)
        {
            gWarningCallback = new UniqueWarningCollection();
            LoadOptions lo = new LoadOptions();
            lo.WarningCallback = gWarningCallback;

            Save(TestUtil.Open(fileName, lo), fileName + ".model");
        }

        /// <summary>
        /// Saves document model into specified file.
        /// </summary>
        internal static void Save(string fileName, LoadOptions lo)
        {
            gWarningCallback = new UniqueWarningCollection();
            lo.WarningCallback = gWarningCallback;

            Save(TestUtil.Open(fileName, lo), fileName + ".model");
        }

        /// <summary>
        /// Saves document model into specified file.
        /// </summary>
        internal static void Save(Document doc, string fileName)
        {
            SortHeadersAndFooters(doc);

            if (JoinRuns)
                JoinRunsWithSameFormatting(doc);

            if(ShowListLabels)
                doc.UpdateListLabels();

            if(NoIstdInStyles)
                foreach (Style style in doc.Styles)
                {
                    style.RunPr.Remove(FontAttr.Istd);
                    style.ParaPr.Remove(ParaAttr.Istd);
                }

            if(GlobalTableAttributes)
                foreach(Table table in doc.GetChildNodes(NodeType.Table, true))
                    ExtractGlobalTableAttrs(table);

            if (UnifyToggles)
                ConvertToggles(doc);

            if (LocalizeFontNames)
            {
                FontInfoCollection fonts = doc.FontInfos;

                DoLocalizeFontNames(doc.Styles.DefaultRunPr, fonts);
                foreach (Style style in doc.Styles)
                    DoLocalizeFontNames(style.RunPr, fonts);

                DoLocalizeFontNames(doc, doc.FontInfos);
            }

            if(LocalizeStyleNames)
                DoLocalizeStyleNames(doc);

            Refine(doc);

            if (NormalizeHorizontalMerge)
                foreach (Table table in doc.GetChildNodes(NodeType.Table, true))
                    table.NormalizeHorizontalMerge();

            if (Cleanup)
            {
                CleanupOptions options = new CleanupOptions() { UnusedBuiltinStyles = true };
                doc.Cleanup(options);
            }

            FileName = fileName;
            FileStream fileStream = File.Create(FileName);
            Writer = new StreamWriter(fileStream, new UTF8Encoding(true));

            DumpNode(doc);

            Writer.Close();
        }

        private static void DoLocalizeFontNames(CompositeNode parentNode, FontInfoCollection fonts)
        {
            foreach (Node node in parentNode.GetChildNodes(NodeType.Any, true))
            {
                IInline inline = node as IInline;
                if (inline != null)
                {
                    DoLocalizeFontNames(inline.RunPr_IInline, fonts);
                    // Don't break here, shapes need additional processing.
                }

                switch (node.NodeType)
                {
                    case NodeType.Paragraph:
                        DoLocalizeFontNames(((Paragraph)node).ParagraphBreakRunPr, fonts);
                        break;

                    case NodeType.Shape:
                        Shape shape = (Shape)node;
                        if (shape.RunPr.AlternateContent != null)
                            DoLocalizeFontNames(shape.RunPr.AlternateContent.FallBack, fonts);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Replaces non-ASCII font names to ASCII alternative font name.
        /// </summary>
        private static void DoLocalizeFontNames(RunPr runPr, FontInfoCollection fonts)
        {
            foreach (int key in RunPr.FontNameAttributes)
            {
                ComplexFontName font = (ComplexFontName)runPr[key];

                if (font == null)
                    continue;

                if(font.IsThemeFont)
                    continue;

                string fontName = font.Name;
                if(IsAscii(fontName))
                    continue;

                FontInfo fontInfo = fonts[fontName];

                foreach(string altName in fontInfo.GetAltNameList())
                    if (IsAscii(altName))
                    {
                        runPr[key] = ComplexFontName.FromName(altName);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets style names to the English language.
        /// </summary>
        private static void DoLocalizeStyleNames(Document doc)
        {
            StyleCollection builtin = StyleCollection.GetBuiltInStyles(LoadFormat.Docx);

            foreach (Style style in doc.Styles)
            {
                if (!style.BuiltIn)
                    continue;

                Style builtinStyle = builtin.GetBySti(style.StyleIdentifier, false);

                if (builtinStyle == null)
                    continue;

                style.SetNameCore(builtinStyle.Name, true);

                if (style.LinkedIstd != StyleIndex.Nil && style.Type == StyleType.Paragraph)
                {
                    Style linkedStyle = doc.Styles.GetByIstd(style.LinkedIstd, false);
                    linkedStyle.SetNameCore(builtinStyle.Name + " Char");
                }
            }
        }

        /// <summary>
        /// Checks that given string contains only ASCII characters.
        /// </summary>
        private static bool IsAscii(string text)
        {
            foreach(char c in text)
                if (((int)c) > 255)
                    return false;

            return true;
        }

        private static void ConvertToggles(Style style, HashSetGeneric<Style> alreadyProcessed)
        {
            if (alreadyProcessed.Contains(style))
                return;

            // Convert base style first.
            if(style.GetBaseStyle() != null)
                ConvertToggles(style.GetBaseStyle(), alreadyProcessed);

            foreach(int key in gToggleAttrs)
            {
                if (style.RunPr.Contains(key))
                {
                    AttrBoolEx value = (AttrBoolEx)style.RunPr[key];

                    if ((value == AttrBoolEx.True) || (value == AttrBoolEx.False))
                    {
                        AttrBoolEx inheritedValue = (AttrBoolEx)((IRunAttrSource)style).FetchInheritedRunAttr(key);

                        style.RunPr.SetAttr(key, (value == inheritedValue)
                            ? AttrBoolEx.Same
                            : AttrBoolEx.Toggle);
                    }
                }
            }

            alreadyProcessed.Add(style);
        }

        private static void ConvertToggles(IInline inline)
        {
            foreach(int key in gToggleAttrs)
            {
                if (inline.RunPr_IInline.Contains(key))
                {
                    AttrBoolEx value = (AttrBoolEx)inline.RunPr_IInline[key];

                    if ((value == AttrBoolEx.True) || (value == AttrBoolEx.False))
                    {
                        AttrBoolEx inheritedValue = (AttrBoolEx)(inline).FetchInheritedRunAttr(key);

                        inline.RunPr_IInline.SetAttr(key, (value == inheritedValue)
                            ? AttrBoolEx.Same
                            : AttrBoolEx.Toggle);
                    }
                }
            }
        }

        private static void ConvertToggles(Paragraph para)
        {
            foreach(int key in gToggleAttrs)
            {
                if (!para.ParagraphBreakRunPr.Contains(key))
                    continue;

                AttrBoolEx value = (AttrBoolEx)para.ParagraphBreakRunPr[key];

                if ((value == AttrBoolEx.True) || (value == AttrBoolEx.False))
                {
                    AttrBoolEx inheritedValue = (AttrBoolEx)((IRunAttrSource)para).FetchInheritedRunAttr(key);

                    para.ParagraphBreakRunPr.SetAttr(key, (value == inheritedValue)
                        ? AttrBoolEx.Same
                        : AttrBoolEx.Toggle);
                }
            }
        }

        private static void ConvertToggles(Document doc)
        {
            HashSetGeneric<Style> alreadyProcessed = new HashSetGeneric<Style>();
            foreach(Style style in doc.Styles)
                ConvertToggles(style, alreadyProcessed);

            foreach (Node node in doc.GetChildNodes(NodeType.Any, true))
            {
                IInline inline = node as IInline;

                if(inline != null)
                    ConvertToggles(inline);
                else if (node.NodeType == NodeType.Paragraph)
                {
                    ConvertToggles((Paragraph)node);
                }
            }
        }

        private static void SortHeadersAndFooters(Document doc)
        {
            foreach (Section section in doc.Sections)
            {
                SortedIntegerListGeneric<HeaderFooter> sortedHeadersFooters = new SortedIntegerListGeneric<HeaderFooter>();
                foreach (HeaderFooter headerFooter in section.HeadersFooters)
                {
                    sortedHeadersFooters.Add(gHeaderSortOrder[(int)headerFooter.HeaderFooterType], headerFooter);
                    headerFooter.Remove();
                }

                for(int i = 0; i < sortedHeadersFooters.Count; i++)
                {
                    HeaderFooter headerFooter = sortedHeadersFooters.GetByIndex(i);
                    section.InsertBefore(headerFooter, section.Body);
                }
            }
        }

        private static void DumpTheme(Theme theme)
        {
            Write("\n[Theme]");
            Write(string.Format("  [Fonts: {0}]", theme.FontSchemeName));
            Write("    [Major]");

            DumpThemeFont(theme.MajorFonts.LatinFontInfo.Name, "HAnsi");
            DumpThemeFont(theme.MajorFonts.EastAsianFontInfo.Name, "EastAsia");
            DumpThemeFont(theme.MajorFonts.ComplexScriptFontInfo.Name, "Bidi");
            DumpSupplementalFonts(theme.MajorFonts.SupplementalFonts);

            Write("    [Minor]");
            DumpThemeFont(theme.MinorFonts.LatinFontInfo.Name, "HAnsi");
            DumpThemeFont(theme.MinorFonts.EastAsianFontInfo.Name, "EastAsia");
            DumpThemeFont(theme.MinorFonts.ComplexScriptFontInfo.Name, "Bidi");
            DumpSupplementalFonts(theme.MinorFonts.SupplementalFonts);

            Write(string.Format("  [Colors: {0}]", theme.Colors.Name));

            foreach(ThemeColor themeColor in gAllThemeColors)
                DumpThemeColor(theme, themeColor);

        }

        private static readonly ThemeColor[] gAllThemeColors = new ThemeColor[]
                {
                    ThemeColor.Accent1, ThemeColor.Accent2, ThemeColor.Accent3, ThemeColor.Accent4, ThemeColor.Accent5, ThemeColor.Accent6,
                    ThemeColor.Background1, ThemeColor.Background2,
                    ThemeColor.Dark1, ThemeColor.Dark2,
                    ThemeColor.FollowedHyperlink, ThemeColor.Hyperlink,
                    ThemeColor.Light1, ThemeColor.Light2,
                    ThemeColor.Text1, ThemeColor.Text2
                };

        private static void DumpThemeColor(Theme theme, ThemeColor colorName)
        {
            DmlColor dmlColor = theme.Colors.GetColor(colorName);
            if (dmlColor == null)
                return;

            if(dmlColor.ColorType == DmlColorType.HexRgbColor)
            {
                DmlHexRgbColor hexRgb = (DmlHexRgbColor)dmlColor;
                Write(string.Format("    {{{0}: '{1}'}}", colorName, hexRgb.Value));
            }
        }

        private static void DumpThemeFont(string fontName, string name)
        {
            Write(string.Format("      {{{0}: '{1}'}}", name, fontName));
        }

        private static void DumpSupplementalFonts(IDictionary<string, ThemeSupplementalFont> table)
        {
            SortedStringListGeneric<object> sortedKeys = new SortedStringListGeneric<object>();
            foreach(string script in table.Keys)
                sortedKeys.Add(script, null);

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < sortedKeys.Count; i++)
            {
                string key = sortedKeys.GetKey(i);
                ThemeSupplementalFont supplementalFont = table.GetValueOrNull(key);
                sb.AppendFormat("{0}='{1}', ", supplementalFont.Script, supplementalFont.Typeface);
            }
            Write(string.Format("      {{Supplemental: {0}}}", sb.ToString().TrimEnd(' ', ',')));
        }

        private static void CollapseExpandableAttrIfNeeded(object childValue, object baseValue)
        {
            if (childValue is IExpandableAttr)
            {
                // Reverse merge the tab stop collection.
                ((IExpandableAttr) childValue).Collapse((IExpandableAttr) baseValue);
            }
        }

        private static void Collapse(AttrCollection baseAttrs, AttrCollection attrs)
        {
            if (baseAttrs == null)
                throw new ArgumentNullException("baseAttrs");

            for (int baseIndex = 0; baseIndex < baseAttrs.Count; baseIndex++)
            {
                int key = baseAttrs.GetKey(baseIndex);

                int childIndex = attrs.IndexOfKey(key);
                if (childIndex >= 0)
                {
                    // The value exists both in the base collection and in the child collection.
                    // We must compare the values to decide what to do.
                    object baseValue = baseAttrs.GetByIndex(baseIndex);
                    object childValue = attrs.GetByIndex(childIndex);
                    if (baseValue.Equals(childValue))
                    {
                        // The value is same in the base collection and in the child collection.
                        // This is a result of expanding, we reverse it by removing the child attribute.
                        attrs.RemoveAt(childIndex);
                    }
                    else
                    {
                        // The value is different in the base collection and in the child collection.
                        // It means a value is directly specified in the child collection.
                        // For an expandable attribute (such as tab stops), need to collapse it.
                        // For other attributes simply do nothing and the child value will be kept.
                        CollapseExpandableAttrIfNeeded(childValue, baseValue);
                    }
                }
            }
        }

        private static void Refine(Document doc)
        {
            if (!DoRefine)
                return;

            // Collapse Table Normal.
            TableStyle tableNormal = doc.Styles.GetBySti(StyleIdentifier.TableNormal, false) as TableStyle;
            foreach (Row row in doc.GetChildNodes(NodeType.Row, true))
            {
                TablePr tablePr = row.TablePr;
                if ((tableNormal != null) && (tablePr.Istd == tableNormal.Istd))
                {
                    Collapse(tableNormal.TablePr, tablePr);
                    if (tablePr.HasFormatRevision && ((TablePr) tablePr.FormatRevision.RevPr).Istd == tableNormal.Istd)
                    {
                        Collapse(tableNormal.TablePr, tablePr.FormatRevision.RevPr);
                    }
                }

                // Remove empty row revisions.
                FormatRevision revision = row.TablePr.FormatRevision;
                if ((revision != null) && (revision.RevPr.Count == 0))
                    row.TablePr.Remove(RevisionAttr.FormatRevision);
            }

            // Remove default ParaAttr.Istd 0x00 && RunAttr.Istd 0x10.
            foreach (Paragraph para in doc.GetChildNodes(NodeType.Paragraph, true))
                if (para.ParaPr.Istd == 0x00)
                    para.ParaPr.Remove(ParaAttr.Istd);

            foreach (Run run in doc.GetChildNodes(NodeType.Run, true))
            {
                if (run.RunPr.Istd == 0x10)
                    run.RunPr.Remove(FontAttr.Istd);

                object runRsid = run.RunPr.GetDirectAttr(FontAttr.RsidR);
                object paraRsid = run.ParentParagraph.ParagraphBreakRunPr.GetDirectAttr(FontAttr.RsidR);

                if (Equals(runRsid, paraRsid))
                    run.RunPr.Remove(FontAttr.RsidR);
            }
        }

        /// <summary>
        /// Prints information about custom parts.
        /// </summary>
        /// <param name="doc"></param>
        private static void DumpPackageCustomParts(Document doc)
        {
            if (doc.PackageCustomParts.Count == 0)
                return;

            Write("");
            Write("[Package Custom Parts]");

            foreach(CustomPart part in doc.PackageCustomParts)
            {
                Write(string.Format("  ['{0}'{1}]", part.Name, part.IsExternal ? ", External": ""));
                Write(string.Format("    {{ParentName: '{0}'}}", part.ParentPartName));
                Write(string.Format("    {{RelationshipType: '{0}'}}", part.RelationshipType));
                Write(string.Format("    {{ContentType: {0}}}", part.ContentType));
                Write(string.Format("    {{Data: {0}}}", ByteArrayToString(part.Data)));
            }
        }

        private static void DumpSignatures(Document doc)
        {
            foreach (DigitalSignature signature in doc.DigitalSignatures)
            {
                Write(string.Format("\n[{0}, Visible: {1}, Valid: {2}]", signature.SignatureType, signature.Visible, signature.IsValid));
                IndentLevel++;
                if (signature.Visible)
                {
                    Write(string.Format("{{SetupId: {0}}}", signature.SetupId));
                    Write(string.Format("{{ImageBytes: {0}}}", ImageBytesToString(signature.ImageBytes)));
                    Write(string.Format("{{ImageBytesValid: {0}}}", ImageBytesToString(signature.ImageBytesValid)));
                    Write(string.Format("{{ImageBytesInvalid: {0}}}", ImageBytesToString(signature.ImageBytesInvalid)));
                }
                if (signature.SignatureType == DigitalSignatureType.XmlDsig)
                {
                    Write(string.Format("{{ReferencesCount: {0}}}", 0 /* TODO signature.ReferenceCollection.Count */));
                }

                if (StringUtil.HasChars(signature.Comments))
                    Write(string.Format("{{Commments: {0}}}", signature.Comments));

                Write(string.Format("{{SignTime: {0}}}", signature.SignTime));

                if (StringUtil.HasChars(signature.Text))
                    Write(string.Format("{{Text: {0}}}", signature.Text));

                IndentLevel--;
            }
        }

        private static string ImageBytesToString(byte[] imageBytes)
        {
            imageBytes = CompressedData.GetData(imageBytes);

            if (imageBytes == null)
                return "null";

            FileFormat imageType = ImageUtil.GetImageType(imageBytes);
            ImageSizeCore imageSize = ImageUtil.GetImageSize(imageBytes);

            return string.Format("{0}, {1}x{2}, {3}", imageType, imageSize.Width, imageSize.Height, ByteArrayToString(imageBytes));
        }

        private static void DumpStatistics(Document doc)
        {
            Write("");
            Write("[Statistics]");

            bool allAttributesSaved = AllAttributes;
            bool noAttributesSaved = NoAttributes;
            AllAttributes = true;
            NoAttributes = false;

            int paraPrCount = 0;
            int paraPrCapacity = 0;
            int paraPrUsed = 0;

            int runPrCount = 0;
            int runPrCapacity = 0;
            int runPrUsed = 0;

            int tablePrCount = 0;
            int tablePrCapacity = 0;
            int tablePrUsed = 0;

            int cellPrCount = 0;
            int cellPrCapacity = 0;
            int cellPrUsed = 0;

            Dictionary<string, int> uniqueParaPr = new Dictionary<string, int>();
            Dictionary<string, int> uniqueRunPr = new Dictionary<string, int>();
            Dictionary<string, int> uniqueTablePr = new Dictionary<string, int>();
            Dictionary<string, int> uniqueCellPr = new Dictionary<string, int>();

            foreach (Paragraph p in doc.GetChildNodes(NodeType.Paragraph, true))
            {
                // Collect paragraph properties.
                paraPrCount++;
                paraPrCapacity += p.ParaPr.Capacity;
                paraPrUsed += p.ParaPr.Count;

                string paraPrText = PrToString(p.ParaPr, ";");
                int oCount = uniqueParaPr.GetValueOrDefault(paraPrText, 0);
                uniqueParaPr[paraPrText] = oCount + 1;

                // Collect paragraph break properties.
                runPrCount++;
                runPrCapacity += p.ParagraphBreakRunPr.Capacity;
                runPrUsed += p.ParagraphBreakRunPr.Count;
                string runPrText = PrToString(p.ParagraphBreakRunPr, ";");
                oCount = uniqueRunPr.GetValueOrDefault(runPrText, 0);
                uniqueRunPr[runPrText] = oCount + 1;

                foreach (Node childNode in p.GetChildNodes(NodeType.Any, false))
                {
                    Inline inline = childNode as Inline;
                    if (inline != null)
                    {
                        // Collect run properties.
                        runPrCount++;
                        runPrCapacity += inline.RunPr.Capacity;
                        runPrUsed += inline.RunPr.Count;
                        runPrText = PrToString(inline.RunPr, ";");
                        oCount = uniqueRunPr.GetValueOrDefault(runPrText, 0);
                        uniqueRunPr[runPrText] = oCount + 1;
                    }
                }
            }

            foreach (Table table in doc.GetChildNodes(NodeType.Table, true))
            {
                foreach (Row row in table.Rows)
                {
                    // Collect row properties.
                    tablePrCount++;
                    tablePrCapacity += row.TablePr.Capacity;
                    tablePrUsed += row.TablePr.Count;

                    string tablePrText = PrToString(row.TablePr, ";");
                    int oCount = uniqueTablePr.GetValueOrDefault(tablePrText, 0);
                    uniqueTablePr[tablePrText] = oCount + 1;

                    foreach (Cell cell in row.Cells)
                    {
                        // Collect cell properties.
                        cellPrCount++;
                        cellPrCapacity += cell.CellPr.Capacity;
                        cellPrUsed += cell.CellPr.Count;

                        string cellPrText = PrToString(cell.CellPr, ";");
                        oCount = uniqueCellPr.GetValueOrDefault(cellPrText, 0);
                        uniqueCellPr[cellPrText] = oCount + 1;
                    }
                }
            }

            DumpPrStatistics("ParaPr: ", paraPrCount, uniqueParaPr.Count, paraPrUsed, paraPrCapacity);
            DumpPrStatistics("RunPr:  ", runPrCount, uniqueRunPr.Count, runPrUsed, runPrCapacity);
            DumpPrStatistics("CellPr: ", cellPrCount, uniqueCellPr.Count, cellPrUsed, cellPrCapacity);
            DumpPrStatistics("TablePr:", tablePrCount, uniqueTablePr.Count, tablePrUsed, tablePrCapacity);

            AllAttributes = allAttributesSaved;
            NoAttributes = noAttributesSaved;
        }

        private static void DumpPrStatistics(string name, int total, int unique, int used, int capacity)
        {
            Write(string.Format("  {{{0} (unique: {1}/{2}/{3}%; usage: {4}/{5}/{6}%)}}",
                name,
                unique, total, total == 0 ? 0 : unique * 100 / total,
                used, capacity, capacity == 0 ? 0 : used * 100 / capacity));
        }

        private static void DumpMailMerge(Document doc)
        {
            const string fmt = "  {{{0}}}";

            MailMergeSettings mms = doc.MailMergeSettings;

            Write("");
            Write("[MailMerge]");

            Write(fmt, string.Format("ActiveRecord: {0}", mms.ActiveRecord));
            Write(fmt, string.Format("AddressFieldName: '{0}'", mms.AddressFieldName));
            Write(fmt, string.Format("CheckErrors: {0}", mms.CheckErrors));
            Write(fmt, string.Format("ConnectString: '{0}'", mms.ConnectString));
            Write(fmt, string.Format("DataSource: '{0}'", mms.DataSource));
            Write(fmt, string.Format("DataType: {0}", mms.DataType));
            Write(fmt, string.Format("Destination: {0}", mms.Destination));

            if (mms.DoNotSupressBlankLines)
                Write(fmt, "DoNotSupressBlankLines");

            Write(fmt, string.Format("HeaderSource: '{0}'", mms.HeaderSource));
            Write(fmt, string.Format("LegacyDataFieldSeparator: {0}", mms.LegacyDataFieldSeparator));
            Write(fmt, string.Format("LegacyDataRowSeparator: {0}", mms.LegacyDataRowSeparator));
            Write(fmt, string.Format("LegacyHeaderFieldSeparator: {0}", mms.LegacyHeaderFieldSeparator));
            Write(fmt, string.Format("LegacyHeaderRowSeparator: {0}", mms.LegacyHeaderRowSeparator));

            if (mms.LinkToQuery)
                Write(fmt, "LinkToQuery");

            if (mms.MailAsAttachment)
                Write(fmt, "MailAsAttachment");

            Write(fmt, string.Format("MailSubject: '{0}'", mms.MailSubject));
            Write(fmt, string.Format("MainDocumentType: {0}", mms.MainDocumentType));

            Write(fmt, string.Format("Query: '{0}'", mms.Query));

            if (mms.ViewMergedData)
                Write(fmt, "ViewMergedData");

            Write(fmt, string.Format("Odso.ColumnDelimiter: '{0}'", mms.Odso.ColumnDelimiter));
            Write(fmt, string.Format("Odso.DataSource: '{0}'", mms.Odso.DataSource));
            Write(fmt, string.Format("DataSourceType: {0}", mms.Odso.DataSourceType));

            // mms.Odso.FieldMapDatas

            if (mms.Odso.FirstRowContainsColumnNames)
                Write(fmt, "Odso.FirstRowContainsColumnNames");

            // mms.Odso.RecipientDatas

            Write(fmt, string.Format("Odso.TableName: '{0}'", mms.Odso.TableName));
            Write(fmt, string.Format("Odso.UdlConnectString: '{0}'", mms.Odso.UdlConnectString));
        }

        private static void DumpFontInfoCollection(Document doc)
        {
            Write("");
            Write("[Fonts]");

            SortedStringListGeneric<FontInfo> sortedFonts = new SortedStringListGeneric<FontInfo>();

            foreach (FontInfo fontInfo in doc.FontInfos)
                sortedFonts.Add(fontInfo.Name, fontInfo);

            foreach (FontInfo fontInfo in sortedFonts.Values)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string altName in fontInfo.GetAltNameList())
                    sb.AppendFormat("'{0}', ", altName);

                string altNames = sb.Length > 0
                    ? string.Format(", ({0})", sb.ToString().Trim(gListSeparatorChars))
                    : string.Empty;

                Write(string.Format("  {{'{0}'{1}, {2} ({3})}}", GetPrintableText(fontInfo.Name), altNames, fontInfo.Family, fontInfo.Charset));

                IndentLevel++;
                Write(string.Format("{{Panose: {0}}}", RawByteArrayToString(fontInfo.Panose)));
                if (fontInfo.Panose != null)
                {
                    for (int i = 0; i < fontInfo.Panose.Length; i++)
                        Write(string.Format("  {{{0}: '{1}'}}", gPanoseStrings[i], GetPanoseValueText(i, fontInfo.Panose[i])));
                }

                Write("{Signature}");
                if (fontInfo.Sig != null)
                {
                    string signature = ArrayUtil.DumpArray(fontInfo.Sig).Replace(" ", "");

                    // CodePage ranges
                    sb = new StringBuilder();
                    sb.Length = 0;
                    for (int i = 0; i < FontSignature.CodePageBits.Length; i++)
                    {
                        int byteNo = 16 + (i / 8);
                        int bitNo = i % 8;

                        if (BitUtil.IsSetByte(fontInfo.Sig[byteNo], (byte)(1 << bitNo)))
                        {
                            if (sb.Length > 0)
                                sb.Append(", ");
                            sb.Append(GetCodePageName(FontSignature.CodePageBits[i]));
                        }
                    }
                    Write(string.Format("  {{CodePages ({0}): {1}}}", signature.Substring(32, 16), sb));
                }

                EmbeddedFont[] embeddedFonts = fontInfo.GetEmbeddedFonts();
                if (embeddedFonts != null)
                {
                    Write("{EmbeddedFonts}");
                    foreach (EmbeddedFont embeddedFont in embeddedFonts)
                    {
                        if (embeddedFont == null)
                        {
                            Write("  {null}");
                        }
                        else
                        {
                            Write(string.Format("  {{{0}, {1}{2}, Data: {3}}}",
                                embeddedFont.Format,
                                embeddedFont.Style,
                                embeddedFont.IsSubsetted ? ", Subsetted" : "",
                                ByteArrayToString(embeddedFont.Data)));
                        }
                    }
                }

                Write("");
                IndentLevel--;
            }
        }

        private static string GetCodePageName(int codepage)
        {
            switch (codepage)
            {
                case 1252: return "ANSI";
                case 1250: return "EastEurope";
                case 1251: return "Cyrillic";
                case 1253: return "Greek";
                case 1254: return "Turkish";
                case 1255: return "Hebrew";
                case 1256: return "Arabic";
                case 1257: return "Baltic";
                case 1258: return "Vietnamese";
                case 874: return "Thai";
                case 932: return "ShiftJIS";
                case 936: return "Chinese Simplified";
                case 949: return "Korean Wansung";
                case 869: return "IBM Greek";
                case 866: return "MS-DOS Russian";
                case 865: return "MS-DOS Nordic";
                case 864: return "Arabic";
                case 863: return "MS-DOS Canadian French";
                case 862: return "Hebrew";
                case 861: return "MS-DOS Icelandic";
                case 860: return "MS-DOS Portuguese";
                case 857: return "IBM Turkish";
                case 855: return "IBM Cyrillic";
                case 852: return "Latin 2";
                case 775: return "MS-DOS Baltic";
                case 737: return "Greek; former 437 G";
                case 708: return "Arabic; ASMO 708";
                case 850: return "WE/Latin 1";
                case 437: return "US";
                case 0: return "Default";
                case 950: return "ChineseBig5";
                case 1361: return "Korean (Johab)";
                case 2: return "Symbol";
                default: throw new InvalidOperationException(string.Format("Codepage: {0}", codepage));
            }
        }

        private static string GetPanoseValueText(int index, int value)
        {
            if (index >= 10)
                return "?";

            if (value == 0) return "Any";
            if(value == 1) return "No fit";
            value -= 2; // -Any -NoFit

            string[] familyStrings = { "Text and Display", "Script", "Decorative", "Pictorial" };
            string[] serifStrings = { "Cove", "Obtuse Cove", "Square Cove", "Obtuse Square Cove", "Square", "Thin", "Bone", "Exaggerated", "Triangle", "Normal Sans", "Obtuse Sans", "Prep Sans", "Flared", "Rounded" };
            string[] weightStrings = { "Very Light", "Light", "Thin", "Book", "Medium", "Demi", "Bold", "Heavy", "Black", "Nord" };
            string[] proportionStrings = { "Old Style", "Modern", "Even Width", "Expanded", "Condensed", "Very Expanded", "Very Condensed", "Monospaced" };
            string[] contrastStrings = { "None", "Very Low", "Low", "Medium Low", "Medium", "Medium High", "High", "Very High" };
            string[] strokeStrings = { "Gradual/Diagonal", "Gradual/Transitional", "Gradual/Vertical", "Gradual/Horizontal", "Rapid/Vertical", "Rapid/Horizontal", "Instant/Vertical" };
            string[] armStrings = { "Straight Arms/Horizontal", "Straight Arms/Wedge", "Straight Arms/Vertical", "Straight Arms/Single-Serif", "Straight Arms/Double-Serif", "Non-Straight Arms/Horizontal", "Non-Straight Arms/Wedge", "Non-Straight Arms/Vertical", "Non-Straight Arms/Single-Serif", "Non-Straight Arms/Double-Serif" };
            string[] letterformStrings = { "Normal/Contact", "Normal/Weighted", "Normal/Boxed", "Normal/Flattened", "Normal/Rounded", "Normal/Off Center", "Normal/Square", "Oblique/Contact", "Oblique/Weighted", "Oblique/Boxed", "Oblique/Flattened", "Oblique/Rounded", "Oblique/Off Center", "Oblique/Square" };
            string[] midlineStrings = { "Standard/Trimmed", "Standard/Pointed", "Standard/Serifed", "High/Trimmed", "High/Pointed", "High/Serifed", "Constant/Trimmed", "Constant/Pointed", "Constant/Serifed", " Low/Trimmed", "Low/Pointed", "Low/Serifed" };
            string[] xheightStrings = { "Constant/Small", "Constant/Standard", "Constant/Large", "Ducking/Small", "Ducking/Standard", "Ducking/Large" };

            string[][] allStrings = {familyStrings, serifStrings, weightStrings, proportionStrings, contrastStrings, strokeStrings, armStrings, letterformStrings, midlineStrings, xheightStrings};

            string[] strings = allStrings[index];
            return (value < strings.Length) ? strings[value] : "?";
        }

        private static void DumpDocPr(Document doc)
        {
            if (NoDocumentProperties)
                return;

            const string fmt = "  {{{0}}}";
            DocPr docPr = doc.DocPr;
            Write("");
            Write("[Properties]");

            if (docPr.RemovePersonalInformation) Write(fmt, "RemovePersonalInformation");
            if (docPr.PrintPostScriptOverText) Write(fmt, "PrintPostScriptOverText");
            if (docPr.PrintFractionalCharacterWidth) Write(fmt, "PrintFractionalCharacterWidth");
            if (docPr.PrintFormsData) Write(fmt, "PrintFormsData");
            if (docPr.EmbedTrueTypeFonts) Write(fmt, "EmbedTrueTypeFonts");
            if (docPr.EmbedSystemFonts) Write(fmt, "EmbedSystemFonts");
            if (docPr.SaveSubsetFonts) Write(fmt, "SaveSubsetFonts");
            if (docPr.SaveFormsData) Write(fmt, "SaveFormsData");
            if (docPr.MultiplePages == MultiplePagesType.MirrorMargins) Write(fmt, "MirrorMargins");
            if (docPr.AlignBordersAndEdges) Write(fmt, "AlignBordersAndEdges");
            if (docPr.BordersDoNotSurroundHeader) Write(fmt, "BordersDoNotSurroundHeader");
            if (docPr.BordersDoNotSurroundFooter) Write(fmt, "BordersDoNotSurroundFooter");
            if (docPr.GutterAtTop) Write(fmt, "GutterAtTop");
            if (docPr.IsGutterSide) Write(fmt, "IsGutterSide");
            Write(fmt, string.Format("ActiveWritingStyle: '{0}', {1}, {2}, {3}, {4}",
                docPr.ActiveWritingStyle.Lang, docPr.ActiveWritingStyle.DllVersion, docPr.ActiveWritingStyle.VendorId,
                                     docPr.ActiveWritingStyle.NlCheck, docPr.ActiveWritingStyle.OptionSet));
            Write(fmt, string.Format("ProofStateSpelling: {0}", docPr.ProofStateSpelling));
            Write(fmt, string.Format("AttachedTemplate: '{0}'", docPr.AttachedTemplate));
            if (docPr.LinkStyles) Write(fmt, "LinkStyles");
            if (docPr.StyleLockQuickFormatSet) Write(fmt, "StyleLockQuickFormatSet");
            if (docPr.StyleLockTheme) Write(fmt, "StyleLockTheme");
            if (docPr.StyleLockQuickFormatSet) Write(fmt, "StyleLockQuickFormatSet");
            Write(fmt, string.Format("StylePaneFormatFilterSettings: {0}", FormatStylePaneFilterSettings(docPr.StylePaneFormatFilterSettings)));
            Write(fmt, string.Format("StylePaneSortMethod: {0}", docPr.StylePaneSortMethod));
            if (docPr.DoNotTrackFormatting) Write(fmt, "DoNotTrackFormatting");
            if (docPr.DoNotTrackMoves) Write(fmt, "DoNotTrackMoves");
            Write(fmt, string.Format("DocumentType: {0}", docPr.DocumentType));
            if (docPr.TrackRevisions) Write(fmt, "TrackRevisions");
            if (docPr.ShowMarkup) Write(fmt, "ShowMarkup");
            if (docPr.ShowAnnotations) Write(fmt, "ShowAnnotations");
            if (docPr.ShowInsertionsDeletions) Write(fmt, "ShowInsertionsDeletions");
            if (docPr.ShowFormatting) Write(fmt, "ShowFormatting");
            if (docPr.ShowInkAnnotations) Write(fmt, "ShowInkAnnotations");
            Write(fmt, string.Format("DocumentProtection: {0}", DocumentProtectionToString(docPr.DocumentProtection)));
            Write(fmt, string.Format("WriteProtection: {0}", WriteProtectionToString(docPr.WriteProtection)));
            Write(fmt, string.Format("DefaultTabStop: {0}", docPr.DefaultTabStop));
            HyphenationOptions hyphenationOptions = docPr.HyphenationOptions;
            if (hyphenationOptions.AutoHyphenation) Write(fmt, "AutoHyphenation");
            Write(fmt, string.Format("ConsecutiveHyphenLimit: {0}", hyphenationOptions.ConsecutiveHyphenLimit));
            Write(fmt, string.Format("HyphenationZone: {0}", hyphenationOptions.HyphenationZone));
            if (!hyphenationOptions.HyphenateCaps) Write(fmt, "DoNotHyphenateCaps");
            if (docPr.ShowEnvelope) Write(fmt, "ShowEnvelope");
            if (docPr.ReadingModeInkLockDown) Write(fmt, "ReadingModeInkLockDown");
            if (docPr.RemoveDateAndTime) Write(fmt, "RemoveDateAndTime");
            Write(fmt, string.Format("ClickTypeParaStyleIstd: 0x{0:x2}", docPr.ClickTypeParaStyleIstd));
            Write(fmt, string.Format("DefaultTableStyleIstd: 0x{0:x2}", docPr.DefaultTableStyleIstd));
            if (docPr.EvenAndOddHeaders) Write(fmt, "EvenAndOddHeaders");
            if (docPr.MultiplePages == MultiplePagesType.BookFoldPrintingReverse) Write(fmt, "BookFoldRevPrinting");
            if (docPr.MultiplePages == MultiplePagesType.BookFoldPrinting) Write(fmt, "BookFoldPrinting");
            Write(fmt, string.Format("BookFoldPrintingSheets: {0}", docPr.BookFoldPrintingSheets));
            Write(fmt, string.Format("DrawingGridHorizontalSpacing: {0}", docPr.DrawingGridHorizontalSpacing));
            Write(fmt, string.Format("DrawingGridVerticalSpacing: {0}", docPr.DrawingGridVerticalSpacing));
            Write(fmt, string.Format("DisplayHorizontalDrawingGridEvery: {0}", docPr.DisplayHorizontalDrawingGridEvery));
            Write(fmt, string.Format("DisplayVerticalDrawingGridEvery: {0}", docPr.DisplayVerticalDrawingGridEvery));
            if (docPr.UseMarginsForDrawingGridOrigin) Write(fmt, "UseMarginsForDrawingGridOrigin");
            Write(fmt, string.Format("DrawingGridHorizontalOrigin: {0}", docPr.DrawingGridHorizontalOrigin));
            Write(fmt, string.Format("DrawingGridVerticalOrigin: {0}", docPr.DrawingGridVerticalOrigin));
            if (docPr.DoNotShadeFormData) Write(fmt, "DoNotShadeFormData");
            if (docPr.PunctuationKerning) Write(fmt, "PunctuationKerning");
            Write(fmt, string.Format("CharacterSpacingType: {0}", docPr.CharacterSpacingType));
            if (docPr.MultiplePages == MultiplePagesType.TwoPagesPerSheet) Write(fmt, "PrintTwoOnOne");
            if (docPr.StrictFirstAndLastChars) Write(fmt, "StrictFirstAndLastChars");
            Write(fmt, string.Format("NoLineBreaksAfter: {0}", docPr.NoLineBreaksAfter));
            Write(fmt, string.Format("NoLineBreaksBefore: {0}", docPr.NoLineBreaksBefore));
            Write(fmt, string.Format("WebPageEncoding: {0}", docPr.WebPageEncoding));
            if (docPr.OptimizeForBrowser) Write(fmt, "OptimizeForBrowser");
            Write(fmt, string.Format("WebTarget: {0}", docPr.WebTarget));
            if (docPr.RelyOnVml) Write(fmt, "RelyOnVml");
            if (docPr.AllowPng) Write(fmt, "AllowPng");
            if (docPr.DoNotRelyOnCss) Write(fmt, "DoNotRelyOnCss");
            if (docPr.DoNotSaveWebPagesAsSingleFile) Write(fmt, "DoNotSaveWebPagesAsSingleFile");
            if (docPr.DoNotOrganizeInFolder) Write(fmt, "DoNotOrganizeInFolder");
            if (docPr.DoNotUseLongFileNames) Write(fmt, "DoNotUseLongFileNames");
            Write(fmt, string.Format("PixelsPerInch: {0}", docPr.PixelsPerInch));
            Write(fmt, string.Format("TargetScreenSize: {0}", docPr.TargetScreenSize));
            if (docPr.ValidateAgainstSchema) Write(fmt, "ValidateAgainstSchema");
            if (docPr.SaveInvalidXml) Write(fmt, "SaveInvalidXml");
            if (docPr.SaveXmlDataOnly) Write(fmt, "SaveXmlDataOnly");
            if (docPr.ShowXmlTags) Write(fmt, "ShowXmlTags");
            if (docPr.IgnoreMixedContent) Write(fmt, "IgnoreMixedContent");
            if (docPr.AlwaysShowPlaceholderText) Write(fmt, "AlwaysShowPlaceholderText");
            if (docPr.DoNotUnderlineInvalidXml) Write(fmt, "DoNotUnderlineInvalidXml");
            if (docPr.UseXsltWhenSaving) Write(fmt, "UseXsltWhenSaving");
            Write(fmt, string.Format("SaveThroughXslt: {0}", docPr.SaveThroughXslt));
            if (docPr.AlwaysMergeEmptyNamespace) Write(fmt, "AlwaysMergeEmptyNamespace");
            if (docPr.DoNotEmbedSmartTags) Write(fmt, "DoNotEmbedSmartTags");
            if (docPr.SaveSmartTagsAsXml) Write(fmt, "SaveSmartTagsAsXml");
            if (docPr.DoNotIncludeSubDocsInStats) Write(fmt, "DoNotIncludeSubDocsInStats");
            if (docPr.NoFitText) Write(fmt, "NoFitText");

            if (!NoRsids)
            {
                Write(fmt, string.Format("RsidRoot: 0x{0:x8}", docPr.RsidRoot));
                IndentLevel++;
                for (int i = 0; i < docPr.Rsids.Count; i++)
                    Write(fmt, string.Format("Rsid: 0x{0:x8}", docPr.Rsids[i]));
                IndentLevel--;
            }

            Write(fmt, string.Format("CodePageText: {0}", docPr.CodePageText));
            if (docPr.FootnotePr.Count > 0)
            {
                Write(fmt, "FootnotePr");
                IndentLevel++;
                DumpPr(docPr.FootnotePr);
                IndentLevel--;
            }

            StringBuilder sb = new StringBuilder();
            if (docPr.ViewOptions.DisplayBackgroundShape)
                sb.Append("DisplayBackgroundShape, ");
            if (docPr.ViewOptions.DoNotDisplayPageBoundaries)
                sb.Append("DoNotDisplayPageBoundaries, ");
            if (docPr.ViewOptions.FormsDesign)
                sb.Append("FormsDesign ");

            Write(fmt, string.Format("ViewOptions: {0}, {1}, {2}, {3}",
                                     docPr.ViewOptions.ViewType, docPr.ViewOptions.ZoomPercent,
                                     docPr.ViewOptions.ZoomType, sb.ToString()).Trim(gListSeparatorChars));

            Write(fmt, string.Format("ThemeFontLanguages: Latin='{0}', EastAsia='{1}', Bidi='{2}'",
                docPr.ThemeFontLanguages.Latin != Language.NoProof ? docPr.ThemeFontLanguages.Latin.ToString() : "",
                docPr.ThemeFontLanguages.EastAsia != Language.NoProof ? docPr.ThemeFontLanguages.EastAsia.ToString() : "",
                docPr.ThemeFontLanguages.Bidi != Language.NoProof ? docPr.ThemeFontLanguages.Bidi.ToString() : ""));

            if (docPr.XmlNamespaces.Count > 0)
            {
                StringBuilder sb2 = new StringBuilder();
                foreach (XmlNamespace xmlNamespace in docPr.XmlNamespaces)
                    sb2.Append(string.Format("('{0}', '{1}', '{2}', '{3}'), ", xmlNamespace.Uri, xmlNamespace.ManifestLocation, xmlNamespace.SchemaLanguage, xmlNamespace.SchemaLocation));
                Write(fmt, string.Format("XmlNamespaces: {0}", sb2.ToString().Trim(gListSeparatorChars)));
            }

            if (docPr.XmlSchemaReferences.Count > 0)
            {
                StringBuilder sb2 = new StringBuilder();
                foreach(XmlSchemaReference xmlSchemaReference in docPr.XmlSchemaReferences)
                    sb2.Append(string.Format("('{0}', '{1}'), ", xmlSchemaReference.Uri, xmlSchemaReference.Location));
                Write(fmt, string.Format("XmlSchemaReferences: {0}", sb2.ToString().Trim(gListSeparatorChars)));
            }

            Write(fmt, string.Format("ShowOutlineLevels: {0}", docPr.ShowOutlineLevels));
            if (docPr.ShowHidden) Write(fmt, "ShowHidden");
            if (docPr.ShowFieldResults) Write(fmt, "ShowFieldResults");
            if (docPr.PrintRevisions) Write(fmt, "PrintRevisions");
            if (docPr.HtmlDoc) Write(fmt, "HtmlDoc");
            if (docPr.WidowControl) Write(fmt, "WidowControl");
            if (docPr.ShowGrid) Write(fmt, "ShowGrid");
            if (docPr.HideLastVersion) Write(fmt, "HideLastVersion");
            if (docPr.HasVersions) Write(fmt, "HasVersions");
            if (docPr.AutoVersion) Write(fmt, "AutoVersion");
        }

        private static string FormatStylePaneFilterSettings(StylePaneFormatFilterSettings settings)
        {
            StringBuilder sb = new StringBuilder();

            if (settings.AllStyles) sb.Append("AllStyles ");
            if (settings.AlternateStyleNames) sb.Append("AlternateStyleNames ");
            if (settings.ClearFormatting) sb.Append("ClearFormatting ");
            if (settings.CustomStyles) sb.Append("CustomStyles ");

            if (settings.DirectFormattingOnNumbering) sb.Append("DirectFormattingOnNumbering ");
            if (settings.DirectFormattingOnParagraphs) sb.Append("DirectFormattingOnParagraphs ");
            if (settings.DirectFormattingOnRuns) sb.Append("DirectFormattingOnRuns ");
            if (settings.DirectFormattingOnTables) sb.Append("DirectFormattingOnTables ");

            if (settings.HeadingStyles) sb.Append("HeadingStyles ");
            if (settings.LatentStyles) sb.Append("LatentStyles ");
            if (settings.NumberingStyles) sb.Append("NumberingStyles ");
            if (settings.StylesInUse) sb.Append("StylesInUse ");
            if (settings.TableStyles) sb.Append("TableStyles ");
            if (settings.Top3HeadingStyles) sb.Append("Top3HeadingStyles ");
            if (settings.VisibleStyles) sb.Append("VisibleStyles ");

            return sb.ToString().Trim();
        }

        private static string DocumentProtectionToString(DocumentProtection docProtection)
        {
            return string.Format("{0}{1}{2}{3}{4}",
                docProtection.Edit,
                docProtection.Enforcement ? ", Enforcement" : "",
                docProtection.Formatting ? ", Formatting" : "",
                docProtection.SelectFormFieldsOnly ? ", SelectFormFieldsOnly" : "",
                docProtection.AutoFormatOverride ? ", AutoFormatOverride" : "");
        }

        private static string WriteProtectionToString(WriteProtection writeProtection)
        {
            if (!writeProtection.IsWriteProtected)
                return "No write protection";

            return string.Format("{0}{1}{2}",
                writeProtection.ReadOnlyRecommended ? "ReadOnly" : "None",
                StringUtil.HasChars(writeProtection.GetPassword())
                    ? string.Format(", Password: '{0}'", writeProtection.GetPassword())
                    : "",
                writeProtection.PasswordHash.Hash != null
                    ? string.Format(", Hash: '{0}'", Convert.ToBase64String(writeProtection.PasswordHash.Hash))
                    : "");
        }

        private static void DumpBackground(Document doc)
        {
            if (doc.BackgroundShape == null)
                return;

            Write("");
            Write("[Background]");
            IndentLevel++;
            DumpNode(doc.BackgroundShape);
            IndentLevel--;
        }

        private static void DumpHtmlBlockCollection(Document doc)
        {
            if (doc.HtmlBlockCollection.Count == 0)
                return;

            Write("");
            DumpHtmlBlock(null, doc);
        }

        private static void DumpHtmlBlock(HtmlBlock htmlBlock, Document doc)
        {
            if (htmlBlock != null)
            {
                Write(string.Format("[{0}, 0x{1:x8}, {2}]", htmlBlock.HtmlBlockType, htmlBlock.Id, htmlBlock.Itap));
                DumpPr(htmlBlock.ParaPr);
                IndentLevel++;
            }

            for (int i = 0; i < doc.HtmlBlockCollection.Count; i++)
            {
                HtmlBlock childHtmlBlock = doc.HtmlBlockCollection.GetHtmlBlockByIndex(i);

                if (childHtmlBlock.ParentId == ((htmlBlock == null) ? 0 : htmlBlock.Id))
                    DumpHtmlBlock(childHtmlBlock, doc);
            }

            if (htmlBlock != null)
            {
                IndentLevel--;
            }
        }

        private static void DumpCompatibility(Document doc)
        {
            if (NoDocumentProperties)
                return;

            const string coFmt = "  {{{0}}}";
            CompatibilityOptions co = doc.CompatibilityOptions;
            Write("");
            Write("[Compatibility]");

            if (co.AdjustLineHeightInTable) Write(coFmt, "AdjustLineHeightInTable");
            if (co.AlignTablesRowByRow) Write(coFmt, "AlignTablesRowByRow");
            if (co.AllowSpaceOfSameStyleInTable) Write(coFmt, "AllowSpaceOfSameStyleInTable");
            if (co.ApplyBreakingRules) Write(coFmt, "ApplyBreakingRules");
            if (co.AutofitToFirstFixedWidthCell) Write(coFmt, "AutofitToFirstFixedWidthCell");
            if (co.AutoSpaceLikeWord95) Write(coFmt, "AutoSpaceLikeWord95");
            if (co.BalanceSingleByteDoubleByteWidth) Write(coFmt, "BalanceSingleByteDoubleByteWidth");
            if (co.CachedColBalance) Write(coFmt, "CachedColBalance");
            if (co.ConvMailMergeEsc) Write(coFmt, "ConvMailMergeEsc");
            if (co.DisplayHangulFixedWidth) Write(coFmt, "DisplayHangulFixedWidth");
            if (co.DoNotAutofitConstrainedTables) Write(coFmt, "DoNotAutofitConstrainedTables");
            if (co.DoNotBreakConstrainedForcedTable) Write(coFmt, "DoNotBreakConstrainedForcedTable");
            if (co.DoNotBreakWrappedTables) Write(coFmt, "DoNotBreakWrappedTables");
            if (co.DoNotExpandShiftReturn) Write(coFmt, "DoNotExpandShiftReturn");
            if (co.DoNotLeaveBackslashAlone) Write(coFmt, "DoNotLeaveBackslashAlone");
            if (co.DoNotSnapToGridInCell) Write(coFmt, "DoNotSnapToGridInCell");
            if (co.DoNotSuppressIndentation) Write(coFmt, "DoNotSuppressIndentation");
            if (co.DoNotSuppressParagraphBorders) Write(coFmt, "DoNotSuppressParagraphBorders");
            if (co.DoNotUseEastAsianBreakRules) Write(coFmt, "DoNotUseEastAsianBreakRules");
            if (co.DoNotUseHTMLParagraphAutoSpacing) Write(coFmt, "DoNotUseHTMLParagraphAutoSpacing");
            if (co.DoNotUseIndentAsNumberingTabStop) Write(coFmt, "DoNotUseIndentAsNumberingTabStop");
            if (co.DoNotVertAlignCellWithSp) Write(coFmt, "DoNotVertAlignCellWithSp");
            if (co.DoNotVertAlignInTxbx) Write(coFmt, "DoNotVertAlignInTxbx");
            if (co.DoNotWrapTextWithPunct) Write(coFmt, "DoNotWrapTextWithPunct");
            if (co.FootnoteLayoutLikeWW8) Write(coFmt, "FootnoteLayoutLikeWW8");
            if (co.ForgetLastTabAlignment) Write(coFmt, "ForgetLastTabAlignment");
            if (co.GrowAutofit) Write(coFmt, "GrowAutofit");
            if (co.LayoutRawTableWidth) Write(coFmt, "LayoutRawTableWidth");
            if (co.LayoutTableRowsApart) Write(coFmt, "LayoutTableRowsApart");
            if (co.LineWrapLikeWord6) Write(coFmt, "LineWrapLikeWord6");
            if (co.MWSmallCaps) Write(coFmt, "MWSmallCaps");
            if (co.NoColumnBalance) Write(coFmt, "NoColumnBalance");
            if (co.NoExtraLineSpacing) Write(coFmt, "NoExtraLineSpacing");
            if (co.NoLeading) Write(coFmt, "NoLeading");
            if (co.NoSpaceRaiseLower) Write(coFmt, "NoSpaceRaiseLower");
            if (co.NoTabHangInd) Write(coFmt, "NoTabHangInd");
            if (co.PrintBodyTextBeforeHeader) Write(coFmt, "PrintBodyTextBeforeHeader");
            if (co.PrintColBlack) Write(coFmt, "PrintColBlack");
            if (co.SelectFldWithFirstOrLastChar) Write(coFmt, "SelectFldWithFirstOrLastChar");
            if (co.ShapeLayoutLikeWW8) Write(coFmt, "ShapeLayoutLikeWW8");
            if (co.ShowBreaksInFrames) Write(coFmt, "ShowBreaksInFrames");
            if (co.SpaceForUL) Write(coFmt, "SpaceForUL");
            if (co.SpacingInWholePoints) Write(coFmt, "SpacingInWholePoints");
            if (co.SplitPgBreakAndParaMark) Write(coFmt, "SplitPgBreakAndParaMark");
            if (co.SubFontBySize) Write(coFmt, "SubFontBySize");
            if (co.SuppressBottomSpacing) Write(coFmt, "SuppressBottomSpacing");
            if (co.SuppressSpacingAtTopOfPage) Write(coFmt, "SuppressSpacingAtTopOfPage");
            if (co.SuppressSpBfAfterPgBrk) Write(coFmt, "SuppressSpBfAfterPgBrk");
            if (co.SuppressTopSpacing) Write(coFmt, "SuppressTopSpacing");
            if (co.SuppressTopSpacingWP) Write(coFmt, "SuppressTopSpacingWP");
            if (co.SwapBordersFacingPgs) Write(coFmt, "SwapBordersFacingPgs");
            if (co.TransparentMetafiles) Write(coFmt, "TransparentMetafiles");
            if (co.TruncateFontHeightsLikeWP6) Write(coFmt, "TruncateFontHeightsLikeWP6");
            if (co.UICompat97To2003) Write(coFmt, "UICompat97To2003");
            if (co.UlTrailSpace) Write(coFmt, "UlTrailSpace");
            if (co.UnderlineTabInNumList) Write(coFmt, "UnderlineTabInNumList");
            if (co.UseAltKinsokuLineBreakRules) Write(coFmt, "UseAltKinsokuLineBreakRules");
            if (co.UseAnsiKerningPairs) Write(coFmt, "UseAnsiKerningPairs");
            if (co.UseFELayout) Write(coFmt, "UseFELayout");
            if (co.UseNormalStyleForList) Write(coFmt, "UseNormalStyleForList");
            if (co.UsePrinterMetrics) Write(coFmt, "UsePrinterMetrics");
            if (co.UseSingleBorderforContiguousCells) Write(coFmt, "UseSingleBorderforContiguousCells");
            if (co.UseWord2002TableStyleRules) Write(coFmt, "UseWord2002TableStyleRules");
            if (co.UseWord97LineBreakRules) Write(coFmt, "UseWord97LineBreakRules");
            if (co.WPJustification) Write(coFmt, "WPJustification");
            if (co.WPSpaceWidth) Write(coFmt, "WPSpaceWidth");
            if (co.WrapTrailSpaces) Write(coFmt, "WrapTrailSpaces");

            foreach(CustomCompatibilitySetting ccs in co.CustomCompatibilitySettings)
                Write(coFmt, string.Format("{0}, '{1}', {2}", ccs.Name, ccs.Value, ccs.Uri));
        }

        private static void DumpWarnings()
        {
            if(NoWarnings)
                return;

            if ((gWarningCallback != null) && (gWarningCallback is UniqueWarningCollection))
            {
                UniqueWarningCollection warnings = (UniqueWarningCollection)gWarningCallback;

                if (warnings.Count > 0)
                {
                    Write("[Warnings]");
                    foreach (WarningInfo warning in warnings)
                        Write(string.Format("  {0}, {1}", warning.Source, warning.Description));
                }
            }
        }

        private static void DumpFootnoteSeparators(FootnoteSeparatorCollection footnoteSeparators)
        {
            foreach(FootnoteSeparator footnoteSeparator in footnoteSeparators)
            {
                Write("");
                Write(string.Format("[{0}]", footnoteSeparator.SeparatorType));
                foreach (Node childNode in footnoteSeparator)
                    DumpNode(childNode);
            }
        }

        private static void DumpVbaProject(Document doc)
        {
            Write("");
            Write("[VBA]");

            Write(string.Format("  {{Name: '{0}'}}", doc.VbaProject.Name));
            Write(string.Format("  {{Description: '{0}'}}", doc.VbaProject.Description));
            Write(string.Format("  {{CodePage: {0}}}", doc.VbaProject.CodePage));
            Write(string.Format("  {{Lcid: {0}}}", doc.VbaProject.Lcid));
            Write(string.Format("  {{LcidInvoke: {0}}}", doc.VbaProject.LcidInvoke));
            Write(string.Format("  {{LibFlags: {0}}}", doc.VbaProject.LibFlags));
            Write(string.Format("  {{SysKind: {0}}}", doc.VbaProject.SysKind));
            Write(string.Format("  {{IsSigned: {0}}}", doc.VbaProject.IsSigned));
            Write(string.Format("  {{VersionMajor: {0}}}", string.Format("0x{0:x8}", doc.VbaProject.VersionMajor)));
            Write(string.Format("  {{VersionMinor: {0}}}", string.Format("0x{0:x8}", doc.VbaProject.VersionMinor)));
            Write(string.Format("  {{Constants: '{0}'}}", doc.VbaProject.Constants));
            Write(string.Format("  {{HelpContext: {0}}}", doc.VbaProject.HelpContext));
            Write(string.Format("  {{HelpFilePath1: '{0}'}}", doc.VbaProject.HelpFilePath1));
            Write(string.Format("  {{HelpFilePath2: '{0}'}}", doc.VbaProject.HelpFilePath2));

            if (doc.VbaProject.MacroNames != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string macroName in doc.VbaProject.MacroNames)
                    sb.AppendFormat("'{0}', ", macroName);
                Write(string.Format("  {{MacroNames: {0}}}", sb.ToString().Trim(gListSeparatorChars)));
            }

            IndentLevel++;
            foreach(VbaModule vbaModule in doc.VbaProject.Modules)
            {
                Write(string.Format("[{0}]", vbaModule.Name));

                Write(string.Format("  {{Type: {0}}}", vbaModule.Type));
                Write(string.Format("  {{HelpContext: {0}}}", vbaModule.HelpContext));
                Write(string.Format("  {{DocString: '{0}'}}", vbaModule.DocString));

                if (vbaModule.FormMemoryStorage != null)
                {
                    FileSystem fs = new FileSystem(vbaModule.FormMemoryStorage);
                    MemoryStream ms = new MemoryStream();
                    fs.Save(ms);

                    Write(string.Format("  {{FormMemoryStorage: '{0}'}}", ByteArrayToString(ms.ToArray())));
                }

                Write(string.Format("  {{SourceCode: '{0}'}}", GetPrintableText(vbaModule.SourceCode)));
            }
            IndentLevel--;
        }

        private static bool IsGoBackBookmarkNode(Node node)
        {
            if(node.NodeType == NodeType.BookmarkStart)
            {
                BookmarkStart bookmarkStart = (BookmarkStart)node;
                if(bookmarkStart.Name == "_GoBack")
                    return true;
            }

            if (node.NodeType == NodeType.BookmarkEnd)
            {
                BookmarkEnd bookmarkEnd = (BookmarkEnd)node;
                if (bookmarkEnd.Name == "_GoBack")
                    return true;
            }

            return false;
        }

        private static bool SkipNode(Node node)
        {
            if (NoGoBackBookmark && IsGoBackBookmarkNode(node))
                return true;

            switch(node.NodeType)
            {
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                    return SkipCrossStructureAnnotations;

                default:
                    return false;
            }
        }

        private static string NodeToString(Node node)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(node.NodeType);
            sb.Append(" ");

            if (!NoNodeId)
            {
                sb.Append(node.GetNodeId());
                sb.Append(" ");
            }

            switch (node.NodeType)
            {
                case NodeType.FieldStart:
                    sb.Append("(" + ((FieldStart)node).GetField().Type + ")");
                    break;

                case NodeType.FieldSeparator:
                    sb.Append("(" + ((FieldSeparator)node).GetField().Type + ")");
                    break;

                case NodeType.FieldEnd:
                    sb.Append(" (" + ((FieldEnd)node).GetField().Type + ")");
                    break;

                case NodeType.Run:
                    sb.Append(GetPrintableText(node.GetText()).Replace(" ", "·"));
                    break;

                case NodeType.BookmarkStart:
                    BookmarkStart bkmkStart = (BookmarkStart)node;
                    sb.AppendFormat("'{0}'{1}", bkmkStart.Name, bkmkStart.DisplacedBy != DisplacedByType.Unspecified ? string.Format(", DisplacedBy:{0}", bkmkStart.DisplacedBy) : "");
                    break;

                case NodeType.BookmarkEnd:
                    BookmarkEnd bkmkEnd = (BookmarkEnd)node;
                    sb.AppendFormat("'{0}'{1}", bkmkEnd.Name, bkmkEnd.DisplacedBy != DisplacedByType.Unspecified ? string.Format(", DisplacedBy:{0}", bkmkEnd.DisplacedBy) : "");
                    break;

                case NodeType.Comment:
                {
                    Comment comment = (Comment)node;
                    sb.AppendFormat("Id:{0}, Parent:{1}, Done:{2}",
                        comment.Id,
                        (comment.ParentId == -1) ? "None" : comment.ParentId.ToString(),
                        comment.Done);
                    break;
                }

                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                {
                    INodeWithAnnotationId commentRangeNode = (INodeWithAnnotationId)node;
                    sb.AppendFormat("Id:{0}", commentRangeNode.IdInternal);
                    break;
                }

                case NodeType.StructuredDocumentTagRangeStart:
                {
                    StructuredDocumentTagRangeStart sdtRangeStart = (StructuredDocumentTagRangeStart)node;
                    sb.AppendFormat("Id:0x{0:x8}", (long)sdtRangeStart.Id);
                    break;
                }

                case NodeType.StructuredDocumentTagRangeEnd:
                {
                    StructuredDocumentTagRangeEnd sdtRangeEnd = (StructuredDocumentTagRangeEnd)node;
                    sb.AppendFormat("Id:0x{0:x8}", (long)sdtRangeEnd.Id);
                    break;
                }

                default:
                    break;
            }

            return sb.ToString();
        }

        private static void DumpNode(Node node)
        {
            if (NoDocumentTree && node.NodeType != NodeType.Document)
                return;

            // Indent sections.
            if ((node.NodeType == NodeType.Section) && (node.Document.NodeType == NodeType.Document))
                Write("");

            if (!SkipNode(node))
                Write(NodeToString(node));

            // FOSS

            if (!NoRevisionGroups && node.Document.NodeType == NodeType.Document)
            {
                RevisionGroupCollection groups = node.FetchDocument().Revisions.Groups;
                for (int i = 0; i < groups.Count; i++)
                {
                    RevisionGroup group = groups[i];

                    if (ReferenceEquals(group.Start, node))
                    {
                        string type = "";
                        if (group.FormatRevision != null)
                            type = "Formatting";
                        else if (group.EditRevision != null)
                            type = group.EditRevision.Type == EditRevisionType.Deletion
                                ? "Deletion"
                                : "Insertion";
                        else if (group.MoveRevision != null)
                            type = group.MoveRevision.Type == MoveRevisionType.MoveFrom
                                ? "MoveFrom"
                                : "MoveTo";

                        Write(string.Format("  {{RevisionGroup: ({0}, {1}, '{2}')}}", i, type, GetPrintableText(groups[i].Text)));
                    }
                }
            }

            switch (node.NodeType)
            {
                case NodeType.Document:
                    {
                        DumpWarnings();

                        Document doc = (Document)node;

                        DumpInternManager(doc);

                        DumpMsoEnvelope(doc);

                        DumpStatistics(doc);

                        if (!NoDocumentProperties)
                        {
                            DumpCompatibility(doc);
                            DumpDocPr(doc);
                            DumpCustomization(doc);
                        }

                        DumpBuiltinDocumentProperties(doc);
                        DumpCustomDocumentProperties(doc);
                        DumpVariables(doc);

                        DumpCustomXmlParts(doc.CustomXmlParts);

                        DumpPackageCustomParts(doc);
                        DumpMailMerge(doc);

                        if (doc.VbaProject != null)
                            DumpVbaProject(doc);

                        DumpFontInfoCollection(doc);
                        DumpHtmlBlockCollection(doc);

                        DumpBackground(doc);
                        DumpSignatures(doc);

                        if (doc.GetThemeInternal() != null)
                            DumpTheme(doc.GetThemeInternal());

                        DumpStyles(doc);
                        DumpLists(doc);

                        DumpFootnoteSeparators(doc.FootnoteSeparators);

                        if (doc.GlossaryDocument != null)
                        {
                            Write("");
                            DumpNode(doc.GlossaryDocument);
                        }

                        break;
                    }
                case NodeType.BookmarkStart:
                    {
                        BookmarkStart bkmkStart = (BookmarkStart)node;

                        if (bkmkStart.IsColumn)
                        {
                            Write(string.Format("  {{First column: '{0}'}}", bkmkStart.FirstColumn));
                            Write(string.Format("  {{Last column: '{0}'}}", bkmkStart.LastColumn));
                        }
                        break;
                    }
                case NodeType.HeaderFooter:
                    {
                        HeaderFooter hf = (HeaderFooter)node;
                        Write(string.Format("  {{Type: '{0}'}}", hf.HeaderFooterType));
                        Write(string.Format("  {{IsLinkedToPrevious: '{0}'}}", hf.IsLinkedToPrevious));
                        break;
                    }
                case NodeType.BuildingBlock:
                    {
                        BuildingBlock block = (BuildingBlock) node;
                        Write(string.Format("  {{Name: '{0}'}}", block.Name));
                        Write(string.Format("  {{Decorated: {0}}}", block.Decorated));
                        Write(string.Format("  {{Guid: {0}}}", block.Guid));
                        Write(string.Format("  {{Description: '{0}'}}", block.Description));
                        Write(string.Format("  {{Gallery: {0}}}", block.Gallery));
                        Write(string.Format("  {{Category: '{0}'}}", block.Category));
                        Write(string.Format("  {{Behavior: {0}}}", block.Behavior));
                        Write(string.Format("  {{Style: '{0}'}}", block.Style));
                        Write(string.Format("  {{Type: {0}}}", block.Type));
                        break;
                    }

                case NodeType.OfficeMath:
                    {
                        OfficeMath officeMath = (OfficeMath)node;

                        Write(string.Format("  {{Type: {0}}}", officeMath.MathObject.MathObjectType));

                        MathObject mathObject = officeMath.MathObject;
                        DumpPr(mathObject);

                        if (!NoRunAttributes)
                            DumpPr(((OfficeMath)node).RunPr);

                        if (mathObject.MathObjectType == MathObjectType.Matrix)
                        {
                            MathObjectMatrix matrix = (MathObjectMatrix) mathObject;
                            for (int i = 0; i < matrix.ColumnPrCollection.Count; i++)
                            {
                                MathMatrixColumnPr columnPr = matrix.ColumnPrCollection[i];
                                Write(string.Format("  {{Column {0}: {1}}}", i, columnPr.HorizontalAlignment));
                            }
                        }
                        break;
                    }

                case NodeType.FieldStart:
                    {
                        FieldStart fieldStart = (FieldStart)node;

                        if(fieldStart.FieldData != null)
                            Write(string.Format("  {{FieldData: {0}}}", ByteArrayToString(fieldStart.FieldData)));

                        if (!NoRunAttributes)
                            DumpPr(fieldStart.RunPr);
                        break;
                    }

                case NodeType.FieldSeparator:
                    {
                        if (!NoRunAttributes)
                            DumpPr(((FieldSeparator)node).RunPr);
                        break;
                    }

                case NodeType.FieldEnd:
                {
                        if (!NoRunAttributes)
                            DumpPr(((FieldEnd)node).RunPr);
                        break;
                    }

                case NodeType.SmartTag:
                    {
                        SmartTag smartTag = (SmartTag)node;
                        foreach(CustomXmlProperty property in smartTag.Properties)
                        {
                            Write(string.Format("  {{Property: {0}:{1}}}", property.Name, property.Value));
                        }
                        break;
                    }
                case NodeType.EditableRangeStart:
                    {
                        EditableRangeStart erStart = (EditableRangeStart)node;
                        Write(string.Format("  {{Id: {0}}}", erStart.Id));
                        Write(string.Format("  {{EditorGroup: '{0}'}}", erStart.EditorGroup));
                        break;
                    }
                case NodeType.EditableRangeEnd:
                    {
                        EditableRangeEnd erEnd = (EditableRangeEnd)node;
                        Write(string.Format("  {{Id: {0}}}", erEnd.Id));
                        break;
                    }
                case NodeType.StructuredDocumentTag:
                {
                    DumpSdt((StructuredDocumentTag) node);
                    break;
                }
                case NodeType.StructuredDocumentTagRangeStart:
                {
                    DumpSdt(((StructuredDocumentTagRangeStart) node).InternalSdt);
                    break;
                }
                case NodeType.Paragraph:
                    {
                        Paragraph para = (Paragraph)node;

                        if (ShowListLabels && (para.IsListItemOriginal || para.IsListItemFinal))
                            Write(string.Format("  {{ListLabel: '{0}' -> '{1}'}}",
                                GetPrintableText(para.ListLabel.LabelStringOriginal),
                                GetPrintableText(para.ListLabel.LabelStringFinal)));

                        if (ShowFloating)
                        {
                            bool direct = para.ParaPr.IsFloating;
                            bool inherited = para.GetExpandedParaPr(ParaPrExpandFlags.Layout).IsFloating;

                            if(inherited && !direct)
                                Write("  {Floating: Inherited}");
                            else if(direct)
                                Write("  {Floating: Direct}");
                        }

                        if (ShowParaId)
                        {
                            if (para.ParaId != 0)
                                Write(string.Format("  {{ParaId: {0}}}", string.Format("0x{0:x8}", para.ParaId)));

                            if (para.TextId != 0)
                                Write(string.Format("  {{TextId: {0}}}", string.Format("0x{0:x8}", para.TextId)));
                        }

                        if (!NoParagraphAttributes)
                        {
                            DumpPr(para.ParaPr, para);

                            if (ShowExpanded)
                            {
                                Write("  >>");
                                DumpPr(para.GetExpandedParaPr(ParaPrExpandFlags.Layout), para);
                            }

                            if (!NoAttributes && HasAttrs(para.ParagraphBreakRunPr, para))
                            {
                                Write(".");
                                DumpPr(para.ParagraphBreakRunPr, para);
                            }
                        }

                        IndentLevel++;
                        if (NoParagraphContent)
                            Write(node.GetText());
                        IndentLevel--;

                        break;
                    }
                case NodeType.Run:
                    if (!NoRunAttributes)
                    {
                        Run run = (Run) node;

                        if (ShowCharacterCategory)
                        {
                            List<CharacterCategory> cats = new List<CharacterCategory>();
                            foreach (char ch in run.Text)
                            {
                                CharacterCategory cat = WordUtil.GetCharacterCategory((int)ch);
                                cats.Add(cat);
                            }

                            StringBuilder sb = new StringBuilder();
                            cats.Sort();
                            foreach (CharacterCategory cat in cats)
                            {
                                if (sb.Length > 0)
                                    sb.Append(", ");
                                sb.Append(cat);
                            }
                            Write(string.Format("  {{Category: {0}}}", sb.ToString()));
                        }

                        if (ShowUnicodeBlocks)
                        {
                            SortedList<UnicodeBlocks, int> blocks = new SortedList<UnicodeBlocks, int>();
                            foreach (char ch in run.Text)
                            {
                                UnicodeBlocks block = UnicodeUtil.GetUnicodeBlock(ch);
                                blocks[block] = 0;
                            }
                            StringBuilder sb = new StringBuilder();
                            foreach (UnicodeBlocks block in blocks.Keys)
                            {
                                if (sb.Length > 0)
                                    sb.Append(", ");
                                sb.Append(block);
                            }
                            Write(string.Format("  {{Unicode: {0}}}", sb.ToString()));
                        }

                        DumpPr(run.RunPr, run);
                    }
                    break;
                case NodeType.Table:
                    DumpPr(((Table) node).TablePr);
                    if(DumpCellerData)
                        DumpCeller((Table)node);
                    break;
                case NodeType.Row:
                    Row row = (Row)node;
                    if (ShowParaId)
                    {
                        if (row.ParaId > 0)
                            Write(string.Format("  {{ParaId: {0}}}", string.Format("0x{0:x8}", row.ParaId)));

                        if (row.TextId > 0)
                            Write(string.Format("  {{TextId: {0}}}", string.Format("0x{0:x8}", row.TextId)));
                    }

                    if (!NoTableAttributes)
                    {
                        int totalWidth = 0;
                        int totalPreferredWidth = 0;
                        bool allFixed = true;
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            Cell cell = row.Cells[i];
                            totalWidth += cell.CellPr.Width;

                            if (cell.CellPr.PreferredWidth.IsFixed)
                                totalPreferredWidth += cell.CellPr.PreferredWidth.ValueTwips;
                            else
                                allFixed = false;
                        }

                        Write(string.Format("  {{TotalWidth: {0}}}", totalWidth));
                        if(allFixed)
                            Write(string.Format("  {{TotalPreferredWidth: {0}}}", totalPreferredWidth));

                        DumpPr(row.TablePr, row);
                    }
                    break;
                case NodeType.Cell:
                {
                    Cell cell = (Cell)node;
                    if (!NoCellAttributes)
                        DumpPr(cell.CellPr, node);

                    if(DumpCellerData)
                        DumpCeller(cell);

                    break;
                }
                case NodeType.Section:
                    if (!NoSectionAttributes)
                        DumpPr(((Section)node).SectPr, node);
                    break;
                case NodeType.Shape:
                case NodeType.GroupShape:
                {
                    ShapeBase shape = (ShapeBase)node;
                    Write(string.Format("  {{Shape.MarkupLanguage: {0}}}", shape.MarkupLanguage));
                    if (!NoShapeAttributes)
                    {
                        DumpShapePr(shape.ShapePr, shape);
                        DumpPr(shape.RunPr, shape);

                    }
                    if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                        DumpDmlNode(shape.DmlNode);

                    if (shape.IsOle)
                        DumpOle(shape);

                    break;
                }
                case NodeType.FormField:
                {
                    FormField formField = (FormField)node;

                    DumpPr(formField.FormFieldPr);

                    if (!NoRunAttributes)
                        DumpPr(formField.RunPr);


                    break;
                }
                case NodeType.Footnote:
                    {
                        Footnote footnote = (Footnote) node;

                        Write(string.Format("  {{{0}, {1}}}",
                                            footnote.FootnoteType,
                                            footnote.IsAuto ? "Auto" : string.Format("Custom: '{0}'", GetPrintableText(footnote.ReferenceMark))));
                        DumpPr(footnote.RunPr);
                        break;
                    }
                case NodeType.SpecialChar:
                {
                    IndentLevel++;
                    Write(GetPrintableText(node.GetText()));
                    IndentLevel--;
                    if (!NoRunAttributes)
                        DumpPr(((SpecialChar)node).RunPr);
                    break;
                }
                case NodeType.MoveFromRangeStart:
                {
                    MoveFromRangeStart moveFrom = (MoveFromRangeStart)node;
                    Write(string.Format("  {{{0}, '{1}', ({2})}}", moveFrom.Id, moveFrom.Name,
                        NoRevisionDetails ? "-" : string.Format("{0} {1}", moveFrom.Author, moveFrom.Date)));
                    break;
                }
                case NodeType.MoveFromRangeEnd:
                {
                    MoveFromRangeEnd moveFrom = (MoveFromRangeEnd)node;
                    Write(string.Format("  {{{0}}}", moveFrom.Id));
                    break;
                }
                case NodeType.MoveToRangeStart:
                {
                    MoveToRangeStart moveTo = (MoveToRangeStart)node;
                    Write(string.Format("  {{{0}, '{1}', ({2})}}", moveTo.Id, moveTo.Name,
                        NoRevisionDetails ? "-" : string.Format("{0} {1}", moveTo.Author, moveTo.Date)));
                    break;
                }
                case NodeType.MoveToRangeEnd:
                {
                    MoveToRangeEnd moveTo = (MoveToRangeEnd)node;
                    Write(string.Format("  {{{0}}}", moveTo.Id));
                    break;
                }
                case NodeType.SubDocument:
                {
                    SubDocument subDoc = (SubDocument)node;
                    Write(string.Format("  {{{0}}}", subDoc.FileName));
                    break;
                }
                case NodeType.System:
                {
                    if (node is SdtMarkerStart)
                    {
                        SdtMarkerStart start = (SdtMarkerStart)node;
                        Write(string.Format("  {{{0};{1}}}", start.InternalSdt.Id, start.InternalSdt.Level));
                    }

                    if (node is SdtMarkerEnd)
                    {
                        SdtMarkerEnd end = (SdtMarkerEnd)node;
                        Write(string.Format("  {{{0}}}", end.Id));
                    }

                    break;
                }
                default:
                    // No additional data from other nodes is required.
                    break;
            }

            if (node is CompositeNode)
            {
                if (!((node.NodeType == NodeType.Paragraph) && NoParagraphContent))
                {
                    foreach (Node child in ((CompositeNode) node).GetChildNodes(NodeType.Any, false))
                    {
                        if (NoTableBody && (node.NodeType == NodeType.Table))
                            continue;

                        // Don't indent after Document node.
                        if (node.NodeType != NodeType.Document)
                            IndentLevel++;

                        DumpNode(child);

                        if (node.NodeType != NodeType.Document)
                            IndentLevel--;
                    }
                }
            }
        }

        private static void DumpSdt(StructuredDocumentTag sdt)
        {
            if (NoAttributes)
                return;

            Write(string.Format("  {{Level: {0}}}", sdt.Level));
            Write(string.Format("  {{Placeholder: {0}}}", sdt.Placeholder));
            Write(string.Format("  {{PlaceholderName: '{0}'}}", sdt.PlaceholderName));
            Write(string.Format("  {{IsShowingPlaceholderText: {0}}}", sdt.IsShowingPlaceholderText));
            Write(string.Format("  {{Type: {0}}}", sdt.SdtType));
            Write(string.Format("  {{Id: {0}}}", sdt.Id));
            Write(string.Format("  {{Tag: '{0}'}}", sdt.Tag));
            Write(string.Format("  {{Title: {0}}}", sdt.Title));
            Write(string.Format("  {{IsDocxExtension: {0}}}", sdt.IsDocxExtension.ToString()));

            if (sdt.SdtType == SdtType.Checkbox)
            {
                Write(string.Format("  {{Checked: {0}}}", sdt.Checked));
                Write(string.Format("  {{ContentChecked: {0}}}", sdt.ContentChecked));
            }

            if (sdt.SdtType == SdtType.DropDownList)
            {
                IndentLevel++;
                foreach (SdtListItem item in sdt.ListItems)
                    Write(string.Format("  {{'{0}': '{1}'}}", item.Value, item.DisplayText));

                IndentLevel--;
            }

            if (!sdt.XmlMapping.IsEmpty)
                DumpXmlMapping(sdt.XmlMapping);
            IndentLevel++;
            DumpPr(sdt.ContentsRunPr);
            Write(".");
            DumpPr(sdt.EndCharacterRunPr);
            IndentLevel--;
        }

        private static void DumpCeller(Table table)
        {
            try
            {
                CellerTable celler = new CellerTable(table);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < celler.ColumnCount; i++)
                    sb.Append(celler.GetColumnWidth(i) + " ");

                Write(string.Format("  {{Celler: ({0})}}", sb.ToString().Trim()));

            }
            catch (Exception e)
            {
                Write(string.Format("  {{Celler: ({0})}}", e.Message));
            }
        }

        private static void DumpCeller(Cell cell)
        {
            CellerTable celler = new CellerTable(cell.ParentTable, CellerTableOptions.GotoMergedCells);

            for(int i = 0; i < cell.RowIndex; i++)
                celler.GotoNextRow();

            celler.GotoFirstCell();

            for (int col = 0; col < celler.ColumnCount; col++)
            {
                if (ReferenceEquals(cell, celler.CurCell.CellNode))
                    Write(string.Format("  {{Celler: ({2}: c{0}, r{1})}}", celler.CurCell.ColSpan, celler.CurCell.RowSpan, col));

                celler.GotoNextCell();
            }
        }

        private static string DmlAdjustableCoordinateToString(DmlAdjustableCoordinate coord)
        {
            return string.Format("{0}", coord.String);
        }

        private static string DmlAdjustableAngleToString(DmlAdjustableAngle angle)
        {
            return string.Format("{0}", angle.String);
        }

        private static string DmlAdjustablePointToString(DmlAdjustablePoint point)
        {
            return string.Format("X:{0}, Y:{1}", DmlAdjustableCoordinateToString(point.X), DmlAdjustableCoordinateToString(point.Y));
        }

        private static void DumpInternManager(DocumentBase doc)
        {
            if (doc.InternManager == null)
                return;

            InternManager intern = doc.InternManager;

            Write("\n[InternManager]");

            Write(string.Format("  {{Called: {0}}}", intern.CalledCount));
            Write(string.Format("  {{Empty: {0}}}", intern.EmptyCount));
            Write(string.Format("  {{Repeated: {0}}}", intern.RepeatedCount));
            Write(string.Format("  {{Complex: {0}}}", intern.ComplexCount));
            Write(string.Format("  {{Interned: {0}}}", intern.InternedCount));
            Write(string.Format("  {{Pooled: {0}}}", intern.PooledCount));
            Write(string.Format("  {{Uninterned: {0}}}", intern.UnInternedCount));
            Write(string.Format("  {{Unpooled: {0}}}", intern.UnPooledCount));

            Write(string.Format("  {{Pool: (size: {0})}}", intern.Pool.Count));

            SortedIntegerListGeneric<InternPoolItem> sortedPool = new SortedIntegerListGeneric<InternPoolItem>();
            foreach (InternPoolItem item in intern.Pool.Values)
                sortedPool.Add(item.Id, item);

            IndentLevel++;
            for (int i = 0; i < sortedPool.Count; i++)
            {
                InternPoolItem item = sortedPool.GetByIndex(i);

                Write(string.Format("{{0x{0:x4}:{1}}}", item.Id, item.RefCount));

                bool savedAllAttributes = AllAttributes;
                bool savedShowIntern = ShowIntern;
                AllAttributes = true;
                DumpPr(item.Pr);
                AllAttributes = savedAllAttributes;
                ShowIntern = savedShowIntern;
            }
            IndentLevel--;
        }

        private static void DumpDmlChartSpace(DmlChartSpace chartSpace)
        {
            StringBuilder sb = new StringBuilder();
            if (chartSpace.AspectRatioLocked) sb.Append("AspectRatioLocked, ");
            if (chartSpace.AutoUpdate) sb.Append("AutoUpdate, ");
            if (chartSpace.Date1904) sb.Append("Date1904, ");
            if (chartSpace.Hidden) sb.Append("Hidden, ");
            if (chartSpace.IsChartEx) sb.Append("IsChartEx, ");
            if (chartSpace.IsWord2007OrLower) sb.Append("IsWord2007OrLower, ");
            if (chartSpace.RoundedCorners) sb.Append("RoundedCorners, ");
            if (chartSpace.UseWord2010Style) sb.Append("UseWord2010Style, ");
            Write("{{DmlChartSpace: {0}}}", sb.ToString().Trim(gListSeparatorChars));

            IndentLevel++;
            DumpDmlChartFormat(chartSpace.ChartFormat);
            IndentLevel--;
        }

        private static void DumpDmlChartFormat(DmlChartFormat chartFormat)
        {
            StringBuilder sb = new StringBuilder();
            if (chartFormat.AutoTitleDeleted) sb.Append("AutoTitleDeleted, ");
            if (chartFormat.PlotVisOnly) sb.Append("PlotVisOnly, ");
            if (chartFormat.ShowDLblsOverMax) sb.Append("ShowDLblsOverMax, ");
            if (chartFormat.TitleDeleted) sb.Append("TitleDeleted, ");
            Write("{{DmlChartFormat: {0}}}", sb.ToString().Trim(gListSeparatorChars));

            IndentLevel++;
            DumpPlotArea(chartFormat.PlotArea);

            DmlChartTitle chartTitle = chartFormat.DCTitle;
            if (chartTitle != null)
            {
                Write("{ChartTitle}");
                Write("  {{Overlay: {0}}}", chartTitle.Overlay);
                Write("  {{Show: {0}}}", chartTitle.Show);
                Write("  {{Text: '{0}'}}", chartTitle.Text);

                if (chartTitle.Tx != null)
                {
                    Write("  {DmlChartTx}");
                    DmlChartTx chartTx = chartTitle.Tx;
                    IndentLevel++;
                    Write("{{TxType: {0}}}", chartTx.TxType);
                    Write("{{PlainText: '{0}'}}", chartTx.PlainText);
                    if (chartTx.Formula != null)
                        Write("{{Formula: '{0}'}}", chartTx.Formula);
                    Write("{{ReachText: '{0}'}}", chartTx.RichText);
                    if (chartTx.StrRef != null)
                        Write("{{StrRef: '{0}'}}", chartTx.StrRef);
                    IndentLevel--;
                }

                if (chartTitle.TxPr != null)
                {
                    Write("  {DmlChartTxPr}");
                    DmlChartTxPr chartTxPr = chartTitle.TxPr;
                    IndentLevel++;
                    DumpDmlRunProperties(chartTxPr.RunPr);
                    IndentLevel--;
                }
            }
            IndentLevel--;
        }

        private static void DumpPlotArea(DmlChartPlotArea plotArea)
        {
            StringBuilder sb = new StringBuilder();
            if (plotArea.HasAxis) sb.Append("HasAxis, ");
            if (plotArea.IsInner) sb.Append("IsInner, ");
            if (plotArea.IsSurface3D) sb.Append("HaIsSurface3DsAxis, ");
            if (plotArea.RenderLegendForDataPoints) sb.Append("RenderLegendForDataPoints, ");
            Write("{{DmlChartPlotArea: {0}}}", sb.ToString().Trim(gListSeparatorChars));

            foreach (DmlChart dmlChart in plotArea.Charts)
            {
                IndentLevel++;
                DumpDmlChart(dmlChart);
                IndentLevel--;
            }
        }

        private static void DumpDmlChart(DmlChart dmlChart)
        {
            StringBuilder sb = new StringBuilder();
            if (dmlChart.Is3D) sb.Append("Is3D, ");
            if ((dmlChart is DmlBubbleChart) && ((DmlBubbleChart)dmlChart).Bubble3D) sb.Append("IsBubble3D, ");
            if (dmlChart.IsRadarChart) sb.Append("IsRadarChart, ");
            if (dmlChart.HasAxis) sb.Append("HasAxis, ");
            if (dmlChart.RenderLegendForDataPoints) sb.Append("RenderLegendForDataPoints, ");
            if (dmlChart.VaryColors) sb.Append("VaryColors, ");
            Write(string.Format("{{DmlChart: {0}, {1}}}", dmlChart.ChartType, sb.ToString().Trim(gListSeparatorChars)));

            IndentLevel++;
            ChartDataLabelCollection dataLabels = dmlChart.DataLabels;
            sb = new StringBuilder();
            if (dataLabels.ShowCategoryName) sb.Append("ShowCategoryName, ");
            if (dataLabels.ShowBubbleSize) sb.Append("ShowBubbleSize, ");
            if (dataLabels.ShowDataLabelsRange) sb.Append("ShowDataLabelsRange, ");
            if (dataLabels.ShowLeaderLines) sb.Append("ShowLeaderLines, ");
            if (dataLabels.ShowLegendKey) sb.Append("ShowLegendKey, ");
            if (dataLabels.ShowPercentage) sb.Append("ShowPercentage, ");
            if (dataLabels.ShowSeriesName) sb.Append("ShowSeriesName, ");
            if (dataLabels.ShowValue) sb.Append("ShowValue, ");
            Write("{{ChartDataLabelCollection: {0}}}", sb.ToString().Trim(gListSeparatorChars));

            IndentLevel++;
            foreach (ChartDataLabel dataLabel in dataLabels)
            {
                sb = new StringBuilder();
                if (dataLabel.ShowCategoryName) sb.Append("ShowCategoryName, ");
                if (dataLabel.ShowBubbleSize) sb.Append("ShowBubbleSize, ");
                if (dataLabel.ShowDataLabelsRange) sb.Append("ShowDataLabelsRange, ");
                if (dataLabel.ShowLeaderLines) sb.Append("ShowLeaderLines, ");
                if (dataLabel.ShowLegendKey) sb.Append("ShowLegendKey, ");
                if (dataLabel.ShowPercentage) sb.Append("ShowPercentage, ");
                if (dataLabel.ShowSeriesName) sb.Append("ShowSeriesName, ");
                if (dataLabel.ShowValue) sb.Append("ShowValue, ");
                Write("{{ChartDataLabel: {0}}}", sb.ToString().Trim(gListSeparatorChars));
            }
            IndentLevel--;

            foreach (ChartSeries series in dmlChart.Series)
            {
                sb = new StringBuilder();
                if(series.HasPoints) sb.Append("HasValues, ");
                if(series.Hidden) sb.Append("Hidden, ");
                if(series.InvertIfNegative) sb.Append("InvertIfNegative, ");
                if(series.Bubble3D) sb.Append("Bubble3D, ");
                if(series.Smooth) sb.Append("Smooth, ");
                if(series.SmoothExplicitlySet) sb.Append("SmoothExplicitlySet, ");
                Write("{{ChartSeries: {0}}}", sb.ToString().Trim(gListSeparatorChars));

                IndentLevel++;
                DumpDmlChartDataSource(series.X, "XValues");
                DumpDmlChartDataSource(series.Y, "YValues");
                IndentLevel--;
            }

            IndentLevel--;
        }

        private static void DumpDmlChartDataSource(DmlChartDimensionData dimensionData, string name)
        {
            StringBuilder sb = new StringBuilder();
            if(dimensionData.IsEmpty) sb.Append("HasValues, ");
            if(dimensionData.IsDate) sb.Append("IsDate, ");
            Write(string.Format("{{DmlChartDimensionData.{0}: {1}}}", name, sb.ToString().Trim(gListSeparatorChars)));

            if (dimensionData.Data != null)
            {
                for (int i = 0; i < dimensionData.Data.ValueCount; i++)
                {
                    IndentLevel++;
                    DmlChartValue dmlChartValue = dimensionData.GetValue(i);

                    if (dmlChartValue != null)
                    {
                        sb = new StringBuilder();
                        if (dmlChartValue.IsVisible) sb.Append("IsVisible, ");
                        if (dmlChartValue.IsDate) sb.Append("IsDate, ");
                        if (dmlChartValue.IsNaN) sb.Append("IsNaN, ");
                        Write(
                            string.Format(
                                "{{DmlChartValue: {0}; {1}; {2}}}",
                                dmlChartValue.Value,
                                dmlChartValue.ValueType,
                                sb.ToString().Trim(gListSeparatorChars)));
                    }

                    IndentLevel--;
                }
            }
        }

        private static void DumpDmlRunProperties(DmlRunProperties runPr)
        {
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.AlternativeLanguage);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Bold);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Baseline);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.BookmarkLinkTarget);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Capitalization);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.IsDirty);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.HasSpellingError);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Italics);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Kerning);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Kumimoji);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Language);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.NoProofing);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.NormalizeHeights);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.SmartTagsClean);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.SmartTagID);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Spacing);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Strikethrough);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.FontSize);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Underline);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Fill);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Outline);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.HighlightColor);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.LatinFont);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.EastAsianFont);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.SymbolFont);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.ComplexScriptFont);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.HlinkClick);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.HlinkHover);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Rtl);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.UnderlineFill);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.UnderlineFillTx);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.UnderlineStroke);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.UnderlineStrokeTx);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.RightToLeftFlowDirection);
            DumpDmlRunProperty(runPr, DmlRunPropertiesIds.Extensions);
        }

        private static void DumpDmlRunProperty(DmlRunProperties runPr, DmlRunPropertiesIds key)
        {
            if (!runPr.IsPropertySpecified(key))
                return;

            switch (key)
            {
                case DmlRunPropertiesIds.LatinFont:
                case DmlRunPropertiesIds.EastAsianFont:
                case DmlRunPropertiesIds.ComplexScriptFont:
                case DmlRunPropertiesIds.SymbolFont:
                    DmlFont dmlFont = (DmlFont)runPr.GetDirectProperty(key);
                    Write(string.Format("{{{0}: {1}}}", key, DmlFontToString(dmlFont)));
                    break;

                case DmlRunPropertiesIds.Kerning:
                case DmlRunPropertiesIds.FontSize:
                case DmlRunPropertiesIds.Spacing:
                    DmlTextPoints dmlTextPoints = (DmlTextPoints)runPr.GetDirectProperty(key);
                    Write(string.Format("{{{0}: {1}}}", key, dmlTextPoints.ValueInPoints));
                    break;

                default:
                    object val = runPr.GetDirectProperty(key);
                    Write(string.Format("{{{0}: {1}}}", key, val));
                    break;
            }
        }

        private static string DmlFontToString(DmlFont dmlFont)
        {
            return string.Format("('{0}', Panose: '{1}', Charset: {2}, SimilarFamily: {3})",
                dmlFont.TextTypeface, dmlFont.PanoseSetting, dmlFont.SimilarCharacterSet, dmlFont.SimilarFontFamily);
        }

        private static void DumpDmlNode(DmlNode dmlNode)
        {
            if (!ShowDmlNode)
                return;

            IndentLevel++;
            Write("{DmlNode}");
            Write(string.Format("  {{Type: {0}}}", dmlNode.DmlNodeType));

            DmlTransform transform = dmlNode.Transform;
            Write(string.Format("  {{Transform: {0}, {1}}}", transform.X, transform.Y));

            IndentLevel++;
            switch (dmlNode.DmlNodeType)
            {
                case DmlNodeType.Chart:
                    DumpDmlChartSpace((DmlChartSpace)dmlNode);
                    break;

                case DmlNodeType.Picture:
                    DmlPicture dmlPicture = (DmlPicture) dmlNode;
                    Write(string.Format("{{Id: {0}}}", dmlPicture.NonVisualPr.NvDrawingProperties.Id));
                    Write(string.Format("{{Name: {0}}}", dmlPicture.NonVisualPr.NvDrawingProperties.Name));
                    Write(string.Format("{{BlipFill.ImageBytes: {0}}}", ImageBytesToString(dmlPicture.BlipFill.ImageBytes)));
                    break;

                case DmlNodeType.Diagram:
                {
                    DmlDiagram dmlDiagram = (DmlDiagram)dmlNode;
                    Write(string.Format("{{Title: {0}}}", dmlDiagram.Title));
                    Write(string.Format("{{Name: {0}}}", dmlDiagram.Name));
                    DmlOutline outline = dmlDiagram.Outline;
                    Write("{Outline}");
                    Write(string.Format("  {{DashType: {0}}}", outline.Dash.DashType));
                    Write(string.Format("  {{DashStyle: {0}}}", outline.DashStyle));
                    break;
                }

                case DmlNodeType.WordprocessingShape:
                {
                    DmlShape dmlShape = (DmlShape)dmlNode;

                    Write(string.Format("{{TextWrappingType: {0}}}", dmlShape.TextBodyPr.TextWrappingType));

                    DmlFill fill = dmlShape.Fill;
                    Write(string.Format("{{Fill: {0}/{1}}}", fill.FillType, fill.DmlFillType));
                    if (dmlShape.Fill.DmlFillType == DmlFillType.BlipFill)
                    {
                        Write(string.Format("  {{Blip.EmbedImage: {0}}}", ImageBytesToString(((DmlBlipFill)fill).Blip.EmbedImage)));
                    }

                    StringToObjDictionary<DmlExtension> spPrExt = dmlShape.Extensions;
                    if (spPrExt != null)
                        Write(string.Format("{{SpPrExtensions: {0}}}", spPrExt.Count));

                    if (dmlShape.Effects != null)
                    {
                        Write(string.Format("{{DmlShapeEffects: {0}}}", dmlShape.Effects.Count));
                        IndentLevel++;
                        foreach (DmlShapeEffect effect in dmlShape.Effects)
                        {
                            Write(string.Format("{{Type: {0}}}", effect.EffectType.ToString()));
                            IndentLevel++;
                            Write(DmlShapeEffectToString(effect));
                            IndentLevel--;
                        }
                        IndentLevel--;
                    }
                    DmlGeometry geometry = dmlShape.Geometry;
                    Write(string.Format("{{Geometry.Paths: {0}}}", geometry.Paths.Count));
                    foreach (DmlPath path in geometry.Paths)
                    {
                        foreach (IDmlPathPart pathPart in path.PathParts)
                        {
                            switch (pathPart.PathPartType)
                            {
                                case DmlPathPartType.ArcTo:
                                    DmlArcTo arcTo = (DmlArcTo)pathPart;
                                    Write(string.Format(
                                        "  {{ArcTo (WidthRadius: {0}, HeightRadius: {1}, StartAngle: {2}, SwingAngle: {3})}}",
                                        DmlAdjustableCoordinateToString(arcTo.WidthRadius),
                                        DmlAdjustableCoordinateToString(arcTo.HeightRadius),
                                        DmlAdjustableAngleToString(arcTo.StartAngle),
                                        DmlAdjustableAngleToString(arcTo.SwingAngle)));
                                    break;

                                case DmlPathPartType.Close:
                                    Write("  {Close}");
                                    break;

                                case DmlPathPartType.MoveTo:
                                    DmlMoveTo moveTo = (DmlMoveTo)pathPart;
                                    Write(string.Format("  {{MoveTo ({0})}}", DmlAdjustablePointToString(moveTo.Point)));
                                    break;

                                case DmlPathPartType.LineTo:
                                    DmlLineTo lineTo = (DmlLineTo)pathPart;
                                    Write(string.Format("  {{LineTo ({0})}}", DmlAdjustablePointToString(lineTo.Point)));
                                    break;

                                case DmlPathPartType.QuadraticBezierTo:
                                    DmlQuadraticBezierTo bezier = (DmlQuadraticBezierTo)pathPart;
                                    Write(string.Format("  {{QuadraticBezierTo (ControlPoint: ({0}), EndPoint: ({1}))}}",
                                        DmlAdjustablePointToString(bezier.ControlPoint),
                                        DmlAdjustablePointToString(bezier.EndPoint)));
                                    break;

                                default:
                                    Write(string.Format("  {{{0}}}", pathPart.PathPartType));
                                    break;
                            }
                        }
                    }

                    IList<DmlAdjustHandle> adjustHandles = dmlShape.Geometry.AdjustHandles;
                    Write(string.Format("  {{Geometry.AdjustHandles: {0}}}", adjustHandles.Count));
                    break;
                }

                default:
                    break;
            }
            IndentLevel--;
            IndentLevel--;
        }

        private static void DumpCustomXmlParts(CustomXmlPartCollection customXmlParts)
        {
            // Dump sorted.
            List<CustomXmlPart> sortedList = new List<CustomXmlPart>();
            foreach (CustomXmlPart xmlPart in customXmlParts)
                sortedList.Add(xmlPart);

            sortedList.Sort(new CustomXmlPartComparer());

            foreach(CustomXmlPart xmlPart in sortedList)
            {
                Write(string.Format("\n[CustomXmlPart: {0}]", xmlPart.Id));

                IndentLevel++;
                foreach(string schema in xmlPart.Schemas)
                    if(StringUtil.HasChars(schema))
                        Write(string.Format("{{Schema: '{0}'}}", schema));

                Write(string.Format("{{Data: {0}}}", ByteArrayToString(xmlPart.Data)));

                if (DumpCustomXmlPartDocuments)
                {
                    try
                    {
                        XmlDocument xmlDoc = Aspose.Xml.XmlUtilPal.LoadXml(new MemoryStream(xmlPart.Data), false);

                        string inner = xmlDoc.FirstChild.InnerText;

                        byte[] innerBytes = Encoding.UTF8.GetBytes(inner);
                        FileFormatDetector detector = new FileFormatDetector();
                        FileFormatInfo fi = detector.Detect(new MemoryStream(innerBytes));
                        if (fi.LoadFormat != LoadFormat.Unknown)
                        {
                            Write(string.Format("{{FileFormat: {0}}}", fi.LoadFormat));

                            Document innerDoc = new Document(new MemoryStream(innerBytes));

                            string savedFileName = FileName;
                            TextWriter savedWriter = Writer;

                            string partName = FileName;
                            if (partName.EndsWith(".model"))
                                partName = partName.Substring(0, partName.Length - 6);

                            Save(innerDoc, string.Format("{0}.{1}.model", partName, xmlPart.Id));
                            FileName = savedFileName;
                            Writer = savedWriter;

                            FileStream f = File.Create(string.Format("{0}.{1}.xml", partName, xmlPart.Id));
                            f.Write(innerBytes, 0, innerBytes.Length);
                            f.Close();
                        }
                    }
                    catch
                    {
                    }
                }


                IndentLevel--;
            }
        }

        private class CustomXmlPartComparer : IComparer<CustomXmlPart>
        {
            int IComparer<CustomXmlPart>.Compare(CustomXmlPart x, CustomXmlPart y)
            {
                return StringOrdinalComparer.Default.Compare(x.Id, y.Id);
            }
        }

        private static void DumpXmlMapping(XmlMapping xmlMapping)
        {
            Write("  {{XmlMapping.IsDocx15Extension: '{0}'}}", xmlMapping.IsDocx15Extension.ToString());
            Write("  {{XmlMapping.StoreItemId: '{0}'}}", xmlMapping.StoreItemId.ToLower());
            Write("  {{XmlMapping.XPath: '{0}'}}", xmlMapping.XPath);
            Write("  {{XmlMapping.StoreItemChecksum: '{0}'}}", xmlMapping.StoreItemChecksum);
        }

        private static void DumpOle(ShapeBase shape)
        {
            IEmbeddedObject embeddedObject = (IEmbeddedObject)shape.ShapePr[ShapeAttr.OleObject];
            if (embeddedObject is Forms2OleControl)
                DumpForms2OleControl((Forms2OleControl)embeddedObject);
            else if (embeddedObject is HtmlOleControl)
                DumpHtmlOleControl((HtmlOleControl)embeddedObject);
            else if (embeddedObject is OleControl)
                DumpOleControl((OleControl)embeddedObject);
            else if (embeddedObject is OleObject)
                DumpOleObject((OleObject)embeddedObject);
            else if (embeddedObject is OoxmlObject)
                DumpOoxmlObject((OoxmlObject)embeddedObject);
            else
                Write("{EmbeddedObject.Null}");
        }

        private static void DumpOleControl(OleControl oleControl)
        {
            Write("{EmbeddedObject.OleControl}");
            DumpEmbeddedObject(oleControl);
        }

        private static void DumpOleObject(OleObject oleObject)
        {
            Write("{EmbeddedObject.OleObject}");

            DumpEmbeddedObject(oleObject);

            MemoryStorage data = oleObject.Data;
            IndentLevel++;
            for (int i = 0; i < data.Count; i++)
            {
                object child = data.GetByIndex(i);
                if (child is MemoryStream)
                {
                    string streamName = data.GetKey(i);
                    MemoryStream stream = (MemoryStream)child;
                    Write(
                        string.Format(
                            "{{'{0}': {1}}}",
                            GetPrintableText(streamName),
                            ByteArrayToString(stream.ToArray())));
                    IndentLevel++;
                    DumpOleStream(streamName, data);
                    IndentLevel--;
                }
            }
            IndentLevel--;
        }

        private static void DumpOoxmlObject(OoxmlObject ooxmlObject)
        {
            Write("{EmbeddedObject.OoXmlObject}");

            DumpEmbeddedObject(ooxmlObject);

            IndentLevel++;
            Write(string.Format("{{ContentType: {0}}}", ooxmlObject.ContentType));
            Write(string.Format("{{Data: {0}}}", ByteArrayToString(((MemoryStream)ooxmlObject.Stream).ToArray())));
            IndentLevel--;
        }

        private static void DumpEmbeddedObject(IEmbeddedObject embeddedObject)
        {
            IndentLevel++;
            Write(string.Format("{{Name: '{0}'}}", embeddedObject.GetName()));
            Write(string.Format("{{Clsid: {0}}}", embeddedObject.ClsidInternal));
            IndentLevel--;
        }

        private static void DumpHtmlOleControl(HtmlOleControl control)
        {
            Write(string.Format("{{Control: {0}}}", control.Type));
            Write(string.Format("  {{Html: '{0}'}}", EllipseText(control.Html)));

            if (control.Type == HtmlOleControlType.Option)
            {
                Write(string.Format("  {{Checked: {0}}}", ((HtmlOptionOleControl)control).Checked));
            }
        }

        private static void DumpForms2OleControl(Forms2OleControl control)
        {
            Write(string.Format("{{Control: {0}}}", control.Type));

            DumpPr(control.Pr);

            if (control is FormControl)
                DumpFormControl((FormControl)control);
        }

        private static void DumpFormControl(FormControl formControl)
        {
            foreach (Forms2OleControl childControl in formControl.ChildNodes)
            {
                IndentLevel++;
                DumpForms2OleControl(childControl);
                IndentLevel--;
            }
        }

        private static void DumpOleStream(string name, MemoryStorage data)
        {
            switch (name)
            {
                case Ole2StreamBase.OleStreamName:
                {
                    OleStream oleStream = OleStream.Read(data);
                    Write(string.Format("ObjectType: {0}", oleStream.ObjectType));
                    Write(string.Format("Path: {0}", oleStream.Path));
                    Write(string.Format("ClsId: {0}", oleStream.ClsId));
                    Write(string.Format("LocalUpdateTime: {0}", oleStream.LocalUpdateTime));
                    break;
                }

                case Ole2StreamBase.ObjInfoStreamName:
                {
                    ObjInfoStream objInfoStream = ObjInfoStream.Read(data);
                    Write(string.Format("Flags1: {0}", objInfoStream.Flags1));
                    Write(string.Format("Flags2: {0}", objInfoStream.Flags2));
                    break;
                }

                case Ole2StreamBase.CompObjStreamName:
                {
                    CompObjStream compObjStream = CompObjStream.Read(data);
                    Write(string.Format("ClsId: {0}", compObjStream.Clsid));
                    Write(string.Format("ProgId: {0}", compObjStream.ProgId));
                    Write(string.Format("UserType: {0}", compObjStream.UserType));
                    break;
                }

                case Ole2StreamBase.AttachDescStreamName:
                {
                    AttachDescStream attachDescStream = AttachDescStream.Read(data);
                    Write(string.Format("FileName: {0}", attachDescStream.FileName));
                    Write(string.Format("Extension: {0}", attachDescStream.Extension));
                    break;
                }

                case Ole2StreamBase.Ole10NativeStreamName:
                {
                    OlePackage olePackager = new OlePackage();
                    BinaryReader reader = new BinaryReader(data.GetStreamSafe(Ole2StreamBase.Ole10NativeStreamName));
                    reader.BaseStream.Position = 0;

                    if (StreamUtil.HasEnoughBytesToRead(reader, 4))
                    {
                        int size = reader.ReadInt32();
                        olePackager.Read(reader);
                        Write(string.Format("FileName: {0}", olePackager.FileName));
                        Write(string.Format("DisplayName: {0}", olePackager.DisplayName));
                        Write(string.Format("TempFileName: {0}", olePackager.TempFileName));
                    }
                    break;
                }

                default:
                    // No additional parsing is required for other streams.
                    break;
            }
        }

        private static string EllipseText(string text)
        {
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            const int maxLen = 80;
            if (text.Length < maxLen)
                return text;

            const int partLen = maxLen / 3;
            return string.Format("{0}...{1}", text.Substring(0, partLen), text.Substring(text.Length - partLen));
        }

        private static void ExtractGlobalTableAttrs(Table table)
        {
            foreach(Row row in table.Rows)
            {
                TablePr tablePr = row.ParentTable.TablePr;
                TablePr rowPr = row.TablePr;

                if (!row.TablePr.HasFormatRevision && (row.TablePr.WidthBefore.ValueRaw == 0))
                    row.TablePr.Remove(TableAttr.WidthBefore);

                if (row.IsFirstRow)
                {
                    foreach (int key in x)
                        MoveAttr(rowPr, tablePr, key);
                }
                else
                {
                    rowPr.Collapse(tablePr, TableAttr.Sys_TableGrid);
                }
            }
        }

        private static int JoinRunsWithSameFormatting(Document doc)
        {
            using (new SuspendMappedCustomXmlUpdateDocument(doc))
            {
                NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
                StringBuilder sb = new StringBuilder(1024);
                int joinCount = 0;

                foreach (Paragraph para in paragraphs)
                    joinCount += JoinRunsWithSameFormatting(para, sb);

                NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
                foreach (StructuredDocumentTag sdt in sdts)
                    if(sdt.Level == MarkupLevel.Inline)
                        joinCount += JoinRunsWithSameFormatting(sdt, sb);

                return joinCount;
            }
        }

        private static int JoinRunsWithSameFormatting(CompositeNode p, StringBuilder sb)
        {
            Debug.Assert((sb != null) && (sb.Length == 0));

            int joinCount = 0;
            Run prevRun = null;

            for (Node curNode = p.FirstChild; curNode != null; curNode = curNode.NextSibling)
            {
                if (curNode.NodeType == NodeType.Run)
                {
                    Run curRun = (Run)curNode;

                    if (prevRun != null)
                    {
                        // Found two adjacent runs. Let's compare their direct properties.
                        // During comparison some attributes should be ignored. See Run.KeysToIgnoreInComparisonOnJoin.
                        // Then collect texts in StringBuilder if properties are equal.
                        // Empty StringBuilder means that it is first join in the sequence.

                        // WORDSNET-4544 We should expand all property of runPrs before comparison runPrs.
                        RunPr expPrevRunPr = new RunPr();
                        // AM. This is workaround to get global defaults. I don't want to make defaults internal for a while.
                        expPrevRunPr.ExpandTo(expPrevRunPr, true);
                        prevRun.GetExpandedRunPr(RunPrExpandFlags.Normal).ExpandTo(expPrevRunPr);

                        RunPr expCurRunPr = new RunPr();
                        expCurRunPr.ExpandTo(expCurRunPr, true);
                        curRun.GetExpandedRunPr(RunPrExpandFlags.Normal).ExpandTo(expCurRunPr);

                        // Quick hack to join runs with edit revisions.
                        if (expPrevRunPr.Contains(RevisionAttr.DeleteRevision) &&
                            expCurRunPr.Contains(RevisionAttr.DeleteRevision))
                        {
                            expPrevRunPr.Remove(RevisionAttr.DeleteRevision);
                            expCurRunPr.Remove(RevisionAttr.DeleteRevision);
                        }

                        if (expPrevRunPr.Contains(RevisionAttr.InsertRevision) &&
                            expCurRunPr.Contains(RevisionAttr.InsertRevision))
                        {
                            expPrevRunPr.Remove(RevisionAttr.InsertRevision);
                            expCurRunPr.Remove(RevisionAttr.InsertRevision);
                        }

                        if (NoAttributes || NoRunAttributes || Equals(expPrevRunPr, expCurRunPr))
                        {
                            if (sb.Length == 0)
                                sb.Append(prevRun.Text);
                            sb.Append(curRun.Text);

                            ++joinCount;
                            p.RemoveChild(prevRun);
                        }
                        else
                        {
                            FlushJoinedRunsText(prevRun, sb);
                        }
                    }

                    prevRun = curRun;
                }
                else
                {
                    // Flush if any collected and forget last candidate since the sequence is finished.
                    FlushJoinedRunsText(prevRun, sb);
                    prevRun = null;
                }
            }

            // Reached the end of paragraph. Flush if any collected.
            FlushJoinedRunsText(prevRun, sb);
            Debug.Assert(sb.Length == 0);

            return joinCount;
        }

        /// <summary>
        /// Flushes collected text into the given run and reset StringBuilder.
        /// </summary>
        private static void FlushJoinedRunsText(Run run, StringBuilder sb)
        {
            if ((run != null) && (sb.Length != 0))
            {
                run.Text = sb.ToString();
                sb.Length = 0;
            }
        }

        private static bool Equals(AttrCollection coll1, AttrCollection coll2)
        {
            int pos1 = 0;
            int pos2 = 0;
            int key1 = 0;
            int key2 = 0;
            bool result;
            bool comparing;

            do
            {
                bool found1 = FindNonIgnoredKey(coll1, ref pos1, ref key1);
                bool found2 = FindNonIgnoredKey(coll2, ref pos2, ref key2);
                comparing = found1 && found2;
                result = comparing ? ((key1 == key2) && coll1.GetByIndex(pos1++).Equals(coll2.GetByIndex(pos2++))) : !(found1 || found2);
            }
            while (result && comparing);

            return result;
        }

        /// <summary>
        /// Finds first non-ignored key starting from pos in collection coll.
        /// If anything is found then returns true and sets pos to that key.
        /// </summary>
        private static bool FindNonIgnoredKey(AttrCollection coll, ref int pos, ref int key)
        {
            int collCount = coll.Count;
            while (pos < collCount)
            {
                key = coll.GetKey(pos);
                if (!IsIgnoreAttr(key))
                    return true;

                ++pos;
            }
            return false;
        }

        private static readonly int[] x = new int[]
        {
            // Table level only
            TableAttr.Istd, TableAttr.StyleOptions, TableAttr.AllowOverlap, TableAttr.StyleRowBandSize, TableAttr.StyleColBandSize, TableAttr.Bidi,
            TableAttr.FrameDistanceFromLeft,TableAttr.FrameDistanceFromRight,TableAttr.FrameDistanceFromTop, TableAttr.FrameDistanceFromBottom,
            TableAttr.RelativeVerticalPosition, TableAttr.RelativeHorizontalPosition,
            TableAttr.FrameLeft, TableAttr.FrameTop,
            TableAttr.HorizontalAlignment, TableAttr.VerticalAlignment,

            // Table level too but never occurred in DOC or RTF.
            TableAttr.Title,
            TableAttr.Description,

            TableAttr.BorderTop, TableAttr.BorderLeft, TableAttr.BorderRight, TableAttr.BorderBottom, TableAttr.BorderHorizontal, TableAttr.BorderVertical,
            TableAttr.PreferredWidth, TableAttr.AllowAutoFit,
            TableAttr.TopPadding, TableAttr.BottomPadding, TableAttr.LeftPadding, TableAttr.RightPadding,
            TableAttr.LeftIndent,
            TableAttr.Alignment,
            TableAttr.CellSpacing,
            TableAttr.Shading,
        };

        private static void MoveAttr(WordAttrCollection attr1, WordAttrCollection attr2, int key)
        {
            if (attr1.Contains(key) && !attr2.Contains(key))
            {
                attr2.SetAttr(key, attr1.GetDirectAttr(key));
                attr1.Remove(key);
            }
        }

        private static void DumpCustomization(Document doc)
        {
            if (doc.HasAllocatedCommands)
            {
                Write("");
                Write("[AllocatedCommands]");
                foreach (AllocatedCommand ac in doc.AllocatedCommands)
                    Write(string.Format("  {0}{1}", ac.FciBasedOn,
                                        (ac.ArgValue.Length > 0) ?
                                        (", " + ArrayUtil.DumpArray(ac.ArgValue).Replace(" ", "")) : ""));
            }

            if (doc.HasKeyMaps)
            {
                Write("");
                Write("[KeyMap]");
                foreach (KeyMap keyMap in doc.KeyMaps)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0}", keyMap.KeymapType);
                    switch (keyMap.KeymapType)
                    {
                        case KeyMapType.AllocatedCommand:
                            sb.AppendFormat("{0}", keyMap.AllocatedCommandIndex);
                            break;
                        case KeyMapType.FixedCommand:
                            sb.AppendFormat("{0}, {1}", keyMap.FixedCommandIdentifier, keyMap.FixedCommandArgument);
                            break;
                        case KeyMapType.InsertCharacter:
                            sb.AppendFormat("{0}", keyMap.CharacterCode);
                            break;
                        case KeyMapType.LegacyMacro:
                        case KeyMapType.Macro:
                            sb.AppendFormat("{0}", keyMap.MacroName);
                            break;
                        case KeyMapType.Mask:
                        case KeyMapType.None:
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("Invalid KeyMapType value {0}.", keyMap.KeymapType));
                    }

                    Write(string.Format("  {0}", sb));
                }
            }

            if (doc.HasAttachedToolbars)
            {
                Write("");
                Write("[AttachedToolbars]");
                Write("  " + ByteArrayToString(doc.AttachedToolbars));
            }
        }

        private static int GetFirstListId(ListDef listDef, ListCollection lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                if (ReferenceEquals(listDef, lists[i].ListDef))
                    return i;
            }

            return int.MaxValue;
        }

        private static void DumpLists(Document doc)
        {
            if (NoLists)
                return;

            SortedIntegerListGeneric<ListDef> sortedListDefs = new SortedIntegerListGeneric<ListDef>();

            SortedIntegerListGeneric<ListDef> orphanListDefs = new SortedIntegerListGeneric<ListDef>();

            foreach (ListDef listDef in doc.Lists.ListDefs)
            {
                int listId = GetFirstListId(listDef , doc.Lists);

                if (listId == int.MaxValue)
                {
                    // Collect orphan list definitions.
                    orphanListDefs[listDef.ListDefId] = listDef;
                }
                else
                {
                    sortedListDefs[listId] = listDef;
                }
            }

            for (int l = 0; l < sortedListDefs.Count; l++)
            {
                ListDef listDef = sortedListDefs.GetByIndex(l);
                DumpListDef(listDef, false);
            }

            for (int l = 0; l < orphanListDefs.Count; l++)
            {
                ListDef listDef = orphanListDefs.GetByIndex(l);
                DumpListDef(listDef, true);
            }

            foreach (List list in doc.Lists)
            {
                Write("");
                Write(string.Format("[List {0} (0x{1:x8})]", list.ListId, list.ListDefId));

                IndentLevel++;
                for (int i = 0; i < list.Overrides.Count; i++)
                {
                    ListLevelOverride lo = list.Overrides[i];
                    StringBuilder sb = new StringBuilder();
                    if (lo.IsStartAt) sb.AppendFormat("StartAt, ");
                    if (lo.IsFormatting) sb.AppendFormat("Formatting, ");
                    sb.AppendFormat("{0}, 0x{1:x8}/0x{2:x8}", lo.ListLevel.LevelNumber, lo.StartAtRaw, lo.StartAtReal);

                    Write(string.Format("[List Level Override ({0})]", sb));
                    IndentLevel++;
                    DumpListLevel(lo.ListLevel);
                    IndentLevel--;
                }
                IndentLevel--;
            }

            for (int i = 0; i < doc.Lists.PictureBulletCount; i++)
            {
                Write("");
                Write(string.Format("[Picture Bullet: {0}]", i));
                Shape pictureBullet = doc.Lists.GetPictureBullet(i);
                IndentLevel++;
                DumpNode(pictureBullet);
                IndentLevel--;
            }
        }

        private static void DumpListDef(ListDef listDef, bool orphaned)
        {
            Write("");
            Write(string.Format("[List Definition 0x{0:x8} ({6} 0x{1:x2}, {2}{4}, '{3}')]{5}",
                listDef.ListDefId,
                listDef.ListStyleIstd,
                listDef.ListType,
                listDef.Name,
                (listDef.IsRestartAtEachSection ? "(RestartAtEachSection)" : ""),
                orphaned ? "*" : "",
                listDef.IsListStyleReference ? "reference to" : "definition for"));
            IndentLevel++;
            for (int i = 0; i < listDef.Levels.Count; i++)
            {
                DumpListLevel(listDef.Levels[i]);
            }
            IndentLevel--;
        }

        private static void DumpListLevel(ListLevel level)
        {
            StringBuilder sb = new StringBuilder();
            if(level.ParaStyleIstd != StyleIndex.Nil)
                sb.AppendFormat("linked style: 0x{0:x2}, ", level.ParaStyleIstd);
            sb.AppendFormat("{0}, ", level.StartAt);
            sb.AppendFormat("{0}, ", level.NumberStyle);
            sb.AppendFormat("'{0}', ", GetPrintableText(level.NumberFormat));
            sb.AppendFormat("{0}, ", level.Alignment);

            if (level.IsLegal) sb.Append("Legal, ");
            if (level.LegacyPrev) sb.Append("LegacyPrev, ");
            if (level.LegacyPrevSpace) sb.Append("LegacyPrevSpace, ");
            if (level.Legacy) sb.Append("Legacy, ");
            if (level.IsTentative) sb.Append("Tentative, ");

            sb.AppendFormat("{0}, ", level.TrailingCharacter);
            sb.AppendFormat("{0}, ", level.LegacySpace);
            sb.AppendFormat("{0}, ", level.LegacyIndent);

            sb.AppendFormat("'{0}'", level.CustomNumberStyle);

            if (level.HasPictureBullet)
                sb.AppendFormat(", PictureBullet: {0}", level.PictureBulletId);

            Write(string.Format("[Level {0} ({1})]", level.LevelNumber, sb));

            RunPr runPr = level.RunPr;
            ParaPr paraPr = level.ParaPr;
            if ((runPr.Count > 0) || (paraPr.Count > 0))
            {
                DumpPr(runPr);
                DumpPr(paraPr);
            }
        }

        private static void DumpBuiltinDocumentProperties(Document doc)
        {
            Write("\n[Built-in Document Properties]");

            foreach (DocumentProperty docProperty in doc.BuiltInDocumentProperties)
                Write(string.Format("  {{'{0}'({1}) = '{2}'{3}}}", docProperty.Name, docProperty.Type,
                    docProperty.Value.ToString(),
                    // Write printable version if different.
                    docProperty.Value.ToString() != GetPrintableText(docProperty.ValueInternal.ToString())
                        ? string.Format(" ('{0}')", GetPrintableText(docProperty.ValueInternal.ToString()))
                        : ""
                    ));
        }

        private static void DumpCustomDocumentProperties(Document doc)
        {
            Write("\n[Custom Document Properties]");

            foreach(DocumentProperty docProperty in doc.CustomDocumentProperties)
                Write(string.Format("  {{'{0}'({1}) = '{2}'{3}{4}}}", docProperty.Name, docProperty.Type,
                    docProperty.Value.ToString(),
                    // Write printable version if different.
                    docProperty.Value.ToString() != GetPrintableText(docProperty.ValueInternal.ToString())
                        ? string.Format(" ('{0}')", GetPrintableText(docProperty.ValueInternal.ToString()))
                        : "",
                    docProperty.IsLinkToContent
                        ? string.Format(", linked to: '{0}'", docProperty.LinkTarget)
                        : ""));
        }

        private static void DumpVariables(Document doc)
        {
            Write("\n[Variables]");

            foreach (KeyValuePair<string, string> var in doc.Variables)
                Write(string.Format("  {{'{0}' = '{1}'{2}}}", var.Key, var.Value,
                    // Write printable version if different.
                    var.Value != GetPrintableText(var.Value) ? string.Format(" ('{0}')", GetPrintableText(var.Value)) : ""));
        }

        private static void DumpStyles(Document doc)
        {
            if (NoStyles)
                return;

            Write("\n[DefaultParaPr]");
            DumpPr(doc.Styles.DefaultParaPr, new ParaPr());

            Write("\n[DefaultRunPr]");
            DumpPr(doc.Styles.DefaultRunPr, new RunPr());

            if (!NoLatentStyles)
            {
                Write("\n[Latent styles]");
                const string lsFmt = "  {{{0}}}";
                LatentStyles ls = doc.Styles.LatentStyles;

                if (ls.DefaultLockedState) Write(lsFmt, "DefaultLockedState");
                if (ls.DefaultQuickFormat) Write(lsFmt, "DefaultQuickFormat");
                if (ls.DefaultSemiHidden) Write(lsFmt, "DefaultSemiHidden");
                if (ls.DefaultUnhideWhenUsed) Write(lsFmt, "DefaultUnhideWhenUsed");
                Write(lsFmt, string.Format("DefaultUIPriority: 0x{0:x2}", ls.DefaultUIPriority));

                IndentLevel++;
                for (int i = 0; i < ls.Count; i++)
                {
                    LatentStyle latentStyle = ls[i];

                    StringBuilder sb = new StringBuilder();

                    if (latentStyle.Locked) sb.Append(" Locked");
                    if (latentStyle.QuickStyle) sb.Append(" QuickFormat");
                    if (latentStyle.SemiHidden) sb.Append(" SemiHidden");
                    if (latentStyle.UnhideWhenUsed) sb.Append(" UnhideWhenUsed");

                    Write(string.Format("{0:x2}, {1}, Priority: 0x{2:x2}{3}",
                        (int)latentStyle.StyleIdentifier, latentStyle.StyleIdentifier, latentStyle.UIPriority, (sb.Length > 0 ? string.Format(" ({0}) ", sb.ToString().Trim()) : "")));
                }
                IndentLevel--;
            }

            // Write styles sorted by istd.
            List<int> istds = new List<int>();
            foreach (Style style in doc.Styles)
                istds.Add(style.Istd);
            istds.Sort();

            for(int i = 0; i < istds.Count; i++)
            {
                Style style = doc.Styles.GetByIstd(istds[i], false);

                StringBuilder sb = new StringBuilder();

                if (style.Locked) sb.Append(" Locked");
                if (style.IsQuickStyle) sb.Append(" QuickFormat");
                if (style.SemiHidden)
                    sb.Append(" SemiHidden");
                if (style.UnhideWhenUsed) sb.Append(" UnhideWhenUsed");

                string styleOptions = string.Format("0x{0:x2}{1}", style.Priority,
                    sb.Length > 0 ? string.Format(" ({0})", sb.ToString().Trim()) : "");


                Write(string.Format("\n[0x{0:x2}({1}), {2}: '{3}'{4}{5}{6}, {7}]",
                                    style.Istd,
                                    style.StyleIdentifier,
                                    style.Type,
                                    GetPrintableText(doc.Styles.GetAliases(style, true)),
                                    style.BasedOnIstd != StyleIndex.Nil
                                        ? string.Format(", based on '{0}'", style.GetBaseStyle().Name): "",
                                    style.NextIstd != StyleIndex.Nil
                                        ? string.Format(", followed by '{0}'", style.GetNextStyle().Name) : "",
                                    style.LinkedIstd != StyleIndex.Nil
                                        ? string.Format(", linked with '{0}'", style.GetLinkedStyle().Name) : "",
                                    styleOptions));

                switch (style.Type)
                {
                    case StyleType.Character:
                        DumpPr(style.RunPr, style);
                        break;

                    case StyleType.Paragraph:
                        DumpPr(style.ParaPr, style);

                        if (ShowExpanded)
                        {
                            Write("  >>");
                            DumpPr(style.GetExpandedParaPr(ParaPrExpandFlags.Layout));
                        }

                        DumpPr(style.RunPr, style);
                        break;

                    case StyleType.List:
                        DumpPr(style.ParaPr);
                        DumpPr(style.RunPr);
                        break;

                    case StyleType.Table:
                        {
                            TableStyle tableStyle = (TableStyle) style;
                            DumpPr(tableStyle.TablePr);
                            DumpPr(tableStyle.RowPr);
                            DumpPr(tableStyle.CellPr);
                            DumpPr(tableStyle.ParaPr);
                            DumpPr(tableStyle.RunPr);
                            IndentLevel++;

                            SortedStringListGeneric<ConditionalStyle> sortedList =
                                new SortedStringListGeneric<ConditionalStyle>();
                            foreach (ConditionalStyle conditionalStyle in tableStyle.ConditionalStyles.DefinedStyles)
                                sortedList.Add(conditionalStyle.OverrideType.ToString(), conditionalStyle);

                            for(int k = 0; k < sortedList.Count; k++)
                            {
                                ConditionalStyle conditionalStyle = sortedList.GetByIndex(k);

                                Write(string.Format("[{0}]", conditionalStyle.OverrideType));
                                DumpPr(conditionalStyle.TablePr);
                                DumpPr(conditionalStyle.RowPr);
                                DumpPr(conditionalStyle.CellPr);
                                DumpPr(conditionalStyle.ParaPr);
                                DumpPr(conditionalStyle.RunPr);
                            }
                            IndentLevel--;
                            break;
                        }

                    default:
                        throw new InvalidOperationException(string.Format("Invalid StyleType value {0}.", style.Type));
                }
            }
        }

        private static string HorizontalAlignmentToString(HorizontalAlignment value)
        {
            string text = value.ToString();
            if (value == HorizontalAlignment.Default)
                text = "None";

            return text;
        }

        private static string RelativeHorizontalPositionToString(RelativeHorizontalPosition value)
        {
            string text = value.ToString();
            if (value == RelativeHorizontalPosition.Default)
                text = "Column";

            return text;
        }

        private static string VerticalAlignmentToString(VerticalAlignment value)
        {
            string text = value.ToString();
            if (value == VerticalAlignment.Default)
                text = "None";

            return text;
        }

        private static string RelativeVerticalPositionToString(RelativeVerticalPosition value)
        {
            string text = value.ToString();
            if (value == RelativeVerticalPosition.TableDefault)
                text = "Margin";
            if (value == RelativeVerticalPosition.TextFrameDefault)
                text = "Paragraph";

            return text;
        }


        private static string AttrBoolExToString(AttrBoolEx boolEx)
        {
            switch (boolEx.Value)
            {
                case 0:
                    return "False";
                case 1:
                    return "True";
                case 128:
                    return "Same";
                case 129:
                    return "Toggle";
                default:
                    return "?";
            }
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            if (bytes == null)
                return "(null)";

            if (bytes.Length == 0)
                return "(empty)";

            if (ArrayUtil.CompareBytes(ImageUtil.GetNoImageBytes(), bytes, bytes.Length))
                return "(NoImage)";

            return string.Format("{0} bytes, MD5: {1}", bytes.Length, ArrayUtil.DumpArray(HashUtil.ComputeHash(DigestAlgorithm.MD5, bytes)).Replace(" ", ""));
        }

        private static string RawByteArrayToString(byte[] bytes)
        {
            if (bytes == null)
                return "(null)";

            if (bytes.Length == 0)
                return "(empty)";

            return string.Format("{0} bytes, {1}", bytes.Length, ArrayUtil.DumpArray(bytes).Replace(" ", ""));
        }

        private static string Int32ArrayToString(int[] ints)
        {
            if (ints == null)
                return "(null)";

            if (ints.Length == 0)
                return "(empty)";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ints.Length; i++)
                sb.AppendFormat("{0}, ", i);

            return RemoveTrailingComma(sb);
        }

        internal static string BorderToString(Border border)
        {
            if(border.Equals(Border.Empty))
                return "(empty)";

            if(border.IsNil)
                return  "(nil)";

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0:x8} {1} {2} {3}{4}{5}",
                border.Color.ToArgb(),
                border.LineStyle,
                border.LineWidth,
                border.DistanceFromText,
                border.Shadow ? " Shadow" : "",
                border.IsInherited ? " Inherited" : "");

            if (StringUtil.HasChars(border.ThemeColorInternal))
                sb.AppendFormat(", ThemeColor: {0}", border.ThemeColorInternal);
            if (StringUtil.HasChars(border.ThemeShade))
                sb.AppendFormat(", ThemeShade: {0}", border.ThemeShade);
            if (StringUtil.HasChars(border.ThemeTint))
                sb.AppendFormat(", ThemeTint: {0}", border.ThemeTint);

            return string.Format("({0})", sb);
        }

        private static string PathPointArrayToString(PathPoint[] pathPointArray)
        {
            if (pathPointArray == null)
                return "(null)";

            if (pathPointArray.Length == 0)
                return "(empty)";

            StringBuilder sb = new StringBuilder();
            foreach (PathPoint pathPoint in pathPointArray)
                sb.AppendFormat("{0},", PathPointToString(pathPoint));

            return RemoveTrailingComma(sb);
        }

        private static string PathInfoArrayToString(PathInfo[] pathInfoArray)
        {
            if (pathInfoArray == null)
                return "(null)";

            if (pathInfoArray.Length == 0)
                return "(empty)";

            StringBuilder sb = new StringBuilder();
            foreach (PathInfo pathInfo in pathInfoArray)
                sb.AppendFormat("{0},", PathInfoToString(pathInfo));

            return RemoveTrailingComma(sb);
        }

        private static string PathPointToString(PathPoint pathPoint)
        {
            return string.Format("{{{0}:{1}}}", pathPoint.X, pathPoint.Y);
        }

        private static string PathInfoToString(PathInfo pathInfo)
        {
            return string.Format("{{{0}:{1}}}", pathInfo.PathType, pathInfo.SegmentCount);
        }

        private static string RemoveTrailingComma(string text)
        {
            return text.TrimEnd(new char[] { ',', ' ' });
        }

        private static string RemoveTrailingComma(StringBuilder sb)
        {
            return RemoveTrailingComma(sb.ToString());
        }

        internal static string ShadingToString(Shading shading)
        {
            StringBuilder sb = new StringBuilder();

            if (shading != null)
            {
                sb.AppendFormat("Back: {0:x8}, Fore: {1:x8}, {2}", shading.BackgroundPatternColorInternal.ToArgb(),
                                shading.ForegroundPatternColorInternal.ToArgb(), shading.Texture);
                if (StringUtil.HasChars(shading.ThemeColor))
                    sb.AppendFormat(", ThemeColor: {0}", shading.ThemeColor);
                if (StringUtil.HasChars(shading.ThemeFill))
                    sb.AppendFormat(", ThemeFill: {0}", shading.ThemeFill);
                if (StringUtil.HasChars(shading.ThemeFillShade))
                    sb.AppendFormat(", ThemeFillShade: {0}", shading.ThemeFillShade);
                if (StringUtil.HasChars(shading.ThemeFillTint))
                    sb.AppendFormat(", ThemeFillTint: {0}", shading.ThemeFillTint);
                if (StringUtil.HasChars(shading.ThemeTint))
                    sb.AppendFormat(", ThemeTint: {0}", shading.ThemeTint);
            }

            return string.Format("({0})", sb);
        }

        private static string TabStopCollectionToString(TabStopCollection tabs)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tabs.Count; i++)
            {
                TabStop tab = tabs[i];
                sb.AppendFormat("{0}{3}:{1}{2} ",
                    tab.Alignment,
                    tab.Position,
                    tab.ToleranceTwips != 0 ? string.Format("({0})", tab.ToleranceTwips) : "",
                    tab.IsLegacyTab ? "(Legacy)" : "");
            }
            return sb.ToString().Trim();
        }

        private static string EmbeddedObjectToString(IEmbeddedObject embeddedObject)
        {
            return string.Format("(ID: {0})", embeddedObject.Id);
        }

        private static string EditRevisionToString(EditRevision revision)
        {
            if (NoRevisionDetails)
                return "(-)";

            return string.Format("({0}, {1})", revision.Author, revision.DateTime);
        }

        private static string MoveRevisionToString(MoveRevision revision)
        {
            if (NoRevisionDetails)
                return "(-)";

            return string.Format("({0}, {1})", revision.Author, revision.DateTime);
        }

        private static string NumberRevisionToString(ParagraphNumberRevision revision)
        {
            StringBuilder sb = new StringBuilder();

            if (revision.IsActive)
                sb.Append("Active, ");
            if (revision.IsInsertion)
                sb.Append("Insertion, ");
            if (revision.IsNumbering)
                sb.Append("Numbering, ");
            if (revision.WasNumbered)
                sb.Append("WasNumbered, ");

            string flags = sb.ToString().Trim(gListSeparatorChars);

            return string.Format("{{{0}, NumberFormat: '{1}', NumberLocations: '{2}'}} ({3}, {4})",
                flags, revision.NumberFormat, revision.NumberLocations, revision.Author, revision.DateTime);
        }

        private static string ArrayListToString(CellPrCollection cellPrCollection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (AttrCollection o in cellPrCollection)
                sb.AppendFormat(" {0} ", o);

            return string.Format("({0})", sb.ToString().Trim());
        }

        private static string Int32ListToString(IntList list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
                sb.AppendFormat(" {0} ", list[i]);

            return string.Format("({0})", sb.ToString().Trim());
        }

        private static bool IsRunAttribute(int key)
        {
            return (0 <= key && key < 1000 && !IsRevisionKey(key));
        }

        private static bool IsParaAttribute(int key)
        {
            return (1000 <= key && key < 2000);
        }

        private static bool IsTableAttribute(int key)
        {
            return (4000 <= key && key < 5200);
        }

        private static bool IsFormFieldAttribute(int key)
        {
            return (21000 <= key && key <= 21170);
        }

        private static bool IsMathAttribute(int key)
        {
            return (15000 <= key && key <= 16000);
        }

        private static bool IsCellAttribute(int key)
        {
            return (3000 <= key && key < 4000);
        }

        private static bool IsForms2Attribute(int key)
        {
            return (31000 <= key && key < 32000);
        }

        private static string GetFmt(int key)
        {
            if (IsRevisionKey(key))
                return "  {{Revision.{0}: {1}}}";
            if (IsRunAttribute(key))
                return "  {{Run.{0}: {1}}}";
            if (IsParaAttribute(key))
                return "  {{Para.{0}: {1}}}";
            if (2000 <= key && key < 3000)
                return "  {{Sect.{0}: {1}}}";
            if (IsCellAttribute(key))
                return "  {{Cell.{0}: {1}}}";
            if (IsTableAttribute(key))
                return "  {{Table.{0}: {1}}}";
            if (IsFormFieldAttribute(key))
                return "  {{FormField.{0}: {1}}}";
            if (IsMathAttribute(key))
                return "  {{Math.{0}: {1}}}";
            if (IsForms2Attribute(key))
                return "  {{Forms2.{0}: {1}}}";

            return "  {{?.{0}: {1}}}";
        }

        private static bool IsRevisionKey(int key)
        {
            return
                (key == RevisionAttr.InsertRevision) ||
                (key == RevisionAttr.DeleteRevision) ||
                (key == RevisionAttr.NumberRevision) ||
                (key == RevisionAttr.MoveFromRevision) ||
                (key == RevisionAttr.MoveToRevision) ||
                (key == RevisionAttr.FormatRevision);
        }

        private static string FormatAttr(int key, object value)
        {
            string fmt = GetFmt(key);

            switch (key)
            {
                case FormFieldAttr.CalcOnExit:
                    return string.Format(fmt, "CalcOnExit", value);
                case FormFieldAttr.CheckBoxChecked:
                    return string.Format(fmt, "CheckBoxChecked", value);
                case FormFieldAttr.CheckBoxDefault:
                    return string.Format(fmt, "CheckBoxDefault", value);
                case FormFieldAttr.CheckBoxSizeAuto:
                    return string.Format(fmt, "CheckBoxSizeAuto", value);
                case FormFieldAttr.CheckBoxSizeHalfPoints:
                    return string.Format(fmt, "CheckBoxSizeHalfPoints", value);
                case FormFieldAttr.DropDownDefault:
                    return string.Format(fmt, "DropDownDefault", value);
                case FormFieldAttr.DropDownItems:
                    return string.Format(fmt, "DropDownItems", value);
                case FormFieldAttr.DropDownResult:
                    return string.Format(fmt, "DropDownResult", value);
                case FormFieldAttr.Enabled:
                    return string.Format(fmt, "Enabled", value);
                case FormFieldAttr.EntryMacro:
                    return string.Format(fmt, "EntryMacro", value);
                case FormFieldAttr.ExitMacro:
                    return string.Format(fmt, "ExitMacro", value);
                case FormFieldAttr.HelpText:
                    return string.Format(fmt, "HelpText", value);
                case FormFieldAttr.Name:
                    return string.Format(fmt, "Name", value);
                case FormFieldAttr.OwnHelpText:
                    return string.Format(fmt, "OwnHelpText", value);
                case FormFieldAttr.OwnStatusText:
                    return string.Format(fmt, "OwnStatusText", value);
                case FormFieldAttr.StatusText:
                    return string.Format(fmt, "StatusText", value);
                case FormFieldAttr.TextInputDefault:
                    return string.Format(fmt, "TextInputDefault", value);
                case FormFieldAttr.TextInputFormat:
                    return string.Format(fmt, "TextInputFormat", value);
                case FormFieldAttr.TextInputMaxLength:
                    return string.Format(fmt, "TextInputMaxLength", value);
                case FormFieldAttr.TextInputType:
                    return string.Format(fmt, "TextInputType", value);
                //////////////////////
                case TableAttr.Sys_TableGridForNewAlgorithm:
                    return string.Format(fmt, "_TableGridForNewAlgorithm", Int32ListToString(((TableGridColumnsAttr)value).GridColumns));
                case TableAttr.Sys_Cells:
                    return string.Format(fmt, "_Cells", ArrayListToString((CellPrCollection) value));
                case TableAttr.Sys_TableGrid:
                    return string.Format(fmt, "_TableGrid", Int32ListToString((IntList)value));
                case TableAttr.Sys_GridBefore:
                    return string.Format(fmt, "_GridBefore", value);
                case TableAttr.Sys_LeftIndent97:
                    return string.Format(fmt, "_LeftIndent97", value);
                case TableAttr.Sys_GridAfter:
                    return string.Format(fmt, "_GridAfter", value);
                case TableAttr.Sys_Alignment97:
                    return string.Format(fmt, "_Alignment97", value);
                case TableAttr.Sys_BidiTable97:
                    return string.Format(fmt, "_BidiTable97", value);
                case TableAttr.Sys_DxaLeft:
                    return string.Format(fmt, "_DxaLeft", value);
                case TableAttr.Sys_DxaGapHalf:
                    return string.Format(fmt, "_DxaGapHalf", value);
                //////////////////////
                case RevisionAttr.InsertRevision:
                case RevisionAttr.DeleteRevision:
                    return string.Format(fmt, ((EditRevision) value).Type, EditRevisionToString((EditRevision) value));
                case RevisionAttr.MoveFromRevision:
                    return string.Format(fmt, ((MoveRevision)value).Type, MoveRevisionToString((MoveRevision)value));
                case RevisionAttr.MoveToRevision:
                    return string.Format(fmt, ((MoveRevision)value).Type, MoveRevisionToString((MoveRevision)value));
                case RevisionAttr.NumberRevision:
                    return string.Format(fmt, "NumberRevision", NumberRevisionToString((ParagraphNumberRevision)value));
                //////////////////////
                case MathAttr.AccentCharacter:
                    return string.Format(fmt, "AccentCharacter", value);
                case MathAttr.BaseJustification:
                    return string.Format(fmt, "BaseJustification", value);
                case MathAttr.RowSpacing:
                    return string.Format(fmt, "RowSpacing", value);
                case MathAttr.RowSpacingRule:
                    return string.Format(fmt, "RowSpacingRule", value);
                case MathAttr.Justification:
                    return string.Format(fmt, "Justification", value);
                //////////////////////

                case FontAttr.UnderlineThemeColor:
                    return string.Format(fmt, "UnderlineThemeColor", value);
                case FontAttr.UnderlineThemeShade:
                    return string.Format(fmt, "UnderlineThemeShade", value);
                case FontAttr.UnderlineThemeTint:
                    return string.Format(fmt, "UnderlineThemeTint", value);
                case FontAttr.MathStyle:
                    return string.Format(fmt, "MathStyle", value);
                case FontAttr.Ruby:
                    return string.Format(fmt, "Ruby", RubyToString((Ruby)value));
                case FontAttr.FarEastLayout:
                    return string.Format(fmt, "FarEastLayout", FarEastLayoutToString((FarEastLayout)value));
                case FontAttr.Sys_Symbol:
                    return string.Format(fmt, "_Symbol", value);
                case FontAttr.PictureBulletId:
                    return string.Format(fmt, "PictureBulletId", value);
                case FontAttr.PictureBulletFlags:
                    return string.Format(fmt, "PictureBulletFlags", value);
                case FontAttr.Border:
                    return string.Format(fmt, "Border", BorderToString((Border)value));
                case FontAttr.ThemeTint:
                    return string.Format(fmt, "ThemeTint", value);
                case FontAttr.TextEffect:
                    return string.Format(fmt, "TextEffect", value);
                case FontAttr.Outline:
                    return string.Format(fmt, "Outline", value);
                case FontAttr.Emboss:
                    return string.Format(fmt, "Emboss", value);
                case FontAttr.Engrave:
                    return string.Format(fmt, "Engrave", value);
                case FontAttr.EmphasisMark:
                    return string.Format(fmt, "EmphasisMark", value);
                case FontAttr.EffectGlow:
                    return string.Format(fmt, "EffectGlow", DmlShapeEffectToString(value));
                case FontAttr.EffectShadow:
                    return string.Format(fmt, "EffectShadow", DmlShapeEffectToString(value));
                case FontAttr.EffectReflection:
                    return string.Format(fmt, "EffectReflection", DmlShapeEffectToString(value));
                case FontAttr.EffectOutline:
                    return string.Format(fmt, "EffectOutline", DmlShapeEffectToString(value));
                case FontAttr.EffectFill:
                    return string.Format(fmt, "EffectFill", DmlShapeEffectToString(value));
                case FontAttr.EffectScene3D:
                    return string.Format(fmt, "EffectScene3D", DmlShapeEffectToString(value));
                case FontAttr.EffectProps3D:
                    return string.Format(fmt, "EffectProps3D", DmlShapeEffectToString(value));
                case FontAttr.OpenTypeStylisticSets:
                    return string.Format(fmt, "OpenTypeStylisticSets", value);
                case FontAttr.OpenTypeLigature:
                    return string.Format(fmt, "OpenTypeLigature", value);
                case FontAttr.OpenTypeNumForm:
                    return string.Format(fmt, "OpenTypeNumForm", value);
                case FontAttr.OpenTypeNumSpacing:
                    return string.Format(fmt, "OpenTypeNumSpacing", value);
                case FontAttr.OpenTypeContextualAlternates:
                    return string.Format(fmt, "OpenTypeContextualAlternates", value);
                case FontAttr.AlternateContent:
                    return string.Format(fmt, "AlternateContent", value);
                case FontAttr.Istd:
                    return string.Format(fmt, "Istd", string.Format("0x{0:x2}", value));
                case FontAttr.StrikeThrough:
                    return string.Format(fmt, "StrikeThrough", value);
                case FontAttr.DoubleStrikeThrough:
                    return string.Format(fmt, "DoubleStrikeThrough", value);
                case FontAttr.HyphenRule:
                    return string.Format(fmt, "HyphenRule", value);
                case FontAttr.HyphenChar:
                    return string.Format(fmt, "HyphenChar", value);
                case FontAttr.Spacing:
                    return string.Format(fmt, "Spacing", value);
                case FontAttr.SpecialHidden:
                    return string.Format(fmt, "SpecialHidden", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.Bold:
                    return string.Format(fmt, "Bold", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.Italic:
                    return string.Format(fmt, "Italic", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.ItalicBi:
                    return string.Format(fmt, "ItalicBi", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.SmallCaps:
                    return string.Format(fmt, "SmallCaps", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.BoldBi:
                    return string.Format(fmt, "BoldBi", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.NameAscii:
                    return string.Format(fmt, "NameAscii", value);
                case FontAttr.NameFarEast:
                    return string.Format(fmt, "NameFarEast", value);
                case FontAttr.NameOther:
                    return string.Format(fmt, "NameOther", value);
                case FontAttr.NameBi:
                    return string.Format(fmt, "NameBi", value);
                case FontAttr.CharacterCategoryHint:
                    return string.Format(fmt, "CharacterCategoryHint", (CharacterCategory) value);
                case FontAttr.Bidi:
                    return string.Format(fmt, "BiDi", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.ComplexScript:
                    return string.Format(fmt, "ComplexScript", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.RsidR:
                    return string.Format(fmt, "RsidR", string.Format("0x{0:x8}", value));
                case FontAttr.RsidRPr:
                    return string.Format(fmt, "RsidRPr", string.Format("0x{0:x8}", value));
                case FontAttr.LocaleIdBi:
                    return string.Format(fmt, "LocaleIdBi", (Language) value);
                case FontAttr.LocaleId:
                    return string.Format(fmt, "LocaleId", (Language) value);
                case FontAttr.LocaleIdFarEast:
                    return string.Format(fmt, "LocaleIdFarEast", (Language) value);
                case FontAttr.Size:
                    return string.Format(fmt, "Size", value);
                case FontAttr.SizeBi:
                    return string.Format(fmt, "SizeBi", value);
                case FontAttr.ThemeShade:
                    return string.Format(fmt, "ThemeShade", value);
                case FontAttr.ThemeColor:
                    return string.Format(fmt, "ThemeColor", value);
                case FontAttr.NoProofing:
                    return string.Format(fmt, "NoProofing", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.Shadow:
                    return string.Format(fmt, "Shadow", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.AllCaps:
                    return string.Format(fmt, "AllCaps", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.Color:
                    return string.Format(fmt, "Color", value);
                case FontAttr.Underline:
                    return string.Format(fmt, "Underline", value);
                case FontAttr.Kerning:
                    return string.Format(fmt, "Kerning", value);
                case FontAttr.VerticalAlignment:
                    return string.Format(fmt, "VerticalAlignment", value);
                case FontAttr.Position:
                    return string.Format(fmt, "Position", value);
                case FontAttr.SnapToGrid:
                    return string.Format(fmt, "SnapToGrid", AttrBoolExToString((AttrBoolEx) value));
                case FontAttr.Hidden:
                    return string.Format(fmt, "Hidden", AttrBoolExToString((AttrBoolEx)value));
                case FontAttr.WebHidden:
                    return string.Format(fmt, "WebHidden", AttrBoolExToString((AttrBoolEx)value));
                case FontAttr.UnderlineColor:
                    return string.Format(fmt, "UnderlineColor", value);
                case FontAttr.HighlightColor:
                    return string.Format(fmt, "HighlightColor", value);
                case FontAttr.Shading:
                    return string.Format(fmt, "Shading", ShadingToString((Shading) value));
                case FontAttr.Scaling:
                    return string.Format(fmt, "Scaling", value);
                case FontAttr.FitText:
                    return string.Format(fmt, "FitText", value);
                case FontAttr.MathIsNormalText:
                    return string.Format(fmt, "MathIsNormalText", value);
                //////////////////////
                case ParaAttr.MirrorIndents:
                    return string.Format(fmt, "MirrorIndents", value);
                case ParaAttr.TopLinePunctuation:
                    return string.Format(fmt, "TopLinePunctuation", value);
                case ParaAttr.SuppressLineNumbers:
                    return string.Format(fmt, "SuppressLineNumbers", value);
                case ParaAttr.HtmlMarginLeft:
                    return string.Format(fmt, "HtmlMarginLeft", value);
                case ParaAttr.HtmlMarginRight:
                    return string.Format(fmt, "HtmlMarginRight", value);
                case ParaAttr.HtmlMarginTop:
                    return string.Format(fmt, "HtmlMarginTop", value);
                case ParaAttr.HtmlMarginBottom:
                    return string.Format(fmt, "HtmlMarginBottom", value);
                case ParaAttr.TextboxTightWrap:
                    return string.Format(fmt, "TextTightWrap", value);
                case ParaAttr.FrameSuppressOverlap:
                    return string.Format(fmt, "FrameSuppressOverlap", value);
                case ParaAttr.DropCapLinesToDrop:
                    return string.Format(fmt, "DropCapLinesToDrop", value);
                case ParaAttr.DropCapPosition:
                    return string.Format(fmt, "DropCapPosition", value);
                case ParaAttr.FrameHeight:
                    return string.Format(fmt, "FrameHeight", HeightToString((Height)value));
                case ParaAttr.Sys_Anld:
                    return string.Format(fmt, "_Anld", "FOSS");
                case ParaAttr.Sys_LvlAnm:
                    return string.Format(fmt, "_LvlAnm", (int)value);
                case ParaAttr.RsidP:
                    return string.Format(fmt, "RsidP", string.Format("0x{0:x8}", value));
                case ParaAttr.Shading:
                    return string.Format(fmt, "Shading", ShadingToString((Shading) value));
                case ParaAttr.FrameVerticalDistanceFromText:
                    return string.Format(fmt, "FrameVerticalDistanceFromText", value);
                case ParaAttr.FrameHorizontalDistanceFromText:
                    return string.Format(fmt, "FrameHorizontalDistanceFromText", value);
                case ParaAttr.KeepWithNext:
                    return string.Format(fmt, "KeepWithNext", value);
                case ParaAttr.NoSpaceBetweenSameStyle:
                    return string.Format(fmt, "NoSpaceBetweenSameStyle", value);
                case ParaAttr.SpaceBefore:
                    return string.Format(fmt, "SpaceBefore", (int) value);
                case ParaAttr.SpaceAfter:
                    return string.Format(fmt, "SpaceAfter", (int) value);
                case ParaAttr.Alignment:
                    return string.Format(fmt, "Alignment", value);
                case ParaAttr.Sys_LeftIndent97:
                    return string.Format(fmt, "_LeftIndent97", value);
                case ParaAttr.Sys_FirstLineIndent97:
                    return string.Format(fmt, "_FirstLineIndent97", value);
                case ParaAttr.Sys_Alignment97:
                    return string.Format(fmt, "_Alignment97", value);
                case ParaAttr.Sys_RightIndent97:
                    return string.Format(fmt, "_RightIndent97", value);
                case ParaAttr.TabStops:
                    return string.Format(fmt, "TabStops", TabStopCollectionToString((TabStopCollection) value));
                case ParaAttr.Istd:
                    return string.Format(fmt, "Istd", string.Format("0x{0:x2}", value));
                case ParaAttr.AutoAdjustRightIndent:
                    return string.Format(fmt, "AutoAdjustRightIndent", (bool) value);
                case ParaAttr.WidowControl:
                    return string.Format(fmt, "WidowControl", (bool) value);
                case ParaAttr.AddSpaceBetweenFarEastAndAlpha:
                    return string.Format(fmt, "AddSpaceBetweenFarEastAndAlpha", (bool) value);
                case ParaAttr.AddSpaceBetweenFarEastAndDigit:
                    return string.Format(fmt, "AddSpaceBetweenFarEastAndDigit", (bool) value);
                case ParaAttr.ListLevel:
                    return string.Format(fmt, "ListLevel", value);
                case ParaAttr.ListId:
                    return string.Format(fmt, "ListId", value);
                case ParaAttr.LineSpacing:
                    return string.Format(fmt, "LineSpacing", string.Format("({0}, {1})", ((LineSpacing)value).Rule, ((LineSpacing)value).Value));
                case ParaAttr.Bidi:
                    return string.Format(fmt, "BiDi", value);
                case ParaAttr.BaselineAlignment:
                    return string.Format(fmt, "BaselineAlignment", value);
                case ParaAttr.LeftIndent:
                    return string.Format(fmt, "LeftIndent", value);
                case ParaAttr.RightIndent:
                    return string.Format(fmt, "RightIndent", value);
                case ParaAttr.FirstLineIndent:
                    return string.Format(fmt, "FirstLineIndent", value);
                case ParaAttr.KeepTogether:
                    return string.Format(fmt, "KeepTogether", value);
                case ParaAttr.SuppressAutoHyphens:
                    return string.Format(fmt, "SuppressAutoHyphens", value);
                case ParaAttr.FrameTop:
                    return string.Format(fmt, "FrameTop", value);
                case ParaAttr.FrameLeft:
                    return string.Format(fmt, "FrameLeft", value);
                case ParaAttr.FrameVerticalAlignment:
                    return string.Format(fmt, "FrameVerticalAlignment", VerticalAlignmentToString((VerticalAlignment)value));
                case ParaAttr.FrameHorizontalAlignment:
                    return string.Format(fmt, "FrameHorizontalAlignment", HorizontalAlignmentToString((HorizontalAlignment)value));
                case ParaAttr.FrameWidth:
                    return string.Format(fmt, "FrameWidth", value);
                case ParaAttr.FrameRelativeHorizontalPosition:
                    return string.Format(fmt, "FrameRelativeHorizontalPosition",
                        RelativeHorizontalPositionToString((RelativeHorizontalPosition)value));
                case ParaAttr.FrameRelativeVerticalPosition:
                    return string.Format(fmt, "FrameRelativeVerticalPosition",
                        RelativeVerticalPositionToString((RelativeVerticalPosition)value));
                case ParaAttr.FrameWrapType:
                    return string.Format(fmt, "FrameWrapType", value);
                case ParaAttr.PageBreakBefore:
                    return string.Format(fmt, "PageBreakBefore", value);
                case ParaAttr.OutlineLevel:
                    return string.Format(fmt, "OutlineLevel", value);
                case ParaAttr.SpaceAfterAuto:
                    return string.Format(fmt, "SpaceAfterAuto", value);
                case ParaAttr.FarEastLineBreakControl:
                    return string.Format(fmt, "FarEastLineBreakControl", value);
                case ParaAttr.WordWrap:
                    return string.Format(fmt, "WordWrap", value);
                case ParaAttr.HangingPunctuation:
                    return string.Format(fmt, "HangingPunctuation", value);
                case ParaAttr.SnapToGrid:
                    return string.Format(fmt, "SnapToGrid", value);
                case ParaAttr.FrameLockAnchor:
                    return string.Format(fmt, "FrameLockAnchor", value);
                case ParaAttr.SpaceBeforeUnits:
                    return string.Format(fmt, "SpaceBeforeUnits", value);
                case ParaAttr.SpaceAfterUnits:
                    return string.Format(fmt, "SpaceAfterUnits", value);
                case ParaAttr.SpaceBeforeAuto:
                    return string.Format(fmt, "SpaceBeforeAuto", value);
                case ParaAttr.LeftIndentUnits:
                    return string.Format(fmt, "LeftIndentUnits", value);
                case ParaAttr.RightIndentUnits:
                    return string.Format(fmt, "RightIndentUnits", value);
                case ParaAttr.FirstLineIndentUnits:
                    return string.Format(fmt, "FirstLineIndentUnits", value);
                case ParaAttr.BorderLeft:
                    return string.Format(fmt, "BorderLeft", BorderToString((Border)value));
                case ParaAttr.BorderRight:
                    return string.Format(fmt, "BorderRight", BorderToString((Border)value));
                case ParaAttr.BorderTop:
                    return string.Format(fmt, "BorderTop", BorderToString((Border)value));
                case ParaAttr.BorderBottom:
                    return string.Format(fmt, "BorderBottom", BorderToString((Border)value));
                case ParaAttr.BorderBetween:
                    return string.Format(fmt, "BorderBetween", BorderToString((Border)value));
                case ParaAttr.HtmlBlockId:
                    return string.Format(fmt, "HtmlBlockId", string.Format("0x{0:x8}", value));
                    //////////////////////
                case CellAttr.Sys_CellSpan:
                    return string.Format(fmt, "_CellSpan", value);
                case CellAttr.HideMark:
                    return string.Format(fmt, "HideMark", value);
                case CellAttr.BorderBottom:
                    return string.Format(fmt, "BorderBottom", BorderToString((Border) value));
                case CellAttr.BorderTop:
                    return string.Format(fmt, "BorderTop", BorderToString((Border) value));
                case CellAttr.BorderLeft:
                    return string.Format(fmt, "BorderLeft", BorderToString((Border) value));
                case CellAttr.BorderRight:
                    return string.Format(fmt, "BorderRight", BorderToString((Border) value));
                case CellAttr.BorderDiagonalDown:
                    return string.Format(fmt, "BorderDiagonalDown", BorderToString((Border) value));
                case CellAttr.BorderDiagonalUp:
                    return string.Format(fmt, "BorderDiagonalUp", BorderToString((Border) value));
                case CellAttr.Width:
                    return string.Format(fmt, "Width", value);
                case CellAttr.PreferredWidth:
                    return string.Format(fmt, "PreferredWidth", value);
                case CellAttr.VerticalAlignment:
                    return string.Format(fmt, "VerticalAlignment", value);
                case CellAttr.WrapText:
                    return string.Format(fmt, "WrapText", value);
                case CellAttr.VerticalMerge:
                    return string.Format(fmt, "VerticalMerge", value);
                case CellAttr.HorizontalMerge:
                    return string.Format(fmt, "HorizontalMerge", value);
                case CellAttr.LeftPadding:
                    return string.Format(fmt, "LeftPadding", value);
                case CellAttr.RightPadding:
                    return string.Format(fmt, "RightPadding", value);
                case CellAttr.TopPadding:
                    return string.Format(fmt, "TopPadding", value);
                case CellAttr.BottomPadding:
                    return string.Format(fmt, "BottomPadding", value);
                case CellAttr.FitText:
                    return string.Format(fmt, "FitText", value);
                case CellAttr.Orientation:
                    return string.Format(fmt, "Orientation", value);
                case CellAttr.Shading:
                    return string.Format(fmt, "Shading", ShadingToString((Shading) value));
                case CellAttr.BorderHorizontal:
                    return string.Format(fmt, "BorderHorizontal", BorderToString((Border)value));
                case CellAttr.BorderVertical:
                    return string.Format(fmt, "BorderVertical", BorderToString((Border)value));
                    //////////////////////
                case TableAttr.HtmlBlockId:
                    return string.Format(fmt, "DivId", value);
                case TableAttr.FrameLeft:
                    return string.Format(fmt, "FrameLeft ", value);
                case TableAttr.HorizontalAlignment:
                    return string.Format(fmt, "HorizontalAlignment ", value);
                case TableAttr.Hidden:
                    return string.Format(fmt, "Hidden", value);
                case TableAttr.Shading:
                    return string.Format(fmt, "Shading", ShadingToString((Shading) value));
                case TableAttr.Istd:
                    return string.Format(fmt, "Istd", string.Format("0x{0:x2}", value));
                case TableAttr.RowHeight:
                    return string.Format(fmt, "RowHeight", HeightToString((Height)value));
                case TableAttr.BorderTop:
                    return string.Format(fmt, "BorderTop", BorderToString((Border) value));
                case TableAttr.BorderBottom:
                    return string.Format(fmt, "BorderBottom", BorderToString((Border) value));
                case TableAttr.BorderLeft:
                    return string.Format(fmt, "BorderLeft", BorderToString((Border) value));
                case TableAttr.BorderRight:
                    return string.Format(fmt, "BorderRight", BorderToString((Border) value));
                case TableAttr.LeftPadding:
                    return string.Format(fmt, "LeftPadding", (int) value);
                case TableAttr.TopPadding:
                    return string.Format(fmt, "TopPadding", (int) value);
                case TableAttr.RightPadding:
                    return string.Format(fmt, "RightPadding", (int) value);
                case TableAttr.BottomPadding:
                    return string.Format(fmt, "BottomPadding", (int) value);
                case TableAttr.LeftIndent:
                    return string.Format(fmt, "LeftIndent", (int) value);
                case TableAttr.WidthBefore:
                    return string.Format(fmt, "WidthBefore", (PreferredWidth) value);
                case TableAttr.WidthAfter:
                    return string.Format(fmt, "WidthAfter", (PreferredWidth) value);
                case TableAttr.BorderHorizontal:
                    return string.Format(fmt, "BorderHorizontal", BorderToString((Border) value));
                case TableAttr.BorderVertical:
                    return string.Format(fmt, "BorderVertical", BorderToString((Border) value));
                case TableAttr.AllowBreakAcrossPages:
                    return string.Format(fmt, "AllowBreakAcrossPages", value);
                case TableAttr.StyleOptions:
                    return string.Format(fmt, "StyleOptions", value);
                case TableAttr.PreferredWidth:
                    return string.Format(fmt, "PreferredWidth", value);
                case TableAttr.AllowAutoFit:
                    return string.Format(fmt, "AllowAutoFit", value);
                case TableAttr.HeadingFormat:
                    return string.Format(fmt, "HeadingFormat", value);
                case TableAttr.RsidTr:
                    return string.Format(fmt, "RsidTr", string.Format("0x{0:x8}", value));
                case TableAttr.Alignment:
                    return string.Format(fmt, "Alignment", value);
                case TableAttr.CellSpacing:
                    return string.Format(fmt, "CellSpacing", value);
                case TableAttr.RelativeHorizontalPosition:
                    return string.Format(fmt, "RelativeHorizontalPosition",
                        RelativeHorizontalPositionToString((RelativeHorizontalPosition)value));
                case TableAttr.RelativeVerticalPosition:
                    return string.Format(fmt, "RelativeVerticalPosition",
                        RelativeVerticalPositionToString((RelativeVerticalPosition)value));
                case TableAttr.VerticalAlignment:
                    return string.Format(fmt, "VerticalAlignment", VerticalAlignmentToString((VerticalAlignment)value));
                case TableAttr.FrameDistanceFromLeft:
                    return string.Format(fmt, "FrameDistanceFromLeft", value);
                case TableAttr.FrameDistanceFromRight:
                    return string.Format(fmt, "FrameDistanceFromRight", value);
                case TableAttr.AllowOverlap:
                    return string.Format(fmt, "AllowOverlap", value);
                case TableAttr.Bidi:
                    return string.Format(fmt, "Bidi", value);
                case TableAttr.FrameTop:
                    return string.Format(fmt, "FrameTop", value);
                case TableAttr.StyleRowBandSize:
                    return string.Format(fmt, "StyleRowBandSize", value);
                case TableAttr.StyleColBandSize:
                    return string.Format(fmt, "StyleColBandSize", value);
                case TableAttr.FrameDistanceFromTop:
                    return string.Format(fmt, "FrameDistanceFromTop", value);
                case TableAttr.FrameDistanceFromBottom:
                    return string.Format(fmt, "FrameDistanceFromBottom", value);
                case TableAttr.Sys_CalculatedTableGrid:
                    return string.Format(fmt, "_CalculatedTableGrid", "TODO");
                    //////////////////////
                case SectAttr.RtlGutter:
                    return string.Format(fmt, "RtlGutter", value);
                case SectAttr.ColumnsEvenlySpaced:
                    return string.Format(fmt, "ColumnsEvenlySpaced", value);
                case SectAttr.Columns:
                    return string.Format(fmt, "Columns", TextColumnCollectionInternalToString((TextColumnCollectionInternal)value));
                case SectAttr.Sys_GprfIhdt:
                    return string.Format(fmt, "_GprfIhdt", value);
                case SectAttr.LinePitch:
                    return string.Format(fmt, "LinePitch", value);
                case SectAttr.PageWidth:
                    return string.Format(fmt, "PageWidth", value);
                case SectAttr.PageHeight:
                    return string.Format(fmt, "PageHeight", value);
                case SectAttr.LeftMargin:
                    return string.Format(fmt, "LeftMargin", value);
                case SectAttr.RightMargin:
                    return string.Format(fmt, "RightMargin", value);
                case SectAttr.Gutter:
                    return string.Format(fmt, "Gutter", value);
                case SectAttr.HeaderDistance:
                    return string.Format(fmt, "HeaderDistance", value);
                case SectAttr.FooterDistance:
                    return string.Format(fmt, "FooterDistance", value);
                case SectAttr.VerticalAlignment:
                    return string.Format(fmt, "VerticalAlignment", value);
                case SectAttr.ColumnsCount:
                    return string.Format(fmt, "ColumnsCount", value);
                case SectAttr.TopMargin:
                    return string.Format(fmt, "TopMargin", value);
                case SectAttr.BottomMargin:
                    return string.Format(fmt, "BottomMargin", value);
                case SectAttr.ColumnsSpacing:
                    return string.Format(fmt, "ColumnsSpacing", value);
                case SectAttr.SectionStart:
                    return string.Format(fmt, "SectionStart", value);
                case SectAttr.FirstPageTray:
                    return string.Format(fmt, "FirstPageTray", value);
                case SectAttr.OtherPagesTray:
                    return string.Format(fmt, "OtherPagesTray", value);
                case SectAttr.BorderAlwaysInFront:
                    return string.Format(fmt, "BorderAlwaysInFront", value);
                case SectAttr.BorderDistanceFrom:
                    return string.Format(fmt, "BorderDistanceFrom", value);
                case SectAttr.Rsid:
                    return string.Format(fmt, "Rsid", string.Format("0x{0:x8}", value));
                case SectAttr.FootnoteLocation:
                    return string.Format(fmt, "FootnoteLocation", value);
                case SectAttr.FootnoteNumberingRule:
                    return string.Format(fmt, "FootnoteNumberingRule", value);
                case SectAttr.FootnoteStartNumber:
                    return string.Format(fmt, "FootnoteStartNumber", value);
                case SectAttr.BorderAppliesTo:
                    return string.Format(fmt, "BorderAppliesTo", value);
                case SectAttr.FootnoteNumberStyle:
                    return string.Format(fmt, "FootnoteNumberStyle", value);
                case SectAttr.EndnoteNumberingRule:
                    return string.Format(fmt, "EndnoteNumberingRule", value);
                case SectAttr.EndnoteStartNumber:
                    return string.Format(fmt, "EndnoteStartNumber", value);
                case SectAttr.EndnoteNumberStyle:
                    return string.Format(fmt, "EndnoteNumberStyle", value);
                case SectAttr.EndnoteLocation:
                    return string.Format(fmt, "EndnoteLocation", value);
                case SectAttr.PaperCode:
                    return string.Format(fmt, "PaperCode", value);
                case SectAttr.SuppressEndnotes:
                    return string.Format(fmt, "SuppressEndnotes", value);
                case SectAttr.PageStartingNumber:
                    return string.Format(fmt, "PageStartingNumber", value);
                case SectAttr.DifferentFirstPageHeaderFooter:
                    return string.Format(fmt, "DifferentFirstPageHeaderFooter", value);
                case SectAttr.RestartPageNumbering:
                    return string.Format(fmt, "RestartPageNumbering", value);
                case SectAttr.LineNumberDistanceFromText:
                    return string.Format(fmt, "LineNumberDistanceFromText", value);
                case SectAttr.GridType:
                    return string.Format(fmt, "GridType", value);
                case SectAttr.Bidi:
                    return string.Format(fmt, "Bidi", value);
                case SectAttr.TextFlow:
                    return string.Format(fmt, "TextFlow", value);
                case SectAttr.Orientation:
                    return string.Format(fmt, "Orientation", value);
                case SectAttr.BorderTop:
                    return string.Format(fmt, "BorderTop", BorderToString((Border) value));
                case SectAttr.BorderBottom:
                    return string.Format(fmt, "BorderBottom", BorderToString((Border) value));
                case SectAttr.BorderRight:
                    return string.Format(fmt, "BorderRight", BorderToString((Border) value));
                case SectAttr.BorderLeft:
                    return string.Format(fmt, "BorderLeft", BorderToString((Border) value));
                case SectAttr.PageNumberStyle:
                    return string.Format(fmt, "PageNumberStyle", value);
                case SectAttr.CharSpace:
                    return string.Format(fmt, "CharsLine", value);
                case SectAttr.Unlocked:
                    return string.Format(fmt, "Unlocked", value);
                //////////////////////
                case Forms2Attr.MultiRow:
                    return string.Format(fmt, "MultiRow", value);
                case Forms2Attr.Tooltips:
                    return string.Format(fmt, "Tooltips", value);
                case Forms2Attr.NewVersion:
                    return string.Format(fmt, "NewVersion", value);
                case Forms2Attr.ProportionalThumb:
                    return string.Format(fmt, "ProportionalThumb", value);
                case Forms2Attr.TakeFocusOnClick:
                    return string.Format(fmt, "TakeFocusOnClick", value);
                case Forms2Attr.AutoSize:
                    return string.Format(fmt, "AutoSize", value);
                case Forms2Attr.Position:
                    return string.Format(fmt, "Position", value);
                case Forms2Attr.PictureTiling:
                    return string.Format(fmt, "PictureTiling", value);
                case Forms2Attr.ID:
                    return string.Format(fmt, "ID", value);
                case Forms2Attr.LogicalSize:
                    return string.Format(fmt, "LogicalSize", value);
                case Forms2Attr.NextAvailableID:
                    return string.Format(fmt, "NextAvailableID", value);
                case Forms2Attr.BooleanProperties:
                    return string.Format(fmt, "BooleanProperties", value);
                case Forms2Attr.GroupCnt:
                    return string.Format(fmt, "GroupCnt", value);
                case Forms2Attr.Cycle:
                    return string.Format(fmt, "Cycle", value);
                case Forms2Attr.Font:
                    return string.Format(fmt, "Font", value);
                case Forms2Attr.Zoom:
                    return string.Format(fmt, "Zoom", value);
                case Forms2Attr.PictureAlignment:
                    return string.Format(fmt, "PictureAlignment", value);
                case Forms2Attr.PictureSizeMode:
                    return string.Format(fmt, "PictureSizeMode", value);
                case Forms2Attr.ShapeCookie:
                    return string.Format(fmt, "ShapeCookie", value);
                case Forms2Attr.DrawBuffer:
                    return string.Format(fmt, "DrawBuffer", value);
                case Forms2Attr.ScrollPosition:
                    return string.Format(fmt, "ScrollPosition", value);
                case Forms2Attr.HelpContextID:
                    return string.Format(fmt, "HelpContextID", value);
                case Forms2Attr.BitFlagsSite:
                    return string.Format(fmt, "BitFlagsSite", value);
                case Forms2Attr.ObjectStreamSize:
                    return string.Format(fmt, "ObjectStreamSize", value);
                case Forms2Attr.TabIndex:
                    return string.Format(fmt, "TabIndex", value);
                case Forms2Attr.ClsidCacheIndex:
                    return string.Format(fmt, "ClsidCacheIndex", value);
                case Forms2Attr.GroupID:
                    return string.Format(fmt, "GroupID", value);
                case Forms2Attr.Name:
                    return string.Format(fmt, "Name", value);
                case Forms2Attr.Tag:
                    return string.Format(fmt, "Tag", value);
                case Forms2Attr.ControlTipText:
                    return string.Format(fmt, "ControlTipText", value);
                case Forms2Attr.RuntimeLicKey:
                    return string.Format(fmt, "RuntimeLicKey", value);
                case Forms2Attr.ControlSource:
                    return string.Format(fmt, "ControlSource", value);
                case Forms2Attr.RowSource:
                    return string.Format(fmt, "RowSource", value);
                case Forms2Attr.ListIndex:
                    return string.Format(fmt, "ListIndex", value);
                case Forms2Attr.TabOrienation:
                    return string.Format(fmt, "TabOrienation", value);
                case Forms2Attr.TabStyle:
                    return string.Format(fmt, "TabStyle", value);
                case Forms2Attr.TabFixedWidth:
                    return string.Format(fmt, "TabFixedWidth", value);
                case Forms2Attr.TabFixedHeight:
                    return string.Format(fmt, "TabFixedHeight", value);
                case Forms2Attr.TabsAllocated:
                    return string.Format(fmt, "TabsAllocated", value);
                case Forms2Attr.TabData:
                    return string.Format(fmt, "TabData", value);
                case Forms2Attr.ForegroundColor:
                    return string.Format(fmt, "ForegroundColor", value);
                case Forms2Attr.BackgroundColor:
                    return string.Format(fmt, "BackgroundColor", value);
                case Forms2Attr.VariousPropertyBits:
                    return string.Format(fmt, "VariousPropertyBits", value);
                case Forms2Attr.Size:
                    return string.Format(fmt, "Size (Displayed size)", value);
                case Forms2Attr.Caption:
                    return string.Format(fmt, "Caption", string.Format("'{0}'", value));
                case Forms2Attr.BorderStyle:
                    return string.Format(fmt, "BorderStyle", value);
                case Forms2Attr.Value:
                    return string.Format(fmt, "Value", string.Format("'{0}'", value));
                case Forms2Attr.PicturePosition:
                    return string.Format(fmt, "PicturePosition", value);
                case Forms2Attr.MousePointer:
                    return string.Format(fmt, "MousePointer", value);
                case Forms2Attr.BorderColor:
                    return string.Format(fmt, "BorderColor", value);
                case Forms2Attr.SpecialEffect:
                    return string.Format(fmt, "SpecialEffect", value);
                case Forms2Attr.Picture:
                    return string.Format(fmt, "Picture", value);
                case Forms2Attr.Accelerator:
                    return string.Format(fmt, "Accelerator", value);
                case Forms2Attr.MouseIcon:
                    return string.Format(fmt, "MouseIcon", value);
                case Forms2Attr.MaxLength:
                    return string.Format(fmt, "MaxLength", value);
                case Forms2Attr.ScrollBars:
                    return string.Format(fmt, "ScrollBars", value);
                case Forms2Attr.DisplayStyle:
                    return string.Format(fmt, "DisplayStyle", value);
                case Forms2Attr.PasswordChar:
                    return string.Format(fmt, "PasswordChar", value);
                case Forms2Attr.ListWidth:
                    return string.Format(fmt, "ListWidth", value);
                case Forms2Attr.BoundColumn:
                    return string.Format(fmt, "BoundColumn", value);
                case Forms2Attr.TextColumn:
                    return string.Format(fmt, "TextColumn", value);
                case Forms2Attr.ColumnCount:
                    return string.Format(fmt, "ColumnCount", value);
                case Forms2Attr.ListRows:
                    return string.Format(fmt, "ListRows", value);
                case Forms2Attr.ColumnInfo:
                    return string.Format(fmt, "ColumnInfo", value);
                case Forms2Attr.MatchEntry:
                    return string.Format(fmt, "MatchEntry", value);
                case Forms2Attr.ListStyle:
                    return string.Format(fmt, "ListStyle", value);
                case Forms2Attr.ShowDropButtonWhen:
                    return string.Format(fmt, "ShowDropButtonWhen", value);
                case Forms2Attr.DropButtonStyle:
                    return string.Format(fmt, "DropButtonStyle", value);
                case Forms2Attr.MultiSelect:
                    return string.Format(fmt, "MultiSelect", value);
                case Forms2Attr.Min:
                    return string.Format(fmt, "Min", value);
                case Forms2Attr.Max:
                    return string.Format(fmt, "Max", value);

                case Forms2Attr.SitePosition:
                    return string.Format(fmt, "SitePosition", value);
                case Forms2Attr.PrevEnabled:
                    return string.Format(fmt, "PrevEnabled", value);
                case Forms2Attr.NextEnabled:
                    return string.Format(fmt, "NextEnabled", value);
                case Forms2Attr.SmallChange:
                    return string.Format(fmt, "SmallChange", value);
                case Forms2Attr.Orientation:
                    return string.Format(fmt, "Orientation", value);
                case Forms2Attr.Delay:
                    return string.Format(fmt, "Delay", value);

                case Forms2Attr.GroupName:
                    return string.Format(fmt, "GroupName", string.Format("'{0}'", value));

                default: return string.Format(fmt, key, value);
            }
        }

        private static string RubyToString(Ruby ruby)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("('{0}', '{1}', Base: {2}, Top: {3}, Distance: {4}, {5}, {6})",
                            ruby.Top.Text,
                            ruby.Base.Text,
                            ruby.BaseSize,
                            ruby.TopSize,
                            ruby.Distance,
                            ruby.Alignment,
                            ruby.Language);

            return sb.ToString();
        }

        private static string FarEastLayoutToString(FarEastLayout feLayout)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("({0}{1}{2}FarEastLayoutId: {3}, CombineBrackets: {4})",
                feLayout.Combine ? "Combine, " : "",
                feLayout.Vertical ? "Vertical, " : "",
                feLayout.VerticalCompress ? "VerticalCompress, " : "",
                feLayout.FarEastLayoutId,
                feLayout.CombineBrackets);

            return sb.ToString();
        }

        private static string TextColumnCollectionInternalToString(TextColumnCollectionInternal columns)
        {
            if (columns.Count == 0)
                return "(empty)";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                TextColumn col = columns[i];
                sb.AppendFormat("(Width: {0}, Space: {1}) ", col.Width, col.SpaceAfter);
            }

            return sb.ToString();
        }

        private static string DmlShapeEffectToString(object effect)
        {
            if (effect is DmlNullEffect)
                return "NullEffect";

            return effect.ToString();
        }

        private static string DmlShapeEffectToString(DmlShapeEffect effect)
        {
            string alpha;
            switch (effect.EffectType)
            {
                case DmlShapeEffectType.PresetShadow:
                    DmlShapePresetShadowEffect preset = (DmlShapePresetShadowEffect)effect;
                    alpha = (preset.Color.Alpha != null) ? System.Math.Round(preset.Color.Alpha.Value, 1).ToString() : "null";
                    return string.Format("(PresetShadow: {0}, Direction: {1:0}, Distance: {2:0}, Color.Alpha: {3})",
                        preset.PresetShadow.ToString(), preset.Direction.Value, preset.Distance, alpha);
                case DmlShapeEffectType.OuterShadow:
                    DmlShapeOuterShadowEffect outer = (DmlShapeOuterShadowEffect)effect;
                    alpha = (outer.Color.Alpha != null) ? System.Math.Round(outer.Color.Alpha.Value, 1).ToString() : "null";
                    string colorStr = outer.Color.ColorType == DmlColorType.PresetColor
                        ? ((DmlPresetColor)outer.Color).Value
                        : ((DmlHexRgbColor)outer.Color).Value;
                    return string.Format("(Color: {0}, BlurRadius: {1:0}, Direction: {2:0}, Distance: {3:0}, Alignment: {4}, " +
                        "HorizontalSkew: {5}, VerticalSkew: {6}, HorizontalScale: {7:0.00}, VerticalScale: {8:0.00}, " +
                        "RotateWithShape: {9}, Color.Alpha: {10})", colorStr, outer.BlurRadius,
                        outer.Direction.Value, outer.Distance, outer.Alignment.ToString(), outer.HorizontalSkew.Value,
                        outer.VerticalSkew.Value, outer.HorizontalScale, outer.VerticalScale, outer.RotateWithShape, alpha);
                case DmlShapeEffectType.InnerShadow:
                    DmlShapeInnerShadowEffect inner = (DmlShapeInnerShadowEffect)effect;
                    alpha = (inner.Color.Alpha != null) ? System.Math.Round(inner.Color.Alpha.Value, 1).ToString() : "null";
                    return string.Format(
                        "(Color: {0}, BlurRadius: {1:0}, Direction: {2:0}, Distance: {3:0}, Color.Alpha: {4}",
                        ((DmlPresetColor)inner.Color).Value, inner.BlurRadius, inner.Direction.Value, inner.Distance, alpha);
            }

            return effect.EffectType.ToString();
        }

        private static string PathRectangeArrayToString(PathRectangle[] pathRectangles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (PathRectangle rect in pathRectangles)
                sb.AppendFormat("(Left:{0}, Top:{1}, Right:{2}, Bottom:{3}) ", rect.Left, rect.Top, rect.Right, rect.Bottom);

            return sb.ToString();
        }

        private static string ConnectorRuleToString(ConnectorRule rule)
        {
            return string.Format("({0}:{1} -> {2}:{3})", rule.ShapeAId, rule.ShapeASite, rule.ShapeBId, rule.ShapeBSite);
        }

        private static string HeightToString(Height height)
        {
            return height.ToString();
        }

        private static string GradientColorToString(GradientColor[] gradients)
        {
            StringBuilder sb = new StringBuilder();

            foreach (GradientColor gradient in gradients)
                sb.AppendFormat("({0}:{1:x8})", gradient.Start, gradient.Color.ToArgb());

            return RemoveTrailingComma(sb);
        }

        private static string FormatShapeAttr(int key, object value)
        {
            const string fmt = "  {{Shape.{0}: {1}}}";

            switch (key)
            {
                case ShapeAttr.FillAngle:
                    return string.Format(fmt, "FillAngle", value);
                case ShapeAttr.FillFocus:
                    return string.Format(fmt, "FillFocus", value);
                case ShapeAttr.LineOpacity:
                    return string.Format(fmt, "LineOpacity", value);
                case ShapeAttr.FillBackOpacity:
                    return string.Format(fmt, "FillBackOpacity", value);
                case ShapeAttr.FillShadeColors:
                    return string.Format(fmt, "FillShadeColors", GradientColorToString((GradientColor[])value));
                case ShapeAttr.ImageDblCrMod:
                    return string.Format(fmt, "ImageDblCrMod", value);
                case ShapeAttr.ImageSourceFullName:
                    return string.Format(fmt, "ImageSourceFullName", value);
                case ShapeAttr.GraphicFrameExtWidth:
                    return string.Format(fmt, "GraphicFrameExtWidth", value);
                case ShapeAttr.GraphicFrameExtHeight:
                    return string.Format(fmt, "GraphicFrameExtHeight", value);
                case ShapeAttr.CoordOriginX:
                    return string.Format(fmt, "CoordOriginX", value);
                case ShapeAttr.CoordOriginY:
                    return string.Format(fmt, "CoordOriginY", value);
                case ShapeAttr.LeftPercent:
                    return string.Format(fmt, "LeftPercent", value);
                case ShapeAttr.TopPercent:
                    return string.Format(fmt, "TopPercent", value);
                case ShapeAttr.OleSourceFullName:
                    return string.Format(fmt, "OleSourceFullName", value);
                case ShapeAttr.OleLocked:
                    return string.Format(fmt, "OleLocked", value);
                case ShapeAttr.OleLinkType:
                    return string.Format(fmt, "OleLinkType", value);
                case ShapeAttr.OleFormatUpdateType:
                    return string.Format(fmt, "OleFormatUpdateType", value);
                case ShapeAttr.ConnectorRule:
                    return string.Format(fmt, "ConnectorRule", ConnectorRuleToString((ConnectorRule)value));
                case ShapeAttr.GeometryVertices:
                    return string.Format(fmt, "GeometryVertices", PathPointArrayToString((PathPoint[])value));
                case ShapeAttr.GeometrySegmentInfo:
                    return string.Format(fmt, "GeometrySegmentInfo", PathInfoArrayToString((PathInfo[])value));
                case ShapeAttr.EditAs:
                    return string.Format(fmt, "EditAs", value);
                case ShapeAttr.DiagramStyle:
                    return string.Format(fmt, "DiagramStyle", value);
                case ShapeAttr.DiagramRelationsTable:
                    return string.Format(fmt, "DiagramRelationsTable", value);
                case ShapeAttr.DiagramScaleX:
                    return string.Format(fmt, "DiagramScaleX", value);
                case ShapeAttr.DiagramScaleY:
                    return string.Format(fmt, "DiagramScaleY", value);
                case ShapeAttr.ShadowHighlight:
                    return string.Format(fmt, "ShadowHighlight", value);
                case ShapeAttr.ShadowSecondOffsetX:
                    return string.Format(fmt, "ShadowSecondOffsetX", value);
                case ShapeAttr.ShadowSecondOffsetY:
                    return string.Format(fmt, "ShadowSecondOffsetY", value);
                case ShapeAttr.ShadowScaleXtoX:
                    return string.Format(fmt, "ShadowScaleXtoX", value);
                case ShapeAttr.ShadowScaleXtoY:
                    return string.Format(fmt, "ShadowScaleXtoY", value);
                case ShapeAttr.ShadowScaleYtoX:
                    return string.Format(fmt, "ShadowScaleYtoX", value);
                case ShapeAttr.ShadowScaleYtoY:
                    return string.Format(fmt, "ShadowScaleYtoY", value);
                case ShapeAttr.ShadowOriginX:
                    return string.Format(fmt, "ShadowOriginX", value);
                case ShapeAttr.ShadowPerspectiveX:
                    return string.Format(fmt, "ShadowPerspectiveX", value);
                case ShapeAttr.ShadowPerspectiveY:
                    return string.Format(fmt, "ShadowPerspectiveY", value);
                case ShapeAttr.TextboxFitShapeToText:
                    return string.Format(fmt, "TextboxFitShapeToText", value);
                case ShapeAttr.ShadowType:
                    return string.Format(fmt, "ShadowTypeCore", value);
                case ShapeAttr.ShadowOpacity:
                    return string.Format(fmt, "ShadowOpacity", value);
                case ShapeAttr.ShadowOffsetX:
                    return string.Format(fmt, "ShadowOffsetX", value);
                case ShapeAttr.ShadowOffsetY:
                    return string.Format(fmt, "ShadowOffsetY", value);
                case ShapeAttr.ShadowOriginY:
                    return string.Format(fmt, "ShadowOriginY", value);
                case ShapeAttr.WidthPercent:
                    return string.Format(fmt, "WidthPercent", value);
                case ShapeAttr.HeightPercent:
                    return string.Format(fmt, "HeightPercent", value);
                case ShapeAttr.RelativeWidth:
                    return string.Format(fmt, "RelativeWidth", value);
                case ShapeAttr.RelativeHeight:
                    return string.Format(fmt, "RelativeHeight", value);
                case ShapeAttr.TextboxLayoutFlow:
                    return string.Format(fmt, "TextboxLayoutFlow", value);
                case ShapeAttr.LineStyle:
                    return string.Format(fmt, "LineStyle", value);
                case ShapeAttr.OleAutoUpdate:
                    return string.Format(fmt, "OleAutoUpdate", value);
                case ShapeAttr.LineEndArrowWidth:
                    return string.Format(fmt, "LineEndArrowWidth", value);
                case ShapeAttr.LineEndArrowLength:
                    return string.Format(fmt, "LineEndArrowLength", value);
                case ShapeAttr.TextboxAnchor:
                    return string.Format(fmt, "TextboxAnchor", value);
                case ShapeAttr.TextboxWrapMode:
                    return string.Format(fmt, "TextboxWrapMode", value);
                case ShapeAttr.ImageCropTop:
                    return string.Format(fmt, "ImageCropTop", CropToString((int)value));
                case ShapeAttr.ImageCropLeft:
                    return string.Format(fmt, "ImageCropLeft", CropToString((int)value));
                case ShapeAttr.ImageCropRight:
                    return string.Format(fmt, "ImageCropRight", CropToString((int)value));
                case ShapeAttr.ImageCropBottom:
                    return string.Format(fmt, "ImageCropBottom", CropToString((int)value));
                case ShapeAttr.FillBackColor:
                    return string.Format(fmt, "FillBackColor", value);
                case ShapeAttr.LineBackColor:
                    return string.Format(fmt, "LineBackColor", value);
                case ShapeAttr.LineJoinStyle:
                    return string.Format(fmt, "LineJoinStyle", value);

                case ShapeAttr.TextboxRotateText:
                    return string.Format(fmt, "TextboxRotateText", value);
                case ShapeAttr.HyperlinkAddress:
                    return string.Format(fmt, "HyperlinkAddress", value);
                case ShapeAttr.ScreenTip:
                    return string.Format(fmt, "ScreenTip", value);
                case ShapeAttr.GeometryConnectionSiteType:
                    return string.Format(fmt, "GeometryConnectionSiteType", value);
                case ShapeAttr.GeometryConnectLocs:
                    return string.Format(fmt, "GeometryConnectLocs", PathPointArrayToString((PathPoint[])value));
                case ShapeAttr.GeometryHandles:
                    return string.Format(fmt, "GeometryHandles", value);
                case ShapeAttr.GeometryFormulas:
                    return string.Format(fmt, "GeometryFormulas", value);
                case ShapeAttr.GeometryAdjust1:
                case ShapeAttr.GeometryAdjust2:
                case ShapeAttr.GeometryAdjust3:
                case ShapeAttr.GeometryAdjust4:
                case ShapeAttr.GeometryAdjust5:
                case ShapeAttr.GeometryAdjust6:
                case ShapeAttr.GeometryAdjust7:
                case ShapeAttr.GeometryAdjust8:
                case ShapeAttr.GeometryAdjust9:
                case ShapeAttr.GeometryAdjust10:
                    return string.Format(fmt, string.Format("GeometryAdjust{0}", key - ShapeAttr.GeometryAdjust1 + 1), value);
                case ShapeAttr.GeometryPathTextBoxRects:
                    return string.Format(fmt, "GeometryPathTextBoxRects", PathRectangeArrayToString((PathRectangle[])value));
                case ShapeAttr.ReallyHidden:
                    return string.Format(fmt, "ReallyHidden", value);
                case ShapeAttr.ScriptAnchor:
                    return string.Format(fmt, "ScriptAnchor", value);
                case ShapeAttr.ImageGrayScale:
                    return string.Format(fmt, "ImageGrayScale", value);
                case ShapeAttr.ImageBiLevel:
                    return string.Format(fmt, "ImageBiLevel", value);
                case ShapeAttr.FillColor:
                    return string.Format(fmt, "FillColor", value);
                case ShapeAttr.FillHitTest:
                    return string.Format(fmt, "FillHitTest", value);
                case ShapeAttr.FillShape:
                    return string.Format(fmt, "FillShape", value);
                case ShapeAttr.FillUseRect:
                    return string.Format(fmt, "FillUseRect", value);
                case ShapeAttr.FillNoFillHitTest:
                    return string.Format(fmt, "FillNoFillHitTest", value);
                case ShapeAttr.LineHitTest:
                    return string.Format(fmt, "LineHitTest", value);
                case ShapeAttr.LineFillShape:
                    return string.Format(fmt, "LineFillShape", value);
                case ShapeAttr.LineNoLineDrawDash:
                    return string.Format(fmt, "LineNoLineDrawDash", value);
                case ShapeAttr.ShadowOn:
                    return string.Format(fmt, "ShadowOn", value);
                case ShapeAttr.ShadowObscured:
                    return string.Format(fmt, "ShadowObscured", value);
                case ShapeAttr.OleIcon:
                    return string.Format(fmt, "OleIcon", value);
                case ShapeAttr.WrapPolygonVertices:
                    return string.Format(fmt, "WrapPolygonVertices", value);
                case ShapeAttr.DistanceLeft:
                    return string.Format(fmt, "DistanceLeft", value);
                case ShapeAttr.DistanceTop:
                    return string.Format(fmt, "DistanceTop", value);
                case ShapeAttr.DistanceRight:
                    return string.Format(fmt, "DistanceRight", value);
                case ShapeAttr.DistanceBottom:
                    return string.Format(fmt, "DistanceBottom", value);
                case ShapeAttr.Print:
                    return string.Format(fmt, "Print", value);
                case ShapeAttr.OneD:
                    return string.Format(fmt, "OneD", value);
                case ShapeAttr.Hidden:
                    return string.Format(fmt, "Hidden", value);
                case ShapeAttr.Button:
                    return string.Format(fmt, "Button", value);
                case ShapeAttr.EditedWrap:
                    return string.Format(fmt, "EditedWrap", value);
                case ShapeAttr.AllowInCell:
                    return string.Format(fmt, "AllowInCell", value);
                case ShapeAttr.OnDblClickNotify:
                    return string.Format(fmt, "OnDblClickNotify", value);
                case ShapeAttr.OleObject:
                    return string.Format(fmt, "OleObject", EmbeddedObjectToString((IEmbeddedObject)value));
                case ShapeAttr.OleProgID:
                    return string.Format(fmt, "OleProgID", value);
                case ShapeAttr.ImageActive:
                    return string.Format(fmt, "ImageActive", value);
                case ShapeAttr.PreferRelativeResize:
                    return string.Format(fmt, "PreferRelativeResize", value);
                case ShapeAttr.UnknownHtmlFlags:
                    return string.Format(fmt, "UnknownHtmlFlags", value);
                case ShapeAttr.LineFillPresetTexture:
                    return string.Format(fmt, "LineFillPresetTexture", value);
                case ShapeAttr.FillOpacity:
                    return string.Format(fmt, "FillOpacity", value);
                case ShapeAttr.FillRecolorAsPicture:
                    return string.Format(fmt, "FillRecolorAsPicture", value);
                case ShapeAttr.FillUseShapeAnchor:
                    return string.Format(fmt, "FillUseShapeAnchor", value);
                case ShapeAttr.LineDashStyle:
                    return string.Format(fmt, "LineDashStyle", value);
                case ShapeAttr.LineEndCapStyle:
                    return string.Format(fmt, "LineEndCapStyle", value);
                case ShapeAttr.LineFillType:
                    return string.Format(fmt, "LineFillType", value);
                case ShapeAttr.LineFillBlipName:
                    return string.Format(fmt, "LineFillBlipName", value);
                case ShapeAttr.LineStartArrow:
                    return string.Format(fmt, "LineStartArrow", value);
                case ShapeAttr.LineEndArrow:
                    return string.Format(fmt, "LineEndArrow", value);
                case ShapeAttr.ConnectorType:
                    return string.Format(fmt, "ConnectorType", value);
                case ShapeAttr.FillBlipNameFlags:
                    return string.Format(fmt, "FillBlipNameFlags", value);
                case ShapeAttr.AllowOverlap:
                    return string.Format(fmt, "AllowOverlap", value);
                case ShapeAttr.FillPresetTexture:
                    return string.Format(fmt, "FillPresetTexture", value);
                case ShapeAttr.FillType:
                    return string.Format(fmt, "FillType", value);
                case ShapeAttr.LockPosition:
                    return string.Format(fmt, "LockPosition", value);
                case ShapeAttr.FillBlipName:
                    return string.Format(fmt, "FillBlipName", value);
                case ShapeAttr.ShapeName:
                    return string.Format(fmt, "ShapeName", value);
                case ShapeAttr.TextboxLeft:
                    return string.Format(fmt, "TextboxLeft", value);
                case ShapeAttr.TextboxTop:
                    return string.Format(fmt, "TextboxTop", value);
                case ShapeAttr.TextboxRight:
                    return string.Format(fmt, "TextboxRight", value);
                case ShapeAttr.TextboxBottom:
                    return string.Format(fmt, "TextboxBottom", value);
                case ShapeAttr.TextboxNextShapeId:
                    return string.Format(fmt, "TextboxNextShapeId", value);
                case ShapeAttr.Sys_TextboxNextShapeIdRaw:
                    return string.Format(fmt, "_TextboxNextShapeIdRaw", value);
                case ShapeAttr.LineColor:
                    return string.Format(fmt, "LineColor", value);
                case ShapeAttr.LineWidth:
                    return string.Format(fmt, "LineWidth", value);
                case ShapeAttr.GeometryFillOK:
                    return string.Format(fmt, "GeometryFillOK", value);
                case ShapeAttr.Filled:
                    return string.Format(fmt, "Filled", value);
                case ShapeAttr.LineArrowHeadsOK:
                    return string.Format(fmt, "LineArrowHeadsOK", value);
                case ShapeAttr.BehindText:
                    return string.Format(fmt, "BehindText", value);
                case ShapeAttr.WrapSide:
                    return string.Format(fmt, "WrapSide", value);
                case ShapeAttr.AnchorLocked:
                    return string.Format(fmt, "AnchorLocked", value);
                case ShapeAttr.ZOrder:
                    return string.Format(fmt, "ZOrder", string.Format("0x{0:x8} ({0})", value));
                case ShapeAttr.RelativeHorizontalPosition:
                    return string.Format(fmt, "RelativeHorizontalPosition", value);
                case ShapeAttr.LockRotation:
                    return string.Format(fmt, "LockRotation", value);
                case ShapeAttr.LockAspectRatio:
                    return string.Format(fmt, "LockAspectRatio", value);
                case ShapeAttr.LockCropping:
                    return string.Format(fmt, "LockCropping", value);
                case ShapeAttr.LockAgainstGrouping:
                    return string.Format(fmt, "LockAgainstGrouping", value);
                case ShapeAttr.ShapeDescription:
                    return string.Format(fmt, "ShapeDescription", value);
                case ShapeAttr.LockVertices:
                    return string.Format(fmt, "LockVertices", value);
                case ShapeAttr.LockText:
                    return string.Format(fmt, "LockText", value);
                case ShapeAttr.LockAdjustHandles:
                    return string.Format(fmt, "LockAdjustHandles", value);
                case ShapeAttr.SigSetupId:
                    return string.Format(fmt, "SigSetupId", value);
                case ShapeAttr.SigSetupProvId:
                    return string.Format(fmt, "SigSetupProvId", value);
                case ShapeAttr.SigSetupSuggSigner:
                    return string.Format(fmt, "SigSetupSuggSigner", value);
                case ShapeAttr.SigSetupSuggSigner2:
                    return string.Format(fmt, "SigSetupSuggSigner2", value);
                case ShapeAttr.SigSetupAddlXml:
                    return string.Format(fmt, "SigSetupAddlXml", value);
                case ShapeAttr.SigSetupProvUrl:
                    return string.Format(fmt, "SigSetupProvUrl", value);
                case ShapeAttr.SigSetupShowSignDate:
                    return string.Format(fmt, "SigSetupShowSignDate", value);
                case ShapeAttr.SigSetupSignInst:
                    return string.Format(fmt, "SigSetupSignInst", value);
                case ShapeAttr.SigSetupSuggSignerEmail:
                    return string.Format(fmt, "SigSetupSuggSignerEmail", value);
                case ShapeAttr.SigSetupAllowComments:
                    return string.Format(fmt, "SigSetupAllowComments", value);
                case ShapeAttr.SigSetupSignInstSet:
                    return string.Format(fmt, "SigSetupSignInstSet", value);
                case ShapeAttr.IsSignatureLine:
                    return string.Format(fmt, "IsSignatureLine", value);
                case ShapeAttr.RelativeVerticalPosition:
                    return string.Format(fmt, "RelativeVerticalPosition", value);
                case ShapeAttr.Flip:
                    return string.Format(fmt, "Flip", value);
                case ShapeAttr.WrapType:
                    return string.Format(fmt, "WrapType", value);
                case ShapeAttr.ImageBytes:
                    return string.Format(fmt, "ImageBytes", ImageBytesToString((byte[])value));
                case ShapeAttr.InkData:
                    return string.Format(fmt, "InkData", ByteArrayToString((byte[]) value));
                case ShapeAttr.FillImageBytes:
                    return string.Format(fmt, "FillImageBytes", ImageBytesToString((byte[])value));
                case ShapeAttr.LineImageBytes:
                    return string.Format(fmt, "LineImageBytes", ImageBytesToString((byte[])value));
                case ShapeAttr.BorderTop:
                    return string.Format(fmt, "BorderTop", BorderToString((Border) value));
                case ShapeAttr.BorderBottom:
                    return string.Format(fmt, "BorderBottom", BorderToString((Border) value));
                case ShapeAttr.BorderLeft:
                    return string.Format(fmt, "BorderLeft", BorderToString((Border) value));
                case ShapeAttr.BorderRight:
                    return string.Format(fmt, "BorderRight", BorderToString((Border) value));
                case ShapeAttr.LineOn:
                    return string.Format(fmt, "LineOn", value);
                case ShapeAttr.ImageTitle:
                    return string.Format(fmt, "ImageTitle", value);
                case ShapeAttr.ShapeId:
                    return string.Format(fmt, "ShapeId", value);
                case ShapeAttr.Width:
                    return string.Format(fmt, "Width", System.Math.Round((double)value, 2));
                case ShapeAttr.Height:
                    return string.Format(fmt, "Height", System.Math.Round((double)value, 2));
                case ShapeAttr.Sys_LayoutWidth:
                    return string.Format(fmt, "_LayoutWidth", System.Math.Round((double)value, 2));
                case ShapeAttr.Sys_LayoutHeight:
                    return string.Format(fmt, "_LayoutHeight", System.Math.Round((double)value, 2));
                case ShapeAttr.DiagramNodeLayout:
                    return string.Format(fmt, "DiagramNodeLayout", value);
                case ShapeAttr.DiagramNodeKind:
                    return string.Format(fmt, "DiagramNodeKind", value);
                case ShapeAttr.ShapeType:
                    return string.Format(fmt, "ShapeType", string.Format("{0} ({1})", value, (ShapeType)value));
                case ShapeAttr.PseudoInline:
                    return string.Format(fmt, "PseudoInline", value);
                case ShapeAttr.Left:
                    return string.Format(fmt, "Left", value);
                case ShapeAttr.Top:
                    return string.Format(fmt, "Top", value);
                case ShapeAttr.DmlEffectExtentLeft:
                    return string.Format(fmt, "DmlEffectExtentLeft", value);
                case ShapeAttr.DmlEffectExtentTop:
                    return string.Format(fmt, "DmlEffectExtentTop", value);
                case ShapeAttr.DmlEffectExtentRight:
                    return string.Format(fmt, "DmlEffectExtentRight", value);
                case ShapeAttr.DmlEffectExtentBottom:
                    return string.Format(fmt, "DmlEffectExtentBottom", value);
                case ShapeAttr.ShadowColor:
                    return string.Format(fmt, "ShadowColor", value);
                case ShapeAttr.TextboxTxid:
                    return string.Format(fmt, "TextboxTxid", value);
                case ShapeAttr.HorizontalAlignment:
                    return string.Format(fmt, "HorizontalAlignment", value);
                case ShapeAttr.VerticalAlignment:
                    return string.Format(fmt, "VerticalAlignment", value);
                case ShapeAttr.CoordSizeWidth:
                    return string.Format(fmt, "CoordSizeWidth", value);
                case ShapeAttr.CoordSizeHeight:
                    return string.Format(fmt, "CoordSizeHeight", value);
                case ShapeAttr.TransformRotation:
                    return string.Format(fmt, "TransformRotation", value);
                case ShapeAttr.GeometryThreeDOK:
                    return string.Format(fmt, "GeometryThreeDOK", value);
                case ShapeAttr.GeometryLineOK:
                    return string.Format(fmt, "GeometryLineOK", value);
                case ShapeAttr.LineInsetPen:
                    return string.Format(fmt, "LineInsetPen", value);
                case ShapeAttr.GeometryConnectAngles:
                    return string.Format(fmt, "GeometryConnectAngles", Int32ArrayToString((Int32[])value));
                case ShapeAttr.LineMiterLimit:
                    return string.Format(fmt, "LineMiterLimit", value);
                case ShapeAttr.BWMode:
                    return string.Format(fmt, "BWMode", value);
                case ShapeAttr.FillToLeft:
                    return string.Format(fmt, "FillToLeft", value);
                case ShapeAttr.FillToTop:
                    return string.Format(fmt, "FillToTop", value);
                case ShapeAttr.FillToRight:
                    return string.Format(fmt, "FillToRight", value);
                case ShapeAttr.FillToBottom:
                    return string.Format(fmt, "FillToBottom", value);
                case ShapeAttr.GfxData:
                    return string.Format(fmt, "GfxData", ByteArrayToString((byte[])value));

                default: return string.Format(fmt, key, value);
            }
        }

        private static void DumpMsoEnvelope(DocumentBase doc)
        {
            byte[] msoEnvelopeRaw = ((Document)doc).MsoEnvelope;

            if (msoEnvelopeRaw == null)
                return;

            BinaryReader reader = new BinaryReader(new MemoryStream(msoEnvelopeRaw));

            MsoEnvelope env = new MsoEnvelope(reader);

            Write("\n[MsoEnvelope]");

            Write(string.Format("  {{Subject: '{0}'}}", env.Subject));
            Write(string.Format("  {{Request: '{0}'}}", env.Request));


            Write(string.Format("  {{LastSentTime: {0}}}", DumpMsoEnvelopeDate(env.LastSentTime)));
            Write(string.Format("  {{ReplyTime: {0}}}", DumpMsoEnvelopeDate(env.ReplyTime)));
            Write(string.Format("  {{ExpireTime: {0}}}", DumpMsoEnvelopeDate(env.ExpireTime)));
            Write(string.Format("  {{DeferredDeliveryTime: {0}}}", DumpMsoEnvelopeDate(env.DeferredDeliveryTime)));

            Write(string.Format("  {{FlagStatus: {0}}}", env.FlagStatus));
            Write(string.Format("  {{Importance: {0}}}", env.Importance));
            Write(string.Format("  {{Sensitivity: {0}}}", env.Sensitivity));
            Write(string.Format("  {{SecurityFlags: {0}}}", env.SecurityFlags));

            Write(string.Format("  {{VotingOptions: '{0}'}}", env.VotingOptions));

            Write(string.Format("  {{SentRepresentingName: '{0}'}}", env.SentRepresentingName));
            Write(string.Format("  {{InetAcctStamp: '{0}'}}", env.InetAcctStamp));
            Write(string.Format("  {{InetAcctName: '{0}'}}", env.InetAcctName));
            Write(string.Format("  {{Categories: '{0}'}}", env.Categories));

            Write(string.Format("  {{DeleteAfterSubmit: {0}}}", env.DeleteAfterSubmit));
            Write(string.Format("  {{OriginatorDeliveryReportRequested: {0}}}", env.OriginatorDeliveryReportRequested));
            Write(string.Format("  {{ReadReceiptRequested: {0}}}", env.ReadReceiptRequested));

            DumpMsoRecipients("ReplyRecipients", env.ReplyRecipients);
            DumpMsoRecipients("ContactLinkRecipients", env.ContactLinkRecipients);
            DumpMsoRecipients("Recipients", env.Recipients);

            // internal byte[] SentRepresentingEntryId;
        }

        private static string DumpMsoEnvelopeDate(DateTime dateTime)
        {
            return dateTime != DateTime.MinValue
                ? dateTime.ToString(CultureInfo.GetCultureInfo("ru-RU"))
                : "None";
        }

        private static void DumpMsoRecipients(string name, Pair[][] col)
        {
            if (col == null)
                return;

            int i = 0;
            foreach (Pair[] childList in col)
            {
                Write(string.Format("  [{0}:{1}]", name, i++));
                IndentLevel++;
                foreach (Pair pair in childList)
                {
                    Write(string.Format("{{{0}; {1}}}", pair.First, pair.Second));
                }
                IndentLevel--;
            }
        }

        private static string CropToString(int value)
        {
            return string.Format("{0}%", System.Math.Round(ConvertUtilCore.FixedToDouble(value) * 100, 2));
        }

        private static void DumpPr(AttrCollection attrs, object IAttrSource)
        {
            string format = "{0}";

            if (NoAttributes)
                return;

            if (attrs == null)
                return;

            if (attrs.InternState == InternState.Interned)
            {
                string id = string.Format("0x{0:x4}", attrs.PoolItem.Id);
                if(ShowIntern)
                    Write(string.Format("  {{Interned: {0}}}", id));
                attrs = attrs.PoolItem.Pr;
            }
            else if (attrs.InternState == InternState.Pooled)
            {
                string id = string.Format("0x{0:x4}", attrs.PoolItem.Id);
                if (ShowIntern)
                    Write(string.Format("  {{Pooled: {0}}}", id));
            }

            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                if (key == RevisionAttr.FormatRevision)
                {
                    FormatRevision formatRevision = (FormatRevision)value;
                    if(NoRevisionDetails)
                        Write("+ (-)");
                    else
                        Write(string.Format("+ ({0}, {1})", formatRevision.Author, formatRevision.DateTime));
                    DumpPr(formatRevision.RevPr);
                }
                else if (key == 9999)
                {
                    IndentLevel++;
                    Write("{AttrBackup}");
                    AttrCollectionBackup attrBackup = (AttrCollectionBackup)value;
                    DumpPr(attrBackup.Pr);
                    IndentLevel--;
                }
                else if ((key == FontAttr.AlternateContent) && DumpAlternateContent)
                {
                    IndentLevel++;
                    AlternateContent alternateContent = (AlternateContent)value;
                    Write(string.Format("+ AlternateContent, Requires: '{0}'", alternateContent.Requires));
                    IndentLevel++;
                    if (alternateContent.FallBack != null)
                        DumpNode(alternateContent.FallBack);
                    IndentLevel--;
                    IndentLevel--;
                }
                else if (key == FontAttr.Ruby)
                {
                    Ruby ruby = (Ruby)attrs[key];
                    Write(string.Format("  {{Ruby: {0}}}", RubyToString(ruby)));
                    IndentLevel++;
                    for (int i = 0; i < ruby.Top.Count; i++)
                    {
                        Write(string.Format("{{Top: '{0}'}}", ruby.Top[i].Text));
                        DumpPr(ruby.Top[i].RunPr);
                    }
                    for (int i = 0; i < ruby.Base.Count; i++)
                    {
                        Write(string.Format("{{Base: '{0}'}}", ruby.Base[i].Text));
                        DumpPr(ruby.Base[i].RunPr);
                    }
                    IndentLevel--;
                }
                else if (!IsIgnoreAttr(key))
                {
                    bool isInherited = IsInheritedValue(key, value, IAttrSource);

                    if (!(DontShowInherited && isInherited))
                    {
                        string formattedValue = string.Format(isInherited ? "{0}*" : format, FormatAttr(key, value));
                        if (StringUtil.HasChars(formattedValue))
                            Write(formattedValue);
                    }
                }
            }
        }

        private static bool IsInheritedValue(int key, object value, object IAttrSource)
        {
            // ListId and ListLevel explicitly set is not considred inherited even if they are equal to defaults.
            if (key == ParaAttr.ListId || key == ParaAttr.ListLevel)
                return false;

            // Protection against fetching unknown defaults,
            try
            {
                bool result = IsInheritedValueCore(key, value, IAttrSource);
                return result;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInheritedValueCore(int key, object value, object IAttrSource)
        {
            if (IAttrSource == null)
                return false;

            object inhValue = null;

            if (IAttrSource is Section)
                inhValue = SectPr.FetchDefaultAttr(key);
            else if (IAttrSource is Shape)
                inhValue = ((IShapeAttrSource)IAttrSource).FetchInheritedShapeAttr(key);
            else if (IsRunAttribute(key))
                inhValue = ((IRunAttrSource)IAttrSource).FetchInheritedRunAttr(key);
            else if (IsParaAttribute(key))
                inhValue = ((IParaAttrSource)IAttrSource).FetchInheritedParaAttr(key);
            else if (IsTableAttribute(key))
                inhValue = ((IRowAttrSource)IAttrSource).FetchInheritedRowAttr(key);
            else if (IsCellAttribute(key))
                inhValue = ((ICellAttrSource)IAttrSource).FetchInheritedCellAttr(key);

            return (object.Equals(inhValue, value));
        }

        /// <summary>
        /// Returns attribute collection as string.
        /// </summary>
        internal static string PrToString(AttrCollection attrs)
        {
            return PrToString(attrs, "");
        }

        /// <summary>
        /// Returns attribute collection as string. Attributes are separated with given separator string.
        /// </summary>
        internal static string PrToString(AttrCollection attrs, string attrSeparator)
        {
            StringBuilder sb = new StringBuilder();

            if (attrs == null)
                return "null";

            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                if (!IsIgnoreAttr(key))
                {
                    if (key == RevisionAttr.FormatRevision)
                    {
                        FormatRevision formatRevision = (FormatRevision) value;
                        sb.AppendFormat("+ {0} {1}", formatRevision.Author, formatRevision.DateTime);
                        sb.Append(attrSeparator);
                        PrToString(formatRevision.RevPr, attrSeparator);
                    }
                    else
                    {
                        sb.Append(FormatAttr(key, value).Trim());
                        sb.Append(attrSeparator);
                    }

                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns attribute collection as string. Attributes are separated with given separator string.
        /// </summary>
        internal static string ShapePrToString(AttrCollection attrs, string attrSeparator)
        {
            StringBuilder sb = new StringBuilder();

            if (attrs == null)
                return "null";

            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                if (!IsIgnoreAttr(key))
                {
                    if (key == RevisionAttr.FormatRevision)
                    {
                        FormatRevision formatRevision = (FormatRevision)value;
                        sb.AppendFormat("+ {0} {1}", formatRevision.Author, formatRevision.DateTime);
                        sb.Append(attrSeparator);
                        PrToString(formatRevision.RevPr, attrSeparator);
                    }
                    else
                    {
                        sb.Append(FormatShapeAttr(key, value).Trim());
                        sb.Append(attrSeparator);
                    }

                }
            }
            return sb.ToString();
        }

        private static void DumpPr(AttrCollection attrs)
        {
            DumpPr(attrs, null);
        }

        private static void DumpShapePr(AttrCollection attrs, object IAttrSource)
        {
            if (NoAttributes)
                return;

            if (attrs == null)
                return;

            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                if (!IsIgnoreShapeAttr(key))
                {
                    bool isInherited = IsInheritedValue(key, value, IAttrSource);

                    if (!(DontShowInherited && isInherited))
                    {
                        string formattedValue = string.Format(isInherited ? "{0}*" : "{0}", FormatShapeAttr(key, value));
                        if (StringUtil.HasChars(formattedValue))
                            Write(formattedValue);
                    }
                }
            }
        }

        private static bool HasAttrs(WordAttrCollection attrs, object attrSource)
        {
            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);
                if (ShowAttribute(key, value, attrSource))
                    return true;
            }

            return false;
        }

        private static bool ShowAttribute(int key, object value, object attrSource)
        {
            if (IsIgnoreAttr(key))
                return false;

            if (DontShowInherited && IsInheritedValue(key, value, attrSource))
                return false;

            return true;
        }

        private static bool IsIgnoreAttr(int key)
        {
            if (AllAttributes)
                return false;

            if (NoSysAttrs && Contains(gSysAttrs, key))
                return true;

            if (TogglesOnly)
                return !Contains(gToggleRelatedAttrs, key);

            if (FloatingOnly)
                return !(Contains(gParaFloatingAttrs, key) || Contains(gTableFloatingAttrs, key));

            if (NoFontNames && Contains(gFontNameAttrs, key))
                return true;

            if (NoLocales && Contains(gLocaleAttrs, key))
                return true;

            if (NoRsids && Contains(gRsidAttrs, key))
                return true;

            if (NoRunAttributes && IsRunAttribute(key))
                return true;

            if (NoTableAttributes && (IsTableAttribute(key) || IsCellAttribute(key)))
                return true;

            return false;
        }

        private static bool IsIgnoreShapeAttr(int key)
        {
            if (ShapeIdsOnly)
                return !Contains(gShapeIdsAttrs, key);

            return false;
        }

        private static bool Contains(int[] array, int key)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == key)
                    return true;

            return false;
        }

        internal static string GetPrintableText(string text)
        {
            if (!StringUtil.HasChars(text))
                return "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == 0x00)
                    sb.Append(@"\0");
                else if (c == 0x07)
                    sb.Append(@"\a");
                else if (c == 0x08)
                    sb.Append(@"\b");
                else if (c == 0x09)
                    sb.Append(@"\t");
                else if (c == 0x0a)
                    sb.Append(@"\n");
                else if (c == 0x0b)
                    sb.Append(@"\v");
                else if (c == 0x0c)
                    sb.Append(@"\f");
                else if (c == 0x0d)
                    sb.Append(@"\r");
                else if (c == '\\')
                    sb.Append(@"\\");
                else if (c < 0x20)
                    sb.AppendFormat("\\x{0:x2}", (int) c);
                else if ((c < 0x80) || char.IsLetterOrDigit(c))
                    sb.Append(c);
                else
                    sb.AppendFormat("\\x{0:x4}", (int) c);
            }

            return sb.ToString();
        }

        private static void Write(string format, object obj)
        {
            Write(string.Format(format, obj.ToString()));
        }

        private static void Write(string format, string text)
        {
            Write(string.Format(format, text));
        }

        private static void Write(string text)
        {
            for (int i = 0; i < IndentLevel*4; i++)
                Writer.Write(' ');

            Writer.WriteLine(text);
        }

        internal static bool NoTableBody = false;
        internal static bool NoAttributes;
        internal static bool NoLists;
        internal static bool NoStyles;
        internal static bool NoParagraphAttributes;
        internal static bool NoRunAttributes;
        internal static bool NoSectionAttributes;
        internal static bool NoCellAttributes;
        internal static bool NoTableAttributes;
        internal static bool NoShapeAttributes;
        internal static bool ShowParaId = false;
        internal static bool NoDocumentProperties;
        internal static bool DoRefine;
        internal static bool NoParagraphContent;
        internal static bool NoWarnings;
        internal static bool JoinRuns;
        internal static bool NoSysAttrs = true;
        /// <summary>
        /// Performs horizontal merge normalization before Model saving.
        /// </summary>
        internal static bool NormalizeHorizontalMerge;
        internal static bool NoFontNames;
        internal static bool NoThemes = false;
        internal static bool NoLocales = false;
        internal static bool NoRsids = true;
        internal static bool NoLatentStyles = true;
        internal static bool GlobalTableAttributes = false;
        internal static bool Cleanup = false;
        internal static bool NoGoBackBookmark = true;

        internal static bool NoNodeId = false;

        internal static bool FloatingOnly = false;

        /// <summary>
        /// Dumps only toggle related attributes.
        /// </summary>
        internal static bool TogglesOnly = false;

        /// <summary>
        /// Converts toggle attributes to unified model.
        /// </summary>
        internal static bool UnifyToggles = false;

        internal static bool LocalizeFontNames = false;
        internal static bool LocalizeStyleNames = false;

        internal static bool ShowListLabels = true;
        internal static bool ShowFloating = true;
        internal static bool ShowIntern = true;

        internal static bool ShowUnicodeBlocks = false;
        internal static bool ShowCharacterCategory = false;
        internal static bool SkipCrossStructureAnnotations = false;
        internal static bool ShapeIdsOnly = false;
        internal static bool DumpAlternateContent = true;
        internal static bool DumpCellerData = true;
        internal static bool ShowDmlNode = true;
        internal static bool DontShowInherited = false;
        internal static bool NoRevisionDetails = false;
        internal static bool NoIstdInStyles = true;
        internal static bool NoRevisionGroups = false;

        internal static bool AllAttributes = false;
        internal static bool NoDocumentTree = false;
        internal static bool ShowExpanded = false;
        internal static bool ShowLayout = false;

        /// <summary>
        /// Additionally dumps model for document contained in CustomXML storage.
        /// </summary>
        internal static bool DumpCustomXmlPartDocuments = false;

        private static IWarningCallback gWarningCallback;
        private static TextWriter Writer;
        private static int IndentLevel;

        private static string FileName = @"";

        private static readonly char[] gListSeparatorChars = new char[] {',', ' '};

        private static readonly int[] gSysAttrs = new int[]
            { ParaAttr.Sys_Alignment97, ParaAttr.Sys_LeftIndent97, ParaAttr.Sys_RightIndent97, ParaAttr.Sys_FirstLineIndent97 };

        private static readonly int[] gFontNameAttrs = new int[]
            { FontAttr.NameAscii, FontAttr.NameBi, FontAttr.NameFarEast, FontAttr.NameOther };

        private static readonly int[] gLocaleAttrs = new int[]
            { FontAttr.LocaleId, FontAttr.LocaleIdBi, FontAttr.LocaleIdFarEast };

        private static readonly int[] gRsidAttrs = new int[]
            { ParaAttr.RsidP, FontAttr.RsidR, FontAttr.RsidRPr, TableAttr.RsidTr, SectAttr.Rsid };

        private static readonly int[] gParaFloatingAttrs = new int[]
            {
                ParaAttr.FrameHorizontalAlignment,
                ParaAttr.FrameHeight,
                ParaAttr.FrameHorizontalDistanceFromText,
                ParaAttr.FrameLeft, ParaAttr.FrameLockAnchor,
                ParaAttr.FrameRelativeHorizontalPosition,
                ParaAttr.FrameRelativeVerticalPosition,
                ParaAttr.FrameSuppressOverlap,
                ParaAttr.FrameTextOrientation, ParaAttr.FrameTop,
                ParaAttr.FrameVerticalAlignment,
                ParaAttr.FrameVerticalDistanceFromText,
                ParaAttr.FrameWidth, ParaAttr.FrameWrapType,
                ParaAttr.Istd
            };

        private static readonly int[] gTableFloatingAttrs = new int[]
            {
                TableAttr.FrameTop, TableAttr.FrameLeft,
                TableAttr.HorizontalAlignment,
                TableAttr.VerticalAlignment,
                TableAttr.FrameDistanceFromTop,
                TableAttr.FrameDistanceFromBottom,
                TableAttr.FrameDistanceFromLeft,
                TableAttr.FrameDistanceFromRight,
                TableAttr.RelativeHorizontalPosition,
                TableAttr.RelativeVerticalPosition,
                TableAttr.Istd, TableAttr.AllowOverlap
            };

        private static readonly int[] gShapeIdsAttrs = new int[]
            {
                ShapeAttr.ShapeId,
                ShapeAttr.ShapeName,
                ShapeAttr.Sys_TextboxNextShapeIdRaw,
                ShapeAttr.TextboxNextShapeId
            };

        private static readonly int[] gToggleAttrs = new int[]
            {
                FontAttr.Bold, FontAttr.BoldBi, FontAttr.Italic, FontAttr.ItalicBi, FontAttr.AllCaps, FontAttr.Hidden,
                FontAttr.SmallCaps
            };

        private static readonly int[] gToggleRelatedAttrs = new int[]
        {
            ParaAttr.Istd, FontAttr.Istd, FontAttr.Bold, FontAttr.Italic, FontAttr.AllCaps
        };

        private static readonly int[] gHeaderSortOrder =
            new int[]
            {
                2, /* HeaderEven */
                1, /* HeaderPrimary */
                5, /* FooterEven */
                4, /* FooterPrimary */
                0, /* HeaderFirst */
                3, /* FooterFirst */
            };

         private static readonly string[] gPanoseStrings =
            new string[] { "FamilyType", "SerifStyle", "Weight", "Proportion", "Contrast", "StrokeVariation", "ArmStyle", "Letterform", "Midline", "XHeight" };
    }
}
