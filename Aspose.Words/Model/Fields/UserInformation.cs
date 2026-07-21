// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2016 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies information about the user.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class UserInformation
    {
        static UserInformation()
        {
            DefaultUser = new UserInformation();
        }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user's initials.
        /// </summary>
        public string Initials { get; set; }

        /// <summary>
        /// Gets or sets the user's postal address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Default user information.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="FieldOptions.CurrentUser"/> property to specify user information for single document.
        /// </remarks>
        public static UserInformation DefaultUser { get; }
    }
}
