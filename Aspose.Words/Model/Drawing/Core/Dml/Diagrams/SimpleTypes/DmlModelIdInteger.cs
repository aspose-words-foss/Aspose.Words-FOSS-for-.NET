// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Common;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes
{
    /// <summary>
    /// Model Identifier with integer value.
    /// </summary>
    internal class DmlModelIdInteger : DmlModelId
    {
        internal DmlModelIdInteger(int value)
        {
            mValue = value;
        }

        protected bool Equals(DmlModelIdInteger other)
        {
            return mValue == other.mValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(DmlModelIdInteger))
                return false;
            return Equals((DmlModelIdInteger)obj);
        }

        public override int GetHashCode()
        {
            return mValue;
        }

        public override string ToString()
        {
            return FormatterPal.IntToStr(mValue);
        }

        internal int Value
        {
            get { return mValue; }
        }

        private readonly int mValue;
    }
}
