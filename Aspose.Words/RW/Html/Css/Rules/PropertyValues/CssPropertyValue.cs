// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2013 by Alexey Butalov

using System.Text;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents CSS property value.
    /// CSS property value contains one or more CSS values.
    /// This class is immutable. All derived classes should be immutable also. 
    /// </summary>
    internal class CssPropertyValue
    {
        internal CssPropertyValue(CssValue cssValue)
        {
            Debug.Assert(cssValue != null);
            mCssValues = new CssValueList(cssValue);
            SetProperties();
        }

        internal CssPropertyValue(CssValueList cssValues)
        {
            Debug.Assert(cssValues.Count != 0);
            mCssValues = new CssValueList(cssValues);
            SetProperties();
        }

        internal CssPropertyValue(CssPropertyValue value)
        {
            mCssValues = new CssValueList(value.Count);
            for (int i = 0; i < value.Count; i++)
                mCssValues.Add(value[i]);
            SetProperties();
        }

        public override int GetHashCode()
        {
            return mCssValues.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CssValue)
                return Equals((CssValue)obj);
            if (obj is CssPropertyValue)
                return Equals((CssPropertyValue)obj);
            if (obj is CssValueList)
                return Equals((CssValueList)obj);
            if (obj is CssValue[])
                return Equals((CssValue[])obj);

            Debug.Assert(false);
            return false;
        }

        /// <summary>
        /// Determines whether the specified CSS property value is equal to the current value.
        /// </summary>
        internal bool Equals(CssPropertyValue other)
        {
            // Standard reference comparisons.
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return mCssValues.Equals(other.mCssValues);
        }

        /// <summary>
        /// Determines whether the specified CSS values are equal to the current value.
        /// </summary>
        internal bool Equals(CssValueList cssValues)
        {
            return mCssValues.Equals(cssValues);
        }

        /// <summary>
        /// Determines whether the specified CSS values are equal to the current value.
        /// </summary>
        internal bool Equals(params CssValue[] cssValues)
        {
            return mCssValues.Equals(cssValues);
        }

        /// <summary>
        /// Determines whether the specified CSS value is equal to the current value.
        /// </summary>
        internal bool Equals(CssValue value)
        {
            return (Count == 1) && mCssValues[0].Equals(value);
        }

        /// <summary>
        /// Gets a CSS representation of the property value.
        /// </summary>
        internal string ToCss()
        {
            StringBuilder sb = new StringBuilder();
            ToCss(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Outputs a CSS representation of the property value to StringBuilder.
        /// </summary>
        internal virtual void ToCss(StringBuilder sb)
        {
            if (IsInherit)
            {
                CssValue.Inherit.ToCss(sb);
            }
            else
            {
                ValueToCss(sb);
            }
        }

        /// <summary>
        /// Override in derived classes to output CSS representation of the property value to StringBuilder.
        /// You don't need process 'inherit' value in this method.
        /// </summary>
        protected virtual void ValueToCss(StringBuilder sb)
        {
            for (int i = 0; i < mCssValues.Count; i++)
            {
                if (i != 0)
                    sb.Append(' ');
                mCssValues[i].ToCss(sb);
            }
        }

        internal bool Contains(CssValue value)
        {
            return mCssValues.Contains(value);
        }

        internal DrColor ParseAsColor()
        {
            return (Count == 1)
                ? FirstValue.ParseAsColor()
                : null;
        }

        private void SetProperties()
        {
            mIsInherit = true;
            foreach (CssValue cssValue in mCssValues)
            {
                if (mIsInherit && !cssValue.Equals(CssValue.Inherit))
                    mIsInherit = false;
            }

            mIsInitial = (mCssValues.Count == 1) && mCssValues[0].Equals(CssValue.Initial);
        }

        /// <summary>
        /// Gets whether the property value is 'inherit'.
        /// </summary>
        internal virtual bool IsInherit
        {
            get { return mIsInherit; }
        }

        /// <summary>
        /// Gets whether the property value is 'initial'.
        /// </summary>
        internal virtual bool IsInitial
        {
            get { return mIsInitial; }
        }

        internal int Count
        {
            get { return mCssValues.Count; }
        }

        internal CssValue this[int index]
        {
            get { return mCssValues[index]; }
        }

        internal CssValue FirstValue
        {
            get { return mCssValues[0]; }
        }

        private readonly CssValueList mCssValues;

        /// <summary>
        /// Gets whether the property value is 'inherit'. Added for performance optimization.
        /// </summary>
        private bool mIsInherit;
        /// <summary>
        /// Gets whether the property value is 'initial'. Added for performance optimization.
        /// </summary>
        private bool mIsInitial;
    }
}
