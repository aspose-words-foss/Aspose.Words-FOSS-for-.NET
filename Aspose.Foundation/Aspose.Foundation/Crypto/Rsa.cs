// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Crypto
{
    /// <summary>
    /// Performs RSA encryption.
    /// </summary>
    public class Rsa
    {
        public Rsa(byte[] modulusBytes, byte[] exponentBytes)
        {
            mModulus = new BigInteger(modulusBytes);
            mExponent = new BigInteger(exponentBytes);
        }

        public BigInteger Modulus
        {
            get { return mModulus; }
        }

        public BigInteger Exponent
        {
            get { return mExponent; }
        }

        internal BigInteger mModulus;
        internal BigInteger mExponent;
    }

}
