// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2012 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents the respondent to user prompts during field update.
    /// </summary>
    /// <remarks>
    /// The ASK and FILLIN fields are the examples of fields that prompt the user for some response. Implement this interface
    /// and assign it to the <see cref="FieldOptions.UserPromptRespondent"/> property to establish interaction between field update
    /// and the user.
    /// </remarks>
    public interface IFieldUserPromptRespondent
    {
        /// <summary>
        /// When implemented, returns a response from the user on prompting.
        /// Your implementation should return <c>null</c> to indicate that the user has not responded to the prompt
        /// (i.e. the user has pressed the Cancel button in the prompt window).
        /// </summary>
        /// <param name="promptText">Prompt text (i.e. title of the prompt window).</param>
        /// <param name="defaultResponse">Default user response (i.e. initial value contained in the prompt window).</param>
        /// <returns>User response (i.e. confirmed value contained in the prompt window).</returns>
        string Respond(string promptText, string defaultResponse);
    }
}
