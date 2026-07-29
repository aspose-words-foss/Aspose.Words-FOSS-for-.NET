// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2013 by Ivan Lyagin

using System;

namespace Aspose.Collections
{
    /// <summary>
    /// Provides utility methods shared between bidirectional map classes.
    /// </summary>
    internal static class BidirectionalMapUtil
    {
        /// <summary>
        /// Validates the given object value. Returns it if it is not null. Otherwise, returns the given
        /// default value if it is not null. Otherwise, throws.
        /// </summary>
        internal static object ValidateValue(object value, object defaultValue)
        {
            return (value != null) ? value : ValidateDefaultValue(defaultValue);
        }

        /// <summary>
        /// Validates the given string value. Returns it if it is not null. Otherwise, returns the given
        /// default value if it is not null. Otherwise, throws.
        /// </summary>
        internal static string ValidateValue(string value, string defaultValue)
        {
            return (value != null) ? value : (string)ValidateDefaultValue(defaultValue);
        }

        /// <summary>
        /// Validates the given integer value. Returns it if it is not equal to <see cref="int.MinValue"/>. 
        /// Otherwise, returns the given default value if it is not equal to <see cref="int.MinValue"/>. 
        /// Otherwise, throws.
        /// </summary>
        internal static int ValidateValue(int value, int defaultValue)
        {
            return (value != int.MinValue) ? value : ValidateDefaultValue(defaultValue);
        }

        private static object ValidateDefaultValue(object defaultValue)
        {
            if (defaultValue != null)
                return defaultValue;

            throw CreateValueValidationException();
        }

        private static int ValidateDefaultValue(int defaultValue)
        {
            if (defaultValue != int.MinValue)
                return defaultValue;

            throw CreateValueValidationException();
        }

        private static InvalidOperationException CreateValueValidationException()
        {
            return new InvalidOperationException(
                "A value for the specified key is missing and the default value is not provided."); 
        }
    }
}
