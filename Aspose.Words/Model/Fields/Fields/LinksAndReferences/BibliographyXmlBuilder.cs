// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System.Collections.Generic;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Bibliography;
using Aspose.Xml;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Builds XML to be transformed with a bibliography style XSLT stylesheet.
    /// </summary>
    internal class BibliographyXmlBuilder
    {
        internal BibliographyXmlBuilder(string name, int defaultLcid)
        {
            mName = name;
            mDefaultLcid = defaultLcid;
        }

        /// <summary>
        /// Builds XML.
        /// </summary>
        internal Stream BuildXml()
        {
            MemoryStream stream = new MemoryStream();
            AnyXmlBuilder builder = new AnyXmlBuilder(stream, false);

            builder.StartDocument(mName);
            builder.WriteAttributeString("xmlns:b", Bibliography.Bibliography.XmlNameSpace);
            builder.WriteAttributeString("xmlns", Bibliography.Bibliography.XmlNameSpace);

            AppendLocales(builder, mDefaultLcid, SelectLocales());

            foreach (KeyValuePair<string, string> element in mOptions)
                AppendElement(builder, element);

            foreach (Source source in mSources)
                builder.WriteRaw(source.GetNormalizedXml());

            builder.EndDocument();

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Adds an option without value if condition is met.
        /// </summary>
        internal BibliographyXmlBuilder WithOptionIf(string name, bool condition)
        {
            if (condition)
                mOptions[name] = null;

            return this;
        }

        /// <summary>
        /// Adds an option with value if value is no null.
        /// </summary>
        internal BibliographyXmlBuilder WithOptionIf(string name, string value)
        {
            if (value != null)
                mOptions[name] = value;

            return this;
        }

        /// <summary>
        /// Adds a source.
        /// </summary>
        internal BibliographyXmlBuilder WithSource(Source source)
        {
            mSources.Add(source);
            return this;
        }

        /// <summary>
        /// Adds a sources.
        /// </summary>
        internal BibliographyXmlBuilder WithSources(IEnumerable<Source> sources)
        {
            mSources.AddRange(sources);
            return this;
        }

        private IEnumerable<int> SelectLocales()
        {
            List<int> result = new List<int>();

            foreach (Source source in mSources)
            {
                if (source.HasNormalizedLcid)
                    result.Add(source.NormalizedLcid);
            }

            return result;
        }

        private static void AppendElement(IAnyXmlBuilder builder, KeyValuePair<string, string> element)
        {
            if (element.Value != null)
                builder.WriteElement(element.Key, element.Value);
            else
                builder.WriteEmptyElement(element.Key);
        }

        private static void AppendLocales(IAnyXmlBuilder builder, int defaultLcid, IEnumerable<int> lcids)
        {
            builder.StartElement("Locals");
            builder.WriteElement("DefaultLCID", defaultLcid);

            HashSetGeneric<int> uniqueLcids = new HashSetGeneric<int>(lcids);
            uniqueLcids.Add(defaultLcid);

            foreach (int lcid in uniqueLcids)
                AppendLocale(builder, lcid);

            builder.EndElement();
        }

        private static void AppendLocale(IAnyXmlBuilder builder, int lcid)
        {
            string localeXml;
            if (gLocales.TryGetValue(lcid, out localeXml))
                builder.WriteRaw(localeXml);
            else
                Debug.Fail(string.Format("Unknown bibliography locale '{0}'", lcid));
        }

        /// <remarks>
        /// See also TestLinksAndReferences.GenerateBibliographyLocales
        /// </remarks>
        [JavaConvertCheckedExceptions]
        private static Dictionary<int, string> LoadLocales()
        {
            using (Stream stream = ResourceUtil.FetchResourceStream(LocalesResourceName))
            {
                AnyXmlReader reader = new AnyXmlReader(stream, true);

                string rootElement = reader.LocalName;

                Dictionary<int,string> result = new Dictionary<int, string>();

                if (reader.ReadChild(rootElement))
                {
                    while (!reader.IsEndElement(rootElement))
                    {
                        Debug.Assert(reader.LocalName == "Local");

                        string localeXml = reader.ReadOuterXml();
                        int lcid = ReadLcid(localeXml);

                        result.Add(lcid, localeXml);
                    }
                }

                Debug.Assert(result.Count == 258);

                return result;
            }
        }

        private static int ReadLcid(string xml)
        {
            AnyXmlReader reader = new AnyXmlReader(xml, null);
            return int.Parse(reader.ReadAttribute("LCID", null));
        }

        private readonly List<Source> mSources = new List<Source>();
        private readonly Dictionary<string, string> mOptions = new Dictionary<string, string>();
        private readonly string mName;
        private readonly int mDefaultLcid;

        private static readonly Dictionary<int, string> gLocales = LoadLocales();
        private const string LocalesResourceName = "Aspose.Words.Resources.Bibliography.BibliographyLocales.xml";
    }
}
