// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Konstantin Kornilov, Andrey Noskov

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests.Fonts
{
    [JavaDelete("Used only by TestFonts that launched only in .Net by design.")]
    public class ComparsionResult
    {
        public List<ComparsionDifference> Differences = new List<ComparsionDifference>();

        public void Serialize(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ComparsionResult));
            serializer.Serialize(stream, this);
        }
    }
}