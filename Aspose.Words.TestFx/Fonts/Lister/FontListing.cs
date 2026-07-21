// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Konstantin Kornilov, Andrey Noskov

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Xml;

namespace Aspose.Words.Tests.Fonts
{
    public class FontListing
    {
        internal readonly List<FontFile> Items = new List<FontFile>();

        /// <summary>
        /// Portable analog XmlSerializer.Serialize() 
        /// </summary>
        /// <param name="stream">Output stream for XmlSchema</param>
        public void Serialize(Stream stream)
        {
            AnyXmlBuilder xmlBuilder = new AnyXmlBuilder(stream,true);
            xmlBuilder.StartDocument();
            xmlBuilder.StartElement("FontListing");
            xmlBuilder.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlBuilder.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            if (Items != null)
            {
                xmlBuilder.StartElement("Items");
                foreach (FontFile fontFile in Items)
                {
                    xmlBuilder.StartElement("FontFile");
                    if (fontFile is OpenTypeFont)
                        xmlBuilder.WriteAttributeString("xsi:type", "OpenTypeFont");
                    else
                        xmlBuilder.WriteAttributeString("xsi:type", "TrueTypeCollection");

                    xmlBuilder.WriteElement("Path", fontFile.Path);
                    xmlBuilder.WriteElement("Md5", fontFile.Md5);
                    xmlBuilder.WriteElement("Size", fontFile.Size.ToString());
                    xmlBuilder.WriteElement("IsInRegistry", fontFile.IsInRegistry.ToString().ToLower());
                    xmlBuilder.WriteElement("IsInFontsFolder", fontFile.IsInFontsFolder.ToString().ToLower());
                    if (fontFile.Items != null)
                    {
                        xmlBuilder.StartElement("Items");
                        foreach (FontMetaInfo fontMetaInfo in fontFile.Items)
                        {
                            xmlBuilder.StartElement("FontMetaInfo");
                            xmlBuilder.WriteElement("FullName", fontMetaInfo.FullName);
                            xmlBuilder.WriteElement("FamilyName", fontMetaInfo.FamilyName);
                            string str = "Regular";
                            if (fontMetaInfo.Style == FontStyle.Regular)
                                str = "Regular";
                            else if (fontMetaInfo.Style == FontStyle.Bold)
                                str = "Bold";
                            else if (fontMetaInfo.Style == FontStyle.Italic)
                                str = "Italic";
                            else if (fontMetaInfo.Style == (FontStyle.Bold | FontStyle.Italic))
                                str = "Bold Italic";
                            else if (fontMetaInfo.Style == FontStyle.Strikeout)
                                str = "Strikeout";
                            else if (fontMetaInfo.Style == FontStyle.Underline)
                                str = "Underline";
                            xmlBuilder.WriteElement("Style", str);
                            xmlBuilder.WriteElement("Version", fontMetaInfo.Version);
                            xmlBuilder.EndElement();
                        }
                        xmlBuilder.EndElement();
                    }
                    xmlBuilder.EndElement();
                }
                xmlBuilder.EndElement();
            }
            xmlBuilder.EndDocument();
        }

        /// <summary>
        /// Portable analog XmlSerializer.Deserialize()
        /// </summary>
        /// <param name="stream">Inutput stream with XmlSchema</param>
        /// <returns>return equivalent FontListing object for .net and Java</returns>
        public static FontListing Deserialize(Stream stream)
        {
            FontListing serializer = new FontListing();
            AnyXmlReader reader = new AnyXmlReader(stream);
            if (reader.Name == "FontListing")
            {
                reader.ReadChild("FontListing");
                if (reader.Name == "Items")
                {
                    reader.ReadChild("Items");
                    while (reader.Name == "FontFile")
                    {
                        FontFile fontFile;
                        if (reader.ReadAttribute("type", "OpenTypeFont") == "OpenTypeFont")
                            fontFile = new OpenTypeFont();
                        else
                            fontFile = new TrueTypeCollection();

                        reader.ReadChild("FontFile");
                        if (reader.Name == "Path")
                            fontFile.Path = reader.ReadString();
                        reader.ReadChild("FontFile");
                        if (reader.Name == "Md5")
                            fontFile.Md5 = reader.ReadString();
                        reader.ReadChild("FontFile");
                        if (reader.Name == "Size")
                            fontFile.Size = reader.ReadStringAsInt();
                        reader.ReadChild("FontFile");
                        if (reader.Name == "IsInRegistry")
                            fontFile.IsInRegistry = reader.ReadStringAsBool();
                        reader.ReadChild("FontFile");
                        if (reader.Name == "IsInFontsFolder")
                            fontFile.IsInFontsFolder = reader.ReadStringAsBool();
                        reader.ReadChild("FontFile");
                        if (reader.Name == "Items")
                        {
                            reader.ReadChild("Items");
                            while (reader.Name == "FontMetaInfo")
                            {
                                FontMetaInfo fontMetaInfo = new FontMetaInfo();
                                reader.ReadChild("FontMetaInfo");
                                if (reader.Name == "FullName")
                                    fontMetaInfo.FullName = reader.ReadString();
                                reader.ReadChild("FontMetaInfo");
                                if (reader.Name == "FamilyName")
                                    fontMetaInfo.FamilyName = reader.ReadString();
                                reader.ReadChild("FontMetaInfo");
                                if (reader.Name == "Style")
                                {
                                    string str = reader.ReadString();
                                    if (str == "Regular")
                                        fontMetaInfo.Style = FontStyle.Regular;
                                    else if (str == "Bold")
                                        fontMetaInfo.Style = FontStyle.Bold;
                                    else if (str == "Italic")
                                        fontMetaInfo.Style = FontStyle.Italic;
                                    else if (str == "Bold Italic")
                                        fontMetaInfo.Style = FontStyle.Bold | FontStyle.Italic;
                                    else if (str == "Strikeout")
                                        fontMetaInfo.Style = FontStyle.Strikeout;
                                    else if (str == "Underline")
                                        fontMetaInfo.Style = FontStyle.Underline;
                                }
                                reader.ReadChild("FontMetaInfo");
                                if (reader.Name == "Version")
                                    fontMetaInfo.Version = reader.ReadString();
                                reader.ReadChild("Items");

                                fontFile.Items.Add(fontMetaInfo);

                            }
                            reader.ReadChild("Items");
                        }
                        serializer.Items.Add(fontFile);
                    }
                }
            }
            return serializer;
        }
    }
}
