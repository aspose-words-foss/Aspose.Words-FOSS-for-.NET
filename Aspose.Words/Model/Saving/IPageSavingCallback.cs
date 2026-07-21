// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2016 by Andrey Noskov

using Aspose.JavaAttributes;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Implement this interface if you want to control how Aspose.Words saves separate pages when 
    /// saving a document to fixed page formats.
    /// </summary>
    public interface IPageSavingCallback
    {
        /// <summary>
        /// Called when Aspose.Words saves a separate page to fixed page formats.
        /// </summary>
        [JavaThrows(true)]
        void PageSaving(PageSavingArgs args);
    }
}
