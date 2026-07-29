// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2015 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Ss;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.Drawing.Ole.Core
{
    /// <summary>
    /// Implements HTML OLE control.
    /// </summary>
    internal class HtmlOleControl : OleControl
    {
        /// <summary>
        /// Creates instance of the <see cref="HtmlOleControl"/> class with specified name, type and HTML string.
        /// </summary>
        internal HtmlOleControl(string name, HtmlOleControlType type, string html)
            : base(name)
        {
            Type = type;
            Html = html;
        }

        /// <summary>
        /// Creates instance of the <see cref="HtmlOleControl"/> class with a specified name.
        /// </summary>
        internal HtmlOleControl(string name) : base(name)
        {
        }

        /// <summary>
        /// Reads the control from a storage.
        /// </summary>
        internal override void Read(MemoryStorage storage)
        {
            MemoryStream dataStream = storage.FetchStream(Ole2StreamBase.OcxDataStreamName);
            BinaryReader reader = new BinaryReader(dataStream);

            Guid clsid = new Guid(reader.ReadBytes(16));
            Type = ClsidToType(clsid.ToString());

            byte[] htmlBytes = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            Html = Encoding.Unicode.GetString(htmlBytes);
        }

        /// <summary>
        /// Writes the control to a binary writer.
        /// </summary>
        internal override uint Write(BinaryWriter writer)
        {
            long originalPosition = writer.BaseStream.Position;

            Guid clsid = new Guid(ClsidVirtual);
            writer.Write(clsid.ToByteArray());
            writer.Write(Encoding.Unicode.GetBytes(Html));

            long writtenSize = writer.BaseStream.Position - originalPosition;
            return (uint)(writtenSize);
        }

        /// <summary>
        /// Writes the control to a storage.
        /// </summary>
        internal override void Write(MemoryStorage storage)
        {
            MemoryStream stream = new MemoryStream();
            Write(new BinaryWriter(stream));

            OcxDataStream ocxData = new OcxDataStream();
            ocxData.Data = stream.ToArray();
            ocxData.Write(storage);

            // As we write HtmlOleControl to the OcxData stream, we must update ObjInfo stream flags accordingly.
            // See OdtPersist1 for details.
            ObjInfoStream objInfoStream = ObjInfoStream.Read(storage);
            objInfoStream.Flags1 |= OdtPersist1.Stream;
            objInfoStream.Write(storage);
        }

        /// <summary>
        /// Gets a Clsid of the control.
        /// </summary>
        protected override string ClsidVirtual
        {
            get
            {
                switch (Type)
                {
                    case HtmlOleControlType.Select:
                        return HtmlSelectClsid;
                    case HtmlOleControlType.Option:
                        return HtmlOptionClsid;
                    case HtmlOleControlType.SubmitButton:
                        return HtmlSubmitButtonClsid;
                    case HtmlOleControlType.Hidden:
                        return HtmlHiddenClsid;
                    case HtmlOleControlType.Text:
                        return HtmlTextClsid;
                    default:
                        return Guid.Empty.ToString();
                }
            }
        }

        /// <summary>
        /// Converts a Clsid of the control to a type.
        /// </summary>
        private static HtmlOleControlType ClsidToType(string clsid)
        {
            switch (clsid)
            {
                case HtmlSelectClsid: return HtmlOleControlType.Select;
                case HtmlOptionClsid: return HtmlOleControlType.Option;
                case HtmlSubmitButtonClsid: return HtmlOleControlType.SubmitButton;
                case HtmlHiddenClsid: return HtmlOleControlType.Hidden;
                case HtmlTextClsid: return HtmlOleControlType.Text;
                default:
                    return HtmlOleControlType.Unknown;
            }
        }

        /// <summary>
        /// Gets a type of the control.
        /// </summary>
        internal HtmlOleControlType Type { get; private set; }

        /// <summary>
        /// Gets HTML string of the control.
        /// </summary>
        internal string Html { get; private set; }
    }
}
