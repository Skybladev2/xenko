// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core.Collections;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering.Shadows;
using SiliconStudio.Xenko.Shaders;

namespace SiliconStudio.Xenko.Rendering.Lights
{
    public struct SpotLightData
    {
        public Vector3 PositionWS;
        private float padding0;
        public Vector3 DirectionWS;
        private float padding1;
        public Vector3 AngleOffsetAndInvSquareRadius;
        private float padding2;
        public Color3 Color;
        private float padding3;
    }

    public class LightSpotGroupRenderer : LightGroupRendererDynamic
    {
        public override void Initialize(RenderContext context)
        {
        }

        public override LightShaderGroupDynamic CreateLightShaderGroup(RenderDrawContext context, ILightShadowMapShaderGroupData shadowGroup)
        {
            return new SpotLightShaderGroup(context, shadowGroup);
        }

        class SpotLightShaderGroup : LightShaderGroupDynamic
        {
            private ValueParameterKey<int> countKey;
            private ValueParameterKey<SpotLightData> lightsKey;
            private FastListStruct<SpotLightData> lightsData = new FastListStruct<SpotLightData>(8);

            public SpotLightShaderGroup(RenderDrawContext context, ILightShadowMapShaderGroupData shadowGroupData)
                : base(context, shadowGroupData)
            {
            }

            public override void UpdateLayout(string compositionName)
            {
                base.UpdateLayout(compositionName);

                countKey = DirectLightGroupPerDrawKeys.LightCount.ComposeWith(compositionName);
                lightsKey = LightSpotGroupKeys.Lights.ComposeWith(compositionName);
            }

            protected override void UpdateLightCount()
            {
                base.UpdateLightCount();

                var mixin = new ShaderMixinSource();
                mixin.Mixins.Add(new ShaderClassSource("LightSpotGroup", LightCurrentCount));
                // Old fixed path kept in case we need it again later
                //mixin.Mixins.Add(new ShaderClassSource("LightSpotGroup", LightCurrentCount));
                //mixin.Mixins.Add(new ShaderClassSource("DirectLightGroupFixed", LightCurrentCount));
                ShadowGroup?.ApplyShader(mixin);

                ShaderSource = mixin;
            }

            public override void ApplyDrawParameters(RenderDrawContext context, int viewIndex, ParameterCollection parameters, ref BoundingBoxExt boundingBox)
            {
                CurrentLights.Clear();
                var lightRange = LightRanges[viewIndex];
                for (int i = lightRange.Start; i < lightRange.End; ++i)
                    CurrentLights.Add(Lights[i]);

                base.ApplyDrawParameters(context, viewIndex, parameters, ref boundingBox);

                // TODO: BoundingBox Light culling
                // TODO: Octree structure to select best lights quicker
                foreach (var lightEntry in CurrentLights)
                {
                    var light = lightEntry.Light;

                    var spotLight = (LightSpot)light.Type;
                    lightsData.Add(new SpotLightData
                    {
                        PositionWS = light.Position,
                        DirectionWS = light.Direction,
                        AngleOffsetAndInvSquareRadius = new Vector3(spotLight.LightAngleScale, spotLight.LightAngleOffset, spotLight.InvSquareRange),
                        Color = light.Color,
                    });
                }

                parameters.Set(countKey, lightsData.Count);
                parameters.Set(lightsKey, lightsData.Count, ref lightsData.Items[0]);
                lightsData.Clear();
            }
        }
    }
}