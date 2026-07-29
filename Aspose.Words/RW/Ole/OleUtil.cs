// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/10/2011 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Collections;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Provides utility methods for manipulation with OLE objects.
    /// </summary>
    internal static class OleUtil
    {
        /// <summary>
        /// Wraps application data as "Ole1Native" stream within structured storage.
        /// </summary>
        internal static MemoryStorage WrapOle10NativeData(string progId, byte[] nativeData)
        {
            string clsId = OleRegistryInfo.GetClsId(progId);

            MemoryStorage oleData = AsOle1Native(new MemoryStream(nativeData), clsId);
            return oleData;
        }

        /// <summary>
        /// Wraps application data as "Package" stream within structured storage.
        /// </summary>
        internal static MemoryStorage WrapOoxmlPackage(string progId, OoxmlObject ooxml, bool isIcon)
        {
            return WrapPackage(progId, ooxml.Stream, "Package", isIcon);
        }

        /// <summary>
        /// Wraps application data as "EmbeddedOdf" stream within structured storage.
        /// </summary>
        internal static MemoryStorage WrapOdtPackage(string progId, Stream stream, bool isIcon)
        {
            stream.Position = 0;

            return WrapPackage(progId, stream, "EmbeddedOdf", isIcon);
        }

        /// <summary>
        /// Converts OOXML/ODT embedded object into OLE object which can be written to DOC/WML/RTF.
        /// </summary>
        /// <remarks>
        /// AM. Although this code looks like duplicate of CreateOleObject() but it has major difference in unknown progId processing.
        /// CreateOleObject() chooses Ole1Package method if progId is unknown while this method always uses Ole2NamedStream.
        /// </remarks>
        private static MemoryStorage WrapPackage(string progId, Stream stream, string streamName, bool isIcon)
        {
            string clsId = OleRegistryInfo.GetClsId(progId);

            MemoryStorage oleData = AsNamedStream(stream, streamName, clsId);
            AddObjInfoStream(oleData, isIcon, false);
            AddCompObjStream(oleData, clsId, progId, "");

            return oleData;
        }

        /// <summary>
        /// Reads [MS-OLEDS] 2.1.4 LengthPrefixedAnsiString.
        /// </summary>
        internal static string ReadLengthPrefixedAnsiString(BinaryReader reader)
        {
            return ReadLengthPrefixedAnsiString(reader, reader.ReadInt32());
        }

        /// <summary>
        /// Reads [MS-OLEDS] 2.1.4 LengthPrefixedAnsiString with length already read.
        /// </summary>
        internal static string ReadLengthPrefixedAnsiString(BinaryReader reader, int length)
        {
            string ret = "";

            if (length > 0)
            {
                byte[] bytes = reader.ReadBytes(length - 1);
                reader.ReadByte();
                ret = Encoding.GetEncoding(1251).GetString(bytes);
            }

            return ret;
        }

        /// <summary>
        /// Writes [MS-OLEDS] 2.1.4 LengthPrefixedAnsiString. Writes empty string as terminating zero byte.
        /// </summary>
        /// <remarks>
        /// AM. The reason that function exist that ClassName of Ole1Object can not be written as zero length string.
        /// Instead it should be written as 1-char string where char is terminating zero char.
        /// </remarks>
        internal static void WriteLengthPrefixedAnsiStringCore(BinaryWriter writer, string text)
        {
            text = (text == null) ? "" : text;

            writer.Write(text.Length + 1);
            writer.Write(Encoding.GetEncoding(1251).GetBytes(text));
            writer.Write((byte)0x00);
        }

        /// <summary>
        /// Writes [MS-OLEDS] 2.1.4 LengthPrefixedAnsiString. Writes empty string as zero length string.
        /// </summary>
        internal static void WriteLengthPrefixedAnsiString(BinaryWriter writer, string text)
        {
            if (StringUtil.HasChars(text))
            {
                WriteLengthPrefixedAnsiStringCore(writer, text);
            }
            else
                writer.Write(0x00);
        }

        /// <summary>
        /// When XLSX is embedded in a Word document, it has the Workbook Hidden flag set for some reason.
        /// When we are saving such XLSX to an external file we must unhide it, otherwise the users report it as invalid XLSX.
        /// </summary>
        internal static void UnhideOoxmlExcelWorkbook(Stream stream, Stream dstStream)
        {
            if (stream == null)
                return;

            stream.Position = 0;
            OpcPackage package = new OpcPackage(stream);

            // First try to get workbookPart by transitional DocxRelationshipType then by strict.
            OpcPackagePart workbookPart = package.GetPartByRelationshipType(null, DocxRelationshipTypes.GetType(DocxRelationshipType.OfficeDocument, false));

            if (workbookPart == null)
                // Try to get workbookPart by strict DocxRelationshipType.
                workbookPart = package.GetPartByRelationshipType(null, DocxRelationshipTypes.GetType(DocxRelationshipType.OfficeDocument, true));

            StreamReader reader = new StreamReader(workbookPart.Stream);
            string prevXml = reader.ReadToEnd();
            string xml = prevXml.Replace("visibility=\"hidden\"", "");

            // WORDSNET-17333 Skip package recreating, when content was not changed.
            if (prevXml.Length > xml.Length)
            {
                // It is better to create a new stream than dealing with truncating the old one.
                workbookPart.Stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(workbookPart.Stream, reader.CurrentEncoding);
                writer.Write(xml);
                writer.Flush();

                package.Save(dstStream);
            }
            else
            {
                stream.Position = 0;
                StreamUtil.CopyStream(stream, dstStream);
            }
        }

        /// <summary>
        /// When XLS is embedded in a Word document, it has the Workbook Hidden flag set for some reason.
        /// When we are saving such XLS to an external file we must unhide it, otherwise the users report it as invalid XLS.
        /// </summary>
        internal static void UnhideExcel97Workbook(Stream stream)
        {
            if (stream == null)
                return;

            stream.Position = 0;
            BinaryReader reader = new BinaryReader(stream);

            while (stream.Position < stream.Length)
            {
                int type = reader.ReadInt16();
                int size = reader.ReadInt16();
                if (type == 61) // Looking for Window1 record
                {
                    reader.ReadInt16();     // xWn
                    reader.ReadInt16();     // yWn
                    reader.ReadInt16();     // dxWn
                    reader.ReadInt16();     // dyWn
                    int flags = reader.ReadByte();
                    flags = BitUtil.SetBit(flags, 0x0001, false);
                    stream.Position -= 1;
                    stream.WriteByte((byte)flags);
                    break;
                }
                else
                {
                    stream.Position += size;
                }
            }
        }

        /// <summary>
        /// Returns default ICO file for given file type.
        /// </summary>
        internal static byte[] GetIcoFile(string icoFileName)
        {
            return ((icoFileName != null) && IcoFileLibrary.ContainsKey(icoFileName))
                ? IcoFileLibrary[icoFileName]
                : IcoFileLibrary[Packager];
        }

        /// <summary>
        /// Returns default icon caption for given file type.
        /// </summary>
        internal static string GetIconCaption(string icoFileName)
        {
            return ((icoFileName != null) && IcoFileNameAndCaptions.ContainsKey(icoFileName))
                ? IcoFileNameAndCaptions[icoFileName]
                : IcoFileNameAndCaptions[Packager];
        }

        /// <summary>
        /// Creates embedded OLE object from given stream.
        /// </summary>
        internal static OleObject CreateOleObject(Stream stream, string progId, bool asIcon)
        {
            return CreateEmbeddedOleObject(stream, progId, null, asIcon);
        }

        /// <summary>
        /// Creates either embedded or linked OLE object from given file.
        /// </summary>
        internal static OleObject CreateOleObject(string fileName, bool isLinked, bool asIcon, OleRegistryInfo oleInfo)
        {
            return isLinked
                ? CreateLinkedOleObject(fileName, asIcon, oleInfo)
                : CreateEmbeddedOleObject(fileName, asIcon, oleInfo);
        }

        private static OleObject CreateLinkedOleObject(string fileName, bool asIcon, OleRegistryInfo oleInfo)
        {
            if (oleInfo == null)
            {
                string fileExt = Path.GetExtension(fileName);
                oleInfo = OleRegistryInfo.GetByFileExt(fileExt);
            }

            // WORDSNET-15431 If the OLE info is Package, embed a package that contains a link to the specified file.
            if (string.Equals(oleInfo.ClsId, OleRegistryInfo.PackageClsId, StringComparison.OrdinalIgnoreCase))
            {
                MemoryStorage oleData = AsOle1Package(null, fileName);
                AddDefaultStreams(oleData, oleInfo.ClsId, oleInfo.ProgId, oleInfo.UserType, asIcon);
                return new OleObject(oleData);
            }
            else
            {
                return CreateAsLink(fileName, asIcon, oleInfo);
            }
        }

        /// <summary>
        /// Creates Ole object as link to specified filename.
        /// </summary>
        private static OleObject CreateAsLink(string fileName, bool asIcon, OleRegistryInfo oleInfo)
        {
             MemoryStorage oleData = new MemoryStorage(ClsidStdLink);

            AddObjInfoStream(oleData, asIcon, true);

            LinkInfoStream linkInfoStream = new LinkInfoStream("", fileName);
            linkInfoStream.Write(oleData);

            OleStream oleStream = new OleStream(OleObjectType.Linked);
            oleStream.Path = fileName;
            oleStream.ClsId = new Guid(oleInfo.ClsId);

            if (gTestMode)
            {
                oleStream.LocalCheckUpdateTime = gTestDateTime;
                oleStream.LocalUpdateTime = gTestDateTime;
                oleStream.RemoteUpdateTime = gTestDateTime;
            }
            else
            {
                oleStream.LocalCheckUpdateTime = DateTime.Now;
                oleStream.LocalUpdateTime = DateTime.Now;
                oleStream.RemoteUpdateTime = DateTime.Now;
            }

            oleStream.Write(oleData);

            return new OleObject(oleData);
        }

        private static OleObject CreateEmbeddedOleObject(string fileName, bool asIcon, OleRegistryInfo oleInfo)
        {
            if (oleInfo == null)
            {
                string fileExt = Path.GetExtension(fileName);
                oleInfo = OleRegistryInfo.GetByFileExt(fileExt);
            }

            using (FileStream fileStream = File.Open(fileName, FileMode.Open))
            {
                return CreateEmbeddedOleObject(fileStream, oleInfo.ProgId, fileName, asIcon);
            }
        }

        private static OleObject CreateEmbeddedOleObject(Stream stream, string progId, string fileName, bool asIcon)
        {
            OleRegistryInfo oleInfo = OleRegistryInfo.GetByProgId(progId);

            MemoryStorage oleData;
            switch (oleInfo.EmbeddingType)
            {
                case OleEmbeddingType.Ole1NativeStream:
                    oleData = AsOle1Native(stream, oleInfo.ClsId);
                    break;
                case OleEmbeddingType.Ole2StreamContents:
                    oleData = AsNamedStream(stream, "CONTENTS", oleInfo.ClsId);
                    break;
                case OleEmbeddingType.Ole2StreamPackage:
                    oleData = AsNamedStream(stream, "Package", oleInfo.ClsId);
                    break;
                case OleEmbeddingType.Ole2StreamEmbeddedOdf:
                    oleData = AsNamedStream(stream, "EmbeddedOdf", oleInfo.ClsId);
                    break;
                case OleEmbeddingType.Ole2StreamEquationNative:
                    oleData = AsNamedStream(stream, "Equation Native", oleInfo.ClsId);
                    break;
                case OleEmbeddingType.Ole2Storage:
                    oleData = AsOle2Storage(stream);
                    break;
                case OleEmbeddingType.Ole1Package:
                    oleData = AsOle1Package(stream, fileName);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected OleEmbeddingType.");
            }

            AddDefaultStreams(oleData, oleInfo.ClsId, progId, oleInfo.UserType, asIcon);

            return new OleObject(oleData);
        }

        /// <summary>
        /// Adds default streams into the specified OLE data object.
        /// </summary>
        private static void AddDefaultStreams(MemoryStorage oleData, string clsId, string progId,
            string userType, bool asIcon)
        {
            if (oleData.Clsid.Equals(Guid.Empty))
                oleData.Clsid = new Guid(clsId);

            AddObjInfoStream(oleData, asIcon, false);

            if (!oleData.ContainsKey(Ole2StreamBase.CompObjStreamName))
                AddCompObjStream(oleData, clsId, progId, userType);
        }

        internal static string GetOriginalFileNameFromOle1(BinaryReader reader)
        {
            if (reader != null)
            {
                OlePackage package = new OlePackage();

                // Skip dataLength.
                reader.ReadInt32();
                package.Read(reader);

                return package.DisplayName;
            }
            return "";
        }

        /// <summary>
        /// Converts a string to a <see cref="NullableBool"/>.
        /// </summary>
        internal static NullableBool ToNullableBool(string value)
        {
            switch (value)
            {
                case "0":
                    return NullableBool.False;
                case "1":
                    return NullableBool.True;
                default:
                    return NullableBool.NotDefined;
            }
        }

        /// <summary>
        /// Converts a <see cref="NullableBool"/> to a string.
        /// </summary>
        internal static string ToString(NullableBool value)
        {
            switch (value)
            {
                case NullableBool.False:
                    return "0";
                case NullableBool.True:
                    return "1";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Converts a type of the Forms 2.0 control to a string.
        /// </summary>
        internal static string ToString(Forms2OleControlType type)
        {
            switch (type)
            {
                case Forms2OleControlType.OptionButton:
                    return "OptionButton";

                case Forms2OleControlType.Label:
                    return "Label";

                case Forms2OleControlType.Textbox:
                    return "TextBox";

                case Forms2OleControlType.CheckBox:
                    return "CheckBox";

                case Forms2OleControlType.ToggleButton:
                    return "ToggleButton";

                case Forms2OleControlType.SpinButton:
                    return "SpinButton";

                case Forms2OleControlType.ComboBox:
                    return "ComboBox";

                case Forms2OleControlType.Frame:
                    return "Frame";

                case Forms2OleControlType.MultiPage:
                    return "MultiPage";

                case Forms2OleControlType.CommandButton:
                    return "CommandButton";

                case Forms2OleControlType.Image:
                    return "Image";

                case Forms2OleControlType.ScrollBar:
                    return "ScrollBar";

                case Forms2OleControlType.TabStrip:
                    return "TabStrip";

                case Forms2OleControlType.ListBox:
                    return "ListBox";

                case Forms2OleControlType.Form:
                    return "Form";

                default:
                    throw new InvalidOperationException(string.Format("Unexpected Forms2 OLE control type: {0}", type));
            }
        }

        /// <summary>
        /// Creates embedded OLE object with application data stored in Ole1Native stream.
        /// </summary>
        private static MemoryStorage AsOle1Native(Stream stream, string clsId)
        {
            MemoryStorage oleData = new MemoryStorage(new Guid(clsId));

            OleStream oleStream = new OleStream(OleObjectType.Embedded);
            oleStream.Write(oleData);

            Ole10NativeStream ole10NativeStream = new Ole10NativeStream();
            int length = (int) stream.Length;
            byte[] nativeData = new byte[length];
            StreamUtil.Read(stream, nativeData, 0, length);
            ole10NativeStream.NativeData = nativeData;
            ole10NativeStream.Write(oleData);

            return oleData;
        }

        /// <summary>
        /// Creates embedded OLE object with application data stored in stream with given name.
        /// </summary>
        /// <remarks>
        /// Position of the passed stream is set to 0 before copying to the memory storage.
        /// </remarks>
        private static MemoryStorage AsNamedStream(Stream stream, string streamName, string clsId)
        {
            MemoryStorage oleData = new MemoryStorage(new Guid(clsId));

            OleStream oleStream = new OleStream(OleObjectType.Embedded);
            oleStream.Write(oleData);

            MemoryStream dstStream = new MemoryStream();
            stream.Position = 0;
            StreamUtil.CopyStream(stream, dstStream);
            oleData.Add(streamName, dstStream);

            return oleData;
        }

        /// <summary>
        /// Creates embedded OLE object with application data stored as structured storage.
        /// </summary>
        private static MemoryStorage AsOle2Storage(Stream stream)
        {
            FileSystem fs = new FileSystem(stream);
            MemoryStorage oleData = fs.Root;

            return oleData;
        }

        /// <summary>
        /// Creates embedded OLE object where application data stored within OLE Package object.
        /// </summary>
        private static MemoryStorage AsOle1Package(Stream stream, string fileName)
        {
            Guid clsId = new Guid(OleRegistryInfo.GetOlePackage().ClsId);

            MemoryStorage oleData = new MemoryStorage(clsId);

            // We can't use empty name here, Word is failed to open such embedding.
            fileName = StringUtil.HasChars(fileName) ? fileName : "Unknown";

            OlePackage package = new OlePackage();
            package.DisplayName = Path.GetFileName(fileName);
            package.FileName = fileName;
            package.TempFileName = fileName;
            if (stream != null)
                package.Data = StreamUtil.CopyStreamToByteArray(stream);

            // Make OLE package object.
            MemoryStream packagerStream = new MemoryStream();
            BinaryWriter packagerWriter = new BinaryWriter(packagerStream);
            package.Write(packagerWriter);

            // And write it as Ole10NativeStream.
            Ole10NativeStream ole10NativeStream = new Ole10NativeStream();
            ole10NativeStream.NativeData = packagerStream.ToArray();
            ole10NativeStream.Write(oleData);

            return oleData;
        }

        /// <summary>
        /// Adds ObjInfo stream to OLE data storage.
        /// </summary>
        private static void AddObjInfoStream(MemoryStorage oleData, bool isIcon, bool isLink)
        {
            ObjInfoStream objInfoStream = ObjInfoStream.DefaultObject(isIcon, isLink);
            objInfoStream.Write(oleData);
        }

        /// <summary>
        /// Adds CompObj stream to OLE data storage.
        /// </summary>
        private static void AddCompObjStream(MemoryStorage oleData, string clsId, string progId, string userType)
        {
            CompObjStream compObjStream = new CompObjStream();
            compObjStream.ProgId = progId;
            compObjStream.Clsid = new Guid(clsId);
            compObjStream.UserType = userType;
            compObjStream.Write(oleData);
        }

        /// <summary>
        /// Predefined library of various images.
        /// </summary>
        internal static StringToObjDictionary<byte[]> ImageLibrary
        {
            get
            {
                if (gImageLibraryCache == null)
                {
                    lock (gImageLibrarySyncRoot)
                    {
                        if (gImageLibraryCache == null)
                        {
                            gImageLibraryCache = new StringToObjDictionary<byte[]>();

                            // If no image found.
                            AddImage(Packager, "Aspose.Words.Resources.OleIcons.packager.emf");

                            // Image for Normal view.
                            AddImage(Normal, "Aspose.Words.Resources.OleIcons.normal.png");

                            // Icons for HTML OLE radio button control.
                            AddImage("RadioChecked", "Aspose.Words.Resources.OleIcons.RadioChecked.wmf");
                            AddImage("RadioUnchecked", "Aspose.Words.Resources.OleIcons.RadioUnchecked.wmf");
                        }
                    }
                }

                return gImageLibraryCache;
            }
        }

        /// <summary>
        /// Predefined library of ICO files for various file types.
        /// </summary>
        internal static StringToObjDictionary<byte[]> IcoFileLibrary
        {
            get
            {
                if (gIcoFileLibraryCache == null)
                {
                    lock (gIcoFileLibrarySyncRoot)
                    {
                        if (gIcoFileLibraryCache == null)
                        {
                            gIcoFileLibraryCache = new StringToObjDictionary<byte[]>();
                            foreach (string icoFile in IcoFileNameAndCaptions.Keys)
                                AddIcoFile(icoFile, string.Format("Aspose.Words.Resources.OleIcons.{0}.ico", icoFile));
                        }
                    }
                }
                return gIcoFileLibraryCache;
            }
        }

        /// <summary>
        /// Sets Test flag. This flag is used to make behavior of some methods deterministic for test purposes.
        /// Call this method before running any tests which can be connected to methods of this class.
        /// </summary>
        public static void SetTestMode()
        {
            gTestMode = true;
        }

        private static void AddImage(string name, string resource)
        {
            gImageLibraryCache.Add(name, StreamUtil.CopyStreamToByteArray(ResourceUtil.FetchResourceStream(resource)));
        }

        private static void AddIcoFile(string name, string resource)
        {
            gIcoFileLibraryCache.Add(name,
                StreamUtil.CopyStreamToByteArray(ResourceUtil.FetchResourceStream(resource)));
        }

        /// <summary>
        /// Gets the content type by progId valid for saving into an OOXML document.
        /// </summary>
        internal static string GetContentType(string progId)
        {
            // RK Word 2007 seems to write specific content types for OLE objects created using Office applications.
            // Without writing a specific content type it will not be possible to edit the object in Word 2007.
            if (progId.StartsWith("Excel.Sheet.8", StringComparison.Ordinal))
                return DocxContentType.Xls;
            else if (progId.StartsWith("Word.Document.8", StringComparison.Ordinal))
                return DocxContentType.Doc;
            else if (progId.StartsWith("PowerPoint.Show.8", StringComparison.Ordinal))
                return DocxContentType.Ppt;
            else if (progId.StartsWith("Visio.Drawing.11", StringComparison.Ordinal))
                return DocxContentType.Vsd;
            else
                return DocxContentType.OleObject;
        }

        /// <summary>
        /// Gets the embedded object id, formatted the "Microsoft Word way", that is "_xyz".
        /// </summary>
        /// <remarks>
        /// Note that in Word it could be with leading zeroes "_0000000xyz" sometimes.
        /// But we DO NOT prefix with zeroes, otherwise OLE10 objects will not work in MS Word!
        /// </remarks>
        internal static string GetMsWordId(IEmbeddedObject embeddedObject)
        {
            return string.Format("_{0}", embeddedObject.Id);
        }

        /// <summary>
        /// This static member is used to use gTestDateTime instead of DateTime.Now in test mode.
        ///
        /// All tests which can be connected to methods of this class must call TestUtil.SetTestMode/>
        /// to set this flag to <c>true</c>.
        /// </summary>
        private static bool gTestMode = false;

        /// <summary>
        /// Commonly used OLE1.0 OleVersion constant.
        /// </summary>
        /// AM. Although spec says that this value is arbitrary Word always writes this one.
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int OleVersion = 0x0501;
        internal const string Normal = "normal";
        internal const string Packager = "packager";

        /// <summary>
        /// Dictionary of pairs of ICO file name and default icon caption.
        /// </summary>
        internal static readonly Dictionary<string, string> IcoFileNameAndCaptions = new Dictionary<string, string> {
            { "bmp", "Bitmap Image" },
            { "doc", "MS Word 97-2003 Document" },
            { "docm", "MS Word Macro- Enabled Document" },
            { "docx", "Microsoft Word Document" },
            { "dot", "MS Word 97-2003 Template" },
            { "dotm", "MS Word Macro- Enabled Template" },
            { "dotx", "Microsoft Word Template" },
            { "html", "HTML Document" },
            { "mpp", "MS Project Document" },
            { "odp", "OpenDocument Presentation" },
            { "ods", "OpenDocument Spreadsheet" },
            { "odt", "OpenDocument Text" },
            { Packager, "Packager" },
            { "pdf", "Adobe Acrobat Document" },
            { "pot", "MS PowerPoint 97-2003 Template" },
            { "potm", "MS PowerPoint Mcr-Enbld Template" },
            { "potx", "MS PowerPoint Template" },
            { "pps", "MS PowerPoint 97-2003 Show" },
            { "ppsm", "MS PowerPoint Mcr-Enbld Show" },
            { "ppsx", "MS PowerPoint Show" },
            { "ppt", "MS PowerPoint 97-2003 Presentation" },
            { "pptm", "MS PowerPoint Mcr-Enbld Presentation" },
            { "pptx", "MS PowerPoint Presentation" },
            { "sldx", "Microsoft PowerPoint Slide" },
            { "vsd", "Microsoft Visio Document" },
            { "xls", "MS Excel 97-2003 Worksheet" },
            { "xlsb", "MS Excel Binary Worksheet" },
            { "xlsm", "MS Excel Macro- Enabled Worksheet" },
            { "xlsx", "Microsoft Excel Worksheet" },
            { "xlt", "MS Excel 97-2003 Template" },
            { "xltm", "MS Excel Macro- Enabled Template" },
            { "xltx", "Microsoft Excel Template" }
        };

        /// <summary>
        /// A Clsid of StdOleLink as specified in [MS-OLEDS], 2.3.3 OLEStream.
        /// </summary>
        internal static readonly Guid ClsidStdLink = new Guid("00000300-0000-0000-c000-000000000046");

        private static readonly DateTime gTestDateTime = new DateTime(2013, 5, 3, 12, 23, 0);

        private static StringToObjDictionary<byte[]> gImageLibraryCache;
        private static readonly object gImageLibrarySyncRoot = new object();
        private static StringToObjDictionary<byte[]> gIcoFileLibraryCache;
        private static readonly object gIcoFileLibrarySyncRoot = new object();
    }
}
