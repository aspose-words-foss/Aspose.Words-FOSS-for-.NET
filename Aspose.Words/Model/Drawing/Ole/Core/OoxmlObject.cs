// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/04/2010 by Roman Korchagin

using System;
using System.IO;
using Aspose.IO;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Ole;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Contains data of an OOXML package embedded in a document.
    /// </summary>
    internal class OoxmlObject : IEmbeddedObject
    {
        /// <summary>
        /// Creates instance of the <see cref="OoxmlObject"/> object from a stream.
        /// </summary>
        internal OoxmlObject(Stream stream, string contentType, string name, Guid clsid)
            : this(stream, contentType, string.Empty)
        {
            mName = name;
            mClsid = clsid;
        }

        /// <summary>
        /// Creates instance of the <see cref="OoxmlObject"/> object from a stream.
        /// </summary>
        internal OoxmlObject(Stream stream, string contentType, string partName)
        {
            Debug.Assert(partName != null);

            mId = UniqueIdManager.GenerateInteger();

            Stream = stream;
            ContentType = contentType;
            mClsid = Guid.Empty;
            mPartName = partName;
        }

        /// <summary>
        /// Returns a name of the embedded object.
        /// </summary>
        string IEmbeddedObject.GetName()
        {
            return mName;
        }

        /// <summary>
        /// Returns instance of the <see cref="OleObject"/> object.
        /// </summary>
        OleObject IEmbeddedObject.GetOleObject()
        {
            return null;
        }

        /// <summary>
        /// Returns the extension (with the leading dot).
        /// The extension must be for the inner (e.g. unwrapped) data that can be saved directly to a file.
        /// </summary>
        string IEmbeddedObject.GetExtensionForUser(string progId)
        {
            return DocxEnum.ContentTypeToExtension(ContentType);
        }

        /// <summary>
        /// Returns the file name.
        /// </summary>
        string IEmbeddedObject.GetFileNameForUser()
        {
            // WORDSNET-24632 Return file name from the package part.
            return Path.GetFileNameWithoutExtension(mPartName);
        }

        /// <summary>
        /// Saves the embedded object data in a way that makes it a valid standalone file.
        /// </summary>
        void IEmbeddedObject.SaveForUser(Stream dstStream, IShapeAttrSource attrSource)
        {
            bool needsUnhide =
                (ContentType == DocxContentType.Xlsx) ||
                (ContentType == DocxContentType.Xlsm) ||
                (ContentType == DocxContentType.Xltx) ||
                (ContentType == DocxContentType.Xltm);

            // WORDSNET-14899 Skip unhiding of a workbook when "DrawAspect" attribute has "Content" value.
            needsUnhide &= (bool)attrSource.FetchShapeAttr(ShapeAttr.OleIcon);

            // Unhide workbook if needed and copy to destination stream.
            if (needsUnhide)
            {
                OleUtil.UnhideOoxmlExcelWorkbook(Stream, dstStream);
            }
            else
            {
                Stream.Position = 0;
                StreamUtil.CopyStream(Stream, dstStream);
            }
        }

        /// <summary>
        /// Gets a boolean value, indicating either the embedded object is Forms2Ole control.
        /// </summary>
        bool IEmbeddedObject.IsForms2OleControlInternal
        {
            get { return OleControl.IsForms2OleClsid(((IEmbeddedObject)this).ClsidInternal); }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        string IEmbeddedObject.ClsidInternal
        {
            get { return mClsid.ToString(); }
        }

        /// <summary>
        /// Gets or sets Id of the embedded object.
        /// </summary>
        int IEmbeddedObject.Id
        {
            get { return mId; }
            set { mId = value; }
        }

        bool IEmbeddedObject.IsEmpty
        {
            get { return Stream.Length == 0; }
        }

        /// <summary>
        /// The Id of the embedded object.
        /// </summary>
        /// <remarks>
        /// Each embedded object, when stored in a file, has a unique id.
        /// Looks like RTF does not use this.
        /// </remarks>
        private int mId;

        /// <summary>
        /// Gets a stream of the OOXML object.
        /// </summary>
        internal Stream Stream { get; }

        /// <summary>
        /// Gets a type of the content of the OOXML object.
        /// </summary>
        internal string ContentType { get; }

        private readonly string mName;
        private readonly Guid mClsid;
        private readonly string mPartName;
    }
}
