// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2006 by Roman Korchagin
using System;
using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Ss;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.RW.Ole;
using Aspose.Words.RW.Ole.Ole2;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Provides access to the data of an OLE object or ActiveX control.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-ole-objects/">Working with Ole Objects</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="Shape.OleFormat"/> property to access the data of an OLE object.
    /// You do not create instances of the <see cref="OleFormat"/> class directly.</p>
    ///
    /// <seealso cref="Shape.OleFormat"/>
    /// </remarks>
    public class OleFormat
    {
        internal OleFormat(IShapeAttrSource parent)
        {
            Debug.Assert(parent != null);
            mParent = parent;
        }

        /// <summary>
        /// Saves the data of the embedded object into the specified stream.
        /// </summary>
        /// <remarks>
        /// <p>It is the responsibility of the caller to dispose the stream.</p>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Throws if you attempt to save a linked object.</exception>
        /// <param name="stream">Where to save the object data.</param>
        /// <javaName>void save(java.io.OutputStream stream)</javaName>
#if PLAIN_JAVA    // WORDSJAVA-25685 - Saving to OutputStream always writes to memory first
        //JAVA-added public wrapper for internalized member
        public void save(java.io.OutputStream stream) throws Exception
        {
            MemoryStream tempStream = new MemoryStream();
            save(tempStream);
            tempStream.setPosition(0);
            com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyStream(tempStream, stream);
        }
#endif
        [JavaInternal]
        public void Save([CppIOStreamWrapper(IOStreamType.OStream)]Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (IsLink)
                throw new InvalidOperationException("This OLE object is linked, and cannot be saved. Only embedded OLE objects can be saved.");

            IEmbeddedObject embeddedObject = EmbeddedObject;
            if (embeddedObject == null)
                throw new InvalidOperationException("There is no OLE or OOXML object for saving.");

            embeddedObject.SaveForUser(stream, mParent);
        }

        /// <summary>
        /// Saves the data of the embedded object into a file with the specified name.
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if you attempt to save a linked object.</exception>
        /// <param name="fileName">Name of the file to save the OLE object data.</param>
        public void Save(string fileName)
        {
            using (Stream stream = File.Create(fileName))
                Save(stream);
        }

        /// <summary>
        /// Gets OLE object data entry.
        /// </summary>
        /// <param name="oleEntryName">Case-sensitive name of the OLE data stream.</param>
        /// <returns>An OLE data <ms>stream</ms><java>byte array</java><cpp>stream</cpp> or <c>null</c>.</returns>
        /// <javaName>byte[] getOleEntry(java.lang.String oleEntryName)</javaName>
#if PYNET
        public Stream GetOleEntry(string oleEntryName)
#else
        public MemoryStream GetOleEntry(string oleEntryName)
#endif
        {
            if (OleObject != null)
                return OleObject.Data.GetStreamSafe(oleEntryName);
            else
                return null;
        }

        /// <summary>
        /// Gets OLE object raw data.
        /// </summary>
        public byte[] GetRawData()
        {
            if (OleObject != null)
            {
                FileSystem fs = new FileSystem(OleObject.Data);
                MemoryStream raw = new MemoryStream();
                fs.Save(raw);

                return raw.ToArray();
            }

            if (OoxmlObject != null)
            {
                MemoryStream raw = new MemoryStream((int)OoxmlObject.Stream.Length);
                StreamUtil.CopyStream(OoxmlObject.Stream, raw);

                return raw.ToArray();
            }

            return null;
        }


        /// <summary>
        /// Sets the draw aspect of the OLE object to a specified value.
        /// </summary>
        /// <remarks>
        /// This is made internal because I don't want it public, see comment in <see cref="OleIcon"/>.
        /// </remarks>
        internal void SetOleIcon(bool isIcon)
        {
            SetAttrIfNotDefault(ShapeAttr.OleIcon, isIcon);
        }

        [JavaConvertCheckedExceptions]
        private void UpdateDrawAspectFromOleObject()
        {
            if (OleObject != null)
            {
                ObjInfoStream objInfoStream = ObjInfoStream.Read(OleObject.Data);

                if (objInfoStream != null)
                    SetOleIcon(objInfoStream.IsIcon);
            }
        }

        private object FetchAttr(int key)
        {
            return mParent.FetchShapeAttr(key);
        }

        private void SetAttrIfNotDefault(int key, object value)
        {
            if (value != null && !value.Equals(ShapePr.FetchDefaultAttr(key)))
                mParent.SetShapeAttr(key, value);
            else
                mParent.RemoveShapeAttr(key);
        }

        private void SetAttr(int key, object value)
        {
            mParent.SetShapeAttr(key, value);
        }

        /// <summary>
        /// Gets icon caption of OLE object.
        /// <para>In case if the OLE object does not have an icon or a caption cannot be retrieved, returns an empty
        /// string.</para>
        /// </summary>
        public string IconCaption
        {
            get
            {
                // FOSS simplified.

                if (OleIcon)
                {
                    // AM. Have seen JPEG image inserted as OLE object so we can not parse icon label from it.
                    // Try to get name from Object Packager data if it's possible.
                    OleObject oleObject = EmbeddedObject as OleObject;
                    if ((oleObject != null) && oleObject.IsOle1)
                        return oleObject.GetOriginalFileNameFromOle1();
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the file extension suggested for the current embedded object if you want to save it into a file.
        /// </summary>
        public string SuggestedExtension
        {
            get
            {
                IEmbeddedObject embeddedObject = EmbeddedObject;
                return (embeddedObject != null) ? embeddedObject.GetExtensionForUser(ProgId) : "";
            }
        }

        /// <summary>
        /// Gets the file name suggested for the current embedded object if you want to save it into a file.
        /// </summary>
        public string SuggestedFileName
        {
            get
            {
                IEmbeddedObject embeddedObject = EmbeddedObject;
                return (embeddedObject != null) ? embeddedObject.GetFileNameForUser() : "";
            }
        }

        /// <summary>
        /// Gets or sets the ProgID of the OLE object.
        /// </summary>
        /// <remarks>
        /// <para>The ProgID property is not always present in Microsoft Word documents and cannot be relied upon.</para>
        /// <para>Cannot be <c>null</c>.</para>
        /// <p>The default value is an empty string.</p>
        /// </remarks>
        public string ProgId
        {
            get
            {
                IEmbeddedObject embeddedObject = EmbeddedObject;
                if ((embeddedObject != null) && embeddedObject.IsForms2OleControlInternal)
                    return Forms2OleControl.GetProgId(embeddedObject.ClsidInternal);

                return (string)FetchAttr(ShapeAttr.OleProgID);
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(ShapeAttr.OleProgID, value);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the OLE object is linked (when <see cref="SourceFullName"/> is specified).
        /// </summary>
        public bool IsLink
        {
            get { return StringUtil.HasChars(SourceFullName); }
        }

        /// <summary>
        /// Gets or sets the path and name of the source file for the linked OLE object.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        ///
        /// <p>If <see cref="SourceFullName"/> is not an empty string, the OLE object is linked.</p>
        /// </remarks>
        public string SourceFullName
        {
            get
            {
                string sourceFullName = (string)FetchAttr(ShapeAttr.OleSourceFullName);

                if(!StringUtil.HasChars(sourceFullName) && (OlePackage != null) && OlePackage.IsLink)
                    sourceFullName = OlePackage.FileName;

                return sourceFullName;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(ShapeAttr.OleSourceFullName, value);
            }
        }

        /// <summary>
        /// Gets or sets a string that is used to identify the portion of the source file that is being linked.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        ///
        /// <p>For example, if the source file is a Microsoft Excel workbook, the <see cref="SourceItem"/>
        /// property might return "Workbook1!R3C1:R4C2" if the OLE object contains only a few cells from
        /// the worksheet.</p>
        /// </remarks>
        public string SourceItem
        {
            get { return (string)FetchAttr(ShapeAttr.OleSourceItem); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(ShapeAttr.OleSourceItem, value);
            }
        }

        /// <summary>
        /// Specifies whether the link to the OLE object is automatically updated or not in Microsoft Word.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool AutoUpdate
        {
            get { return (bool)FetchAttr(ShapeAttr.OleAutoUpdate); }
            set { SetAttr(ShapeAttr.OleAutoUpdate, value); }
        }

        /// <summary>
        /// Gets the draw aspect of the OLE object. When <c>true</c>, the OLE object is displayed as an icon.
        /// When <c>false</c>, the OLE object is displayed as content.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not allow to set this property to avoid confusion. If you were able to change
        /// the draw aspect in Aspose.Words, Microsoft Word would still display the OLE object in its original
        /// draw aspect until you edit or update the OLE object in Microsoft Word.</para>
        /// </remarks>
        public bool OleIcon
        {
            get { return (bool)FetchAttr(ShapeAttr.OleIcon); }
        }

        /// <summary>
        /// Specifies whether the link to the OLE object is locked from updates.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool IsLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.OleLocked); }
            set { SetAttr(ShapeAttr.OleLocked, value); }
        }

        /// <summary>
        /// Gets the CLSID of the OLE object.
        /// </summary>
        public Guid Clsid
        {
            get
            {
                IEmbeddedObject embeddedObject = EmbeddedObject;
                return (embeddedObject != null) ? new Guid(embeddedObject.ClsidInternal) : Guid.Empty;
            }
        }

        /// <summary>
        /// Provide access to <see cref="Drawing.OlePackage" /> if OLE object is an OLE Package.
        /// Returns <c>null</c> otherwise.
        /// </summary>
        /// <remarks>
        /// OLE Package is a legacy technology that allows to wrap any file format not present in the OLE registry of
        /// a Windows system into a generic package allowing to embed almost anything into a document.
        /// See <see cref="Drawing.OlePackage"/> type for more info.
        /// </remarks>
        /// <dev>
        ///  Has meaning only if ProgId equal to "Package" otherwise is null.
        /// </dev>
        public OlePackage OlePackage
        {
            get
            {
                if (mOlePackageCache == null)
                {
                    // WORDSNET-15758 System.InvalidCastException is thrown while loading DOC file.
                    // Here EmbeddedObject may be OoxmlObject if OLE object isn't structured.
                    if ((ProgId == OleRegistryInfo.GetOlePackage().ProgId) && (EmbeddedObject is OleObject))
                    {
                        OleObject oleObject = (OleObject)EmbeddedObject;
                        mOlePackageCache = new OlePackage(oleObject);
                        Ole10NativeStream ole10NativeStream = Ole10NativeStream.Read(oleObject.Data);
                        if (ole10NativeStream != null)
                        {
                            Encoding encoding = Encoding.GetEncoding(OlePackage.DefaultCodePage);
                            using (MemoryStream memoryStream = new MemoryStream(ole10NativeStream.NativeData))
                            using (BinaryReader reader = new BinaryReader(memoryStream, encoding))
                            {
                                mOlePackageCache.Read(reader);
                            }
                        }
                    }
                }
                return mOlePackageCache;
            }
        }

        /// <summary>
        /// Gets <see cref="OleControl"/> objects if this OLE object is an ActiveX control. Otherwise this property is null.
        /// </summary>
        /// <dev>
        /// If value is binary OleObject and it contains Forms2OleControl, then it means we have unparsed
        /// Forms2OleControl in ShapePr[ShapeAttr.OleObject]. In this case we should parse it and replace
        /// with new parsed OleControl created from this unparsed OleObject.
        /// </dev>
        public OleControl OleControl
        {
            get
            {
                IEmbeddedObject embeddedObject = EmbeddedObject;
                OleControl oleControl = embeddedObject as OleControl;
                if (oleControl != null)
                    return oleControl;

                // Ole control can be unparsed OleObject. Try to parse it here.
                OleObject oleObject = embeddedObject as OleObject;
                if (oleObject != null)
                {
                    oleControl = OleControl.Create(oleObject.Data);

                    // If OleObject was successfully parsed, then we need to update shape with the new parsed control.
                    if (oleControl != null)
                        SetAttr(ShapeAttr.OleObject, oleControl);

                    return oleControl;
                }

                return null;
            }
        }

        /// <summary>
        /// Provides access to the raw data of the embedded (and sometimes linked) OLE or OOXML object.
        ///
        /// Unfortunately, linked objects in DOC also have data, but linked objects in WML do not have it.
        /// So this property will not be set (return <c>null</c>) for linked objects loaded from WML, but not null for linked
        /// objects loaded from DOC. Same applies to RTF.
        /// </summary>
        internal IEmbeddedObject EmbeddedObject
        {
            get { return (IEmbeddedObject)FetchAttr(ShapeAttr.OleObject); }
            set
            {
                SetAttrIfNotDefault(ShapeAttr.OleObject, value);

                // WORDSNET-15616 Need to rebuild OlePackage after changing EmbeddedObject.
                mOlePackageCache = null;

                // We need to update the boolean OleIcon (DrawAspect) by getting data from the OLE object if possible
                // because it seems to be the most reliable source of this information.
                UpdateDrawAspectFromOleObject();
            }
        }

        /// <summary>
        /// Safely gets the embedded object as an OLE object. If there is no embedded object or it is an OOXML object, returns <c>null</c>.
        /// </summary>
        internal OleObject OleObject
        {
            get { return EmbeddedObject as OleObject; }
        }

        /// <summary>
        /// Safely gets the embedded object as an OOXML package. If there is no embedded object or it is an OLE object, returns <c>null</c>.
        /// </summary>
        internal OoxmlObject OoxmlObject
        {
            get { return EmbeddedObject as OoxmlObject; }
        }

        /// <summary>
        /// It will be good to refactor this to remove at some time in the future.
        /// </summary>
        internal int OleTxid
        {
            get { return (int)FetchAttr(ShapeAttr.OleTxid); }
            set { SetAttr(ShapeAttr.OleTxid, value); }
        }

        /// <summary>
        /// Gets or sets the type of the link for the OLE object.
        /// </summary>
        internal OleLinkType OleLinkType
        {
            get { return (OleLinkType)FetchAttr(ShapeAttr.OleLinkType); }
            set { SetAttr(ShapeAttr.OleLinkType, value); }
        }

        /// <summary>
        /// Returns true if this OLE is linked and has no embedded data.
        /// </summary>
        internal bool IsLinkNoData
        {
            get { return ((IsLink) && (EmbeddedObject == null)); }
        }

        /// <summary>
        /// Returns true if the OLE object is physically stored as an embedded document part.
        /// </summary>
        internal bool NeedEmbeddedPart
        {
            get { return !IsLink || (OlePackage != null); }
        }

        /// <summary>
        /// This is a number used in the \f field switch.
        /// The following values are documented at this stage:
        /// 0 Maintain the formatting of the source file.
        /// 1 Not supported
        /// 2 Match the formatting of the destination document.
        /// 3 Not supported
        /// 4 Maintain the formatting of the source file, if the source file is an Excel workbook.
        /// 5 Match the formatting of the destination document if the source is an Excel workbook.
        ///
        /// Lets keep it integer for now. Don't see a need for enum yet.
        /// </summary>
        internal int FormatUpdateType
        {
            get { return (int)FetchAttr(ShapeAttr.OleFormatUpdateType); }
            set { SetAttr(ShapeAttr.OleFormatUpdateType, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IShapeAttrSource mParent;
        private OlePackage mOlePackageCache;
   }
}
