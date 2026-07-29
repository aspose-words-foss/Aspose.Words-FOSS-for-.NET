// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2010 by Dmitry Vorobyev

using System;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements a SYMBOL field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the character whose code point value is specified in decimal or hexadecimal.
    /// </remarks>
    public class FieldSymbol : Field, IFieldCodeTokenInfoProvider
    {
        internal override NodeRange GetFakeResult()
        {
            Inline sourceNode = GetResultFormatSourceNode();
            RunPr resultFormat = sourceNode != null
                ? sourceNode.RunPr.Clone()
                : null;
            ParaPr resultParagraphFormat = sourceNode != null && sourceNode.ParentParagraph != null
                ? sourceNode.ParentParagraph.ParaPr.Clone()
                : null;

            Run run = new Run(Document, GetResultText(), resultFormat);
            Paragraph para = new Paragraph(Document, resultParagraphFormat, new RunPr());
            para.AppendChild(run);

            if (StringUtil.HasChars(FontName))
                run.Font.Name = FontName;
            if (FontSizeAsDouble > 0d)
                run.Font.Size = FontSizeAsDouble;

            return new NodeRange(run, run);
        }

        private string GetResultText()
        {
            // WORDSNET-11095 Field can have incorrect syntax, without character code.
            if (!FieldCodeCache.HasArgument(0))
                return ErroneousResult;

            int intCharCode = GetIntCharacterCode();

            // This condition includes int.MinValue (parsing error), negative values, and zero.
            if (intCharCode <= 0)
                return ErroneousResult;

            if (!IsUnicode)
            {
                // WORDSNET-12021 The symbol is considered ANSI unless the /u char is specified explicitly.

                if (intCharCode > 0xFF)
                    return ErroneousResult;

                return new string(gAnsiEncoding.GetChars(new byte[] { (byte)intCharCode }));
            }
            else
            {
                // Since the .NET string is Unicode string, we can simply cast the character code.
                return ((char)intCharCode).ToString();
            }
        }

        private Inline GetResultFormatSourceNode()
        {
            foreach (Node node in GetFieldCodeRange())
            {
                Inline inline = node as Inline;
                if (inline != null)
                    return inline;
            }

            Debug.Fail("Could not find source node");

            return null;
        }

        /// <summary>
        /// Gets or sets the character's code point value in decimal or hexadecimal.
        /// </summary>
        public string CharacterCode
        {
            get { return FieldCodeCache.GetArgumentAsString(CharacterCodeArgumentIndex); }
            set { FieldCodeCache.SetArgument(CharacterCodeArgumentIndex, value); }
        }

        /// <summary>
        /// Gets the code of the character retrieved by the field.
        /// </summary>
        private int GetIntCharacterCode()
        {
            return IsHex
                ? FormatterPal.TryParseHex(CharacterCode.Substring(2))
                : FormatterPal.TryParseInt(CharacterCode);
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsAnsiSwitch:
                case DontAffectsLineSpacingSwitch:
                case IsShiftJisSwitch:
                case IsUnicodeSwitch:
                    {
                        return FieldSwitchType.Flag;
                    }
                case FontNameSwitch:
                case FontSizeSwitch:
                    {
                        return FieldSwitchType.HasArgument;
                    }
                default:
                    {
                        return FieldSwitchType.Unknown;
                    }
            }
        }

        private bool IsHex
        {
            get
            {
                return CharacterCode.StartsWith("0x", StringComparison.Ordinal) ||
                    CharacterCode.StartsWith("0X", StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Gets or sets the name of the font of the character retrieved by the field.
        /// </summary>
        public string FontName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FontNameSwitch); }
            set { FieldCodeCache.SetSwitch(FontNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the size in points of the font of the character retrieved by the field.
        /// </summary>
        public string FontSize //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FontSizeSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(FontSizeSwitch, value); }
        }

        private double FontSizeAsDouble
        {
            get { return FieldCodeCache.GetSwitchArgumentAsDouble(FontSizeSwitch); }
        }

        /// <summary>
        /// Gets or sets whether the character code is interpreted as the value of an ANSI character.
        /// </summary>
        public bool IsAnsi
        {
            get { return FieldCodeCache.HasSwitch(IsAnsiSwitch); }
            set { FieldCodeCache.SetSwitch(IsAnsiSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the character code is interpreted as the value of a Unicode character.
        /// </summary>
        public bool IsUnicode
        {
            get { return FieldCodeCache.HasSwitch(IsUnicodeSwitch); }
            set { FieldCodeCache.SetSwitch(IsUnicodeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the character code is interpreted as the value of a SHIFT-JIS character.
        /// </summary>
        public bool IsShiftJis
        {
            get { return FieldCodeCache.HasSwitch(IsShiftJisSwitch); }
            set { FieldCodeCache.SetSwitch(IsShiftJisSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the character retrieved by the field affects the line spacing of the paragraph.
        /// </summary>
        public bool DontAffectsLineSpacing
        {
            get { return FieldCodeCache.HasSwitch(DontAffectsLineSpacingSwitch); }
            set { FieldCodeCache.SetSwitch(DontAffectsLineSpacingSwitch, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int CharacterCodeArgumentIndex = 0;

        private const string IsAnsiSwitch = "\\a";
        private const string FontNameSwitch = "\\f";
        private const string DontAffectsLineSpacingSwitch = "\\h";
        private const string IsShiftJisSwitch = "\\j";
        private const string FontSizeSwitch = "\\s";
        private const string IsUnicodeSwitch = "\\u";

        /// <summary>
        /// This string is displayed when symbol code parsing goes wrong.
        /// </summary>
        private const string ErroneousResult = "###";

        private static readonly Encoding gAnsiEncoding = Encoding.GetEncoding(1252);
    }
}
