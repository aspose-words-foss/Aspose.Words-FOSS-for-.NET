// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2008 by Roman Korchagin

using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// This is a library of standard shape type definitions.
    /// Allows to get shape type as an XML string or as a set of parsed shape properties.
    /// </summary>
    internal static class ShapeTypeLibrary
    {
        /// <summary>
        /// Returns a shape type definition as an XML string. Returns null if shape type is not found.
        /// </summary>
        internal static string GetShapeTypeXml(ShapeType shapeType)
        {
            return ShapeTypesXml[(int)shapeType];
        }

        /// <summary>
        /// Returns a shape type definition as an XML string. Returns null if shape type is not found.
        /// </summary>
        /// <param name="shapeTypeId">The string in format "#_x0000_t{0}</param>
        internal static string GetShapeTypeXml(string shapeTypeId)
        {
            // Search for the start index of typeId within input shapeTypeId string.
            int nonDigitIdx = shapeTypeId.Length - 1;
            while ((nonDigitIdx >= 0) && char.IsDigit(shapeTypeId[nonDigitIdx]))
                nonDigitIdx--;

            int typeIdIdx = nonDigitIdx + 1;
            if (typeIdIdx < shapeTypeId.Length)
            {
                int typeId = FormatterPal.ParseInt(shapeTypeId.Substring(typeIdIdx));
                return GetShapeTypeXml((ShapeType)typeId);
            }

            return null;
        }

        /// <summary>
        /// Returns a shape type definition as a shape properties collection. Returns null if shape type is not found.
        /// Note, you should not modify the returned collection of atrributes!
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal static ShapePr GetShapeTypePr(ShapeType shapeType)
        {
            ShapePr shapeTypePr;
            lock (gShapeTypesPrSyncRoot)
            {
                shapeTypePr = gShapeTypesPr[(int)shapeType];

                if (shapeTypePr == null)
                {
                    // We must use ContainsKey here instead of getting by key directly because we need
                    // to really know whether the shape type is already in the collection.
                    // If the shape type is in the collection, but null it means it is being read.
                    if (gShapeTypesPr.ContainsKey((int)shapeType))
                    {
                        shapeTypePr = gShapeTypesPr[(int)shapeType];
                    }
                    else
                    {
                        // This is to avoid endless recursion in VmlShapeReader. The code there might
                        // access some properties while reading a shape type and that causes it to try
                        // to resolve the attribute value and attempt to load the shape type again and so on.
                        gShapeTypesPr[(int)shapeType] = null;

                        string shapeTypeXml = GetShapeTypeXml(shapeType);
                        if (StringUtil.HasChars(shapeTypeXml))
                        {
                            IVmlShapeTypeReader vmlShapeTypeReader = WriterFactory.CreateVmlShapeTypeReader();
                            shapeTypePr = vmlShapeTypeReader.ReadShapeType(shapeTypeXml);
                            gShapeTypesPr[(int)shapeType] = shapeTypePr;
                        }
                    }
                }

                return shapeTypePr;
            }
        }

        /// <summary>
        /// Loads VML of standard shape types from an embedded resource into a dictionary.
        /// </summary>
        private static IntToObjDictionary<string> LoadShapeTypesXml()
        {
            const string resourceName = "Aspose.Words.Resources.ShapeTypes.txt";
            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    IntToObjDictionary<string> result = new IntToObjDictionary<string>(220);  // There are about 202 standard shape types.

                    int shapeTypeId = (int)ShapeType.MinValue;
                    string shapeTypeXml = sr.ReadLine();
                    while (shapeTypeXml != null)
                    {
                        result.Add(shapeTypeId, shapeTypeXml);
                        shapeTypeId++;
                        shapeTypeXml = sr.ReadLine();
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Provides access to the VML definitions of standard shape types.
        /// Initialized and loaded on first access.
        /// </summary>
        private static IntToObjDictionary<string> ShapeTypesXml
        {
            get
            {
                // double-checked locking pattern.
                if (gShapeTypesXmlCache == null)
                {
                    lock (gShateTypesXmlSyncRoot)
                    {
                        if (gShapeTypesXmlCache == null)
                            gShapeTypesXmlCache = LoadShapeTypesXml();
                    }
                }

                return gShapeTypesXmlCache;
            }
        }

        /// <summary>
        /// Do not access this variable directly, access it via the property only.
        ///
        /// JAVA: volatile modifier is added to static field with purpose to double-check pattern work in java.
        ///
        /// Key is <see cref="ShapeType"/>. Value is a string that contains XML of a shape type definition.
        /// </summary>
        private static volatile IntToObjDictionary<string> gShapeTypesXmlCache;
        private static readonly object gShateTypesXmlSyncRoot = new object();

        /// <summary>
        /// Key is <see cref="ShapeType"/>. Value is <see cref="ShapePr"/> that contains the shape type definition.
        /// </summary>
        private static readonly IntToObjDictionary<ShapePr> gShapeTypesPr = new IntToObjDictionary<ShapePr>();
        /// <summary>
        /// Added to allow autoporting to Java.
        /// </summary>
        private static readonly object gShapeTypesPrSyncRoot = new object();
    }
}
