// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2016 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Performs additional actions necessary for unlinking fields.
    /// </summary>
    internal class UnlinkFieldReplaceListener : IFieldReplaceListener
    {
        internal UnlinkFieldReplaceListener(Field field)
        {
            mField = field;
        }

        void IFieldReplaceListener.BeforeReplaceWithResult()
        {
            mField.BeforeUnlink();
        }

        private readonly Field mField;
    }
}