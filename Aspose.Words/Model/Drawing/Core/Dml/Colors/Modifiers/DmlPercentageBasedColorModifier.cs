// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// Base class for color modifiers with the percentage property.
    /// </summary>
    internal abstract class DmlPercentageBasedColorModifier : DmlColorModifier
    {
        public override IDmlColorModifier Clone()
        {
            DmlPercentageBasedColorModifier newColorModifier = CreateEmptyObject();
            newColorModifier.Value = Value;
            return newColorModifier;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlPercentageBasedColorModifier value = (DmlPercentageBasedColorModifier)obj;

            return (MathUtil.AreEqual(value.Value, Value));
        }

        public override int GetHashCode()
        {
            int hash = ModifierType.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        protected static void WriteCore(string tagName, string prefix, double value, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string tagNameWithPrefix = string.Format("{0}:{1}", prefix, tagName);
            builder.StartElement(tagNameWithPrefix);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "val"),
                DmlPercentageUtil.ToPercentOrDmlPercent(value, isIsoStrict));
            builder.EndElement(tagNameWithPrefix);
        }

        protected abstract DmlPercentageBasedColorModifier CreateEmptyObject();

        /// <summary>
        /// Value is in fraction representation.
        /// </summary>
        internal double Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        private double mValue;
    }
}
