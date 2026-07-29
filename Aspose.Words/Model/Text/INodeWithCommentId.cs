// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2011 by Andrey Soldatov

namespace Aspose.Words
{
    /// <summary>
    /// Classes that have comment or editable range id should implement this interface.
    /// </summary>
    internal interface INodeWithAnnotationId
    {
        /// <summary>
        /// These methods have such ugly names to avoid name clashes during autoporting to Java.
        /// </summary>
        int IdInternal { get; set; }

        /// <summary>
        /// Parent comment Id. <see cref="Comment.NoParent" /> value means that comment has no parent.
        /// </summary>
        int ParentIdInternal { get; set; }
    }
}
