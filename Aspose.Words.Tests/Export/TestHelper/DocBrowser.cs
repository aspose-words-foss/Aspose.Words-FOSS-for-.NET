// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Dmitry Sokolov

#if !NETSTANDARD

using System.Collections.Generic;
using System.Web;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Essence for testing purpose.
    /// Helper for testing "Save" method which send document to client browser.
    /// </summary>
    [JavaDelete("We do not provide saving document to a response stream in Java.")]
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("We do not provide saving document to a response stream in Java.")]
    internal class DocTestStub : Document
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DocTestStub():base()
        { }

        /// <summary>
        /// Ctr. with file name.
        /// </summary>
        /// <param name="fileName">Name of the destination file for save.</param>
        public DocTestStub(string fileName)
            : base(fileName)
        {
            if (InitialShapeId > 0)
                SetNextShapeId(InitialShapeId);
        }

        protected override void WriteDocToHttResponse(
            HttpResponse response,
            byte[] docData,
            int dataLength,
            string headerName,
            string headerValue)
        {           
           // Add received header for check.
           mAttachedHeaders.Add(headerName, headerValue);
        }

        internal override int MapShapeToRange(int currentId, int lnkItemsCount)
        {
            int newId = base.MapShapeToRange(currentId, lnkItemsCount);
                       
            // It means that current identifier was moved to the beginning of the new range.
            if (newId != currentId)
               mShapeIdChages.Add(newId);

            return newId;
        }

        internal override int GetNextShapeId(ShapeBase shape)
        {
            if (gShapeIdMapping.ContainsKey(shape.Id))
                return gShapeIdMapping[shape.Id];

            return base.GetNextShapeId(shape);
        }

        /// <summary>
        /// Attached headers to response object.
        /// </summary>
        internal IDictionary<string, string> AttachedHeaders
        {
            get { return mAttachedHeaders; }
        }

        /// <summary>
        /// Stores directly changed shape identifier value on the document level. 
        /// </summary>
        internal HashSetGeneric<int> ShapeIdChages
        {
            get { return mShapeIdChages; }
        }

        /// <summary>
        /// Map shape identifiers to new values.
        /// </summary>
        internal static IntToIntDictionary ShapeIdMapping
        {
            get { return gShapeIdMapping; }
        }        
        
        /// <summary>
        /// Initial value for shape identifier in the document.
        /// </summary>
        internal static int InitialShapeId = 0;
        
        private readonly Dictionary<string, string> mAttachedHeaders = new Dictionary<string, string>();
        private readonly HashSetGeneric<int> mShapeIdChages = new HashSetGeneric<int>();

        private static readonly IntToIntDictionary gShapeIdMapping = new IntToIntDictionary();
    }
}
#endif
