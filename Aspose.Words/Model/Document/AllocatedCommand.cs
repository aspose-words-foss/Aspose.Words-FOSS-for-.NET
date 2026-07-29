// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2010 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// http://www.documentinteropinitiative.org/additionalinfo/sect1.aspx#_Toc253758987
    /// 1.2.1.1.1. acd (Allocated Command)
    /// </summary>
    internal class AllocatedCommand
    {
        /// <summary>
        /// Ctor. By the default this creates an "unused" command.
        /// </summary>
        internal AllocatedCommand()
        {
        }

        internal AllocatedCommand(FixedCommandIdentifier fciBasedOn, byte[] argValue)
        {
            this.FciBasedOn = fciBasedOn;
            this.ArgValue = argValue;
        }

        /// <summary>
        /// Specifies the fixed command that shall be used for this allocated command.
        /// </summary>
        internal FixedCommandIdentifier FciBasedOn;

        /// <summary>
        /// Specifies the argument that should be passed to the fixed command.
        /// </summary>
        internal byte[] ArgValue = ArrayUtil.EmptyByteArray;
    }
}
