// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2008 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies write protection settings for a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/protect-or-encrypt-a-document/">Protect or Encrypt a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Write protection specifies whether the author has recommended that 
    /// the document is to be opened as read-only and/or require a password to modify a document.</para>
    /// 
    /// <para>Write protection is different from document protection. Write protection is specified in 
    /// Microsoft Word in the options of the Save As dialog box.</para>
    /// 
    /// <para>You do not create instances of this class directly. You access document protection settings
    /// via the <see cref="Document.WriteProtection"/> property.</para>
    /// 
    /// </remarks>
    public class WriteProtection
    {
        /// <summary>
        /// No public ctor.
        /// </summary>
        internal WriteProtection()
        {
        }

        /// <summary>
        /// Makes a deep clone.
        /// </summary>
        internal WriteProtection Clone()
        {
            WriteProtection lhs = (WriteProtection)MemberwiseClone();
            lhs.mPasswordHash = mPasswordHash.Clone();
            return lhs;
        }

        /// <summary>
        /// Specifies whether the document author has recommended that the document be opened as read-only.
        /// </summary>
        public bool ReadOnlyRecommended
        {
            get { return mReadOnlyRecommended; }
            set { mReadOnlyRecommended = value; }
        }

        /// <summary>
        /// Sets the write protection password for the document. 
        /// </summary>
        /// <remarks>
        /// <para>If a password is set, Microsoft Word will require the user to enter it or open 
        /// the document as read-only.</para>
        /// </remarks>
        /// <param name="password">The password to set. Cannot be <c>null</c>, but can be an empty string.</param>
        public void SetPassword(string password)
        {
            ArgumentUtil.CheckNotNull(password, "password");

            // RK This is unfortunate, but we have to store write protection password as clear text
            // because that's what MS Word does in DOC files.
            mPassword = password;

            // Clear the DOCX hash, it will be calculated on demand due to performance reasons.
            mPasswordHash.Hash = null;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified password is the same as the write-protection password the document was protected with.
        /// If document is not write-protected with password then returns <c>false</c>.
        /// </summary>
        public bool ValidatePassword(string password)
        {
            if (StringUtil.HasChars(password))
            {
                if (mPasswordHash.Hash != null)
                {
                    PasswordHash hash = new PasswordHash();
                    hash.CalculateWriteProtectionHash(password, mPasswordHash);

                    return ArrayUtil.IsArrayEqual(mPasswordHash.Hash, hash.Hash);
                }
                else
                {
                    return mPassword == password;
                }
            }

            return false;
        }

        /// <summary>
        /// Has to be internal because we don't want the users to retrieve the password.
        /// When this is an empty string, the password might still be present (hash only) if document 
        /// was read from DOCX for example.
        /// </summary>
        internal string GetPassword()
        {
            return mPassword;
        }

        /// <summary>
        /// Specifies write protection password. 
        /// When not empty, write protection is turned on.
        /// Never null.
        /// </summary>
        internal PasswordHash PasswordHash
        {
            get { return mPasswordHash; }
        }

        /// <summary>
        /// Returns <c>true</c> when a write protection password is set.
        /// </summary>
        public bool IsWriteProtected
        {
            get { return (StringUtil.HasChars(mPassword) || !mPasswordHash.IsEmpty); }
        }

        /// <summary>
        /// Updates DOCX hash from the password string if needed. Safe to call many times.
        /// This is a CPU intensive operation and therefore called only when needed.
        /// </summary>
        internal void UpdateDocxHash()
        {
            if (StringUtil.HasChars(mPassword) && mPasswordHash.IsEmpty)
                mPasswordHash.CalculateWriteProtectionHash(mPassword, mPasswordHash);
        }

        private bool mReadOnlyRecommended;
        private string mPassword = "";
        private PasswordHash mPasswordHash = new PasswordHash();
    }
}
