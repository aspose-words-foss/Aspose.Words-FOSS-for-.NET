// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2014 by Alexey Butalov

using System.Text;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a function value.
    /// The format of a function value is name(argument).
    ///   name - an CSS identifier value.
    ///   argument - collection of any supported CSS values. Can be empty.
    /// </summary>
    internal class CssFunctionValue : CssValue
    {
        internal CssFunctionValue(string name, CssValueList arguments)
            : base(CssValueType.Function, GetCss(name, arguments))
        {
            Debug.Assert(name != null);
            Debug.Assert(arguments != null);
            mName = name;
            mArguments = new CssValueList(arguments);
            mValue = GetCss(mName, mArguments);
        }

        internal CssFunctionValue(string name, CssValue value)
            : this(name, new CssValueList(value))
        {
            // Empty constructor.
        }

        internal override void ToCss(StringBuilder sb)
        {
            ToCss(mName, mArguments, sb);
        }

        protected override bool DoEquals(CssValue other)
        {
            Debug.Assert(other is CssFunctionValue);
            CssFunctionValue otherFunction = (CssFunctionValue)other;
            return StringUtil.EqualsIgnoreCase(mName, otherFunction.mName) && mArguments.Equals(otherFunction.mArguments);
        }

        internal override DrColor ParseAsColor()
        {
            switch (Name)
            {
                case "rgb":
                    return CssColorParser.ParseRgbFunctionColor(Arguments);
                case "hsl":
                    return CssColorParser.ParseHslFunctionColor(Arguments);
                default:
                    return null;
            }
        }

        private static void ToCss(string name, CssValueList argument, StringBuilder sb)
        {
            sb.Append(name);
            sb.Append('(');
            sb.Append(argument.ToCss());
            sb.Append(')');
        }

        private static string GetCss(string name, CssValueList argument)
        {
            StringBuilder sb = new StringBuilder();
            ToCss(name, argument, sb);
            return sb.ToString();
        }

        internal new string Value
        {
            get { return mValue; }
        }

        internal string Name
        {
            get { return mName; }
        }

        internal CssValueList Arguments
        {
            get { return mArguments; }
        }

        private readonly string mName;
        private readonly CssValueList mArguments;
        private readonly string mValue;
    }
}
