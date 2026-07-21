// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2006 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Drawing;

namespace Aspose.Words
{
    /// <summary>
    /// Resolves font auto color into black or white depending on the shading color.
    /// </summary>
    internal class FontColorResolver
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FontColorResolver()
        {
            // Lets mShadingStack.Peek() be always valid.
            mShadingStack.Push(null);
        }

        internal void PushShading(Shading shading)
        {
            mShadingStack.Push(IsVisible(shading) ? shading : mShadingStack.Peek());
        }

        internal void PopShading()
        {
            mShadingStack.Pop();
        }

        /// <summary>
        /// Resolves automatic font color to black or white depending on the current background.
        /// </summary>
        internal DrColor ResolveFontColor(DrColor color)
        {
            // If there is no background, assume it is white paper, make the color black.
            return ResolveFontColor(color, DrColor.Black);
        }

        /// <summary>
        /// Resolves automatic font color to black or white depending on the current background.
        /// If there is no background returns <paramref name="defaultColor"/>
        /// </summary>
        internal DrColor ResolveFontColor(DrColor color, DrColor defaultColor)
        {
            if (!color.IsEmpty)
                return color;

            Shading shading = mShadingStack.Peek();

            // At the moment only Shading objects can be on the stack, but later
            // I might have backgrounds of VML shapes on the stack too.

            // If there is no background, use default color.
            return (shading == null) ? defaultColor : ResolveAutoFontColorOnShading(shading);
        }

        /// <summary>
        /// Returns true if the specified shading is visible for rendering, i.e. is not the same as its background shading.
        /// </summary>
        internal bool IsRenderShading(Shading shading, bool optimizeOutput)
        {
            // WORDSNET-15199 Skip rendering shading of background color only if SAveOptions.OptimizeOutput flag is set.
            if (!optimizeOutput)
                return true;

            if (!IsVisible(shading))
                return false;

            // Always return true if current shading has texture.
            if (((shading.Texture != TextureIndex.TextureNone && shading.Texture != TextureIndex.TextureNil)))
                return true;

            // This is supposed to be the same shading as passed as parameter.
            // Keep it to restore stack when finish processing.
            Shading currentShading = mShadingStack.Top();

            PopShading();

            Shading backShading = mShadingStack.Top();

            bool result;
            // If background shading has texture, the passes shading must be rendered.
            if ((backShading != null) &&
                (backShading.Texture != TextureIndex.TextureNone) &&
                (backShading.Texture != TextureIndex.TextureNil))
            {
                result = true;
            }
            else
            {
                DrColor backColor = (backShading != null) ? backShading.BackgroundPatternColorInternal : gPaperColor;
                result = backColor != shading.BackgroundPatternColorInternal;
            }

            // Restore current shading.
            mShadingStack.Push(currentShading);

            return result;
        }

        /// <summary>
        /// In Microsoft Word, auto background color is resolved into "paper" or window background color.
        /// </summary>
        internal static DrColor ResolveBackgroundPatternColor(DrColor color)
        {
            return (color.IsEmpty) ? gPaperColor : color;
        }

        /// <summary>
        /// Auto foreground color of the texture seems to be always black in Microsoft Word.
        /// </summary>
        internal static DrColor ResolveForegroundPatternColor(DrColor color)
        {
            return (color.IsEmpty) ? DrColor.Black : color;
        }

        /// <summary>
        /// Returns the color of the font (black or white) to be used on the given shading.
        /// You need to call this only if your font color is auto color.
        /// </summary>
        /// <param name="shading">The background texture and color. Must not be <c>null</c>.</param>
        /// <returns>Black or white font color.</returns>
        internal static DrColor ResolveAutoFontColorOnShading(Shading shading)
        {
            TextureIndex texture = shading.Texture;

            if ((texture == TextureIndex.TextureNone) || (texture == TextureIndex.TextureNil))
            {
                if (shading.BackgroundPatternColorInternal.IsEmpty)
                {
                    // There is no texture and no background color,
                    // so the background is assumed white paper and the font is black.
                    return DrColor.Black;
                }
                else
                {
                    // There is no texture, but background color is specified, it results in a solid background.
                    float luminance = shading.BackgroundPatternColorInternal.GetLuminance();
                    return GetAutoColorFromBackLuminance(luminance);
                }
            }
            else if (WordUtil.IsComplexTexture(texture))
            {
                // The pattern is a texture (not a percent fill) and from what I can see
                // Microsoft Word always uses black font color on it regardless of the pattern colors.
                return DrColor.Black;
            }
            else
            {
                // The pattern is a percent fill.
                float fillDensity = (float)WordUtil.GetFillingLevelByTextureIndex(texture);
                // Automatic fill color is resolved to black in MS Word.
                DrColor actualFillColor = ResolveForegroundPatternColor(shading.ForegroundPatternColorInternal);
                float foreLuminance = actualFillColor.GetLuminance() * fillDensity;

                // For example, if fill density is 80% then background density is 20%.
                float backDensity = 1.0f - fillDensity;
                // In Microsoft Word, the automatic background color is resolved to "paper" color.
                DrColor actualBackColor = ResolveBackgroundPatternColor(shading.BackgroundPatternColorInternal);
                float backLuminance = actualBackColor.GetLuminance() * backDensity;

                // It is a percent shading. The appropriate parts of luminance of background
                // and foreground color are combined to create the total luminance of the background.
                float luminance = foreLuminance + backLuminance;
                return GetAutoColorFromBackLuminance(luminance);
            }
        }

        private static DrColor GetAutoColorFromBackLuminance(float luminance)
        {
            return (luminance > BackgroundLuminanceThreshold) ? DrColor.Black : DrColor.White;
        }

        /// <summary>
        /// Checks that shading is visible. Returns <c>false</c> for <c>null</c> parameter.
        /// </summary>
        private static bool IsVisible(Shading shading)
        {
            return (shading != null) && (shading.IsVisible);
        }

        private readonly Stack<Shading> mShadingStack = new Stack<Shading>();

        /// <summary>
        /// Auto font color is black on backgrounds that are brighter than this threshold.
        /// Auto font color is white on backgrounds that are darker than this threshold.
        /// Found this value experimentally in MS Word. It is close, but not EXACTLY like MS Word.
        /// It is still possible to come up with colors where incorrect font color will be selected.
        ///
        /// The current value is relevant for MS Word 2016 and later versions.
        /// Earlier versions used ~0.2383f as a luminance threshold.
        ///
        /// See https://www.w3.org/TR/AERT/#color-contrast for more info about luminance calculation.
        /// </summary>
        private const float BackgroundLuminanceThreshold = 0.2345f;

        /// <summary>
        /// Automatic background pattern color is resolved into paper color in Microsoft Word.
        /// </summary>
        private static readonly DrColor gPaperColor = DrColor.White;
    }
}

