// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Enumerates available types of a web extension store.
    /// </summary>
    public enum WebExtensionStoreType
    {
        /// <summary>
        /// Specifies that the store type is SharePoint corporate catalog.
        /// </summary>
        SPCatalog,
        /// <summary>
        /// Specifies that the store type is Office.com.
        /// </summary>
        OMEX,
        /// <summary>
        /// Specifies that the store type is a SharePoint web application.
        /// </summary>
        SPApp,
        /// <summary>
        /// Specifies that the store type is an Exchange server.
        /// </summary>
        Exchange,
        /// <summary>
        /// Specifies that the store type is a file system share.
        /// </summary>
        FileSystem,
        /// <summary>
        /// Specifies that the store type is the system registry.
        /// </summary>
        Registry,
        /// <summary>
        /// Specifies that the store type is Centralized Deployment via Exchange.
        /// </summary>
        ExCatalog,
        /// <summary>
        /// Default value.
        /// </summary>
        Default = SPCatalog // Default value according to spec (MS-OWEXML).
    }
}
