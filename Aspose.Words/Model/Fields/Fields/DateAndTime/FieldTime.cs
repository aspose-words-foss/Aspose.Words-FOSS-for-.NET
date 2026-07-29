// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2009 by Dmitry Vorobyev

using Aspose.Common;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the TIME field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the current date and time.
    /// </remarks>
    public class FieldTime : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return new FieldUpdateActionApplyResult(this, new DateTimeConstant(FetchDocument().CurrentDateTimeCache));
        }

        internal override string GetDefaultDateTimeFormat()
        {
            // Can't use .NET's standard specifier here as Word does not have them.
            return FormatterPal.GetShortTimePatternCurrent();
        }
    }
}
