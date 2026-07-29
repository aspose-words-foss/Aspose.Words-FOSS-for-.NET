// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2012 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Provides methods to log warning in different ways.
    /// </summary>
    /// <remarks>
    /// AM. The main purpose of this class is remove often used static Warn helper methods.
    /// </remarks>
    internal static class WarningUtil
    {
        /// <summary>
        /// Logs warning into specified warning callback adapter.
        /// </summary>
        internal static void Warn(IWarningCallback warningCallback, WarningType warningType, WarningSource warningSource, string description)
        {
            if (warningCallback != null)
                warningCallback.Warning(new WarningInfo(warningType, warningSource, description));
        }

        /// <summary>
        /// Logs warning into specified warning callback adapter.
        /// </summary>
        internal static void Warn(IWarningCallback warningCallback, WarningType warningType, WarningSource warningSource, string description, params object[] args)
        {
            if (warningCallback != null)
                warningCallback.Warning(new WarningInfo(warningType, warningSource, string.Format(description, args)));
        }

        /// <summary>
        /// Logs Unexpected warning into specified warning callback adapter.
        /// </summary>
        internal static void WarnUnexpected(IWarningCallback warningCallback, WarningSource warningSource, string description, params object[] args)
        {
            Warn(warningCallback, WarningType.UnexpectedContent, warningSource, description, args);
        }

        /// <summary>
        /// Logs Unexpected warning with Unknown source into specified warning callback adapter.
        /// </summary>
        internal static void WarnUnexpected(IWarningCallback warningCallback, string description, params object[] args)
        {
            WarnUnexpected(warningCallback, WarningSource.Unknown, description, args);
        }

        /// <summary>
        /// Logs DataLoss warning into specified warning callback adapter.
        /// </summary>
        internal static void WarnDataLoss(IWarningCallback warningCallback, WarningSource warningSource, string description, params object[] args)
        {
            Warn(warningCallback, WarningType.DataLoss, warningSource, description, args);
        }

        /// <summary>
        /// Logs DataLoss warning with Unknown source into specified warning callback adapter.
        /// </summary>
        internal static void WarnDataLoss(IWarningCallback warningCallback, string description, params object[] args)
        {
            WarnDataLoss(warningCallback, WarningSource.Unknown, description, args);
        }

        /// <summary>
        /// Converts load format to appropriate warning source.
        /// </summary>
        internal static WarningSource ToLoadWarningSource(LoadFormat lf)
        {
            switch (lf)
            {
                case LoadFormat.Docm:
                case LoadFormat.Dotm:
                case LoadFormat.Dotx:
                case LoadFormat.FlatOpc:
                case LoadFormat.FlatOpcMacroEnabled:
                case LoadFormat.FlatOpcTemplate:
                case LoadFormat.FlatOpcTemplateMacroEnabled:
                case LoadFormat.Docx:
                    return WarningSource.Docx;
                case LoadFormat.Rtf:
                    return WarningSource.Rtf;
                case LoadFormat.WordML:
                    return WarningSource.WordML;
                case LoadFormat.Odt:
                    return WarningSource.Odt;
                case LoadFormat.Doc:
                    return WarningSource.Doc;
                case LoadFormat.Text:
                    return WarningSource.Text;
                case LoadFormat.Pdf:
                    return WarningSource.Pdf;
                case LoadFormat.Markdown:
                    return WarningSource.Markdown;
                case LoadFormat.Mhtml:
                case LoadFormat.Html:
                    return WarningSource.Html;
                case LoadFormat.Chm:
                    return WarningSource.Chm;
                default:
                    return WarningSource.Unknown;
            }
        }

        /// <summary>
        /// Converts save format to appropriate warning source.
        /// </summary>
        internal static WarningSource ToSaveWarningSource(SaveFormat sf)
        {
            switch (sf)
            {
                case SaveFormat.Docm:
                case SaveFormat.Dotm:
                case SaveFormat.Dotx:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                case SaveFormat.Docx:
                    return WarningSource.Docx;
                case SaveFormat.Rtf:
                    return WarningSource.Rtf;
                case SaveFormat.WordML:
                    return WarningSource.WordML;
                case SaveFormat.Odt:
                case SaveFormat.Ott:
                    return WarningSource.Odt;
                case SaveFormat.Doc:
                    return WarningSource.Doc;
                case SaveFormat.Text:
                    return WarningSource.Text;
                case SaveFormat.Pdf:
                    return WarningSource.Pdf;
                case SaveFormat.Markdown:
                    return WarningSource.Markdown;
                case SaveFormat.Mhtml:
                case SaveFormat.Html:
                    return WarningSource.Html;
                case SaveFormat.XamlFixed:
                case SaveFormat.XamlFlow:
                case SaveFormat.XamlFlowPack:
                    return WarningSource.Xaml;
                case SaveFormat.Xps:
                case SaveFormat.OpenXps:
                    return WarningSource.Xps;
                default:
                    return WarningSource.Unknown;
            }
        }
    }
}
