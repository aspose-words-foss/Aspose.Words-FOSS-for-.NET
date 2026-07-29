// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Roman Korchagin

using System.Text;
using Aspose.Words.Revisions;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// This class provides common code for DOCX and WML annotation readers.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class NrxAnnotationReader
    {
        internal NrxAnnotationReader(NrxRunPrReaderBase runPrReader,
            NrxParaPrReaderBase paraPrReader)
        {
            Debug.Assert(runPrReader != null);
            Debug.Assert(paraPrReader != null);
            mRunPrReader = runPrReader;
            mParaPrReader = paraPrReader;
        }

        /// <summary>
        /// In DOCX and WML we have AfterChanges and BeforeChanges properties, but in the model
        /// we need to have BeforeChanges and PositiveDifference. 
        /// This method reads previous formatting properties (BeforeChanges) and converts to the model way. 
        /// The AfterChanges must be already read and the reader must have the paragraph as the current container.
        /// </summary>
        internal void ReadParaPr(NrxDocumentReaderBase reader, ParaPr paraPr, RunPr paragraphBreakRunPr, NrxAnnotation annotation)
        {
            // This is now AfterChanges.
            ParaPr revPr = paraPr.Clone();

            // Read the BeforeChanges into the original runPr because we do not return a new collection.
            paraPr.Clear();
            mParaPrReader.Read(reader, paraPr, paragraphBreakRunPr);

            revPr.MoveTo(paraPr, RevisionAttr.NumberRevision);
            revPr.MoveTo(paraPr, ParaAttr.RsidP);

            paraPr.FormatRevision = new FormatRevision(revPr, annotation.Author, annotation.Date);
        }

        internal void ReadRunPr(NrxDocumentReaderBase reader, RunPr runPr, NrxAnnotation annotation)
        {
            // This is now AfterChanges.
            RunPr revPr = runPr.Clone();

            // Read the BeforeChanges into the original runPr because we do not return a new collection.
            runPr.Clear();
            mRunPrReader.Read(reader, runPr);

            // WORDSNET-23778 Ignore a nested insertion revision.
            runPr.Remove(RevisionAttr.InsertRevision);

            revPr.MoveTo(runPr, RevisionAttr.InsertRevision);
            revPr.MoveTo(runPr, RevisionAttr.DeleteRevision);
            revPr.MoveTo(runPr, RevisionAttr.MoveFromRevision);
            revPr.MoveTo(runPr, RevisionAttr.MoveToRevision);
            revPr.MoveTo(runPr, FontAttr.RsidR);
            revPr.MoveTo(runPr, FontAttr.RsidRPr);

            runPr.FormatRevision = new FormatRevision(revPr, annotation.Author, annotation.Date);
        }

        /// <summary>
        /// Sets numbering related properties of NumberRevision according to w:original attribute value.
        /// Described in ECMA TC 45: 2.13.5.30 numberingChange (Previous Paragraph Numbering Properties).
        /// Example: "%1:3:0:-%2:1:0:%3:1:4:.".
        /// </summary>
        internal static void ReadOriginal(ParagraphNumberRevision numberRevision, string original)
        {
            // Position in w:original string.
            int pos = 0;

            // Index into NumberRevision array properties (NumberLocations, NumberStyles, NumberValues).
            int index = 0;

            StringBuilder numFormatBuilder = new StringBuilder();

            while (pos < original.Length)
            {
                // I have seen w:original to be "-" sometimes. I don't know how to parse such cases,
                // so I simply skip them for now.
                if (original[pos] != '%')
                    break;

                pos++;

                numFormatBuilder.Append((char)(original[pos] - '1'));
                // WORDSNET-5583 By storing number location after appending we solve the problem.
                // RK It is strange, but does it look like indexes in NumberLocations are 1-based?
                numberRevision.NumberLocations[index] = (byte)numFormatBuilder.Length;

                pos += 2; // Skip ':'.
                numberRevision.NumberValues[index] = original[pos] - '0';
                pos += 2; // Skip ':'.
                numberRevision.NumberStyles[index] = (NumberStyle)(original[pos] - '0');
                pos += 2; // Skip ':'.

                if ((pos < original.Length) && original[pos] != '%')
                {
                    numFormatBuilder.Append(original[pos]);
                    pos++;
                }

                index++;
            }

            numberRevision.NumberFormat = numFormatBuilder.ToString();
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly NrxRunPrReaderBase mRunPrReader;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly NrxParaPrReaderBase mParaPrReader;
    }
}
