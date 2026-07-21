// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2010 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Crypto;
using Aspose.Words.DigitalSignatures;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Represents the Reference element in XmlDsig.
    /// </summary>
    internal class Reference
    {
        internal Reference(string uri)
        {
            mUri = uri;
        }

        internal Reference(string uri, string contentType)
        {
            mUri = string.Format("{0}?ContentType={1}", uri, contentType);
        }

        /// <summary>
        /// Constructs object from XML reader.
        /// </summary>
        internal Reference(AnyXmlReader xmlReader)
        {
            mUri = xmlReader.ReadAttribute("URI", null);

            while (xmlReader.ReadChild("Reference"))
            {
                switch (xmlReader.LocalName)
                {
                    case "DigestValue":
                        DigestValue = xmlReader.ReadString();
                        break;
                    case "DigestMethod":
                        DigestMethod = xmlReader.ReadAttribute("Algorithm", "");
                        break;
                    case "Transforms":
                        TransformCollection.Read(xmlReader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Writes Reference element which contains uri and digest value for signed part.
        /// </summary>
        internal void Write(AnyXmlBuilder writer, DigestAlgorithm digestAlgorithm)
        {
            writer.StartElement("Reference");

            writer.WriteAttributeString("URI", Entitize(Uri));

            TransformCollection.Write(writer);

            writer.StartElement("DigestMethod");
            writer.WriteAttributeString("Algorithm", DigitalSignatureUtil.GetDigestMethod(digestAlgorithm));
            writer.EndElement();

            writer.WriteElement("DigestValue", ComputeDigestValue(digestAlgorithm));

            writer.EndElement("Reference");
        }

        /// <summary>
        /// Computes a digest value using a specified digest algorithm.
        /// </summary>
        private string ComputeDigestValue(DigestAlgorithm digestAlgorithm)
        {
            MemoryStream memoryStream = mReferenceResolver.Resolve(this);
            byte[] dataBytes = TransformCollection.Apply(memoryStream).ToArray();

            byte[] hash = HashUtil.ComputeHash(CryptoUtilPal.CreateHashAlgorithm(digestAlgorithm), dataBytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Name of document part combined with MIME type.
        /// Name could be treated as stream name for DOC and part name for DOCX, ODT.
        /// </summary>
        internal string Uri
        {
            get { return mUri; }
            set { mUri = value; }
        }

        /// <summary>
        /// Only name of document part without the MIME type.
        /// Additionally Name is decoded from XmlEncoding to Unicode.
        /// </summary>
        internal string Name
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return XmlEntities.Expand(mUri).Split('?')[0]; }
        }

        /// <summary>
        /// Base64 encoded SHA1 hash of referenced document part.
        /// </summary>
        /// <remarks>
        /// Seems that Word uses only SHA1 hash so type of hash isn't stored.
        /// </remarks>
        internal string DigestValue
        {
            get { return mDigestValue; }
            set { mDigestValue = value; }
        }

        internal string DigestMethod
        {
            get { return mDigestMethod; }
            set { mDigestMethod = value; }
        }

        /// <summary>
        /// Transform collection for referenced document part. Should be applied before hash calculation.
        /// </summary>
        internal TransformCollection TransformCollection
        {
            get { return mTransformCollection; }
        }

        /// <summary>
        /// Indicates whether reference is external.
        /// </summary>
        internal bool IsExternal
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return !mUri.StartsWith("#", StringComparison.Ordinal); }
        }

        /// <summary>
        /// Reference verification result.
        /// </summary>
        internal bool IsValid
        {
            get { return mIsValid; }
            set { mIsValid = value; }
        }

        /// <summary>
        /// IReferenceResolver object used to resolve this Reference.
        /// </summary>
        internal IReferenceResolver ReferenceResolver
        {
            get { return mReferenceResolver; }
            set { mReferenceResolver = value; }
        }

        /// <summary>
        /// Entities given string. Currently replaces only 0x0001..0x0009 characters. This seem to be enough.
        /// </summary>
        private static string Entitize(string uri)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < uri.Length; i++)
                if (('\x0001' <= uri[i]) && (uri[i] <= '\x0009'))
                    sb.AppendFormat("&#{0};", (char)('0' + uri[i]));
                else
                    sb.Append(uri[i]);

            return sb.ToString();
        }

#if DEBUG
        public override string ToString()
        {
            return String.Format("Reference [{0}, {1}, {2}]",
                (IsExternal ? "External" : "Internal"), (mIsValid ? "Valid" : "Not valid"), Name);
        }
#endif

        private IReferenceResolver mReferenceResolver;
        private readonly TransformCollection mTransformCollection = new TransformCollection();
        private string mUri;
        private string mDigestValue;
        private string mDigestMethod;
        private bool mIsValid;
    }
}
