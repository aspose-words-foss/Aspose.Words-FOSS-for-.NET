// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2011 by Kamimura Andrey

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Provides data for the <see cref="IResourceLoadingCallback.ResourceLoading"/> method.
    /// </summary>
    public class ResourceLoadingArgs
    {
        /// <summary>
        /// Internal ctor.
        /// </summary>
        /// <param name="absoluteUri">Absolute URI of the resource. Aspose.Words uses it to download the resource by default.</param>
        /// <param name="originalUri">Original URI of the resource as specified in imported document.</param>
        /// <param name="resourceType">Type of resource.</param>
        internal ResourceLoadingArgs(string absoluteUri, string originalUri, ResourceType resourceType)
        {
            OriginalUri = originalUri;
            ResourceType = resourceType;
            Uri = absoluteUri;
        }

        /// <summary>
        /// Type of resource.
        /// </summary>
        public ResourceType ResourceType { get; }

        /// <summary>
        /// <para>URI of the resource which is used for downloading
        /// if <see cref="IResourceLoadingCallback.ResourceLoading"/>
        /// returns <see cref="ResourceLoadingAction.Default"/>.</para>
        /// <para>Initially it's set to absolute URI of the resource,
        /// but user can redefine it to any value.</para>
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Original URI of the resource as specified in imported document.
        /// </summary>
        public string OriginalUri { get; }

        /// <summary>
        /// Sets user provided data of the resource which is used
        /// if <see cref="IResourceLoadingCallback.ResourceLoading"/>
        /// returns <see cref="ResourceLoadingAction.UserProvided"/>.
        /// </summary>
        public void SetData(byte[] data)
        {
            mData = data;
        }

        /// <summary>
        /// Returns user provided data of the resource.
        /// </summary>
        internal byte[] GetData()
        {
            return mData;
        }

        /// <summary>
        /// Returns <c>true</c> if data is empty.
        /// </summary>
        internal bool IsDataEmpty
        {
            get { return (mData == null) || (mData.Length == 0); }
        }

        private byte[] mData;
    }
}
