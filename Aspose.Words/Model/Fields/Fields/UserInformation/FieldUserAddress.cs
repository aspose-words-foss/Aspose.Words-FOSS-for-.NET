// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the USERADDRESS field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the current user's postal address.
    /// </remarks>
    public class FieldUserAddress : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            string result = FieldCodeCache.HasArgument(UserAddressArgumentIndex)
                ? UserAddress
                : FetchDocument().FieldOptions.EffectiveUser.Address;

            result = StringUtil.Truncate(result, MaxResultLength);

            return new FieldUpdateActionApplyResult(this, result);
        }

        /// <summary>
        /// Gets or sets the current user's postal address.
        /// </summary>
        public string UserAddress
        {
            get { return FieldCodeCache.GetArgumentAsString(UserAddressArgumentIndex); }
            set { FieldCodeCache.SetArgument(UserAddressArgumentIndex, value); }
        }

        private const int UserAddressArgumentIndex = 0;
        private const int MaxResultLength = 255;
    }
}
