// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/10/2016 by Nikolay Sezganov

using Aspose.Words.Loading;

namespace Aspose.Words.RW.HtmlCommon
{
    /// <summary>
    /// Stores information about resource loading result.
    /// </summary>
    internal class ResourceLoadResult
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ResourceLoadResult(
            byte[] resourceBytes,
            ResourceLoadingAction action)
        {
            mResourceBytes = resourceBytes;
            mLoadingAction = action;
        }

        /// <summary>
        /// Stores loaded resource bytes. 
        /// Can be null in case if resource loading was skipped by user, it was unknown resource type 
        /// or in case exception occurred during loading.
        /// </summary>
        internal byte[] ResourceBytes
        {
            get { return mResourceBytes; }
        }

        /// <summary>
        /// Indicates action that was used by user.
        /// </summary>
        internal ResourceLoadingAction LoadingAction
        {
            get { return mLoadingAction; }
        }

        private readonly byte[] mResourceBytes;
        private readonly ResourceLoadingAction mLoadingAction;
    }
}