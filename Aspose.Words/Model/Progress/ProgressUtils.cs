// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2021 by Dmitry Sokolov

using Aspose.Words.Loading;
using Aspose.Words.Saving;

namespace Aspose.Words.Progress
{
    internal static class ProgressUtils
    {
        // This method may be removed when all formats will be supported.
        internal static bool IsLoadingProgressSupported(LoadFormat lf)
        {
            switch (lf)
            {
                case LoadFormat.Docx:
                case LoadFormat.Docm:
                case LoadFormat.Dotm:
                case LoadFormat.Dotx:
                case LoadFormat.FlatOpc:
                case LoadFormat.FlatOpcMacroEnabled:
                case LoadFormat.FlatOpcTemplate:
                case LoadFormat.FlatOpcTemplateMacroEnabled:
                case LoadFormat.Markdown:
                case LoadFormat.Text:
                case LoadFormat.WordML:
                case LoadFormat.Rtf:
                case LoadFormat.Doc:
                case LoadFormat.Dot:
                case LoadFormat.Odt:
                case LoadFormat.Ott:
                case LoadFormat.Pdf:
                    return true;
                default:
                    return false;
            }
        }

        // This method may be removed when all formats will be supported.
        internal static bool IsSavingProgressSupported(SaveFormat sf)
        {
            switch (sf)
            {
                case SaveFormat.Docx:
                case SaveFormat.Docm:
                case SaveFormat.Dotm:
                case SaveFormat.Dotx:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    return true;
                case SaveFormat.Doc:
                case SaveFormat.Dot:
                    return true;
                case SaveFormat.Html:
                case SaveFormat.Mhtml:
                case SaveFormat.Epub:
                case SaveFormat.Azw3:
                case SaveFormat.Mobi:
                case SaveFormat.XamlFlow:
                case SaveFormat.XamlFlowPack:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Conditionally warns that progress notification is not supported yet.
        /// </summary>
        internal static void WarnWhenProgressUnsupported(
            LoadFormat loadFormat,
            IWarningCallback warningCallback,
            IDocumentLoadingCallback loadingProgressCallback)
        {
            if (IsLoadingProgressSupported(loadFormat) || (loadingProgressCallback == null))
                return;

            // Notify a client about progress feature limitations. Once per attempt of document loading.
            WarningUtil.Warn(
                    warningCallback,
                    WarningType.Hint,
                    WarningUtil.ToLoadWarningSource(loadFormat),
                    string.Format(WarningStrings.LoadingProgressUnsupported, loadFormat));
        }

        /// <summary>
        /// Conditionally warns that progress notification is not supported yet.
        /// </summary>
        internal static void WarnWhenProgressUnsupported(
            SaveFormat saveFormat,
            IWarningCallback warningCallback,
            IDocumentSavingCallback savingProgressCallback)
        {
            if (IsSavingProgressSupported(saveFormat) || (savingProgressCallback == null))
                return;

            // Notify a client about progress feature limitations. Once per attempt of document saving.
            WarningUtil.Warn(
                    warningCallback,
                    WarningType.Hint,
                    WarningUtil.ToSaveWarningSource(saveFormat),
                    string.Format(WarningStrings.SavingProgressUnsupported, saveFormat));
        }
    }
}
