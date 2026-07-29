// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// Set of properties of <see cref="DmlPropertySet"/>.
    /// </summary>
    internal class DmlPropertySetPr
    {
        internal void SetProperty(DmlPropertySetAttr attr, object value)
        {
            mPrSet[(int)attr] = value;
        }

        internal object GetProperty(DmlPropertySetAttr attr)
        {
            return mPrSet[(int)attr];
        }

        internal DmlModelId PresAssocId
        {
            get
            {
                if(!mPrSet.ContainsKey((int)DmlPropertySetAttr.PresAssocId))
                    return null;

                return (DmlModelId)mPrSet[(int) DmlPropertySetAttr.PresAssocId];
            }
        }

        internal string PresName
        {
            get
            {
                if (!mPrSet.ContainsKey((int)DmlPropertySetAttr.PresName))
                    return null;

                return (string)mPrSet[(int)DmlPropertySetAttr.PresName];
            }
        }

        private readonly IntToObjDictionary<object> mPrSet = new IntToObjDictionary<object>();
    }
}
