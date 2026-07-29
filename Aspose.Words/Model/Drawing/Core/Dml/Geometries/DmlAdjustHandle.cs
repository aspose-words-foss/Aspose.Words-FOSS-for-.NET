// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal abstract class DmlAdjustHandle
    {
        /// <summary>
        /// Returns adjust type if it is used in this handle and <see cref="DmlAdjustType.Unknown"/> otherwise.
        /// </summary>
        public abstract DmlAdjustType GetAdjustType(string adjustName);

        /// <summary>
        /// Clones this instance of adjust handle.
        /// </summary>
        internal abstract DmlAdjustHandle Clone();

        protected static bool CompareAdjustNames(string adjustName, string guideReference)
        {
            if (!StringUtil.HasChars(adjustName))
                return false;

            if (!StringUtil.HasChars(guideReference))
                return false;

            return adjustName == guideReference;
        }
    }
}
