// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/04/2018 by Ilya Navrotskiy

using Aspose.Collections.Generic;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Allows to set up language preferences.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Implements 'Set the Office Language Preferences' dialog in Word.
    /// </remarks>
    public class LanguagePreferences
    {
        /// <summary>
        /// Adds additional editing language.
        /// </summary>
        public void AddEditingLanguage(EditingLanguage language)
        {
            mEditingLanguages.Add(language);
        }

        /// <summary>
        /// Adds additional editing languages.
        /// </summary>
        /// <javaName>void addEditingLanguages(int[] languages)</javaName>
        public void AddEditingLanguages(EditingLanguage[] languages)
        {
            ArgumentUtil.CheckNotNull(languages, "languages");

            foreach (EditingLanguage language in languages)
                AddEditingLanguage(language);
        }

        /// <summary>
        /// <para>Gets or sets default editing language.</para>
        /// <para>The default value is <see cref="EditingLanguage.EnglishUS"/>.</para>
        /// </summary>
        public EditingLanguage DefaultEditingLanguage
        {
            get { return mDefaultEditingLanguage; }
            set { mDefaultEditingLanguage = value; }
        }

        /// <summary>
        /// Gets or sets Test flag.
        /// </summary>
        /// <remarks>
        /// When it is <c>true</c>, languages are not applying to the defaults, avoiding to fail a large number of old tests.
        /// You should mark tests that don't use test mode as NonParallelizable.
        /// </remarks>
        internal static bool TestMode
        {
            get { return gTestMode; }
            set { gTestMode = value; }
        }

        /// <summary>
        /// Returns true, if collection of editing languages contains RTL (Right-To-Left) language.
        /// </summary>
        internal bool ContainRtlLanguage
        {
            get { return (ContainsHebrew || ContainsArabic); }
        }

        /// <summary>
        /// Returns LocaleId depending on contained editing languages.
        /// </summary>
        internal EditingLanguage LocaleId
        {
            get
            {
                return (LocaleClassifier.IsLatin((int)mDefaultEditingLanguage) ||
                    LocaleClassifier.IsCyrillic((int)mDefaultEditingLanguage))
                    ? mDefaultEditingLanguage
                    : EditingLanguage.EnglishUS;
            }
        }

        /// <summary>
        /// Returns LocaleIdFarEast depending on contained editing languages.
        /// </summary>
        internal EditingLanguage LocaleIdFarEast
        {
            get
            {
                if (LocaleClassifier.IsChineseOrJapanese((int)mDefaultEditingLanguage))
                    return LocaleIdFarEastByDefaultLanguage;

                EditingLanguage editingLanguage = LocaleIdFarEastByEditingLanguagesCollection;
                if (editingLanguage != EditingLanguage.EnglishUS)
                    return editingLanguage;

                // If there are no any Chinese or Japanese languages set as default or added into editing languages,
                // then LocaleIdFarEast is the same as LocaleId.
                return LocaleId;
            }
        }

        /// <summary>
        /// Gets LocaleIdFarEast by default editing language.
        /// </summary>
        internal EditingLanguage LocaleIdFarEastByDefaultLanguage
        {
            get
            {
                if (LocaleClassifier.IsChineseSimplified((int)mDefaultEditingLanguage))
                    return EditingLanguage.ChinesePRC;

                if (LocaleClassifier.IsChineseTraditional((int)mDefaultEditingLanguage))
                    return EditingLanguage.ChineseTaiwan;

                if (mDefaultEditingLanguage == EditingLanguage.Japanese)
                    return EditingLanguage.Japanese;

                if (mDefaultEditingLanguage == EditingLanguage.Korean)
                    return EditingLanguage.Korean;

                return EditingLanguage.EnglishUS;
            }
        }

        /// <summary>
        /// Gets LocaleIdFarEast by editing languages collection.
        /// </summary>
        private EditingLanguage LocaleIdFarEastByEditingLanguagesCollection
        {
            get
            {
                if (ContainsChineseSimplified)
                    return EditingLanguage.ChinesePRC;
                else if (ContainsChineseTraditional)
                    return EditingLanguage.ChineseTaiwan;
                else if (ContainsJapanese)
                    return EditingLanguage.Japanese;

                return EditingLanguage.EnglishUS;
            }
        }

        /// <summary>
        /// Returns LocaleIdBi depending on contained editing languages.
        /// </summary>
        internal EditingLanguage LocaleIdBi
        {
            get
            {
                if (LocaleClassifier.IsArabic((int)mDefaultEditingLanguage) || ContainsArabic)
                    return EditingLanguage.ArabicSaudiArabia;
                else if (LocaleClassifier.IsHebrew((int)mDefaultEditingLanguage) || ContainsHebrew)
                    return EditingLanguage.Hebrew;
                else
                    return EditingLanguage.ArabicSaudiArabia;
            }
        }

        /// <summary>
        /// Returns true, if collection of editing languages contains Chinese Simplified language.
        /// </summary>
        private bool ContainsChineseSimplified
        {
            get
            {
                return (mEditingLanguages.Contains(EditingLanguage.ChinesePRC) ||
                    mEditingLanguages.Contains(EditingLanguage.ChineseSingapore));
            }
        }

        /// <summary>
        /// Returns true, if collection of editing languages contains Chinese Traditional language.
        /// </summary>
        private bool ContainsChineseTraditional
        {
            get
            {
                return (mEditingLanguages.Contains(EditingLanguage.ChineseTaiwan) ||
                    mEditingLanguages.Contains(EditingLanguage.ChineseHongKong) ||
                    mEditingLanguages.Contains(EditingLanguage.ChineseMacao));
            }
        }

        /// <summary>
        /// Returns true, if collection of editing languages contains Hebrew language.
        /// </summary>
        private bool ContainsHebrew
        {
            get
            {
                return mEditingLanguages.Contains(EditingLanguage.Hebrew);
            }
        }

        /// <summary>
        /// Returns true, if collection of editing languages contains Arabic language.
        /// </summary>
        private bool ContainsArabic
        {
            get
            {
                foreach (EditingLanguage editingLanguage in mEditingLanguages)
                {
                    if (LocaleClassifier.IsArabic((int)editingLanguage))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns true, if collection of editing languages contains Japanese language.
        /// </summary>
        private bool ContainsJapanese
        {
            get { return mEditingLanguages.Contains(EditingLanguage.Japanese); }
        }

        private readonly HashSetGeneric<EditingLanguage> mEditingLanguages = new HashSetGeneric<EditingLanguage>();
        private EditingLanguage mDefaultEditingLanguage = EditingLanguage.EnglishUS;

        private static bool gTestMode;
    }
}
