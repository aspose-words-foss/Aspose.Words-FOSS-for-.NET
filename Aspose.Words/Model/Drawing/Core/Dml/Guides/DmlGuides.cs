// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Readers;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlGuides : IDmlGuideValueProvider
    {
        public DmlGuides Clone()
        {
            DmlGuides lhs = new DmlGuides();
            lhs.mGuides = new List<DmlGuide>();
            foreach (DmlGuide guid in mGuides)
                lhs.mGuides.Add(guid.Clone());

            lhs.mAdjustableValues = new List<DmlGuide>();
            foreach (DmlGuide guid in mAdjustableValues)
                lhs.mAdjustableValues.Add(guid.Clone());

            // Let's clone calculated values for more reliability.
            lhs.mGuideNameToValue = new ObjToDoubleDictionary<string>();
            ObjToDoubleDictionary<string>.Enumerator enumerator = mGuideNameToValue.GetEnumerator();
            while (enumerator.MoveNext())
                lhs.mGuideNameToValue.Add(enumerator.CurrentKey, enumerator.CurrentValue);
            if (mCommonGuides != null)
            {
                lhs.mCommonGuides = new List<DmlGuide>();
                foreach (DmlGuide guid in mCommonGuides)
                    lhs.mCommonGuides.Add(guid.Clone());
            }

            return lhs;
        }

        /// <summary>
        /// Gets the value of a guide by its name. If value isn't found then an exception will be thrown.
        /// </summary>
        /// <param name="guideName">Name of the guide.</param>
        public double GetValue(string guideName)
        {
            double result = mGuideNameToValue[guideName];
            if (ObjToDoubleDictionary<string>.IsNullSubstitute(result))
                throw new ArgumentOutOfRangeException(guideName, string.Format("Guide with name '{0}' not found.", guideName));

            return result;
        }

        /// <summary>
        /// Adds the adjustable value to the end of adjustable value list.
        /// </summary>
        internal void AddAdjustableValue(string name, string formula, bool isPreset)
        {
            DmlGuide guide = GuideFactory.CreateGuide(formula, name, isPreset);
            mAdjustableValues.Add(guide);
        }

        /// <summary>
        /// Adds the adjustable value to the end of adjustable value list.
        /// </summary>
        internal void AddAdjustableValue(string name, double value)
        {
            AddAdjustableValue(name, string.Format("val {0}", FormatterPal.DoubleToStr(value)), false);
        }

        /// <summary>
        /// Creates custom adjustable values based on changed preset values.
        /// </summary>
        internal void CreateCustomAdjustableValues()
        {
            int count = AdjustableValues.Count;
            for (int i = 0; i < count; i++)
                AddAdjustableValue(AdjustableValues[i].Name, AdjustableValues[i].Formula.Source, false);
        }

        /// <summary>
        /// Returns true if it contains custom adjustable values.
        /// </summary>
        internal bool HasCustomAdjustableValues()
        {
            foreach (DmlGuide guide in AdjustableValues)
            {
                if (!guide.IsPreset)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if it contains edited preset values.
        /// </summary>
        /// <param name="presetName">Name of the guide.</param>
        internal bool IsEditedPresetAdjustableValues(string presetName)
        {
            DmlGeometry presetGeom = DmlGeometryReader.GetPresetGeometry(presetName);
            List<int> prstAdjustValues = GetAdjustableValues(presetGeom.Guides.AdjustableValues);
            List<int> currAdjustValues = GetAdjustableValues(AdjustableValues);

            // Chart have no adjustable values, we need to skip them.
            if (presetGeom.Guides.AdjustableValues.Count == 0 ||
                currAdjustValues.Count == 0 ||
                ArrayUtil.IsArrayEqual(prstAdjustValues.ToArray(), currAdjustValues.ToArray()))
                return false;

            return true;
        }

        /// <summary>
        /// Gets a list of values from the current adjustable values collection.
        /// </summary>
        /// <param name="adjustValues">Adjustable values collection.</param>
        private static List<int> GetAdjustableValues(IList<DmlGuide> adjustValues)
        {
            List<int> values = new List<int>();
            foreach (DmlGuide guide in adjustValues)
            {
                int value = FormatterPal.TryParseInt(guide.Formula.Source.Split(' ')[1]);
                values.Add(value);
            }

            return values;
        }

        /// <summary>
        /// Adds the guide to the end of guides list.
        /// </summary>
        internal void AddGuide(string name, string formula, bool isPreset)
        {
            DmlGuide guide = GuideFactory.CreateGuide(formula, name, isPreset);
            mGuides.Add(guide);
        }

        /// <summary>
        /// Calculates all guides.
        /// </summary>
        /// <param name="width">The width of shape.</param>
        /// <param name="height">The height of shape.</param>
        internal void Calculate(double width, double height)
        {
            mGuideNameToValue.Clear();
            mGuideNameToValue["w"] = width;
            mGuideNameToValue["h"] = height;
            // Calculate guides in correct sequence
            mCommonGuides = GuideFactory.CreateCommonGuides();
            CalculateGuides(mCommonGuides);
            CalculateGuides(mAdjustableValues);
            CalculateGuides(mGuides);
        }


        private void CalculateGuides(IList<DmlGuide> guides)
        {
            foreach (DmlGuide guide in guides)
            {
                // During calculation values of previously calculated guides will be used
                // Old existing values of guides with same name should be replaced by
                // ne ones. This is used in preset shapes. Preset shapes can have several guides
                // with the same name but only latest should be used.
                mGuideNameToValue[guide.Name] = guide.Formula.Calculate(this);
            }
        }

        /// <summary>
        /// Gets adjustable values.
        /// </summary>
        /// <remarks>
        /// 20.1.9.5 avLst (List of Shape Adjust Values)
        /// This element specifies
        /// the adjust values that are applied to the specified shape.
        /// An adjust value is simply a guide that has a value based formula
        /// specified. That is, no calculation takes place for an adjust value guide.
        /// Instead, this guide specifies a parameter value that is used for calculations within the shape guides.
        /// </remarks>
        internal IList<DmlGuide> AdjustableValues
        {
            get { return mAdjustableValues; }
        }

        /// <summary>
        /// Gets guides
        /// </summary>
        /// <remarks>
        /// 20.1.9.12 gdLst (List of Shape Guides)
        /// This element specifies all the guides that are used for this shape.
        /// A guide is specified by the gd element and defines a calculated value
        /// that can be used for the construction of the corresponding shape.
        /// </remarks>
        internal IList<DmlGuide> Guides
        {
            get { return mGuides; }
        }

        internal IDmlGuideFactory GuideFactory
        {
            get { return mGuideFactory; }
            set { mGuideFactory = value; }
        }

        private List<DmlGuide> mAdjustableValues = new List<DmlGuide>();
        private IList<DmlGuide> mCommonGuides;
        private IDmlGuideFactory mGuideFactory = new DmlGuideFactory();
        private ObjToDoubleDictionary<string> mGuideNameToValue = new ObjToDoubleDictionary<string>();
        private List<DmlGuide> mGuides = new List<DmlGuide>();
    }
}
