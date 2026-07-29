// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2019 by Alexander Zhiltsov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a repeating section item SDT. See chapter 2.5.1.11 repeatingSectionItem of [MS-DOCX] Word Extensions
    /// to the Office Open XML.
    /// </summary>
    internal class SdtRepeatingSectionItem : SdtControlProperties
    {
        internal override SdtType Type
        {
            get { return SdtType.RepeatingSectionItem; }
        }
    }
}
