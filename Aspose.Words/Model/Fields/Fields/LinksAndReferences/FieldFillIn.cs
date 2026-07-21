// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2012 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the FILLIN field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Prompts the user to enter text.
    /// </remarks>
    public class FieldFillIn : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return FieldUserPromptingUtil.GetFieldUpdateAction(this);
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            return FieldUserPromptingUtil.GetSwitchType(switchName);
        }

        /// <summary>
        /// Gets or sets the prompt text (the title of the prompt window).
        /// </summary>
        public string PromptText
        {
            get { return FieldUserPromptingUtil.GetPromptText(this); }
            set { FieldUserPromptingUtil.SetPromptText(this, value); }
        }

        /// <summary>
        /// Gets or sets whether the user response should be recieved once per a mail merge operation.
        /// </summary>
        public bool PromptOnceOnMailMerge
        {
            get { return FieldUserPromptingUtil.GetPromptOnceOnMailMerge(this); }
            set { FieldUserPromptingUtil.SetPromptOnceOnMailMerge(this, value); }
        }

        /// <summary>
        /// Gets or sets default user response (initial value contained in the prompt window).
        /// </summary>
        public string DefaultResponse
        {
            get { return FieldUserPromptingUtil.GetDefaultResponse(this); }
            set { FieldUserPromptingUtil.SetDefaultResponse(this, value); }
        }
    }
}
