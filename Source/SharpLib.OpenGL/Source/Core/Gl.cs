using System.Runtime.InteropServices;

namespace SharpLib.OpenGL
{
    using GLsizei = System.Int32;
    using GLsizeiptr = System.IntPtr;
    using GLintptr = System.IntPtr;
    using GLenum = System.Int32;
    using GLboolean = System.Int32;
    using GLbitfield = System.Int32;
    using GLvoid = System.Object;
    using GLchar = System.Char;
    using GLbyte = System.Byte;
    using GLubyte = System.Byte;
    using GLshort = System.Int16;
    using GLushort = System.Int16;
    using GLint = System.Int32;
    using GLuint = System.Int32;
    using GLfloat = System.Single;
    using GLclampf = System.Single;
    using GLdouble = System.Double;
    using GLclampd = System.Double;
    using GLstring = System.String;
    using GLsizeiptrARB = System.IntPtr;
    using GLintptrARB = System.IntPtr;
    using GLhandleARB = System.Int32;
    using GLhalfARB = System.Int16;
    using GLhalfNV = System.Int16;
    using GLcharARB = System.Char;
    using GLint64EXT = System.Int64;
    using GLuint64EXT = System.Int64;
    using GLint64 = System.Int64;
    using GLuint64 = System.Int64;

    public partial class Gl
    {
        #region Константы

        internal const string GL_NATIVE_LIBRARY = "opengl32.dll";

        #endregion

