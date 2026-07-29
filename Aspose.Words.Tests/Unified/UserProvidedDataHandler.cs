// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using Aspose.Words.Loading;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    public class UserProvidedDataHandler : IResourceLoadingCallback
    {
        public ResourceLoadingAction ResourceLoading(ResourceLoadingArgs args)
        {
            if (args.ResourceType == ResourceType.Image)
            {
                Assert.That(args.OriginalUri, IsNot.Empty());
                    
                // Set test data.
                args.SetData(new byte[5]);
                return ResourceLoadingAction.UserProvided;
            }

            return ResourceLoadingAction.Default;
        }
    }
}
