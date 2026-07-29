// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2015 by Vadim Polienko

using System;
using System.IO;
using System.Text;
using Aspose.Images;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#else
using Image = System.Drawing.Image;
#endif

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Common functionality for FieldDisplayBarcode and FieldMergeBarcode
    /// </summary>
    [JavaManual]
    internal static class FieldBarcodeUtil
    {
        /// <summary>
        /// Straightforward processing of MERGEBARCODE field.
        /// This is done just like Word does - replacing by DISPLAYBARCODE field.
        /// </summary>
        /// <param name="field"></param>
        internal static void ReplaceMergeBarcode(Field field)
        {
            // Create new field code from current arguments & switches.
            string newFieldCode = RebuildFieldCode(field, "DISPLAYBARCODE");

            // Modify field type.
            field.SetFieldType(FieldType.FieldDisplayBarcode);

            // Set new field code and forcibly invalidate field code cache.
            field.SetFieldCode(newFieldCode);
            field.InvalidateFieldCodeCache();

            // Remove the result.
            field.RemoveFieldResult();
        }

        /// <summary>
        /// Rebuild full field code from field arguments and switches (in FieldCodeCache).
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldTypeName"></param>
        /// <returns>Field code</returns>
        private static string RebuildFieldCode(Field field, string fieldTypeName)
        {
            FieldCode fieldCodeCache = field.FieldCodeCache;

            bool isInQuotes = field.Result.StartsWith("\"", StringComparison.Ordinal) &&
                field.Result.EndsWith("\"", StringComparison.Ordinal);
            StringBuilder sb = new StringBuilder();

            sb.Append(fieldTypeName ?? fieldCodeCache.FieldType);

            sb.Append(' ');

            if (!isInQuotes)
                sb.Append('"');

            sb.Append(field.Result);

            if (!isInQuotes)
                sb.Append('"');

            for (int i = 1; i < fieldCodeCache.Arguments.Count; i++)
            {
                sb.Append(' ');
                sb.Append(fieldCodeCache.GetArgument(i).Text);
            }

            foreach (FieldSwitch fieldSwitch in fieldCodeCache.Switches)
            {
                sb.Append(' ');
                sb.Append(fieldSwitch.Name);
                sb.Append(' ');
                sb.Append(fieldSwitch.Argument.Text);
            }

            return sb.ToString();
        }

        private static Node GetFakeResultNode(FieldType fieldType, Document document, BarcodeParameters parameters)
        {
            RunPr errorRunPr = new RunPr();
            errorRunPr.Bold = AttrBoolEx.True;

            IBarcodeGenerator generator = document.FieldOptions.BarcodeGenerator;
            if (generator == null)
                return new Run(document, NoGeneratorErrorMessage, errorRunPr);

            Stream image = GenerateBarcodeImage(generator, fieldType, parameters);
            if (image == null)
                return new Run(document, ImageGenerationErrorMessage, errorRunPr);

            // Check whether the barcode data provided by IBarcodeGenerator is in supported image format.
            FileFormat imageType = ImageUtil.GetImageType(image);
            if (!ImageUtil.IsImage(imageType))
                return new Run(document, ImageFormatErrorMessage, errorRunPr);

            Shape shape = new Shape(document, ShapeType.Image);
            shape.ImageData.SetImage(image);

            // WORDSNET-14821 There are two stages of fields update. The common one, after which ShapeValidator is called,
            // and field's update through layout after which ShapeValidator is not called and zero shape size is not fixed.
            // So we have to validate zero shape size here.
            shape.MakeSizeValid();

            // WORDSNET-17480 Make sure generated image is rendered inline.
            shape.WrapType = WrapType.Inline;

            return shape;
        }

        private static Stream GenerateBarcodeImage(
            IBarcodeGenerator generator,
            FieldType fieldType,
            BarcodeParameters parameters)
        {
            switch (fieldType)
            {
                case FieldType.FieldBarcode:
                    return generator.GetOldBarcodeImage(parameters);
                case FieldType.FieldDisplayBarcode:
                    return generator.GetBarcodeImage(parameters);
                default:
                    Debug.Fail("Invalid field type");
                    return null;
            }
        }

        /// <summary>
        /// Core logic for updating BARCODE or DISPLAYBARCODE fields.
        /// </summary>
        internal static NodeRange GetFakeResult(FieldType fieldType, Document document, BarcodeParameters parameters)
        {
            Node result = GetFakeResultNode(fieldType, document, parameters);
            return new NodeRange(result, result);
        }

        internal static FieldSwitchType GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case DisplayTextSwitch:
                case AddStartStopCharSwitch:
                case FixCheckDigitSwitch:
                    return FieldSwitchType.Flag;
                case SymbolHeightSwitch:
                case SymbolRotationSwitch:
                case ScalingFactorSwitch:
                case ForegroundColorSwitch:
                case BackgroundColorSwitch:
                case PosCodeStyleSwitch:
                case CaseCodeStyleSwitch:
                case ErrorCorrectionLevelSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int BarcodeValueArgumentIndex = 0;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int BarcodeTypeArgumentIndex = 1;

        private const string NoGeneratorErrorMessage = "Error! Bar code generator is not set.";
        private const string ImageGenerationErrorMessage = "Error! Image was not generated.";
        private const string ImageFormatErrorMessage = "Error! Image format is not recognized. Supported image formats are Bmp, Emf, Gif, Jpeg, Png, Tiff, Wmf, Pict, Ico, WebP, Svg.";

        internal const string SymbolHeightSwitch = "\\h";
        internal const string SymbolRotationSwitch = "\\r";
        internal const string ScalingFactorSwitch = "\\s";
        internal const string DisplayTextSwitch = "\\t";
        internal const string ForegroundColorSwitch = "\\f";
        internal const string BackgroundColorSwitch = "\\b";
        internal const string PosCodeStyleSwitch = "\\p";
        internal const string CaseCodeStyleSwitch = "\\c";
        internal const string AddStartStopCharSwitch = "\\d";
        internal const string FixCheckDigitSwitch = "\\x";
        internal const string ErrorCorrectionLevelSwitch = "\\q";
    }
}
