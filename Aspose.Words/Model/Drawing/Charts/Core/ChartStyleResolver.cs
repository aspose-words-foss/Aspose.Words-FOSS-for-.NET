// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2025 by Alexander Zhiltsov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Contains maps related to chart styles.
    /// </summary>
    internal static class ChartStyleResolver
    {
        /// <summary>
        /// Gets a style ID for the specified chart type and chart style.
        /// </summary>
        internal static int GetStyleId(ChartType chartType, ChartStyle chartStyle)
        {
            int mapKey = GetKey(chartType, chartStyle);
            int styleId;
            if (!gChartTypeAndStyleToStyleIdMap.TryGetValue(mapKey, out styleId))
                styleId = gChartStyleToDefaultStyleIdMap[chartStyle];

            Debug.Assert(styleId > 0);
            return styleId;
        }

        /// <summary>
        /// Gets a <see cref="ChartStyle"/> item which the style with the specified ID represents.
        /// </summary>
        internal static ChartStyle GetChartStyle(int styleId)
        {
            ChartStyle chartStyle;

            if (gStyleIdToChartStyleMap.TryGetValue(styleId, out chartStyle))
                return chartStyle;
            else
                return ChartStyle.Normal;
        }

        /// <summary>
        /// Creates and fills a map of a chart type and chart style to a style ID.
        /// </summary>
        private static Dictionary<int,int> GetStyleIdMap()
        {
            // MS Word 2019 allows specifying the following styles in UI.
            // The commented lines relate to styles that are currently not supported in Aspose.Words.

            Dictionary<int,int> map = new Dictionary<int,int>();

            map.Add(GetKey(ChartType.Area, ChartStyle.Normal), 276); // Style 1
            //map.Add(GetKey(ChartType.Area, ChartStyle.), 277); // Style 2
            map.Add(GetKey(ChartType.Area, ChartStyle.Shaded), 278); // Style 3
            map.Add(GetKey(ChartType.Area, ChartStyle.Grey), 279); // Style 4
            map.Add(GetKey(ChartType.Area, ChartStyle.Muted), 280); // Style 5
            map.Add(GetKey(ChartType.Area, ChartStyle.Saturated), 281); // Style 6
            map.Add(GetKey(ChartType.Area, ChartStyle.ShadedPlot), 282); // Style 7
            map.Add(GetKey(ChartType.Area, ChartStyle.Black), 283); // Style 8
            //map.Add(GetKey(ChartType.Area, ChartStyle.), 284); // Style 9
            map.Add(GetKey(ChartType.Area, ChartStyle.Shadowed), 346); // Style 10

            map.Add(GetKey(ChartType.Area3D, ChartStyle.Normal), 276); // Style 1
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Transparent2), 312); // Style 2
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Shaded), 313); // Style 3
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Grey), 314); // Style 4
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Muted), 280); // Style 5
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Saturated), 281); // Style 6
            map.Add(GetKey(ChartType.Area3D, ChartStyle.ShadedPlot), 282); // Style 7
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Black), 315); // Style 8
            map.Add(GetKey(ChartType.Area3D, ChartStyle.Shadowed), 350); // Style 9

            map.Add(GetKey(ChartType.Bar, ChartStyle.Normal), 216); // Style 1
            map.Add(GetKey(ChartType.Bar, ChartStyle.Shaded), 217); // Style 2
            map.Add(GetKey(ChartType.Bar, ChartStyle.Grey), 218); // Style 3
            map.Add(GetKey(ChartType.Bar, ChartStyle.Muted), 219); // Style 4
            map.Add(GetKey(ChartType.Bar, ChartStyle.Saturated), 220); // Style 5
            map.Add(GetKey(ChartType.Bar, ChartStyle.ShadedPlot), 221); // Style 6
            map.Add(GetKey(ChartType.Bar, ChartStyle.Black), 222); // Style 7
            map.Add(GetKey(ChartType.Bar, ChartStyle.Gradient), 223); // Style 8
            //map.Add(GetKey(ChartType.Bar, ChartStyle.), 224); // Style 9
            map.Add(GetKey(ChartType.Bar, ChartStyle.Flat), 225); // Style 10
            map.Add(GetKey(ChartType.Bar, ChartStyle.OutlineBlack), 339); // Style 11
            map.Add(GetKey(ChartType.Bar, ChartStyle.Shadowed), 341); // Style 12

            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Normal), 297); // Style 1
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Original), 298); // Style 2
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Shaded), 299); // Style 3
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Grey), 300); // Style 4
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Muted), 301); // Style 5
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Saturated), 302); // Style 6
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.ShadedPlot), 303); // Style 7
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Black), 304); // Style 8
            //map.Add(GetKey(ChartType.BarStacked, ChartStyle.), 305); // Style 9
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Flat), 306); // Style 10
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Shadowed), 348); // Style 11

            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Normal), 286); // Style 1
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Shaded), 287); // Style 2
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Grey), 288); // Style 3
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Muted), 289); // Style 4
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Saturated), 290); // Style 5
            //map.Add(GetKey(ChartType.Bar3D, ChartStyle.), 291); // Style 6
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Transparent2), 292); // Style 7
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Gradient), 349); // Style 8
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Black), 294); // Style 9
            map.Add(GetKey(ChartType.Bar3D, ChartStyle.Flat), 296); // Style 10
            //map.Add(GetKey(ChartType.Bar3D, ChartStyle.), 347); // Style 11

            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Normal), 286); // Style 1
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Shaded), 299); // Style 2
            //map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.), 310); // Style 3
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Muted), 289); // Style 4
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Saturated), 290); // Style 5
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Black), 294); // Style 6
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Flat), 296); // Style 7
            //map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.), 347); // Style 8

            map.Add(GetKey(ChartType.Bubble, ChartStyle.Normal), 269); // Style 1
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Shaded), 270); // Style 2
            map.Add(GetKey(ChartType.Bubble, ChartStyle.OutlineBlack), 271); // Style 3
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Grey), 272); // Style 4
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Muted), 246); // Style 5
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Saturated), 242); // Style 6
            map.Add(GetKey(ChartType.Bubble, ChartStyle.ShadedPlot), 273); // Style 7
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Black), 248); // Style 8
            //map.Add(GetKey(ChartType.Bubble, ChartStyle.), 274); // Style 9
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Blue), 275); // Style 10
            map.Add(GetKey(ChartType.Bubble, ChartStyle.Shadowed), 343); // Style 11

            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Normal), 269); // Style 1
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Shaded), 270); // Style 2
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Grey), 272); // Style 3
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Muted), 246); // Style 4
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Saturated), 242); // Style 5
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.ShadedPlot), 273); // Style 6
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Black), 248); // Style 7
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Blue), 275); // Style 8
            map.Add(GetKey(ChartType.Bubble3D, ChartStyle.Shadowed), 343); // Style 9

            map.Add(GetKey(ChartType.Column, ChartStyle.Normal), 201); // Style 1
            map.Add(GetKey(ChartType.Column, ChartStyle.Original), 202); // Style 2
            map.Add(GetKey(ChartType.Column, ChartStyle.Shaded), 203); // Style 3
            map.Add(GetKey(ChartType.Column, ChartStyle.Grey), 205); // Style 4
            map.Add(GetKey(ChartType.Column, ChartStyle.Muted), 206); // Style 5
            map.Add(GetKey(ChartType.Column, ChartStyle.Saturated), 207); // Style 6
            map.Add(GetKey(ChartType.Column, ChartStyle.ShadedPlot), 208); // Style 7
            map.Add(GetKey(ChartType.Column, ChartStyle.Black), 209); // Style 8
            map.Add(GetKey(ChartType.Column, ChartStyle.Gradient), 210); // Style 9
            map.Add(GetKey(ChartType.Column, ChartStyle.Outline), 211); // Style 10
            map.Add(GetKey(ChartType.Column, ChartStyle.Flat), 212); // Style 11
            map.Add(GetKey(ChartType.Column, ChartStyle.OutlineBlack), 213); // Style 12
            map.Add(GetKey(ChartType.Column, ChartStyle.Transparent1), 215); // Style 13
            map.Add(GetKey(ChartType.Column, ChartStyle.Shadowed), 340); // Style 14

            // Column3D has the same styles as Bar3D in MS Word except the style 8.
            map.Add(GetKey(ChartType.Column3D, ChartStyle.Gradient), 293); // Style 8

            map.Add(GetKey(ComboChart, ChartStyle.Normal), 201); // Style 1
            map.Add(GetKey(ComboChart, ChartStyle.Shaded), 323); // Style 2
            map.Add(GetKey(ComboChart, ChartStyle.Muted), 325); // Style 3
            map.Add(GetKey(ComboChart, ChartStyle.Saturated), 326); // Style 4
            map.Add(GetKey(ComboChart, ChartStyle.ShadedPlot), 221); // Style 5
            map.Add(GetKey(ComboChart, ChartStyle.Black), 328); // Style 6
            map.Add(GetKey(ComboChart, ChartStyle.Flat), 225); // Style 7
            map.Add(GetKey(ComboChart, ChartStyle.Shadowed), 352); // Style 8

            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Normal), 251); // Style 1
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Shaded), 252); // Style 2
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Grey), 253); // Style 3
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Muted), 254); // Style 4
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Saturated), 255); // Style 5
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.ShadedPlot), 256); // Style 6
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Black), 257); // Style 7
            //map.Add(GetKey(ChartType.Doughnut, ChartStyle.), 258); // Style 8
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Blue), 260); // Style 9
            //map.Add(GetKey(ChartType.Doughnut, ChartStyle.), 261); // Style 10

            map.Add(GetKey(ChartType.Stock, ChartStyle.Normal), 322); // Style 1
            map.Add(GetKey(ChartType.Stock, ChartStyle.Shaded), 323); // Style 2
            map.Add(GetKey(ChartType.Stock, ChartStyle.Grey), 324); // Style 3
            map.Add(GetKey(ChartType.Stock, ChartStyle.Muted), 325); // Style 4
            map.Add(GetKey(ChartType.Stock, ChartStyle.Saturated), 326); // Style 5
            map.Add(GetKey(ChartType.Stock, ChartStyle.ShadedPlot), 327); // Style 6
            map.Add(GetKey(ChartType.Stock, ChartStyle.Black), 328); // Style 7
            //map.Add(GetKey(ChartType.Stock, ChartStyle.), 329); // Style 8
            map.Add(GetKey(ChartType.Stock, ChartStyle.Blue), 330); // Style 9
            //map.Add(GetKey(ChartType.Stock, ChartStyle.), 331); // Style 10
            map.Add(GetKey(ChartType.Stock, ChartStyle.Shadowed), 352); // Style 11

            map.Add(GetKey(ChartType.Line, ChartStyle.Normal), 227); // Style 1
            map.Add(GetKey(ChartType.Line, ChartStyle.Grey), 228); // Style 2
            map.Add(GetKey(ChartType.Line, ChartStyle.Muted), 230); // Style 3
            map.Add(GetKey(ChartType.Line, ChartStyle.Saturated), 231); // Style 4
            map.Add(GetKey(ChartType.Line, ChartStyle.ShadedPlot), 232); // Style 5
            map.Add(GetKey(ChartType.Line, ChartStyle.Black), 233); // Style 6
            //map.Add(GetKey(ChartType.Line, ChartStyle.), 234); // Style 7
            map.Add(GetKey(ChartType.Line, ChartStyle.Flat), 235); // Style 8
            map.Add(GetKey(ChartType.Line, ChartStyle.OutlineBlack), 236); // Style 9
            map.Add(GetKey(ChartType.Line, ChartStyle.Shaded), 237); // Style 10
            map.Add(GetKey(ChartType.Line, ChartStyle.Original), 239); // Style 11
            //map.Add(GetKey(ChartType.Line, ChartStyle.), 332); // Style 12
            map.Add(GetKey(ChartType.Line, ChartStyle.Shadowed), 342); // Style 13

            map.Add(GetKey(ChartType.Line3D, ChartStyle.Normal), 307); // Style 1
            map.Add(GetKey(ChartType.Line3D, ChartStyle.Muted), 311); // Style 2
            map.Add(GetKey(ChartType.Line3D, ChartStyle.Saturated), 308); // Style 3
            map.Add(GetKey(ChartType.Line3D, ChartStyle.Black), 309); // Style 4

            map.Add(GetKey(ChartType.Pie, ChartStyle.Normal), 251); // Style 1
            map.Add(GetKey(ChartType.Pie, ChartStyle.Shaded), 252); // Style 2
            map.Add(GetKey(ChartType.Pie, ChartStyle.Grey), 253); // Style 3
            map.Add(GetKey(ChartType.Pie, ChartStyle.Muted), 254); // Style 4
            map.Add(GetKey(ChartType.Pie, ChartStyle.Saturated), 255); // Style 5
            map.Add(GetKey(ChartType.Pie, ChartStyle.ShadedPlot), 256); // Style 6
            map.Add(GetKey(ChartType.Pie, ChartStyle.Black), 257); // Style 7
            //map.Add(GetKey(ChartType.Pie, ChartStyle.), 258); // Style 8
            map.Add(GetKey(ChartType.Pie, ChartStyle.Original), 259); // Style 9
            map.Add(GetKey(ChartType.Pie, ChartStyle.Blue), 260); // Style 10
            //map.Add(GetKey(ChartType.Pie, ChartStyle.), 261); // Style 11
            map.Add(GetKey(ChartType.Pie, ChartStyle.Shadowed), 344); // Style 12

            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Normal), 333); // Style 1
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Shaded), 252); // Style 2
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Grey), 334); // Style 3
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Muted), 335); // Style 4
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Saturated), 336); // Style 5
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.ShadedPlot), 337); // Style 6
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Black), 338); // Style 7
            //map.Add(GetKey(ChartType.PieOfPie, ChartStyle.), 258); // Style 8
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Original), 259); // Style 9
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Blue), 260); // Style 10
            //map.Add(GetKey(ChartType.PieOfPie, ChartStyle.), 261); // Style 11
            map.Add(GetKey(ChartType.PieOfPie, ChartStyle.Shadowed), 344); // Style 12

            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Normal), 262); // Style 1
            //map.Add(GetKey(ChartType.Pie3D, ChartStyle.), 263); // Style 2
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Grey), 264); // Style 3
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Muted), 265); // Style 4
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Saturated), 266); // Style 5
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.ShadedPlot), 267); // Style 6
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Black), 268); // Style 7
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Original), 259); // Style 8
            //map.Add(GetKey(ChartType.Pie3D, ChartStyle.), 261); // Style 9
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Shadowed), 345); // Style 10

            map.Add(GetKey(ChartType.Radar, ChartStyle.Normal), 317); // Style 1
            map.Add(GetKey(ChartType.Radar, ChartStyle.Shaded), 318); // Style 2
            map.Add(GetKey(ChartType.Radar, ChartStyle.Muted), 206); // Style 3
            map.Add(GetKey(ChartType.Radar, ChartStyle.Transparent2), 319); // Style 4
            //map.Add(GetKey(ChartType.Radar, ChartStyle.), 320); // Style 5
            map.Add(GetKey(ChartType.Radar, ChartStyle.Saturated), 207); // Style 6
            map.Add(GetKey(ChartType.Radar, ChartStyle.OutlineBlack), 321); // Style 7
            map.Add(GetKey(ChartType.Radar, ChartStyle.Shadowed), 351); // Style 8

            map.Add(GetKey(ChartType.Scatter, ChartStyle.Normal), 240); // Style 1
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Original), 241); // Style 2
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Saturated), 242); // Style 3
            //map.Add(GetKey(ChartType.Scatter, ChartStyle.), 243); // Style 4
            //map.Add(GetKey(ChartType.Scatter, ChartStyle.), 244); // Style 5
            //map.Add(GetKey(ChartType.Scatter, ChartStyle.), 245); // Style 6
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Muted), 246); // Style 7
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Blue), 247); // Style 8
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Black), 248); // Style 9
            //map.Add(GetKey(ChartType.Scatter, ChartStyle.), 249); // Style 10
            map.Add(GetKey(ChartType.Scatter, ChartStyle.ShadedPlot), 250); // Style 11
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Shadowed), 343); // Style 12

            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Normal), 406); // Style 1
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Shaded), 407); // Style 2
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Grey), 408); // Style 3
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Black), 409); // Style 4
            //map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.), 373); // Style 5
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Flat), 374); // Style 6

            map.Add(GetKey(ChartType.Funnel, ChartStyle.Normal), 419); // Style 1
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Shaded), 423); // Style 2
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Grey), 424); // Style 3
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Muted), 425); // Style 4
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Saturated), 426); // Style 5
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Black), 427); // Style 6
            //map.Add(GetKey(ChartType.Funnel, ChartStyle.), 428); // Style 7
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Blue), 429); // Style 8
            //map.Add(GetKey(ChartType.Funnel, ChartStyle.), 430); // Style 9

            map.Add(GetKey(ChartType.Histogram, ChartStyle.Normal), 366); // Style 1
            map.Add(GetKey(ChartType.Histogram, ChartStyle.Shaded), 367); // Style 2
            map.Add(GetKey(ChartType.Histogram, ChartStyle.Grey), 368); // Style 3
            map.Add(GetKey(ChartType.Histogram, ChartStyle.Muted), 369); // Style 4
            map.Add(GetKey(ChartType.Histogram, ChartStyle.Black), 370); // Style 5
            map.Add(GetKey(ChartType.Histogram, ChartStyle.Blue), 371); // Style 6

            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Normal), 381); // Style 1
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Shaded), 382); // Style 2
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Grey), 383); // Style 3
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Muted), 384); // Style 4
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Saturated), 385); // Style 5
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.ShadedPlot), 386); // Style 6
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Black), 387); // Style 7
            //map.Add(GetKey(ChartType.Sunburst, ChartStyle.), 388); // Style 8

            map.Add(GetKey(ChartType.Treemap, ChartStyle.Normal), 410); // Style 1
            map.Add(GetKey(ChartType.Treemap, ChartStyle.Grey), 411); // Style 2
            map.Add(GetKey(ChartType.Treemap, ChartStyle.Muted), 412); // Style 3
            map.Add(GetKey(ChartType.Treemap, ChartStyle.Saturated), 413); // Style 4
            map.Add(GetKey(ChartType.Treemap, ChartStyle.ShadedPlot), 414); // Style 5
            map.Add(GetKey(ChartType.Treemap, ChartStyle.Black), 415); // Style 6
            map.Add(GetKey(ChartType.Treemap, ChartStyle.Original), 416); // Style 7
            //map.Add(GetKey(ChartType.Treemap, ChartStyle.), 417); // Style 8
            map.Add(GetKey(ChartType.Treemap, ChartStyle.Shaded), 418); // Style 9

            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Normal), 395); // Style 1
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Shaded), 396); // Style 2
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Grey), 397); // Style 3
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Muted), 398); // Style 4
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Saturated), 399); // Style 5
            //map.Add(GetKey(ChartType.Waterfall, ChartStyle.), 400); // Style 6
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Black), 372); // Style 7
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Flat), 389); // Style 8

            // Special definitions for styles that are not available in MS Word to replace the default style defined
            // in GetDefaultStyleIdMap.
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Saturated), 3851);
            map.Add(GetKey(ChartType.Line3D, ChartStyle.Shaded), 299);
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Shaded), 299);
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Shadowed), 343);
            map.Add(GetKey(ChartType.BarStacked, ChartStyle.Gradient), 223);
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Gradient), 349);
            map.Add(GetKey(ChartType.Funnel, ChartStyle.Gradient), 223);
            map.Add(GetKey(ChartType.Pie3D, ChartStyle.Gradient), 349);
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Original), 298); // BarStacked
            map.Add(GetKey(ChartType.Doughnut, ChartStyle.Original), 310); // Bar3DStacked
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Original), 416); // Treemap
            map.Add(GetKey(ChartType.Bar3DStacked, ChartStyle.Grey), 288); // Bar 3D
            map.Add(GetKey(ChartType.Line3D, ChartStyle.Grey), 288); // Bar 3D
            map.Add(GetKey(ChartType.Radar, ChartStyle.Grey), 288); // Bar 3D
            map.Add(GetKey(ChartType.Scatter, ChartStyle.Grey), 288); // Bar 3D
            map.Add(GetKey(ChartType.Surface3D, ChartStyle.Grey), 314); // Area 3D
            map.Add(GetKey(ChartType.Sunburst, ChartStyle.Blue), 260); // Doughnut
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.Blue), 247); // Scatter
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.Blue), 247); // Scatter
            map.Add(GetKey(ChartType.Surface, ChartStyle.Blue), 275); // Bubble
            map.Add(GetKey(ChartType.Surface3D, ChartStyle.Blue), 275); // Bubble
            map.Add(GetKey(ChartType.Histogram, ChartStyle.ShadedPlot), 386); // Sunburst
            map.Add(GetKey(ChartType.BoxAndWhisker, ChartStyle.ShadedPlot), 386); // Sunburst
            map.Add(GetKey(ChartType.Funnel, ChartStyle.ShadedPlot), 386); // Sunburst
            map.Add(GetKey(ChartType.Waterfall, ChartStyle.ShadedPlot), 386); // Sunburst

            return map;
        }

        /// <summary>
        /// Gets a combined chart type and chart style value to use as a key in maps.
        /// </summary>
        private static int GetKey(ChartType chartType, ChartStyle chartStyle)
        {
            // The following chart types have the same set of styles in MS Word.
            switch (chartType)
            {
                case ChartType.AreaStacked:
                case ChartType.AreaPercentStacked:
                    chartType = ChartType.Area;
                    break;
                case ChartType.Area3DStacked:
                case ChartType.Area3DPercentStacked:
                    chartType = ChartType.Area3D;
                    break;
                case ChartType.BarPercentStacked:
                    chartType = ChartType.BarStacked;
                    break;
                case ChartType.Bar3DPercentStacked:
                    chartType = ChartType.Bar3DStacked;
                    break;
                case ChartType.ColumnStacked:
                case ChartType.ColumnPercentStacked:
                    chartType = (chartStyle != ChartStyle.Gradient) ? ChartType.BarStacked : ChartType.ColumnStacked;
                    break;
                case ChartType.Column3D:
                case ChartType.Column3DClustered:
                    chartType = (chartStyle != ChartStyle.Gradient) ? ChartType.Bar3D : ChartType.Column3D;
                    break;
                case ChartType.Column3DStacked:
                case ChartType.Column3DPercentStacked:
                    chartType = (chartStyle != ChartStyle.Gradient) ? ChartType.Bar3DStacked : ChartType.Column3DStacked;
                    break;
                case ChartType.LineStacked:
                case ChartType.LinePercentStacked:
                    chartType = ChartType.Line;
                    break;
                case ChartType.PieOfBar:
                    chartType = ChartType.PieOfPie;
                    break;
                case ChartType.Pareto:
                    chartType = ChartType.Histogram;
                    break;
            }

            return ((int)chartType << 16) + (int)chartStyle;
        }

        /// <summary>
        /// Creates and fills a map of a chart style to its default style ID.
        /// </summary>
        private static Dictionary<ChartStyle,int> GetDefaultStyleIdMap()
        {
            // These style IDs are used when there is no direct mapping from chart type and chart style to style ID.
            Dictionary<ChartStyle,int> map = new Dictionary<ChartStyle,int>();
            map.Add(ChartStyle.Normal, 216); // Bar
            map.Add(ChartStyle.Muted, 206); // Column
            map.Add(ChartStyle.Saturated, 220); // Bar
            map.Add(ChartStyle.Shaded, 252); // Pie
            map.Add(ChartStyle.Flat, 212); // Column
            map.Add(ChartStyle.Shadowed, 346); // Area
            map.Add(ChartStyle.Gradient, 210); // Column
            map.Add(ChartStyle.Original, 239); // Line
            map.Add(ChartStyle.Transparent1, 215); // Column
            map.Add(ChartStyle.Transparent2, 312); // Area 3D
            map.Add(ChartStyle.Outline, 224); // Bar
            map.Add(ChartStyle.OutlineBlack, 213); // Column
            map.Add(ChartStyle.Black, 283); // Area
            map.Add(ChartStyle.Grey, 205); // Column
            map.Add(ChartStyle.Blue, 2601); // Pie with updated dataPoint3D
            map.Add(ChartStyle.ShadedPlot, 208); // Column
            return map;
        }

        /// <summary>
        /// Creates and fills a map of a style ID to a chart style.
        /// </summary>
        private static Dictionary<int,ChartStyle> GetChartStyleMap(
            Dictionary<int,int> styleIdMap, Dictionary<ChartStyle,int> chartStyleMap)
        {
            Dictionary<int,ChartStyle> map = new Dictionary<int,ChartStyle>();

            foreach (KeyValuePair<int,int> pair in styleIdMap)
            {
                if (map.ContainsKey(pair.Value))
                    continue;

                ChartStyle style = (ChartStyle)(pair.Key & 0xffff);
                map.Add(pair.Value, style);
            }

            foreach (KeyValuePair<ChartStyle,int> pair in chartStyleMap)
            {
                if (map.ContainsKey(pair.Value))
                    continue;

                map.Add(pair.Value, pair.Key);
            }

            return map;
        }

        static ChartStyleResolver()
        {
            gChartTypeAndStyleToStyleIdMap = GetStyleIdMap();
            gChartStyleToDefaultStyleIdMap = GetDefaultStyleIdMap();
            gStyleIdToChartStyleMap = GetChartStyleMap(gChartTypeAndStyleToStyleIdMap, gChartStyleToDefaultStyleIdMap);
        }

        private static readonly Dictionary<int,int> gChartTypeAndStyleToStyleIdMap;
        private static readonly Dictionary<ChartStyle,int> gChartStyleToDefaultStyleIdMap;
        private static readonly Dictionary<int,ChartStyle> gStyleIdToChartStyleMap;

        private const ChartType ComboChart = (ChartType)(-1);
    }
}
