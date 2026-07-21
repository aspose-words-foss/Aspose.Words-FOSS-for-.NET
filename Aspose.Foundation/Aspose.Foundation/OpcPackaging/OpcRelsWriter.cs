// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2007 by Vladimir Averkin

using System;
using System.Collections.Generic;
using Aspose.Xml;

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Provides static method for writing relationships package part
    /// </summary>
    public static class OpcRelsWriter
    {
        /// <summary>
        /// Creates and writes a relationship part for the specified package.
        /// </summary>
        public static void Write(OpcPackageBase package, bool isPrettyFormat)
        {
            WriteCore(package, package.Rels, "", isPrettyFormat);

            List<OpcPackagePart> partsWithRels = new List<OpcPackagePart>();

            foreach (OpcPackagePart part in package.Parts)
            {
                if (part.Rels.Count > 0)
                    partsWithRels.Add(part);
            }

            foreach (OpcPackagePart part in partsWithRels)
                WriteCore(package, part.Rels, part.Name, isPrettyFormat);
        }
        
        /// <summary>
        /// Builds URI for relationships.
        /// </summary>
        public static string GetRelsUri(string uri)
        {
            // Build URI for relationships.
            int lastSlashIndex = uri.LastIndexOf('/');
            string path = uri.Substring(0, lastSlashIndex + 1);
            string name = uri.Substring(lastSlashIndex + 1);
            // AN: Relationship part names should start with a slash. If there is no slash at the beginning of the part name,
            // we should add it. This is needed to write Flat Opc packages correctly.
            string relsUri = string.Format(
                @"{0}{1}_rels/{2}.rels",
                path.StartsWith("/", StringComparison.Ordinal) ? "" : "/",
                path,
                name);

            return relsUri;
        }

        /// <summary>
        /// Writes relationships for the specified package part.
        /// </summary>
        private static void WriteCore(OpcPackageBase package, OpcRelationshipCollection rels, string uri, bool isPrettyFormat)
        {
            OpcPackagePart part = new OpcPackagePart(GetRelsUri(uri), OpcContentType.Relationships);
            AnyXmlBuilder builder = new AnyXmlBuilder(part.Stream, isPrettyFormat);

            builder.StartDocument("Relationships");
            builder.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            foreach (OpcRelationship rel in rels)
            {
                builder.StartElement("Relationship");

                builder.WriteAttributeString("Id", rel.Id);
                builder.WriteAttributeString("Type", rel.Type);
                builder.WriteAttributeString("Target", rel.Target);

                if (rel.IsExternal)
                    builder.WriteAttributeString("TargetMode", "External");

                builder.EndElement();
            }

            builder.EndDocument();

            package.Parts.Add(part);
        }
    }
}
