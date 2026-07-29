// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2013 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.OpcPackaging;
using Aspose.Words.DigitalSignatures;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements signing OpcPackage using XmlDsig method. Used for DOCX document.
    /// </summary>
    internal class OpcSignatureWriterDocx : OpcSignatureWriterBase
    {
        internal OpcSignatureWriterDocx(OpcPackage package) : base(package, true)
        {
        }

        /// <summary>
        /// Adds reference to part relationships.
        /// </summary>
        protected override void AddRelationshipReference(OpcPackage package, OpcPackagePart part, ReferenceCollection references)
        {
            string relsUri = OpcRelsWriter.GetRelsUri((part == null) ? "" : part.Name);

            Reference r = new Reference(relsUri, OpcContentType.Relationships);
            references.Add(r);

            // Apply RelationshipTransform.
            RelationshipTransform transform = (RelationshipTransform)Transform.Create(RelationshipTransform.Algorithm);
            r.TransformCollection.Add(transform);

            // Loop thought package.Rels or part.Rels
            OpcRelationshipCollection rels = (part == null) ? package.Rels : part.Rels;
            foreach (OpcRelationship rel in rels)
            {
                // Filter external relationships.
                if (rel.IsExternal)
                {
                    // WORDSNET-27977 Word adds all references to transform, even it is external.
                    transform.AddRelationshipReference(rel.Id);
                }
                else
                {
                    OpcPackagePart relatedPart = (part == null)
                        ? package.GetPartByName(string.Format("/{0}", rel.Target))
                        : package.GetPartByName(part.GetRelatedPartName(rel));

                    // Filter excluded parts.
                    if (!IsExcludedPart(relatedPart))
                        transform.AddRelationshipReference(rel.Id);
                }
            }

            // Apply C14nTransform.
            r.TransformCollection.Add(Transform.Create(C14Transform.Algorithm));
        }

        protected override string GetSignatureOriginPartName()
        {
            return @"/_xmlsignatures/origin.sigs";
        }

        protected override string GetSignaturePartName(OpcPackage package)
        {
            // Do this in straight way.
            int i = 1;
            while (package.GetPartByName(string.Format("/_xmlsignatures/sig{0}.xml", i)) != null)
                i++;

            return string.Format("sig{0}.xml", i);
        }

        protected override MemoryStream GetSignatureStream(ReferenceCollection refs, DigitalSignature signature)
        {
            XmlDsigSignerOoxml xmlDsigWriter = new XmlDsigSignerOoxml();
            return xmlDsigWriter.GetSignature(refs, signature);
        }

        /// <summary>
        /// Indicates that given part must be excluded from signing.
        /// </summary>
        protected override bool IsExcludedPart(OpcPackagePart part)
        {
            if (gExcludeNames.Contains(part.Name))
                return true;

            if (part.Name.Contains("/[trash]"))
                return true;

            if ((part.ContentType == OpcContentType.DigitalSignature) ||
                (part.ContentType == OpcContentType.DigitalSignatureOrigin))
                return true;

            return false;
        }

        protected override string GetCertificatePartName(OpcPackage package)
        {
            throw new NotImplementedException("Should not be called.");
        }

        static OpcSignatureWriterDocx()
        {
            gExcludeNames = new List<string>();
            gExcludeNames.Add(OpcPackage.ContentTypesPartName);
            gExcludeNames.Add(@"/docProps/app.xml");
            gExcludeNames.Add(@"/docProps/core.xml");
        }

        private static readonly List<string> gExcludeNames;
    }
}
