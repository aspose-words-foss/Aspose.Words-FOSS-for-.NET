// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Describes relationships between diagram nodes.
    /// 
    /// In DOC, RTF and model the values are CRC32 hashes of the shape names.
    /// In WordML and DOCX these values refer to shape id (which is the shape name).
    /// 
    /// Made into a class to simplify autporting to Java. Do not make this a struct.
    /// </summary>
    internal class DiagramNodeRelation
    {
        /// <summary>
        /// idsrc in WML.
        /// </summary>
        internal int A;
        /// <summary>
        /// iddst in WML.
        /// </summary>
        internal int B;
        /// <summary>
        /// idcntr in WML.
        /// </summary>
        internal int C;
    }
}
