// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin


namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies the type of a text form field.
    /// </summary>
    public enum TextFormFieldType
    {
        /// <summary>
        /// The text form field can contain any text.
        /// </summary>
        Regular = 0,
        /// <summary>
        /// The text form field can contain only numbers.
        /// </summary>
        Number = 1,
        /// <summary>
        /// The text form field can contain only a valid date value.
        /// </summary>
        Date = 2,
        /// <summary>
        /// The text form field value is the current date when the field is updated.
        /// </summary>
        CurrentDate = 3,
        /// <summary>
        /// The text form field value is the current time when the field is updated.
        /// </summary>
        CurrentTime = 4,
        /// <summary>
        /// The text form field value is calculated from the expression specified in
        /// the <see cref="FormField.TextInputDefault"/> property.
        /// </summary>
        Calculated = 5
    }
}
