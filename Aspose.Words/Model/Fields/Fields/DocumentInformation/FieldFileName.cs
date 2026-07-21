// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2011 by Dmitry Vorobyev

using System.IO;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the FILENAME field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Retrieves the name of the current document from its storage location.</p>
    /// <p>In the current implementation, uses the <see cref="Document.OriginalFileName"/> property to retrieve
    /// the file name. If the document was loaded from a stream or created blank, uses the name of the file that is being saved to (if known).</p>
    /// </remarks>
    public class FieldFileName : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Document document = FetchDocument();

            string fileName = GetResultInternal(document, FieldCodeCache);

            return new FieldUpdateActionApplyResult(this, fileName);
        }

        private static string GetResultInternal(Document document, IFieldCode fieldCode)
        {
            string fileName = document.FieldOptions.FileName;
            if (!StringUtil.HasChars(fileName))
                fileName = document.OriginalFileName;
            if (!StringUtil.HasChars(fileName))
                fileName = document.SavedFileName;

            if (StringUtil.HasChars(fileName) && !fieldCode.HasSwitch(IncludeFullPathSwitch))
                fileName = Path.GetFileName(fileName);

            return fileName;
        }

        /// <summary>
        /// Gets or sets whether to include the full file path name.
        /// </summary>
        public bool IncludeFullPath
        {
            get { return FieldCodeCache.HasSwitch(IncludeFullPathSwitch); }
            set { FieldCodeCache.SetSwitch(IncludeFullPathSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            return GetSwitchTypeInternal(switchName);
        }

        internal static FieldSwitchType GetSwitchTypeInternal(string switchName)
        {
            switch (switchName)
            {
                case IncludeFullPathSwitch:
                    return FieldSwitchType.Flag;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const string IncludeFullPathSwitch = "\\p";

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new FileNameInfoResultProvider();

        private class FileNameInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return new StringConstant(GetResultInternal(document, fieldCode));
            }
        }
    }
}
