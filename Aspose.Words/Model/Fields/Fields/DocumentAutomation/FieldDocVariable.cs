// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2005 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements DOCVARIABLE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldDocVariable : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            if (!StringUtil.HasChars(VariableName))
                return new FieldUpdateActionInsertErrorMessage(this, "Error! Document Variable not defined.");

            Document doc = FetchDocument();
            string result = doc.Variables[VariableName];

            if (string.IsNullOrEmpty(result))
            {
                // WORDSNET-18909 Insert error message if variable is empty or not found
                // and there is any not empty variable.
                if (IsAnyNotEmpty(doc.Variables))
                    return new FieldUpdateActionInsertErrorMessage(this, "Error! No document variable supplied.");

                result = string.Empty;
            }

            return new FieldUpdateActionApplyResult(this, result);
        }

        private static bool IsAnyNotEmpty(VariableCollection variables)
        {
            if (variables.Count == 0)
                return false;

            foreach (KeyValuePair<string, string> variable in variables)
            {
                if (!string.IsNullOrEmpty(variable.Value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the name of the document variable to retrieve.
        /// </summary>
        public string VariableName
        {
            get { return FieldCodeCache.GetArgumentAsString(VariableNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(VariableNameArgumentIndex, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int VariableNameArgumentIndex = 0;
    }
}
