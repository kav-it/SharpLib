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

        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glClearColor", ExactSpelling = true)]
        public static extern void glClearColor(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha);

        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glMatrixMode", ExactSpelling = true)]
        public static extern void glMatrixMode(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glLoadIdentity", ExactSpelling = true)]
        public static extern void glLoadIdentity();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glOrtho", ExactSpelling = true)]
        public static extern void glOrtho(GLdouble left, GLdouble right, GLdouble bottom, GLdouble top, GLdouble zNear, GLdouble zFar);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glColor3f", ExactSpelling = true)]
        public static extern void glColor3f(GLfloat red, GLfloat green, GLfloat blue);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glBegin", ExactSpelling = true)]
        public static extern void glBegin(GLenum mode);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glVertex3f", ExactSpelling = true)]
        public static extern void glVertex3f(GLfloat x, GLfloat y, GLfloat z);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glEnd", ExactSpelling = true)]
        public static extern void glEnd();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFlush", ExactSpelling = true)]
        public static extern void glFlush();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glFinish", ExactSpelling = true)]
        public extern static void glFinish();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport(GL_NATIVE_LIBRARY, EntryPoint = "glGetError", ExactSpelling = true)]
        public extern static GLenum glGetError();

        #endregion
    }
}