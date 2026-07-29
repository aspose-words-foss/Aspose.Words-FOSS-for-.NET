// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    internal class BoolConstant : ConditionalExpression
    {
        internal BoolConstant(bool value)
        {
            mValue = value;
        }

        internal override bool Matches(Features features)
        {
            return mValue;
        }

        public override string ToString()
        {
            return (mValue)
                ? "true"
                : "false";
        }

        protected override bool IsSubexpression
        {
            get { return false; }
        }

        private readonly bool mValue;
    }
}