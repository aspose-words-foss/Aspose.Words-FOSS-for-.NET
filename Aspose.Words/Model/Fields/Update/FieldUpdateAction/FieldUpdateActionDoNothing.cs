// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Used by fields that update themselves in UpdateCore.
    /// </summary>
    internal class FieldUpdateActionDoNothing : FieldUpdateAction
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionDoNothing(Field field)
            : base(field)
        {
        }

        internal override void Perform()
        {
            // Do nothing.
        }
    }
}
