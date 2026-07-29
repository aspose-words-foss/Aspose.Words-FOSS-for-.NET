// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2016 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Performs additional actions during field replacing with result.
    /// </summary>
    internal interface IFieldReplaceListener
    {
        [JavaThrows(true)]
        void BeforeReplaceWithResult();
    }

    /// <summary>
    /// Represents listener without any side effects.
    /// </summary>
    internal class EmptyFieldReplaceListener : IFieldReplaceListener
    {
        private EmptyFieldReplaceListener()
        {
        }

        internal static readonly EmptyFieldReplaceListener Instance = new EmptyFieldReplaceListener();

        void IFieldReplaceListener.BeforeReplaceWithResult()
        {
            // Do nothing.
        }
    }
}