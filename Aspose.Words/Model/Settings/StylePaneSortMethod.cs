// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/03/2010 by Denis Darkin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// This element specifies a suggested sorting which should be applied to the list of document styles in this
    /// application if the styles are displayed in a user interface.
    /// </summary>
    internal enum StylePaneSortMethod
    {
        /// <summary>
        /// Specifies that styles which are visible should be sorted by their names.
        /// </summary>
        Name,

        /// <summary>
        /// Specifies that styles which are visible should be sorted by their UI priority 
        /// using the uiPriority element. (iso29500, part1, §17.7.4.19).
        /// </summary>
        Priority,
        
        /// <summary>
        /// Specifies that styles which are visible should be sorted by the default sorting of the host application.
        /// </summary>
        Default,
        
        /// <summary>
        /// Specifies that styles which are visible should be sorted by the font which they apply.
        /// </summary>
        Font,
        
        /// <summary>
        /// Specifies that styles which are visible should be sorted by the style on which they
        /// are based using the basedOn element (iso29500, part1, §17.7.4.3).
        /// </summary>
        BasedOn,
        
        /// <summary>
        /// Specifies that styles which are visible should be sorted by their style types 
        /// (i.e. character, linked, paragraph).
        /// </summary>
        StyleType,
    }
}
