// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the SHAPE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the specified text.
    /// </remarks>
    public class FieldShape : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            FieldQuoteUpdater updater = new FieldQuoteUpdater(this);
            return updater.Update();
        }

        /// <summary>
        /// Gets or sets the text to retrieve.
        /// </summary>
        public string Text
        {
            get { return FieldCodeCache.GetArgumentAsString(TextArgumentIndex); }
            set { FieldCodeCache.SetArgument(TextArgumentIndex, value); }
        }

        private const int TextArgumentIndex = 0;
    }
}
