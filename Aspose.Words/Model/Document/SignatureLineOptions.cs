// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2015 by Alexander Zhiltsov

namespace Aspose.Words
{
    /// <summary>
    /// Allows to specify options for signature line being inserted. Used in <see cref="DocumentBuilder"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    public class SignatureLineOptions
    {
        /// <summary>
        /// Gets or sets suggested signer of the signature line.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Signer
        {
            get { return mSigner; }
            set { mSigner = value; }
        }

        /// <summary>
        /// Gets or sets suggested signer's title.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string SignerTitle
        {
            get { return mSignerTitle; }
            set { mSignerTitle = value; }
        }

        /// <summary>
        /// Gets or sets suggested signer's e-mail address.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that default instructions is shown in the Sign dialog.
        /// Default value for this property is <c>true</c>.
        /// </summary>
        public bool DefaultInstructions
        {
            get { return mDefaultInstructions; }
            set
            {
                mDefaultInstructions = value;
                if (value)
                    mInstructions = string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets instructions to the signer that are displayed on signing the signature line.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Instructions
        {
            get { return mInstructions; }
            set { mInstructions = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the signer can add comments in the Sign dialog.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        public bool AllowComments
        {
            get { return mAllowComments; }
            set { mAllowComments = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that sign date is shown in the signature line.
        /// Default value for this property is <c>true</c>.
        /// </summary>
        public bool ShowDate
        {
            get { return mShowDate; }
            set { mShowDate = value; }
        }

        private string mSigner = string.Empty;
        private string mSignerTitle = string.Empty;
        private string mEmail = string.Empty;
        private bool mDefaultInstructions = true;
        private string mInstructions = string.Empty;
        private bool mAllowComments;
        private bool mShowDate = true;
    }
}
