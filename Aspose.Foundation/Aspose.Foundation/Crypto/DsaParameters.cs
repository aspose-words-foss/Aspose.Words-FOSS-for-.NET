// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/02/2020 by Alexander Dudin

namespace Aspose.Crypto
{
    /// <summary>
    /// DSA algorithm parameters.
    /// </summary>
    public struct DsaParameters
    {
        /// <summary>
        /// P parameter of the DSA algorithm.
        /// </summary>
        public byte[] P { get; set; }

        /// <summary>
        /// Q parameter of the DSA algorithm.
        /// </summary>
        public byte[] Q { get; set; }

        /// <summary>
        /// G parameter of the DSA algorithm.
        /// </summary>
        public byte[] G { get; set; }

        /// <summary>
        /// Y parameter of the DSA algorithm (public key).
        /// </summary>
        public byte[] Y { get; set; }

        /// <summary>
        /// X parameter of the DSA algorithm (private key).
        /// </summary>
        public byte[] X { get; set; }
    }
}
