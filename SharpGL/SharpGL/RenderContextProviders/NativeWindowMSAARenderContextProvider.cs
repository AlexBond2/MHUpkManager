using SharpGL.Version;
using System;
using System.Runtime.InteropServices;
 
namespace SharpGL.RenderContextProviders
{
    public class NativeWindowMSAARenderContextProvider : RenderContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeWindowMSAARenderContextProvider"/> class.
        /// </summary>
        public NativeWindowMSAARenderContextProvider()
        {
            //  We cannot layer GDI drawing on top of open gl drawing.
            GDIDrawingEnabled = false;
        }

        /// <summary>
        /// Creates the render context provider. Must also create the OpenGL extensions.
        /// </summary>
        /// <param name="openGLVersion">The desired OpenGL version.</param>
        /// <param name="gl">The OpenGL context.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bitDepth">The bit depth.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">A valid Window Handle must be provided for the NativeWindowMSAARenderContextProvider</exception>
        public override bool Create(OpenGLVersion openGLVersion, OpenGL gl, int width, int height, int bitDepth, object parameter)
        {
            base.Create(openGLVersion, gl, width, height, bitDepth, parameter);

            //  Cast the parameter to the device context.
            try { windowHandle = (IntPtr)parameter; }
            catch { throw new Exception("A valid Window Handle must be provided"); }

            deviceContextHandle = Win32.GetDC(windowHandle);

            int[] pixelFormatAttribs =
            [
                OpenGL.WGL_DRAW_TO_WINDOW_ARB, 1,
                OpenGL.WGL_SUPPORT_OPENGL_ARB, 1,
                OpenGL.WGL_DOUBLE_BUFFER_ARB, 1,
                OpenGL.WGL_PIXEL_TYPE_ARB, OpenGL.WGL_TYPE_RGBA_ARB,
                OpenGL.WGL_COLOR_BITS_ARB, bitDepth,
                OpenGL.WGL_DEPTH_BITS_ARB, 24,
                OpenGL.WGL_STENCIL_BITS_ARB, 8,
                OpenGL.WGL_SAMPLE_BUFFERS_ARB, 1,
                OpenGL.WGL_SAMPLES_ARB, 4, // 2/4/8
                0
            ];

            int[] formats = new int[1];
            bool result = gl.ChoosePixelFormatARB(bitDepth, pixelFormatAttribs, null, 1, formats, out uint numFormats);
            if (!result || numFormats == 0)
            {
                // No suitable pixel format found (ARB)
                Win32.ReleaseDC(windowHandle, deviceContextHandle);
                return false;
            }

            int chosenFormat = formats[0];

            Win32.PIXELFORMATDESCRIPTOR pfd = new();
            pfd.Init();
            pfd.nVersion = 1;
            pfd.dwFlags = Win32.PFD_DRAW_TO_WINDOW | Win32.PFD_SUPPORT_OPENGL | Win32.PFD_DOUBLEBUFFER;
            pfd.iPixelType = Win32.PFD_TYPE_RGBA;
            pfd.cColorBits = (byte)bitDepth;
            pfd.cDepthBits = 32;
            pfd.cStencilBits = 8;
            pfd.iLayerType = Win32.PFD_MAIN_PLANE;

            if (Win32.SetPixelFormat(deviceContextHandle, chosenFormat, pfd) == 0)
            {
                // SetPixelFormat on real DC failed
                Win32.ReleaseDC(windowHandle, deviceContextHandle);
                return false;
            }

            renderContextHandle = Win32.wglCreateContext(deviceContextHandle);
            if (renderContextHandle == IntPtr.Zero)
            {
                // wglCreateContext failed on real DC
                Win32.ReleaseDC(windowHandle, deviceContextHandle);
                return false;
            }

            MakeCurrent();

            UpdateContextVersion(gl);

            //  Return success.
            return true;
        }

        /// <summary>
        /// Destroys the render context provider instance.
        /// </summary>
	    public override void Destroy()
	    {
		    //	Release the device context.
		    Win32.ReleaseDC(windowHandle, deviceContextHandle);
            
		    //	Call the base, which will delete the render context handle.
            base.Destroy();
	    }

        /// <summary>
        /// Sets the dimensions of the render context provider.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
	    public override void SetDimensions(int width, int height)
	    {
            //  Call the base.
            base.SetDimensions(width, height);
	    }

        /// <summary>
        /// Blit the rendered data to the supplied device context.
        /// </summary>
        /// <param name="hdc">The HDC.</param>
	    public override void Blit(IntPtr hdc)
	    {
		    if(deviceContextHandle != IntPtr.Zero || windowHandle != IntPtr.Zero)
		    {
			    //	Swap the buffers.
                Win32.SwapBuffers(deviceContextHandle);
		    }
	    }

        /// <summary>
        /// Makes the render context current.
        /// </summary>
	    public override void MakeCurrent()
	    {
		    if(renderContextHandle != IntPtr.Zero)
			    Win32.wglMakeCurrent(deviceContextHandle, renderContextHandle);
	    }

        /// <summary>
        /// The window handle.
        /// </summary>
        protected IntPtr windowHandle = IntPtr.Zero;
    }
}
