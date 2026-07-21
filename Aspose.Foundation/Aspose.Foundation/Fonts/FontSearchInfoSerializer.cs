// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/11/2021 by Konstantin Kornilov

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.Fonts.TrueType;
using Aspose.Fonts.Ttc;
using Aspose.Xml;

namespace Aspose.Fonts
{
    /// <summary>
    /// This class helps to serialize/deserialize <see cref="FontSearchInfoCache"/>.
    /// </summary>
    internal static class FontSearchInfoSerializer
    {
        public static void SaveXml(Stream outputStream, IEnumerable<FontSearchInfo> searchInfos)
        {
            AnyXmlBuilder builder = new AnyXmlBuilder(outputStream, true);
            builder.StartDocument(RootElement);
            builder.WriteAttributeString("xmlns", "Aspose.Words");
            builder.StartElement(CollectionElement);

            foreach (FontSearchInfo searchInfo in searchInfos)
            {
                if(searchInfo.FontData.FileData.GetCacheKeyInternal() == null)
                    continue;

                builder.StartElement(ItemElement);
                builder.WriteAttributeString(FamilyNameAttribute, searchInfo.FontFamilyName);
                builder.WriteAttributeString(StyleAttribute, FormatterPal.IntToXml((int)searchInfo.Style));
                builder.WriteAttributeString(FullNameAttribute, searchInfo.FontFullName);
                builder.WriteAttributeString(PanoseAttribute, StringUtil.BytesToHex(searchInfo.Panose.Values));
                builder.WriteAttributeString(UnicodeRangesAttribute, UnicodeRangesToXml(searchInfo.UnicodeRanges));
                builder.WriteAttributeString(CodepageRangesAttribute, CodepageRangesToXml(searchInfo.CodepageRanges));
                builder.WriteAttributeString(IsCffAttribute, FormatterPal.BoolToTrueFalseLower(searchInfo.IsCff));
                builder.WriteAttributeString(IsTtcAttribute, FormatterPal.BoolToTrueFalseLower(searchInfo.IsTtc));
                if (searchInfo.IsTtc)
                    builder.WriteAttributeString(
                        TtcIndexAttribute,
                        FormatterPal.IntToXml(((TtcPhysicalFontData)searchInfo.FontData).TtcIndex));
                builder.WriteAttributeString(VersionAttribute, searchInfo.Version);
                builder.WriteAttributeString(FsTypeAttribute, FormatterPal.IntToXml(searchInfo.FsType.Value));
                builder.WriteAttributeString(FontSourceKeyAttribute, searchInfo.FontData.FileData.GetCacheKeyInternal());

                WriteNameList(builder, FamilyNamesElement, searchInfo.FontFamilyNamesAllLanguages);
                WriteNameList(builder, FullNamesElement, searchInfo.FontFullNamesAllLanguages);

                builder.EndElement();
            }

            builder.EndElement();
            builder.EndDocument();
        }

        private static void WriteNameList(AnyXmlBuilder builder, string tagName, ICollection<string> list)
        {
            builder.StartElement(tagName);
            foreach (string name in list)
            {
                builder.StartElement(NameElement);
                builder.WriteAttributeString(NameAttribute, name);
                builder.EndElement();
            }
            builder.EndElement();
        }

        private static string UnicodeRangesToXml(FontUnicodeRanges ranges)
        {
            return string.Format(
                "{0}{1}{2}{3}",
                FormatterPal.IntToStrX8((int)ranges.Range1),
                FormatterPal.IntToStrX8((int)ranges.Range2),
                FormatterPal.IntToStrX8((int)ranges.Range3),
                FormatterPal.IntToStrX8((int)ranges.Range4));
        }

        private static FontUnicodeRanges UnicodeRangesFromXml(string xml)
        {
            if (xml.Length != 8 * 4)
                return FontUnicodeRanges.Empty;

            uint range1 = (uint)FormatterPal.ParseHex(xml.Substring(0, 8));
            uint range2 = (uint)FormatterPal.ParseHex(xml.Substring(8, 8));
            uint range3 = (uint)FormatterPal.ParseHex(xml.Substring(16, 8));
            uint range4 = (uint)FormatterPal.ParseHex(xml.Substring(24, 8));
            return new FontUnicodeRanges(range1, range2, range3, range4);
        }

        private static string CodepageRangesToXml(FontCodepageRanges ranges)
        {
            return string.Format(
                "{0}{1}",
                FormatterPal.IntToStrX8((int)ranges.Range1),
                FormatterPal.IntToStrX8((int)ranges.Range2));
        }

        private static FontCodepageRanges CodepageRangesFromXml(string xml)
        {
            if (xml.Length != 8 * 2)
                return FontCodepageRanges.Empty;

            uint range1 = (uint)FormatterPal.ParseHex(xml.Substring(0, 8));
            uint range2 = (uint)FormatterPal.ParseHex(xml.Substring(8, 8));
            return new FontCodepageRanges(range1, range2);
        }

