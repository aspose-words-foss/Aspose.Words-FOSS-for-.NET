// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2018 by Edward Voronov

using System.Globalization;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, provides a <see cref="CultureInfo"/> object that should be used during the update of a particular field.
    /// </summary>
    [JavaUsePublicApiMapOnly]
    public interface IFieldUpdateCultureProvider
    {
        /// <summary>
        /// Returns a <see cref="CultureInfo"/> object to be used during the field's update.
        /// </summary>
        /// <param name="culture">The name of the culture requested for the field being updated.</param>
        /// <param name="field">The field being updated.</param>
        /// <returns>The culture object that should be used for the field's update.</returns>
        CultureInfo GetCulture(string culture, Field field);
    }
}
