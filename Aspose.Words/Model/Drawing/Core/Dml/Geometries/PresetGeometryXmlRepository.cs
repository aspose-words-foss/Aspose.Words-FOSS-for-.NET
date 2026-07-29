// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal class PresetGeometryXmlRepository : IPresetGeometryXmlRepository
    {
        [JavaConvertCheckedExceptions]
        public string GetPresetGeometryXml(string presetName)
        {
            return  PresetXmls.GetValueOrNull(presetName);
        }

        private static Dictionary<string, string> LoadPresetGeometriesXml()
        {
            const string resourceName = "Aspose.Words.Resources.PresetShapeDefinitions.xml";

            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    Dictionary<string, string> result = new Dictionary<string, string>(190); // There are about 188 standard preset shapes.

                    SkipHeader(sr);

                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        // We have read last line and finish
                        if (line.StartsWith("</presetShapeDefinitons>", StringComparison.Ordinal))
                            break;

                        string presetName = ReadPresetName(line);
                        result.Add(presetName, line);

                        line = sr.ReadLine();
                    }

                    return result;
                }
            }
        }

        private static string ReadPresetName(string line)
        {
            int tagStart = line.IndexOf("<", StringComparison.Ordinal);
            int tagEnd = line.IndexOf(">", StringComparison.Ordinal);
            return line.Substring(tagStart + 1, tagEnd - tagStart - 1);
        }

        private static void SkipHeader(StreamReader reader)
        {
            reader.ReadLine(); // Skip <?xml...
            reader.ReadLine(); // Skip <presetShapeDefinitons>
        }

        /// <summary>
        /// Provides access to the XML definitions of preset geometries.
        /// Initialized and loaded on first access.
        /// </summary>
        private static IDictionary<string, string> PresetXmls
        {
            get
            {
                // double-checked locking pattern.
                if (gXmlCache == null)
                {
                    lock (gXmlCacheSyncRoot)
                    {
                        if (gXmlCache == null)
                            gXmlCache = LoadPresetGeometriesXml();
                    }
                }

                return gXmlCache;
            }
        }

        private static readonly object gXmlCacheSyncRoot = new object();
        //JAVA: volatile modifier is added to static field with purpose to double-check pattern work in java.
        private static volatile Dictionary<string, string> gXmlCache;
    }
}
