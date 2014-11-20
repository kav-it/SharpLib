using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SharpLib.Native.Windows;
using SharpLib.WinForms;

namespace SharpLib.OpenGL
{
    public class GlControl : UserControl, ISupportInitialize
    {
        #region Поля

        private IContainer _components;

        private IntPtr _deviceContext;

        private int _errorCode;

        private IntPtr _renderingContext;

        private IntPtr _windowHandle;

        #endregion

        #region Свойства

        [Category("OpenGL")]
        [DefaultValue(0)]
        public int AccumBits { get; set; }

        [Category("OpenGL")]
        [DefaultValue(32)]
        public int ColorBits { get; set; }

        [Category("OpenGL")]
        [DefaultValue(16)]
        public int DepthBits { get; set; }

        [Category("OpenGL")]
        [DefaultValue(0)]
        public int StencilBits { get; set; }

        [Category("OpenGL")]
        [DefaultValue(false)]
        public bool AutoCheckErrors { get; set; }

        [Category("OpenGL")]
        [DefaultValue(false)]
        public bool AutoFinish { get; set; }

        [Category("OpenGL")]
        [DefaultValue(true)]
        public bool AutoMakeCurrent { get; set; }

        [Category("OpenGL")]
        [DefaultValue(true)]
        public bool AutoSwapBuffers { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                var csVredraw = 0x1;
                var csHredraw = 0x2;
                var csOwndc = 0x20;
                var cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | csVredraw | csHredraw | csOwndc;
                return cp;
            }
        }

        #endregion

        #region Конструктор

        public GlControl()
        {
            AutoMakeCurrent = true;
            AutoSwapBuffers = true;
            ColorBits = 32;
            DepthBits = 16;
            _deviceContext = IntPtr.Zero;
            _errorCode = Gl.GL_NO_ERROR;
            _renderingContext = IntPtr.Zero;
            _windowHandle = IntPtr.Zero;
            InitializeStyles();
            InitializeComponent();
            InitializeBackground();
        }

        #endregion

