// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/12/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A simple implementation of field code tokenizer that iterates over a field code represented by a text string.
    /// </summary>
    internal class TextFieldCodeTokenizer : IFieldCodeTokenizer
    {
        internal TextFieldCodeTokenizer(string fieldCode, bool moveToFirstToken)
        {
            mFieldCode = fieldCode;
            mIndex = -1;

            if (moveToFirstToken)
                MoveToNextToken();
        }

        public bool MoveToNextToken()
        {
            mIndex++;
            return !IsEof;
        }

        public DocumentPosition GetCurrentPosition()
        {
            return DocumentPosition.Void;
        }

        public FieldCodeToken CurrentToken
        {
            get { return FieldCodeToken.TextChar; }
        }

        public bool IsEndOfToken
        {
            get { return false; }
        }

        public char CurrentChar
        {
            get { return mFieldCode[mIndex]; }
        }

        public Node CurrentNode
        {
            get { return null; }
        }

        public bool IsEof
        {
            get { return mIndex >= mFieldCode.Length;  }
        }

        public int CurrentLocaleId
        {
            // Always return a constant as no formatting properties are available to this tokenizer.
            get { return RunPr.ProcessOrUserDefaultLanguageId; }
        }

        public int CurrentLocaleIdFarEast
        {
            // Always return a constant as no formatting properties are available to this tokenizer.
            get { return RunPr.ProcessOrUserDefaultLanguageId; }
        }

        public int CurrentLocaleIdBi
        {
            // Always return a constant as no formatting properties are available to this tokenizer.
            get { return RunPr.ProcessOrUserDefaultLanguageId; }
        }

        public bool CurrentBidi
        {
            // Always return a constant as no formatting properties are available to this tokenizer.
            get { return false; }
        }

        public string CurrentFontName
        {
            get { return null; }
        }

        private readonly string mFieldCode;
        private int mIndex;
    }
}
