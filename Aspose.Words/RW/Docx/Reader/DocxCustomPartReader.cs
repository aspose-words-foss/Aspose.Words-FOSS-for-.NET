// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2010 by Roman Korchagin

using Aspose.Collections.Generic;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads custom parts. Custom parts is a set of related parts or external targets that have
    /// relationship types that are not specified in the ISO29500 standard. Microsoft Word preserves
    /// such parts during open/save cycles and so do we.
    /// </summary>
    internal static class DocxCustomPartReader
    {
        /// <summary>
        /// Reads custom parts or external targets from the specified collection of relationships.
        /// </summary>
        /// <param name="rels">The relationships to child parts to enumerate. We do not pass the parent part itself here
        /// because the parent can actually be the package itself. It is enough to pass just the relationship collection.</param>
        /// <param name="package">The whole package. We need it to get the related parts.</param>
        /// <param name="customParts">Where the data is read to.</param>
        internal static void Read(
            OpcRelationshipCollection rels,
            OpcPackageBase package,
            CustomPartCollection customParts)
        {
            foreach (OpcRelationship rel in rels)
            {
                // Any part can have unknown relationships, but at the moment we load only custom parts related to the package. It is enough.
                if (gStandardPackageRelTypes.Contains(rel.Type))
                    continue;

                CustomPart part = ReadCustomPart(rel, rels.PartName, package, customParts);
                if (part != null)
                    customParts.Add(part);
            }
        }

        private static CustomPart ReadCustomPart(OpcRelationship rel, string parentPartName, OpcPackageBase package, CustomPartCollection customParts)
        {
            CustomPart customPart = new CustomPart();

            customPart.OriginalId = rel.Id;
            customPart.RelationshipType = rel.Type;
            customPart.IsExternal = rel.IsExternal;            

            if (rel.IsExternal)
            {
                customPart.Name = rel.Target;
            }
            else
            {
                OpcPackagePart opcPart = package.GetPartByName(OpcPackageBase.MakeAbsolute(parentPartName, rel.Target));
                // If we cannot find the related part, ignore this relationship.
                if (opcPart == null)
                    return null;

                customPart.Name = opcPart.Name;
                customPart.ContentType = opcPart.ContentType;
                customPart.Data = StreamUtil.CopyStreamToByteArray(opcPart.Stream);

                // WORDSNET-10389 Custom parts can has relationships with another parts recursively.
                if (opcPart.Rels != null)
                    foreach (OpcRelationship childRel in opcPart.Rels)
                    {
                        CustomPart childPart = ReadCustomPart(childRel, customPart.Name, package, customParts);
                        if (childPart != null)
                            customParts.Add(childPart);
                    }
            }

            customPart.ParentPartName = parentPartName;

            return customPart;
        }

        /// <summary>
        /// Contains relationship types that the standard defines for an OOXML package.
        /// </summary>
        private static readonly HashSetGeneric<string> gStandardPackageRelTypes = new HashSetGeneric<string>();

        static DocxCustomPartReader()
        {
            // Standard relationship types defined for an transition OOXML package.
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.OfficeDocument, false));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.CoreProperties, false));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.CustomProperties, false));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.ExtendedProperties, false));
            gStandardPackageRelTypes.Add(OpcRelationshipType.DigitalSignatureOrigin);
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.Thumbnail, false));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.WebExtensionTaskPanes, false));

            // Standard relationship types defined for an strict OOXML package.
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.OfficeDocument, true));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.CoreProperties, true));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.CustomProperties, true));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.ExtendedProperties, true));
            gStandardPackageRelTypes.Add(OpcRelationshipType.DigitalSignatureOrigin);
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.Thumbnail, true));
            gStandardPackageRelTypes.Add(DocxRelationshipTypes.GetType(DocxRelationshipType.WebExtensionTaskPanes, true));
        }
    }
}
