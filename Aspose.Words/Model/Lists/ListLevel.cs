// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2005 by Roman Korchagin

using System;
using Aspose.Collections.Generic;
using Aspose.Images;
using Aspose.Words.Drawing;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Defines formatting for a list level.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-lists/">Working with Lists</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>You do not create objects of this class. List level objects are created automatically
    /// when a list is created. You access <see cref="ListLevel"/> objects via the
    /// <see cref="ListLevelCollection"/> collection.</p>
    ///
    /// <p>Use the properties of <see cref="ListLevel"/> to specify list formatting
    /// for individual list levels.</p>
    ///
    /// </remarks>
    /// <dev>
    /// Not sure where lvlElt.tplc is stored in the DOC file.
    /// Sounds like it is a template code for individual list levels.
    /// VA: All tplc are stored in a separate table in a document.
    ///     They can be seen in binary going one after another.
    ///     Not sure how to locate them in the document structure though.
    /// </dev>
    public class ListLevel : IRunAttrSource
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="document">Need to know the parent document so ListLevel.Font.Style can work.</param>
        /// <param name="levelNumber">Zero based level number.</param>
        ///
        /// <dev>
        /// It seems that ListLevel should also be IParaAttrSource but lets postpone for a while.
        /// </dev>
        internal ListLevel(DocumentBase document, int levelNumber)
        {
            mDocument = document;

            mLevelNumber = MakeLevelNumberValid(levelNumber);

            // Default is to restart on the previous level.
            mRestartAfterLevel = levelNumber - 1;
        }

        /// <summary>
        /// Constructor to create "proxy" facade object.
        /// </summary>
        /// <remarks>
        /// AM. This object proxies all interaction to parent ListLevel object
        /// but additionally holds IRunAttrSource of object for which ListFormat was requested.
        /// This allow to obtain attributes which are not set directly in this ListLevel from parent AttrSource.
        /// </remarks>
        internal ListLevel(ListLevel parentListLevel, IRunAttrSource parentRunAttrs)
        {
            mDocument = parentListLevel.mDocument;
            mParentListLevel = parentListLevel;
            mParentRunAttrSource = parentRunAttrs;
        }

        /// <summary>
        /// Creates picture bullet shape for the current list level.
        /// </summary>
        /// <remarks>Please note, <see cref="NumberStyle"/> will be set to <see cref="NumberStyle.Bullet"/> and
        /// <see cref="NumberFormat"/> to "\xF0B7" to properly display picture bullet.
        /// Red cross image will be set as picture bullet image upon creating.
        /// To change it please use <see cref="ImageData"/>.</remarks>
        public void CreatePictureBullet()
        {
            Shape shape = new Shape(mDocument, ShapeType.Image);

            // Add picture bullet shape to the collection of picture bullets available in this document.
            mDocument.Lists.AddPictureBullet(shape);

            // Set picture bullet Id.
            PictureBulletId = mDocument.Lists.PictureBulletCount - 1;

            NumberStyle = NumberStyle.Bullet;
            NumberFormat = "\xF0B7";

            // Set "NoImage.png" as picture bullet image.
            shape.ImageData.SetImage(ImageUtil.GetNoImageStream());
        }

        /// <summary>
        /// Deletes picture bullet for the current list level.
        /// </summary>
        /// <remarks>Default bullet will be shown after deleting.</remarks>
        public void DeletePictureBullet()
        {
            if (HasPictureBullet)
            {
                PictureBulletId = -1;

                // Apply default bullet parameters.
                NumberStyle = NumberStyle.Bullet;
                NumberFormat = "\xF0B7";
                RunPr.Name = "Symbol";
            }
        }

        /// <summary>
        /// Reports the string representation of the <see cref="ListLevel"/> object for the specified index
        /// of the list item. Parameters specify the <see cref="Words.NumberStyle"/> and an optional format string
        /// used when <see cref="Aspose.Words.NumberStyle.Custom"/> is specified.
        /// </summary>
        /// <param name="index">The index of the list item (must be in the range from 1 to 32767).</param>
        /// <param name="numberStyle">
        /// The <see cref="Words.NumberStyle"/> of the <see cref="ListLevel"/> object.
        /// </param>
        /// <param name="customNumberStyleFormat">
        /// The optional format string used when <see cref="Aspose.Words.NumberStyle.Custom"/> is specified (e.g. "a, ç, ĝ, ...").
        /// In other cases, this parameter must be <c>null</c> or empty.
        /// </param>
        /// <returns>
        /// The string representation of the <see cref="ListLevel"/> object, described by the <paramref name="numberStyle"/> parameter and
        /// the <paramref name="customNumberStyleFormat"/> parameter, in the list item at the position determined by the <paramref name="index"/> parameter.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customNumberStyleFormat"/> is <c>null</c> or empty when the <paramref name="numberStyle"/> is custom.-or-
        /// <paramref name="customNumberStyleFormat"/> is not <c>null</c> or empty when the <paramref name="numberStyle"/> is non-custom.-or-
        /// <paramref name="customNumberStyleFormat"/> is invalid.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">index is out of range.</exception>
        public static string GetEffectiveValue(int index, NumberStyle numberStyle, string customNumberStyleFormat)
        {
            const int minIndex = 1;
            const int maxIndex = 32767;
            const string customNumberStyleFormatArgName = "customNumberStyleFormat";

            if ((index < minIndex) || (index > maxIndex))
            {
                throw new ArgumentOutOfRangeException("index", index,
                    string.Format("Specified argument must be in the range from {0} to {1}.", minIndex, maxIndex));
            }

            if (string.IsNullOrEmpty(customNumberStyleFormat) && (numberStyle == NumberStyle.Custom))
            {
                throw new ArgumentException(
                    "Specified argument must not be null or empty if the number style is custom.",
                    customNumberStyleFormatArgName);
            }

            if (!string.IsNullOrEmpty(customNumberStyleFormat) && (numberStyle != NumberStyle.Custom))
            {
                throw new ArgumentException(
                    "For this number style, the specified argument must be null or empty.",
                    customNumberStyleFormatArgName);
            }

            return NumberConverter.NumberToString(index, numberStyle, customNumberStyleFormat, true);
        }

        /// <summary>
        /// Makes a deep copy of the list level.
        /// Note that the paragraph style that might be linked to this level is not copied.
        /// </summary>
        internal ListLevel Clone(DocumentBase dstDocument)
        {
            ListLevel lhs = (ListLevel)MemberwiseClone();
            lhs.mDocument = dstDocument;
            lhs.mParaPr = mParaPr.Clone();
            lhs.mRunPr = mRunPr.Clone();
            lhs.mFont = null;
            return lhs;
        }

        /// <summary>
        /// Compares with the specified ListLevel.
        /// </summary>
        public bool Equals(ListLevel level)
        {
            return EqualsCore(level, new HashSetGeneric<Pair>());
        }

        /// <summary>
        /// Compares with the specified <see cref="ListLevel"/>.
        /// </summary>
        /// <param name="level"><see cref="ListLevel"/> that will be compared with this <see cref="ListLevel"/>.</param>
        /// <param name="alreadyComparedLinkedStyles">HashSet for collecting already compared linked styles to avoid dead loops.</param>
        internal bool EqualsCore(ListLevel level, HashSetGeneric<Pair> alreadyComparedLinkedStyles)
        {
            // Verify list-level generic attributes.
            if (!GenericEquals(level))
                return false;

            // Compare run properties.
            if (!this.RunPr.Equals(level.RunPr, Style.ComparisonIgnorableKeys))
                return false;

            // Compare paragraph properties.
            if (!this.ParaPr.Equals(level.ParaPr, Style.ComparisonIgnorableKeys))
                return false;

            // Compare formatting of linked styles.
            // WORDSNET-19497 Compare linked styles from existing ones.
            Style linkedStyle = mDocument.Styles.GetByIstd(ParaStyleIstd, false);
            Style levelLinkedStyle = level.mDocument.Styles.GetByIstd(level.ParaStyleIstd, false);

            if (!Style.AreEqual(linkedStyle, levelLinkedStyle, alreadyComparedLinkedStyles))
                return false;

            return true;
        }

        /// <summary>
        /// Calculates hash code for this object.
        /// </summary>
        /// <dev>
        /// To be compatible with the <see cref="Equals(ListLevel)"/> method, only properties that affect visual
        /// representation of the list are included into the calculation. Object ID and similar properties should
        /// be ignored.
        /// </dev>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            int hashCode = NumberStyle.GetHashCode();
            if (NumberFormat != null)
                hashCode = (hashCode * 397) ^ NumberFormat.GetHashCode();
            hashCode = (hashCode * 397) ^ Alignment.GetHashCode();
            hashCode = (hashCode * 397) ^ LabelCharacterCategory.GetHashCode();
            hashCode = (hashCode * 397) ^ IsLegal.GetHashCode();
            hashCode = (hashCode * 397) ^ RestartAfterLevel.GetHashCode();
            hashCode = (hashCode * 397) ^ IsRestartAfterLevelCustom.GetHashCode();
            hashCode = (hashCode * 397) ^ TrailingCharacter.GetHashCode();
            hashCode = (hashCode * 397) ^ PictureBulletId.GetHashCode();
            hashCode = (hashCode * 397) ^ LevelNumber.GetHashCode();
            hashCode = (hashCode * 397) ^ TabPosition.GetHashCode();
            hashCode = (hashCode * 397) ^ NumberPosition.GetHashCode();
            hashCode = (hashCode * 397) ^ TextPosition.GetHashCode();

            hashCode = (hashCode * 397) ^ RunPr.GetHashCode(Style.ComparisonIgnorableKeys);
            hashCode = (hashCode * 397) ^ ParaPr.GetHashCode(Style.ComparisonIgnorableKeys);

            if (ParaStyleIstd != StyleIndex.Nil)
                hashCode = (hashCode * 397) ^ 1;

            return hashCode;
        }

        /// <summary>
        /// Sets start at value only if it is in the valid range. Use it during loading from a file.
        /// </summary>
        internal void SetStartAtSafe(int value)
        {
            if (IsStartAtValid(value))
                GetInstance.mStartAt = value;
        }

        /// <summary>
        /// Sets a restart after level value safely. Use it during loading from a file.
        /// </summary>
        internal void SetRestartAfterLevelSafe(int value)
        {
            if (IsRestartAfterLevelValid(value))
                GetInstance.mRestartAfterLevel = value;
        }

        /// <summary>
        /// Sets the number format value safely. Use it during loading from a file.
        /// </summary>
        internal void SetNumberFormatSafe(string value)
        {
            if (IsNumberFormatValid(value))
                GetInstance.mNumberFormat = value;
        }

        /// <summary>
        /// Verifies string is valid for list level number format.
        /// </summary>
        private static bool IsNumberFormatValid(string value)
        {
            return IsNumberFormatLengthValid(value) && IsNumberFormatCharsValid(value);
        }

        /// <summary>
        /// Returns true if string contains only allowed for list level number format characters.
        /// </summary>
        private static bool IsNumberFormatCharsValid(string value)
        {
            return (value != null) && (value.IndexOf('\xffff') < 0);
        }

        /// <summary>
        /// Returns true if string has allowed for list level number format length.
        /// </summary>
        private static bool IsNumberFormatLengthValid(string value)
        {
            return (value != null) && (value.Length <= MaxNumberFormatLength);
        }

        /// <summary>
        /// Indicates that given char is a placeholder character in <see cref="ListLevel.NumberFormat"/>.
        /// </summary>
        internal static bool IsPlaceholder(char c)
        {
            return (c >= '\x0000') && (c <= '\x0008');
        }

        /// <summary>
        /// Number format must not contain top-level placeholders.
        /// </summary>
        internal static bool IsPlaceholderValid(string numberFormat, int levelNumber)
        {
            foreach (char c in numberFormat)
                if (PlaceHoldersString.IndexOf(c) > levelNumber)
                    return false;

            return true;
        }

        internal static bool IsLegacySpaceValid(int legacySpace)
        {
            return (legacySpace >= -31680) && (legacySpace <= 31680);
        }

        /// <summary>
        /// Compares generic ListLevel attributes.
        /// </summary>
        private bool GenericEquals(ListLevel level)
        {
            return ((NumberStyle == level.NumberStyle) &&
                    (NumberFormat == level.NumberFormat) &&
                    (Alignment == level.Alignment) &&
                    (LabelCharacterCategory == level.LabelCharacterCategory) &&
                    (IsLegal == level.IsLegal) &&
                    (RestartAfterLevel == level.RestartAfterLevel) &&
                    (IsRestartAfterLevelCustom == level.IsRestartAfterLevelCustom) &&
                    (TrailingCharacter == level.TrailingCharacter) &&
                    (PictureBulletId == level.PictureBulletId) &&
                    (PictureBullet == level.PictureBullet) &&
                    (LevelNumber == level.LevelNumber) &&
                    MathUtil.AreEqual(TabPosition, level.TabPosition) &&
                    MathUtil.AreEqual(NumberPosition, level.NumberPosition) &&
                    MathUtil.AreEqual(TextPosition, level.TextPosition));
        }

        /// <summary>
        /// Returns or sets the starting number for this list level.
        /// </summary>
        /// <remarks>
        /// <p>Default value is 1.</p>
        /// </remarks>
        public int StartAt
        {
            get { return GetInstance.mStartAt; }
            set
            {
                if (!IsStartAtValid(value))
                    throw new ArgumentOutOfRangeException("value");

                GetInstance.mStartAt = value;
            }
        }

        /// <summary>
        /// Returns or sets the number style for this list level.
        /// </summary>
        public NumberStyle NumberStyle
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mNumberStyle; }
            set
            {
                if (value != NumberStyle.Custom)
                    GetInstance.mCustomNumberStyle = "";
                GetInstance.mNumberStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom number style format for this list level. For example: "a, ç, ĝ, ...".
        /// </summary>
        public string CustomNumberStyleFormat
        {
            get { return CustomNumberStyle; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "CustomNumberStyleFormat");

                CustomNumberStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom number style for this list level. For example: a, ç, ĝ, ...
        /// </summary>
        internal string CustomNumberStyle
        {
            get
            {
                if (GetInstance.NumberStyle == NumberStyle.Custom)
                    return GetInstance.mCustomNumberStyle;
                else
                    return "";
            }
            set
            {
                if (StringUtil.HasChars(value))
                    GetInstance.NumberStyle = NumberStyle.Custom;
                GetInstance.mCustomNumberStyle = value;
            }
        }

        /// <summary>
        /// Gets label character category to format the label with.
        /// </summary>
        /// <remarks>
        /// This is a simplification. Actually, list label can consist of characters with different categories.
        /// However, currently SpanList only supports one span per list label.
        /// TODO Above statement is not True. Span list handles labels consisting of multiple spans. And it does not use this method at all.
        /// </remarks>
        internal CharacterCategory LabelCharacterCategory
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                switch (NumberStyle)
                {
                    case NumberStyle.Aiueo:
                    case NumberStyle.AiueoHalfWidth:
                    case NumberStyle.Iroha:
                    case NumberStyle.IrohaHalfWidth:
                    case NumberStyle.Kanji:
                    case NumberStyle.KanjiDigit:
                    case NumberStyle.KanjiTraditional:
                    case NumberStyle.KanjiTraditional2:
                    // WORDSNET-5985 encircled numbers are displayed in Far Eastern font by MS Word when used in list labels.
                    case NumberStyle.NumberInCircle:
                        // Add other eastern styles as needed.
                        return CharacterCategory.FarEast;
                    // WORDSNET-6256 Font specified for complex script is used for rendering Hebrew labels.
                    case NumberStyle.Hebrew1:
                    case NumberStyle.Hebrew2:
                        return CharacterCategory.ComplexScript;
                    default:
                        return CharacterCategory.Ascii;
                }
            }
        }

        /// <summary>
        /// Returns or sets the number format for the list level.
        /// </summary>
        /// <remarks>
        /// <p>Among normal text characters, the string can contain placeholder characters \x0000 to \x0008
        /// representing the numbers from the corresponding list levels.</p>
        ///
        /// <p>For example, the string "\x0000.\x0001)" will generate a list label
        /// that looks something like "1.5)". The number "1" is the current number from
        /// the 1st list level, the number "5" is the current number from the 2nd list level.</p>
        ///
        /// <p>Null is not allowed, but an empty string meaning no number is valid.</p>
        /// </remarks>
        public string NumberFormat
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mNumberFormat; }
            set
            {
                if (!IsNumberFormatValid(value))
                    throw new ArgumentException("value");

                GetInstance.mNumberFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the justification of the actual number of the list item.
        /// </summary>
        /// <remarks>
        /// <p>The list label is justified relative to the <see cref="NumberPosition"/> property.</p>
        /// </remarks>
        public ListLevelAlignment Alignment
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mAlignment; }
            set { GetInstance.mAlignment = value; }
        }

        /// <summary>
        /// True if the level turns all inherited numbers to Arabic, false if it preserves their number style.
        /// </summary>
        public bool IsLegal
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mIsLegal; }
            set { GetInstance.mIsLegal = value; }
        }

        /// <summary>
        /// Sets or returns the list level that must appear before the specified list level restarts numbering.
        /// </summary>
        /// <remarks>
        /// <para>The value of -1 means the numbering will continue.</para>
        /// </remarks>
        public int RestartAfterLevel
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mRestartAfterLevel; }
            set
            {
                if (!IsRestartAfterLevelValid(value))
                    throw new ArgumentOutOfRangeException("value");

                GetInstance.mRestartAfterLevel = value;
            }
        }

        /// <summary>
        /// Returns or sets the character inserted after the number for the list level.
        /// </summary>
        public ListTrailingCharacter TrailingCharacter
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mTrailingCharacter; }
            set { GetInstance.mTrailingCharacter = value; }
        }

        /// <summary>
        /// Specifies character formatting used for the list label.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(this, mDocument);

                return mFont;
            }
        }

        /// <summary>
        /// Returns or sets the tab position (in points) for the list level.
        /// </summary>
        /// <remarks>
        /// <p>Has effect only when <see cref="TrailingCharacter"/> is a tab.</p>
        ///
        /// <seealso cref="NumberPosition"/>
        /// <seealso cref="TextPosition"/>
        /// </remarks>
        public double TabPosition
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if ((ParaPr.TabStops != null) && (ParaPr.TabStops.Count > 0))
                    return ParaPr.TabStops[0].Position;
                return 0;
            }
            set
            {
                if (ParaPr.TabStops == null)
                    ParaPr.TabStops = new TabStopCollection();

                ParaPr.TabStops.Clear();

                ParaPr.TabStops.Add(value, TabAlignment.List, TabLeader.None);
            }
        }

        /// <summary>
        /// Returns or sets the position (in points) of the number or bullet for the list level.
        /// </summary>
        /// <remarks>
        /// <p><see cref="NumberPosition"/> corresponds to LeftIndent plus FirstLineIndent of the paragraph.</p>
        ///
        /// <seealso cref="TextPosition"/>
        /// <seealso cref="TabPosition"/>
        /// </remarks>
        public double NumberPosition
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return ConvertUtilCore.TwipToPoint(ParaPr.LeftIndent + ParaPr.FirstLineIndent); }
            set
            {
                // Lets' say left indent is 100 and the user sets a new number position 80.
                // New left indent should become -20.
                ParaPr.FirstLineIndent = ConvertUtilCore.PointToTwip(value) - ParaPr.LeftIndent;
            }
        }

        /// <summary>
        /// Returns or sets the position (in points) for the second line of wrapping text for the list level.
        /// </summary>
        /// <remarks>
        /// <p><see cref="TextPosition"/> corresponds to LeftIndent of the paragraph.</p>
        ///
        /// <seealso cref="NumberPosition"/>
        /// <seealso cref="TabPosition"/>
        /// </remarks>
        public double TextPosition
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return ConvertUtilCore.TwipToPoint(ParaPr.LeftIndent); }
            set
            {
                // TextPosition corresponds to LeftIndent of the paragraph, but
                // NumberPosition is derived from LeftIndent and FirstLineIndent.
                // Therefore, changing TextPosition will change NumberPosition and we don't want that.
                // So remember the old NumberPosition and reapply it after LeftIndent was modified.
                double oldNumberPosition = NumberPosition;
                ParaPr.LeftIndent = ConvertUtilCore.PointToTwip(value);
                NumberPosition = oldNumberPosition;
            }
        }

        /// <summary>
        /// Gets or sets the paragraph style that is linked to this list level.
        /// </summary>
        /// <remarks>
        /// <p>This property is <c>null</c> when the list level is not linked to a paragraph style.
        /// This property can be set to <c>null</c>.</p>
        /// </remarks>
        public Style LinkedStyle
        {
            get
            {
                if (ParaStyleIstd == StyleIndex.Nil)
                    return null;
                else
                    return mDocument.Styles.GetByIstd(ParaStyleIstd, true);
            }
            set
            {
                if (value == null)
                {
                    ParaStyleIstd = StyleIndex.Nil;
                }
                else
                {
                    if (value.Type != StyleType.Paragraph)
                        throw new ArgumentException("The style must be a paragraph style.");

                    if (value.Document != mDocument)
                        throw new ArgumentException("The style must belong to this document.");

                    ParaStyleIstd = value.Istd;
                }
            }
        }

        /// <summary>
        /// Returns image data of the picture bullet shape for the current list level.
        /// </summary>
        /// <remarks>
        /// If this level doesn't define picture bullet returns <c>null</c>.
        /// Before setting new image for non picture bullet shape, please use <see cref="CreatePictureBullet()"/> method first.
        /// </remarks>
        public ImageData ImageData
        {
            get
            {
                if (PictureBullet != null)
                    return PictureBullet.ImageData;
                return null;
            }
        }

        internal static bool IsStartAtValid(int value)
        {
            // This is what MS Word limits the value to.
            return ((value >= 0) && (value <= 32767));
        }

        internal static bool IsRestartAfterLevelValid(int value)
        {
            return ((value >= -1) && (value < MaxLevels));
        }

        internal static bool IsLevelNumberValid(int levelNumber)
        {
            return levelNumber >= MinLevel && levelNumber < MaxLevels;
        }

        internal static int MakeLevelNumberValid(int levelNumber)
        {
            return IsLevelNumberValid(levelNumber) ? levelNumber : MinLevel;
        }

        internal static bool IsNumberStyleValid(NumberStyle numberStyle)
        {
            return (NumberStyle.Arabic <= numberStyle && numberStyle <= NumberStyle.UppercaseRussian)
                   || (numberStyle == NumberStyle.None) || (numberStyle == NumberStyle.Custom);
        }

        internal static bool IsAlignmentValid(ListLevelAlignment alignment)
        {
            return (ListLevelAlignment.Left <= alignment) && (alignment <= ListLevelAlignment.Right);
        }

        internal static bool IsTrailingCharacterValid(ListTrailingCharacter trailingChar)
        {
            return (ListTrailingCharacter.Tab <= trailingChar) && (trailingChar <= ListTrailingCharacter.Nothing);
        }

        /// <summary>
        /// Returns picture bullet shape for current list level.
        /// </summary>
        /// <remarks>
        /// Shape is returned unscaled. If this level doesn't define picture bullet returns null.
        /// </remarks>
        internal Shape PictureBullet
        {
            get
            {
                if ((PictureBulletId >= 0) && (PictureBulletId < mDocument.Lists.PictureBulletCount))
                    return mDocument.Lists.GetPictureBullet(PictureBulletId);
                return null;
            }
        }

        /// <summary>
        /// Istd of the paragraph style to be applied to list items of this level.
        /// </summary>
        internal int ParaStyleIstd
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mParaStyleIstd; }
            set { GetInstance.mParaStyleIstd = value; }
        }

        /// <summary>
        /// Paragraph formatting for the list paragraph.
        /// Can contain any combination of three paragraph properties:
        /// left indents, first line left indents, and tabs.
        /// </summary>
        internal ParaPr ParaPr
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mParaPr; }
        }

        /// <summary>
        /// Contains font formatting for the list number.
        /// Can contain any character properties (all of which affect all text for that level).
        /// </summary>
        internal RunPr RunPr
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mRunPr; }
        }

        /// <summary>
        /// Gets or sets the picture bullet id. The picture bullets themselves are stored in <see cref="ListCollection"/>.
        ///
        /// Negative value means there is no picture bullet.
        ///
        /// Setting this property does not validate that picture bullet with this id exists because
        /// picture bullets might be loaded after list levels. Therefore, this id might be invalid.
        /// </summary>
        internal int PictureBulletId
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mPictureBulletId; }
            set { GetInstance.mPictureBulletId = value; }
        }

        /// <summary>
        /// RK It is better to check for null because it safely covers scenarios where bullet id is set,
        /// but bullet itself is not available.
        /// </summary>
        internal bool HasPictureBullet
        {
            get { return (PictureBullet != null); }
        }

        /// <summary>
        /// Zero based 'nesting level' of this list level. Set only during create/load.
        /// </summary>
        internal int LevelNumber
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mLevelNumber; }
        }

        /// <summary>
        /// True when restart after level is not at one level higher.
        /// </summary>
        internal bool IsRestartAfterLevelCustom
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return GetInstance.mRestartAfterLevel != GetInstance.mLevelNumber - 1; }
        }

        /// <summary>
        /// Do NOT use this when getting/calculating list formatting.
        /// Legacy formatting is converted into normal formatting in <see cref="Aspose.Words.Validation.DocumentValidator"/>.
        ///
        /// Specifies whether the legacy numbering properties present for this numbering level shall
        /// be used to format the numbering for any paragraph which references it.
        /// </summary>
        internal bool Legacy
        {
            get { return GetInstance.mLegacy; }
            set { GetInstance.mLegacy = value; }
        }

        /// <summary>
        /// Do NOT use this when getting/calculating list formatting.
        /// This value only exists in DOC and we just preserve it.
        /// </summary>
        internal bool LegacyPrev
        {
            get { return GetInstance.mLegacyPrev; }
            set { GetInstance.mLegacyPrev = value; }
        }

        /// <summary>
        /// Do NOT use this when getting/calculating list formatting.
        /// This value only exists in DOC and we just preserve it.
        /// </summary>
        internal bool LegacyPrevSpace
        {
            get { return GetInstance.mLegacyPrevSpace; }
            set { GetInstance.mLegacyPrevSpace = value; }
        }

        /// <summary>
        /// Do NOT use this when getting/calculating list formatting.
        /// Legacy formatting is converted into normal formatting in <see cref="Aspose.Words.Validation.DocumentValidator"/>.
        ///
        /// According to my experiments this value specifies the distance between the end of the list label text and
        /// beginning of the paragraph text.
        /// </summary>
        /// <dev>
        /// Specs say:
        /// Specifies the indentation which shall be applied between a legacy numbering symbol and the accompanying text
        /// of the associated paragraph in the document. This value is specified in twentieths of a point.
        /// </dev>
        internal int LegacySpace
        {
            get { return GetInstance.mLegacySpace; }
            set { GetInstance.mLegacySpace = value; }
        }

        /// <summary>
        /// Do NOT use this when getting/calculating list formatting.
        /// Legacy formatting is converted into normal formatting in <see cref="Aspose.Words.Validation.DocumentValidator"/>.
        ///
        /// According to my experiments this value specifies the distance between the beginning of the list label
        /// and beginning of the paragraph text.
        /// </summary>
        /// <dev>
        /// Specs say:
        /// Specifies the indentation which shall be applied to a legacy numbering symbol from the text margin of the document.
        /// This value is specified in twentieths of a point.
        /// </dev>
        internal int LegacyIndent
        {
            get { return GetInstance.mLegacyIndent; }
            set { GetInstance.mLegacyIndent = value; }
        }

        /// <summary>
        /// Returns true if expected ListLevel equal actual ListLevel.
        /// </summary>
        internal static bool AreSameLegacyListLevels(ListLevel expected, ListLevel actual)
        {
            return
                ((expected.Legacy == actual.Legacy) &&
                (expected.NumberStyle == actual.NumberStyle) &&
                (expected.NumberFormat == actual.NumberFormat) &&
                (expected.RunPr.NameAscii == actual.RunPr.NameAscii) &&
                (expected.RunPr.NameOther == actual.RunPr.NameOther) &&
                (expected.LegacyIndent == actual.LegacyIndent) &&
                (expected.LegacySpace == actual.LegacySpace) &&
                (expected.LegacyPrev == actual.LegacyPrev) &&
                (expected.StartAt == actual.StartAt) &&
                (expected.RunPr.Color == actual.RunPr.Color));
        }

        /// <summary>
        /// Returns either parent ListLevel if this ListLevel is inherited or returns ListLevel itself.
        /// </summary>
        /// <remarks>
        /// AM. Inherited ListLevel acts like proxy. All set operation are proxied to parent ListLevel.
        /// Getters are also proxied to parent ListLevel except FetchInheritedRunAttr which gets attribute value
        /// from parent attribute source if attribute is missing in ListLevel.
        /// </remarks>
        private ListLevel GetInstance
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mParentListLevel != null)
                {
                    return mParentListLevel;
                }

                return this;
            }
        }

        /// <summary>
        /// I'm not fully sure, but looks like corresponds to WordML lvlElt.tentative.
        /// I think it is used by MS Word to indicate levels that are not yet used in the document.
        /// I don't maintain this value.
        /// </summary>
        internal bool IsTentative;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDocument;
        private readonly int mLevelNumber;
        private int mStartAt = 1;
        private NumberStyle mNumberStyle;
        private string mCustomNumberStyle = "";
        private string mNumberFormat = "";
        private ListLevelAlignment mAlignment;
        private bool mIsLegal;
        private int mRestartAfterLevel;
        private ListTrailingCharacter mTrailingCharacter;
        private ParaPr mParaPr = new ParaPr();
        private RunPr mRunPr = new RunPr();
        private int mPictureBulletId = -1;

        private bool mLegacy;
        private int mLegacySpace;
        private int mLegacyIndent;
        private bool mLegacyPrevSpace;
        private bool mLegacyPrev;
        // WORDSNET-3189 Added the default value, otherwise it was referring to the Normal style.
        private int mParaStyleIstd = StyleIndex.Nil;

        /// <summary>
        /// This is a facade object. Null until the Font property is accessed.
        /// </summary>
        private Font mFont;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IRunAttrSource mParentRunAttrSource;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly ListLevel mParentListLevel;

        /// <summary>
        /// MS Word seems to allow 31 chars for number format only.
        /// </summary>
        internal const int MaxNumberFormatLength = 31;
        /// <summary>
        /// Minimum list level.
        /// </summary>
        internal const int MinLevel = 0;
        /// <summary>
        /// Maximum number of list levels. Valid value of list level
        /// is from MinLevel to (MaxLevels - 1) inclusively.
        /// </summary>
        internal const int MaxLevels = 9;
        /// <summary>
        /// Left indent which MS Word adds for every list level.
        /// </summary>
        internal const int LeftIndent = 36;
        /// <summary>
        /// List number hanging which MS Word uses for lists by default.
        /// </summary>
        internal const int DefaultNumberHanging = 18;

        private const string PlaceHoldersString = "\x0000\x0001\x0002\x0003\x0004\x0005\x0006\x0007\x0008";

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return ((IRunAttrSource)this).GetDirectRunAttr(key, RevisionsView.Original);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            // Use RunPr instead of mRunPr for inheritance worked.
            return RunPr[key];
        }

        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            if (mParentRunAttrSource == null)
                return RunPr.GetDefaultAttr(key);

            // AM. According to RK comment in ListLabel.FetchInheritedRunAttr don't take decoration from paragraph.
            // WORDSNET-15727 ListFormat.ListLevel.Font does not report bold attribute properly.
            // Bold or Italic attributes have to be get from Paragraph if ListLevel is not bullet.
            if (((key == FontAttr.Bold) || (key == FontAttr.Italic)) && (NumberStyle == NumberStyle.Bullet))
                return RunPr.GetDefaultAttr(key);

            return InlineHelper.FetchAttr(mParentRunAttrSource, key);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            RunPr.SetAttr(key, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            RunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            RunPr.Clear();
        }

        #endregion
    }
}
