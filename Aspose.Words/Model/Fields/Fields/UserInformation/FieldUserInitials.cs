// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the USERINITIALS field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the current user's initials.
    /// </remarks>
    public class FieldUserInitials : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            string result = FieldCodeCache.HasArgument(UserInitialsArgumentIndex)
                ? UserInitials
                : FetchDocument().FieldOptions.EffectiveUser.Initials;

            result = StringUtil.Truncate(result, MaxResultLength);

            return new FieldUpdateActionApplyResult(this, result);
        }

        /// <summary>
        /// Gets or sets the current user's initials.
        /// </summary>
        public string UserInitials
        {
            get { return FieldCodeCache.GetArgumentAsString(UserInitialsArgumentIndex); }
            set { FieldCodeCache.SetArgument(UserInitialsArgumentIndex, value); }
        }

        private const int UserInitialsArgumentIndex = 0;
        private const int MaxResultLength = 10;
    }
}
