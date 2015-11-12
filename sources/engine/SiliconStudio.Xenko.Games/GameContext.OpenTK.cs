// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#if SILICONSTUDIO_PLATFORM_WINDOWS_DESKTOP && SILICONSTUDIO_XENKO_GRAPHICS_API_OPENGL && !SILICONSTUDIO_UI_SDL2
using System;
using OpenTK;
using OpenTK.Graphics;
using SiliconStudio.Xenko.Graphics.OpenGL;

namespace SiliconStudio.Xenko.Games
{
    /// <summary>
    /// A <see cref="GameContext"/> to use for rendering to an existing WinForm <see cref="Control"/>.
    /// </summary>
    public partial class GameContextOpenTk : GameContextWindows<OpenTK.GameWindow>
    {
        /// <inheritDoc/>
        public GameContextOpenTk(OpenTK.GameWindow control, int requestedWidth = 0, int requestedHeight = 0)
            : base(control, requestedWidth, requestedHeight)
        {
            ContextType = AppContextType.DesktopOpenTK;
            if (requestedWidth == 0 || requestedHeight == 0)
            {
                requestedWidth = 1280;
                requestedHeight = 720;
            }

            var creationFlags = GraphicsContextFlags.Default;
#if SILICONSTUDIO_XENKO_GRAPHICS_API_OPENGLES
            creationFlags |= GraphicsContextFlags.Embedded;
#endif
            // force the stencil buffer to be not null.
            var defaultMode = GraphicsMode.Default;
            var graphicMode = new GraphicsMode(defaultMode.ColorFormat, defaultMode.Depth, 8, defaultMode.Samples, defaultMode.AccumulatorFormat, defaultMode.Buffers, defaultMode.Stereo);
            
            GraphicsContext.ShareContexts = true;

            if (control == null)
            {
                int versionMajor, versionMinor;
                if (RequestedGraphicsProfile == null || RequestedGraphicsProfile.Length == 0)
                {
#if SILICONSTUDIO_XENKO_GRAPHICS_API_OPENGLES
                    versionMajor = 3;
                    versionMinor = 0;
#else
                    // PC: 4.3 is commonly available (= compute shaders)
                    // MacOS X: 4.1 maximum
                    versionMajor = 4;
                    versionMinor = 1;
#endif
                    Control = TryGameWindow(requestedWidth, requestedHeight, graphicMode, versionMajor, versionMinor, creationFlags);
                }
                else
                {
                    foreach (var profile in RequestedGraphicsProfile)
                    {
                        OpenGLUtils.GetGLVersion(profile, out versionMajor, out versionMinor);
                        var gameWindow = TryGameWindow(requestedWidth, requestedHeight, graphicMode, versionMajor, versionMinor, creationFlags);
                        if (gameWindow != null)
                        {
                            Control = gameWindow;
                            break;
                        }
                    }
                }
            }

            if (Control == null)
            {
                throw new Exception("Unable to initialize graphics context.");
            }
        }

        /// <summary>
        /// Try to create the graphics context.
        /// </summary>
        /// <param name="requestedWidth">The requested width.</param>
        /// <param name="requestedHeight">The requested height.</param>
        /// <param name="graphicMode">The graphics mode.</param>
        /// <param name="versionMajor">The major version of OpenGL.</param>
        /// <param name="versionMinor">The minor version of OpenGL.</param>
        /// <param name="creationFlags">The creation flags.</param>
        /// <returns>The created GameWindow.</returns>
        private static OpenTK.GameWindow TryGameWindow(int requestedWidth, int requestedHeight, GraphicsMode graphicMode, int versionMajor, int versionMinor, GraphicsContextFlags creationFlags)
        {
            try
            {
#if SILICONSTUDIO_XENKO_GRAPHICS_API_OPENGLES
                // Preload proper SDL native library (depending on CPU type)
                // This is for OpenGL ES on desktop
                Core.NativeLibrary.PreloadLibrary("SDL2.dll");
#endif

                var gameWindow = new OpenTK.GameWindow(requestedWidth, requestedHeight, graphicMode, "Xenko Game", GameWindowFlags.Default, DisplayDevice.Default, versionMajor, versionMinor,
                    creationFlags);
                return gameWindow;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
#endif
