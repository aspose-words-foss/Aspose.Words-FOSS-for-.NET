// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2011 by Alexey Titov

using System.Runtime.InteropServices;

namespace Aspose.Warnings
{
    /// <summary>
    /// Used to warn users.
    /// </summary>
    [ComVisible(false)]
    public interface IWarningCallbackCore
    {
        /// <summary>
        /// Warns user
        /// </summary>
        /// <param name="warningType">Type of the warning.</param>
        /// <param name="source">The module that generates the warning.</param>
        /// <param name="description">Short description of the warning.</param>
        void Warn(WarningTypeCore warningType, WarningSourceCore source, string description);

        /// <summary>
        /// Warns user
        /// </summary>
        /// <param name="warningType">Type of the warning.</param>
        /// <param name="source">The module that generates the warning.</param>
        /// <param name="format">Format string of short description of the warning.</param>
        /// <param name="formatArgument">Format argument used in format string.</param>
        void Warn(WarningTypeCore warningType, WarningSourceCore source, string format, object formatArgument);

        /// <summary>
        /// Warns user
        /// </summary>
        /// <param name="warningType">Type of the warning.</param>
        /// <param name="source">The module that generates the warning.</param>
        /// <param name="format">Format string of short description of the warning.</param>
        /// <param name="arg0">Format argument used in format string.</param>
        /// <param name="arg1">Format argument used in format string.</param>
        void Warn(WarningTypeCore warningType, WarningSourceCore source, string format, object arg0, object arg1);
    }
}