        #region Методы

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glAccum", ExactSpelling = true)]
        public static extern void Accum(GLenum op, GLfloat value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glActiveTexture", ExactSpelling = true)]
        public static extern void ActiveTexture(GLenum texture);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glAlphaFunc", ExactSpelling = true)]
        public static extern void AlphaFunc(GLenum func, GLclampf @ref);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glAreTexturesResident", ExactSpelling = true)]
        public static extern GLboolean AreTexturesResident(GLsizei n, System.IntPtr textures, System.IntPtr residences);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glArrayElement", ExactSpelling = true)]
        public static extern void ArrayElement(GLint i);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glAttachShader", ExactSpelling = true)]
        public static extern void AttachShader(GLuint program, GLuint shader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBegin", ExactSpelling = true)]
        public static extern void Begin(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBeginQuery", ExactSpelling = true)]
        public static extern void BeginQuery(GLenum target, GLuint id);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBindAttribLocation", ExactSpelling = true)]
        public static extern void BindAttribLocation(GLuint program, GLuint index, System.String name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBindBuffer", ExactSpelling = true)]
        public static extern void BindBuffer(GLenum target, GLuint buffer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBindTexture", ExactSpelling = true)]
        public static extern void BindTexture(GLenum target, GLuint texture);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBitmap", ExactSpelling = true)]
        public static extern void Bitmap(GLsizei width, GLsizei height, GLfloat xorig, GLfloat yorig, GLfloat xmove, GLfloat ymove, System.IntPtr bitmap);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBlendColor", ExactSpelling = true)]
        public static extern void BlendColor(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBlendEquation", ExactSpelling = true)]
        public static extern void BlendEquation(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBlendEquationSeparate", ExactSpelling = true)]
        public static extern void BlendEquationSeparate(GLenum modeRgb, GLenum modeAlpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBlendFunc", ExactSpelling = true)]
        public static extern void BlendFunc(GLenum sfactor, GLenum dfactor);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBlendFuncSeparate", ExactSpelling = true)]
        public static extern void BlendFuncSeparate(GLenum sfactorRgb, GLenum dfactorRgb, GLenum sfactorAlpha, GLenum dfactorAlpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBufferData", ExactSpelling = true)]
        public static extern void BufferData(GLenum target, GLsizeiptr size, System.IntPtr data, GLenum usage);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBufferSubData", ExactSpelling = true)]
        public static extern void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCallList", ExactSpelling = true)]
        public static extern void CallList(GLuint list);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCallLists", ExactSpelling = true)]
        public static extern void CallLists(GLsizei n, GLenum type, System.IntPtr lists);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClear", ExactSpelling = true)]
        public static extern void Clear(GLbitfield mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClearAccum", ExactSpelling = true)]
        public static extern void ClearAccum(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClearColor", ExactSpelling = true)]
        public static extern void ClearColor(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClearDepth", ExactSpelling = true)]
        public static extern void ClearDepth(GLclampd depth);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClearIndex", ExactSpelling = true)]
        public static extern void ClearIndex(GLfloat c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClearStencil", ExactSpelling = true)]
        public static extern void ClearStencil(GLint s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClientActiveTexture", ExactSpelling = true)]
        public static extern void ClientActiveTexture(GLenum texture);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClipPlane", ExactSpelling = true)]
        public static extern void ClipPlane(GLenum plane, System.IntPtr equation);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3b", ExactSpelling = true)]
        public static extern void Color3b(GLbyte red, GLbyte green, GLbyte blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3bv", ExactSpelling = true)]
        public static extern void Color3bv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3d", ExactSpelling = true)]
        public static extern void Color3d(GLdouble red, GLdouble green, GLdouble blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3dv", ExactSpelling = true)]
        public static extern void Color3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3f", ExactSpelling = true)]
        public static extern void Color3f(GLfloat red, GLfloat green, GLfloat blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3fv", ExactSpelling = true)]
        public static extern void Color3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3i", ExactSpelling = true)]
        public static extern void Color3i(GLint red, GLint green, GLint blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3iv", ExactSpelling = true)]
        public static extern void Color3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3s", ExactSpelling = true)]
        public static extern void Color3s(GLshort red, GLshort green, GLshort blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3sv", ExactSpelling = true)]
        public static extern void Color3sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3ub", ExactSpelling = true)]
        public static extern void Color3ub(GLubyte red, GLubyte green, GLubyte blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3ubv", ExactSpelling = true)]
        public static extern void Color3ubv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3ui", ExactSpelling = true)]
        public static extern void Color3ui(GLuint red, GLuint green, GLuint blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3uiv", ExactSpelling = true)]
        public static extern void Color3uiv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3us", ExactSpelling = true)]
        public static extern void Color3us(GLushort red, GLushort green, GLushort blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3usv", ExactSpelling = true)]
        public static extern void Color3usv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4b", ExactSpelling = true)]
        public static extern void Color4b(GLbyte red, GLbyte green, GLbyte blue, GLbyte alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4bv", ExactSpelling = true)]
        public static extern void Color4bv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4d", ExactSpelling = true)]
        public static extern void Color4d(GLdouble red, GLdouble green, GLdouble blue, GLdouble alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4dv", ExactSpelling = true)]
        public static extern void Color4dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4f", ExactSpelling = true)]
        public static extern void Color4f(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4fv", ExactSpelling = true)]
        public static extern void Color4fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4i", ExactSpelling = true)]
        public static extern void Color4i(GLint red, GLint green, GLint blue, GLint alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4iv", ExactSpelling = true)]
        public static extern void Color4iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4s", ExactSpelling = true)]
        public static extern void Color4s(GLshort red, GLshort green, GLshort blue, GLshort alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4sv", ExactSpelling = true)]
        public static extern void Color4sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4ub", ExactSpelling = true)]
        public static extern void Color4ub(GLubyte red, GLubyte green, GLubyte blue, GLubyte alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4ubv", ExactSpelling = true)]
        public static extern void Color4ubv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4ui", ExactSpelling = true)]
        public static extern void Color4ui(GLuint red, GLuint green, GLuint blue, GLuint alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4uiv", ExactSpelling = true)]
        public static extern void Color4uiv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4us", ExactSpelling = true)]
        public static extern void Color4us(GLushort red, GLushort green, GLushort blue, GLushort alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor4usv", ExactSpelling = true)]
        public static extern void Color4usv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorMask", ExactSpelling = true)]
        public static extern void ColorMask(GLboolean red, GLboolean green, GLboolean blue, GLboolean alpha);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorMaterial", ExactSpelling = true)]
        public static extern void ColorMaterial(GLenum face, GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorPointer", ExactSpelling = true)]
        public static extern void ColorPointer(GLint size, GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorSubTable", ExactSpelling = true)]
        public static extern void ColorSubTable(GLenum target, GLsizei start, GLsizei count, GLenum format, GLenum type, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorTable", ExactSpelling = true)]
        public static extern void ColorTable(GLenum target, GLenum internalformat, GLsizei width, GLenum format, GLenum type, System.IntPtr table);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorTableParameterfv", ExactSpelling = true)]
        public static extern void ColorTableParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColorTableParameteriv", ExactSpelling = true)]
        public static extern void ColorTableParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompileShader", ExactSpelling = true)]
        public static extern void CompileShader(GLuint shader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompressedTexImage1D", ExactSpelling = true)]
        public static extern void CompressedTexImage1D(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLint border, GLsizei imageSize, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompressedTexImage2D", ExactSpelling = true)]
        public static extern void CompressedTexImage2D(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompressedTexImage3D", ExactSpelling = true)]
        public static extern void CompressedTexImage3D(GLenum target,
            GLint level,
            GLenum internalformat,
            GLsizei width,
            GLsizei height,
            GLsizei depth,
            GLint border,
            GLsizei imageSize,
            System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompressedTexSubImage1D", ExactSpelling = true)]
        public static extern void CompressedTexSubImage1D(GLenum target, GLint level, GLint xoffset, GLsizei width, GLenum format, GLsizei imageSize, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompressedTexSubImage2D", ExactSpelling = true)]
        public static extern void CompressedTexSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLsizei imageSize, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCompressedTexSubImage3D", ExactSpelling = true)]
        public static extern void CompressedTexSubImage3D(GLenum target,
            GLint level,
            GLint xoffset,
            GLint yoffset,
            GLint zoffset,
            GLsizei width,
            GLsizei height,
            GLsizei depth,
            GLenum format,
            GLsizei imageSize,
            System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glConvolutionFilter1D", ExactSpelling = true)]
        public static extern void ConvolutionFilter1D(GLenum target, GLenum internalformat, GLsizei width, GLenum format, GLenum type, System.IntPtr image);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glConvolutionFilter2D", ExactSpelling = true)]
        public static extern void ConvolutionFilter2D(GLenum target, GLenum internalformat, GLsizei width, GLsizei height, GLenum format, GLenum type, System.IntPtr image);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glConvolutionParameterf", ExactSpelling = true)]
        public static extern void ConvolutionParameterf(GLenum target, GLenum pname, GLfloat @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glConvolutionParameterfv", ExactSpelling = true)]
        public static extern void ConvolutionParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glConvolutionParameteri", ExactSpelling = true)]
        public static extern void ConvolutionParameteri(GLenum target, GLenum pname, GLint @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glConvolutionParameteriv", ExactSpelling = true)]
        public static extern void ConvolutionParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyColorSubTable", ExactSpelling = true)]
        public static extern void CopyColorSubTable(GLenum target, GLsizei start, GLint x, GLint y, GLsizei width);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyColorTable", ExactSpelling = true)]
        public static extern void CopyColorTable(GLenum target, GLenum internalformat, GLint x, GLint y, GLsizei width);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyConvolutionFilter1D", ExactSpelling = true)]
        public static extern void CopyConvolutionFilter1D(GLenum target, GLenum internalformat, GLint x, GLint y, GLsizei width);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyConvolutionFilter2D", ExactSpelling = true)]
        public static extern void CopyConvolutionFilter2D(GLenum target, GLenum internalformat, GLint x, GLint y, GLsizei width, GLsizei height);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyPixels", ExactSpelling = true)]
        public static extern void CopyPixels(GLint x, GLint y, GLsizei width, GLsizei height, GLenum type);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyTexImage1D", ExactSpelling = true)]
        public static extern void CopyTexImage1D(GLenum target, GLint level, GLenum internalformat, GLint x, GLint y, GLsizei width, GLint border);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyTexImage2D", ExactSpelling = true)]
        public static extern void CopyTexImage2D(GLenum target, GLint level, GLenum internalformat, GLint x, GLint y, GLsizei width, GLsizei height, GLint border);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyTexSubImage1D", ExactSpelling = true)]
        public static extern void CopyTexSubImage1D(GLenum target, GLint level, GLint xoffset, GLint x, GLint y, GLsizei width);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyTexSubImage2D", ExactSpelling = true)]
        public static extern void CopyTexSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCopyTexSubImage3D", ExactSpelling = true)]
        public static extern void CopyTexSubImage3D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLint x, GLint y, GLsizei width, GLsizei height);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCreateProgram", ExactSpelling = true)]
        public static extern GLuint CreateProgram();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCreateShader", ExactSpelling = true)]
        public static extern GLuint CreateShader(GLenum type);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glCullFace", ExactSpelling = true)]
        public static extern void CullFace(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDeleteBuffers", ExactSpelling = true)]
        public static extern void DeleteBuffers(GLsizei n, System.IntPtr buffers);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDeleteLists", ExactSpelling = true)]
        public static extern void DeleteLists(GLuint list, GLsizei range);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDeleteProgram", ExactSpelling = true)]
        public static extern void DeleteProgram(GLuint program);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDeleteQueries", ExactSpelling = true)]
        public static extern void DeleteQueries(GLsizei n, System.IntPtr ids);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDeleteShader", ExactSpelling = true)]
        public static extern void DeleteShader(GLuint shader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDeleteTextures", ExactSpelling = true)]
        public static extern void DeleteTextures(GLsizei n, System.IntPtr textures);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDepthFunc", ExactSpelling = true)]
        public static extern void DepthFunc(GLenum func);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDepthMask", ExactSpelling = true)]
        public static extern void DepthMask(GLboolean flag);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDepthRange", ExactSpelling = true)]
        public static extern void DepthRange(GLclampd near, GLclampd far);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDetachShader", ExactSpelling = true)]
        public static extern void DetachShader(GLuint program, GLuint shader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDisable", ExactSpelling = true)]
        public static extern void Disable(GLenum cap);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDisableClientState", ExactSpelling = true)]
        public static extern void DisableClientState(GLenum array);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDisableVertexAttribArray", ExactSpelling = true)]
        public static extern void DisableVertexAttribArray(GLuint index);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDrawArrays", ExactSpelling = true)]
        public static extern void DrawArrays(GLenum mode, GLint first, GLsizei count);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDrawBuffer", ExactSpelling = true)]
        public static extern void DrawBuffer(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDrawBuffers", ExactSpelling = true)]
        public static extern void DrawBuffers(GLsizei n, System.IntPtr bufs);

        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDrawElements", ExactSpelling = true)]
        public static extern void DrawElements(GLenum mode, GLsizei count, GLenum type, System.IntPtr indices);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDrawPixels", ExactSpelling = true)]
        public static extern void DrawPixels(GLsizei width, GLsizei height, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glDrawRangeElements", ExactSpelling = true)]
        public static extern void DrawRangeElements(GLenum mode, GLuint start, GLuint end, GLsizei count, GLenum type, System.IntPtr indices);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEdgeFlag", ExactSpelling = true)]
        public static extern void EdgeFlag(GLboolean flag);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEdgeFlagPointer", ExactSpelling = true)]
        public static extern void EdgeFlagPointer(GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEdgeFlagv", ExactSpelling = true)]
        public static extern void EdgeFlagv(System.IntPtr flag);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEnable", ExactSpelling = true)]
        public static extern void Enable(GLenum cap);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEnableClientState", ExactSpelling = true)]
        public static extern void EnableClientState(GLenum array);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEnableVertexAttribArray", ExactSpelling = true)]
        public static extern void EnableVertexAttribArray(GLuint index);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEnd", ExactSpelling = true)]
        public static extern void End();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEndList", ExactSpelling = true)]
        public static extern void EndList();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEndQuery", ExactSpelling = true)]
        public static extern void EndQuery(GLenum target);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord1d", ExactSpelling = true)]
        public static extern void EvalCoord1d(GLdouble u);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord1dv", ExactSpelling = true)]
        public static extern void EvalCoord1dv(System.IntPtr u);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord1f", ExactSpelling = true)]
        public static extern void EvalCoord1f(GLfloat u);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord1fv", ExactSpelling = true)]
        public static extern void EvalCoord1fv(System.IntPtr u);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord2d", ExactSpelling = true)]
        public static extern void EvalCoord2d(GLdouble u, GLdouble v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord2dv", ExactSpelling = true)]
        public static extern void EvalCoord2dv(System.IntPtr u);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord2f", ExactSpelling = true)]
        public static extern void EvalCoord2f(GLfloat u, GLfloat v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalCoord2fv", ExactSpelling = true)]
        public static extern void EvalCoord2fv(System.IntPtr u);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalMesh1", ExactSpelling = true)]
        public static extern void EvalMesh1(GLenum mode, GLint i1, GLint i2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalMesh2", ExactSpelling = true)]
        public static extern void EvalMesh2(GLenum mode, GLint i1, GLint i2, GLint j1, GLint j2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalPoint1", ExactSpelling = true)]
        public static extern void EvalPoint1(GLint i);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEvalPoint2", ExactSpelling = true)]
        public static extern void EvalPoint2(GLint i, GLint j);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFeedbackBuffer", ExactSpelling = true)]
        public static extern void FeedbackBuffer(GLsizei size, GLenum type, System.IntPtr buffer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFinish", ExactSpelling = true)]
        public static extern void Finish();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFlush", ExactSpelling = true)]
        public static extern void Flush();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogCoordd", ExactSpelling = true)]
        public static extern void FogCoordd(GLdouble coord);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogCoorddv", ExactSpelling = true)]
        public static extern void FogCoorddv(System.IntPtr coord);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogCoordf", ExactSpelling = true)]
        public static extern void FogCoordf(GLfloat coord);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogCoordfv", ExactSpelling = true)]
        public static extern void FogCoordfv(System.IntPtr coord);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogCoordPointer", ExactSpelling = true)]
        public static extern void FogCoordPointer(GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogf", ExactSpelling = true)]
        public static extern void Fogf(GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogfv", ExactSpelling = true)]
        public static extern void Fogfv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogi", ExactSpelling = true)]
        public static extern void Fogi(GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFogiv", ExactSpelling = true)]
        public static extern void Fogiv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFrontFace", ExactSpelling = true)]
        public static extern void FrontFace(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFrustum", ExactSpelling = true)]
        public static extern void Frustum(GLdouble left, GLdouble right, GLdouble bottom, GLdouble top, GLdouble zNear, GLdouble zFar);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGenBuffers", ExactSpelling = true)]
        public static extern void GenBuffers(GLsizei n, System.IntPtr buffers);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGenLists", ExactSpelling = true)]
        public static extern GLuint GenLists(GLsizei range);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGenQueries", ExactSpelling = true)]
        public static extern void GenQueries(GLsizei n, System.IntPtr ids);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGenTextures", ExactSpelling = true)]
        public static extern void GenTextures(GLsizei n, System.IntPtr textures);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetActiveAttrib", ExactSpelling = true)]
        public static extern void GetActiveAttrib(GLuint program, GLuint index, GLsizei bufSize, System.IntPtr length, System.IntPtr size, System.IntPtr type, System.Text.StringBuilder name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetActiveUniform", ExactSpelling = true)]
        public static extern void GetActiveUniform(GLuint program, GLuint index, GLsizei bufSize, System.IntPtr length, System.IntPtr size, System.IntPtr type, System.Text.StringBuilder name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetAttachedShaders", ExactSpelling = true)]
        public static extern void GetAttachedShaders(GLuint program, GLsizei maxCount, System.IntPtr count, System.IntPtr obj);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetAttribLocation", ExactSpelling = true)]
        public static extern GLint GetAttribLocation(GLuint program, System.String name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetBooleanv", ExactSpelling = true)]
        public static extern void GetBooleanv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetBufferParameteriv", ExactSpelling = true)]
        public static extern void GetBufferParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetBufferPointerv", ExactSpelling = true)]
        public static extern void GetBufferPointerv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetBufferSubData", ExactSpelling = true)]
        public static extern void GetBufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, System.IntPtr data);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetClipPlane", ExactSpelling = true)]
        public static extern void GetClipPlane(GLenum plane, System.IntPtr equation);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetColorTable", ExactSpelling = true)]
        public static extern void GetColorTable(GLenum target, GLenum format, GLenum type, System.IntPtr table);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetColorTableParameterfv", ExactSpelling = true)]
        public static extern void GetColorTableParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetColorTableParameteriv", ExactSpelling = true)]
        public static extern void GetColorTableParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetCompressedTexImage", ExactSpelling = true)]
        public static extern void GetCompressedTexImage(GLenum target, GLint level, System.IntPtr img);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetConvolutionFilter", ExactSpelling = true)]
        public static extern void GetConvolutionFilter(GLenum target, GLenum format, GLenum type, System.IntPtr image);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetConvolutionParameterfv", ExactSpelling = true)]
        public static extern void GetConvolutionParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetConvolutionParameteriv", ExactSpelling = true)]
        public static extern void GetConvolutionParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetDoublev", ExactSpelling = true)]
        public static extern void GetDoublev(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetError", ExactSpelling = true)]
        public static extern GLenum GetError();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetFloatv", ExactSpelling = true)]
        public static extern void GetFloatv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetHistogram", ExactSpelling = true)]
        public static extern void GetHistogram(GLenum target, GLboolean reset, GLenum format, GLenum type, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetHistogramParameterfv", ExactSpelling = true)]
        public static extern void GetHistogramParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetHistogramParameteriv", ExactSpelling = true)]
        public static extern void GetHistogramParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetIntegerv", ExactSpelling = true)]
        public static extern void GetIntegerv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetLightfv", ExactSpelling = true)]
        public static extern void GetLightfv(GLenum light, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetLightiv", ExactSpelling = true)]
        public static extern void GetLightiv(GLenum light, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMapdv", ExactSpelling = true)]
        public static extern void GetMapdv(GLenum target, GLenum query, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMapfv", ExactSpelling = true)]
        public static extern void GetMapfv(GLenum target, GLenum query, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMapiv", ExactSpelling = true)]
        public static extern void GetMapiv(GLenum target, GLenum query, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMaterialfv", ExactSpelling = true)]
        public static extern void GetMaterialfv(GLenum face, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMaterialiv", ExactSpelling = true)]
        public static extern void GetMaterialiv(GLenum face, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMinmax", ExactSpelling = true)]
        public static extern void GetMinmax(GLenum target, GLboolean reset, GLenum format, GLenum type, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMinmaxParameterfv", ExactSpelling = true)]
        public static extern void GetMinmaxParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetMinmaxParameteriv", ExactSpelling = true)]
        public static extern void GetMinmaxParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetPixelMapfv", ExactSpelling = true)]
        public static extern void GetPixelMapfv(GLenum map, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetPixelMapuiv", ExactSpelling = true)]
        public static extern void GetPixelMapuiv(GLenum map, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetPixelMapusv", ExactSpelling = true)]
        public static extern void GetPixelMapusv(GLenum map, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetPointerv", ExactSpelling = true)]
        public static extern void GetPointerv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetPolygonStipple", ExactSpelling = true)]
        public static extern void GetPolygonStipple(System.IntPtr mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetProgramInfoLog", ExactSpelling = true)]
        public static extern void GetProgramInfoLog(GLuint program, GLsizei bufSize, System.IntPtr length, System.Text.StringBuilder infoLog);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetProgramiv", ExactSpelling = true)]
        public static extern void GetProgramiv(GLuint program, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetQueryiv", ExactSpelling = true)]
        public static extern void GetQueryiv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetQueryObjectiv", ExactSpelling = true)]
        public static extern void GetQueryObjectiv(GLuint id, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetQueryObjectuiv", ExactSpelling = true)]
        public static extern void GetQueryObjectuiv(GLuint id, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetSeparableFilter", ExactSpelling = true)]
        public static extern void GetSeparableFilter(GLenum target, GLenum format, GLenum type, System.IntPtr row, System.IntPtr column, System.IntPtr span);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetShaderInfoLog", ExactSpelling = true)]
        public static extern void GetShaderInfoLog(GLuint shader, GLsizei bufSize, System.IntPtr length, System.Text.StringBuilder infoLog);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetShaderiv", ExactSpelling = true)]
        public static extern void GetShaderiv(GLuint shader, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetShaderSource", ExactSpelling = true)]
        public static extern void GetShaderSource(GLuint shader, GLsizei bufSize, System.IntPtr length, System.Text.StringBuilder source);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetString", ExactSpelling = true)]
        public static extern GLsizeiptr GetString(GLenum name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexEnvfv", ExactSpelling = true)]
        public static extern void GetTexEnvfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexEnviv", ExactSpelling = true)]
        public static extern void GetTexEnviv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexGendv", ExactSpelling = true)]
        public static extern void GetTexGendv(GLenum coord, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexGenfv", ExactSpelling = true)]
        public static extern void GetTexGenfv(GLenum coord, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexGeniv", ExactSpelling = true)]
        public static extern void GetTexGeniv(GLenum coord, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexImage", ExactSpelling = true)]
        public static extern void GetTexImage(GLenum target, GLint level, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexLevelParameterfv", ExactSpelling = true)]
        public static extern void GetTexLevelParameterfv(GLenum target, GLint level, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexLevelParameteriv", ExactSpelling = true)]
        public static extern void GetTexLevelParameteriv(GLenum target, GLint level, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexParameterfv", ExactSpelling = true)]
        public static extern void GetTexParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetTexParameteriv", ExactSpelling = true)]
        public static extern void GetTexParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetUniformfv", ExactSpelling = true)]
        public static extern void GetUniformfv(GLuint program, GLint location, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetUniformiv", ExactSpelling = true)]
        public static extern void GetUniformiv(GLuint program, GLint location, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetUniformLocation", ExactSpelling = true)]
        public static extern GLint GetUniformLocation(GLuint program, System.String name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetVertexAttribdv", ExactSpelling = true)]
        public static extern void GetVertexAttribdv(GLuint index, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetVertexAttribfv", ExactSpelling = true)]
        public static extern void GetVertexAttribfv(GLuint index, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetVertexAttribiv", ExactSpelling = true)]
        public static extern void GetVertexAttribiv(GLuint index, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetVertexAttribPointerv", ExactSpelling = true)]
        public static extern void GetVertexAttribPointerv(GLuint index, GLenum pname, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glHint", ExactSpelling = true)]
        public static extern void Hint(GLenum target, GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glHistogram", ExactSpelling = true)]
        public static extern void Histogram(GLenum target, GLsizei width, GLenum internalformat, GLboolean sink);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexd", ExactSpelling = true)]
        public static extern void Indexd(GLdouble c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexdv", ExactSpelling = true)]
        public static extern void Indexdv(System.IntPtr c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexf", ExactSpelling = true)]
        public static extern void Indexf(GLfloat c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexfv", ExactSpelling = true)]
        public static extern void Indexfv(System.IntPtr c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexi", ExactSpelling = true)]
        public static extern void Indexi(GLint c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexiv", ExactSpelling = true)]
        public static extern void Indexiv(System.IntPtr c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexMask", ExactSpelling = true)]
        public static extern void IndexMask(GLuint mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexPointer", ExactSpelling = true)]
        public static extern void IndexPointer(GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexs", ExactSpelling = true)]
        public static extern void Indexs(GLshort c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexsv", ExactSpelling = true)]
        public static extern void Indexsv(System.IntPtr c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexub", ExactSpelling = true)]
        public static extern void Indexub(GLubyte c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIndexubv", ExactSpelling = true)]
        public static extern void Indexubv(System.IntPtr c);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glInitNames", ExactSpelling = true)]
        public static extern void InitNames();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glInterleavedArrays", ExactSpelling = true)]
        public static extern void InterleavedArrays(GLenum format, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsBuffer", ExactSpelling = true)]
        public static extern GLboolean IsBuffer(GLuint buffer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsEnabled", ExactSpelling = true)]
        public static extern GLboolean IsEnabled(GLenum cap);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsList", ExactSpelling = true)]
        public static extern GLboolean IsList(GLuint list);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsProgram", ExactSpelling = true)]
        public static extern GLboolean IsProgram(GLuint program);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsQuery", ExactSpelling = true)]
        public static extern GLboolean IsQuery(GLuint id);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsShader", ExactSpelling = true)]
        public static extern GLboolean IsShader(GLuint shader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glIsTexture", ExactSpelling = true)]
        public static extern GLboolean IsTexture(GLuint texture);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightf", ExactSpelling = true)]
        public static extern void Lightf(GLenum light, GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightfv", ExactSpelling = true)]
        public static extern void Lightfv(GLenum light, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLighti", ExactSpelling = true)]
        public static extern void Lighti(GLenum light, GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightiv", ExactSpelling = true)]
        public static extern void Lightiv(GLenum light, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightModelf", ExactSpelling = true)]
        public static extern void LightModelf(GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightModelfv", ExactSpelling = true)]
        public static extern void LightModelfv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightModeli", ExactSpelling = true)]
        public static extern void LightModeli(GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLightModeliv", ExactSpelling = true)]
        public static extern void LightModeliv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLineStipple", ExactSpelling = true)]
        public static extern void LineStipple(GLint factor, GLushort pattern);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLineWidth", ExactSpelling = true)]
        public static extern void LineWidth(GLfloat width);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLinkProgram", ExactSpelling = true)]
        public static extern void LinkProgram(GLuint program);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glListBase", ExactSpelling = true)]
        public static extern void ListBase(GLuint @base);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadIdentity", ExactSpelling = true)]
        public static extern void LoadIdentity();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadMatrixd", ExactSpelling = true)]
        public static extern void LoadMatrixd(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadMatrixf", ExactSpelling = true)]
        public static extern void LoadMatrixf(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadName", ExactSpelling = true)]
        public static extern void LoadName(GLuint name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadTransposeMatrixd", ExactSpelling = true)]
        public static extern void LoadTransposeMatrixd(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadTransposeMatrixf", ExactSpelling = true)]
        public static extern void LoadTransposeMatrixf(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLogicOp", ExactSpelling = true)]
        public static extern void LogicOp(GLenum opcode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMap1d", ExactSpelling = true)]
        public static extern void Map1d(GLenum target, GLdouble u1, GLdouble u2, GLint stride, GLint order, System.IntPtr points);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMap1f", ExactSpelling = true)]
        public static extern void Map1f(GLenum target, GLfloat u1, GLfloat u2, GLint stride, GLint order, System.IntPtr points);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMap2d", ExactSpelling = true)]
        public static extern void Map2d(GLenum target, GLdouble u1, GLdouble u2, GLint ustride, GLint uorder, GLdouble v1, GLdouble v2, GLint vstride, GLint vorder, System.IntPtr points);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMap2f", ExactSpelling = true)]
        public static extern void Map2f(GLenum target, GLfloat u1, GLfloat u2, GLint ustride, GLint uorder, GLfloat v1, GLfloat v2, GLint vstride, GLint vorder, System.IntPtr points);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMapBuffer", ExactSpelling = true)]
        public static extern GLsizeiptr MapBuffer(GLenum target, GLenum access);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMapGrid1d", ExactSpelling = true)]
        public static extern void MapGrid1d(GLint un, GLdouble u1, GLdouble u2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMapGrid1f", ExactSpelling = true)]
        public static extern void MapGrid1f(GLint un, GLfloat u1, GLfloat u2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMapGrid2d", ExactSpelling = true)]
        public static extern void MapGrid2d(GLint un, GLdouble u1, GLdouble u2, GLint vn, GLdouble v1, GLdouble v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMapGrid2f", ExactSpelling = true)]
        public static extern void MapGrid2f(GLint un, GLfloat u1, GLfloat u2, GLint vn, GLfloat v1, GLfloat v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMaterialf", ExactSpelling = true)]
        public static extern void Materialf(GLenum face, GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMaterialfv", ExactSpelling = true)]
        public static extern void Materialfv(GLenum face, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMateriali", ExactSpelling = true)]
        public static extern void Materiali(GLenum face, GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMaterialiv", ExactSpelling = true)]
        public static extern void Materialiv(GLenum face, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMatrixMode", ExactSpelling = true)]
        public static extern void MatrixMode(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMinmax", ExactSpelling = true)]
        public static extern void Minmax(GLenum target, GLenum internalformat, GLboolean sink);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiDrawArrays", ExactSpelling = true)]
        public static extern void MultiDrawArrays(GLenum mode, System.IntPtr first, System.IntPtr count, GLsizei primcount);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiDrawElements", ExactSpelling = true)]
        public static extern void MultiDrawElements(GLenum mode, System.IntPtr count, GLenum type, System.IntPtr indices, GLsizei primcount);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1d", ExactSpelling = true)]
        public static extern void MultiTexCoord1d(GLenum target, GLdouble s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1dv", ExactSpelling = true)]
        public static extern void MultiTexCoord1dv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1f", ExactSpelling = true)]
        public static extern void MultiTexCoord1f(GLenum target, GLfloat s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1fv", ExactSpelling = true)]
        public static extern void MultiTexCoord1fv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1i", ExactSpelling = true)]
        public static extern void MultiTexCoord1i(GLenum target, GLint s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1iv", ExactSpelling = true)]
        public static extern void MultiTexCoord1iv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1s", ExactSpelling = true)]
        public static extern void MultiTexCoord1s(GLenum target, GLshort s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord1sv", ExactSpelling = true)]
        public static extern void MultiTexCoord1sv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2d", ExactSpelling = true)]
        public static extern void MultiTexCoord2d(GLenum target, GLdouble s, GLdouble t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2dv", ExactSpelling = true)]
        public static extern void MultiTexCoord2dv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2f", ExactSpelling = true)]
        public static extern void MultiTexCoord2f(GLenum target, GLfloat s, GLfloat t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2fv", ExactSpelling = true)]
        public static extern void MultiTexCoord2fv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2i", ExactSpelling = true)]
        public static extern void MultiTexCoord2i(GLenum target, GLint s, GLint t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2iv", ExactSpelling = true)]
        public static extern void MultiTexCoord2iv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2s", ExactSpelling = true)]
        public static extern void MultiTexCoord2s(GLenum target, GLshort s, GLshort t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord2sv", ExactSpelling = true)]
        public static extern void MultiTexCoord2sv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3d", ExactSpelling = true)]
        public static extern void MultiTexCoord3d(GLenum target, GLdouble s, GLdouble t, GLdouble r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3dv", ExactSpelling = true)]
        public static extern void MultiTexCoord3dv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3f", ExactSpelling = true)]
        public static extern void MultiTexCoord3f(GLenum target, GLfloat s, GLfloat t, GLfloat r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3fv", ExactSpelling = true)]
        public static extern void MultiTexCoord3fv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3i", ExactSpelling = true)]
        public static extern void MultiTexCoord3i(GLenum target, GLint s, GLint t, GLint r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3iv", ExactSpelling = true)]
        public static extern void MultiTexCoord3iv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3s", ExactSpelling = true)]
        public static extern void MultiTexCoord3s(GLenum target, GLshort s, GLshort t, GLshort r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord3sv", ExactSpelling = true)]
        public static extern void MultiTexCoord3sv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4d", ExactSpelling = true)]
        public static extern void MultiTexCoord4d(GLenum target, GLdouble s, GLdouble t, GLdouble r, GLdouble q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4dv", ExactSpelling = true)]
        public static extern void MultiTexCoord4dv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4f", ExactSpelling = true)]
        public static extern void MultiTexCoord4f(GLenum target, GLfloat s, GLfloat t, GLfloat r, GLfloat q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4fv", ExactSpelling = true)]
        public static extern void MultiTexCoord4fv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4i", ExactSpelling = true)]
        public static extern void MultiTexCoord4i(GLenum target, GLint s, GLint t, GLint r, GLint q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4iv", ExactSpelling = true)]
        public static extern void MultiTexCoord4iv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4s", ExactSpelling = true)]
        public static extern void MultiTexCoord4s(GLenum target, GLshort s, GLshort t, GLshort r, GLshort q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultiTexCoord4sv", ExactSpelling = true)]
        public static extern void MultiTexCoord4sv(GLenum target, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultMatrixd", ExactSpelling = true)]
        public static extern void MultMatrixd(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultMatrixf", ExactSpelling = true)]
        public static extern void MultMatrixf(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultTransposeMatrixd", ExactSpelling = true)]
        public static extern void MultTransposeMatrixd(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMultTransposeMatrixf", ExactSpelling = true)]
        public static extern void MultTransposeMatrixf(System.IntPtr m);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNewList", ExactSpelling = true)]
        public static extern void NewList(GLuint list, GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3b", ExactSpelling = true)]
        public static extern void Normal3b(GLbyte nx, GLbyte ny, GLbyte nz);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3bv", ExactSpelling = true)]
        public static extern void Normal3bv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3d", ExactSpelling = true)]
        public static extern void Normal3d(GLdouble nx, GLdouble ny, GLdouble nz);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3dv", ExactSpelling = true)]
        public static extern void Normal3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3f", ExactSpelling = true)]
        public static extern void Normal3f(GLfloat nx, GLfloat ny, GLfloat nz);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3fv", ExactSpelling = true)]
        public static extern void Normal3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3i", ExactSpelling = true)]
        public static extern void Normal3i(GLint nx, GLint ny, GLint nz);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3iv", ExactSpelling = true)]
        public static extern void Normal3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3s", ExactSpelling = true)]
        public static extern void Normal3s(GLshort nx, GLshort ny, GLshort nz);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormal3sv", ExactSpelling = true)]
        public static extern void Normal3sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glNormalPointer", ExactSpelling = true)]
        public static extern void NormalPointer(GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glOrtho", ExactSpelling = true)]
        public static extern void Ortho(GLdouble left, GLdouble right, GLdouble bottom, GLdouble top, GLdouble zNear, GLdouble zFar);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPassThrough", ExactSpelling = true)]
        public static extern void PassThrough(GLfloat token);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelMapfv", ExactSpelling = true)]
        public static extern void PixelMapfv(GLenum map, GLint mapsize, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelMapuiv", ExactSpelling = true)]
        public static extern void PixelMapuiv(GLenum map, GLint mapsize, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelMapusv", ExactSpelling = true)]
        public static extern void PixelMapusv(GLenum map, GLint mapsize, System.IntPtr values);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelStoref", ExactSpelling = true)]
        public static extern void PixelStoref(GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelStorei", ExactSpelling = true)]
        public static extern void PixelStorei(GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelTransferf", ExactSpelling = true)]
        public static extern void PixelTransferf(GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelTransferi", ExactSpelling = true)]
        public static extern void PixelTransferi(GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPixelZoom", ExactSpelling = true)]
        public static extern void PixelZoom(GLfloat xfactor, GLfloat yfactor);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPointParameterf", ExactSpelling = true)]
        public static extern void PointParameterf(GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPointParameterfv", ExactSpelling = true)]
        public static extern void PointParameterfv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPointParameteri", ExactSpelling = true)]
        public static extern void PointParameteri(GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPointParameteriv", ExactSpelling = true)]
        public static extern void PointParameteriv(GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPointSize", ExactSpelling = true)]
        public static extern void PointSize(GLfloat size);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPolygonMode", ExactSpelling = true)]
        public static extern void PolygonMode(GLenum face, GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPolygonOffset", ExactSpelling = true)]
        public static extern void PolygonOffset(GLfloat factor, GLfloat units);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPolygonStipple", ExactSpelling = true)]
        public static extern void PolygonStipple(System.IntPtr mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPopAttrib", ExactSpelling = true)]
        public static extern void PopAttrib();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPopClientAttrib", ExactSpelling = true)]
        public static extern void PopClientAttrib();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPopMatrix", ExactSpelling = true)]
        public static extern void PopMatrix();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPopName", ExactSpelling = true)]
        public static extern void PopName();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPrioritizeTextures", ExactSpelling = true)]
        public static extern void PrioritizeTextures(GLsizei n, System.IntPtr textures, System.IntPtr priorities);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPushAttrib", ExactSpelling = true)]
        public static extern void PushAttrib(GLbitfield mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPushClientAttrib", ExactSpelling = true)]
        public static extern void PushClientAttrib(GLbitfield mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPushMatrix", ExactSpelling = true)]
        public static extern void PushMatrix();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glPushName", ExactSpelling = true)]
        public static extern void PushName(GLuint name);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2d", ExactSpelling = true)]
        public static extern void RasterPos2d(GLdouble x, GLdouble y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2dv", ExactSpelling = true)]
        public static extern void RasterPos2dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2f", ExactSpelling = true)]
        public static extern void RasterPos2f(GLfloat x, GLfloat y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2fv", ExactSpelling = true)]
        public static extern void RasterPos2fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2i", ExactSpelling = true)]
        public static extern void RasterPos2i(GLint x, GLint y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2iv", ExactSpelling = true)]
        public static extern void RasterPos2iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2s", ExactSpelling = true)]
        public static extern void RasterPos2s(GLshort x, GLshort y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos2sv", ExactSpelling = true)]
        public static extern void RasterPos2sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3d", ExactSpelling = true)]
        public static extern void RasterPos3d(GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3dv", ExactSpelling = true)]
        public static extern void RasterPos3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3f", ExactSpelling = true)]
        public static extern void RasterPos3f(GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3fv", ExactSpelling = true)]
        public static extern void RasterPos3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3i", ExactSpelling = true)]
        public static extern void RasterPos3i(GLint x, GLint y, GLint z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3iv", ExactSpelling = true)]
        public static extern void RasterPos3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3s", ExactSpelling = true)]
        public static extern void RasterPos3s(GLshort x, GLshort y, GLshort z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos3sv", ExactSpelling = true)]
        public static extern void RasterPos3sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4d", ExactSpelling = true)]
        public static extern void RasterPos4d(GLdouble x, GLdouble y, GLdouble z, GLdouble w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4dv", ExactSpelling = true)]
        public static extern void RasterPos4dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4f", ExactSpelling = true)]
        public static extern void RasterPos4f(GLfloat x, GLfloat y, GLfloat z, GLfloat w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4fv", ExactSpelling = true)]
        public static extern void RasterPos4fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4i", ExactSpelling = true)]
        public static extern void RasterPos4i(GLint x, GLint y, GLint z, GLint w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4iv", ExactSpelling = true)]
        public static extern void RasterPos4iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4s", ExactSpelling = true)]
        public static extern void RasterPos4s(GLshort x, GLshort y, GLshort z, GLshort w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRasterPos4sv", ExactSpelling = true)]
        public static extern void RasterPos4sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glReadBuffer", ExactSpelling = true)]
        public static extern void ReadBuffer(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glReadPixels", ExactSpelling = true)]
        public static extern void ReadPixels(GLint x, GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRectd", ExactSpelling = true)]
        public static extern void Rectd(GLdouble x1, GLdouble y1, GLdouble x2, GLdouble y2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRectdv", ExactSpelling = true)]
        public static extern void Rectdv(System.IntPtr v1, System.IntPtr v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRectf", ExactSpelling = true)]
        public static extern void Rectf(GLfloat x1, GLfloat y1, GLfloat x2, GLfloat y2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRectfv", ExactSpelling = true)]
        public static extern void Rectfv(System.IntPtr v1, System.IntPtr v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRecti", ExactSpelling = true)]
        public static extern void Recti(GLint x1, GLint y1, GLint x2, GLint y2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRectiv", ExactSpelling = true)]
        public static extern void Rectiv(System.IntPtr v1, System.IntPtr v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRects", ExactSpelling = true)]
        public static extern void Rects(GLshort x1, GLshort y1, GLshort x2, GLshort y2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRectsv", ExactSpelling = true)]
        public static extern void Rectsv(System.IntPtr v1, System.IntPtr v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRenderMode", ExactSpelling = true)]
        public static extern GLint RenderMode(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glResetHistogram", ExactSpelling = true)]
        public static extern void ResetHistogram(GLenum target);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glResetMinmax", ExactSpelling = true)]
        public static extern void ResetMinmax(GLenum target);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRotated", ExactSpelling = true)]
        public static extern void Rotated(GLdouble angle, GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glRotatef", ExactSpelling = true)]
        public static extern void Rotatef(GLfloat angle, GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSampleCoverage", ExactSpelling = true)]
        public static extern void SampleCoverage(GLclampf value, GLboolean invert);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glScaled", ExactSpelling = true)]
        public static extern void Scaled(GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glScalef", ExactSpelling = true)]
        public static extern void Scalef(GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glScissor", ExactSpelling = true)]
        public static extern void Scissor(GLint x, GLint y, GLsizei width, GLsizei height);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3b", ExactSpelling = true)]
        public static extern void SecondaryColor3b(GLbyte red, GLbyte green, GLbyte blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3bv", ExactSpelling = true)]
        public static extern void SecondaryColor3bv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3d", ExactSpelling = true)]
        public static extern void SecondaryColor3d(GLdouble red, GLdouble green, GLdouble blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3dv", ExactSpelling = true)]
        public static extern void SecondaryColor3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3f", ExactSpelling = true)]
        public static extern void SecondaryColor3f(GLfloat red, GLfloat green, GLfloat blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3fv", ExactSpelling = true)]
        public static extern void SecondaryColor3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3i", ExactSpelling = true)]
        public static extern void SecondaryColor3i(GLint red, GLint green, GLint blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3iv", ExactSpelling = true)]
        public static extern void SecondaryColor3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3s", ExactSpelling = true)]
        public static extern void SecondaryColor3s(GLshort red, GLshort green, GLshort blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3sv", ExactSpelling = true)]
        public static extern void SecondaryColor3sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3ub", ExactSpelling = true)]
        public static extern void SecondaryColor3ub(GLubyte red, GLubyte green, GLubyte blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3ubv", ExactSpelling = true)]
        public static extern void SecondaryColor3ubv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3ui", ExactSpelling = true)]
        public static extern void SecondaryColor3ui(GLuint red, GLuint green, GLuint blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3uiv", ExactSpelling = true)]
        public static extern void SecondaryColor3uiv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3us", ExactSpelling = true)]
        public static extern void SecondaryColor3us(GLushort red, GLushort green, GLushort blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColor3usv", ExactSpelling = true)]
        public static extern void SecondaryColor3usv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSecondaryColorPointer", ExactSpelling = true)]
        public static extern void SecondaryColorPointer(GLint size, GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSelectBuffer", ExactSpelling = true)]
        public static extern void SelectBuffer(GLsizei size, System.IntPtr buffer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glSeparableFilter2D", ExactSpelling = true)]
        public static extern void SeparableFilter2D(GLenum target, GLenum internalformat, GLsizei width, GLsizei height, GLenum format, GLenum type, System.IntPtr row, System.IntPtr column);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glShadeModel", ExactSpelling = true)]
        public static extern void ShadeModel(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glShaderSource", ExactSpelling = true)]
        public static extern void ShaderSource(GLuint shader, GLsizei count, System.String[] @string, System.IntPtr length);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glStencilFunc", ExactSpelling = true)]
        public static extern void StencilFunc(GLenum func, GLint @ref, GLuint mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glStencilFuncSeparate", ExactSpelling = true)]
        public static extern void StencilFuncSeparate(GLenum frontfunc, GLenum backfunc, GLint @ref, GLuint mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glStencilMask", ExactSpelling = true)]
        public static extern void StencilMask(GLuint mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glStencilMaskSeparate", ExactSpelling = true)]
        public static extern void StencilMaskSeparate(GLenum face, GLuint mask);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glStencilOp", ExactSpelling = true)]
        public static extern void StencilOp(GLenum fail, GLenum zfail, GLenum zpass);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glStencilOpSeparate", ExactSpelling = true)]
        public static extern void StencilOpSeparate(GLenum face, GLenum sfail, GLenum dpfail, GLenum dppass);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1d", ExactSpelling = true)]
        public static extern void TexCoord1d(GLdouble s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1dv", ExactSpelling = true)]
        public static extern void TexCoord1dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1f", ExactSpelling = true)]
        public static extern void TexCoord1f(GLfloat s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1fv", ExactSpelling = true)]
        public static extern void TexCoord1fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1i", ExactSpelling = true)]
        public static extern void TexCoord1i(GLint s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1iv", ExactSpelling = true)]
        public static extern void TexCoord1iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1s", ExactSpelling = true)]
        public static extern void TexCoord1s(GLshort s);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord1sv", ExactSpelling = true)]
        public static extern void TexCoord1sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2d", ExactSpelling = true)]
        public static extern void TexCoord2d(GLdouble s, GLdouble t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2dv", ExactSpelling = true)]
        public static extern void TexCoord2dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2f", ExactSpelling = true)]
        public static extern void TexCoord2f(GLfloat s, GLfloat t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2fv", ExactSpelling = true)]
        public static extern void TexCoord2fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2i", ExactSpelling = true)]
        public static extern void TexCoord2i(GLint s, GLint t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2iv", ExactSpelling = true)]
        public static extern void TexCoord2iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2s", ExactSpelling = true)]
        public static extern void TexCoord2s(GLshort s, GLshort t);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord2sv", ExactSpelling = true)]
        public static extern void TexCoord2sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3d", ExactSpelling = true)]
        public static extern void TexCoord3d(GLdouble s, GLdouble t, GLdouble r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3dv", ExactSpelling = true)]
        public static extern void TexCoord3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3f", ExactSpelling = true)]
        public static extern void TexCoord3f(GLfloat s, GLfloat t, GLfloat r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3fv", ExactSpelling = true)]
        public static extern void TexCoord3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3i", ExactSpelling = true)]
        public static extern void TexCoord3i(GLint s, GLint t, GLint r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3iv", ExactSpelling = true)]
        public static extern void TexCoord3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3s", ExactSpelling = true)]
        public static extern void TexCoord3s(GLshort s, GLshort t, GLshort r);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord3sv", ExactSpelling = true)]
        public static extern void TexCoord3sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4d", ExactSpelling = true)]
        public static extern void TexCoord4d(GLdouble s, GLdouble t, GLdouble r, GLdouble q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4dv", ExactSpelling = true)]
        public static extern void TexCoord4dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4f", ExactSpelling = true)]
        public static extern void TexCoord4f(GLfloat s, GLfloat t, GLfloat r, GLfloat q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4fv", ExactSpelling = true)]
        public static extern void TexCoord4fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4i", ExactSpelling = true)]
        public static extern void TexCoord4i(GLint s, GLint t, GLint r, GLint q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4iv", ExactSpelling = true)]
        public static extern void TexCoord4iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4s", ExactSpelling = true)]
        public static extern void TexCoord4s(GLshort s, GLshort t, GLshort r, GLshort q);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoord4sv", ExactSpelling = true)]
        public static extern void TexCoord4sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexCoordPointer", ExactSpelling = true)]
        public static extern void TexCoordPointer(GLint size, GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexEnvf", ExactSpelling = true)]
        public static extern void TexEnvf(GLenum target, GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexEnvfv", ExactSpelling = true)]
        public static extern void TexEnvfv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexEnvi", ExactSpelling = true)]
        public static extern void TexEnvi(GLenum target, GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexEnviv", ExactSpelling = true)]
        public static extern void TexEnviv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexGend", ExactSpelling = true)]
        public static extern void TexGend(GLenum coord, GLenum pname, GLdouble param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexGendv", ExactSpelling = true)]
        public static extern void TexGendv(GLenum coord, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexGenf", ExactSpelling = true)]
        public static extern void TexGenf(GLenum coord, GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexGenfv", ExactSpelling = true)]
        public static extern void TexGenfv(GLenum coord, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexGeni", ExactSpelling = true)]
        public static extern void TexGeni(GLenum coord, GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexGeniv", ExactSpelling = true)]
        public static extern void TexGeniv(GLenum coord, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexImage1D", ExactSpelling = true)]
        public static extern void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexImage2D", ExactSpelling = true)]
        public static extern void TexImage2D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexImage3D", ExactSpelling = true)]
        public static extern void TexImage3D(GLenum target,
            GLint level,
            GLint internalformat,
            GLsizei width,
            GLsizei height,
            GLsizei depth,
            GLint border,
            GLenum format,
            GLenum type,
            System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexParameterf", ExactSpelling = true)]
        public static extern void TexParameterf(GLenum target, GLenum pname, GLfloat param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexParameterfv", ExactSpelling = true)]
        public static extern void TexParameterfv(GLenum target, GLenum pname, System.IntPtr @params);

        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexParameteri", ExactSpelling = true)]
        public static extern void TexParameteri(GLenum target, GLenum pname, GLint param);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexParameteriv", ExactSpelling = true)]
        public static extern void TexParameteriv(GLenum target, GLenum pname, System.IntPtr @params);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexSubImage1D", ExactSpelling = true)]
        public static extern void TexSubImage1D(GLenum target, GLint level, GLint xoffset, GLsizei width, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexSubImage2D", ExactSpelling = true)]
        public static extern void TexSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTexSubImage3D", ExactSpelling = true)]
        public static extern void TexSubImage3D(GLenum target,
            GLint level,
            GLint xoffset,
            GLint yoffset,
            GLint zoffset,
            GLsizei width,
            GLsizei height,
            GLsizei depth,
            GLenum format,
            GLenum type,
            System.IntPtr pixels);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTranslated", ExactSpelling = true)]
        public static extern void Translated(GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glTranslatef", ExactSpelling = true)]
        public static extern void Translatef(GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform1f", ExactSpelling = true)]
        public static extern void Uniform1f(GLint location, GLfloat v0);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform1fv", ExactSpelling = true)]
        public static extern void Uniform1fv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform1i", ExactSpelling = true)]
        public static extern void Uniform1i(GLint location, GLint v0);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform1iv", ExactSpelling = true)]
        public static extern void Uniform1iv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform2f", ExactSpelling = true)]
        public static extern void Uniform2f(GLint location, GLfloat v0, GLfloat v1);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform2fv", ExactSpelling = true)]
        public static extern void Uniform2fv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform2i", ExactSpelling = true)]
        public static extern void Uniform2i(GLint location, GLint v0, GLint v1);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform2iv", ExactSpelling = true)]
        public static extern void Uniform2iv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform3f", ExactSpelling = true)]
        public static extern void Uniform3f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform3fv", ExactSpelling = true)]
        public static extern void Uniform3fv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform3i", ExactSpelling = true)]
        public static extern void Uniform3i(GLint location, GLint v0, GLint v1, GLint v2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform3iv", ExactSpelling = true)]
        public static extern void Uniform3iv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform4f", ExactSpelling = true)]
        public static extern void Uniform4f(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform4fv", ExactSpelling = true)]
        public static extern void Uniform4fv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform4i", ExactSpelling = true)]
        public static extern void Uniform4i(GLint location, GLint v0, GLint v1, GLint v2, GLint v3);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniform4iv", ExactSpelling = true)]
        public static extern void Uniform4iv(GLint location, GLsizei count, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix2fv", ExactSpelling = true)]
        public static extern void UniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix2x3fv", ExactSpelling = true)]
        public static extern void UniformMatrix2x3fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix2x4fv", ExactSpelling = true)]
        public static extern void UniformMatrix2x4fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix3fv", ExactSpelling = true)]
        public static extern void UniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix3x2fv", ExactSpelling = true)]
        public static extern void UniformMatrix3x2fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix3x4fv", ExactSpelling = true)]
        public static extern void UniformMatrix3x4fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix4fv", ExactSpelling = true)]
        public static extern void UniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix4x2fv", ExactSpelling = true)]
        public static extern void UniformMatrix4x2fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUniformMatrix4x3fv", ExactSpelling = true)]
        public static extern void UniformMatrix4x3fv(GLint location, GLsizei count, GLboolean transpose, System.IntPtr value);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUnmapBuffer", ExactSpelling = true)]
        public static extern GLboolean UnmapBuffer(GLenum target);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glUseProgram", ExactSpelling = true)]
        public static extern void UseProgram(GLuint program);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glValidateProgram", ExactSpelling = true)]
        public static extern void ValidateProgram(GLuint program);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2d", ExactSpelling = true)]
        public static extern void Vertex2d(GLdouble x, GLdouble y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2dv", ExactSpelling = true)]
        public static extern void Vertex2dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2f", ExactSpelling = true)]
        public static extern void Vertex2f(GLfloat x, GLfloat y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2fv", ExactSpelling = true)]
        public static extern void Vertex2fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2i", ExactSpelling = true)]
        public static extern void Vertex2i(GLint x, GLint y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2iv", ExactSpelling = true)]
        public static extern void Vertex2iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2s", ExactSpelling = true)]
        public static extern void Vertex2s(GLshort x, GLshort y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex2sv", ExactSpelling = true)]
        public static extern void Vertex2sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3d", ExactSpelling = true)]
        public static extern void Vertex3d(GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3dv", ExactSpelling = true)]
        public static extern void Vertex3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3f", ExactSpelling = true)]
        public static extern void Vertex3f(GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3fv", ExactSpelling = true)]
        public static extern void Vertex3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3i", ExactSpelling = true)]
        public static extern void Vertex3i(GLint x, GLint y, GLint z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3iv", ExactSpelling = true)]
        public static extern void Vertex3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3s", ExactSpelling = true)]
        public static extern void Vertex3s(GLshort x, GLshort y, GLshort z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3sv", ExactSpelling = true)]
        public static extern void Vertex3sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4d", ExactSpelling = true)]
        public static extern void Vertex4d(GLdouble x, GLdouble y, GLdouble z, GLdouble w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4dv", ExactSpelling = true)]
        public static extern void Vertex4dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4f", ExactSpelling = true)]
        public static extern void Vertex4f(GLfloat x, GLfloat y, GLfloat z, GLfloat w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4fv", ExactSpelling = true)]
        public static extern void Vertex4fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4i", ExactSpelling = true)]
        public static extern void Vertex4i(GLint x, GLint y, GLint z, GLint w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4iv", ExactSpelling = true)]
        public static extern void Vertex4iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4s", ExactSpelling = true)]
        public static extern void Vertex4s(GLshort x, GLshort y, GLshort z, GLshort w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex4sv", ExactSpelling = true)]
        public static extern void Vertex4sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib1d", ExactSpelling = true)]
        public static extern void VertexAttrib1d(GLuint index, GLdouble x);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib1dv", ExactSpelling = true)]
        public static extern void VertexAttrib1dv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib1f", ExactSpelling = true)]
        public static extern void VertexAttrib1f(GLuint index, GLfloat x);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib1fv", ExactSpelling = true)]
        public static extern void VertexAttrib1fv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib1s", ExactSpelling = true)]
        public static extern void VertexAttrib1s(GLuint index, GLshort x);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib1sv", ExactSpelling = true)]
        public static extern void VertexAttrib1sv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib2d", ExactSpelling = true)]
        public static extern void VertexAttrib2d(GLuint index, GLdouble x, GLdouble y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib2dv", ExactSpelling = true)]
        public static extern void VertexAttrib2dv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib2f", ExactSpelling = true)]
        public static extern void VertexAttrib2f(GLuint index, GLfloat x, GLfloat y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib2fv", ExactSpelling = true)]
        public static extern void VertexAttrib2fv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib2s", ExactSpelling = true)]
        public static extern void VertexAttrib2s(GLuint index, GLshort x, GLshort y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib2sv", ExactSpelling = true)]
        public static extern void VertexAttrib2sv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib3d", ExactSpelling = true)]
        public static extern void VertexAttrib3d(GLuint index, GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib3dv", ExactSpelling = true)]
        public static extern void VertexAttrib3dv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib3f", ExactSpelling = true)]
        public static extern void VertexAttrib3f(GLuint index, GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib3fv", ExactSpelling = true)]
        public static extern void VertexAttrib3fv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib3s", ExactSpelling = true)]
        public static extern void VertexAttrib3s(GLuint index, GLshort x, GLshort y, GLshort z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib3sv", ExactSpelling = true)]
        public static extern void VertexAttrib3sv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4bv", ExactSpelling = true)]
        public static extern void VertexAttrib4bv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4d", ExactSpelling = true)]
        public static extern void VertexAttrib4d(GLuint index, GLdouble x, GLdouble y, GLdouble z, GLdouble w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4dv", ExactSpelling = true)]
        public static extern void VertexAttrib4dv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4f", ExactSpelling = true)]
        public static extern void VertexAttrib4f(GLuint index, GLfloat x, GLfloat y, GLfloat z, GLfloat w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4fv", ExactSpelling = true)]
        public static extern void VertexAttrib4fv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4iv", ExactSpelling = true)]
        public static extern void VertexAttrib4iv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Nbv", ExactSpelling = true)]
        public static extern void VertexAttrib4Nbv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Niv", ExactSpelling = true)]
        public static extern void VertexAttrib4Niv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Nsv", ExactSpelling = true)]
        public static extern void VertexAttrib4Nsv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Nub", ExactSpelling = true)]
        public static extern void VertexAttrib4Nub(GLuint index, GLubyte x, GLubyte y, GLubyte z, GLubyte w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Nubv", ExactSpelling = true)]
        public static extern void VertexAttrib4Nubv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Nuiv", ExactSpelling = true)]
        public static extern void VertexAttrib4Nuiv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4Nusv", ExactSpelling = true)]
        public static extern void VertexAttrib4Nusv(GLuint index, System.IntPtr v);

        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4s", ExactSpelling = true)]
        public static extern void VertexAttrib4s(GLuint index, GLshort x, GLshort y, GLshort z, GLshort w);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4sv", ExactSpelling = true)]
        public static extern void VertexAttrib4sv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4ubv", ExactSpelling = true)]
        public static extern void VertexAttrib4ubv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4uiv", ExactSpelling = true)]
        public static extern void VertexAttrib4uiv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttrib4usv", ExactSpelling = true)]
        public static extern void VertexAttrib4usv(GLuint index, System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexAttribPointer", ExactSpelling = true)]
        public static extern void VertexAttribPointer(GLuint index, GLint size, GLenum type, GLboolean normalized, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertexPointer", ExactSpelling = true)]
        public static extern void VertexPointer(GLint size, GLenum type, GLsizei stride, System.IntPtr pointer);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glViewport", ExactSpelling = true)]
        public static extern void Viewport(GLint x, GLint y, GLsizei width, GLsizei height);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2d", ExactSpelling = true)]
        public static extern void WindowPos2d(GLdouble x, GLdouble y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2dv", ExactSpelling = true)]
        public static extern void WindowPos2dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2f", ExactSpelling = true)]
        public static extern void WindowPos2f(GLfloat x, GLfloat y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2fv", ExactSpelling = true)]
        public static extern void WindowPos2fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2i", ExactSpelling = true)]
        public static extern void WindowPos2i(GLint x, GLint y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2iv", ExactSpelling = true)]
        public static extern void WindowPos2iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2s", ExactSpelling = true)]
        public static extern void WindowPos2s(GLshort x, GLshort y);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos2sv", ExactSpelling = true)]
        public static extern void WindowPos2sv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3d", ExactSpelling = true)]
        public static extern void WindowPos3d(GLdouble x, GLdouble y, GLdouble z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3dv", ExactSpelling = true)]
        public static extern void WindowPos3dv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3f", ExactSpelling = true)]
        public static extern void WindowPos3f(GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3fv", ExactSpelling = true)]
        public static extern void WindowPos3fv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3i", ExactSpelling = true)]
        public static extern void WindowPos3i(GLint x, GLint y, GLint z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3iv", ExactSpelling = true)]
        public static extern void WindowPos3iv(System.IntPtr v);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3s", ExactSpelling = true)]
        public static extern void WindowPos3s(GLshort x, GLshort y, GLshort z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glWindowPos3sv", ExactSpelling = true)]
        public static extern void WindowPos3sv(System.IntPtr v);

        #endregion
    }
}