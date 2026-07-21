// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2017 by Alexey Butalov

using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;

namespace Aspose.Words
{
    /// <summary>
    /// This interface is used to break the direct dependency Aspose.Words.Model from RW.Docx.
    /// We use a stub implementation of IChartInserter in C++ branches until RW.Docx isn't ported to C++.
    /// </summary>
    internal interface IChartInserter
    {
        /// <summary>
        /// Chart insertion with options.
        /// </summary>
        [JavaThrows(true)]
        Shape InsertChart(
            ChartType chartType,
            ChartStyle chartStyle,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType,
            DocumentBuilder documentBuilder);

        /// <summary>
        /// Creates a chart space for the specified chart type.
        /// </summary>
        [JavaThrows(true)]
        DmlChartSpace CreateChartSpace(ChartType chartType, Document document);

        /// <summary>
        /// Reads chart of specific type from the Aspose.Words.Resources.Charts.ChartTypes.xml
        /// </summary>
        [JavaThrows(true)]
        void ReadPresetChart(DmlChartSpace chartSpace, ChartType chartType, int documentVersion);
    }
}
