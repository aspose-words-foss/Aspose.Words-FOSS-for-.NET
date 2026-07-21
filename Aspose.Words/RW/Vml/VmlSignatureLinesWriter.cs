// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/04/2012 by Serkov Igor

using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'o:signatureline' sub-element of v:shape.
    /// </summary>
    internal class VmlSignatureLinesWriter
    {

        internal VmlSignatureLinesWriter(NrxXmlBuilder builder)
        {
            mBuilder = builder;
        }

        /// <summary>
        /// Add 'o:signatureline' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            mAttrCount++;
            switch (key)
            {
                case ShapeAttr.SigSetupAddlXml:
                    mAddlXml = (string) value;
                    break;
                case ShapeAttr.SigSetupAllowComments:
                    mAllowComments = (bool) value;
                    break;
                case ShapeAttr.SigSetupId:
                    mId = (string) value;
                    break;
                case ShapeAttr.IsSignatureLine:
                    mIsSignatureLine = (bool) value;
                    break;
                case ShapeAttr.SigSetupProvId:
                    mProvId = (string) value;
                    break;
                case ShapeAttr.SigSetupShowSignDate:
                    mShowDate = (bool) value;
                    break;
                case ShapeAttr.SigSetupSignInst:
                    mInstructions = (string) value;
                    break;
                case ShapeAttr.SigSetupProvUrl:
                    mProvUrl = (string) value;
                    break;
                case ShapeAttr.SigSetupSuggSigner:
                    mSuggestedSigner = (string) value;
                    break;
                case ShapeAttr.SigSetupSuggSigner2:
                    mSuggestedSigner2 = (string) value;
                    break;
                case ShapeAttr.SigSetupSuggSignerEmail:
                    mSuggestedSignereMail = (string) value;
                    break;
                case ShapeAttr.SigSetupSignInstSet:
                    mInstructionsSet = (bool) value;
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'o:signatureline' element.
        /// </summary>
        internal void Write()
        {
            if(mAttrCount == 0)
                return;
            mBuilder.StartElement("o:signatureline");
            mBuilder.WriteAttribute("o:addlxml", mAddlXml);
            mBuilder.WriteAttribute("allowcomments", mAllowComments);
            mBuilder.WriteAttribute("id", mId);
            mBuilder.WriteAttribute("issignatureline", mIsSignatureLine);
            mBuilder.WriteAttribute("provid", mProvId);
            mBuilder.WriteAttribute("showsigndate", mShowDate);
            mBuilder.WriteAttribute("o:signinginstructions", mInstructions);
            mBuilder.WriteAttribute("signinginstructionsset", mInstructionsSet);
            mBuilder.WriteAttribute("o:sigprovurl", mProvUrl);
            mBuilder.WriteAttribute("o:suggestedsigner", mSuggestedSigner);
            mBuilder.WriteAttribute("o:suggestedsigner2", mSuggestedSigner2);
            mBuilder.WriteAttribute("o:suggestedsigneremail", mSuggestedSignereMail);
            mBuilder.EndElement();
        }


        private readonly NrxXmlBuilder mBuilder;

        private string mAddlXml;
        private bool mAllowComments;
        private string mId;
        private string mProvId;
        private bool mShowDate;
        private string mInstructions;
        private string mProvUrl;
        private string mSuggestedSigner;
        private string mSuggestedSigner2;
        private string mSuggestedSignereMail;
        private bool mIsSignatureLine;
        private bool mInstructionsSet;

        private int mAttrCount = 0;
    }
}
