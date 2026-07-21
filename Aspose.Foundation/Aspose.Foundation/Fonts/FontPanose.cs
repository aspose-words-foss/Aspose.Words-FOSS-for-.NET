// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Konstantin Kornilov

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents the font PANOSE classification number.
    /// </summary>
    /// <remarks>
    /// See https://www.microsoft.com/typography/otspec/os2.htm#pan for more details.
    /// </remarks>
    public sealed class FontPanose
    {
        public FontPanose(byte[] values)
        {
            if(values == null)
                values = new byte[PanoseLength];

            if (values.Length != PanoseLength)
            {
                Debug.Fail("Incorrect PANOSE length.");
                values = new byte[PanoseLength];
            }

            mValues = values;
        }

        public byte[] Values
        {
            get { return mValues; }
        }

        #region Equality members

        private bool Equals(FontPanose other)
        {
            return ArrayUtil.IsArrayEqual(Values, other.Values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FontPanose)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (byte value in mValues)
                hashCode = (hashCode * 397) ^ value;

            return hashCode;
        }

        #endregion

        private readonly byte[] mValues;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int PanoseLength = 10;

        public const byte ValueAny = 0;
        public const byte ValueNoFit = 1;
        public static readonly FontPanose Empty = new FontPanose(new byte[PanoseLength]);
    }
}
