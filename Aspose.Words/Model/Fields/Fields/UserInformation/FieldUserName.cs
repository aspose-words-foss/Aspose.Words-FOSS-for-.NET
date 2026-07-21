// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the USERNAME field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the current user's name.
    /// </remarks>
    public class FieldUserName : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            string result = FieldCodeCache.HasArgument(UserNameArgumentIndex)
                ? UserName
                : FetchDocument().FieldOptions.EffectiveUser.Name;

            result = StringUtil.Truncate(result, MaxResultLength);

            return new FieldUpdateActionApplyResult(this, result);
        }

        /// <summary>
        /// Gest or sets the current user's name.
        /// </summary>
        public string UserName
        {
            get { return FieldCodeCache.GetArgumentAsString(UserNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(UserNameArgumentIndex, value); }
        }

        private const int UserNameArgumentIndex = 0;
        private const int MaxResultLength = 55;
    }
}
