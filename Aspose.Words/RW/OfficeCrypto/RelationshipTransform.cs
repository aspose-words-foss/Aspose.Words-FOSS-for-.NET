// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/03/2013 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.OpcPackaging;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    internal class RelationshipTransform : Transform, IComparer<OpcRelationship>
    {
        int IComparer<OpcRelationship>.Compare(OpcRelationship x, OpcRelationship y)
        {
            return StringOrdinalComparer.Default.Compare(x.Id, y.Id);
        }

        protected override void Read(AnyXmlReader reader)
        {
            while (reader.ReadChild("Transform"))
            {
                if (reader.LocalName == "RelationshipReference")
                {
                    // Collect allowed relationship references.
                    AddRelationshipReference(reader.ReadAttribute("SourceId", null));
                }
                else if(reader.LocalName == "RelationshipsGroupReference")
                {
                    // Collect allowed relationship types.
                    AddRelationshipsGroupReference(reader.ReadAttribute("SourceType", null));
                }
            }
        }

        /// <summary>
        /// Applies RelationshipTransform. See ISO29500-2 13.2.4.24 Relationships Transform Algorithm.
        /// </summary>
        /// <remarks>
        /// 1) Sort elements by Id attribute.
        /// 2) Add "TargetMode" attribute with default "Internal" value if it's absent.
        /// 3) Filter elements those are not in mRelationshipReferences list.
        /// </remarks>
        internal override MemoryStream Apply(Stream stream)
        {
            AnyXmlReader reader = new AnyXmlReader(stream);
            List<OpcRelationship> relationships = new List<OpcRelationship>();

            while (reader.ReadChild("Relationships"))
            {
                switch (reader.LocalName)
                {
                    case "Relationship":
                        ReadRelationship(reader, relationships);
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unexpected element '{0}'.", reader.LocalName));
                }
            }

            // Sort relationship by Id.
            relationships.Sort(this);

            MemoryStream output = new MemoryStream();
            AnyXmlBuilder xmlBuilder = new AnyXmlBuilder(output, false);

            xmlBuilder.StartDocument("Relationships");
            xmlBuilder.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");
            foreach (OpcRelationship rel in relationships)
            {
                // Output relationship only if it has allowed Id or allowed Type.
                if (mRelationshipReferences.Contains(rel.Id) || mRelationshipsGroupReferences.Contains(rel.Type))
                {
                    xmlBuilder.StartElement("Relationship");

                    xmlBuilder.WriteAttributeString("Id", rel.Id);
                    xmlBuilder.WriteAttributeString("Type", rel.Type);
                    xmlBuilder.WriteAttributeString("Target", rel.Target);
                    xmlBuilder.WriteAttributeString("TargetMode", rel.IsExternal ? "External" : "Internal");

                    xmlBuilder.EndElement("Relationship");
                }
            }

            xmlBuilder.EndDocument();
            return output;
        }

        internal override void Write(AnyXmlBuilder writer)
        {
            writer.StartElement("Transform");
            writer.WriteAttributeString("Algorithm", Algorithm);

            foreach (string rel in mRelationshipReferences)
            {
                writer.StartElement("mdssi:RelationshipReference");
                writer.WriteAttributeString("SourceId", rel);
                writer.EndElement("mdssi:RelationshipReference");
            }

            foreach(string relGroup in mRelationshipsGroupReferences)
            {
                writer.StartElement("opc:RelationshipsGroupReference");
                writer.WriteAttributeString("SourceType", relGroup);
                writer.EndElement("opc:RelationshipsGroupReference");
            }

            writer.EndElement("Transform");
        }

        internal void AddRelationshipReference(string id)
        {
            mRelationshipReferences.Add(id);
        }

        internal void AddRelationshipsGroupReference(string type)
        {
            if (!mRelationshipsGroupReferences.Contains(type))
                mRelationshipsGroupReferences.Add(type);
        }

        /// <summary>
        /// Reads and collects relationships.
        /// </summary>
        private static void ReadRelationship(AnyXmlReader reader, List<OpcRelationship> relationships)
        {
            string id = "";
            string type = "";
            string target = "";
            bool isExternal = false;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "Id":
                        id = reader.Value;
                        break;
                    case "Type":
                        type = reader.Value;
                        break;
                    case "Target":
                        target = reader.Value;
                        break;
                    case "TargetMode":
                        isExternal = (reader.Value == "External");
                        break;
                    default:
                        // Ignore any other attributes might occur.
                        break;
                }
            }

            relationships.Add(new OpcRelationship(id, type, target, isExternal));
        }

        internal const string Algorithm = @"http://schemas.openxmlformats.org/package/2006/RelationshipTransform";

        /// <summary>
        /// Stores allowed relationship ids.
        /// </summary>
        private readonly List<string> mRelationshipReferences = new List<string>();

        /// <summary>
        /// Stores allowed relationship types.
        /// </summary>
        private readonly List<string> mRelationshipsGroupReferences = new List<string>();
    }
}
