// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2011 by Alexey Morozov

using System;
using System.IO;
using Aspose.Common;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Implements reading OLE specific elements in WordML and DOCX files.
    /// </summary>
    internal static class VmlOleReader
    {
        /// <summary>
        /// Reads DOCX 'control' element which specifies activeX object.
        /// </summary>
        internal static Shape ReadDocxControl(IVmlShapeReaderContext context)
        {
            string name = "";
            FileSystem fs = null;
            string shapeId = "";
            string id = "";

            NrxXmlReader reader = context.XmlReader;

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "id":
                        id = value;
                        break;
                    case "shapeid":
                        shapeId = value;
                        break;
                    case "name":
                        name = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(reader.LocalName);
                }
            }

            Shape shape = (Shape)context.GetShapeById(shapeId);

            // WORDSNET-11823 If there is no shape with the specified in OLE control 'id', MS Word creates new one from the scratch.
            if (shape == null)
            {
                shape = new Shape(context.Document);
                shape.WrapType = WrapType.Inline;
            }

            shape.SetShapeType(ShapeType.OleControl);

            DocxDocumentReaderBase docxReader = (DocxDocumentReaderBase)context;

            // Read XML wrapper for activeX.
            OpcPackagePart xmlPart = docxReader.FetchPartByName(context.GetRelationshipTarget(id));

            NrxXmlReader xmlPartReader = new NrxXmlReader(xmlPart.Stream);
            string persistence = xmlPartReader.ReadAttribute("persistence", null);
            string clsid = xmlPartReader.ReadAttribute("classid", null);

            // ActiveX control can be saved in several ways specified by persistence option:
            //
            // 1) persistPropertyBag - control is serialized as XML data. In such case we cannot save this control into different format such as DOC or WML because there is no
            //    way to convert XML serialization to binary serialization without control interaction i.e only control itself knows data being serialized.
            // 2) persistStorage - control is serialized as structured storage. This is the way control is stored in DOC and WML files. Control with such persistence option can be
            //    easily saved into DOC or WML.
            // 3) persistStream - "lightweight" version of persistStorage. Only serialized data is saved without structured storage. Can be easily converted to persistStorage.
            // 4) persistStreamInit - never seen.
            switch(persistence)
            {
                case "persistPropertyBag":
                {
                    // Save whole XML serialization data as OoxmlObject.
                    shape.OleFormat.EmbeddedObject = new OoxmlObject(xmlPart.Stream, DocxContentType.Control, name, new Guid(clsid));
                    break;
                }
                case "persistStorage":
                case "persistStream":
                {
                    // Read control itself. target is always rId1.
                    OpcPackagePart binPart = docxReader.FetchPartByName(xmlPart.GetRelationshipTarget("rId1"));

                    if (persistence == "persistStorage")
                    {
                        // Data is structured storage and ready for Model.
                        fs = new FileSystem(binPart.GetAsMemoryStream());
                    }
                    else
                    {
                        // convert persistStream into persistStorage: make structured storage, set read ClsId and add serialized binary data as stream with certain name.
                        fs = new FileSystem(new Guid(clsid));
                        fs.Root.Add(Ole2StreamBase.OcxDataStreamName, binPart.GetAsMemoryStream());
                    }

                    // OleObject doesn't have name in Model and it is specified in OCXNAME stream.
                    // In DOCX this stream might be skipped so we should add it to get uniform behavior with DOC format.
                    OcxNameStream ocxNameStream = new OcxNameStream(name);
                    ocxNameStream.Write(fs.Root);

                    ObjInfoStream objInfoStream = ObjInfoStream.DefaultControl();
                    objInfoStream.Write(fs.Root);

                    shape.OleFormat.EmbeddedObject = new OleObject(shape.Id, fs.Root);

                    break;
                }
                case "persistStreamInit":
                reader.IgnoreElement(WarningType.DataLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                    break;
                default:
                    throw new InvalidOperationException("Invalid persistence value.");
            }

            return shape;
        }

        /// <summary>
        /// This is WordML only at the moment.
        ///
        /// Reads OCX control. Reader should be positioned to 'w:ocx' element start.
        /// </summary>
        internal static Shape ReadWmlControl(IVmlShapeReaderContext context)
        {
            string data = "";
            string name = "";
            string shapeId = "";
            string w = "";
            string h = "";
            string classid = "";

            NrxXmlReader reader = context.XmlReader;
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "data":
                        data = value;
                        break;
                    case "id":
                        // That is a duplication of "name".
                        break;
                    case "name":
                        name = value;
                        break;
                    case "classid":
                        classid = value;
                        break;
                    case "shapeid":
                        shapeId = value;
                        break;
                    case "class":
                        // Always "shape" as far as I have seen. Ignore.
                        break;
                    case "w":
                        w = value;
                        break;
                    case "h":
                        h = value;
                        break;
                    case "align":
                        // Not in the model. Ignore.
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                    case "iPersistPropertyBag": // wx:iPersistPropertyBag
                        // Not in the model. Ignore.
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }

            Shape shape = (Shape)context.GetShapeById(shapeId);
            if (shape == null)
            {
                shape = new Shape(context.Document);
                shape.WrapType = WrapType.Inline;
            }

            // The shape for this OCX might been read already as Image type.
            // But as we read OCX control for that shape now, we change its type to OleControl.
            shape.SetShapeType(ShapeType.OleControl);

            if (StringUtil.HasChars(w))
                shape.SetWidthSafe(ConvertUtil.PixelToPoint(FormatterPal.ParseDouble(w)));

            if (StringUtil.HasChars(h))
                shape.SetHeightSafe(ConvertUtil.PixelToPoint(FormatterPal.ParseDouble(h)));

            // RK In most cases "data" is an id, but in TestTobyHenderson6 ms.wml it is actually an OCXDATA stream.
            const string InlineDataHeader = "DATA:application/x-oleobject;BASE64,";
            FileSystem fs = null;
            if (data.StartsWith(InlineDataHeader, StringComparison.Ordinal))
            {
                data = data.Substring(InlineDataHeader.Length);
                byte[] ocxData = Convert.FromBase64String(data);

                const string ClsidPrefix = "CLSID:";
                if (classid.StartsWith(ClsidPrefix, StringComparison.Ordinal))
                    classid = classid.Substring(ClsidPrefix.Length);

                Guid guid = StringUtil.HasChars(classid) ? new Guid(classid) : Guid.Empty;
                fs = new FileSystem(new MemoryStorage(guid));
                fs.Root.Add(Ole2StreamBase.OcxDataStreamName, new MemoryStream(ocxData));
            }
            else
            {
                // This is the most common case.
                byte[] binData = context.GetBinData(data);
                if (binData != null)
                    fs = new FileSystem(new MemoryStream(binData));
            }

            if (fs != null)
            {
                // Add some "standard" streams to the structured storage.
                OcxNameStream ocxNameStream = new OcxNameStream(name);
                ocxNameStream.Write(fs.Root);

                ObjInfoStream objInfoStream = ObjInfoStream.DefaultControl();
                objInfoStream.Write(fs.Root);

                shape.OleFormat.EmbeddedObject = new OleObject(shape.Id, fs.Root);
            }

            return shape;
        }

        /// <summary>
        /// Reads shape's OLE data for the specified shape. Reader should be positioned to 'o:OLEObject' element start.
        /// </summary>
        internal static void ReadOleObject(IVmlShapeReaderContext context)
        {
            //    <o:OLEObject Type="Embed" ProgID="Equation.3" ShapeID="_x0000_i1025" DrawAspect="Content" ObjectID="_1254857050" r:id="rId5" />
            bool isLink = false;
            string progId = null;
            string shapeId = null;
            string objectId = null;
            string moniker = "";  // WordML thing.
            bool isAutoUpdate = false;
            bool isOleIcon = false;
            string relId = null;    // DOCX thing.

            NrxXmlReader reader = context.XmlReader;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "Type":
                        // Comparing without case just adds resiliency like for the DrawAspect fix.
                        isLink = (StringUtil.EqualsIgnoreCase(reader.Value, "Link"));
                        break;
                    case "ProgID":
                        progId = reader.Value;
                        break;
                    case "ShapeID":
                        shapeId = reader.Value;
                        break;
                    case "DrawAspect":
                        // WORDSNET-7946 DrawAspect is changed from Icon to Content. In fact the value is "icon" in the file,
                        // this is incorrect according to the OOXML schema, but let's read it as MS Word does.
                        isOleIcon = (StringUtil.EqualsIgnoreCase(reader.Value, "Icon"));
                        break;
                    case "ObjectID":
                        objectId = reader.Value;
                        break;
                    case "Moniker":
                        // WordML thing.
                        moniker = reader.Value;
                        break;
                    case "UpdateMode":
                        // Comparing without case just adds resiliency like for the DrawAspect fix.
                        isAutoUpdate = (StringUtil.EqualsIgnoreCase(reader.Value, "Always"));
                        break;
                    case "id":
                        // DOCX thing.
                        relId = reader.Value;
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }

            Shape shape = (Shape)context.GetShapeById(shapeId);
            if (shape == null)
                return;

            shape.SetShapeType(ShapeType.OleObject);

            OleFormat oleFormat = shape.OleFormat;
            oleFormat.AutoUpdate = isAutoUpdate;
            oleFormat.SetOleIcon(isOleIcon);
            if (progId != null)
                oleFormat.ProgId = progId;

            if (isLink)
            {
                // DOCX specific.
                if (StringUtil.HasChars(relId))
                    moniker = context.GetRelationshipTarget(relId);

                if (StringUtil.HasChars(moniker))
                {
                    string[] monikerParts = moniker.Split(new char[] { '!' }, 2);
                    oleFormat.SourceFullName = monikerParts[0];

                    if (monikerParts.Length > 1)
                        oleFormat.SourceItem = monikerParts[1];
                }
            }
            else
            {
                // If relationship id is specified, this must be a DOCX file and we should use the relationship id.
                // Otherwise it is a WordML file and we need to use the object id to get the data.
                NrxDocumentReaderBase ooxmlContext = context as NrxDocumentReaderBase;
                string effectiveId = StringUtil.HasChars(relId) ? relId : objectId;

                // WORDSNET-24477 Ignore embedded OLE objects according to settings.
                if ((ooxmlContext == null) || !ooxmlContext.LoadOptions.IgnoreOleData)
                {
                    IEmbeddedObject embeddedObject = context.GetEmbeddedObject(effectiveId);

                    if (embeddedObject != null)
                    {
                        int objectIdInt = NrxXmlUtil.TryParseId(objectId);
                        if (objectIdInt != int.MinValue)
                            embeddedObject.Id = objectIdInt;

                        oleFormat.EmbeddedObject = embeddedObject;
                    }
                }
            }

            while (reader.ReadChild("OLEObject")) // o:OLEObject
            {
                switch (reader.LocalName)
                {
                    case "LinkType":
                        oleFormat.OleLinkType = VmlEnum.VmlToOleLinkType(reader.ReadString());
                        break;
                    case "LockedField":
                        // RK When o:LockedField is present, but empty it means true.
                        oleFormat.IsLocked = reader.ReadBoolString(true);
                        break;
                    case "WordFieldCodes":
                    case "FieldCodes":
                        {
                            string value = reader.ReadString();
                            if (value.Length > 0)
                            {
                                char linkTypeDigitChar = value[value.Length - 1];
                                if (linkTypeDigitChar >= '0' && linkTypeDigitChar <= '5')
                                    oleFormat.FormatUpdateType = linkTypeDigitChar - '0';
                            }
                            else
                            {
                                oleFormat.FormatUpdateType = 0;
                            }
                            break;
                        }
                    default:
                        reader.IgnoreElement(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private const string WarningMessageFormat = "Import of element '{0}' is not supported upon reading OLE specific elements by Aspose.Words.";
    }
}
