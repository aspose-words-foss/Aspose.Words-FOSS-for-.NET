// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Dmitry Bormashov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal class PresetTextWarpXmlRepository : IPresetTextWarpXmlRepository
    {
        [JavaConvertCheckedExceptions]
        public string GetPresetTextWarpXml(string presetName)
        {
            return PresetXmls.GetValueOrNull(presetName);
        }

        private static Dictionary<string, string> LoadPresetTextWarpXml()
        {
            const string resourceName = "Aspose.Words.Resources.PresetTextWarpDefinitions.xml";

            using (StreamReader sr = new StreamReader(ResourceUtil.FetchResourceStream(resourceName)))
            {
                // There are about 188 standard preset shapes.
                Dictionary<string, string> result = new Dictionary<string, string>(190);

                SkipHeader(sr);

                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;

                    // We have read last line and finish
                    if (line.StartsWith("</presetTextWarpDefinitions>", StringComparison.Ordinal))
                        break;

                    string presetName = ReadPresetName(line);
                    result.Add(presetName, line);
                }

                return result;
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
        private static Dictionary<string, string> PresetXmls
        {
            get
            {
                // double-checked locking pattern.
                if (gXmlCache == null)
                {
                    lock (gXmlCacheSyncRoot)
                    {
                        if (gXmlCache == null)
                            gXmlCache = LoadPresetTextWarpXml();
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
