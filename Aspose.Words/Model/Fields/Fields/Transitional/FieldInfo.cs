// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the INFO field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts information about a document property.
    /// </remarks>
    public class FieldInfo : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            IFieldInfoResultProvider provider = GetProvider();
            if (provider == null)
                return new FieldUpdateActionInsertErrorMessage(this, UnknownInfoTypeError);

            FieldCodeDecorator fieldCode = new FieldCodeDecorator(FieldCodeCache);

            Constant result = provider.GetResult(FetchDocument(), fieldCode);

            return new FieldUpdateActionApplyResult(this, result);
        }

        private IFieldInfoResultProvider GetProvider()
        {
            string infoType = InfoType;

            if (string.IsNullOrEmpty(infoType))
                return null;

            FieldType infoFieldType = FieldUtil.GetFieldType(infoType);

            switch (infoFieldType)
            {
                case FieldType.FieldAuthor: return FieldAuthor.FieldInfoResultProvider;
                case FieldType.FieldComments: return FieldComments.FieldInfoResultProvider;
                case FieldType.FieldCreateDate: return FieldCreateDate.FieldInfoResultProvider;
                case FieldType.FieldEditTime: return FieldEditTime.FieldInfoResultProvider;
                case FieldType.FieldFileName: return FieldFileName.FieldInfoResultProvider;
                case FieldType.FieldFileSize: return FieldFileSize.FieldInfoResultProvider;
                case FieldType.FieldKeyword: return FieldKeywords.FieldInfoResultProvider;
                case FieldType.FieldLastSavedBy: return FieldLastSavedBy.FieldInfoResultProvider;
                case FieldType.FieldNumChars: return FieldNumChars.FieldInfoResultProvider;
                case FieldType.FieldNumWords: return FieldNumWords.FieldInfoResultProvider;
                case FieldType.FieldPrintDate: return FieldPrintDate.FieldInfoResultProvider;
                case FieldType.FieldRevisionNum: return FieldRevNum.FieldInfoResultProvider;
                case FieldType.FieldSaveDate: return FieldSaveDate.FieldInfoResultProvider;
                case FieldType.FieldSubject: return FieldSubject.FieldInfoResultProvider;
                case FieldType.FieldTemplate: return FieldTemplate.FieldInfoResultProvider;
                case FieldType.FieldTitle: return FieldTitle.FieldInfoResultProvider;
                case FieldType.FieldNumPages: return FieldNumPages.FieldInfoResultProvider;
                default: return null;
            }
        }

        /// <summary>
        /// Gets or sets the type of the document property to insert.
        /// </summary>
        public string InfoType
        {
            get { return FieldCodeCache.GetArgumentAsString(InfoTypeArgumentIndex); }
            set { FieldCodeCache.SetArgument(InfoTypeArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets an optional value that updates the property.
        /// </summary>
        public string NewValue
        {
            get { return FieldCodeCache.GetArgumentAsString(NewValueArgumentIndex); }
            set { FieldCodeCache.SetArgument(NewValueArgumentIndex, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            // The INFO field ignores CREATEDATE, PRINTDATE and SAVEDATE switches.
            FieldSwitchType result = FieldFileSize.GetSwitchTypeInternal(switchName);
            if (result != FieldSwitchType.Unknown)
                return result;

            result = FieldFileName.GetSwitchTypeInternal(switchName);
            if (result != FieldSwitchType.Unknown)
                return result;

            return FieldTemplate.GetSwitchTypeInternal(switchName);
        }

        private const int InfoTypeArgumentIndex = 0;
        private const int NewValueArgumentIndex = 1;

        private const string UnknownInfoTypeError = "Error! Unknown info keyword.";

        private class FieldCodeDecorator : IFieldCode
        {
            internal FieldCodeDecorator(FieldCode fieldCode)
            {
                mFieldCode = fieldCode;
            }

            string IFieldCode.GetArgumentAsString(int authorNameArgumentIndex)
            {
                return mFieldCode.GetArgumentAsString(authorNameArgumentIndex + 1);
            }

            bool IFieldCode.HasSwitch(string switchName)
            {
                return mFieldCode.HasSwitch(switchName);
            }

            private readonly FieldCode mFieldCode;
        }
    }
}
