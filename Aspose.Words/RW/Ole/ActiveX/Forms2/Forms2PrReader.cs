// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements reading of the Forms2 control properties.
    /// </summary>
    internal class Forms2PrReader
    {
        internal Forms2PrReader(BinaryReader reader, Forms2Pr pr)
        {
            mStart = (int) reader.BaseStream.Position;
            mReader = reader;
            mPr = pr;
        }

        internal void ReadData(uint mask, int count, int key)
        {
            if ((Flags & mask) != 0)
            {
                uint val = ReadData(mask, count);
                HandleValue(key, val);
            }
        }

        internal uint ReadData(uint mask, int count)
        {
            if ((Flags & mask) == 0)
                return 0;

            ReadPadding(count);
            byte[] raw = new byte[4];
            byte[] read = mReader.ReadBytes(count);
            Array.Copy(read, raw, count);
            uint val = BitConverter.ToUInt32(raw, 0);

            return val;
        }

        internal void SkipData(uint mask, int count)
        {
            if ((Flags & mask) == 0)
                return;

            ReadPadding(count);
            mReader.BaseStream.Seek(count, SeekOrigin.Current);
        }

        internal void ReadString(uint mask, uint cValue, int key)
        {
            string value = ReadString(mask, cValue);
            if (value != null)
                HandleValue(key, value);
        }

        private string ReadString(uint mask, uint cValue)
        {
            if ((Flags & mask) == 0)
                return null;

            ReadPadding(4);

            return ReadString(mReader, cValue);
        }

        internal void ReadBytes(uint mask, int len, int key)
        {
            if ((Flags & mask) == 0)
                return;

            ReadPadding(4);
            byte[] bytes =  mReader.ReadBytes(len);
                HandleValue(key, bytes);
        }

        internal void ReadPadding(int val)
        {
            int pos = (int)mReader.BaseStream.Position - mStart;

            int div = pos%val;

            if (div == 0 || val == 1)
                return;
            int pad = val - div;
            mReader.ReadBytes(pad);
        }

        private static string ReadString(BinaryReader reader, uint cValue)
        {
            bool compressed = (cValue & 0x80000000) != 0;
            uint size = cValue & 0x0fffffff;

            byte[] raw = reader.ReadBytes((int)size);

            Encoding encoding = compressed ? Encoding.ASCII : Encoding.Unicode;
            return encoding.GetString(raw);
        }

        internal IList<string> ReadStringArray(uint mask, uint size)
        {
            IList<string> stringList = new List<string>();

            long startPos = mReader.BaseStream.Position;
            while (startPos + size > mReader.BaseStream.Position)
            {
                uint cText = ReadData(mask, 4);
                string text = ReadString(mask, cText);
                ReadPadding(4);
                stringList.Add(text);
            }

            return stringList;
        }

        /// <summary>
        /// Reads GuidAndPicture as specified in [MS-OFORMS] 2.4.8 GuidAndPicture into a specified key.
        /// </summary>
        internal void ReadGuidAndPicture(int key)
        {
            Guid clsid = new Guid(mReader.ReadBytes(16));
            Debug.Assert(clsid == Forms2OleControl.ClsidStdPicture);

            // SdtPicture structure.
            int preamble = mReader.ReadInt32();
            Debug.Assert(preamble == Forms2OleControl.StdPicturePreamble);

            uint size = mReader.ReadUInt32();
            mPr.SetAttr(key, mReader.ReadBytes((int) size));
        }

        /// <summary>
        /// Reads GuidAndFont as specified in [MS-OFORMS] 2.4.7 GuidAndFont.
        /// </summary>
        /// <remarks>
        /// FormFont in this structure can be of two types: <see cref="StdFont"/> or <see cref="Forms2TextProps"/>.
        /// If font is StdFont, then it will be read into <see cref="Forms2Attr.Font"/> key,
        /// otherwise it will be read into a set of Forms2Attr keys that correspond to [MS-OFORMS] 2.3.2 TextPropsPropMask.
         /// </remarks>
        internal void ReadGuidAndFont()
        {
            string guid = new Guid(mReader.ReadBytes(16)).ToString();

            switch (guid)
            {
                case StdFont.Guid:
                {
                    StdFont.Read(mReader, mPr);
                    break;
                }
                case Forms2TextProps.Guid:
                {
                    Forms2TextProps.Read(mReader, mPr);
                    break;
                }
                default:
                    throw new InvalidOperationException(string.Format("Invalid GuidAndFont GUID: {0}", guid));
            }
        }

        private void HandleValue(int key, uint value)
        {
            switch (key)
            {
                case Forms2Attr.ForegroundColor:
                case Forms2Attr.BackgroundColor:
                case Forms2Attr.BorderColor:
                    mPr.SetAttr(key, OleColor.FromRaw(value));
                    break;

                case Forms2Attr.VariousPropertyBits:
                    mPr.SetAttr(key, (VariousPropertiesBits)value);
                    break;

                case Forms2Attr.MatchEntry:
                    mPr.SetAttr(key, (MatchEntry)value);
                    break;

                case Forms2Attr.ShowDropButtonWhen:
                    mPr.SetAttr(key, (ShowDropButtonWhen)value);
                    break;

                case Forms2Attr.DropButtonStyle:
                    mPr.SetAttr(key, (DropButtonStyle)value);
                    break;

                case Forms2Attr.ListStyle:
                    mPr.SetAttr(key, (ListStyle)value);
                    break;

                case Forms2Attr.MultiSelect:
                    mPr.SetAttr(key, (MultiSelect)value);
                    break;

                case Forms2Attr.BorderStyle:
                    mPr.SetAttr(key, (BorderStyle)value);
                    break;

                case Forms2Attr.DisplayStyle:
                    mPr.SetAttr(key, (DisplayStyle)value);
                    break;

                case Forms2Attr.ScrollBars:
                    mPr.SetAttr(key, (ScrollBars)value);
                    break;

                case Forms2Attr.PicturePosition:
                    mPr.SetAttr(key, (PicturePosition)value);
                    break;

                case Forms2Attr.PictureSizeMode:
                    mPr.SetAttr(key, (PictureSizeMode)value);
                    break;

                case Forms2Attr.PictureAlignment:
                    mPr.SetAttr(key, (PictureAlignment)value);
                    break;

                case Forms2Attr.SpecialEffect:
                    mPr.SetAttr(key, (SpecialEffect)value);
                    break;

                case Forms2Attr.Orientation:
                    mPr.SetAttr(key, (FormOrientation)value);
                    break;

                case Forms2Attr.ProportionalThumb:
                    mPr.SetAttr(key, (ProportionalThumb)value);
                    break;

                case Forms2Attr.MousePointer:
                    mPr.SetAttr(key, (MousePointer)value);
                    break;

                case Forms2Attr.ParagraphAlign:
                    mPr.SetAttr(key, (ParagraphAlign)value);
                    break;

                case Forms2Attr.Cycle:
                    mPr.SetAttr(key, (Cycle)value);
                    break;

                case Forms2Attr.BitFlagsSite:
                    mPr.SetAttr(key, (SiteFlag)value);
                    break;

                case Forms2Attr.BitFlagsDX:
                    mPr.SetAttr(key, (DesignExtenderFlag)value);
                    break;

                case Forms2Attr.TabOrienation:
                    mPr.SetAttr(key, (TabOrientation)value);
                    break;

                case Forms2Attr.TabStyle:
                    mPr.SetAttr(key, (TabStyle)value);
                    break;

                case Forms2Attr.TransitionEffect:
                    mPr.SetAttr(key, (TransitionEffect)value);
                    break;

                case Forms2Attr.ClickControlMode:
                    mPr.SetAttr(key, (ClickControlMode)value);
                    break;

                case Forms2Attr.DblClickControlMode:
                    mPr.SetAttr(key, (DblClickControlMode)value);
                    break;

                case Forms2Attr.ClsidCacheIndex:
                    mPr.SetAttr(key, (ClsidCacheIndex)value);
                    break;

                case Forms2Attr.BooleanProperties:
                    mPr.SetAttr(key, (FormFlags)value);
                    break;

                case Forms2Attr.Min:
                case Forms2Attr.Max:
                case Forms2Attr.Position:
                case Forms2Attr.PrevEnabled:
                case Forms2Attr.NextEnabled:
                case Forms2Attr.SmallChange:
                case Forms2Attr.Delay:
                case Forms2Attr.Zoom:
                case Forms2Attr.ID:
                case Forms2Attr.HelpContextID:
                case Forms2Attr.TabIndex:
                case Forms2Attr.ListIndex:
                case Forms2Attr.PageCount:
                case Forms2Attr.GridX:
                case Forms2Attr.GridY:
                    mPr.SetAttr(key, (int)value);
                    break;

                case Forms2Attr.Accelerator:
                    mPr.SetAttr(key, (char)value);
                    break;

                default:
                    mPr.SetAttr(key, value);
                    break;
            }
        }

        private void HandleValue(int key, string value)
        {
            switch (key)
            {
                case Forms2Attr.Caption:
                case Forms2Attr.Value:
                case Forms2Attr.GroupName:
                case Forms2Attr.FontName:
                    mPr.SetAttr(key, value);
                    break;

                default:
                    mPr.SetAttr(key, value);
                    break;
            }
        }

        private void HandleValue(int key, byte[] value)
        {
            switch (key)
            {
                case Forms2Attr.Size:
                case Forms2Attr.LogicalSize:
                    mPr.SetAttr(key, OleSize.FromRaw(value));
                    break;

                case Forms2Attr.ScrollPosition:
                case Forms2Attr.SitePosition:
                    mPr.SetAttr(key, OlePosition.FromRaw(value));
                    break;

                default:
                    mPr.SetAttr(key, value);
                    break;
            }
        }

        internal uint Flags;

        private readonly BinaryReader mReader;
        private readonly int mStart;
        private readonly Forms2Pr mPr;
    }
}
