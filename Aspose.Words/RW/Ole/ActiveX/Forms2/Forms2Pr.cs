// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Forms2 attributes collection.
    /// </summary>
    internal class Forms2Pr : AttrCollection
    {
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        static Forms2Pr()
        {
            gDefaults = new Forms2Pr();

            gDefaults.Add(Forms2Attr.BooleanProperties, FormFlags.Enabled);
            gDefaults.Add(Forms2Attr.ClsidCacheIndex, 0x7FFF);
            gDefaults.Add(Forms2Attr.Cycle, Cycle.AllForms);
            gDefaults.Add(Forms2Attr.ObjectStreamSize, (uint)0);
            gDefaults.Add(Forms2Attr.MaxLength, 0);

            gDefaults.Add(Forms2Attr.ID, 0);
            gDefaults.Add(Forms2Attr.HelpContextID, 0);
            gDefaults.Add(Forms2Attr.TabIndex, -1);
            gDefaults.Add(Forms2Attr.GroupID, (uint)0x00);
            gDefaults.Add(Forms2Attr.NextAvailableID, (uint)0x00);
            gDefaults.Add(Forms2Attr.MultiRow, false);
            gDefaults.Add(Forms2Attr.BitFlagsSite,
                SiteFlag.TabStop |
                SiteFlag.Visible |
                SiteFlag.Streamed |
                SiteFlag.AutoSize);

            gDefaults.Add(Forms2Attr.ListIndex, -1);

            gDefaults.Add(Forms2Attr.TabOrienation, TabOrientation.Top);
            gDefaults.Add(Forms2Attr.TabStyle, TabStyle.Tabs);
            gDefaults.Add(Forms2Attr.TabFixedWidth, (uint)0);
            gDefaults.Add(Forms2Attr.TabsAllocated, (uint)0);
            gDefaults.Add(Forms2Attr.TabData, (uint)0);

            gDefaults.Add(Forms2Attr.PageCount, -1);
            gDefaults.Add(Forms2Attr.Flags, true);
            gDefaults.Add(Forms2Attr.TransitionEffect, TransitionEffect.None);
            gDefaults.Add(Forms2Attr.TransitionPeriod, (uint)0);
            gDefaults.Add(Forms2Attr.GridX, 0);
            gDefaults.Add(Forms2Attr.GridY, 0);
            gDefaults.Add(Forms2Attr.ClickControlMode, ClickControlMode.InsertionPoint);
            gDefaults.Add(Forms2Attr.DblClickControlMode, DblClickControlMode.SelectText);
            gDefaults.Add(Forms2Attr.BitFlagsDX,
                DesignExtenderFlag.InheritDesign |
                DesignExtenderFlag.InheritShowToolbox |
                DesignExtenderFlag.InheritShowGrid |
                DesignExtenderFlag.InheritSnapToGrid |
                DesignExtenderFlag.InheritGridX |
                DesignExtenderFlag.InheritGridY |
                DesignExtenderFlag.InheritClickControl |
                DesignExtenderFlag.InheritDblClickControl |
                DesignExtenderFlag.InheritShowInvisible |
                DesignExtenderFlag.InheritShowTooltips |
                DesignExtenderFlag.InheritLayoutImmediate);

            gDefaults.Add(Forms2Attr.Min, 0);
            gDefaults.Add(Forms2Attr.Max, 100);
            gDefaults.Add(Forms2Attr.Position, 0);

            gDefaults.Add(Forms2Attr.PrevEnabled, 0x01);
            gDefaults.Add(Forms2Attr.NextEnabled, 0x01);
            gDefaults.Add(Forms2Attr.SmallChange, 0x01);
            gDefaults.Add(Forms2Attr.LargeChange, 0x01);
            gDefaults.Add(Forms2Attr.ProportionalThumb, ProportionalThumb.Proportional);

            gDefaults.Add(Forms2Attr.Orientation, FormOrientation.Auto);
            gDefaults.Add(Forms2Attr.MatchEntry, MatchEntry.None);
            gDefaults.Add(Forms2Attr.ShowDropButtonWhen, ShowDropButtonWhen.Never);
            gDefaults.Add(Forms2Attr.DropButtonStyle, DropButtonStyle.Arrow);
            gDefaults.Add(Forms2Attr.ScrollBars, ScrollBars.None);

            gDefaults.Add(Forms2Attr.ListWidth, (uint)0x00);
            gDefaults.Add(Forms2Attr.ListStyle, ListStyle.Plain);
            gDefaults.Add(Forms2Attr.MultiSelect, MultiSelect.Single);
            gDefaults.Add(Forms2Attr.BoundColumn, (uint)0x01);
            gDefaults.Add(Forms2Attr.TextColumn, (uint)0xFFFF);
            gDefaults.Add(Forms2Attr.ColumnCount, (uint)0x01);
            gDefaults.Add(Forms2Attr.ListRows, (uint)0x08);
            gDefaults.Add(Forms2Attr.ColumnInfo, (uint)0x00);

            gDefaults.Add(Forms2Attr.PasswordChar, (uint)0x00);

            gDefaults.Add(Forms2Attr.Delay, 0x00000032);

            gDefaults.Add(Forms2Attr.MousePointer, MousePointer.Default);
            gDefaults.Add(Forms2Attr.MouseIcon, null);

            gDefaults.Add(Forms2Attr.Caption, "");
            gDefaults.Add(Forms2Attr.Value, "");
            gDefaults.Add(Forms2Attr.GroupName, "");
            gDefaults.Add(Forms2Attr.Name, "");
            gDefaults.Add(Forms2Attr.Tag, "");
            gDefaults.Add(Forms2Attr.Tooltips, "");
            gDefaults.Add(Forms2Attr.RuntimeLicKey, "");
            gDefaults.Add(Forms2Attr.ControlSource, "");
            gDefaults.Add(Forms2Attr.RowSource, "");

            gDefaults.Add(Forms2Attr.Font, null);

            gDefaults.Add(Forms2Attr.Picture, null);
            gDefaults.Add(Forms2Attr.PictureSizeMode, PictureSizeMode.Clip);
            gDefaults.Add(Forms2Attr.PictureAlignment, PictureAlignment.Center);
            gDefaults.Add(Forms2Attr.PicturePosition, PicturePosition.AboveCenter);
            gDefaults.Add(Forms2Attr.PictureTiling, false);
            gDefaults.Add(Forms2Attr.AutoSize, false);

            gDefaults.Add(Forms2Attr.Zoom, 100);
            gDefaults.Add(Forms2Attr.ShapeCookie, 0x00);
            gDefaults.Add(Forms2Attr.DrawBuffer, (uint)0x00);

            gDefaults.Add(Forms2Attr.DisplayStyle, 0x01);

            gDefaults.Add(Forms2Attr.LogicalSize, new OleSize(4000, 3000));
            gDefaults.Add(Forms2Attr.ScrollPosition, new OlePosition(0, 0));
            gDefaults.Add(Forms2Attr.SitePosition, new OlePosition(0, 0));

            gDefaults.Add(Forms2Attr.FontName, "MS Sans Serif");
            gDefaults.Add(Forms2Attr.FontHeight, (uint)160);
            gDefaults.Add(Forms2Attr.FontWeight, 400);
            gDefaults.Add(Forms2Attr.FontCharSet, 0x01);
            gDefaults.Add(Forms2Attr.FontEffects, FontEffects.None);
            gDefaults.Add(Forms2Attr.FontPitchAndFamily, 0x00);
            gDefaults.Add(Forms2Attr.ParagraphAlign, ParagraphAlign.Left);

            gDefaults.Add(Forms2Attr.TakeFocusOnClick, true);

            gDefaults.Add(Forms2Attr.Accelerator, (char)0);

            // The following defaults are different for various controls, so they can be overridden in concrete control.
            gDefaults.Add(Forms2Attr.BackgroundColor, OleColor.FromRaw(0x8000000F));
            gDefaults.Add(Forms2Attr.ForegroundColor, OleColor.FromRaw(0x80000012));
            gDefaults.Add(Forms2Attr.VariousPropertyBits,
                VariousPropertiesBits.Reserved1 |
                VariousPropertiesBits.Enabled |
                VariousPropertiesBits.BackStyle |
                VariousPropertiesBits.Reserved2);
            gDefaults.Add(Forms2Attr.BorderColor, OleColor.FromRaw(0x80000006));
            gDefaults.Add(Forms2Attr.BorderStyle, BorderStyle.None);
            gDefaults.Add(Forms2Attr.SpecialEffect, SpecialEffect.Flat);
            gDefaults.Add(Forms2Attr.Size, new OleSize(0, 0));
        }

        private static readonly Forms2Pr gDefaults;
    }
}
