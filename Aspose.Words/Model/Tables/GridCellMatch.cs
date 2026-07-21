// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2013 by Dmitry Matveenko

using System;

namespace Aspose.Words.Tables
{
    internal class GridCellMatch
    {
        internal GridCellMatch()
        {
            Match = GridCellMatchType.None;
        }

        private GridCellMatchType Match { get; set; }

        internal void Add(GridCellMatchType cellMatch)
        {
            Debug.Assert(cellMatch != GridCellMatchType.None, "No point in adding cell match None.");
            Match = Match | cellMatch;
        }

        internal bool IsSet(GridCellMatchType cellMatch)
        {
            Debug.Assert(cellMatch != GridCellMatchType.None, "No point in checking cell match None.");
            return (Match & cellMatch) == cellMatch;
        }

        /// <summary>
        /// No cell match types set.
        /// </summary>
        internal bool IsNone
        {
            get { return Match == GridCellMatchType.None; }
        }

        /// <summary>
        /// Only Auto cell match type is set.
        /// </summary>
        internal bool IsAuto
        {
            get { return Match == GridCellMatchType.Auto; }
        }

        internal void Update(PreferredWidth cellPreferred)
        {
            switch (cellPreferred.Type)
            {
                case PreferredWidthType.Points:
                    Add(GridCellMatchType.Twip);
                    // TODO ? add a cellMatch.Auto for twips == 0?
                    break;
                case PreferredWidthType.Percent:
                    Add(GridCellMatchType.Percent);
                    break;
                case PreferredWidthType.Auto:
                    Add(GridCellMatchType.Auto);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected cell width type.");
            }
        }
    }
}
