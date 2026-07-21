// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Used to validate task pane add-ins.
    /// </summary>
    internal class WebExtensionValidator
    {
        /// <summary>
        /// Validates office task pane add-ins.
        /// </summary>
        internal static void VisitDocumentStart(DocumentBase doc, SaveInfo saveInfo, IWarningCallback warningCallback)
        {
            Document document = doc as Document;
            if ((document == null) || (document.WebExtensionTaskPanes.Count == 0))
                return;

            // Task pane add-ins will be missed in the output for non-OOXML format. Note about it.
            if (!saveInfo.IsOoxmlFormat)
                Warn(warningCallback, WarningType.DataLossCategory, WarningStrings.TaskPaneAddinsWillBeSkippedOnSaving);
        }

        /// <summary>
        /// Logs warning into specified warning callback adapter.
        /// </summary>
        private static void Warn(IWarningCallback warnCallback, WarningType type, string desc)
        {
            WarningUtil.Warn(warnCallback, type, WarningSource.Validator, desc);
        }
    }
}
