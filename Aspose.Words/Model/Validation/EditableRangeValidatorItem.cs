// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2013 by Andrey Noskov

namespace Aspose.Words.Validation
{
    /// <summary>
    /// A data holder. Keeps info about editable range start/end during document validation.
    /// </summary>
    internal class EditableRangeValidatorItem
    {
        /// <summary>
        /// Used to renumber editable range start/end nodes during validation.
        /// </summary>
        internal void SetNewId()
        {
            if (EditableRangeStart != null)
                EditableRangeStart.Id = NewId;
            if (EditableRangeEnd != null)
                EditableRangeEnd.Id = NewId;
        }

        internal void RemoveEditableRange()
        {
            if (EditableRangeStart != null)
            {
                if (EditableRangeStart.ParentNode != null)
                    EditableRangeStart.Remove();
                EditableRangeStart = null;
            }

            if (EditableRangeEnd != null)
            {
                if (EditableRangeEnd.ParentNode != null)
                    EditableRangeEnd.Remove();
                EditableRangeEnd = null;
            }
        }

        internal bool HasStart
        {
            get { return (EditableRangeStart != null); }
        }

        internal bool HasEnd
        {
            get { return (EditableRangeEnd != null); }
        }

        internal EditableRangeStart EditableRangeStart;
        internal EditableRangeEnd EditableRangeEnd;
        internal int NewId;
    }
}