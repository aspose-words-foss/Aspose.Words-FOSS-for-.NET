// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2011 by Alexey Morozov
using System;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements reading of 'keyEncryptor' element.
    /// </summary>
    internal class PasswordKeyEncryptor : EncryptionInfoReader
    {
        /// <summary>
        /// Reads PasswordKeyEncryptor data.
        /// </summary>
        /// <param name="reader"></param>
        internal PasswordKeyEncryptor(NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (!ProcessTag(reader))
                {
                    switch (reader.LocalName)
                    {
                        case "encryptedVerifierHashInput":
                            mEncryptedVerifierHashInput = Convert.FromBase64String(reader.Value);
                            break;
                        case "encryptedVerifierHashValue":
                            mEncryptedVerifierHashValue = Convert.FromBase64String(reader.Value);
                            break;
                        case "encryptedKeyValue":
                            mEncryptedKeyValue = Convert.FromBase64String(reader.Value);
                            break;
                        default:
                            throw new InvalidOperationException("Unknown tag name: " + reader.LocalName);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies encrypted verifier hash input for a PasswordKeyEncryptor used in password verification.
        /// </summary>
        internal byte[] EncryptedVerifierHashInput
        {
            get { return mEncryptedVerifierHashInput; }
        }

        /// <summary>
        /// Specifies encrypted verifier hash value for a PasswordKeyEncryptor used in password verification.
        /// </summary>
        internal byte[] EncryptedVerifierHashValue
        {
            get { return mEncryptedVerifierHashValue; }
        }

        /// <summary>
        /// Specifies encrypted form of the intermediate key.
        /// </summary>
        internal byte[] EncryptedKeyValue
        {
            get { return mEncryptedKeyValue; }
        }

        private readonly byte[] mEncryptedVerifierHashInput;
        private readonly byte[] mEncryptedVerifierHashValue;
        private readonly byte[] mEncryptedKeyValue;
    }

}
