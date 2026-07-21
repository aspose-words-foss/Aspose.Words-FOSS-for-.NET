// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2023 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides  auto-updatable fields utility functions.
    /// </summary>
    internal static class FieldsAutoUpdateUtils
    {
        internal static bool IsFieldUpdatedBeforeSave(FieldUpdateContext context)
        {
            if (IsFieldUpdatedBeforeSave(context.Field))
                return true;

            // For now considers only fields in main story.
            if (context.Field.IsInHeaderFooter)
                return false;

            EnsureUpdateBeforeSaveState(context.TopmostParentContextOrSelf);

            return context.IsAutoUpdatableBeforeSave == NullableBool.True;
        }

        private static bool IsFieldUpdatedBeforeSave(Field field)
        {
            return field.IsInHeaderFooter
                ? IsUpdatedBeforeSaveInHeader(field.Type)
                : IsUpdatedBeforeSaveInBody(field);
        }

        private static void EnsureUpdateBeforeSaveState(FieldUpdateContext context)
        {
            Debug.Assert(!context.HasParentContext);

            if (context.IsAutoUpdatableBeforeSave != NullableBool.NotDefined)
                return;

            // 0. Marks field and all its descendants as not auto-updatable.
            DisableAutoUpdate(context);

            // 1. Marks certain field types (like REF) as auto-updatable as well as all theirs ancestors.
            List<FieldUpdateContext> autoUpdatedFields = ExtractAutoUpdatedFields(context);
            if (autoUpdatedFields.Count == 0)
                return;

            foreach (FieldUpdateContext field in autoUpdatedFields)
                EnableAutoUpdate(field);

            // 2. Marks some nested IF fields in topmost IF field.
            EnableIfFieldsAutoUpdate(context);
        }

        private static List<FieldUpdateContext> ExtractAutoUpdatedFields(FieldUpdateContext context)
        {
            List<FieldUpdateContext> result = new List<FieldUpdateContext>();

            foreach (FieldUpdateContext child in context)
            {
                result.AddRange(ExtractAutoUpdatedFields(child));

                if (IsFieldUpdatedBeforeSave(child.Field))
                    result.Add(child);
            }

            return result;
        }

        private static void EnableIfFieldsAutoUpdate(FieldUpdateContext context)
        {
            if (context.Field.Type != FieldType.FieldIf)
                return;

            // MS Word updates some nested IF fields (even if without auto-updatable child fields) in topmost IF field..
            // There are two kinds of nested IF fields:
            // - without own children fields in true and false result arguments (lets call them as "simple")
            // - with own children fields in true or false result arguments (lets call them as "complex")
            // MS Words stops to update nested IF fields at first "complex" field if it has any "simple" leading siblings.
            bool isSimpleChildFound = false;
            foreach (FieldUpdateContext child in context)
            {
                if (child.Field.Type != FieldType.FieldIf)
                    continue;

                bool isSimpleChild = IsSimpleIfField(child);
                if (isSimpleChildFound && !isSimpleChild)
                    break;

                if (isSimpleChild)
                    isSimpleChildFound = true;

                child.IsAutoUpdatableBeforeSave = NullableBool.True;
            }
        }

        private static bool IsSimpleIfField(FieldUpdateContext context)
        {
            Debug.Assert(context.Field.Type == FieldType.FieldIf);

            FieldIf fieldIf = (FieldIf)context.Field;
            FieldArgument trueResultArgument = fieldIf.TrueResultArgument;
            FieldArgument falseResultArgument = fieldIf.FalseResultArgument;

            foreach (FieldUpdateContext child in context)
            {
                if (child.IsAutoUpdatableBeforeSave == NullableBool.True)
                    continue;

                if (child.ParentArgument == trueResultArgument)
                    return false;

                if (child.ParentArgument == falseResultArgument)
                    return false;
            }

            return true;
        }

        private static void DisableAutoUpdate(FieldUpdateContext context)
        {
            context.IsAutoUpdatableBeforeSave = NullableBool.False;
            foreach (FieldUpdateContext child in context)
                DisableAutoUpdate(child);
        }

        private static void EnableAutoUpdate(FieldUpdateContext context)
        {
            context.IsAutoUpdatableBeforeSave =  NullableBool.True;
            if (context.HasParentContext)
                EnableAutoUpdate(context.ParentContext);
        }

        private static bool IsUpdatedBeforeSaveInHeader(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldAsk:
                case FieldType.FieldBarcode:
                case FieldType.FieldData:
                case FieldType.FieldDate:
                case FieldType.FieldDDEAuto:
                case FieldType.FieldDisplayBarcode:
                case FieldType.FieldMergeBarcode:
                case FieldType.FieldEquation:
                case FieldType.FieldFormTextInput:
                case FieldType.FieldFormula:
                case FieldType.FieldNoteRef:
                case FieldType.FieldNumPages:
                case FieldType.FieldPage:
                case FieldType.FieldPageRef:
                case FieldType.FieldSaveDate:
                case FieldType.FieldSection:
                case FieldType.FieldSectionPages:
                case FieldType.FieldSequence:
                case FieldType.FieldSet:
                case FieldType.FieldStyleRef:
                case FieldType.FieldTime:
                case FieldType.FieldIf:
                case FieldType.FieldAddin:
                case FieldType.FieldAuthor:
                case FieldType.FieldAutoText:
                case FieldType.FieldComments:
                case FieldType.FieldCompare:
                case FieldType.FieldCreateDate:
                case FieldType.FieldDocProperty:
                case FieldType.FieldDocVariable:
                case FieldType.FieldEditTime:
                case FieldType.FieldFileName:
                case FieldType.FieldFillIn:
                case FieldType.FieldGlossary:
                case FieldType.FieldGreetingLine:
                case FieldType.FieldInfo:
                case FieldType.FieldKeyword:
                case FieldType.FieldLastSavedBy:
                case FieldType.FieldNumChars:
                case FieldType.FieldNumWords:
                case FieldType.FieldPrintDate:
                case FieldType.FieldQuote:
                case FieldType.FieldRef:
                case FieldType.FieldRevisionNum:
                case FieldType.FieldShape:
                case FieldType.FieldSubject:
                case FieldType.FieldTemplate:
                case FieldType.FieldTitle:
                case FieldType.FieldUserAddress:
                case FieldType.FieldUserInitials:
                case FieldType.FieldUserName:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsUpdatedBeforeSaveInBody(Field field)
        {
            switch (field.Type)
            {
                case FieldType.FieldAsk:
                case FieldType.FieldBarcode:
                case FieldType.FieldData:
                case FieldType.FieldDate:
                case FieldType.FieldDDEAuto:
                case FieldType.FieldDisplayBarcode:
                case FieldType.FieldMergeBarcode:
                case FieldType.FieldEquation:
                case FieldType.FieldSaveDate:
                case FieldType.FieldSection:
                case FieldType.FieldSet:
                case FieldType.FieldTime:
                case FieldType.FieldNoteRef:
                case FieldType.FieldNumPages:
                case FieldType.FieldPage:
                case FieldType.FieldPageRef:
                case FieldType.FieldPrintDate:
                case FieldType.FieldRef:
                case FieldType.FieldSectionPages:
                case FieldType.FieldSequence:
                case FieldType.FieldStyleRef:
                    return true;
                case FieldType.FieldFormTextInput:
                    FormField formField = field.Start.FormField;
                    if (formField == null)
                        return false;

                    return IsUpdatedBeforeSaveInBody(formField.TextInputType);
                default:
                    return false;
            }
        }

        private static bool IsUpdatedBeforeSaveInBody(TextFormFieldType textInputType)
        {
            switch (textInputType)
            {
                case TextFormFieldType.CurrentDate:
                case TextFormFieldType.CurrentTime:
                    return true;
                default:
                    return false;
            }
        }
    }
}
