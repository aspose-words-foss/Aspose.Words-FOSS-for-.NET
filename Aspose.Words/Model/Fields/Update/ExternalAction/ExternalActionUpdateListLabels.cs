// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/12/2010 by Dmitry Vorobyev

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Updates list labels in the document.
    /// </summary>
    internal class ExternalActionUpdateListLabels : IExternalAction
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="document"></param>
        internal ExternalActionUpdateListLabels(Document document)
        {
            mDocument = document;
        }

        void IExternalAction.Perform()
        {
            if (mPerformed)
                return;

            mDocument.UpdateListLabels();
            mPerformed = true;
        }

        Type IExternalAction.Id
        {
            get { return GetType(); }
        }

        private bool mPerformed;

        private readonly Document mDocument;
    }
}
