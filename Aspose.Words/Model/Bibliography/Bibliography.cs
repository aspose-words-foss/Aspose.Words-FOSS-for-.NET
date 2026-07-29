// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Xml;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents the list of bibliography sources available in the document.
    /// </summary>
    public sealed class Bibliography
    {
        internal Bibliography(Document document)
        {
            Sources = new List<Source>();
            mDocument = document;
            mCustomXmlPart = BibliographyReader.Read(this, document);

            mVersions = BibliographyVersion.Calculate(this);
        }

        private Bibliography(Document document, CustomXmlPart customXmlPart, int version)
        {
            Sources = new List<Source>();
            mDocument = document;
            mCustomXmlPart = customXmlPart;
            mVersions = version;
        }

        static Bibliography()
        {
            gBuiltInStyles = new Dictionary<string, BibliographyStyleInfo>
            {
#if !JAVA       // until fix WORDSJAVA-2881
                { "apasixtheditionofficeonline.xsl", new BibliographyStyleInfo("APA", "6") },
                { "chicago.xsl", new BibliographyStyleInfo("Chicago", "16") },
                { "gb.xsl", new BibliographyStyleInfo("GB7714", "2005") },
                { "gostname.xsl", new BibliographyStyleInfo("GOST - Name Sort", "2003") },
                { "gosttitle.xsl", new BibliographyStyleInfo("GOST - Title Sort", "2003") },
                { "harvardanglia2008officeonline.xsl", new BibliographyStyleInfo("Harvard - Anglia", "2008") },
                { "iso690.xsl", new BibliographyStyleInfo("ISO 690 - First Element and Date", "1987") },
                { "iso690nmerical.xsl", new BibliographyStyleInfo("ISO 690 - Numerical Reference", "1987") },
                { "mlaseventheditionofficeonline.xsl", new BibliographyStyleInfo("MLA", "7") },
                { "sist02.xsl", new BibliographyStyleInfo("SIST02", "2003") },
                { "turabian.xsl", new BibliographyStyleInfo("Turabian", "6") },
#endif
                { "ieee2006officeonline.xsl", new BibliographyStyleInfo("IEEE", "2006") },
            };
        }

        /// <summary>
        /// Transforms bibliography or citation xml with specified in <see cref="BibliographyStyle"/> active style.
        /// </summary>
        internal Stream Transform(Stream xml)
        {
            if (!EnsureTransform())
                throw new InvalidOperationException();

            return TransformSafe(xml);
        }

        /// <summary>
        /// Ensures XSL transformation.
        /// </summary>
        [JavaThrows(true)]
        internal bool EnsureTransform()
        {
            if (mIsTransformInitialized)
                return mTransform != null;

            mIsTransformInitialized = true;

            if (string.IsNullOrEmpty(BibliographyStyle))
                return false;

            using (Stream style = LoadStyle(BibliographyStyle, mDocument))
            {
                if (style == null)
                    return false;

                mTransform = new XslTransform();
                mTransform.Load(style);
            }

            return true;
        }

        private Stream TransformSafe(Stream xml)
        {
            try
            {
                return mTransform.Transform(xml);
            }
            catch
            {
                return null;
            }
        }

        private static Stream LoadStyle(string style, Document document)
        {
            return LoadUserProvidedStyle(style, document) ?? LoadBuiltInStyle(style);
        }

        private static Stream LoadUserProvidedStyle(string style, Document document)
        {
            IBibliographyStylesProvider provider = document.FieldOptions.BibliographyStylesProvider;
            return provider != null
                ? provider.GetStyle(style)
                : null;
        }

        private static BibliographyStyleInfo GetBuiltInStyleInfo(string style)
        {
            string invariantStyle = style.TrimStart('\\').ToLowerInvariant();
            BibliographyStyleInfo result = null;
            gBuiltInStyles.TryGetValue(invariantStyle, out result);
            return result;
        }

        private static Stream LoadBuiltInStyle(string style)
        {
            string invariantStyle = style.TrimStart('\\').ToLowerInvariant();
            return gBuiltInStyles.ContainsKey(invariantStyle)
                ? ResourceUtil.FetchResourceStream("Aspose.Words.Resources.Bibliography." + invariantStyle)
                : null;
        }

        /// <summary>
        /// Gets or sets a string that represents the name of the active style to use for a bibliography.
        /// </summary>
        public string BibliographyStyle
        {
            get { return mBibliographyStyle; }
            set
            {
                if (mBibliographyStyle == value)
                    return;

                mBibliographyStyle = value;

                BibliographyStyleInfo bibliographyStyleInfo = mBibliographyStyle != null
                    ? GetBuiltInStyleInfo(mBibliographyStyle)
                    : null;

                if (bibliographyStyleInfo != null)
                {
                    StyleName = bibliographyStyleInfo.Name;
                    Version = bibliographyStyleInfo.Version;
                }
                else
                {
                    StyleName = null;
                    Version = null;
                }
            }
        }

        internal string StyleName { get; set; }

        internal string Version { get; set; }

        /// <summary>
        /// Gets a collection that represents all the sources contained in a bibliography.
        /// </summary>
        public IList<Source> Sources { get; }

        internal void Save()
        {
            int version = BibliographyVersion.Calculate(this);
            if (mVersions == version)
                return;

            mCustomXmlPart = BibliographyWriter.Write(this, mDocument, mCustomXmlPart);
            mVersions = version;
        }

        internal Bibliography Clone(Document document)
        {
            CustomXmlPart customXmlPart = mCustomXmlPart != null
                ? document.CustomXmlParts.GetById(mCustomXmlPart.Id)
                : null;

            Bibliography lhs = new Bibliography(document, customXmlPart, mVersions);

            lhs.BibliographyStyle = BibliographyStyle;
            lhs.StyleName = StyleName;
            lhs.Version = Version;

            foreach (Source source in Sources)
                lhs.Sources.Add(source.Clone());

            return lhs;
        }

        private XslTransform mTransform;
        private bool mIsTransformInitialized;
        private CustomXmlPart mCustomXmlPart;
        private string mBibliographyStyle;
        private int mVersions;

        private readonly Document mDocument;

        private static readonly IDictionary<string, BibliographyStyleInfo> gBuiltInStyles;

        internal const string XmlNameSpace = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography";

        /// <summary>
        /// Provides static information about bibliography style.
        /// </summary>
        private class BibliographyStyleInfo
        {
            internal BibliographyStyleInfo(string name, string version)
            {
                Name = name;
                Version = version;
            }

            internal string Name { get; }
            internal string Version { get; }
        }
    }
}
