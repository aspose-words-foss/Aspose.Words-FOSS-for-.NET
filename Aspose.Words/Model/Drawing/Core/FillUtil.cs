// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/04/2021 by Vadim Saltykov

using System;
using System.Drawing.Drawing2D;
using System.IO;
using Aspose.Collections;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.IO;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.RW.Factories;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Helper class containing methods used for fill formatting.
    /// </summary>
    internal static class FillUtil
    {
        /// <summary>
        /// Returns <see cref="PresetTexture"/> according to the specified hash value.
        /// </summary>
        internal static PresetTexture GetTextureByHash(int textureHash)
        {
            unchecked
            {
                switch (textureHash)
                {
                    case 0x4b6f30f3:
                        return PresetTexture.BlueTissuePaper;
                    case 0x19f95cce:
                        return PresetTexture.Bouquet;
                    case 0x6060b43e:
                        return PresetTexture.BrownMarble;
                    case 0x749f777e:
                        return PresetTexture.Canvas;
                    case 0x7d93c35b:
                        return PresetTexture.Cork;
                    case 0x49d6b1d0:
                        return PresetTexture.Denim;
                    case (int)0xc4befeea:
                        return PresetTexture.FishFossil;
                    case 0x520dad48:
                        return PresetTexture.Granite;
                    case 0x229b9757:
                        return PresetTexture.GreenMarble;
                    case 0x706c7a5:
                        return PresetTexture.MediumWood;
                    case (int)0xf7b9214c:
                        return PresetTexture.Newsprint;
                    case 0x7193b784:
                        return PresetTexture.Oak;
                    case (int)0xac1bca2c:
                        return PresetTexture.PaperBag;
                    case (int)0x82bdb4e2:
                        return PresetTexture.Papyrus;
                    case (int)0xc64275c1:
                        return PresetTexture.Parchment;
                    case (int)0xe58841fa:
                        return PresetTexture.PinkTissuePaper;
                    case 0xa02b859:
                        return PresetTexture.PurpleMesh;
                    case 0x514b7f5a:
                        return PresetTexture.RecycledPaper;
                    case (int)0xb28a8bc0:
                        return PresetTexture.Sand;
                    case 0x2bb08a79:
                        return PresetTexture.Stationery;
                    case (int)0xfaedF5e7:
                        return PresetTexture.Walnut;
                    case 0x7af8100d:
                        return PresetTexture.WaterDroplets;
                    case (int)0xaa54ea96:
                        return PresetTexture.WhiteMarble;
                    case (int)0xd993b560:
                        return PresetTexture.WovenMat;
                    default:
                        return PresetTexture.None;
                }
            }
        }

        /// <summary>
        /// Returns the texture name according to the specified <see cref="PresetTexture"/>.
        /// </summary>
        internal static string GetTextureName(PresetTexture presetTexture)
        {
            switch (presetTexture)
            {
                case PresetTexture.BlueTissuePaper:
                    return "BlueTissuePaper";
                case PresetTexture.Bouquet:
                    return "Bouquet";
                case PresetTexture.BrownMarble:
                    return "BrownMarble";
                case PresetTexture.Canvas:
                    return "Canvas";
                case PresetTexture.Cork:
                    return "Cork";
                case PresetTexture.Denim:
                    return "Denim";
                case PresetTexture.FishFossil:
                    return "FishFossil";
                case PresetTexture.Granite:
                    return "Granite";
                case PresetTexture.GreenMarble:
                    return "GreenMarble";
                case PresetTexture.MediumWood:
                    return "MediumWood";
                case PresetTexture.Newsprint:
                    return "Newsprint";
                case PresetTexture.Oak:
                    return "Oak";
                case PresetTexture.PaperBag:
                    return "PaperBag";
                case PresetTexture.Papyrus:
                    return "Papyrus";
                case PresetTexture.Parchment:
                    return "Parchment";
                case PresetTexture.PinkTissuePaper:
                    return "PinkTissuePaper";
                case PresetTexture.PurpleMesh:
                    return "PurpleMesh";
                case PresetTexture.RecycledPaper:
                    return "RecycledPaper";
                case PresetTexture.Sand:
                    return "Sand";
                case PresetTexture.Stationery:
                    return "Stationery";
                case PresetTexture.Walnut:
                    return "Walnut";
                case PresetTexture.WaterDroplets:
                    return "WaterDroplets";
                case PresetTexture.WhiteMarble:
                    return "WhiteMarble";
                case PresetTexture.WovenMat:
                    return "WovenMat";
                default:
                    throw new ArgumentOutOfRangeException("presetTexture", "The specified texture is out of range.");
            }
        }

        /// <summary>
        /// Returns the pattern name according to the specified <see cref="PatternType"/>.
        /// </summary>
        internal static string GetPatternName(PatternType patternType)
        {
            switch (patternType)
            {
                case PatternType.Percent5:
                    return "5Percent";
                case PatternType.Percent10:
                    return "10Percent";
                case PatternType.Percent20:
                    return "20Percent";
                case PatternType.Percent25:
                    return "25Percent";
                case PatternType.Percent30:
                    return "30Percent";
                case PatternType.Percent40:
                    return "40Percent";
                case PatternType.Percent50:
                    return "50Percent";
                case PatternType.Percent60:
                    return "60Percent";
                case PatternType.Percent70:
                    return "70Percent";
                case PatternType.Percent75:
                    return "75Percent";
                case PatternType.Percent80:
                    return "80Percent";
                case PatternType.Percent90:
                    return "90Percent";
                case PatternType.Cross:
                    return "Cross";
                case PatternType.DarkDownwardDiagonal:
                    return "DarkDownwardDiagonal";
                case PatternType.DarkHorizontal:
                    return "DarkHorizontal";
                case PatternType.DarkUpwardDiagonal:
                    return "DarkUpwardDiagonal";
                case PatternType.DarkVertical:
                    return "DarkVertical";
                case PatternType.DashedDownwardDiagonal:
                    return "DashedDownwardDiagonal";
                case PatternType.DashedHorizontal:
                    return "DashedHorizontal";
                case PatternType.DashedUpwardDiagonal:
                    return "DashedUpwardDiagonal";
                case PatternType.DashedVertical:
                    return "DashedVertical";
                case PatternType.DiagonalBrick:
                    return "DiagonalBrick";
                case PatternType.DiagonalCross:
                    return "DiagonalCross";
                case PatternType.Divot:
                    return "Divot";
                case PatternType.DottedDiamond:
                    return "DottedDiamond";
                case PatternType.DottedGrid:
                    return "DottedGrid";
                case PatternType.DownwardDiagonal:
                    return "DownwardDiagonal";
                case PatternType.Horizontal:
                    return "Horizontal";
                case PatternType.HorizontalBrick:
                    return "HorizontalBrick";
                case PatternType.LargeCheckerBoard:
                    return "LargeCheckerBoard";
                case PatternType.LargeConfetti:
                    return "LargeConfetti";
                case PatternType.LargeGrid:
                    return "LargeGrid";
                case PatternType.LightDownwardDiagonal:
                    return "LightDownwardDiagonal";
                case PatternType.LightHorizontal:
                    return "LightHorizontal";
                case PatternType.LightUpwardDiagonal:
                    return "LightUpwardDiagonal";
                case PatternType.LightVertical:
                    return "LightVertical";
                case PatternType.NarrowHorizontal:
                    return "NarrowHorizontal";
                case PatternType.NarrowVertical:
                    return "NarrowVertical";
                case PatternType.OutlinedDiamond:
                    return "OutlinedDiamond";
                case PatternType.Plaid:
                    return "Plaid";
                case PatternType.Shingle:
                    return "Shingle";
                case PatternType.SmallCheckerBoard:
                    return "SmallCheckerBoard";
                case PatternType.SmallConfetti:
                    return "SmallConfetti";
                case PatternType.SmallGrid:
                    return "SmallGrid";
                case PatternType.SolidDiamond:
                    return "SolidDiamond";
                case PatternType.Sphere:
                    return "Sphere";
                case PatternType.Trellis:
                    return "Trellis";
                case PatternType.UpwardDiagonal:
                    return "UpwardDiagonal";
                case PatternType.Vertical:
                    return "Vertical";
                case PatternType.Wave:
                    return "Wave";
                case PatternType.Weave:
                    return "Weave";
                case PatternType.WideDownwardDiagonal:
                    return "WideDownwardDiagonal";
                case PatternType.WideUpwardDiagonal:
                    return "WideUpwardDiagonal";
                case PatternType.ZigZag:
                    return "ZigZag";
                default:
                    throw new ArgumentOutOfRangeException("patternType", "The specified pattern is out of range.");
            }
        }

        /// <summary>
        /// Returns <see cref="PatternType"/> according to the specified image data.
        /// </summary>
        internal static PatternType GetPatternType(byte[] binData)
        {
            HatchStyle style = RW.Vml.VmlEnum.GetHatchStyle(binData);
            if ((style != HatchStyle.Horizontal) && (style != HatchStyle.Vertical) &&
#if NETSTANDARD || JAVA
                // .NET Standard code cannot distinguish WideDownwardDiagonal vs DownwardDiagonal and WideUpwardDiagonal vs UpwardDiagonal.
                // The reason seems to be BitonalConverter, it is hard to tune it to work like in .NET.
                // So fall back to the code below to determine the pattern type by binData hash.
                (style != HatchStyle.WideUpwardDiagonal) && (style != HatchStyle.WideDownwardDiagonal) &&
#endif
                (style != HatchStyle.BackwardDiagonal) && (style != HatchStyle.ForwardDiagonal) &&
                (style != HatchStyle.DiagonalCross))
                return HatchStyleToPatternType(style);

            int textureHash = HashUtil.GetSHA512Hash(binData).GetHashCode();
            unchecked
            {
                switch (textureHash)
                {
                    case (int)0xa58e7134:
                        return PatternType.Horizontal;
                    case (int)0x865eeb88:
                        return PatternType.Vertical;
                    case (int)0xc23f7cc5:
                        return PatternType.DownwardDiagonal;
                    case 0x14226ec2:
                        return PatternType.UpwardDiagonal;
                    case 0x23bb9fa8:
                        return PatternType.DiagonalCross;
#if NETSTANDARD || JAVA
                    // Not sure why, but in TestShapePatternedVml and in TestShapePatternedVml the same patterns has different hashes.
                    case (int)0x90780831:
                    case 0xb0cc343:
                        return PatternType.WideUpwardDiagonal;
                    case (int)0xe2eb1a93:
                    case (int)0xacde8b53:
                        return PatternType.WideDownwardDiagonal;
#endif
                    default:
                        return PatternType.None;
                }
            }
        }

        /// <summary>
        /// Returns the texture title according to the specified <see cref="PresetTexture"/>.
        /// </summary>
        internal static string GetTextureTitle(PresetTexture presetTexture)
        {
            switch (presetTexture)
            {
                case PresetTexture.BlueTissuePaper:
                    return "Blue tissue paper";
                case PresetTexture.Bouquet:
                    return "Bouquet";
                case PresetTexture.BrownMarble:
                    return "Brown marble";
                case PresetTexture.Canvas:
                    return "Canvas";
                case PresetTexture.Cork:
                    return "Cork";
                case PresetTexture.Denim:
                    return "Denim";
                case PresetTexture.FishFossil:
                    return "Fish fossil";
                case PresetTexture.Granite:
                    return "Granite";
                case PresetTexture.GreenMarble:
                    return "Green marble";
                case PresetTexture.MediumWood:
                    return "Medium wood";
                case PresetTexture.Newsprint:
                    return "Newsprint";
                case PresetTexture.Oak:
                    return "Oak";
                case PresetTexture.PaperBag:
                    return "Paper bag";
                case PresetTexture.Papyrus:
                    return "Papyrus";
                case PresetTexture.Parchment:
                    return "Parchment";
                case PresetTexture.PinkTissuePaper:
                    return "Pink tissue paper";
                case PresetTexture.PurpleMesh:
                    return "Purple mesh";
                case PresetTexture.RecycledPaper:
                    return "Recycled paper";
                case PresetTexture.Sand:
                    return "Sand";
                case PresetTexture.Stationery:
                    return "Stationery";
                case PresetTexture.Walnut:
                    return "Walnut";
                case PresetTexture.WaterDroplets:
                    return "Water droplets";
                case PresetTexture.WhiteMarble:
                    return "White marble";
                case PresetTexture.WovenMat:
                    return "Woven mat";
                default:
                    throw new ArgumentOutOfRangeException("presetTexture", "The specified texture is out of range.");
            }
        }

        /// <summary>
        /// Returns the value <see cref="DrColor"/> according to the specified <see cref="PresetTexture"/>.
        /// </summary>
        internal static DrColor GetFillColor(PresetTexture presetTexture)
        {
            switch (presetTexture)
            {
                case PresetTexture.BlueTissuePaper:
                    return DrColor.FromArgb(204, 236, 255);
                case PresetTexture.Bouquet:
                    return DrColor.FromArgb(204, 204, 255);
                case PresetTexture.BrownMarble:
                    return DrColor.FromArgb(102, 51, 0);
                case PresetTexture.Canvas:
                    return DrColor.FromArgb(255, 255, 204);
                case PresetTexture.Cork:
                    return DrColor.FromArgb(153, 102, 0);
                case PresetTexture.Denim:
                    return DrColor.FromArgb(102, 153, 255);
                case PresetTexture.FishFossil:
                case PresetTexture.Oak:
                case PresetTexture.PaperBag:
                case PresetTexture.Papyrus:
                    return DrColor.FromArgb(255, 204, 153);
                case PresetTexture.Granite:
                    return DrColor.FromArgb(221, 221, 221);
                case PresetTexture.GreenMarble:
                    return DrColor.FromArgb(0, 102, 0);
                case PresetTexture.MediumWood:
                    return DrColor.FromArgb(153, 102, 51);
                case PresetTexture.Newsprint:
                    return DrColor.FromArgb(248, 248, 248);
                case PresetTexture.Parchment:
                    return DrColor.FromArgb(255, 255, 204);
                case PresetTexture.PinkTissuePaper:
                    return DrColor.FromArgb(255, 204, 204);
                case PresetTexture.PurpleMesh:
                    return DrColor.FromArgb(153, 0, 204);
                case PresetTexture.RecycledPaper:
                case PresetTexture.WhiteMarble:
                    return null;
                case PresetTexture.Sand:
                    return DrColor.Silver;
                case PresetTexture.Stationery:
                    return DrColor.FromArgb(255, 255, 204);
                case PresetTexture.Walnut:
                    return DrColor.FromArgb(102, 51, 0);
                case PresetTexture.WaterDroplets:
                    return DrColor.FromArgb(204, 255, 255);
                case PresetTexture.WovenMat:
                    return DrColor.FromArgb(255, 204, 102);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the texture binary data according to the specified texture name.
        /// </summary>
        internal static byte[] GetTextureBytes(string textureName)
        {
            byte[] bytes = ResourceBytesCache[textureName];
            if ((bytes == null) || (bytes.Length == 0))
            {
                bytes = LoadResource(string.Format("Aspose.Words.Resources.PresetTextures.{0}.jpeg", textureName));
                ResourceBytesCache[textureName] = bytes;
            }

            return bytes;
        }

        /// <summary>
        /// Returns the pattern binary data according to the specified pattern name.
        /// </summary>
        internal static byte[] GetPatternBytes(string patternName)
        {
            byte[] bytes = ResourceBytesCache[patternName];
            if ((bytes == null) || (bytes.Length == 0))
            {
                bytes = LoadResource(string.Format("Aspose.Words.Resources.Patterns.{0}.png", patternName));
                ResourceBytesCache[patternName] = bytes;
            }

            return bytes;
        }

        /// <summary>
        /// Returns PatternType according to the specified HatchStyle.
        /// </summary>
        /// <remarks>
        /// HatchStyle.Cross processed as the special case. HatchStyle.Min and HatchStyle.Max not currently used.
        /// </remarks>
        internal static PatternType HatchStyleToPatternType(HatchStyle value)
        {
            switch (value)
            {
                case HatchStyle.BackwardDiagonal:
                    return PatternType.DownwardDiagonal;
                case HatchStyle.ForwardDiagonal:
                    return PatternType.UpwardDiagonal;
                case HatchStyle.LargeGrid:
                    return PatternType.LargeGrid;
                case HatchStyle.Horizontal:
                    return PatternType.Horizontal;
                case HatchStyle.Vertical:
                    return PatternType.Vertical;
                case HatchStyle.DiagonalCross:
                    return PatternType.DiagonalCross;
                case HatchStyle.Percent05:
                    return PatternType.Percent5;
                case HatchStyle.Percent10:
                    return PatternType.Percent10;
                case HatchStyle.Percent20:
                    return PatternType.Percent20;
                case HatchStyle.Percent25:
                    return PatternType.Percent25;
                case HatchStyle.Percent30:
                    return PatternType.Percent30;
                case HatchStyle.Percent40:
                    return PatternType.Percent40;
                case HatchStyle.Percent50:
                    return PatternType.Percent50;
                case HatchStyle.Percent60:
                    return PatternType.Percent60;
                case HatchStyle.Percent70:
                    return PatternType.Percent70;
                case HatchStyle.Percent75:
                    return PatternType.Percent75;
                case HatchStyle.Percent80:
                    return PatternType.Percent80;
                case HatchStyle.Percent90:
                    return PatternType.Percent90;
                case HatchStyle.LightDownwardDiagonal:
                    return PatternType.LightDownwardDiagonal;
                case HatchStyle.LightUpwardDiagonal:
                    return PatternType.LightUpwardDiagonal;
                case HatchStyle.DarkDownwardDiagonal:
                    return PatternType.DarkDownwardDiagonal;
                case HatchStyle.DarkUpwardDiagonal:
                    return PatternType.DarkUpwardDiagonal;
                case HatchStyle.WideDownwardDiagonal:
                    return PatternType.WideDownwardDiagonal;
                case HatchStyle.WideUpwardDiagonal:
                    return PatternType.WideUpwardDiagonal;
                case HatchStyle.LightVertical:
                    return PatternType.LightVertical;
                case HatchStyle.LightHorizontal:
                    return PatternType.LightHorizontal;
                case HatchStyle.NarrowVertical:
                    return PatternType.NarrowVertical;
                case HatchStyle.NarrowHorizontal:
                    return PatternType.NarrowHorizontal;
                case HatchStyle.DarkVertical:
                    return PatternType.DarkVertical;
                case HatchStyle.DarkHorizontal:
                    return PatternType.DarkHorizontal;
                case HatchStyle.DashedDownwardDiagonal:
                    return PatternType.DashedDownwardDiagonal;
                case HatchStyle.DashedUpwardDiagonal:
                    return PatternType.DashedUpwardDiagonal;
                case HatchStyle.DashedHorizontal:
                    return PatternType.DashedHorizontal;
                case HatchStyle.DashedVertical:
                    return PatternType.DashedVertical;
                case HatchStyle.SmallConfetti:
                    return PatternType.SmallConfetti;
                case HatchStyle.LargeConfetti:
                    return PatternType.LargeConfetti;
                case HatchStyle.ZigZag:
                    return PatternType.ZigZag;
                case HatchStyle.Wave:
                    return PatternType.Wave;
                case HatchStyle.DiagonalBrick:
                    return PatternType.DiagonalBrick;
                case HatchStyle.HorizontalBrick:
                    return PatternType.HorizontalBrick;
                case HatchStyle.Weave:
                    return PatternType.Weave;
                case HatchStyle.Plaid:
                    return PatternType.Plaid;
                case HatchStyle.Divot:
                    return PatternType.Divot;
                case HatchStyle.DottedGrid:
                    return PatternType.DottedGrid;
                case HatchStyle.DottedDiamond:
                    return PatternType.DottedDiamond;
                case HatchStyle.Shingle:
                    return PatternType.Shingle;
                case HatchStyle.Trellis:
                    return PatternType.Trellis;
                case HatchStyle.Sphere:
                    return PatternType.Sphere;
                case HatchStyle.SmallGrid:
                    return PatternType.SmallGrid;
                case HatchStyle.SmallCheckerBoard:
                    return PatternType.SmallCheckerBoard;
                case HatchStyle.LargeCheckerBoard:
                    return PatternType.LargeCheckerBoard;
                case HatchStyle.OutlinedDiamond:
                    return PatternType.OutlinedDiamond;
                case HatchStyle.SolidDiamond:
                    return PatternType.SolidDiamond;
                default:
                    return PatternType.None;
            }
        }

        /// <summary>
        /// Returns HatchStyle according to the specified PatternType.
        /// </summary>
        internal static HatchStyle GetHatchStyle(PatternType value)
        {
            switch (value)
            {
                case PatternType.DownwardDiagonal:
                    return HatchStyle.BackwardDiagonal;
                case PatternType.UpwardDiagonal:
                    return HatchStyle.ForwardDiagonal;
                case PatternType.LargeGrid:
                    return HatchStyle.LargeGrid;
                case PatternType.Horizontal:
                    return HatchStyle.Horizontal;
                case PatternType.Vertical:
                    return HatchStyle.Vertical;
                case PatternType.DiagonalCross:
                    return HatchStyle.DiagonalCross;
                case PatternType.Percent5:
                    return HatchStyle.Percent05;
                case PatternType.Percent10:
                    return HatchStyle.Percent10;
                case PatternType.Percent20:
                    return HatchStyle.Percent20;
                case PatternType.Percent25:
                    return HatchStyle.Percent25;
                case PatternType.Percent30:
                    return HatchStyle.Percent30;
                case PatternType.Percent40:
                    return HatchStyle.Percent40;
                case PatternType.Percent50:
                    return HatchStyle.Percent50;
                case PatternType.Percent60:
                    return HatchStyle.Percent60;
                case PatternType.Percent70:
                    return HatchStyle.Percent70;
                case PatternType.Percent75:
                    return HatchStyle.Percent75;
                case PatternType.Percent80:
                    return HatchStyle.Percent80;
                case PatternType.Percent90:
                    return HatchStyle.Percent90;
                case PatternType.LightDownwardDiagonal:
                    return HatchStyle.LightDownwardDiagonal;
                case PatternType.LightUpwardDiagonal:
                    return HatchStyle.LightUpwardDiagonal;
                case PatternType.DarkDownwardDiagonal:
                    return HatchStyle.DarkDownwardDiagonal;
                case PatternType.DarkUpwardDiagonal:
                    return HatchStyle.DarkUpwardDiagonal;
                case PatternType.WideDownwardDiagonal:
                    return HatchStyle.WideDownwardDiagonal;
                case PatternType.WideUpwardDiagonal:
                    return HatchStyle.WideUpwardDiagonal;
                case PatternType.LightVertical:
                    return HatchStyle.LightVertical;
                case PatternType.LightHorizontal:
                    return HatchStyle.LightHorizontal;
                case PatternType.NarrowVertical:
                    return HatchStyle.NarrowVertical;
                case PatternType.NarrowHorizontal:
                    return HatchStyle.NarrowHorizontal;
                case PatternType.DarkVertical:
                    return HatchStyle.DarkVertical;
                case PatternType.DarkHorizontal:
                    return HatchStyle.DarkHorizontal;
                case PatternType.DashedDownwardDiagonal:
                    return HatchStyle.DashedDownwardDiagonal;
                case PatternType.DashedUpwardDiagonal:
                    return HatchStyle.DashedUpwardDiagonal;
                case PatternType.DashedHorizontal:
                    return HatchStyle.DashedHorizontal;
                case PatternType.DashedVertical:
                    return HatchStyle.DashedVertical;
                case PatternType.SmallConfetti:
                    return HatchStyle.SmallConfetti;
                case PatternType.LargeConfetti:
                    return HatchStyle.LargeConfetti;
                case PatternType.ZigZag:
                    return HatchStyle.ZigZag;
                case PatternType.Wave:
                    return HatchStyle.Wave;
                case PatternType.DiagonalBrick:
                    return HatchStyle.DiagonalBrick;
                case PatternType.HorizontalBrick:
                    return HatchStyle.HorizontalBrick;
                case PatternType.Weave:
                    return HatchStyle.Weave;
                case PatternType.Plaid:
                    return HatchStyle.Plaid;
                case PatternType.Divot:
                    return HatchStyle.Divot;
                case PatternType.DottedGrid:
                    return HatchStyle.DottedGrid;
                case PatternType.DottedDiamond:
                    return HatchStyle.DottedDiamond;
                case PatternType.Shingle:
                    return HatchStyle.Shingle;
                case PatternType.Trellis:
                    return HatchStyle.Trellis;
                case PatternType.Sphere:
                    return HatchStyle.Sphere;
                case PatternType.SmallGrid:
                    return HatchStyle.SmallGrid;
                case PatternType.SmallCheckerBoard:
                    return HatchStyle.SmallCheckerBoard;
                case PatternType.LargeCheckerBoard:
                    return HatchStyle.LargeCheckerBoard;
                case PatternType.OutlinedDiamond:
                    return HatchStyle.OutlinedDiamond;
                case PatternType.SolidDiamond:
                    return HatchStyle.SolidDiamond;
                case PatternType.Cross:
                    return HatchStyle.Cross;
                default:
                    return HatchStyle.Min;
            }
        }

        /// <summary>
        /// Returns TextureAlignment according to the specified DmlRectangleAlignment.
        /// </summary>
        internal static TextureAlignment DmlRectangleToTextureAlignment(DmlRectangleAlignment value)
        {
            switch (value)
            {
                case DmlRectangleAlignment.TopLeft:
                    return TextureAlignment.TopLeft;
                case DmlRectangleAlignment.Top:
                    return TextureAlignment.Top;
                case DmlRectangleAlignment.TopRight:
                    return TextureAlignment.TopRight;
                case DmlRectangleAlignment.Left:
                    return TextureAlignment.Left;
                case DmlRectangleAlignment.Center:
                    return TextureAlignment.Center;
                case DmlRectangleAlignment.Right:
                    return TextureAlignment.Right;
                case DmlRectangleAlignment.BottomLeft:
                    return TextureAlignment.BottomLeft;
                case DmlRectangleAlignment.Bottom:
                    return TextureAlignment.Bottom;
                case DmlRectangleAlignment.BottomRight:
                    return TextureAlignment.BottomRight;
                default:
                    return TextureAlignment.None;
            }
        }

        /// <summary>
        /// Returns DmlRectangleAlignment according to the specified TextureAlignment.
        /// </summary>
        internal static DmlRectangleAlignment TextureToDmlRectangleAlignment(TextureAlignment value)
        {
            switch (value)
            {
                case TextureAlignment.TopLeft:
                    return DmlRectangleAlignment.TopLeft;
                case TextureAlignment.Top:
                    return DmlRectangleAlignment.Top;
                case TextureAlignment.TopRight:
                    return DmlRectangleAlignment.TopRight;
                case TextureAlignment.Left:
                    return DmlRectangleAlignment.Left;
                case TextureAlignment.Center:
                    return DmlRectangleAlignment.Center;
                case TextureAlignment.Right:
                    return DmlRectangleAlignment.Right;
                case TextureAlignment.BottomLeft:
                    return DmlRectangleAlignment.BottomLeft;
                case TextureAlignment.Bottom:
                    return DmlRectangleAlignment.Bottom;
                case TextureAlignment.BottomRight:
                    return DmlRectangleAlignment.BottomRight;
                default:
                    return DmlRectangleAlignment.None;
            }
        }

        /// <summary>
        /// Returns <see cref="DrColor"/> according to the specified <see cref="ThemeColor"/>.
        /// </summary>
        internal static DrColor ThemeToNativeColor(ThemeColor themeColor, Theme theme)
        {
            // Word VBA resets wdNotThemeColor to Color.White.
            if (themeColor == ThemeColor.None)
                return DrColor.White;

            DmlSchemeColor schemeColor = new DmlSchemeColor(themeColor);

            if (theme == null)
                theme = DefaultTheme;

            return schemeColor.CreateDrColor(theme, null);
        }

        /// <summary>
        /// Returns <see cref="ThemeColor"/> according to the specified <see cref="DrColor"/>.
        /// </summary>
        internal static ThemeColor NativeToThemeColor(DrColor nativeColor, Theme theme)
        {
            ThemeColor[] themeColors = new ThemeColor[]
            {
                ThemeColor.Accent1, ThemeColor.Accent2, ThemeColor.Accent3, ThemeColor.Accent4, ThemeColor.Accent5,
                ThemeColor.Accent6, ThemeColor.Light1, ThemeColor.Light2, ThemeColor.Dark1, ThemeColor.Dark2,
                ThemeColor.Text1, ThemeColor.Text2, ThemeColor.Background1, ThemeColor.Background2,
                ThemeColor.FollowedHyperlink, ThemeColor.Hyperlink
            };

            if (nativeColor == null)
                return ThemeColor.None;

            foreach (ThemeColor themeColor in themeColors)
            {
                DrColor curColor = ThemeToNativeColor(themeColor, theme);
                if (curColor.Equals(nativeColor))
                    return themeColor;
            }

            return ThemeColor.None;
        }

        /// <summary>
        /// Returns the integer value of TintAndShade for VML.
        /// </summary>
        internal static int VmlTintAndShadeModifier(double value)
        {
            return RoundNear(255 * (1 - System.Math.Abs(value)));
        }

        /// <summary>
        /// Returns the <see cref="DrColor"/> converted as VML Tint modifier.
        /// </summary>
        /// <remarks>
        /// The algorithm is interpolated by MS Word VBA multiple results.
        /// Specifications for the original algorithm have not been found.
        /// </remarks>
        internal static DrColor VmlTint(DrColor color, int modifier)
        {
            double tintFactor = 1.0d - (modifier / 255.0d);

            return DrColor.FromArgb(
                RoundNear(color.R + (255 - color.R) * tintFactor),
                RoundNear(color.G + (255 - color.G) * tintFactor),
                RoundNear(color.B + (255 - color.B) * tintFactor));
        }

        /// <summary>
        /// Returns the <see cref="DrColor"/> converted as VML Shade modifier.
        /// </summary>
        /// <remarks>
        /// The algorithm is interpolated by MS Word VBA multiple results.
        /// Specifications for the original algorithm have not been found.
        /// </remarks>
        internal static DrColor VmlShade(DrColor color, int modifier)
        {
            double shadeFactor = 1.0d - (modifier / 255.0d);

            return DrColor.FromArgb(
                (int)(color.R * (1 - shadeFactor)),
                (int)(color.G * (1 - shadeFactor)),
                (int)(color.B * (1 - shadeFactor)));
        }

        /// <summary>
        /// Rounds up to the nearest integer value, casts result to int.
        /// </summary>
        private static int RoundNear(double value)
        {
            return (int)System.Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }

        private static byte[] LoadResource(string resourceName)
        {
            try
            {
                using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
                {
                    byte[] bytes = StreamUtil.CopyStreamToByteArray(stream);
                    return bytes;
                }
            }
            catch
            {
                return new byte[0];
            }
        }

        private static StringToObjDictionary<byte[]> ResourceBytesCache
        {
            get
            {
                if (gResourceBytesCache == null)
                {
                    lock (gResourceBytesCacheSyncRoot)
                    {
                        if (gResourceBytesCache == null)
                            gResourceBytesCache = new StringToObjDictionary<byte[]>();
                    }
                }

                return gResourceBytesCache;
            }
        }

        private static Theme DefaultTheme
        {
            get
            {
                if (gDefaultTheme == null)
                {
                    lock (gDefaultThemeSyncRoot)
                    {
                        IDefaultThemeProvider defaultThemeProvider = ReaderFactory.CreateDefaultThemeProvider();
                        gDefaultTheme = defaultThemeProvider.GetDefaultTheme();
                    }
                }

                return gDefaultTheme;
            }
        }

        private static Theme gDefaultTheme;
        private static readonly object gDefaultThemeSyncRoot = new object();
        private static StringToObjDictionary<byte[]> gResourceBytesCache;
        private static readonly object gResourceBytesCacheSyncRoot = new object();
    }
}
