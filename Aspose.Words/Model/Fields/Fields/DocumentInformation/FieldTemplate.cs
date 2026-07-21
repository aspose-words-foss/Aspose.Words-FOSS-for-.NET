// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2011 by Dmitry Vorobyev

using System.IO;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the TEMPLATE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the file name of the template used by the current document.
    /// </remarks>
    public class FieldTemplate : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Document document = FetchDocument();

            string templateName = GetResultInternal(document, FieldCodeCache);

            return new FieldUpdateActionApplyResult(this, templateName);
        }

        private static string GetResultInternal(Document document, IFieldCode fieldCode)
        {
            const string defaultTemplate = "Normal.dotm";

            string templateName = document.AttachedTemplate;

            if (string.IsNullOrEmpty(templateName))
                templateName = document.FieldOptions.TemplateName;

            if (string.IsNullOrEmpty(templateName))
                templateName = defaultTemplate;

            if (!fieldCode.HasSwitch(IncludeFullPathSwitch))
                templateName = Path.GetFileName(templateName);

            return templateName;
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

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new TemplateInfoResultProvider();

        private class TemplateInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return new StringConstant(GetResultInternal(document, fieldCode));
            }
        }
    }
}
