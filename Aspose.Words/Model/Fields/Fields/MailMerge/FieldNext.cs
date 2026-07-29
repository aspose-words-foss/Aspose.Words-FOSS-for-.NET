// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the NEXT field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Merges the next data record into the current resulting merged document, rather than starting a
    /// new merged document.
    /// </remarks>
    public class FieldNext : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return MergeFieldUtil.UpdateRuleField(this, "«Next Record»");
        }
    }
}
