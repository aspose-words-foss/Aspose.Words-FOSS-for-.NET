// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Morozov

using System.IO;
using Aspose.Ss;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Implements [MS-OLEDS] 2.2.5 EmbeddedObject.
    /// </summary>
    internal class Ole1EmbeddedObject : Ole1Object
    {
        internal Ole1EmbeddedObject(ObjectHeader objectHeader, BinaryReader reader)
        {
            mClassName = objectHeader.ClassName;

            uint nativeDataSize = reader.ReadUInt32();
            // WORDSNET-18063 Resilience against invalid size.
            nativeDataSize = System.Math.Min(nativeDataSize, (uint)(reader.BaseStream.Length - reader.BaseStream.Position));

            mNativeData = reader.ReadBytes((int)nativeDataSize);

            mPresentationObject = PresentationObject.Read(reader);
        }

        internal Ole1EmbeddedObject(string progId, MemoryStorage embeddedData, byte[] metafileData)
        {
            mClassName = progId;
            
            FileSystem fs = new FileSystem(embeddedData);
            MemoryStream ms = new MemoryStream();
            fs.Save(ms);
            mNativeData = ms.ToArray();

            // So far we support just Metafile Presentation object. Later we can add more objects if needed.
            mPresentationObject = (metafileData != null) ?
                (PresentationObject)new PresentationObjectMetafile(metafileData) :
                new PresentationObjectEmpty();
        }

        internal override void Write(BinaryWriter writer)
        {
            ObjectHeader objectHeader = new ObjectHeader(OleObjectType.Embedded, mClassName, "", "");
            objectHeader.Write(writer);

            writer.Write(mNativeData.Length);
            writer.Write(mNativeData);

            mPresentationObject.Write(writer);
        }

        internal override OleObjectType ObjectType
        {
            get { return OleObjectType.Embedded; }
        }

        /// <summary>
        /// Specifies OLE1.0 Application specific native data.
        /// </summary>
        internal byte[] NativeData
        {
            get { return mNativeData; }
        }

        /// <summary>
        /// Specifies OLE1.0 presentation data to display this object in container applications.
        /// </summary>
        internal PresentationObject PresentationObject
        {
            get { return mPresentationObject; }
        }

        private readonly byte[] mNativeData;
        private readonly PresentationObject mPresentationObject;
    }
}
