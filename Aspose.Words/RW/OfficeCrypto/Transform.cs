// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/09/2010 by Alexey Morozov

using System;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Base class for XML transformation objects.
    /// </summary>
    internal abstract class Transform
    {
        /// <summary>
        /// Factory method used to create Transform object of given type.
        /// </summary>
        internal static Transform Create(string alg)
        {
            switch (alg)
            {
                case C14Transform.Algorithm:
                    return new C14Transform();

                case RelationshipTransform.Algorithm:
                    return new RelationshipTransform();

                default:
                    throw new InvalidOperationException(string.Format("Unknown transform algorithm '{0}'", alg));
            }
        }

        /// <summary>
        /// Factory method used to create appropriate Transform object from XML element. 
        /// </summary>
        /// <remarks>
        /// Reader should be positioned at Transform XML element start.
        /// </remarks>
        internal static Transform Create(AnyXmlReader reader)
        {
            Transform transform = Create(reader.ReadAttribute("Algorithm", null));
            transform.Read(reader);
            return transform;
        }

        /// <summary>
        /// Apply current transform to given XML.
        /// </summary>
        [JavaThrows(true)]
        internal abstract MemoryStream Apply(Stream xml);

        /// <summary>
        /// Writes Transform object in XML form.
        /// </summary>
        internal abstract void Write(AnyXmlBuilder writer);

        /// <summary>
        /// Reads object from Xml element.
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void Read(AnyXmlReader reader);
    }
}
