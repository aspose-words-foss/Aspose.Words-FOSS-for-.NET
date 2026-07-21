// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2006 by Roman Korchagin

using Aspose.Crypto;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Stores document protection settings.
    /// 
    /// Note that DOCX stores hash of hash of DOC password. And there is no way we can restore 
    /// original password hash for the document from the hash's hash. Therefore the use cases are as follows:
    /// 
    /// 1. User opens a DOCX file and saves as DOCX. The full DOCX hash is preserved.
    /// 2. User opens a DOCX file and saves as DOC/RTF/WordML, the password is lost (like in MS Word). 
    /// 3. User opens a DOC file and saves as DOCX. Full DOCX hash is calculated during save.
    /// 4. User protects a document with a password. We set the DOC hash only initially. Full DOCX hash is 
    /// calculated if document is saved as DOCX.
    /// </summary>
    internal class DocumentProtection 
    {
        /// <summary>
        /// Performs a deep clone.
        /// </summary>
        internal DocumentProtection Clone()
        {
            DocumentProtection lhs = (DocumentProtection)MemberwiseClone();
            lhs.mPasswordHash = mPasswordHash.Clone();
            return lhs;
        }

        /// <summary>
        /// Updates DOCX hash from the legacy hash if needed. Safe to call many times.
        /// This is a CPU intensive operation and therefore called only when needed.
        /// </summary>
        internal void UpdateDocxHash()
        {
            // Calculate DOCX hash on demand.
            if ((mLegacyHash != 0) && mPasswordHash.IsEmpty)
                mPasswordHash.CalculateDocumentProtectionHash(mLegacyHash, mPasswordHash);
        }
        
        /// <summary>
        /// This is the "new" method for protection that allows to protect with or without a password.
        /// </summary>
        internal void Protect(ProtectionType type, string password)
        {
            EnforcedProtectionType = type;
            mLegacyHash = CalculateLegacyHash(password);
            // Clear the DOCX hash, it will be calculated on demand.
            mPasswordHash.Hash = null;
        }

        /// <summary>
        /// This is the "old" method for protecting that was used before we knew the legacy hash algorithm.
        /// It protects a document. If there is an existing password, does not change it.
        /// If there is no password, assigns some hardcoded password that we don't really know.
        /// </summary>
        internal void ProtectOld(ProtectionType type)
        {
            EnforcedProtectionType = type;

            // When protecting a doc that does not have a password yet, assign some password hash.
            if ((mEdit != ProtectionType.NoProtection) && (mLegacyHash == 0) && mPasswordHash.IsEmpty)
            {
                mLegacyHash = 0x56AE3201;
                mPasswordHash.Hash = null;
            }
        }

        /// <summary>
        /// Turns the protection off.
        /// </summary>
        internal void Unprotect()
        {
            EnforcedProtectionType = ProtectionType.NoProtection;
        }

        /// <summary>
        /// Returns true if the specified password is the same as the password the document was protected with.
        /// </summary>
        internal bool ValidatePassword(string password)
        {
            int legacyHash = CalculateLegacyHash(password);
            // If the password is empty then the hash equals to 0 and the document shouldn't be unprotected.
            if (legacyHash == 0)
                return false;

            PasswordHash hash = new PasswordHash();
            // Calculate password hash only if it is already calculated for the document's password.
            // This means that input document is DOCX and does not have legacy hash, so we need to compare OOXML hash.
            if (!mPasswordHash.IsEmpty)
                hash.CalculateDocumentProtectionHash(legacyHash, mPasswordHash);

            bool result = (mLegacyHash == legacyHash) ||
                (!mPasswordHash.IsEmpty && !hash.IsEmpty && (HashUtil.GetSHA512Hash(mPasswordHash.Hash) == HashUtil.GetSHA512Hash(hash.Hash)));

            if (result)
                return true;

            // WORDSNET-12016 If above hash check is failed do additional try using another initial bytes generation method.
            if (!mPasswordHash.IsEmpty)
                hash.CalculateWriteProtectionHash(password, mPasswordHash);

            return !mPasswordHash.IsEmpty && !hash.IsEmpty && (HashUtil.GetSHA512Hash(mPasswordHash.Hash) == HashUtil.GetSHA512Hash(hash.Hash));
        }

        /// <summary>
        /// Calculates hash code of the document protection password.
        /// Algorithm taken from ECMA TC 45 § 2.15.1.28
        /// </summary>
        internal static int CalculateLegacyHash(string password)
        {
            // If password is empty, its hash code is 0.
            if (!StringUtil.HasChars(password))
                return 0x0;

            // Truncate password to maximum significant length.
            if (password.Length > MaxPasswordLength)
                password = password.Substring(0, MaxPasswordLength);

            int len = password.Length;

            byte[] pwdBytes = new byte[len];

            for (int i = 0; i < len; i++)
            {
                // Take Unicode char from string.
                ushort charWord = password[i];

                // Take low byte as significant byte.
                byte charByte = (byte)charWord;

                // If low byte is 0, then take high byte.
                if (charByte == 0)
                    charByte = (byte)(charWord >> 16);

                // Store char byte to password bytes array.
                pwdBytes[i] = charByte;
            }

            // Calculate high-order word of the key.
            int keyHi = gInitialCodes[len - 1];

            for (int i = 0; i < len; i++)
            {
                int[] encryptionRow = gEncryptionMatrix[MaxPasswordLength - len + i];
                byte b = pwdBytes[i];

                for (int bitIndex = 0; bitIndex < 7; bitIndex++)
                {
                    if ((b & 1) != 0)
                        keyHi ^= encryptionRow[bitIndex];

                    b >>= 1;
                }
            }

            // Calculate low-order word of the key.
            int keyLo = 0;

            for (int i = len - 1; i >= 0; i--)
            {
                keyLo = ProcessKeyLow(keyLo, pwdBytes[i]);
            }

            keyLo = ProcessKeyLow(keyLo, len) ^ 0xce4b;

            return ((keyHi << 16) | (keyLo & 0xffff));
        }

        private static int ProcessKeyLow(int keyLo, int operand)
        {
            return ((((keyLo >> 14) & 0x0001) | (keyLo << 1) & 0x7FFF) ^ operand);
        }

        /// <summary>
        /// Gets or sets editing restrictions. Helps prevent unintentional editing changes as specified.
        /// Note that this setting has no effect unless <see cref="Enforcement"/> is set,
        /// therefore if you need to get actual status, use <see cref="EnforcedProtectionType"/>.
        /// </summary>
        internal ProtectionType Edit
        {
            get { return mEdit; }
            set { mEdit = value; }
        }

        /// <summary>
        /// Gets or sets whether the editing restrictions are currently being enforced for this document.
        /// </summary>
        internal bool Enforcement
        {
            get { return mEnforcement; }
            set { mEnforcement = value; }
        }

        /// <summary>
        /// This "ties" together the <see cref="Edit"/> and <see cref="Enforcement"/> properties.
        /// When getting, returns the "actual" protection setting. That is Edit + Enforcement.
        /// When setting, sets Edit and sets Enforcement = true.
        /// </summary>
        internal ProtectionType EnforcedProtectionType
        {
            get { return (mEnforcement) ? mEdit : ProtectionType.NoProtection; }
            set
            {
                mEdit = value;
                // RK NoProtection cannot be enforced in MS Word and it we should really set enforcement 
                // to false in this case so the selection in the protection combo box appears normally.
                mEnforcement = mEdit != ProtectionType.NoProtection;
            }
        }
        
        /// <summary>
        /// Only Allow Formatting With Unlocked Styles.
        /// Specifies if formatting restrictions are in effect for the document.
        /// </summary>
        internal bool Formatting
        {
            get { return mFormatting; }
            set { mFormatting = value; }
        }

        /// <summary>
        /// Restrict selections to occur only within form fields. 
        /// Not a WordML field, but available in binary DOC.
        /// </summary>
        internal bool SelectFormFieldsOnly
        {
            get { return mSelectFormFieldsOnly; }
            set { mSelectFormFieldsOnly = value; }
        }

        /// <summary>
        /// Allows the AutoFormat feature to override formatting restrictions.
        /// </summary>
        internal bool AutoFormatOverride
        {
            get { return mAutoFormatOverride; }
            set { mAutoFormatOverride = value; }
        }

        /// <summary>
        /// Gets or sets password key to unprotect this document from unintentional formatting/editing changes. 
        /// This is Word 97-2003 hash.
        /// </summary>
        internal int LegacyHash
        {
            get { return mLegacyHash; }
            set { mLegacyHash = value; }
        }
        
        /// <summary>
        /// This is the OOXML password hash.
        /// </summary>
        internal PasswordHash PasswordHash
        {
            get { return mPasswordHash; }
        }

        private ProtectionType mEdit = ProtectionType.NoProtection;
        private bool mEnforcement;
        private bool mFormatting;
        private bool mSelectFormFieldsOnly;
        private bool mAutoFormatOverride;
        private int mLegacyHash;
        private PasswordHash mPasswordHash = new PasswordHash();

        private const int MaxPasswordLength = 15;
        private static readonly int[] gInitialCodes = new int[] { 0xE1F0, 0x1D0F, 0xCC9C, 0x84C0, 0x110C, 0x0E10, 0xF1CE, 0x313E, 0x1872, 0xE139, 0xD40F, 0x84F9, 0x280C, 0xA96A, 0x4EC3 };
        private static readonly int[][] gEncryptionMatrix = new int[][] 
        {
            /* Last-14 */ new int[] {0xAEFC, 0x4DD9, 0x9BB2, 0x2745, 0x4E8A, 0x9D14, 0x2A09},
            /* Last-13 */ new int[] {0x7B61, 0xF6C2, 0xFDA5, 0xEB6B, 0xC6F7, 0x9DCF, 0x2BBF},
            /* Last-12 */ new int[] {0x4563, 0x8AC6, 0x05AD, 0x0B5A, 0x16B4, 0x2D68, 0x5AD0},
            /* Last-11 */ new int[] {0x0375, 0x06EA, 0x0DD4, 0x1BA8, 0x3750, 0x6EA0, 0xDD40},
            /* Last-10 */ new int[] {0xD849, 0xA0B3, 0x5147, 0xA28E, 0x553D, 0xAA7A, 0x44D5},
            /* Last-9  */ new int[] {0x6F45, 0xDE8A, 0xAD35, 0x4A4B, 0x9496, 0x390D, 0x721A},
            /* Last-8  */ new int[] {0xEB23, 0xC667, 0x9CEF, 0x29FF, 0x53FE, 0xA7FC, 0x5FD9},
            /* Last-7  */ new int[] {0x47D3, 0x8FA6, 0x0F6D, 0x1EDA, 0x3DB4, 0x7B68, 0xF6D0},
            /* Last-6  */ new int[] {0xB861, 0x60E3, 0xC1C6, 0x93AD, 0x377B, 0x6EF6, 0xDDEC},
            /* Last-5  */ new int[] {0x45A0, 0x8B40, 0x06A1, 0x0D42, 0x1A84, 0x3508, 0x6A10},
            /* Last-4  */ new int[] {0xAA51, 0x4483, 0x8906, 0x022D, 0x045A, 0x08B4, 0x1168},
            /* Last-3  */ new int[] {0x76B4, 0xED68, 0xCAF1, 0x85C3, 0x1BA7, 0x374E, 0x6E9C},
            /* Last-2  */ new int[] {0x3730, 0x6E60, 0xDCC0, 0xA9A1, 0x4363, 0x86C6, 0x1DAD},
            /* Last-1  */ new int[] {0x3331, 0x6662, 0xCCC4, 0x89A9, 0x0373, 0x06E6, 0x0DCC},
            /* Last    */ new int[] {0x1021, 0x2042, 0x4084, 0x8108, 0x1231, 0x2462, 0x48C4}
        };
    }
}
