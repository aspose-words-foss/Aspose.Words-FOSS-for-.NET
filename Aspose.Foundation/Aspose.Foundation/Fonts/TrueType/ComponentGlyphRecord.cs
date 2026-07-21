// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/06/2025 by Konstantin Kornilov

using Aspose.Drawing;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    internal class ComponentGlyphRecord
    {
        public static ComponentGlyphRecord Read(BigEndianBinaryReader reader)
        {
            ComponentGlyphRecord result = new ComponentGlyphRecord();
            result.Flags = reader.ReadUInt16();
            result.GlyphIndex = reader.ReadUInt16();
            result.ReadArgs(reader);
            result.ReadScale(reader);
            return result;
        }

        public void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(Flags);
            writer.WriteUInt16(GlyphIndex);
            WriteArgs(writer);
            WriteScale(writer);
        }

        private void ReadArgs(BigEndianBinaryReader reader)
        {
            if (Arg1And2AreWords && ArgsAreXYValues)
            {
                Arg1 = reader.ReadInt16();
                Arg2 = reader.ReadInt16();
            }
            else if (Arg1And2AreWords && !ArgsAreXYValues)
            {
                Arg1 = reader.ReadUInt16();
                Arg2 = reader.ReadUInt16();
            }
            else if (!Arg1And2AreWords && ArgsAreXYValues)
            {
                Arg1 = reader.ReadSByte();
                Arg2 = reader.ReadSByte();
            }
            else if (!Arg1And2AreWords && !ArgsAreXYValues)
            {
                Arg1 = reader.ReadByte();
                Arg2 = reader.ReadByte();
            }
        }

        public void WriteArgs(BigEndianBinaryWriter writer)
        {
            if (Arg1And2AreWords && ArgsAreXYValues)
            {
                writer.WriteInt16(Arg1);
                writer.WriteInt16(Arg2);
            }
            else if (Arg1And2AreWords && !ArgsAreXYValues)
            {
                writer.WriteUInt16(Arg1);
                writer.WriteUInt16(Arg2);
            }
            else if (!Arg1And2AreWords && ArgsAreXYValues)
            {
                writer.WriteSByte(Arg1);
                writer.WriteSByte(Arg2);
            }
            else if (!Arg1And2AreWords && !ArgsAreXYValues)
            {
                writer.WriteByte((byte)Arg1);
                writer.WriteByte((byte)Arg2);
            }
        }

        private void ReadScale(BigEndianBinaryReader reader)
        {
            if (WeHaveAScale)
            {
                XScale = new FixedPoint2Dot14(reader.ReadInt16());
            }
            else if (WeHaveXAndYScale)
            {
                XScale = new FixedPoint2Dot14(reader.ReadInt16());
                YScale = new FixedPoint2Dot14(reader.ReadInt16());
            }
            else if (WeHaveATwoByTwo)
            {
                XScale = new FixedPoint2Dot14(reader.ReadInt16());
                Scale01 = new FixedPoint2Dot14(reader.ReadInt16());
                Scale10 = new FixedPoint2Dot14(reader.ReadInt16());
                YScale = new FixedPoint2Dot14(reader.ReadInt16());
            }
        }

        private void WriteScale(BigEndianBinaryWriter writer)
        {
            if (WeHaveAScale)
            {
                writer.WriteInt16(XScale.Value);
            }
            else if (WeHaveXAndYScale)
            {
                writer.WriteInt16(XScale.Value);
                writer.WriteInt16(YScale.Value);
            }
            else if (WeHaveATwoByTwo)
            {
                writer.WriteInt16(XScale.Value);
                writer.WriteInt16(Scale01.Value);
                writer.WriteInt16(Scale10.Value);
                writer.WriteInt16(YScale.Value);
            }
        }

        public void UpdateXYSizeFlag()
        {
            // Update Arg1And2AreWords flag after updating args for the case of X/Y values.
            // For now only increase the size if it is required.
            Debug.Assert(ArgsAreXYValues);
            if (!Arg1And2AreWords && (ValueExceedsSByte(Arg1) || ValueExceedsSByte(Arg2)))
                Flags = (ushort)(Flags | FlagsArg1And2AreWords);
        }

        private static bool ValueExceedsSByte(int value)
        {
            return value > sbyte.MaxValue || value < sbyte.MinValue;
        }

        public ushort Flags { get; private set; }
        public ushort GlyphIndex { get; set; }
        public int Arg1 { get; set; }
        public int Arg2 { get; set; }
        public FixedPoint2Dot14 XScale { get; private set; }
        public FixedPoint2Dot14 YScale { get;  private set; }
        public FixedPoint2Dot14 Scale01 { get; private  set; }
        public FixedPoint2Dot14 Scale10 { get; private set; }

        public bool Arg1And2AreWords
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsArg1And2AreWords); }
        }

        public bool ArgsAreXYValues
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsArgsAreXyValues); }
        }

        public bool WeHaveAScale
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsWeHaveAScale); }
        }

        public bool MoreComponents
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsMoreComponents); }
        }

        public bool WeHaveXAndYScale
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsWeHaveXAndYScale); }
        }

        public bool WeHaveATwoByTwo
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsWeHaveATwoByTwo); }
        }

        public bool WeHaveInstructions
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsWeHaveInstructions); }
        }

        public bool RoundXYToGrid
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsRoundXyToGrid); }
        }

        public bool UseMyMetrics
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsUseMyMetrics); }
        }

        public bool OverlapCompound
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsOverlapCompound); }
        }

        public bool ScaledComponentOffset
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsScaledComponentOffset); }
        }

        public bool UnscaledComponentOffset
        {
            get { return BitUtil.IsSetUInt16(Flags, FlagsUnscaledComponentOffset); }
        }

        public bool ActualScaledComponentOffset
        {
            get
            {
                return !UnscaledComponentOffset && ScaledComponentOffset;
            }
        }

        public const ushort FlagsArg1And2AreWords = 0x0001;
        public const ushort FlagsArgsAreXyValues = 0x0002;
        public const ushort FlagsRoundXyToGrid = 0x0004;
        public const ushort FlagsWeHaveAScale = 0x0008;
        public const ushort FlagsMoreComponents = 0x0020;
        public const ushort FlagsWeHaveXAndYScale = 0x0040;
        public const ushort FlagsWeHaveATwoByTwo = 0x0080;
        public const ushort FlagsWeHaveInstructions = 0x0100;
        public const ushort FlagsUseMyMetrics = 0x0200;
        public const ushort FlagsOverlapCompound = 0x0400;
        public const ushort FlagsScaledComponentOffset = 0x0800;
        public const ushort FlagsUnscaledComponentOffset = 0x1000;
    }
}
