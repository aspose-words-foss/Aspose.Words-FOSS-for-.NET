// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2010 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.DigitalSignatures
{
    /// <summary>
    /// Provides a read-only collection of digital signatures attached to a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <see cref="Document.DigitalSignatures"/>
    /// </remarks>
    public class DigitalSignatureCollection : IEnumerable<DigitalSignature>
    {
        /// <summary>
        /// Returns <c>true</c> if all digital signatures in this collection are valid and the document has not been tampered with
        /// Also returns <c>true</c> if there are no digital signatures.
        /// Returns <c>false</c> if at least one digital signature is invalid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                foreach (DigitalSignature signature in mItems)
                {
                    if (!signature.IsValid)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets a document signature at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the signature.</param>
        public DigitalSignature this[int index]
        {
            get { return mItems[index]; }
        }

        /// <summary>
        /// This method is used only internally. From user side this collection can be only read.
        /// </summary>
        internal void Add(DigitalSignature signature)
        {
            mItems.Add(signature);

            // Collect visible signatures SetupIDs.
            if(signature.Visible)
            {
                // AM. I think only visible signatures have SetupID.
                mSignatureBySetupId.Add(signature.SetupId, signature);
            }
            else
            {
                // AM. Non-visible signatures must not have SetupID.
                Debug.Assert(signature.SetupId.Equals(Guid.Empty));
            }
        }

        /// <summary>
        /// Returns DigitalSignature object by given SetupID or null if not found.
        /// </summary>
        internal DigitalSignature GetBySetupId(string setupId)
        {
            if (!StringUtil.HasChars(setupId))
                return null;

            return mSignatureBySetupId[new Guid(setupId)];
        }

        /// <summary>
        /// Returns a dictionary <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<DigitalSignature> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<DigitalSignature> mItems = new List<DigitalSignature>();

        /// <summary>
        /// Provides quick access to visible signatures by SetupId.
        /// </summary>
        private readonly GuidToObjDictionary<DigitalSignature> mSignatureBySetupId = 
            new GuidToObjDictionary<DigitalSignature>();
    }
}
