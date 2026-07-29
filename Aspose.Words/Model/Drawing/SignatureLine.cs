// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Alexey Morozov

using System;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    // AM. There is one possible threat now. Word provides rendered images for both valid and invalid signature line states.
    // It seems to be OK to use them as is but consider following scenario:
    // Hacker injected valid image instead of invalid image into digital signature.
    // Digital signature is violated of course and it will be shown in both MS Word and Aspose.Words.
    // But what if user just renders such document to PDF (or any format which not support digital signatures)?
    // Render engine gets SignatureLine.ImageBytesInvalid (but it is changed by hacker!),
    // prints out this image and output PDF looks like valid document.
    //
    // The only way to avoid this situation is to render valid/invalid images by render engine and do not use Word prepared images.
    // We need to get Shape.ImageData.ImageBytes as template (placeholder) and render either
    // user selected signature DigitalSiagnture.ImageBytes in case signature line is valid or
    // some picture that indicates that signature line is invalid.

    /// <summary>
    /// Provides access to signature line properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    public class SignatureLine
    {
        internal SignatureLine(Shape parent)
        {
            mParent = parent;

            string sigSetupId = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupId);
            if (!StringUtil.HasChars(sigSetupId))
                mParent.SetShapeAttrInternal(ShapeAttr.SigSetupId, Guid.NewGuid().ToString("B").ToUpper());

            // The value {00000000-0000-0000-0000-000000000000} is Word reserved for default signature provider:
            // https://msdn.microsoft.com/en-us/library/ff531802(v=office.12).aspx
            string providerId = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupProvId);
            if (!StringUtil.HasChars(providerId))
                mParent.SetShapeAttrInternal(ShapeAttr.SigSetupProvId, "{00000000-0000-0000-0000-000000000000}");
        }

         /// <summary>
        /// Generates new signature line image and places it into the shape.
        /// </summary>
        internal void UpdateShapeImage()
        {
        }

        /// <summary>
        /// Gets or sets suggested signer of the signature line.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Signer
        {
            get
            {
                string result = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupSuggSigner);
                return (result != null) ? result : string.Empty;
            }
            set
            {
                if (Signer != value)
                {
                    if (StringUtil.HasChars(value))
                        mParent.SetShapeAttrInternal(ShapeAttr.SigSetupSuggSigner, value);
                    else
                        mParent.RemoveShapeAttrInternal(ShapeAttr.SigSetupSuggSigner);
                    UpdateShapeImage();
                }
            }
        }

        /// <summary>
        /// Gets or sets suggested signer's title (for example, Manager).
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string SignerTitle
        {
            get
            {
                string result = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupSuggSigner2);
                return (result != null) ? result : string.Empty;
            }
            set
            {
                if (SignerTitle != value)
                {
                    if (StringUtil.HasChars(value))
                        mParent.SetShapeAttrInternal(ShapeAttr.SigSetupSuggSigner2, value);
                    else
                        mParent.RemoveShapeAttrInternal(ShapeAttr.SigSetupSuggSigner2);
                    UpdateShapeImage();
                }
            }
        }

        /// <summary>
        /// Gets or sets suggested signer's e-mail address.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Email
        {
            get
            {
                string result = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupSuggSignerEmail);
                return (result != null) ? result : string.Empty;
            }
            set
            {
                if (StringUtil.HasChars(value))
                    mParent.SetShapeAttrInternal(ShapeAttr.SigSetupSuggSignerEmail, value);
                else
                    mParent.RemoveShapeAttrInternal(ShapeAttr.SigSetupSuggSignerEmail);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that default instructions is shown in the Sign dialog.
        /// Default value for this property is <c>true</c>.
        /// </summary>
        public bool DefaultInstructions
        {
            get { return !(bool)mParent.FetchShapeAttrInternal(ShapeAttr.SigSetupSignInstSet); }
            set
            {
                mParent.SetShapeAttrInternal(ShapeAttr.SigSetupSignInstSet, !value);
                if (value)
                    mParent.RemoveShapeAttrInternal(ShapeAttr.SigSetupSignInst);
            }
        }

        /// <summary>
        /// Gets or sets instructions to the signer that are displayed on signing the signature line.
        /// This property is ignored if <see cref="DefaultInstructions"/> is set.
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Instructions
        {
            get
            {
                string result = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupSignInst);
                return (result != null) ? result : string.Empty;
            }
            set
            {
                if (StringUtil.HasChars(value))
                    mParent.SetShapeAttrInternal(ShapeAttr.SigSetupSignInst, value);
                else
                    mParent.RemoveShapeAttrInternal(ShapeAttr.SigSetupSignInst);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the signer can add comments in the Sign dialog.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        public bool AllowComments
        {
            get { return (bool)mParent.FetchShapeAttrInternal(ShapeAttr.SigSetupAllowComments); }
            set { mParent.SetShapeAttrInternal(ShapeAttr.SigSetupAllowComments, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating that sign date is shown in the signature line.
        /// Default value for this property is <c>true</c>.
        /// </summary>
        public bool ShowDate
        {
            get { return (bool)mParent.FetchShapeAttrInternal(ShapeAttr.SigSetupShowSignDate); }
            set { mParent.SetShapeAttrInternal(ShapeAttr.SigSetupShowSignDate, value); }
        }

        /// <summary>
        /// Gets or sets identifier for this signature line.
        /// <p>This identifier can be associated with a digital signature, when signing document using <see cref="DigitalSignatureUtil"/>.
        /// This value must be unique and by default it is randomly generated new Guid
        /// <ms> (<see cref="Guid.NewGuid"/>)</ms>
        /// <cpp> (<see cref="Guid.NewGuid"/>)</cpp>.</p>
        /// </summary>
        public Guid Id
        {
            get
            {
                string sigSetupId = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupId);
                return new Guid(sigSetupId);
            }
            set
            {
                mParent.SetShapeAttrInternal(ShapeAttr.SigSetupId, value.ToString("B").ToUpper());
            }
        }

        /// <summary>
        /// Gets or sets signature provider identifier for this signature line.
        /// Default value is "{00000000-0000-0000-0000-000000000000}".
        /// </summary>
        /// <remarks>
        /// <para>The cryptographic service provider (CSP) is an independent software module that actually performs
        /// cryptography algorithms for authentication, encoding, and encryption. MS Office reserves the value
        /// of {00000000-0000-0000-0000-000000000000} for its default signature provider.</para>
        /// <para>The GUID of the additionally installed provider should be obtained from the documentation shipped with the provider.</para>
        /// <para>In addition, all the installed cryptographic providers are enumerated in windows registry.
        /// It can be found in the following path: HKLM\SOFTWARE\Microsoft\Cryptography\Defaults\Provider.
        /// There is a key name "CP Service UUID" which corresponds to a GUID of signature provider.</para>
        /// </remarks>
        public Guid ProviderId
        {
            get
            {
                string sigSetupProvId = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupProvId);
                return new Guid(sigSetupProvId);
            }
            set
            {
                mParent.SetShapeAttrInternal(ShapeAttr.SigSetupProvId, value.ToString("B").ToUpper());
            }
        }

        /// <summary>
        /// Digital signature used to sign this signature line or <c>null</c> if signature line is not signed.
        /// </summary>
        /// <remarks>
        /// Don't be confused here. DigitalSignature and SignatureLine are different things.
        /// SignatureLine is just placeholder that can be signed and till it's not signed this property is null.
        /// DigitalSignature is real digital signature that signs document content and linked to this SignatureLine.
        ///
        /// AM.  I think this won't be accessed often, I didn't implement caching.
        /// </remarks>
        internal DigitalSignature DigitalSignature
        {
            get
            {
                Document doc = ((Document)mParent.Document);
                string setupId = (string)mParent.GetDirectShapeAttrInternal(ShapeAttr.SigSetupId);

                return doc.DigitalSignatures.GetBySetupId(setupId);
            }
        }

        /// <summary>
        /// Indicates that signature line is signed by digital signature.
        /// </summary>
        public bool IsSigned
        {
            get { return DigitalSignature != null; }
        }

        /// <summary>
        /// Indicates that signature line is signed by digital signature and this digital signature is valid.
        /// </summary>
        public bool IsValid
        {
            get { return IsSigned && DigitalSignature.IsValid; }
        }

        /// <summary>
        /// Returns image bytes depending on signature line state.
        /// </summary>
        internal byte[] ImageBytes
        {
            get
            {
                if (IsValid)
                {
                    // Signed and valid signature line.
                    byte[] image = DigitalSignature.ImageBytesValid;
                    return image;
                }
                else if (IsSigned)
                {
                    throw new NotSupportedException("FOSS");
                }
                else
                {
                    // Not signed signature line. Return parent's image bytes.
                    return mParent.ImageData.ImageBytes;
                }
            }
        }

        /// <summary>
        /// Returns parent shape of this signature line.
        /// </summary>
        internal Shape Parent
        {
            get { return mParent; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Shape mParent;
    }
}

