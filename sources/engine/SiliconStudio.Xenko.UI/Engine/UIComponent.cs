﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System.ComponentModel;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Design;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.UI.Rendering.UI;

namespace SiliconStudio.Xenko.UI.Engine
{
    /// <summary>
    /// Add an <see cref="UIElement"/> to an <see cref="Entity"/>.
    /// </summary>
    [DataContract("UIComponent")]
    [Display("UI", Expand = ExpandRule.Once)]
    [DefaultEntityComponentRenderer(typeof(UIRenderProcessor))]
    [ComponentOrder(9800)]
    public sealed class UIComponent : ActivableEntityComponent
    {
        public UIComponent()
        {
            SnapText = true;
            IsBillboard = true;
            IsFullScreen = true;
            VirtualResolution = new Vector3(1280, 720, 1000);
            ActualSize = new Vector3(1, 1, 1);
            VirtualResolutionMode = VirtualResolutionMode.FixedWidthAdaptableHeight;
        }

        /// <summary>
        /// Gets or sets the root element of the UI hierarchy.
        /// </summary>
        /// <userdoc>The root element of the UI hierarchy.</userdoc>
        [DataMember(10)]
        [Display("Root Element")]
        [DataMemberIgnore] // TODO this is temporary as long as we don't have an UI editor and UI data asset.
        public UIElement RootElement { get; set; }
        
        /// <summary>
        /// Gets or sets the value indicating whether the UI should be full screen.
        /// </summary>
        /// <userdoc>Check this checkbox to display UI of this component on full screen. Uncheck it to display UI using standard camera.</userdoc>
        [DataMember(20)]
        [Display("Full Screen")]
        [DefaultValue(true)]
        public bool IsFullScreen { get; set; }

        /// <summary>
        /// Gets or sets the virtual resolution of the UI in virtual pixels.
        /// </summary>
        /// <userdoc>The value in pixels of the resolution of the UI</userdoc>
        [DataMember(30)]
        [Display("Virtual Resolution")]
        public Vector3 VirtualResolution { get; set; }

        /// <summary>
        /// Gets or sets the actual size of the UI component in world units. The actual size is only used when no in fullscreen.
        /// </summary>
        /// <userdoc>Gets or sets the actual size of the UI component in world units. The actual size is only used when no in fullscreen.</userdoc>
        [DataMember(35)]
        [Display("Actual Size")]
        public Vector3 ActualSize { get; set; }

        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>The camera.</value>
        /// <userdoc>Indicate how the virtual resolution value should be interpreted</userdoc>
        [DataMember(40)]
        [Display("Virtual Resolution Mode")]
        [DefaultValue(VirtualResolutionMode.FixedWidthAdaptableHeight)]
        public VirtualResolutionMode VirtualResolutionMode { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the UI should be displayed as billboard.
        /// </summary>
        /// <userdoc>If checked, the UI is displayed as a billboard. That is, it is automatically rotated parallel to the screen.</userdoc>
        [DataMember(50)]
        [Display("Billboard")]
        [DefaultValue(true)]
        public bool IsBillboard { get; set; }

        /// <summary>
        /// Gets or sets the value indicating of the UI texts should be snapped to closest pixel.
        /// </summary>
        /// <userdoc>If checked, all the text of the UI is snapped to the closest pixel (pixel perfect).</userdoc>
        [DataMember(60)]
        [Display("Snap Text")]
        [DefaultValue(true)]
        public bool SnapText { get; set; }
    }
}
