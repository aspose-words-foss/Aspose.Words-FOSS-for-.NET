// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes
{
    /// <summary>
    /// Model Identifier with GUID value.
    /// </summary>
    internal class DmlModelIdGuid : DmlModelId
    {
        internal DmlModelIdGuid(Guid value)
        {
            mValue = value;
        }

        protected bool Equals(DmlModelIdGuid other)
        {
            return mValue.Equals(other.mValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(DmlModelIdGuid))
                return false;
            return Equals((DmlModelIdGuid)obj);
        }

        public override int GetHashCode()
        {
            return mValue.GetHashCode();
        }

        public override string ToString()
        {
            return mValue.ToString();
        }

        internal Guid Value
        {
            get { return mValue; }
        }

        private readonly Guid mValue;
    }
}