        #region Методы

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            DestroyContexts();
            base.Dispose(disposing);
        }

        private void InitializeBackground()
        {
            BackgroundImage = null;
        }

        private void InitializeComponent()
        {
            _components = new Container();
            BackColor = Color.Black;
            Size = new Size(50, 50);
        }

        private void InitializeStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, false);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        public void DestroyContexts()
        {
            if (_renderingContext != IntPtr.Zero)
            {
                Gl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Gl.wglDeleteContext(_renderingContext);
                _renderingContext = IntPtr.Zero;
            }

            if (_deviceContext != IntPtr.Zero)
            {
                if (_windowHandle != IntPtr.Zero)
                {
                    NativeMethods.ReleaseDC(_windowHandle, _deviceContext);
                }
                _deviceContext = IntPtr.Zero;
            }
        }

        public void Draw()
        {
            Invalidate();
        }

        private void InitializeContexts()
        {
            // Get window handle
            _windowHandle = Handle;

            if (_windowHandle == IntPtr.Zero)
            {
                // No window handle means something is wrong
                MessageBox.Show("Window creation error.  No window handle.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            NativeMethods.PIXELFORMATDESCRIPTOR pfd = new NativeMethods.PIXELFORMATDESCRIPTOR(); // The pixel format descriptor
            pfd.nSize = (short)Marshal.SizeOf(pfd); // Size of the pixel format descriptor
            pfd.nVersion = 1; // Version number (always 1)
            pfd.dwFlags = NativeMethods.PFD_DRAW_TO_WINDOW | // Format must support windowed mode
                          NativeMethods.PFD_SUPPORT_OPENGL | // Format must support OpenGL
                          NativeMethods.PFD_DOUBLEBUFFER; // Must support double buffering
            pfd.iPixelType = NativeMethods.PFD_TYPE_RGBA; // Request an RGBA format
            pfd.cColorBits = (byte)ColorBits; // Select our color depth
            pfd.cRedBits = 0; // Individual color bits ignored
            pfd.cRedShift = 0;
            pfd.cGreenBits = 0;
            pfd.cGreenShift = 0;
            pfd.cBlueBits = 0;
            pfd.cBlueShift = 0;
            pfd.cAlphaBits = 0; // No alpha buffer
            pfd.cAlphaShift = 0; // Alpha shift bit ignored
            pfd.cAccumBits = (byte)AccumBits; // Accumulation buffer
            pfd.cAccumRedBits = 0; // Individual accumulation bits ignored
            pfd.cAccumGreenBits = 0;
            pfd.cAccumBlueBits = 0;
            pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = (byte)DepthBits; // Z-buffer (depth buffer)
            pfd.cStencilBits = (byte)StencilBits; // No stencil buffer
            pfd.cAuxBuffers = 0; // No auxiliary buffer
            pfd.iLayerType = NativeMethods.PFD_MAIN_PLANE; // Main drawing layer
            pfd.bReserved = 0; // Reserved
            pfd.dwLayerMask = 0; // Layer masks ignored
            pfd.dwVisibleMask = 0;
            pfd.dwDamageMask = 0;

            _deviceContext = NativeMethods.GetDC(_windowHandle); // Attempt to get the device context
            if (_deviceContext == IntPtr.Zero)
            {
                // Did we not get a device context?
                MessageBox.Show("Can not create a GL device context.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            int pixelFormat = NativeMethods.ChoosePixelFormat(_deviceContext, ref pfd);
            if (pixelFormat == 0)
            {
                // Did windows not find a matching pixel format?
                MessageBox.Show("Can not find a suitable PixelFormat.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            NativeMethods.LoadLibrary("opengl32.dll");

            if (!NativeMethods.SetPixelFormat(_deviceContext, pixelFormat, ref pfd))
            {
                // Are we not able to set the pixel format?
                MessageBox.Show("Can not set the chosen PixelFormat.  Chosen PixelFormat was " + pixelFormat + ".", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            _renderingContext = Gl.wglCreateContext(_deviceContext); // Attempt to get the rendering context
            if (_renderingContext == IntPtr.Zero)
            {
                // Are we not able to get a rendering context?
                MessageBox.Show("Can not create a GL rendering context.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            // Attempt to activate the rendering context
            MakeCurrent();

            // Force A Reset On The Working Set Size
            NativeMethods.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        public void MakeCurrent()
        {
            if (!Gl.wglMakeCurrent(_deviceContext, _renderingContext))
            {
                MessageBox.Show("Can not activate the GL rendering context.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }
        }

        public void SwapBuffers()
        {
            NativeMethods.SwapBuffersFast(_deviceContext);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignHelper.IsDesigntime)
            {
                e.Graphics.Clear(BackColor);
                if (BackgroundImage != null)
                {
                    e.Graphics.DrawImage(BackgroundImage, ClientRectangle, 0, 0, BackgroundImage.Width, BackgroundImage.Height, GraphicsUnit.Pixel);
                }
                e.Graphics.Flush();
                return;
            }

            if (_deviceContext == IntPtr.Zero || _renderingContext == IntPtr.Zero)
            {
                MessageBox.Show("Не назначен контекст элементу " + Name);

                return;
            }

            if (AutoMakeCurrent)
            {
                MakeCurrent();
            }

            base.OnPaint(e);

            if (AutoFinish)
            {
                Gl.Finish();
            }

            if (AutoCheckErrors)
            {
                _errorCode = Gl.GetError();
                if (_errorCode != Gl.GL_NO_ERROR)
                {
                    switch (_errorCode)
                    {
                        case Gl.GL_INVALID_ENUM:
                            MessageBox.Show("GL_INVALID_ENUM - An unacceptable value has been specified for an enumerated argument.  The offending function has been ignored.", "OpenGL Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        case Gl.GL_INVALID_VALUE:
                            MessageBox.Show("GL_INVALID_VALUE - A numeric argument is out of range.  The offending function has been ignored.", "OpenGL Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            break;
                        case Gl.GL_INVALID_OPERATION:
                            MessageBox.Show("GL_INVALID_OPERATION - The specified operation is not allowed in the current state.  The offending function has been ignored.", "OpenGL Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        case Gl.GL_STACK_OVERFLOW:
                            MessageBox.Show("GL_STACK_OVERFLOW - This function would cause a stack overflow.  The offending function has been ignored.", "OpenGL Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            break;
                        case Gl.GL_STACK_UNDERFLOW:
                            MessageBox.Show("GL_STACK_UNDERFLOW - This function would cause a stack underflow.  The offending function has been ignored.", "OpenGL Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            break;
                        case Gl.GL_OUT_OF_MEMORY:
                            MessageBox.Show("GL_OUT_OF_MEMORY - There is not enough memory left to execute the function.  The state of OpenGL has been left undefined.", "OpenGL Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        default:
                            MessageBox.Show("Unknown GL error.  This should never happen.", "OpenGL Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                    }
                }
            }

            if (AutoSwapBuffers)
            {
                SwapBuffers();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            InitializeContexts();
        }

        #endregion
    }
}