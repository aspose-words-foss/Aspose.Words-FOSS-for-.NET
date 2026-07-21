// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Morozov

using System;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Implements base class for OLE1.0 objects.
    /// </summary>
    internal abstract class Ole1Object
    {
        /// <summary>
        /// Factory method that reads and returns either Ole1LinkedObject or Ole1EmbeddedObject instance.
        /// </summary>
        internal static Ole1Object Read(BinaryReader reader)
        {
            ObjectHeader objectHeader = ObjectHeader.Read(reader);

            // Invalid OLE object header, cancel further reading.
            if (objectHeader == null)
                return null;

            switch (objectHeader.OleObjectType)
            {
                case OleObjectType.Linked:
                    return new Ole1LinkedObject(objectHeader, reader);

                case OleObjectType.Embedded:
                    return new Ole1EmbeddedObject(objectHeader, reader);

                default:
                    throw new InvalidOperationException("Unexpected OleObjectType value.");

            }
        }

        internal abstract OleObjectType ObjectType
        {
            get;
        }

        /// <summary>
        /// Writes object to given writer.
        /// </summary>
        [JavaThrows(true)]
        internal abstract void Write(BinaryWriter writer);

        /// <summary>
        /// Specifies the creating application in an implementation-specific manner.
        /// </summary>
        internal string ClassName
        {
            get { return mClassName; }
        }

        protected string mClassName;
    }
}
