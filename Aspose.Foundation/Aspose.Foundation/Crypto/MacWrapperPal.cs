// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2016 by Alexey Butalov

using Aspose.JavaAttributes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Aspose.Crypto
{
    /// <summary>
    /// This class wraps a BouncyCastle's IMac object and exposes Aspose.Crypto IMac inerface so we don't pass around a BouncyCastle object in AW code.
    /// </summary>
    [JavaManual("Platform abstraction for cryptography library. Manual porting by design.")]
    public class MacWrapperPal : Aspose.Crypto.IMac
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public MacWrapperPal(Org.BouncyCastle.Crypto.IMac mac)
        {
            mMac = mac;
        }

        /// <summary>
        /// Initializes underlying MAC with the specified key.
        /// </summary>
        public void Init(byte[] key)
        {
            mMac.Init(new KeyParameter(key));
        }

        /// <summary>
        /// Computes hash for the specified content.
        /// </summary>
        public byte[] ComputeHash(byte[] content)
        {
            byte[] hash = new byte[mMac.GetMacSize()];

            mMac.BlockUpdate(content, 0, content.Length);
            mMac.DoFinal(hash, 0);

            return hash;
        }

        /// <summary>
        /// Gets MAC size.
        /// </summary>
        public int Size
        {
            get { return mMac.GetMacSize(); }
        }
        
        /// <summary>
        /// Message authentication code algorithm (MAC).
        /// </summary>
        private readonly Org.BouncyCastle.Crypto.IMac mMac;
    }
}
