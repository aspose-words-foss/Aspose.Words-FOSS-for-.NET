// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2011 by Kamimura Andrey

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Specifies the mode of resource loading.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    public enum ResourceLoadingAction
    {
        /// <summary>
        /// Aspose.Words will load this resource as usual.
        /// </summary>
        Default, 

        /// <summary>
        /// Aspose.Words will skip loading of this resource.
        /// Only link without data will be stored for an image, CSS style sheet will be ignored for HTML format.
        /// </summary>
        Skip,

        /// <summary>
        /// Aspose.Words will use byte array provided by user in <see cref="ResourceLoadingArgs.SetData"/> as resource data.
        /// </summary>
        UserProvided
    }
}
