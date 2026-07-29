// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/23/2016 by Andrey Noskov

using Aspose.Collections;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Base class for all elements which have extension list (CT_OfficeArtExtensionList - [ISO/IEC29500-1:2012] section A.4.1) 
    /// in which all future extensions of element type ext is defined. The extension list, along
    /// with corresponding future extensions, is used to extend the storage capabilities of the DrawingML 
    /// framework. This enables various types of data to be stored natively in the framework.
    /// </summary>
    internal abstract class DmlExtensionListSource : IDmlExtensionListSource
    {
        public virtual StringToObjDictionary<DmlExtension> Extensions
        {
            get { return mExtLst; }
            set { mExtLst = value; }
        }

        /// <summary>
        /// Clones this extension collection.
        /// </summary>
        protected StringToObjDictionary<DmlExtension> CloneExtensions()
        {
            return CloneExtensions(mExtLst);
        }

        /// <summary>
        /// Clones this extension collection.
        /// </summary>
        internal static StringToObjDictionary<DmlExtension> CloneExtensions(StringToObjDictionary<DmlExtension> src)
        {
            if (src == null)
                return null;

            StringToObjDictionary<DmlExtension> lhsExtLst = new StringToObjDictionary<DmlExtension>();
            foreach (DmlExtension ext in src.Values)
                lhsExtLst[ext.Uri] = ext.Clone();

            return lhsExtLst;
        }

        internal bool HasExtensions
        {
            get { return mExtLst != null; }
        }

        private StringToObjDictionary<DmlExtension> mExtLst;
    }
}