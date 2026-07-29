// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.10.29 ST_LightRigDirection (Light Rig Direction)
    /// Represents the direction from which the light rig is positioned relative to the scene. The light rig, itself, can be
    /// made up of multiple lights in any orientation around a given shape. This simple type defines the orientation of
    /// the light rig as a whole, and not the individual lights within the rig.
    /// </summary>
    internal enum DmlLightRigDirection
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }
}