        public static ICollection<FontSearchInfo> LoadXml(Stream xmlStream, IEnumerable<FontSearchDataContainer> dataList)
        {
            AnyXmlReader reader = new AnyXmlReader(xmlStream);
            while (reader.ReadChild(RootElement))
            {
                switch (reader.LocalName)
                {
                    case CollectionElement:
                        return ReadSearchInfos(reader, dataList);
                    default:
                        break;
                }
            }

            return new List<FontSearchInfo>();
        }

        private static ICollection<FontSearchInfo> ReadSearchInfos(AnyXmlReader reader, IEnumerable<FontSearchDataContainer> dataList)
        {
            List<FontSearchInfo> result = new List<FontSearchInfo>();
            while (reader.ReadChild(CollectionElement))
            {
                if (reader.LocalName != ItemElement)
                    continue;

                string familyName = reader.ReadAttribute(FamilyNameAttribute, "");
                FontStyle style = (FontStyle)reader.ReadIntAttribute(StyleAttribute, (int)FontStyle.Regular);
                string fullName = reader.ReadAttribute(FullNameAttribute, "");
                FontPanose panose = new FontPanose(StringUtil.HexToBytes(reader.ReadAttribute(PanoseAttribute, "")));
                FontUnicodeRanges unicodeRanges = UnicodeRangesFromXml(reader.ReadAttribute(UnicodeRangesAttribute, ""));
                FontCodepageRanges codepageRanges = CodepageRangesFromXml(reader.ReadAttribute(CodepageRangesAttribute, ""));
                bool isCff = reader.ReadBoolAttribute(IsCffAttribute, false);
                bool isTtc = reader.ReadBoolAttribute(IsTtcAttribute, false);
                int ttcIndex = reader.ReadIntAttribute(TtcIndexAttribute, -1);
                string version = reader.ReadAttribute(VersionAttribute, "");
                int fsType = reader.ReadIntAttribute(FsTypeAttribute, 0);
                string key = reader.ReadAttribute(FontSourceKeyAttribute, "");
                ICollection<string> familyNames = ReadNameList(reader, FamilyNamesElement);
                ICollection<string> fullNames = ReadNameList(reader, FullNamesElement);
                FontSearchDataContainer searchDataContainer = FindSearchData(key, dataList);

                if(searchDataContainer == null || !StringUtil.HasChars(familyName))
                    continue;

                PhysicalFontData physicalFontData;
                if (isTtc)
                    physicalFontData = new TtcPhysicalFontData(searchDataContainer.FontData, ttcIndex, fullName);
                else
                    physicalFontData = new PhysicalFontData(searchDataContainer.FontData);

                result.Add(
                    new FontSearchInfo(
                        physicalFontData, fullName, familyName, fullNames, familyNames, style,
                        isCff, searchDataContainer.SourcePriority, version, panose, unicodeRanges, codepageRanges,
                        new FontFsType((ushort)fsType)));
            }

            return result;
        }

        private static FontSearchDataContainer FindSearchData(string key, IEnumerable<FontSearchDataContainer> dataList)
        {
            foreach (FontSearchDataContainer data in dataList)
                if (data.FontData.GetCacheKeyInternal() == key)
                    return data;

            return null;
        }

        private static ICollection<string> ReadNameList(AnyXmlReader reader, string element)
        {
            List<string> names = new List<string>();
            while (reader.ReadChild(element))
            {
                if (reader.LocalName != NameElement)
                    continue;

                string name = reader.ReadAttribute(NameAttribute, "");

                if(StringUtil.HasChars(name))
                    names.Add(name);
            }

            return names;
        }

        private const string RootElement = "FontSearchInfoCache";
        private const string CollectionElement = "SearchInfos";
        private const string ItemElement = "SearchInfo";
        private const string FamilyNameAttribute = "FamilyName";
        private const string StyleAttribute = "Style";
        private const string FullNameAttribute = "FullName";
        private const string PanoseAttribute = "Panose";
        private const string UnicodeRangesAttribute = "UnicodeRanges";
        private const string CodepageRangesAttribute = "CodepageRanges";
        private const string IsCffAttribute = "IsCff";
        private const string IsTtcAttribute = "IsTtc";
        private const string TtcIndexAttribute = "TtcIndex";
        private const string VersionAttribute = "Version";
        private const string FsTypeAttribute = "FsType";
        private const string FontSourceKeyAttribute = "FontSourceKey";
        private const string FamilyNamesElement = "FamilyNamesAllLanguages";
        private const string FullNamesElement = "FullNamesAllLanguages";
        private const string NameElement = "Name";
        private const string NameAttribute = "Value";
    }
}
