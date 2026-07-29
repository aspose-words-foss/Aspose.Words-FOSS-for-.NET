// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/10/2011 by Alexey Titov

using System;
using Aspose.Warnings;

namespace Aspose.Words
{
    /// <summary>
    /// Adapts IWarningCallbackCore interface to IWarningCallback interface.
    /// </summary>
    internal class WarningCallbackCoreAdapter : IWarningCallbackCore
    {
        internal WarningCallbackCoreAdapter(IWarningCallback warningCallback)
        {
            mWarningCallback = warningCallback;
        }

        void IWarningCallbackCore.Warn(WarningTypeCore warningType, WarningSourceCore source, string description)
        {
            if (mWarningCallback == null)
                return;

            WarningType type = WarningInfo.ConvertWarningTypeCoreToWarningType(warningType);
            WarningSource src = WarningInfo.ConvertWarningSourceCoreToWarningSource(source);
            mWarningCallback.Warning(new WarningInfo(type, src, description));
        }

        void IWarningCallbackCore.Warn(WarningTypeCore warningType, WarningSourceCore source, string format, object formatArgument)
        {
            if (mWarningCallback == null)
                return;

            string description = String.Format(format, formatArgument);
            ((IWarningCallbackCore)this).Warn(warningType, source, description);
        }

        public void Warn(WarningTypeCore warningType, WarningSourceCore source, string format, object arg0, object arg1)
        {
            if (mWarningCallback == null)
                return;

            string description = String.Format(format, arg0, arg1);
            ((IWarningCallbackCore)this).Warn(warningType, source, description);
        }

        private readonly IWarningCallback mWarningCallback;
    }
}
