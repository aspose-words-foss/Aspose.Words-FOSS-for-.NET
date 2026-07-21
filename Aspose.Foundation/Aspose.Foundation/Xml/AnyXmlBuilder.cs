// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Bidi;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.IO;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Xml
{
    public class AnyXmlBuilder : IAnyXmlBuilder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="encoding">Encoding to use. Do not forget about BOM in case of Encoding.UTF8</param>
        /// <param name="isPrettyFormat">True to use pretty formatting for the output XML.</param>
        public AnyXmlBuilder(Stream stream, Encoding encoding, bool isPrettyFormat)
        {
            // .NET does not seem to write a preamble when writing in the UTF-7 encoding.
            // We write it ourselves here, but before writing check - .NET might have it fixed one day and we don't write in that case.
            const int codePageUtf7 = 65000;
            byte[] utf7Bom = new byte[] { 0x2B, 0x2F, 0x76, 0x38, 0x2D };
            if (encoding.CodePage == codePageUtf7)
            {
                Debug.Assert(encoding.WebName == "utf-7");
                byte[] bom = encoding.GetPreamble();
                if (bom.Length == 0)
                {
                    bom = utf7Bom;
                    stream.Write(bom, 0, bom.Length);
                }
            }

            mXmlWriter = XmlUtilPal.CreateXmlTextWriter(stream, encoding);
            mXmlCharsFilter = new StringBuilder(2048);

            DebugInitStackOfNames();

            // SPEED RK: This makes export in .NET almost twice as fast. We take care about namespaces in our code ourselves.
            mXmlWriter.Namespaces = false;

            if (isPrettyFormat)
            {
                mXmlWriter.Formatting = Formatting.Indented;
                mXmlWriter.Indentation = 1;
                mXmlWriter.IndentChar = '\t';
            }
        }

        /// <summary>
        /// Ctor. Uses UTF8 encoding.
        /// </summary>
        public AnyXmlBuilder(Stream stream, bool isPrettyFormat)
            : this(stream, new UTF8Encoding(false), isPrettyFormat)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether elements of the output XML are indented.
        /// </summary>
        public bool PrettyFormat
        {
            get { return mXmlWriter.Formatting == Formatting.Indented; }
            set
            {
                mXmlWriter.Formatting = value
                    ? Formatting.Indented
                    : Formatting.None;
            }
        }

        /// <summary>
        /// Gets the underlying stream object.
        /// </summary>
        public Stream BaseStream
        {
            get { return mXmlWriter.BaseStream; }
        }

        /// <summary>
        /// Starts the XML document and the root element. The document is marked with the "standalone=yes" attribute.
        /// </summary>
        public virtual void StartDocument([CppForceStringParam] string rootElementName)
        {
            mXmlWriter.WriteStartDocument(true);
            StartElement(rootElementName);
        }

        /// <summary>
        /// Starts the XML document. No root element and no standalone attribute are written.
        /// </summary>
        public void StartDocument()
        {
            mXmlWriter.WriteStartDocument();
        }

        /// <summary>
        /// Starts the XML document with the specified standalone attribute value. No root element is written.
        /// </summary>
        public void StartDocument(bool standalone)
        {
            mXmlWriter.WriteStartDocument(standalone);
        }

        /// <summary>
        /// Starts the XML document and the root element.
        /// </summary>
        public void StartDocument(string rootElementName, bool standalone)
        {
            mXmlWriter.WriteStartDocument(standalone);
            StartElement(rootElementName);
        }

        /// <summary>
        /// Closes all opened elements and the XML document itself and flushes it to the undelying stream.
        /// </summary>
        public virtual void EndDocument()
        {
            while (mDepth > 0)
            {
                EndElement();
            }
            mXmlWriter.WriteEndDocument();
            mXmlWriter.Flush();
        }

        /// <summary>
        /// Writes start of the element with the specified name.
        /// </summary>
        public void StartElement(string elementName)
        {
            StartElement(elementName, false);
        }

        /// <summary>
        /// Writes start of the element with the specified name.
        /// </summary>
        /// <remarks>
        /// This overload accepts a flag that indicates whether children of this element are forcibly written without pretty
        /// format, even if pretty format is turned on for the rest of the XML document. This flag allows to correctly track
        /// indentation of elements and, as a result, allows to correctly format inserted multi-line raw text.
        /// </remarks>
        public void StartElement(string elementName, bool forceNoPrettyFormatForChildren)
        {
            RestoreIndentationAfterIndentedRawContentIfNeeded();

            mXmlWriter.WriteStartElement(elementName);
            DebugPushElementName(elementName);

            ++mDepth;
            if (PrettyFormat && !forceNoPrettyFormatForChildren)
            {
                ++mIndentationLevel;
            }
        }

        /// <summary>
        /// Writes end of the current element.
        /// </summary>
        public void EndElement()
        {
            EndElement(null, false);
        }

        /// <summary>
        /// Writes end of the current element.
        /// In debug mode checks the closing element name.
        /// </summary>
        public void EndElement(string elementName)
        {
            EndElement(elementName, false);
        }

        /// <summary>
        /// Writes end of the current element (self-closing or full end tag).
        /// In debug mode optionally checks the closing element name.
        /// </summary>
        private void EndElement(string elementName, bool alwaysWriteFullEndTag)
        {
            Debug.Assert(mDepth > 0);
            mDepth = Math.Max(mDepth - 1, 0);

            DebugPopElementName(elementName);
            if (PrettyFormat)
            {
                mIndentationLevel = Math.Max(mIndentationLevel - 1, 0);
            }
            RestoreIndentationAfterIndentedRawContentIfNeeded();
            if (alwaysWriteFullEndTag)
            {
                mXmlWriter.WriteFullEndElement();
            }
            else
            {
                mXmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the full end tag of the current element.
        /// </summary>
        public void FullEndElement()
        {
            EndElement(null, true);
        }

        /// <summary>
        /// Writes the full end tag of the current element.
        /// In debug mode checks the closing element name.
        /// </summary>
        public void FullEndElement(string elementName)
        {
            EndElement(elementName, true);
        }

        /// <summary>
        /// Writes text as is without entitizing or filtering invalid chars.
        /// </summary>
        public void WriteRaw(string text)
        {
            WriteRaw(text, false);
        }

        /// <summary>
        /// Writes text as is without entitizing or filtering invalid chars.
        /// Optionally indents lines of the text to match parent element indentation.
        /// </summary>
        [JavaAttributes.JavaConvertCheckedExceptions]
        public void WriteRaw(string text, bool indentLinesIfFormattingEnabled)
        {
            if (indentLinesIfFormattingEnabled)
            {
                WriteRaw(new StringReader(text), true);
            }
            else
            {
                mXmlWriter.WriteRaw(text);
            }
        }

        /// <summary>
        /// Reads text from the stream and writes it as is without entitizing or filtering invalid chars.
        /// The text must be encoded in UTF8.
        /// Optionally indents lines of the text to match parent element indentation.
        /// </summary>
        public void WriteRaw(Stream textStream, bool indentLinesIfFormattingEnabled)
        {
            WriteRaw(new StreamReader(textStream, Encoding.UTF8), indentLinesIfFormattingEnabled);
        }

        /// <summary>
        /// Writes empty element with the specified name.
        /// </summary>
        public void WriteEmptyElement(string elementName)
        {
            WriteElement(elementName, null);
        }

        /// <summary>
        /// Writes element with the specified name and value.
        /// </summary>
        public void WriteElement(string elementName, string value)
        {
            StartElement(elementName);
            WriteString(value);
            EndElement();
        }

        /// <summary>
        /// Writes element with the specified name and a string value if the string is not null or empty.
        /// </summary>
        public void WriteOptionalElement(string elementName, string value)
        {
            if (StringUtil.HasChars(value))
                WriteElement(elementName, value);
        }

        /// <summary>
        /// Writes element with the specified name and a DateTime value.
        /// DateTime is validated (year value should be greater than 1).
        /// If DateTime is invalid the element is not written.
        /// </summary>
        public void WriteOptionalElement(string elementName, DateTime value)
        {
            if (value.Year > 1)
                WriteElement(elementName, FormatterPal.DateTimeToXmlUtc(value));
        }

        /// <summary>
        /// Writes element with the specified name and an integer value.
        /// </summary>
        public void WriteElement(string elementName, int value)
        {
            WriteElement(elementName, FormatterPal.IntToXml(value));
        }

        /// <summary>
        /// Writes the given text content. Filters out invalid characters.
        /// </summary>
        public void WriteString(string value)
        {
            mXmlWriter.WriteString(FilterInvalidXmlChars(value));
        }

        /// <summary>
        /// Writes the given text content. Filters out invalid characters. Converts standalone /r character into _x000D string.
        /// </summary>
        /// <remarks>See <see cref="AnyXmlReader.ReadStringConvertSpecialCharacters" /> for explanation.</remarks>
        public void WriteStringConvertSpecialCharacters(string value)
        {
            WriteString(value.Replace("\r\n", "\n").Replace("\r", "_x000D_").Replace("\n", "\r\n"));
        }

        /// <summary>
        /// Writes an attribute string if value is not default. Always. Filters out invalid characters.
        /// </summary>
        public void WriteAttributeStringIfNotDefault(string name, string value, string defaultValue)
        {
            if (value == defaultValue)
                return;

            WriteAttributeString(name, value);
        }

        /// <summary>
        /// Writes an attribute string. Always. Filters out invalid characters.
        /// </summary>
        public void WriteAttributeString(string name, string value)
        {
            mXmlWriter.WriteAttributeString(name, FilterInvalidXmlChars(value));
        }

        /// <summary>
        /// Writes MIME-compliant Base64 data. Compliance means that Base64 string is broken into lines, containing
        /// exactly 76 characters each. Only the last line of base64 data can contain less than 76 characters.
        /// </summary>
        public void WriteBase64(Stream stream)
        {
            // Little optimization. In case of MemoryStream lets access bytes directly.
            MemoryStream streamMemoryStream = stream as MemoryStream;

            if (streamMemoryStream != null)
                WriteBase64(streamMemoryStream.GetBuffer(), 0, (int)stream.Length);
            else
                WriteBase64(StreamUtil.CopyStreamToByteArray(stream), 0, (int)stream.Length);
        }

        /// <summary>
        /// Writes MIME-compliant Base64 data. Compliance means that Base64 string is broken into lines, containing
        /// exactly 76 characters each. Only the last line of base64 data can contain less than 76 characters.
        /// </summary>
        public void WriteBase64(byte[] buffer, int index, int count)
        {
            Base64Splitter splitter = new Base64Splitter(buffer, index, count);
            while (!splitter.IsEof)
            {
                WriteString(splitter.GetNext());
                mXmlWriter.WriteWhitespace(NewLine);
            }
        }

        /// <summary>
        /// Writes out a comment &lt;!--...--&gt; containing the specified text.
        /// </summary>
        public void WriteComment(string text)
        {
            mXmlWriter.WriteComment(text);
        }

        /// <summary>
        /// Writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.
        /// </summary>
        public void WriteProcessingInstruction(string name, string text)
        {
            mXmlWriter.WriteProcessingInstruction(name, text);
        }

        /// <summary>
        /// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
        /// </summary>
        public void Flush()
        {
            mXmlWriter.Flush();
        }

        /// <summary>
        /// Removes all characters that are invalid in XML.
        /// </summary>
        public string FilterInvalidXmlChars(string text)
        {
            // RK Performance optimization.
            if (!HasInvalidChars(text))
                return text;

            mXmlCharsFilter.Length = 0;
            foreach (int utf32Char in new StringUtf32Enumerator(text))
            {
                if (IsValidXmlChar(utf32Char))
                    UnicodeUtil.AppendUtf32(mXmlCharsFilter, utf32Char);
            }

            return mXmlCharsFilter.ToString();
        }

        public bool HasInvalidChars(string text)
        {
            if (!StringUtil.HasChars(text))
                return false;

            // Performace optimization. Use the field instead of creating a new StringUtf32Enumerator object
            // to reduce memory allocations and fragmentation.
            mStringEnumerator.SetText(text);
            foreach (int utf32Char in mStringEnumerator)
            {
                if (!IsValidXmlChar(utf32Char))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// According to the XML specification valid characters are:
        /// #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]
        ///
        /// RK In DOC files we could sometimes get characters that will be invalid
        /// from XML point of view. Also, a user can insert text that contains invalid XML
        /// characters via the model. So we should actually take care and filter all text
        /// and document properties (and probably all attribute values) before writing to
        /// any XML format.
        /// </summary>
        private static bool IsValidXmlChar(int utf32Char)
        {
            return
                (utf32Char == 0x0009) ||
                (utf32Char == 0x000a) ||
                (utf32Char == 0x000d) ||
                ((utf32Char >= 0x0020) && (utf32Char <= 0xd7ff)) ||
                ((utf32Char >= 0xe000) && (utf32Char <= 0xfffd)) ||
                ((utf32Char >= 0x10000) && (utf32Char <= 0x10FFFF));
        }

        /// <summary>
        /// Reads text from the reader and writes it as is without entitizing or filtering invalid chars.
        /// The text must be encoded in UTF8.
        /// Optionally indents lines of the text to match parent element indentation.
        /// </summary>
        private void WriteRaw(TextReader textReader, bool indentLines)
        {
            // Do not write anything if the reader is empty.
            if (textReader.Peek() == -1)
            {
                return;
            }

            if (indentLines)
            {
                string indentation = new string(mXmlWriter.IndentChar, mXmlWriter.Indentation * mIndentationLevel);

                bool isMultiline = false;
                string line = textReader.ReadLine();
                while (line != null)
                {
                    if (isMultiline || PrettyFormat)
                    {
                        mXmlWriter.WriteRaw(NewLine);
                        mXmlWriter.WriteRaw(indentation);
                    }
                    mXmlWriter.WriteRaw(line);

                    line = textReader.ReadLine();
                    isMultiline = true;
                }

                mAfterIndentedRawContent = PrettyFormat;
            }
            else
            {
                mXmlWriter.WriteRaw(textReader.ReadToEnd());
            }
        }

        private void RestoreIndentationAfterIndentedRawContentIfNeeded()
        {
            if (mAfterIndentedRawContent)
            {
                mXmlWriter.WriteRaw(NewLine);
                string indentation = new string(mXmlWriter.IndentChar, mXmlWriter.Indentation * mIndentationLevel);
                mXmlWriter.WriteRaw(indentation);
                mAfterIndentedRawContent = false;
            }
        }

        private readonly XmlTextWriter mXmlWriter;
        private readonly StringBuilder mXmlCharsFilter;
        private readonly StringUtf32Enumerator mStringEnumerator = new StringUtf32Enumerator(string.Empty);

        /// <summary>
        /// Current element nesting depth (number of currently opened XML elements).
        /// </summary>
        private int mDepth;

        /// <summary>
        /// Current element indentation level.
        /// </summary>
        /// <remarks>
        /// This is different from the current nesting depth, because some elements might be written with pretty print
        /// turned off. For example, inline contents (spans, images) of HTML paragraphs.
        /// </remarks>
        private int mIndentationLevel;

        /// <summary>
        /// Indicates whether indented raw content was written into the current element. This flag is needed to correctly indent
        /// text after the raw content.
        /// </summary>
        private bool mAfterIndentedRawContent;

        /// <summary>
        /// Line terminator string (line break characters). Used to split XML code into lines when pretty format is on.
        /// </summary>
        private const string NewLine = "\r\n";

        #region Checking matching start/end element tags
#if DEBUG
        /// <summary>
        /// Initializes stack of names needed to check matching start/end element tags.
        /// Called in debug mode only.
        /// </summary>
        [Conditional("DEBUG")]
        private void DebugInitStackOfNames()
        {
            mStackOfNames = new Stack<string>();
        }

        /// <summary>
        /// Pushes to the stack of names needed to check matching start/end element tags.
        /// Called in debug mode only.
        /// </summary>
        [Conditional("DEBUG")]
        private void DebugPushElementName(string elementName)
        {
            mStackOfNames.Push(elementName);
        }

        /// <summary>
        /// Checks matching start/end element tags using internally maintained stack.
        /// Called in debug mode only.
        /// </summary>
        [Conditional("DEBUG")]
        private void DebugPopElementName(string elementName)
        {
            // RK This method had Debug.Assert, but for some reason I cannot compile it with Aspose.Debug and
            // I don't want to compile it with System.Diagnostics.Debug. So I changed these to exceptions.
            if (mStackOfNames.Count <= 0)
                throw new InvalidOperationException("Stack of names empty.");

            string closing = mStackOfNames.Pop();
            if (StringUtil.HasChars(elementName))
            {
                string closingElementName = closing;
                if (DebugOneOfParagraphMatchingNodes(closingElementName))
                {
                    if (!DebugOneOfParagraphMatchingNodes(elementName))
                        throw new InvalidOperationException("DebugOneOfParagraphMatchingNodes " + elementName);
                }
                else if (DebugOneOfListMatchingNodes(closingElementName))
                {
                    if (!DebugOneOfListMatchingNodes(elementName))
                        throw new InvalidOperationException("DebugOneOfListMatchingNodes " + elementName);
                }
                else
                {
                    if (closingElementName != elementName)
                    {
                        throw new InvalidOperationException("Closing tag does not match expected " + closingElementName +
                                                            " " + elementName);
                    }
                }
            }
        }

        private static bool DebugOneOfParagraphMatchingNodes(string elementName)
        {
            // RK Even if this is just for debugging, I still don't like this HTML specific code in a common library class.
            return
                (elementName == "p") ||
                (elementName == "li") ||
                (elementName == "h1") ||
                (elementName == "h2") ||
                (elementName == "h3") ||
                (elementName == "h4") ||
                (elementName == "h5") ||
                (elementName == "h6");
        }

        private static bool DebugOneOfListMatchingNodes(string elementName)
        {
            return
                (elementName == "ol") ||
                (elementName == "ul");
        }

        /// <summary>
        /// Maintains a stack of started elements.
        /// </summary>
        private Stack<string> mStackOfNames;
#else
        [Conditional("DEBUG")]
        private static void DebugInitStackOfNames()
        {
            // Do nothing.
        }
        [Conditional("DEBUG")]
        private static void DebugPushElementName(string elementName)
        {
            // Do nothing.
        }
        [Conditional("DEBUG")]
        private static void DebugPopElementName(string elementName)
        {
            // Do nothing.
        }
#endif
#endregion Checking matching start/end element tags
    }
}
