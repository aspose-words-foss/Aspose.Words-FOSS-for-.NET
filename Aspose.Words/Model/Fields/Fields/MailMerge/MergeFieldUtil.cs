// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2013 by Ivan Lyagin

using System;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides a set of utility methods shared between fields of different types participating in a mail merge.
    /// </summary>
    internal static class MergeFieldUtil
    {
        /// <summary>
        /// Returns <see cref="FieldUpdateAction"/> setting the specified field's result to the specified static text value,
        /// </summary>
        internal static FieldUpdateAction ProcessUnusedField(Field field, string staticText)
        {
            return new FieldUpdateActionApplyResult(field, staticText);
        }

        /// <summary>
        /// Updates NEXT, NEXTIF and SKIPIF fields.
        /// </summary>
        internal static FieldUpdateAction UpdateRuleField(Field field, string staticText)
        {
            Debug.Assert(field.Type == FieldType.FieldNext || field.Type == FieldType.FieldNextIf || field.Type == FieldType.FieldSkipIf);

            Constant value = field.Updater.DataProviders.GetValue(field);
            if (value == null)
                return ProcessUnusedField(field, staticText);

            if (value.IsError)
            {
                // I'm not sure if this is correct inserting an error message here, but it seems like
                // we have no another option.
                return new FieldUpdateActionInsertErrorMessage(field, value.ValueString);
            }

            return null;
        }
    }
}
