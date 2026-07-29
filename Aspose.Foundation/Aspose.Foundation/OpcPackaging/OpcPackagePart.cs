// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2007 by Vladimir Averkin
using System;
using System.IO;
using Aspose.Common;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.OpcPackaging
{
    public class OpcPackagePart
    {
        public OpcPackagePart(string partName, string contentType)
        {
            ArgumentUtil.CheckHasChars(partName, "partName");

            mName = partName;
            mContentType = contentType;
            mStream = new MemoryStream();

            mRels = new OpcRelationshipCollection(partName);
        }

        /// <summary>
        /// Retrieves the target for the specified relationship id.
        /// If the target is external, removes the file:/// prefix.
        /// If the target is internal, returns the absolute part name.
        /// </summary>
        public string GetRelationshipTarget(string relId)
        {
            if (!StringUtil.HasChars(relId))
                return "";

            OpcRelationship rel = mRels.GetById(relId);
            if (rel == null)
                return "";

            if (rel.IsExternal)
            {
                string target = rel.Target;
                // RK File paths are stored with the protocol prefix in DOCX, but without it
                // in the model and other formats, therefore it is best to remove it.
                if (UriUtil.HasFileScheme(target))
                {
                    target = UriUtil.RemoveFileSchemePrefix(target);
                    target = UriUtil.UnescapeHref(target);
                }
                return target;
            }
            else
            {
                // An internal relationship.
                if (UriUtil.IsSubAddressOnly(rel.Target))
                {
                    // I think this is used when target is a bookmark. In this case just return the name as is.
                    return rel.Target;
                }
                else
                {
                    // The target is another part, return its absolute name.
                    return GetRelatedPartName(rel);
                }
            }
        }

        /// <summary>
        /// Gets the absolute name of the related (child) part from the specified relationship.
        /// The relationship must be from the relationship collection of this part.
        /// The relationship must be internal.
        /// </summary>
        public string GetRelatedPartName(OpcRelationship rel)
        {
            if (rel.IsExternal)
                throw new InvalidOperationException("An internal target is expected here.");

            return OpcPackageBase.MakeAbsolute(mName, rel.Target);
        }

        /// <summary>
        /// Creates a deep copy of the part and copies its content data stream.
        /// </summary>
        [JavaConvertCheckedExceptions]
        public OpcPackagePart Clone()
        {
            OpcPackagePart lhs = (OpcPackagePart)MemberwiseClone();

            lhs.mRels = mRels.Clone();

            // andrnosk: WORDSNET-9372 The same stream is used upon cloning in multiple threads,
            // this leads incorrect Theme part writing. Using lock we avoid this.
            lock (mLocker)
            {
                lhs.Stream = new MemoryStream();
                Stream.Position = 0;
                StreamUtil.CopyStream(Stream, lhs.Stream);

                lhs.mLocker = new object();

                return lhs;
            }
        }

        /// <summary>
        /// The name of the part (with name path).
        ///
        /// This seems to corresponds to "part name" as per the OPC specification,
        /// that is "/segment1/segment2" (starts with "/").
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// The extension of the part name (without the dot).
        /// </summary>
        public string Extension
        {
            get { return Path.GetExtension(mName).TrimStart('.'); }
        }

        /// <summary>
        /// The MIME type of the content stream.
        /// </summary>
        public string ContentType
        {
            get { return mContentType; }
            set { mContentType = value; }
        }

        /// <summary>
        /// The content data stream of the part.
        ///
        /// If the package is loaded into memory (normal method), then this is a MemoryStream.
        /// But if the package is loaded using the memory-optimized method, then this is a FileStream of a temporary file.
        /// </summary>
        public Stream Stream
        {
            get { return mStream; }
            set { mStream = value; }
        }

        /// <summary>
        /// This is part of the memory-optimized method. Allows to get the part as a memory stream.
        /// </summary>
        public MemoryStream GetAsMemoryStream()
        {
            mStream.Position = 0;

            Stream stream = CompressedData.GetStream(mStream);

            return StreamUtil.CopyStreamIfNotMemory(stream);
        }

        /// <summary>
        /// Relationships for this package part.
        /// </summary>
        public OpcRelationshipCollection Rels
        {
            get { return mRels; }
        }

        /// <summary>
        /// LastModified property of the associated entry.
        /// </summary>
        internal DateTime ZipEntryLastModified
        {
            get { return mZipEntryLastModified; }
            set { mZipEntryLastModified = value; }
        }

        private object mLocker = new object();
        private readonly string mName;
        private string mContentType;
        private Stream mStream;
        private OpcRelationshipCollection mRels;
        private DateTime mZipEntryLastModified;
    }

}
