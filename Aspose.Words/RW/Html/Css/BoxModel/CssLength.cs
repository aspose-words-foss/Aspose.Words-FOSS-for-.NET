// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/05/2014 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a length value and a flag indicating whether the value is custom or comes from default (user agent) CSS.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable. All arithmetic operations on instances of this class return a new instance. 
    /// </remarks>
    internal class CssLength
    {
        internal CssLength(double value, bool isCustom)
        {
            mValue = value;
            mIsCustom = isCustom;
        }

        /// <summary>
        /// Length value.
        /// </summary>
        internal double Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Indicates whether the length value is custom (non-default), i.e. explicitly specified in the source CSS.
        /// </summary>
        internal bool IsCustom
        {
            get { return mIsCustom; }
        }

        public override string ToString()
        {
            string isCustom = (mIsCustom)
                ? "custom"
                : "default";
            return mValue + "; " + isCustom;
        }

        /// <summary>
        /// Sums two length values.
        /// </summary>
        /// <returns>
        /// The sum of the values. The resulting length is considered custom if any of specified lengths is custom.
        /// </returns>
        public static CssLength operator + (CssLength a, CssLength b)
        {
            return new CssLength(a.Value + b.Value, a.IsCustom || b.IsCustom);
        }

        /// <summary>
        /// Inverses the sing of this value.
        /// </summary>
        /// <returns>
        /// The value with the opposite sign. The custom flag remains unchanged.
        /// </returns>
        public static CssLength operator - (CssLength a)
        {
            return new CssLength(-a.Value, a.IsCustom);
        }

        /// <summary>
        /// Subtracts second value from the first one.
        /// </summary>
        /// <returns>
        /// The difference of the values. The resulting length is considered custom if any of specified lengths is custom.
        /// </returns>
        public static CssLength operator - (CssLength a, CssLength b)
        {
            return new CssLength(a.Value - b.Value, a.IsCustom || b.IsCustom);
        }

        /// <summary>
        /// A precalculated zero default length value.
        /// </summary>
        internal static CssLength ZeroDefault = new CssLength(0, false);

        private readonly double mValue;
        private readonly bool mIsCustom;
    }
}
