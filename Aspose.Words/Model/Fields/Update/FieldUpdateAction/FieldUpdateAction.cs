// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2009 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies what to do with the field after it is updated.
    /// </summary>
    internal abstract class FieldUpdateAction
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="field"></param>
        protected FieldUpdateAction(Field field)
        {
            Field = field;
        }

        /// <summary>
        /// Performs the update action (sets field result, removes field etc).
        /// </summary>
        [JavaThrows(true)]
        internal abstract void Perform();

        protected Document Document
        {
            get { return Field.FetchDocument(); }
        }

        protected Field Field { get; }
    }
}
