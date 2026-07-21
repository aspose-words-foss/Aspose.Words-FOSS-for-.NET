// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Designates a one-dimension area of table cells.
    /// </summary>
    internal class OneDimensionCellRange : ICellRange
    {
        private OneDimensionCellRange(CellPosition referencePosition, CellEnumerationDirection direction)
        {
            ReferencePosition = referencePosition;
            Direction = direction;
        }

        /// <summary>
        /// Attempts to parse the specified string to a <see cref="OneDimensionCellRange"/> object.
        /// Returns an erroneous constant if there was a problem during parse.
        /// </summary>
        internal static Constant TryParse(FieldContext context, string name, out ICellRange range)
        {
            range = null;

            CellEnumerationDirection direction;
            switch (name.ToUpper())
            {
                // Word reacts on these words regardless of the culture set.
                // English
                // German
                // Spanish
                case "ABOVE":
                case "ÜBER":
                case "ENCIMA":
                    direction = CellEnumerationDirection.Up;
                    break;
                case "LEFT":
                case "LINKS":
                case "IZQUIERDA":
                    direction = CellEnumerationDirection.Left;
                    break;
                case "BELOW":
                case "UNTER":
                case "DEBAJO":
                    direction = CellEnumerationDirection.Down;
                    break;
                case "RIGHT":
                case "RECHTS":
                case "DERECHA":
                    direction = CellEnumerationDirection.Right;
                    break;
                default:
                    // Failed to parse.
                    return null;
            }

            if (context.ParentCell == null)
                return new ErrorConstant("!The Formula Not In Table");

            CellPosition referencePosition = new CellPosition(context.ParentCell);

            switch (direction)
            {
                case CellEnumerationDirection.Up:
                    if (referencePosition.IsAtFirstRow)
                        return new ErrorConstant("!Table Index Cannot be Zero");
                    break;
                case CellEnumerationDirection.Down:
                    if (referencePosition.IsAtLastRow)
                        return new DoubleConstant(0d);
                    break;
                case CellEnumerationDirection.Left:
                    if (referencePosition.IsAtFirstColumn)
                        return new ErrorConstant("!Table Index Cannot be Zero");
                    break;
                case CellEnumerationDirection.Right:
                    if (referencePosition.IsAtLastColumn)
                        return new DoubleConstant(0d);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            range = new OneDimensionCellRange(referencePosition, direction);
            return null;
        }

        [JavaConvertCheckedExceptions]
        public IEnumerator<Cell> GetEnumerator()
        {
            return OneDimensionCellRangeEnumerator.CreateInstance(this);
        }

        [JavaConvertCheckedExceptions]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICellRange.IsRectangular
        {
            get { return false; }
        }

        bool ICellRange.IsOneCell
        {
            get { return false; }
        }

        CellPosition ICellRange.Start
        {
            get { return null; }
        }

        CellPosition ICellRange.End
        {
            get { return null; }
        }

        Constant ICellRange.EmptyCellValue
        {
            get
            {
                switch (Orientation)
                {
                    case CellEnumerationOrientation.Horizontal:
                        return NullConstant.Instance;
                    case CellEnumerationOrientation.Vertical:
                        return new DoubleConstant(0d);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        bool ICellRange.AlwaysEvaluateCellText
        {
            get { return false; }
        }

        internal CellPosition ReferencePosition { get; }

        internal CellEnumerationDirection Direction { get; }

        private CellEnumerationOrientation Orientation
        {
            get
            {
                switch (Direction)
                {
                    case CellEnumerationDirection.Up:
                    case CellEnumerationDirection.Down:
                        return CellEnumerationOrientation.Vertical;
                    case CellEnumerationDirection.Left:
                    case CellEnumerationDirection.Right:
                        return CellEnumerationOrientation.Horizontal;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
