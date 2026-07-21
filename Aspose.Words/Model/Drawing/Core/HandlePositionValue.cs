// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2010 by Dmitry Burov

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Describes a positioning of adjust handle along the specific coordinate axis.
    /// </summary>
    internal class HandlePositionValue
    {
        internal HandlePositionValue() : this(HandlePositionType.Unknown, 0)
        {
        }

        internal HandlePositionValue(HandlePositionType type, int value)
        {
            mType = type;
            mValue = value;
        }

        /// <summary>
        /// Expands the structure from a packed binary value.
        /// Please see MS-ODRAW p. 2.2.57 for details (apX and apY props).
        /// </summary>
        internal HandlePositionValue(int packedValue)
        {
            mValue = 0;
            switch (packedValue)
            {
                case 0x00000000:
                {
                    mType = HandlePositionType.LeftTop;
                    return;
                }
                case 0x00000001:
                {
                    mType = HandlePositionType.RightBottom;
                    return;
                }
                case 0x00000002:
                {
                    mType = HandlePositionType.Center;
                    return;
                }
                default:
                {
                    // Formula index
                    if (packedValue >= 0x00000003 && packedValue <= 0x00000084)
                    {
                        mType = HandlePositionType.Formula;
                        mValue = packedValue - 0x00000003;
                        return;
                    }

                    // Adjust value
                    Debug.Assert(packedValue >= 0x00000100 && packedValue <= 0x00000107);
                    mType = HandlePositionType.Adjust;
                    mValue = packedValue - 0x00000100;
                    return;
                }
            }
        }

        /// <summary>
        /// Gets a packed binary representation of the structure according to MS-ODRAW p. 2.2.57.
        /// </summary>
        internal int PackedValue
        {
            get
            {
                switch (mType)
                {
                    case HandlePositionType.LeftTop:
                        return 0x00000000;
                    case HandlePositionType.RightBottom:
                        return 0x00000001;
                    case HandlePositionType.Center:
                        return 0x00000002;
                    case HandlePositionType.Formula:
                        return 0x00000003 + mValue;
                    case HandlePositionType.Adjust:
                        return 0x00000100 + mValue;
                    default:
                        return mValue;
                }
            }
        }

        /// <summary>
        /// Type of positioning.
        /// Please see (MS-ODRAW p 2.2.57 ADJH) for further details.
        /// </summary>
        internal HandlePositionType Type
        {
            get { return mType; }
        }

        /// <summary>
        /// Type dependent value.
        /// Please see (MS-ODRAW p 2.2.57 ADJH) for further details.
        /// </summary>
        internal int Value
        {
            get { return mValue; }
        }

        private readonly HandlePositionType mType;
        private readonly int mValue;
    }
}
