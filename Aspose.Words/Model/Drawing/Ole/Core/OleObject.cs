// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2006 by Roman Korchagin
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.IO;
using Aspose.Ss;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.RW.Ole;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Contains data of an OLE object embedded in a document.
    /// </summary>
    internal class OleObject : IEmbeddedObject
    {
        /// <summary>
        /// Creates instance of <see cref="OleObject"/> object from a storage.
        /// </summary>
        internal OleObject(MemoryStorage data) : this(UniqueIdManager.GenerateInteger(), data)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="OleObject"/> object from a storage.
        /// </summary>
        internal OleObject(int id, MemoryStorage data)
        {
            Debug.Assert(data != null);
            mData = data;

            mId = id;
        }

        /// <summary>
        /// Returns a file name from OLE1 package.
        /// </summary>
        internal string GetOriginalFileNameFromOle1()
        {
            if (!IsOle1)
                return string.Empty;

            BinaryReader reader = CreateOle10NativeReader();
            return OleUtil.GetOriginalFileNameFromOle1(reader);
        }

        /// <summary>
        /// Office documents can be attached in an old "97-2003" format with the progId of a greater Office version.
        /// As we save them as is it would be better to correct the extension.
        /// </summary>
        private string CorrectOfficeExtension(string fileExt)
        {
            if (IsOfficeExt(fileExt) && Data != null)
            {
                if (Data.ContainsKey("Workbook"))
                    return ".xls";
                if (Data.ContainsKey("WordDocument"))
                    return ".doc";
                if (Data.ContainsKey("PowerPoint Document"))
                    return ".ppt";
                if (Data.ContainsKey("VisioDocument"))
                    return ".vsd";
            }

            return fileExt;
        }

        /// <summary>
        /// Returns true, if a specified string is one of the Office extensions.
        /// </summary>
        private static bool IsOfficeExt(string fileExt)
        {
            if (fileExt == ".xlsx" || fileExt == ".docx" || fileExt == ".pptx" || fileExt == ".vsdx")
                return true;

            return false;
        }

        /// <summary>
        /// Gets a file extension for Outlook by a specified progId.
        /// </summary>
        private string GetExtensionForOutlookAttachment(string progId)
        {
            if (progId.StartsWith("MailMsgAtt", StringComparison.Ordinal) ||
                progId.StartsWith("Outlook.MsgAttach", StringComparison.Ordinal))
                return ".msg"; // maybe

            if (progId.StartsWith("Outlook.FileAttach", StringComparison.Ordinal))
            {
                AttachDescStream attachDescStream = AttachDescStream.Read(Data);
                if ((attachDescStream != null) && StringUtil.HasChars(attachDescStream.Extension))
                    return attachDescStream.Extension;
            }

            return "";
        }

        /// <summary>
        /// Returns a name of the embedded object.
        /// </summary>
        string IEmbeddedObject.GetName()
        {
            Debug.Assert(mData != null);

            OcxNameStream ocxNameStream = OcxNameStream.Read(mData);
            if (ocxNameStream == null)
                return null;

            return ocxNameStream.Value;
        }

        /// <summary>
        /// Returns instance of <see cref="OleObject"/> object.
        /// </summary>
        OleObject IEmbeddedObject.GetOleObject()
        {
            return this;
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        string IEmbeddedObject.ClsidInternal
        {
            get { return mData.Clsid.ToString(); }
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
            get { return false; }
        }

        /// <summary>
        /// Returns the extension (with the leading dot).
        /// The extension must be for the inner (e.g. unwrapped) data that can be saved directly to a file.
        /// </summary>
        string IEmbeddedObject.GetExtensionForUser(string progId)
        {
            if (!string.IsNullOrEmpty(progId))
            {
                // Try to look for ProgId in Registry database.
                OleRegistryInfo oleInfo = OleRegistryInfo.GetByProgId(progId);
                string extension = CorrectOfficeExtension(oleInfo.FileExt);
                if (StringUtil.HasChars(extension))
                    return extension;

                // Now try to get the extension of the Outlook attachment.
                extension = GetExtensionForOutlookAttachment(progId);
                if (!string.IsNullOrEmpty(extension))
                    return extension;
            }

            // Try to get the extension from OLE10 native data.
            string originalFileName = GetOriginalFileNameFromOle1();
            if (!string.IsNullOrEmpty(originalFileName))
                return Path.GetExtension(originalFileName).ToLower();

            // Try to get the extension by clsID.
            return OleRegistryInfo.GetExtensionByClsId(Data.Clsid);
        }

        /// <summary>
        /// Returns the file name.
        /// </summary>
        string IEmbeddedObject.GetFileNameForUser()
        {
            AttachDescStream attachDesc = AttachDescStream.Read(mData);

            if (attachDesc != null)
                return attachDesc.FileName;

            return Path.GetFileNameWithoutExtension(GetOriginalFileNameFromOle1());
        }


        /// <summary>
        /// Saves the embedded object data in a way that makes it a valid standalone file.
        /// </summary>
        void IEmbeddedObject.SaveForUser(Stream stream, IShapeAttrSource attrSource)
        {
            string progId = (string)attrSource.FetchShapeAttr(ShapeAttr.OleProgID);

            if (IsOle1)
                SaveOle1Unwrap(stream, progId);
            else
                SaveOle2Unwrap(stream, progId);
        }

        /// <summary>
        /// Gets a boolean value, indicating either the embedded object is Forms2Ole control.
        /// </summary>
        bool IEmbeddedObject.IsForms2OleControlInternal
        {
            get { return OleControl.IsForms2OleClsid(((IEmbeddedObject)this).ClsidInternal); }
        }

        /// <summary>
        /// Tries to extract the native data without OLE headers and saves it to the stream.
        /// </summary>
        private void SaveOle1Unwrap(Stream dstStream, string progId)
        {
            if (StringUtil.StartsWithOrdinalIgnoreCase(progId, molProgId) &&
                UnwrapMol(dstStream))
                return;

            BinaryReader reader = CreateOle10NativeReader();
            if (reader == null)
                return;

            Ole10Header header = new Ole10Header(reader);
            // WORDSNET-3576 “System.OutOfMemoryException” occurs during extracting OLE10 object from document.
            // RK This file has OLE10 data in some other format, probably corrupted as MS Word does not like
            // it much either. MS Word even crashes sometimes.
            if (header.Header1 == 0x0002)
            {
                switch (header.Header2)
                {
                    case 0x0001:
                    {
                        // WORDSNET-10087 Ole10NativeStream ends with filename2 if header2 is 0x0001, so there is no content.
                        break;
                    }
                    case 0x0003:
                    {
                        // Read content.
                        byte[] content = new byte[header.ContentLength];
                        reader.Read(content, 0, content.Length);
                        dstStream.Write(content, 0, content.Length);
                        break;
                    }
                    default:
                    {
                        // Unknown value, stop reading and write nothing into the output stream.
                        break;
                    }
                }
            }
            else
            {
                // We don't know how to parse this OLE data, simply save it as is, but without the length header.
                Ole10NativeStream.Position = 4;
                StreamUtil.CopyStream(Ole10NativeStream, dstStream);
            }
        }

        /// <summary>
        /// Returns OLE1 reader or null if there is nothing to read.
        /// </summary>
        private BinaryReader CreateOle10NativeReader()
        {
            const int codePage = 1251;

            Debug.Assert(IsOle1);

            // WORDSNET-10249 The problem occurred because there are Ole10NativeStream with zero length in the document.
            // Write nothing into the output stream in such case.
            if (Ole10NativeStream.Length == 0)
                return null;

            // WORDSNET-10248 The problem occurred because in the document there are two OLE objects, which refer to the same Ole10NativeStream.
            // So to resolve the issue set stream position to 0 before reading it.
            Ole10NativeStream.Position = 0;

            return new BinaryReader(Ole10NativeStream, Encoding.GetEncoding(codePage));
        }

        /// <summary>
        /// Saves OLE2 embedded data. If progId is known to be one of the types we can unwrap,
        /// unwraps it to save only document data.
        /// </summary>
        /// <remarks>
        /// AM. I think we should refactor all this extraction using information from OleRegistryInfo class.
        /// Too busy now, postpone for a while.
        /// </remarks>
        private void SaveOle2Unwrap(Stream stream, string progId)
        {
            Debug.Assert(!IsOle1);

            // If data doesn't exist, then there is nothing to save.
            if (mData == null)
                return;

            OleRegistryInfo oleInfo = OleRegistryInfo.GetByProgId(progId);
            // ProgId data may be empty or invalid. In this case, we determine OleRegistryInfo by CLSID.
            if (StringUtil.EqualsOrdinalIgnoreCase(oleInfo.ClsId, OleRegistryInfo.PackageClsId))
                oleInfo = OleRegistryInfo.GetByClsId(mData.Clsid);

            MemoryStream contentsStream = null;
            MemoryStorage contentsStorage = null;
            if (oleInfo.ProgId.StartsWith("Excel.Sheet.8", StringComparison.Ordinal))
            {
                Stream workbookStream = mData.GetStreamSafe("Workbook");

                // WORDSNET-9995 The XLS file appears hidden in Excel after extracting. We have to unhide it before saving.
                OleUtil.UnhideExcel97Workbook(workbookStream);
            }
            else if (IsExcel12(oleInfo.ProgId))
            {
                // WORDSNET-8477 Perform these checks to keep contentsStream equal to null,
                // if mData do not have proper stream.
                if (mData.ContainsKey("Package"))
                {
                    if (oleInfo.ProgId == "Excel.SheetBinaryMacroEnabled.12")
                    {
                        // AM. Do not "unhide" Excel Binary Workbook because output is corrupted.
                        // Will investigate it later.
                        contentsStream = mData.GetStreamSafe("Package");
                    }
                    else
                    {
                        MemoryStream package = mData.GetStreamSafe("Package");
                        package.Position = 0;

                        // WORDSNET-26316 Detect OLE object embedded in nested package.
                        if (FileSystem.IsStructuredStorage(package))
                            package = new FileSystem(package).Root.GetStreamSafe("package");

                        contentsStream = new MemoryStream();

                        // andrnosk: WORDSNET-6032 The XLSX file appears hidden in Excel after extracting.
                        // We have to unhide it before saving.
                        OleUtil.UnhideOoxmlExcelWorkbook(package, contentsStream);
                    }
                }
                else if (mData.ContainsKey("Workbook"))
                {
                    // WORDSNET-8477 We should rely on mData content to be able to unwrap Excel97Workbook.
                    OleUtil.UnhideExcel97Workbook(mData.GetStreamSafe("Workbook"));
                }
            }
            else if ((oleInfo.EmbeddingType == OleEmbeddingType.Ole2StreamPackage))
            {
                // WORDSNET-12589 The XLSX file is wrapped into a structured storage. We need to extract it.
                contentsStream = mData.GetStreamSafe("Package");
            }
            else if ((oleInfo.EmbeddingType == OleEmbeddingType.Ole2StreamEmbeddedOdf))
            {
                contentsStream = mData.GetStreamSafe("EmbeddedOdf");
            }
            else if (progId.StartsWith("Outlook.FileAttach", StringComparison.Ordinal))
            {
                // WORDSNET-3983 The extracted embedded OLE object is corrupted.
                // RK The object is Outlook.FileAttach that contains the actual data (another OLE object)
                // inside it. The customers want to extract the inner object that is inside the attachment.
                // So I think it is fair to automatically unwrap Outlook attachment objects.
                contentsStream = mData.GetStreamSafe("AttachContents");
            }
            else if ((oleInfo.ProgId.StartsWith("AcroExch.Document", StringComparison.Ordinal) ||
                      oleInfo.ProgId.StartsWith("Acrobat.Document", StringComparison.Ordinal)))
            {
                // WORDSNET-5238 Extracted PDF document can’t be opened by Adobe PDF Reader 7.
                contentsStream = mData.GetStreamSafe("CONTENTS");
            }
            else if ((progId.StartsWith("MailMsgAtt", StringComparison.Ordinal) ||
                progId.StartsWith("Outlook.MsgAttach", StringComparison.Ordinal)))
            {
                // WORDSNET-10314 Unwrap MailMsgAtt OLE objects, so the extracted file can be opened by outlook.
                // WORDSNET-4542 The same problem with Outlook.MsgAttach OLE objects. Fixed by adding one more condition.
                contentsStorage = mData.GetStorageSafe("MAPIMessage");
            }
            else if (progId.StartsWith("ISISServer", StringComparison.Ordinal))
            {
                // MOL - Chemical table file
                if (UnwrapMol(stream))
                    return;
            }
            else if ((oleInfo.EmbeddingType == OleEmbeddingType.Ole1Package))
            {
                // WORDSNET-27408 Extract OLE content from nested streams.
                if (oleInfo.ProgId == "PowerPoint.Slide.12")
                    contentsStream = mData.GetStreamSafe("Package");
                else if(oleInfo.ProgId == "WordPad.Document.1")
                    contentsStream = mData.GetStreamSafe("Contents");
            }

            if (contentsStream != null)
            {
                // We have an "unwrapped contents" stream, write it out.
                contentsStream.Position = 0;
                StreamUtil.CopyStream(contentsStream, stream);
            }
            else
            {
                SaveOle2NoUnwrap(stream, contentsStorage != null ? contentsStorage : mData);
            }
        }

        /// <summary>
        /// Extracts data of a Chemical table file (MOL) stored in this OLE object into the stream if possible.
        /// </summary>
        private bool UnwrapMol(Stream stream)
        {
            MemoryStream contentsStream = mData.GetStreamSafe("CONTENTS");
            if (contentsStream == null)
                return false;

            byte[] buffer = contentsStream.GetBuffer();
            Encoding encoding = Encoding.UTF8;

            // Skip the first 4 bytes of contents.
            const int offset = 4;
            string contents = encoding.GetString(buffer, offset, (int)contentsStream.Length - offset);

            Match match = gMolEndRegex.Match(contents);
            if (!match.Success)
                return false;

            int length = match.Index + match.Length;
            // Include \n
            if (length < contents.Length)
                length++;
            if ((contents[length - 1] == '\r') && (length < contents.Length) && (contents[length] == '\n'))
                length++;

            string molContents = contents.Substring(0, length);

            byte[] data = encoding.GetBytes(molContents);
            stream.Write(data, 0, data.Length);

            return true;
        }

        /// <summary>
        /// Returns true if progId is MS Excel OOXML document.
        /// </summary>
        private static bool IsExcel12(string progId)
        {
            return (progId.StartsWith("Excel.Sheet.12", StringComparison.Ordinal) ||
                    progId.StartsWith("Excel.SheetBinaryMacroEnabled.12", StringComparison.Ordinal) ||
                    progId.StartsWith("Excel.SheetMacroEnabled.12", StringComparison.Ordinal) ||
                    progId.StartsWith("Excel.Template", StringComparison.Ordinal) ||
                    progId.StartsWith("Excel.Template.MacroEnabled", StringComparison.Ordinal)

                    );
        }

        /// <summary>
        /// Save the OLE2 object as is without unwrapping.
        /// </summary>
        private static void SaveOle2NoUnwrap(Stream stream, MemoryStorage contentsStorage)
        {
            FileSystem fs = new FileSystem(contentsStorage);
            fs.Save(stream);
        }

        /// <summary>
        /// Gets the META stream that is often present in OLE10 objects.
        /// </summary>
        internal MemoryStream MetaStream
        {
            get { return mData.GetStreamSafe(Ole2StreamBase.MetaStreamName); }
        }

        /// <summary>
        /// Gets the PIC stream that is often present in OLE10 objects.
        /// </summary>
        internal MemoryStream PicStream
        {
            get { return mData.GetStreamSafe(Ole2StreamBase.PicStreamName); }
        }

        /// <summary>
        /// The object can be either in the OLE10 or OLE20 format.
        /// Returns true if this is OLE10 object (contains Ole10Native stream).
        /// When false, this is an OLE20 object.
        /// </summary>
        internal bool IsOle1
        {
            get { return Ole10NativeStream != null; }
        }

        /// <summary>
        /// Gets or sets the data of the OLE object in the Structured Storage format.
        /// </summary>
        internal MemoryStorage Data
        {
            get { return mData; }
            set
            {
                Debug.Assert(value != null);
                mData = value;
            }
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
        /// Gets the Ole10Native stream that is present in OLE10 objects.
        /// </summary>
        private MemoryStream Ole10NativeStream
        {
            get { return mData.GetStreamSafe(Ole2StreamBase.Ole10NativeStreamName); }
        }

        private MemoryStorage mData;

        private static readonly Regex gMolEndRegex =
            new Regex("^M  END$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private const string molProgId = "ISISServer";
    }
}
