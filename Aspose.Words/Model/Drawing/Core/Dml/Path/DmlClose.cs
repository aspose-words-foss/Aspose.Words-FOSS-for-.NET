// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents a close path part.
    /// </summary>
    /// <remarks>
    /// 20.1.9.6 close (Close Shape Path)
    /// This element specifies the ending of a series of lines and curves in the creation path of a
    /// custom geometric shape. When this element is encountered, the generating application should
    /// consider the corresponding path closed. That is, any further lines or curves that follow this
    /// element should be ignored.
    /// </remarks>
    internal class DmlClose : IDmlPathPart
    {
        public DmlPathPartType PathPartType
        {
            get { return DmlPathPartType.Close; }
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlClose"/>.
        /// </summary>
        public IDmlPathPart Clone()
        {
            return (DmlClose)MemberwiseClone();
        }

        public void Write(NrxXmlBuilder builder)
        {
            builder.WriteEmptyElement("a:close");
        }
    }
}
