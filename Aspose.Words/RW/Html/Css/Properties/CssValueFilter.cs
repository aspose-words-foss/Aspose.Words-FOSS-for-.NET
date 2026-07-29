// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2021 by Victor Chebotok

using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements a CSS value filter that passes (accepts) only values of a specified subset.
    /// </summary>
    internal static class CssValueFilter
    {
        /// <summary>
        /// Creates a filter that accepts values that are accepted by any of the specified sub-filters (logical "OR").
        /// </summary>
        internal static ICssValueFilter AnyOf(params ICssValueFilter[] filters)
        {
            return new CssAnyOfOtherFiltersFilter(filters);
        }

        /// <summary>
        /// Creates a filter that accepts only the specified CSS value.
        /// </summary>
        internal static ICssValueFilter Value(CssValue value)
        {
            return Values(value);
        }

        /// <summary>
        /// Creates a filter that accepts any of the specified CSS values (logical "OR").
        /// </summary>
        internal static ICssValueFilter Values(params CssValue[] values)
        {
            return new CssValuesFilter(values);
        }

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Number"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Number = new CssValueTypeFilter(CssValueType.Number);

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Length"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Length = new CssValueTypeFilter(CssValueType.Length);

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Length"/> values.
        /// In Quirks mode, this filter also accepts <see cref="CssValueType.Number"/> values.
        /// </summary>
        internal static readonly ICssValueFilter QuirkyLength = new CssQuirkyLengthFilter();

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Percentage"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Percentage = new CssValueTypeFilter(CssValueType.Percentage);

        /// <summary>
        /// A filter that accepts non-negative <see cref="CssValueType.Number"/> values.
        /// </summary>
        internal static readonly ICssValueFilter NonNegativeNumber = new CssNonNegativeNumericFilter(CssValueType.Number);

        /// <summary>
        /// A filter that accepts non-negative <see cref="CssValueType.Length"/> values.
        /// </summary>
        internal static readonly ICssValueFilter NonNegativeLength = new CssNonNegativeNumericFilter(CssValueType.Length);

        /// <summary>
        /// A filter that accepts non-negative <see cref="CssValueType.Length"/> values.
        /// In Quirks mode, this filter also accepts non-negative <see cref="CssValueType.Number"/> values.
        /// </summary>
        internal static readonly ICssValueFilter NonNegativeQuirkyLength = new CssNonNegativeQuirkyLengthFilter();

        /// <summary>
        /// A filter that accepts non-negative <see cref="CssValueType.Percentage"/> values.
        /// </summary>
        internal static readonly ICssValueFilter NonNegativePercentage = new CssNonNegativeNumericFilter(CssValueType.Percentage);

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Identifier"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Identifier = new CssValueTypeFilter(CssValueType.Identifier);

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.String"/> values.
        /// </summary>
        internal static readonly ICssValueFilter String = new CssValueTypeFilter(CssValueType.String);

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Hash"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Hash = new CssValueTypeFilter(CssValueType.Hash);

        /// <summary>
        /// A filter that accepts color values.
        /// </summary>
        /// <remarks>
        /// Note that color values may be represented by identifiers (color names), hash (hex color values),
        /// or functions ("rgb" or "hsl").
        /// </remarks>
        internal static readonly ICssValueFilter Color = new CssColorFilter();

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Uri"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Uri = new CssValueTypeFilter(CssValueType.Uri);

        /// <summary>
        /// A filter that accepts <see cref="CssValueType.Function"/> values.
        /// </summary>
        internal static readonly ICssValueFilter Function = new CssValueTypeFilter(CssValueType.Function);

        /// <summary>
        /// Implements a filter that is a logical "OR" combination of other filters.
        /// </summary>
        private class CssAnyOfOtherFiltersFilter : ICssValueFilter
        {
            internal CssAnyOfOtherFiltersFilter(ICssValueFilter[] filters)
            {
                mFilters = filters;
            }

            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                foreach (ICssValueFilter filter in mFilters)
                {
                    if (filter.Accepts(value, isInQuirksMode))
                    {
                        return true;
                    }
                }
                return false;
            }

            private readonly ICssValueFilter[] mFilters;
        }

        /// <summary>
        /// Implements a filter that accepts only specified concrete CSS values.
        /// </summary>
        private class CssValuesFilter : ICssValueFilter
        {
            internal CssValuesFilter(CssValue[] values)
            {
                mValues = values;
            }

            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                return value.EqualsAny(mValues);
            }

            private readonly CssValue[] mValues;
        }

        /// <summary>
        /// Implements a filter that accepts only non-negative numeric values of the specified type.
        /// </summary>
        private class CssNonNegativeNumericFilter : ICssValueFilter
        {
            internal CssNonNegativeNumericFilter(CssValueType valueType)
            {
                mValueType = valueType;
            }

            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                return (value.ValueType == mValueType) && (value.DoubleValue >= 0);
            }

            private readonly CssValueType mValueType;
        }

        /// <summary>
        /// Implements a filter that accepts lengths. In Quirks mode, this filter also accepts unitless numbers.
        /// </summary>
        private class CssQuirkyLengthFilter : ICssValueFilter
        {
            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                return value.IsLength(isInQuirksMode);
            }
        }

        /// <summary>
        /// Implements a filter that accepts non-negative lengths.
        /// In Quirks mode, this filter also accepts non-negative unitless numbers.
        /// </summary>
        private class CssNonNegativeQuirkyLengthFilter : ICssValueFilter
        {
            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                return value.IsLength(isInQuirksMode) && (value.DoubleValue >= 0);
            }
        }

        /// <summary>
        /// Implements a filter that accepts only CSS values of the specified type.
        /// </summary>
        private class CssValueTypeFilter : ICssValueFilter
        {
            internal CssValueTypeFilter(CssValueType valueType)
            {
                mValueType = valueType;
            }

            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                return value.ValueType == mValueType;
            }

            private readonly CssValueType mValueType;
        }

        /// <summary>
        /// Implements a filter that accepts only CSS values that represent colors.
        /// </summary>
        private class CssColorFilter : ICssValueFilter
        {
            public bool Accepts(CssValue value, bool isInQuirksMode)
            {
                DrColor color = value.ParseAsColor();
                return color != null;
            }
        }
    }
}
