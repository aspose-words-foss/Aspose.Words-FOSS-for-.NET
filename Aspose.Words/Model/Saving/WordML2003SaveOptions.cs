// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/07/2010 by Roman Korchagin

using System;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Can be used to specify additional options when saving a document into the <see cref="Words.SaveFormat.WordML"/> format.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-save-options/">Specify Save Options</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>At the moment provides only the <see cref="SaveFormat"/> property, but in the future may have other options added.</para>
    /// </remarks>
    public class WordML2003SaveOptions : SaveOptions
    {
        /// <summary>
        /// Specifies the format in which the document will be saved if this save options object is used.
        /// Can only be <see cref="Words.SaveFormat.WordML"/>.
        /// </summary>
        public override SaveFormat SaveFormat
        {
            get { return SaveFormat.WordML; }
            set
            {
                if (value != SaveFormat.WordML)
                    throw new ArgumentException("An invalid SaveFormat for this options type was chosen.");
            }
        }
    }
}
