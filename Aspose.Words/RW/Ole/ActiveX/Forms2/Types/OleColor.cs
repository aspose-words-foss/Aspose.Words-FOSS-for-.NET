// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System.Drawing;
using Aspose.JavaAttributes;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements [MS-OFORMS] 2.4.9 OLE_COLOR structure.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    internal class OleColor
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private OleColor(byte red, byte green, byte blue, OleColorType type)
        {
            mRed = red;
            mGreen = green;
            mBlue = blue;

            mOleColorType = type;
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((OleColor)obj);
        }

        /// <summary>
        /// Gets hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)mOleColorType;
                hashCode = (hashCode * 397) ^ mRed.GetHashCode();
                hashCode = (hashCode * 397) ^ mGreen.GetHashCode();
                hashCode = (hashCode * 397) ^ mBlue.GetHashCode();
                return hashCode;
            }
        }

#if TEST || DEBUG
        /// <summary>
        /// Converts the OleColor object to a string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Raw:0x{0:X8}, RGB:({1}, R:0x{2:x2}, G:0x{3:x2}, B:0x{4:x2})",
                ToRaw(), mOleColorType, mRed, mGreen, mBlue);
        }
#endif

        /// <summary>
        /// Creates a OleColor object from a raw value.
        /// </summary>
        internal static OleColor FromRaw(uint raw)
        {
            return new OleColor(
                (byte)(raw & 0x000000ff),
                (byte)((raw & 0x0000ff00) >> 8),
                (byte)((raw & 0x00ff0000) >> 16),
                (OleColorType)((raw & 0xff000000) >> 24));
        }

        /// <summary>
        /// Creates a OleColor object from a Color object.
        /// </summary>
        internal static OleColor FromColor(Color color)
        {
            return new OleColor((byte)color.R, (byte)color.G, (byte)color.B, OleColorType.Default);
        }

        /// <summary>
        /// Gets raw value from this OleColor instance.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal uint ToRaw()
        {
            uint raw = mRed | (uint)(mGreen << 8) | (uint)(mBlue << 16) | (uint)((byte)mOleColorType << 24);
            return raw;
        }

        /// <summary>
        /// Determines whether two OleColor objects are equal.
        /// </summary>
        private bool Equals(OleColor other)
        {
            Debug.Assert(other != null);

            return (mOleColorType == other.mOleColorType) &&
                   (mRed == other.mRed) &&
                   (mGreen == other.mGreen) &&
                   (mBlue == other.mBlue);
        }


        /// <summary>
        /// Gets Color from this OleColor instance.
        /// </summary>
        internal Color Color
        {
            get
            {
                if (mOleColorType == OleColorType.SystemPalette)
                    return GetSystemPaletteColor(mRed | (mGreen << 8));
                return Color.FromArgb(mRed, mGreen, mBlue);
            }
        }

        private static Color GetSystemPaletteColor(int paletteIndex)
        {
            switch (paletteIndex)
                    {
                        case 0:
                            return SystemColors.ScrollBar;
                        case 1:
                            return SystemColors.Desktop;
                        case 2: // Active title bar
                            return SystemColors.ActiveCaption;
                        case 3: // Inactive title bar
                            return SystemColors.InactiveCaption;
                        case 4:
                            return SystemColors.MenuBar;
                        case 5:
                            return SystemColors.Window;
                        case 6:
                            return SystemColors.WindowFrame;
                        case 7:
                            return SystemColors.MenuText;
                        case 8:
                            return SystemColors.WindowText;
                        case 9: // Active title bar text
                            return SystemColors.ActiveCaptionText;
                        case 10:
                            return SystemColors.ActiveBorder;
                        case 11:
                            return SystemColors.InactiveBorder;
                        case 12:
                            return SystemColors.AppWorkspace;
                        case 13:
                            return SystemColors.Highlight;
                        case 14:
                            return SystemColors.HighlightText;
                        case 15:
                            return SystemColors.ButtonFace;
                        case 16:
                            return SystemColors.ButtonShadow;
                        case 17: // Disabled text
                            return SystemColors.GrayText;
                        case 18: // Button text
                            return SystemColors.ControlText;
                        case 19: // Inactive title bar text
                            return SystemColors.InactiveCaptionText;
                        case 20:
                            return SystemColors.ButtonHighlight;
                        case 21: // Button dark shadow
                            return SystemColors.ControlDark;
                        case 22: // Button light shadow
                            return SystemColors.ControlLight;
                        case 23:
                            return SystemColors.InfoText;
                        case 24:
                            return SystemColors.Info;
                        default:
                            return SystemColors.Desktop;
                    }
        }

        private readonly byte mRed;
        private readonly byte mGreen;
        private readonly byte mBlue;

        private readonly OleColorType mOleColorType;
    }
}
