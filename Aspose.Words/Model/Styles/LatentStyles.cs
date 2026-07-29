// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2007 by Vladimir Averkin
using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Latent styles provide a way to store style behavior properties which must be specified for all styles
    /// known to an application without requiring the store of the formatting properties.
    ///
    /// Latent styles refer to any set of style definitions known to an application
    /// which have not been included in the current document.
    ///
    /// In particular is needed for correct display of styles quick format list in Word 2007.
    /// We do not know where this properties are stored in DOC but they are present in DOCX and WordML.
    ///
    /// Specifies the properties and exceptions which shall be applied to a set of latent styles for
    /// this document.
    /// </summary>
    internal class LatentStyles
    {
        // Example:
        //    <w:latentStyles w:defLockedState="0" w:defUIPriority="0" w:defSemiHidden="0" w:defUnhideWhenUsed="0" w:defQFormat="0" w:count="267">

        /// <summary>
        /// Makes a deep clone.
        /// </summary>
        internal LatentStyles Clone()
        {
            LatentStyles latentStyles = (LatentStyles)MemberwiseClone();

            latentStyles.mItems = new SortedIntegerListGeneric<LatentStyle>();

            for (int i = 0; i < mItems.Count; i++)
            {
                LatentStyle latentStyle = mItems.GetByIndex(i);
                latentStyles.mItems.Add(mItems.GetKey(i), latentStyle.Clone());
            }

            return latentStyles;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified latent style exception has the same properties as default.
        /// </summary>
        public bool EqualsDefault(LatentStyle latentStyle)
        {
            return
                (latentStyle.Locked == DefaultLockedState) &&
                (latentStyle.QuickStyle == DefaultQuickFormat) &&
                (latentStyle.SemiHidden == DefaultSemiHidden) &&
                (latentStyle.UIPriority == DefaultUIPriority) &&
                (latentStyle.UnhideWhenUsed == DefaultUnhideWhenUsed);
        }

        /// <summary>
        /// Specifies the default setting for the locked element (§2.7.3.7) which shall be applied to
        /// any style made available by the hosting application which is not explicitly defined in the
        /// current document. This setting shall be overridden for every style for which a latent style
        /// exception (§2.7.3.8) exists.
        /// </summary>
        internal bool DefaultLockedState
        {
            get { return mDefaultLockedState; }
            set { mDefaultLockedState = value; }
        }

        /// <summary>
        /// Specifies the default setting for the qFormat element.
        /// </summary>
        internal bool DefaultQuickFormat
        {
            get { return mDefaultQuickFormat; }
            set { mDefaultQuickFormat = value; }
        }

        /// <summary>
        /// Specifies the default setting for the semiHidden element.
        /// </summary>
        internal bool DefaultSemiHidden
        {
            get { return mDefaultSemiHidden; }
            set { mDefaultSemiHidden = value; }
        }

        /// <summary>
        /// Specifies the default setting for the uiPriority element.
        /// </summary>
        internal int DefaultUIPriority
        {
            get { return mDefaultUIPriority; }
            set { mDefaultUIPriority = value; }
        }

        /// <summary>
        /// Specifies the default setting for the unhideWhenUsed element.
        /// </summary>
        internal bool DefaultUnhideWhenUsed
        {
            get { return mDefaultUnhideWhenUsed; }
            set { mDefaultUnhideWhenUsed = value; }
        }

        /// <summary>
        /// Specifies the number of known styles which shall be initialized to the current latent style
        /// defaults when this document is first processed.
        /// </summary>
        internal int KnownStylesCount
        {
            get { return mKnownStylesCount; }
            set { mKnownStylesCount = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if LatentStyles differs from the initial state.
        /// </summary>
        internal bool IsCustomized()
        {
            return !IsWord2003Default() &&
                   !IsWord2007Default() &&
                   !IsWord2013Default() &&
                   !IsWord2019Default();
        }

        private bool IsWord2003Default()
        {
            LatentStyles defLatentStyles = new LatentStyles();
            defLatentStyles.InitToWord2003Default();

            return Equals(defLatentStyles);
        }

        private bool IsWord2007Default()
        {
            LatentStyles defLatentStyles = new LatentStyles();
            defLatentStyles.InitToWord2007Default();

            return Equals(defLatentStyles);
        }

        private bool IsWord2013Default()
        {
            LatentStyles defLatentStyles = new LatentStyles();
            defLatentStyles.InitToWord2013Default();

            return Equals(defLatentStyles);
        }

        private bool IsWord2019Default()
        {
            LatentStyles defLatentStyles = new LatentStyles();
            defLatentStyles.InitToWord2019Default();

            return Equals(defLatentStyles);
        }

        /// <summary>
        /// Creates default set of latent styles exceptions as in Word 2003.
        /// </summary>
        internal void InitToWord2003Default()
        {
            mDefaultUIPriority = 0;
            mDefaultSemiHidden = false;
            mDefaultUnhideWhenUsed = false;

            mItems.Clear();

            AddDefault(StyleIdentifier.Normal, 0, false, false, true);
            AddDefault(StyleIdentifier.Heading1, 0, false, false, true);
            AddDefault(StyleIdentifier.Heading2, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading3, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading4, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading5, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading6, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading7, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading8, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading9, 0, true, true, true);
            AddDefault(StyleIdentifier.Caption, 0, true, true, true);
            AddDefault(StyleIdentifier.Title, 0, false, false, true);
            AddDefault(StyleIdentifier.Subtitle, 0, false, false, true);
            AddDefault(StyleIdentifier.Strong, 0, false, false, true);
            AddDefault(StyleIdentifier.Emphasis, 0, false, false, true);

            AddCommonDefaults();
        }

        /// <summary>
        /// Creates default set of latent styles exceptions for Word 2007.
        /// </summary>
        internal void InitToWord2007Default()
        {
            mItems.Clear();

            AddDefault(StyleIdentifier.Normal, 0, false, false, true);
            AddDefault(StyleIdentifier.Heading1, 9, false, false, true);
            AddDefault(StyleIdentifier.Heading2, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading3, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading4, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading5, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading6, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading7, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading8, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading9, 9, true, true, true);
            AddDefault(StyleIdentifier.Toc1, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc2, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc3, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc4, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc5, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc6, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc7, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc8, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc9, 39, true, true, false);
            AddDefault(StyleIdentifier.Caption, 35, true, true, true);
            AddDefault(StyleIdentifier.Title, 10, false, false, true);
            AddDefault(StyleIdentifier.DefaultParagraphFont, 1, true, true, false);
            AddDefault(StyleIdentifier.Subtitle, 11, false, false, true);
            AddDefault(StyleIdentifier.Strong, 22, false, false, true);
            AddDefault(StyleIdentifier.Emphasis, 20, false, false, true);
            AddDefault(StyleIdentifier.TableGrid, 59, false, false, false);

            AddCommonDefaults();
        }

        /// <summary>
        /// Safely adds a latent style exception. 
        /// If another item with the same style identifier exists, overwrites it.
        /// </summary>
        internal void Add(LatentStyle latentStyle)
        {
            mItems[(int)latentStyle.StyleIdentifier] = latentStyle;
        }

        /// <summary>
        /// Gets a latent style exception by style identifier.
        /// </summary>
        internal LatentStyle this[StyleIdentifier styleIdenitifier]
        {
            get { return mItems[(int)styleIdenitifier]; }
        }

        /// <summary>
        /// Gets a latent style exception by index.
        /// </summary>
        internal LatentStyle this[int index]
        {
            get { return mItems.GetByIndex(index); }
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        private void InitToWord2013Default()
        {
            mItems.Clear();

            AddDefault(StyleIdentifier.Normal, 0, false, false, true);
            AddDefault(StyleIdentifier.Heading1, 9, false, false, true);
            AddDefault(StyleIdentifier.Heading2, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading3, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading4, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading5, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading6, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading7, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading8, 9, true, true, true);
            AddDefault(StyleIdentifier.Heading9, 9, true, true, true);
            AddDefault(StyleIdentifier.Index1, 39, true, true, false);
            AddDefault(StyleIdentifier.Index2, 39, true, true, false);
            AddDefault(StyleIdentifier.Index3, 39, true, true, false);
            AddDefault(StyleIdentifier.Index4, 39, true, true, false);
            AddDefault(StyleIdentifier.Index5, 39, true, true, false);
            AddDefault(StyleIdentifier.Index6, 39, true, true, false);
            AddDefault(StyleIdentifier.Index7, 39, true, true, false);
            AddDefault(StyleIdentifier.Index8, 39, true, true, false);
            AddDefault(StyleIdentifier.Index9, 39, true, true, false);
            AddDefault(StyleIdentifier.Toc1, 35, true, true, true);
            AddDefault(StyleIdentifier.Toc2, 10, false, false, true);
            AddDefault(StyleIdentifier.Toc3, 1, true, true, false);
            AddDefault(StyleIdentifier.Toc4, 11, false, false, true);
            AddDefault(StyleIdentifier.Toc5, 22, false, false, true);
            AddDefault(StyleIdentifier.Toc6, 20, false, false, true);
            AddDefault(StyleIdentifier.Toc7, 39, false, false, false);
            AddDefault(StyleIdentifier.Toc8, 99, true, false, false);
            AddDefault(StyleIdentifier.Toc9, 1, false, false, true);
            AddDefault(StyleIdentifier.NormalIndent, 60, true, true, false);
            AddDefault(StyleIdentifier.FootnoteText, 61, true, true, false);
            AddDefault(StyleIdentifier.CommentText, 62, true, true, false);
            AddDefault(StyleIdentifier.Header, 63, true, true, false);
            AddDefault(StyleIdentifier.Footer, 64, true, true, false);
            AddDefault(StyleIdentifier.IndexHeading, 65, true, true, false);
            AddDefault(StyleIdentifier.Caption, 66, true, true, false);
            AddDefault(StyleIdentifier.TableOfFigures, 67, true, true, false);
            AddDefault(StyleIdentifier.EnvelopeAddress, 68, true, true, false);
            AddDefault(StyleIdentifier.EnvelopeReturn, 69, true, true, false);
            AddDefault(StyleIdentifier.FootnoteReference, 70, true, true, false);
            AddDefault(StyleIdentifier.CommentReference, 71, true, true, false);
            AddDefault(StyleIdentifier.LineNumber, 72, true, true, false);
            AddDefault(StyleIdentifier.PageNumber, 73, true, true, false);
            AddDefault(StyleIdentifier.EndnoteReference, 60, true, true, false);
            AddDefault(StyleIdentifier.EndnoteText, 61, true, true, false);
            AddDefault(StyleIdentifier.TableOfAuthorities, 62, true, true, false);
            AddDefault(StyleIdentifier.Macro, 63, true, true, false);
            AddDefault(StyleIdentifier.ToaHeading, 64, true, true, false);
            AddDefault(StyleIdentifier.List, 65, true, true, false);
            AddDefault(StyleIdentifier.ListBullet, 99, true, false, false);
            AddDefault(StyleIdentifier.ListNumber, 34, false, false, true);
            AddDefault(StyleIdentifier.List2, 29, false, false, true);
            AddDefault(StyleIdentifier.List3, 30, false, false, true);
            AddDefault(StyleIdentifier.List4, 66, true, true, false);
            AddDefault(StyleIdentifier.List5, 67, true, true, false);
            AddDefault(StyleIdentifier.ListBullet2, 68, true, true, false);
            AddDefault(StyleIdentifier.ListBullet3, 69, true, true, false);
            AddDefault(StyleIdentifier.ListBullet4, 70, true, true, false);
            AddDefault(StyleIdentifier.ListBullet5, 71, true, true, false);
            AddDefault(StyleIdentifier.ListNumber2, 72, true, true, false);
            AddDefault(StyleIdentifier.ListNumber3, 73, true, true, false);
            AddDefault(StyleIdentifier.ListNumber4, 60, true, true, false);
            AddDefault(StyleIdentifier.ListNumber5, 61, true, true, false);
            AddDefault(StyleIdentifier.Title, 62, true, true, false);
            AddDefault(StyleIdentifier.Closing, 63, true, true, false);
            AddDefault(StyleIdentifier.Signature, 64, true, true, false);
            AddDefault(StyleIdentifier.DefaultParagraphFont, 65, true, true, false);
            AddDefault(StyleIdentifier.BodyText, 66, true, true, false);
            AddDefault(StyleIdentifier.BodyTextInd, 67, true, true, false);
            AddDefault(StyleIdentifier.ListContinue, 68, true, true, false);
            AddDefault(StyleIdentifier.ListContinue2, 69, true, true, false);
            AddDefault(StyleIdentifier.ListContinue3, 70, true, true, false);
            AddDefault(StyleIdentifier.ListContinue4, 71, true, true, false);
            AddDefault(StyleIdentifier.ListContinue5, 72, true, true, false);
            AddDefault(StyleIdentifier.MessageHeader, 73, true, true, false);
            AddDefault(StyleIdentifier.Subtitle, 60, true, true, false);
            AddDefault(StyleIdentifier.Salutation, 61, true, true, false);
            AddDefault(StyleIdentifier.Date, 62, true, true, false);
            AddDefault(StyleIdentifier.BodyText1I, 63, true, true, false);
            AddDefault(StyleIdentifier.BodyText1I2, 64, true, true, false);
            AddDefault(StyleIdentifier.NoteHeading, 65, true, true, false);
            AddDefault(StyleIdentifier.BodyText2, 66, true, true, false);
            AddDefault(StyleIdentifier.BodyText3, 67, true, true, false);
            AddDefault(StyleIdentifier.BodyTextInd2, 68, true, true, false);
            AddDefault(StyleIdentifier.BodyTextInd3, 69, true, true, false);
            AddDefault(StyleIdentifier.BlockText, 70, true, true, false);
            AddDefault(StyleIdentifier.Hyperlink, 71, true, true, false);
            AddDefault(StyleIdentifier.FollowedHyperlink, 72, true, true, false);
            AddDefault(StyleIdentifier.Strong, 73, true, true, false);
            AddDefault(StyleIdentifier.Emphasis, 60, true, true, false);
            AddDefault(StyleIdentifier.DocumentMap, 61, true, true, false);
            AddDefault(StyleIdentifier.PlainText, 62, true, true, false);
            AddDefault(StyleIdentifier.EmailSignature, 63, true, true, false);
            AddDefault(StyleIdentifier.HtmlTopOfForm, 64, true, true, false);
            AddDefault(StyleIdentifier.HtmlBottomOfForm, 65, true, true, false);
            AddDefault(StyleIdentifier.NormalWeb, 66, true, true, false);
            AddDefault(StyleIdentifier.HtmlAcronym, 67, true, true, false);
            AddDefault(StyleIdentifier.HtmlAddress, 68, true, true, false);
            AddDefault(StyleIdentifier.HtmlCite, 69, true, true, false);
            AddDefault(StyleIdentifier.HtmlCode, 70, true, true, false);
            AddDefault(StyleIdentifier.HtmlDefinition, 71, true, true, false);
            AddDefault(StyleIdentifier.HtmlKeyboard, 72, true, true, false);
            AddDefault(StyleIdentifier.HtmlPreformatted, 73, true, true, false);
            AddDefault(StyleIdentifier.HtmlSample, 60, true, true, false);
            AddDefault(StyleIdentifier.HtmlTypewriter, 61, true, true, false);
            AddDefault(StyleIdentifier.HtmlVariable, 62, true, true, false);
            AddDefault(StyleIdentifier.TableNormal, 63, true, true, false);
            AddDefault(StyleIdentifier.CommentSubject, 64, true, true, false);
            AddDefault(StyleIdentifier.NoList, 65, true, true, false);
            AddDefault(StyleIdentifier.OutlineList1, 66, true, true, false);
            AddDefault(StyleIdentifier.OutlineList2, 67, true, true, false);
            AddDefault(StyleIdentifier.OutlineList3, 68, true, true, false);
            AddDefault(StyleIdentifier.TableSimple1, 69, true, true, false);
            AddDefault(StyleIdentifier.TableSimple2, 70, true, true, false);
            AddDefault(StyleIdentifier.TableSimple3, 71, true, true, false);
            AddDefault(StyleIdentifier.TableClassic1, 72, true, true, false);
            AddDefault(StyleIdentifier.TableClassic2, 73, true, true, false);
            AddDefault(StyleIdentifier.TableClassic3, 60, true, true, false);
            AddDefault(StyleIdentifier.TableClassic4, 61, true, true, false);
            AddDefault(StyleIdentifier.TableColorful1, 62, true, true, false);
            AddDefault(StyleIdentifier.TableColorful2, 63, true, true, false);
            AddDefault(StyleIdentifier.TableColorful3, 64, true, true, false);
            AddDefault(StyleIdentifier.TableColumns1, 65, true, true, false);
            AddDefault(StyleIdentifier.TableColumns2, 66, true, true, false);
            AddDefault(StyleIdentifier.TableColumns3, 67, true, true, false);
            AddDefault(StyleIdentifier.TableColumns4, 68, true, true, false);
            AddDefault(StyleIdentifier.TableColumns5, 69, true, true, false);
            AddDefault(StyleIdentifier.TableGrid1, 70, true, true, false);
            AddDefault(StyleIdentifier.TableGrid2, 71, true, true, false);
            AddDefault(StyleIdentifier.TableGrid3, 72, true, true, false);
            AddDefault(StyleIdentifier.TableGrid4, 73, true, true, false);
            AddDefault(StyleIdentifier.TableGrid5, 19, false, false, true);
            AddDefault(StyleIdentifier.TableGrid6, 21, false, false, true);
            AddDefault(StyleIdentifier.TableGrid7, 31, false, false, true);
            AddDefault(StyleIdentifier.TableGrid8, 32, false, false, true);
            AddDefault(StyleIdentifier.TableList1, 33, false, false, true);
            AddDefault(StyleIdentifier.TableList2, 37, true, true, false);
            AddDefault(StyleIdentifier.TableList3, 39, true, true, true);
            AddDefault(StyleIdentifier.TableList4, 41, false, false, false);
            AddDefault(StyleIdentifier.TableList5, 42, false, false, false);
            AddDefault(StyleIdentifier.TableList6, 43, false, false, false);
            AddDefault(StyleIdentifier.TableList7, 44, false, false, false);
            AddDefault(StyleIdentifier.TableList8, 45, false, false, false);
            AddDefault(StyleIdentifier.Table3DEffects1, 40, false, false, false);
            AddDefault(StyleIdentifier.Table3DEffects2, 46, false, false, false);
            AddDefault(StyleIdentifier.Table3DEffects3, 47, false, false, false);
            AddDefault(StyleIdentifier.TableContemporary, 48, false, false, false);
            AddDefault(StyleIdentifier.TableElegant, 49, false, false, false);
            AddDefault(StyleIdentifier.TableProfessional, 50, false, false, false);
            AddDefault(StyleIdentifier.TableSubtle1, 51, false, false, false);
            AddDefault(StyleIdentifier.TableSubtle2, 52, false, false, false);
            AddDefault(StyleIdentifier.TableWeb1, 46, false, false, false);
            AddDefault(StyleIdentifier.TableWeb2, 47, false, false, false);
            AddDefault(StyleIdentifier.TableWeb3, 48, false, false, false);
            AddDefault(StyleIdentifier.BalloonText, 49, false, false, false);
            AddDefault(StyleIdentifier.TableGrid, 50, false, false, false);
            AddDefault(StyleIdentifier.TableTheme, 51, false, false, false);
            AddDefault(StyleIdentifier.PlaceholderText, 52, false, false, false);
            AddDefault(StyleIdentifier.NoSpacing, 46, false, false, false);
            AddDefault(StyleIdentifier.LightShading, 47, false, false, false);
            AddDefault(StyleIdentifier.LightList, 48, false, false, false);
            AddDefault(StyleIdentifier.LightGrid, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumList1, 52, false, false, false);
            AddDefault(StyleIdentifier.MediumList2, 46, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1, 47, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2, 48, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3, 49, false, false, false);
            AddDefault(StyleIdentifier.DarkList, 50, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShading, 51, false, false, false);
            AddDefault(StyleIdentifier.ColorfulList, 52, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGrid, 46, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent1, 47, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent1, 48, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent1, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent1, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent1, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent1, 52, false, false, false);
            AddDefault(StyleIdentifier.Revision, 46, false, false, false);
            AddDefault(StyleIdentifier.ListParagraph, 47, false, false, false);
            AddDefault(StyleIdentifier.Quote, 48, false, false, false);
            AddDefault(StyleIdentifier.IntenseQuote, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent1, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent1, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent1, 52, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent1, 46, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent1, 47, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent1, 48, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent1, 49, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent1, 50, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent2, 51, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent2, 52, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent2, 46, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent2, 47, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent2, 48, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent2, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent2, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent2, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent2, 52, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent2, 46, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent2, 47, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent2, 48, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent2, 49, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent2, 50, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent3, 51, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent3, 52, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent3, 46, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent3, 47, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent3, 48, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent3, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent3, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent3, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent3, 52, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent3, 46, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent3, 47, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent3, 48, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent3, 49, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent3, 50, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent4, 51, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent4, 52, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent4, 46, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent4, 47, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent4, 48, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent4, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent4, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent4, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent4, 52, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent4, 46, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent4, 47, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent4, 48, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent4, 49, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent4, 50, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent5, 51, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent5, 52, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent5, 46, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent5, 47, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent5, 48, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent5, 49, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent5, 50, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent5, 51, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent5, 52, false, false, false);
        }

        private void InitToWord2019Default()
        {
            mItems.Clear();

            AddDefault(StyleIdentifier.Normal, 0, false, false, true);
            AddDefault(StyleIdentifier.Heading1, 0, false, false, true);
            AddDefault(StyleIdentifier.Heading2, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading3, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading4, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading5, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading6, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading7, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading8, 0, true, true, true);
            AddDefault(StyleIdentifier.Heading9, 0, true, true, true);
            AddDefault(StyleIdentifier.Index1, 0, false, false, false);
            AddDefault(StyleIdentifier.Index2, 0, false, false, false);
            AddDefault(StyleIdentifier.Index3, 0, false, false, false);
            AddDefault(StyleIdentifier.Index4, 0, false, false, false);
            AddDefault(StyleIdentifier.Index5, 0, false, false, false);
            AddDefault(StyleIdentifier.Index6, 0, false, false, false);
            AddDefault(StyleIdentifier.Index7, 0, false, false, false);
            AddDefault(StyleIdentifier.Index8, 0, false, false, false);
            AddDefault(StyleIdentifier.Index9, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc1, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc2, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc3, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc4, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc5, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc6, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc7, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc8, 0, false, false, false);
            AddDefault(StyleIdentifier.Toc9, 0, false, false, false);
            AddDefault(StyleIdentifier.NormalIndent, 0, false, false, false);
            AddDefault(StyleIdentifier.FootnoteText, 0, false, false, false);
            AddDefault(StyleIdentifier.CommentText, 0, false, false, false);
            AddDefault(StyleIdentifier.Header, 0, false, false, false);
            AddDefault(StyleIdentifier.Footer, 0, false, false, false);
            AddDefault(StyleIdentifier.IndexHeading, 0, false, false, false);
            AddDefault(StyleIdentifier.Caption, 0, true, true, true);
            AddDefault(StyleIdentifier.TableOfFigures, 0, false, false, false);
            AddDefault(StyleIdentifier.EnvelopeAddress, 0, false, false, false);
            AddDefault(StyleIdentifier.EnvelopeReturn, 0, false, false, false);
            AddDefault(StyleIdentifier.FootnoteReference, 0, false, false, false);
            AddDefault(StyleIdentifier.CommentReference, 0, false, false, false);
            AddDefault(StyleIdentifier.LineNumber, 0, false, false, false);
            AddDefault(StyleIdentifier.PageNumber, 0, false, false, false);
            AddDefault(StyleIdentifier.EndnoteReference, 0, false, false, false);
            AddDefault(StyleIdentifier.EndnoteText, 0, false, false, false);
            AddDefault(StyleIdentifier.TableOfAuthorities, 0, false, false, false);
            AddDefault(StyleIdentifier.Macro, 0, false, false, false);
            AddDefault(StyleIdentifier.ToaHeading, 0, false, false, false);
            AddDefault(StyleIdentifier.List, 0, false, false, false);
            AddDefault(StyleIdentifier.ListBullet, 0, false, false, false);
            AddDefault(StyleIdentifier.ListNumber, 0, false, false, false);
            AddDefault(StyleIdentifier.List2, 0, false, false, false);
            AddDefault(StyleIdentifier.List3, 0, false, false, false);
            AddDefault(StyleIdentifier.List4, 0, false, false, false);
            AddDefault(StyleIdentifier.List5, 0, false, false, false);
            AddDefault(StyleIdentifier.ListBullet2, 0, false, false, false);
            AddDefault(StyleIdentifier.ListBullet3, 0, false, false, false);
            AddDefault(StyleIdentifier.ListBullet4, 0, false, false, false);
            AddDefault(StyleIdentifier.ListBullet5, 0, false, false, false);
            AddDefault(StyleIdentifier.ListNumber2, 0, false, false, false);
            AddDefault(StyleIdentifier.ListNumber3, 0, false, false, false);
            AddDefault(StyleIdentifier.ListNumber4, 0, false, false, false);
            AddDefault(StyleIdentifier.ListNumber5, 0, false, false, false);
            AddDefault(StyleIdentifier.Title, 0, false, false, true);
            AddDefault(StyleIdentifier.Closing, 0, false, false, false);
            AddDefault(StyleIdentifier.Signature, 0, false, false, false);
            AddDefault(StyleIdentifier.DefaultParagraphFont, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyText, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyTextInd, 0, false, false, false);
            AddDefault(StyleIdentifier.ListContinue, 0, false, false, false);
            AddDefault(StyleIdentifier.ListContinue2, 0, false, false, false);
            AddDefault(StyleIdentifier.ListContinue3, 0, false, false, false);
            AddDefault(StyleIdentifier.ListContinue4, 0, false, false, false);
            AddDefault(StyleIdentifier.ListContinue5, 0, false, false, false);
            AddDefault(StyleIdentifier.MessageHeader, 0, false, false, false);
            AddDefault(StyleIdentifier.Subtitle, 0, false, false, true);
            AddDefault(StyleIdentifier.Salutation, 0, false, false, false);
            AddDefault(StyleIdentifier.Date, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyText1I, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyText1I2, 0, false, false, false);
            AddDefault(StyleIdentifier.NoteHeading, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyText2, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyText3, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyTextInd2, 0, false, false, false);
            AddDefault(StyleIdentifier.BodyTextInd3, 0, false, false, false);
            AddDefault(StyleIdentifier.BlockText, 0, false, false, false);
            AddDefault(StyleIdentifier.Hyperlink, 0, false, false, false);
            AddDefault(StyleIdentifier.FollowedHyperlink, 0, false, false, false);
            AddDefault(StyleIdentifier.Strong, 0, false, false, true);
            AddDefault(StyleIdentifier.Emphasis, 0, false, false, true);
            AddDefault(StyleIdentifier.DocumentMap, 0, false, false, false);
            AddDefault(StyleIdentifier.PlainText, 0, false, false, false);
            AddDefault(StyleIdentifier.EmailSignature, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlTopOfForm, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlBottomOfForm, 0, false, false, false);
            AddDefault(StyleIdentifier.NormalWeb, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlAcronym, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlAddress, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlCite, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlCode, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlDefinition, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlKeyboard, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlPreformatted, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlSample, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlTypewriter, 0, false, false, false);
            AddDefault(StyleIdentifier.HtmlVariable, 0, false, false, false);
            AddDefault(StyleIdentifier.TableNormal, 0, false, false, false);
            AddDefault(StyleIdentifier.CommentSubject, 0, false, false, false);
            AddDefault(StyleIdentifier.NoList, 0, false, false, false);
            AddDefault(StyleIdentifier.OutlineList1, 0, false, false, false);
            AddDefault(StyleIdentifier.OutlineList2, 0, false, false, false);
            AddDefault(StyleIdentifier.OutlineList3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableSimple1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableSimple2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableSimple3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableClassic1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableClassic2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableClassic3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableClassic4, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColorful1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColorful2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColorful3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColumns1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColumns2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColumns3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColumns4, 0, false, false, false);
            AddDefault(StyleIdentifier.TableColumns5, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid4, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid5, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid6, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid7, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid8, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList4, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList5, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList6, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList7, 0, false, false, false);
            AddDefault(StyleIdentifier.TableList8, 0, false, false, false);
            AddDefault(StyleIdentifier.Table3DEffects1, 0, false, false, false);
            AddDefault(StyleIdentifier.Table3DEffects2, 0, false, false, false);
            AddDefault(StyleIdentifier.Table3DEffects3, 0, false, false, false);
            AddDefault(StyleIdentifier.TableContemporary, 0, false, false, false);
            AddDefault(StyleIdentifier.TableElegant, 0, false, false, false);
            AddDefault(StyleIdentifier.TableProfessional, 0, false, false, false);
            AddDefault(StyleIdentifier.TableSubtle1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableSubtle2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableWeb1, 0, false, false, false);
            AddDefault(StyleIdentifier.TableWeb2, 0, false, false, false);
            AddDefault(StyleIdentifier.TableWeb3, 0, false, false, false);
            AddDefault(StyleIdentifier.BalloonText, 0, false, false, false);
            AddDefault(StyleIdentifier.TableGrid, 0, false, false, false);
            AddDefault(StyleIdentifier.TableTheme, 0, false, false, false);
            AddDefault(StyleIdentifier.PlaceholderText, 99, true, false, false);
            AddDefault(StyleIdentifier.NoSpacing, 1, false, false, true);
            AddDefault(StyleIdentifier.LightShading, 60, false, false, false);
            AddDefault(StyleIdentifier.LightList, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGrid, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkList, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShading, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulList, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGrid, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent1, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent1, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent1, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent1, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent1, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent1, 65, false, false, false);
            AddDefault(StyleIdentifier.Revision, 99, true, false, false);
            AddDefault(StyleIdentifier.ListParagraph, 34, false, false, true);
            AddDefault(StyleIdentifier.Quote, 29, false, false, true);
            AddDefault(StyleIdentifier.IntenseQuote, 30, false, false, true);
            AddDefault(StyleIdentifier.MediumList2Accent1, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent1, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent1, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent1, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent1, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent1, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent1, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent1, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent2, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent2, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent2, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent2, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent2, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent2, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent2, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent2, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent2, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent2, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent2, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent2, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent2, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent2, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent3, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent3, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent3, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent3, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent3, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent3, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent3, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent3, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent3, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent3, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent3, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent3, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent3, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent3, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent4, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent4, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent4, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent4, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent4, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent4, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent4, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent4, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent4, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent4, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent4, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent4, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent4, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent4, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent5, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent5, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent5, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent5, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent5, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent5, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent5, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent5, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent5, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent5, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent5, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent5, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent5, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent5, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent6, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent6, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent6, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent6, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent6, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent6, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent6, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent6, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent6, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent6, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent6, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent6, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent6, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent6, 73, false, false, false);
            AddDefault(StyleIdentifier.SubtleEmphasis, 19, false, false, true);
            AddDefault(StyleIdentifier.IntenseEmphasis, 21, false, false, true);
            AddDefault(StyleIdentifier.SubtleReference, 31, false, false, true);
            AddDefault(StyleIdentifier.IntenseReference, 32, false, false, true);
            AddDefault(StyleIdentifier.BookTitle, 33, false, false, true);
            AddDefault(StyleIdentifier.Bibliography, 37, true, true, false);
            AddDefault(StyleIdentifier.TocHeading, 39, true, true, true);
        }

        private void AddCommonDefaults()
        {
            AddDefault(StyleIdentifier.PlaceholderText, 99, true, false, false);
            AddDefault(StyleIdentifier.NoSpacing, 1, false, false, true);
            AddDefault(StyleIdentifier.LightShading, 60, false, false, false);
            AddDefault(StyleIdentifier.LightList, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGrid, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkList, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShading, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulList, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGrid, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent1, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent1, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent1, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent1, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent1, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent1, 65, false, false, false);
            AddDefault(StyleIdentifier.Revision, 99, true, false, false);
            AddDefault(StyleIdentifier.ListParagraph, 34, false, false, true);
            AddDefault(StyleIdentifier.Quote, 29, false, false, true);
            AddDefault(StyleIdentifier.IntenseQuote, 30, false, false, true);
            AddDefault(StyleIdentifier.MediumList2Accent1, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent1, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent1, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent1, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent1, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent1, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent1, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent1, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent2, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent2, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent2, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent2, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent2, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent2, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent2, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent2, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent2, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent2, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent2, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent2, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent2, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent2, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent3, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent3, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent3, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent3, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent3, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent3, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent3, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent3, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent3, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent3, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent3, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent3, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent3, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent3, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent4, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent4, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent4, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent4, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent4, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent4, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent4, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent4, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent4, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent4, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent4, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent4, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent4, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent4, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent5, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent5, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent5, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent5, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent5, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent5, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent5, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent5, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent5, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent5, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent5, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent5, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent5, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent5, 73, false, false, false);
            AddDefault(StyleIdentifier.LightShadingAccent6, 60, false, false, false);
            AddDefault(StyleIdentifier.LightListAccent6, 61, false, false, false);
            AddDefault(StyleIdentifier.LightGridAccent6, 62, false, false, false);
            AddDefault(StyleIdentifier.MediumShading1Accent6, 63, false, false, false);
            AddDefault(StyleIdentifier.MediumShading2Accent6, 64, false, false, false);
            AddDefault(StyleIdentifier.MediumList1Accent6, 65, false, false, false);
            AddDefault(StyleIdentifier.MediumList2Accent6, 66, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid1Accent6, 67, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid2Accent6, 68, false, false, false);
            AddDefault(StyleIdentifier.MediumGrid3Accent6, 69, false, false, false);
            AddDefault(StyleIdentifier.DarkListAccent6, 70, false, false, false);
            AddDefault(StyleIdentifier.ColorfulShadingAccent6, 71, false, false, false);
            AddDefault(StyleIdentifier.ColorfulListAccent6, 72, false, false, false);
            AddDefault(StyleIdentifier.ColorfulGridAccent6, 73, false, false, false);
            AddDefault(StyleIdentifier.SubtleEmphasis, 19, false, false, true);
            AddDefault(StyleIdentifier.IntenseEmphasis, 21, false, false, true);
            AddDefault(StyleIdentifier.SubtleReference, 31, false, false, true);
            AddDefault(StyleIdentifier.IntenseReference, 32, false, false, true);
            AddDefault(StyleIdentifier.BookTitle, 33, false, false, true);
            AddDefault(StyleIdentifier.Bibliography, 37, true, true, false);
            AddDefault(StyleIdentifier.TocHeading, 39, true, true, true);
        }

        private bool Equals(LatentStyles defLatentStyles)
        {
            if (defLatentStyles.Count != Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (!defLatentStyles[i].Equals(this[i]))
                    return false;
            }

            return true;
        }

        private void AddDefault(StyleIdentifier sti, int uiPriority, bool semiHidden, bool unhideWhenUsed, bool quickFormat)
        {
            Add(new LatentStyle(sti, false, quickFormat, semiHidden, uiPriority, unhideWhenUsed));
        }

        /// <summary>
        /// Key is <see cref="StyleIdentifier"/>, item is <see cref="LatentStyle"/>.
        /// </summary>
        private SortedIntegerListGeneric<LatentStyle> mItems = new SortedIntegerListGeneric<LatentStyle>();

        private bool mDefaultLockedState;
        private bool mDefaultQuickFormat;
        private bool mDefaultSemiHidden = true;
        private int mDefaultUIPriority = 99;
        private bool mDefaultUnhideWhenUsed = true;
        private int mKnownStylesCount = StyleCollection.MaxKnownSti2007;
    }
}
