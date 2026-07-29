// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/01/2007 by Vladimir Averkin
using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Words.Drawing.Core;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Reads the 'path' attribute of a VML shape and converts it into the model representation
    /// of path commands and path points.
    /// 
    /// VML description of path is here http://www.w3.org/TR/NOTE-VML#_Toc416858391
    /// </summary>
    internal class VmlPathReader
    {
        /// <summary>
        /// Ctor. Parses the specified VML path. The results are returned in the <see cref="Points"/> and
        /// <see cref="PathInfos"/> properties.
        /// </summary>
        internal VmlPathReader(string path)
        {
            ParsePath(path.ToLower());

            // Build final array of path infos.
            mPathInfos = new PathInfo[mCommands.Count];
            for (int i = 0; i < mCommands.Count; i++)
                mPathInfos[i] = mCommands[i];

            // Build final array of points.
            mPoints = new PathPoint[mCoords.Count / 2];
            for (int i = 0; i < mCoords.Count / 2; i++)
                mPoints[i] = new PathPoint(mCoords[2 * i], mCoords[2 * i + 1]);
        }

        /// <summary>
        /// Parses the path into commands and coordinates arrays.
        /// </summary>
        private void ParsePath(string path)
        {
            // VA State machine seems to be a best parsing method here.
            // RK Start in the command state because first char must be a command?
            PathParserState state = PathParserState.Command;

            for (int i = 0; i < path.Length; i++)
            {
                char c = path[i];
                if (Char.IsDigit(c) || (c == '-'))
                {
                    switch (state)
                    {
                        case PathParserState.Restart:
                            state = PathParserState.Number;
                            break;
                        case PathParserState.Command:
                            HandlePathCommand();
                            state = PathParserState.Number;
                            break;
                        case PathParserState.Ref:
                        case PathParserState.Number:
                            // Keep accumulating.
                            break;
                        default:
                            Debug.Fail("Unexpected VML path parser state.");
                            break;
                    }

                    mBuilder.Append(c);
                }
                else if (Char.IsLower(c))
                {
                    switch (state)
                    {
                        case PathParserState.Restart:
                            HandlePathValue();
                            break;
                        case PathParserState.Command:
                            if (IsPathCommand(mBuilder.ToString()))
                                HandlePathCommand();
                            break;
                        case PathParserState.Ref:
                        case PathParserState.Number:
                            HandlePathValue();
                            break;
                        default:
                            Debug.Fail("Unexpected VML path parser state.");
                            break;
                    }

                    state = PathParserState.Command;
                    mBuilder.Append(c);
                }
                else if (c == ',')
                {
                    switch (state)
                    {
                        case PathParserState.Command:
                            HandlePathCommand();
                            break;
                        case PathParserState.Ref:
                        case PathParserState.Number:
                            // Do nothing, code after switch is executed.
                            break;
                        case PathParserState.Restart:
                            // RK Do nothing?
                            break;
                        default:
                            Debug.Fail("Unexpected VML path parser state.");
                            break;
                    }

                    HandlePathValue();
                    state = PathParserState.Restart;
                }
                else if (c == '@')
                {
                    switch (state)
                    {
                        case PathParserState.Command:
                            HandlePathCommand();
                            break;
                        case PathParserState.Ref:
                        case PathParserState.Number:
                            HandlePathValue();
                            break;
                        case PathParserState.Restart:
                            // RK Do nothing?
                            break;
                        default:
                            Debug.Fail("Unexpected VML path parser state.");
                            break;
                    }

                    state = PathParserState.Ref;
                    mBuilder.Append(c);
                }

                // andrnosk: WORDSNET-5188 MSWord simply ignores white spaces, AW should do the same.
                // WORDSNET-13556 Line breaks are ignored as well.
                else if ((c == ' ') || (c == '\r') || (c == '\n'))
                {
                    // andrnosk: Do nothing.
                }
                else
                {
                    Debug.Fail("Unexpected VML path parser state.");
                }
            }

            HandlePathEnd(state);
        }

        /// <summary>
        /// Process the end statement of path whatever it is command or value.
        /// </summary>
        private void HandlePathEnd(PathParserState state)
        {
            // andrnosk: the path can end without 'e' in this case we should handle the last number as pathValue instead of pathCommand.
            switch (state)
            {
                case PathParserState.Ref:
                case PathParserState.Number:
                    HandlePathValue();
                    // andrnosk: WORDSNET-5188(part2) - There is a path without 'e' (end) command at the end. AW was unable to handle this.
                    // Made code resilient by explicitly adding 'e' at the end if path ends with value.
                    mBuilder.Append('e');
                    break;
                case PathParserState.Command:
                case PathParserState.Restart:
                    // Do nothing.
                    break;
                default:
                    Debug.Fail("Unexpected VML path parser state.");
                    break;
            }

            // RK Flush the last one. 
            HandlePathCommand();
        }

        /// <summary>
        /// RK Called when the parser reaches an end of a command.
        /// </summary>
        private void HandlePathCommand()
        {
            // VA Update the segment count of the previous command 
            // according to the accumulated number of coordinates.
            if (mCommands.Count > 0)
            {
                PathInfo prevPathInfo = mCommands[mCommands.Count - 1];
                mCommands[mCommands.Count - 1] = new PathInfo(
                    prevPathInfo.PathType,
                    GetSegmentCount(prevPathInfo, mCurCommandCoordCount / 2));
            }

            string value = mBuilder.ToString();
            mBuilder.Length = 0;

            // Create and add a new path info object. Segment count is not known at this stage yet.
            PathInfo pathInfo = new PathInfo(GetPathType(value), 0);
            mCommands.Add(pathInfo);

            mIsCurCommentRelative = IsPathCommandRelative(value);
            mCurCommandCoordCount = 0;
        }

        /// <summary>
        /// Gets the count of segments given a command type and number of points this command has in VML.
        /// </summary>
        private static int GetSegmentCount(PathInfo pathInfo, int pointCount)
        {
            switch (pathInfo.PathType)
            {
                case PathType.Close:
                {
                    // This command seems to be a special case. It has no points, but segment count must be 1.
                    return 1;
                }
                default:
                {
                    int pointsPerSegment = pathInfo.GetPointsPerSegment();
                    return (pointsPerSegment > 0) ? pointCount / pointsPerSegment : 0;
                }
            }
        }
        
        /// <summary>
        /// Returns true if the string is a recognized VML path command.
        /// </summary>
        private static bool IsPathCommand(string value)
        {
            return (GetPathType(value) != PathType.Unknown);
        }

        /// <summary>
        /// Converts a VML string path command into model enum.
        /// </summary>
        private static PathType GetPathType(string value)
        {
            switch (value)
            {
                case "r": // relative line to
                    // RK Both relative and absolute commands get converted to absolute.
                    return PathType.LineTo;
                case "v": // relative curve to
                    // RK Both relative and absolute commands get converted to absolute.
                    return PathType.CurveTo;
                case "t": // relative move to
                    // RK Both relative and absolute commands get converted to absolute.
                    return PathType.MoveTo;
                default:
                    return VmlEnum.VmlToPathType(value);
            }
        }

        /// <summary>
        /// Returns true if the VML path command is relative.
        /// </summary>
        private static bool IsPathCommandRelative(string value)
        {
            switch (value)
            {
                case "r":
                case "v":
                case "t":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// RK Called when the parser reaches an end of a value.
        /// </summary>
        private void HandlePathValue()
        {
            string value = mBuilder.ToString();
            mBuilder.Length = 0;

            PathValue pathValue = ParsePathValue(value);

            // RK If there is an odd number of coordinates in the array, we are processing an Y value.
            bool isYCoord = MathUtil.IsOdd(mCurCommandCoordCount);

            if (mIsCurCommentRelative)
            {
                // RK We have to translate from relative coordinates to absolute because
                // the model follows DOC and RTF and they don't have support for relative coordinates.
                // RK It seems that we don't need to update last point when processing a relative command.
                if (isYCoord)
                    pathValue = new PathValue(pathValue.Value + mLastY);
                else
                    pathValue = new PathValue(pathValue.Value + mLastX);
            }
            else
            {
                if (isYCoord)
                    mLastY = pathValue.Value;
                else
                    mLastX = pathValue.Value;
            }

            mCoords.Add(pathValue);
            mCurCommandCoordCount++;
        }
        
        /// <summary>
        /// Parses an integer value, optionally prefixed with the '@' character.
        /// </summary>
        internal static PathValue ParsePathValue(string value)
        {
            if (StringUtil.HasChars(value))
            {
                bool isFormula = false;
                if (value.StartsWith("@", StringComparison.Ordinal))
                {
                    isFormula = true;
                    value = value.TrimStart('@');
                }
                return new PathValue(FormatterPal.ParseInt(value), isFormula);
            }
            else
            {
                return new PathValue();
            }
        }

        /// <summary>
        /// Info how to connect points is available here after parsing.
        /// </summary>
        internal PathInfo[] PathInfos
        {
            get { return mPathInfos; }
        }

        /// <summary>
        /// Points are available here after parsing.
        /// </summary>
        internal PathPoint[] Points
        {
            get { return mPoints; }
        }

        private enum PathParserState
        {
            Restart,
            Command,
            Ref,
            Number
        }

        /// <summary>
        /// The builder used for parsing the path string.
        /// </summary>
        private readonly StringBuilder mBuilder = new StringBuilder();
        /// <summary>
        /// The commands extracted from the path. Items are <see cref="PathInfo"/> objects.
        /// </summary>
        private readonly List<PathInfo> mCommands = new List<PathInfo>();
        /// <summary>
        /// The coordinates extracted from the path. Items are <see cref="PathPoint"/> objects.
        /// </summary>
        private readonly List<PathValue> mCoords = new List<PathValue>();
        /// <summary>
        /// The final array of path infos in the model format. 
        /// </summary>
        private readonly PathInfo[] mPathInfos;
        /// <summary>
        /// The final array of points in the model format.
        /// </summary>
        private readonly PathPoint[] mPoints;
        /// <summary>
        /// Counts the number of coordinates (refs/numbers) in the current command.
        /// </summary>
        private int mCurCommandCoordCount;
        /// <summary>
        /// Signals if the last command read was relative.
        /// </summary>
        private bool mIsCurCommentRelative;
        /// <summary>
        /// Used in recalculating relative coordinates.
        /// </summary>
        private int mLastX;
        private int mLastY;
    }
}
