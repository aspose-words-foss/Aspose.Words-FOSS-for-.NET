// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/10/2011 by Alexey Morozov

using System;
using System.IO;
using Aspose.IO;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Reads and parses EncryptionInfo stream.
    /// </summary>
    internal class EncryptionInfo : EncryptionInfoReader
    {
        internal EncryptionInfo(BinaryReader encryptionInfoReader)
        {
            // Copy rest of stream.
            MemoryStream xmlStream = new MemoryStream();
            StreamUtil.CopyStream(encryptionInfoReader.BaseStream, xmlStream);

            NrxXmlReader reader = new NrxXmlReader(xmlStream);

            string tag = reader.LocalName;
            while(reader.ReadChild(tag))
            {
                switch(reader.LocalName)
                {
                    case "keyData":
                        ReadKeyData(reader);
                        break;
                    case "dataIntegrity":
                        ReadDataIntergity(reader);
                        break;
                    case "keyEncryptors":
                        // This is container element for keyEncryptor elements. Because we anticipate only one keyEncryptor we can skip this element.
                        break;
                    case "keyEncryptor":
                        // This is container element for encryptedKey elements. Actual data is read below.
                        break;
                    case "encryptedKey":
                        Debug.Assert(mPasswordKeyEncryptor == null);
                        mPasswordKeyEncryptor = new PasswordKeyEncryptor(reader);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown tag name.");
                }
            }
        }

        /// <summary>
        /// Reads 'keyData' element.
        /// </summary>
        /// <param name="xmlReader"></param>
        private void ReadKeyData(NrxXmlReader xmlReader)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                if (!ProcessTag(xmlReader))
                    throw new InvalidOperationException("Unknown tag name.");
            }
        }

        /// <summary>
        /// Reads 'dataIntergity' element.
        /// </summary>
        /// <param name="reader"></param>
        private void ReadDataIntergity(NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "encryptedHmacKey":
                        mEncryptedHmacKey = Convert.FromBase64String(reader.Value);
                        break;
                    case "encryptedHmacValue":
                        mEncryptedHmacValue = Convert.FromBase64String(reader.Value);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown tag name.");
                }
            }
        }

        /// <summary>
        /// Specifies an encrypted key utilized in calculating the encryptedHmacValue.
        /// </summary>
        internal byte[] EncryptedHmacKey
        {
            get { return mEncryptedHmacKey; }
        }

        /// <summary>
        /// Specifies an HMAC derived from the encryptedHmacKey and the encrypted data.
        /// </summary>
        internal byte[] EncryptedHmacValue
        {
            get { return mEncryptedHmacValue; }
        }

        /// <summary>
        /// Specifies the parameters used to encrypt an intermediate key, which is used to perform the final encryption of the document.
        /// </summary>
        internal PasswordKeyEncryptor PasswordKeyEncryptor
        {
            get { return mPasswordKeyEncryptor; }
        }

        private byte[] mEncryptedHmacKey;
        private byte[] mEncryptedHmacValue;
        private readonly PasswordKeyEncryptor mPasswordKeyEncryptor;
    }
}
