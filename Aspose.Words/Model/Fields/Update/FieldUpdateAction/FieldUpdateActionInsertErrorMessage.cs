// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Inserts an error message in Word style.
    /// </summary>
    internal class FieldUpdateActionInsertErrorMessage : FieldUpdateAction
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionInsertErrorMessage(Field field, string message)
            : base(field)
        {
            mMessage = message;
        }

        internal override void Perform()
        {
            using (Field.UpdateContext.RemoveOldResultSafe())
            {
                DocumentBuilder builder = new DocumentBuilder(Document);
                builder.MoveTo(Field.End);
                builder.Bold = true;
                builder.Write(mMessage);
            }
        }

        private readonly string mMessage;
    }
}
