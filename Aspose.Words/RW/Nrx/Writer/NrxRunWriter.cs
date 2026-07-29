// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/08/2015 by Alexey Morozov

using Aspose.JavaAttributes;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Base class to refactor methods from DocxRunWriter and WmlRunWriter.
    /// </summary>
    internal abstract class NrxRunWriter
    {
        /// <summary>
        /// Writes <see cref="Ruby"/> objects as part of 'r' element.
        /// </summary>
        protected void WriteRuby(INrxWriterContext writer, Inline inline)
        {
            Ruby ruby = (Ruby)inline.RunPr[FontAttr.Ruby];
            WriteRunStart(inline);

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("w:ruby");

            builder.StartElement("w:rubyPr");

            // WORDSNET-18692 Preserve element writing order as described in the ECMA-376 spec.
            builder.WriteVal("w:rubyAlign", NrxRunEnum.RubyAlignmentToXml(ruby.Alignment));
            builder.WriteVal("w:hps", ruby.TopSize);
            builder.WriteVal("w:hpsRaise", ruby.Distance);
            builder.WriteVal("w:hpsBaseText", ruby.BaseSize);

            string lidTag = writer.IsDocx
                ? LocaleConverter.LocaleToDocxTag((int)ruby.Language)
                : LocaleConverter.LocaleToWmlTag((int)ruby.Language);

            builder.WriteVal("w:lid", lidTag);

            builder.EndElement("w:rubyPr");

            builder.StartElement("w:rt");
            WriteRubyChunks(writer, ruby.Top);
            builder.EndElement("w:rt");

            builder.StartElement("w:rubyBase");
            WriteRubyChunks(writer, ruby.Base);
            builder.EndElement("w:rubyBase");

            builder.EndElement("w:ruby");
            WriteRunEnd(inline.RunPr);
        }

        private void WriteRubyChunks(INrxWriterContext writer, RubyChunkCollection chunks)
        {
            foreach (RubyChunk rubyChunk in chunks)
            {
                Run topRun = new Run(writer.Document, rubyChunk.Text, rubyChunk.RunPr);
                WriteInline(topRun);
            }
        }

        [JavaThrows(true)]
        internal abstract void WriteRunStart(Inline inline);
        [JavaThrows(true)]
        internal abstract void WriteInline(Inline inline);
        [JavaThrows(true)]
        internal abstract void WriteRunEnd(RunPr runPr);
    }
}
