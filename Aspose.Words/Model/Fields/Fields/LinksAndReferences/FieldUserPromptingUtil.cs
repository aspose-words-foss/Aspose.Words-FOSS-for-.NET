// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2012 by Ivan Lyagin

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents utility methods to work with the fields which prompt the user to respond on update
    /// such as ASK and FILLIN.
    /// </summary>
    internal static class FieldUserPromptingUtil
    {
        /// <summary>
        /// Returns the field update action for the given field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static FieldUpdateAction GetFieldUpdateAction(Field field)
        {
            Document document = field.FetchDocument();
            IFieldUserPromptRespondent respondent = document.FieldOptions.UserPromptRespondent;
            if (respondent == null)
            {
                // If respondent is not specified, do not update the field for the backward compatibility.
                return new FieldUpdateActionDoNothing(field);
            }

            // Prompt the user to respond.
            string response = respondent.Respond(GetPromptText(field), GetDefaultResponse(field));
            return GetFieldUpdateAction(field, response);
        }

        private static FieldUpdateAction GetFieldUpdateAction(Field field, string response)
        {
            // MS Word does not update the field in case when user skips the prompt
            // (i.e. presses "Cancel" button in prompt window).
            if (response == null)
                return new FieldUpdateActionDoNothing(field);

            switch (field.Type)
            {
                case FieldType.FieldAsk:
                    return new FieldUpdateActionSetBookmarkValue(field, ((FieldAsk)field).BookmarkName, response);
                case FieldType.FieldFillIn:
                    return new FieldUpdateActionApplyResult(field, response);
                default:
                    throw CreateWrongFieldTypeException();
            }
        }

        private static InvalidOperationException CreateWrongFieldTypeException()
        {
            return new InvalidOperationException("Wrong field type for this operation.");
        }

        /// <summary>
        /// Returns the promt text (i.e. the title of the prompt window) for the given field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static string GetPromptText(Field field)
        {
            return field.FieldCodeCache.GetArgumentAsString(GetPromptTextArgumentIndex(field));
        }

        internal static void SetPromptText(Field field, string text)
        {
            field.FieldCodeCache.SetArgument(GetPromptTextArgumentIndex(field), text);
        }

        private static int GetPromptTextArgumentIndex(Field field)
        {
            switch (field.Type)
            {
                case FieldType.FieldAsk:
                    return 1;
                case FieldType.FieldFillIn:
                    return 0;
                default:
                    throw CreateWrongFieldTypeException();
            }
        }

        /// <summary>
        /// Returns the value, indicating whether the user response should be recieved once per a mail merge operation
        /// for the given field.
        /// </summary>
        internal static bool GetPromptOnceOnMailMerge(Field field)
        {
            return field.FieldCodeCache.HasSwitch(PromptOnceOnMailMergeSwitch);
        }

        internal static void SetPromptOnceOnMailMerge(Field field, bool value)
        {
            field.FieldCodeCache.SetSwitch(PromptOnceOnMailMergeSwitch, value);
        }

        /// <summary>
        /// Returns default user response (i.e. initial value contained in the prompt window) for the given field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static string GetDefaultResponse(Field field)
        {
            return field.FieldCodeCache.GetSwitchArgumentAsString(DefaultResponseSwitch);
        }

        internal static void SetDefaultResponse(Field field, string value)
        {
            field.FieldCodeCache.SetSwitch(DefaultResponseSwitch, value);
        }

        /// <summary>
        /// Gets the switch type by the switch name in the context of the fields which prompt the user to respond on update.
        /// </summary>
        /// <param name="switchName"></param>
        /// <returns></returns>
        internal static FieldSwitchType GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case PromptOnceOnMailMergeSwitch:
                    return FieldSwitchType.Flag;
                case DefaultResponseSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const string PromptOnceOnMailMergeSwitch = "\\o";
        private const string DefaultResponseSwitch = "\\d";
    }
}
