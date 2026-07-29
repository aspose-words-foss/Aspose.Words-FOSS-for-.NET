// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/10/2018 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements writing of a Forms 2.0 control properties.
    /// </summary>
    internal class Forms2PrWriter
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal Forms2PrWriter(BinaryWriter writer, Forms2Pr pr)
        {
            mStart = (int)writer.BaseStream.Position;
            mWriter = writer;
            mPr = pr;
        }

        /// <summary>
        /// Writes padding.
        /// </summary>
        internal void WritePadding(int size)
        {
            if (size == 1)
                return;

            // Padding size cannot be greater 4 by spec.
            int padSize = System.Math.Min(4, size);

            int pos = (int)mWriter.BaseStream.Position - mStart;

            int div = pos % padSize;
            if (div == 0)
                return;

            mWriter.Write(gPadding, 0, padSize - div);
        }

        /// <summary>
        /// Writes unsigned integer with padding.
        /// </summary>
        internal void Write(uint value)
        {
            byte[] raw = BitConverter.GetBytes(value);

            WritePadding(4);
            mWriter.Write(raw, 0, 4);
        }

        /// <summary>
        /// Writes property specified by a key.
        /// </summary>
        internal bool Write(int key, int valueSize)
        {
            byte[] raw = GetRawValue(key, valueSize);
            if (raw == null)
                return false;

            WriteBytes(raw, valueSize);

            return true;
        }

        /// <summary>
        /// Writes byte array with a specified padding size.
        /// </summary>
        internal bool WriteBytes(byte[] raw, int paddingSize)
        {
            if (!ArrayUtil.HasData(raw))
                return false;

            WritePadding(paddingSize);
            mWriter.Write(raw);

            return true;
        }

        /// <summary>
        /// Writes array of byte array.
        /// </summary>
        internal bool WriteBytes(byte[][] raw)
        {
            if ((raw == null) || (raw.Length == 0))
                return false;

            foreach (byte[] bytes in raw)
                WriteBytes(bytes);

            return true;
        }

        /// <summary>
        /// Returns array of byte arrays representing input list of strings
        /// as specified in [MS-OFORMS], 2.4.14.1 ArrayString.
        /// </summary>
        internal static byte[][] GetBytes(IList<string> list, out uint size)
        {
            byte[][] bytes = new byte[list.Count][];

            size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                bytes[i] = GetBytes(list[i]);
                size += (uint)bytes[i].Length;
            }

            return bytes;
        }

        /// <summary>
        /// Writes count of bytes with compression flag for a specified string.
        /// </summary>
        /// <returns>Byte array representing input string.</returns>
        internal byte[] WriteCountOfBytesWithCompressionFlag(string value)
        {
            if (!StringUtil.HasChars(value))
                return null;

            Encoding encoding = StringUtil.ContainsOnlyAscii(value) ? Encoding.ASCII : Encoding.Unicode;

            byte[] raw = encoding.GetBytes(value);

            uint countOfBytes = (uint)raw.Length & 0x0fffffff;
            uint compressionFlag = (encoding.Equals(Encoding.ASCII) && (countOfBytes > 0)) ? 0x80000000 : 0x0;

            Write(compressionFlag | countOfBytes);

            return raw;
        }

        /// <summary>
        /// Writes GuidAndPicture as specified in [MS-OFORMS], 2.4.8 GuidAndPicture.
        /// </summary>
        internal void WriteGuidAndPicture(byte[] imageBytes)
        {
            Debug.Assert(ArrayUtil.HasData(imageBytes));

            // Write CLSID_StdPicture.
            WriteBytes(Forms2OleControl.ClsidStdPicture.ToByteArray());

            // Write StdPicture.
            WriteWithoutPadding(Forms2OleControl.StdPicturePreamble);
            WriteWithoutPadding((uint)imageBytes.Length);
            WriteBytes(imageBytes);
        }

        /// <summary>
        /// Writes GuidAndFont as specified in [MS-OFORMS], 2.4.7 GuidAndFont.
        /// </summary>
        internal void WriteGuidAndFont()
        {
            // The font must be either StdFont or TextProps.
            Debug.Assert(!StdFont.HasDefaultValue(mPr) || !Forms2TextProps.HasAllDefaultValues(mPr));

            if (!StdFont.HasDefaultValue(mPr))
            {
                // Write GUID and StdFont.
                Guid guid = new Guid(StdFont.Guid);
                mWriter.Write(guid.ToByteArray());

                StdFont font = (StdFont)mPr[Forms2Attr.Font];
                font.Write(mWriter);
            }
            else
            {
                // Write GUID and TextProps.
                Guid guid = new Guid(Forms2TextProps.Guid);
                mWriter.Write(guid.ToByteArray());

                Forms2TextProps.Write(mWriter, mPr);
            }
        }

        /// <summary>
        /// Writes the sum of sizes of PropMask, DataBlock, and ExtraDataBlock and a specified PropMask.
        /// </summary>
        internal void WriteDataSizeAndPropMask(int startPos, uint propMask)
        {
            WriteDataSizeAndPropMasks(startPos, propMask, 0xFFFFFFFF);
        }

        /// <summary>
        /// Writes the sum of sizes of PropMasks, DataBlock, and ExtraDataBlock and a specified PropMasks.
        /// </summary>
        internal void WriteDataSizeAndPropMasks(int startPos, uint propMask1, uint propMask2)
        {
            int sizeOffset = startPos + 2;
            short size = (short)((mWriter.BaseStream.Position - 2) - sizeOffset);
            mWriter.Seek(sizeOffset, SeekOrigin.Begin);

            mWriter.Write(size);
            mWriter.Write(propMask1);
            if (propMask2 != 0xFFFFFFFF)
                mWriter.Write(propMask2);

            mWriter.Seek(0, SeekOrigin.End);
        }

        /// <summary>
        /// Gets property specified by a key as raw bytes.
        /// </summary>
        private byte[] GetRawValue(int key, int valueSize)
        {
            // Special check for Font as it can be of two types: StdFont and TextProps.
            // In case of TextProps it is located in a bunch of corresponding Forms2Attrs,
            // and not in just a single Forms2Attr.Font.
            if (key == Forms2Attr.Font)
            {
                if (!HasFont())
                    return null;
            }
            else
            {
                // We should not write default values.
                if (mPr.IsDefaultValue(key))
                    return null;
            }

            switch (key)
            {
                case Forms2Attr.ForegroundColor:
                case Forms2Attr.BackgroundColor:
                case Forms2Attr.BorderColor:
                    return BitConverter.GetBytes(((OleColor)mPr[key]).ToRaw());

                case Forms2Attr.Size:
                case Forms2Attr.LogicalSize:
                    return ((OleSize)mPr[key]).ToRaw();

                case Forms2Attr.ScrollPosition:
                case Forms2Attr.SitePosition:
                    return ((OlePosition)mPr[key]).ToRaw();

                case Forms2Attr.Value:
                case Forms2Attr.Caption:
                case Forms2Attr.GroupName:
                {
                    string val = (string)mPr[key];
                    Encoding encoding = StringUtil.ContainsOnlyAscii(val)
                        ? Encoding.ASCII
                        : Encoding.Unicode;
                    return encoding.GetBytes(val);
                }

                case Forms2Attr.MouseIcon:
                case Forms2Attr.Picture:
                case Forms2Attr.Font:
                    return BitConverter.GetBytes((ushort)0xFFFF);

                case Forms2Attr.MatchEntry:
                case Forms2Attr.ShowDropButtonWhen:
                case Forms2Attr.DropButtonStyle:
                case Forms2Attr.BorderStyle:
                case Forms2Attr.DisplayStyle:
                case Forms2Attr.ScrollBars:
                case Forms2Attr.PicturePosition:
                case Forms2Attr.PictureSizeMode:
                case Forms2Attr.PictureAlignment:
                case Forms2Attr.PictureTiling:
                case Forms2Attr.SpecialEffect:
                case Forms2Attr.MousePointer:
                case Forms2Attr.ParagraphAlign:
                case Forms2Attr.ClsidCacheIndex:
                case Forms2Attr.Min:
                case Forms2Attr.Max:
                case Forms2Attr.Position:
                case Forms2Attr.PrevEnabled:
                case Forms2Attr.NextEnabled:
                case Forms2Attr.SmallChange:
                case Forms2Attr.Delay:
                case Forms2Attr.Cycle:
                case Forms2Attr.Zoom:
                case Forms2Attr.ID:
                case Forms2Attr.HelpContextID:
                case Forms2Attr.TabIndex:
                case Forms2Attr.ListIndex:
                case Forms2Attr.TabOrienation:
                case Forms2Attr.TabStyle:
                case Forms2Attr.TabData:
                case Forms2Attr.PageCount:
                case Forms2Attr.GridX:
                case Forms2Attr.GridY:
                case Forms2Attr.ClickControlMode:
                case Forms2Attr.DblClickControlMode:
                case Forms2Attr.MultiSelect:
                    return GetBytes((int)mPr[key], valueSize);

                case Forms2Attr.Accelerator:
                    return GetBytes((char)mPr[key], valueSize);

                case Forms2Attr.ProportionalThumb:
                    return GetBytes((ushort)mPr[key], valueSize);

                default:
                    return GetBytesUint((uint)mPr[key], valueSize);
            }
        }

        /// <summary>
        /// Returns raw bytes from a unsigned integer padded to the needed size from the end with zeroes.
        /// </summary>
        private static byte[] GetBytesUint(uint value, int size)
        {
            byte[] raw = BitConverter.GetBytes(value);
            if (size == 4)
                return raw;

            byte[] val = new byte[size];
            Array.Copy(raw, val, size);
            return val;
        }

        /// <summary>
        /// Returns bytes array from a specified string as specified [MS-OFORMS], 2.4.14.1 ArrayString.
        /// </summary>
        private static byte[] GetBytes(string value)
        {
            Encoding encoding = StringUtil.ContainsOnlyAscii(value) ? Encoding.ASCII : Encoding.Unicode;
            byte[] itemBytes = encoding.GetBytes(value);

            uint countOfBytes = (uint)itemBytes.Length & 0x0fffffff;
            uint compressionFlag = (encoding.Equals(Encoding.ASCII) && (countOfBytes > 0)) ? 0x80000000 : 0x0;
            byte[] countOfBytesWithCompressionFlag = BitConverter.GetBytes(compressionFlag | countOfBytes);

            int len = countOfBytesWithCompressionFlag.Length + itemBytes.Length;
            int pad = ((len % 4) == 0) ? 0 : 4 - (len % 4);

            byte[] bytes = new byte[len + pad];
            countOfBytesWithCompressionFlag.CopyTo(bytes, 0);
            itemBytes.CopyTo(bytes, 4);

            return bytes;
        }

        /// <summary>
        /// Returns raw bytes from a integer padded to the needed size from the end with zeroes.
        /// </summary>
        private static byte[] GetBytes(int value, int size)
        {
            return GetBytesUint((uint)value, size);
        }

        /// <summary>
        /// Writes byte array without paddings.
        /// </summary>
        private void WriteBytes(byte[] raw)
        {
            WriteBytes(raw, 1);
        }

        /// <summary>
        /// Writes unsigned integer without padding.
        /// </summary>
        private void WriteWithoutPadding(uint value)
        {
            byte[] raw = BitConverter.GetBytes(value);
            mWriter.Write(raw, 0, 4);
        }

        /// <summary>
        /// Returns true, if there is at least one non-empty font property.
        /// </summary>
        private bool HasFont()
        {
            return !StdFont.HasDefaultValue(mPr) || !Forms2TextProps.HasAllDefaultValues(mPr);
        }

        private readonly BinaryWriter mWriter;
        private readonly int mStart;
        private readonly Forms2Pr mPr;
        private static readonly byte[] gPadding = new byte[4];
    }
}
