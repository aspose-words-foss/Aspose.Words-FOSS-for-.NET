// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Guides;

namespace Aspose.Words.Tests.Dml
{
    internal class DmlGuideValueProviderStub : IDmlGuideValueProvider
    {
        public double GetValue(string guideName)
        {
            double result;
            if (!mNameToValue.TryGetValue(guideName, out result))
                throw new ArgumentOutOfRangeException("guideName");
            return result;
        }

        public DmlGuideValueProviderStub Add(string guideName, double value)
        {
            mNameToValue[guideName] = value;
            return this;
        }

        private readonly Dictionary<string, double> mNameToValue = new Dictionary<string, double>();
    }
}