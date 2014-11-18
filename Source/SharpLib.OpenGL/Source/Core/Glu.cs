using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.OpenGL
{
    public static class Glu
    {
        private const string DLL_NAME = "glu32.dll";

        #region Делегаты

        public delegate void NurbsBeginCallback(int type);

        public delegate void NurbsBeginDataCallback(int type, [In] IntPtr[] userData);

        public delegate void NurbsColorCallback([In] float[] colorData);

        public delegate void NurbsColorDataCallback([In] float[] colorData, [In] IntPtr[] userData);

        public delegate void NurbsEndCallback();

        public delegate void NurbsEndDataCallback([In] IntPtr[] userData);

        public delegate void NurbsErrorCallback(int type);

        public delegate void NurbsNormalCallback([In] float[] normalData);

        public delegate void NurbsNormalDataCallback([In] float[] normalData, [In] IntPtr[] userData);

        public delegate void NurbsTexCoordCallback([In] float[] texCoord);

        public delegate void NurbsTexCoordDataCallback([In] float[] texCoord, [In] IntPtr[] userData);

        public delegate void NurbsVertexCallback([In] float[] vertexData);

        public delegate void NurbsVertexDataCallback([In] float[] vertexData, [In] IntPtr[] userData);

        public delegate void QuadricErrorCallback(int errorCode);

        public delegate void TessBeginCallback(int type);

        public delegate void TessBeginDataCallback(int type, [In] IntPtr polygonData);

        public delegate void TessCombineCallback([In] double[] coordinates, [In] IntPtr[] vertexData, [In] float[] weight, [Out] IntPtr[] outData);

        public delegate void TessCombineCallback1(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] [In] double[] coordinates,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] [In] double[] vertexData,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] [In] float[] weight,
            [Out] double[] outData);

        public delegate void TessCombineDataCallback([In] double[] coordinates, [In] IntPtr[] vertexData, [In] float[] weight, [Out] IntPtr[] outData, [In] IntPtr polygonData);

        public delegate void TessEdgeFlagCallback(int flag);

        public delegate void TessEdgeFlagDataCallback(int flag, [In] IntPtr polygonData);

        public delegate void TessEndCallback();

        public delegate void TessEndDataCallback(IntPtr polygonData);

        public delegate void TessErrorCallback(int errorCode);

        public delegate void TessErrorDataCallback(int errorCode, [In] IntPtr polygonData);

        public delegate void TessVertexCallback([In] IntPtr vertexData);

        public delegate void TessVertexCallback1([In] double[] vertexData);

        public delegate void TessVertexDataCallback([In] IntPtr vertexData, [In] IntPtr polygonData);

        #endregion

        #region Константы

        private const CallingConvention CALLING_CONVENTION = CallingConvention.Winapi;

        public const int GLU_AUTO_LOAD_MATRIX = 100200;

        public const int GLU_BEGIN = 100100;

        public const int GLU_CCW = 100121;

        public const int GLU_CULLING = 100201;

        public const int GLU_CW = 100120;

        public const int GLU_DISPLAY_MODE = 100204;

        public const int GLU_DOMAIN_DISTANCE = 100217;

        public const int GLU_EDGE_FLAG = 100104;

        public const int GLU_END = 100102;

        public const int GLU_ERROR = 100103;

        public const int GLU_EXTENSIONS = 100801;

        public const int GLU_EXTERIOR = 100123;

        public const int GLU_EXT_NURBS_TESSELLATOR = 1;

        public const int GLU_EXT_OBJECT_SPACE_TESS = 1;

        public const int GLU_FALSE = 0;

        public const int GLU_FILL = 100012;

        public const int GLU_FLAT = 100001;

        public const int GLU_INCOMPATIBLE_GL_VERSION = 100903;

        public const int GLU_INSIDE = 100021;

        public const int GLU_INTERIOR = 100122;

        public const int GLU_INVALID_ENUM = 100900;

        public const int GLU_INVALID_OPERATION = 100904;

        public const int GLU_INVALID_VALUE = 100901;

        public const int GLU_LINE = 100011;

        public const int GLU_MAP1_TRIM_2 = 100210;

        public const int GLU_MAP1_TRIM_3 = 100211;

        public const int GLU_NONE = 100002;

        public const int GLU_NURBS_BEGIN = 100164;

        public const int GLU_NURBS_BEGIN_DATA = 100170;

        public const int GLU_NURBS_BEGIN_DATA_EXT = 100170;

        public const int GLU_NURBS_BEGIN_EXT = 100164;

        public const int GLU_NURBS_COLOR = 100167;

        public const int GLU_NURBS_COLOR_DATA = 100173;

        public const int GLU_NURBS_COLOR_DATA_EXT = 100173;

        public const int GLU_NURBS_COLOR_EXT = 100167;

        public const int GLU_NURBS_END = 100169;

        public const int GLU_NURBS_END_DATA = 100175;

        public const int GLU_NURBS_END_DATA_EXT = 100175;

        public const int GLU_NURBS_END_EXT = 100169;

        public const int GLU_NURBS_ERROR = 100103;

        public const int GLU_NURBS_ERROR1 = 100251;

        public const int GLU_NURBS_ERROR10 = 100260;

        public const int GLU_NURBS_ERROR11 = 100261;

        public const int GLU_NURBS_ERROR12 = 100262;

        public const int GLU_NURBS_ERROR13 = 100263;

        public const int GLU_NURBS_ERROR14 = 100264;

        public const int GLU_NURBS_ERROR15 = 100265;

        public const int GLU_NURBS_ERROR16 = 100266;

        public const int GLU_NURBS_ERROR17 = 100267;

        public const int GLU_NURBS_ERROR18 = 100268;

        public const int GLU_NURBS_ERROR19 = 100269;

        public const int GLU_NURBS_ERROR2 = 100252;

        public const int GLU_NURBS_ERROR20 = 100270;

        public const int GLU_NURBS_ERROR21 = 100271;

        public const int GLU_NURBS_ERROR22 = 100272;

        public const int GLU_NURBS_ERROR23 = 100273;

        public const int GLU_NURBS_ERROR24 = 100274;

        public const int GLU_NURBS_ERROR25 = 100275;

        public const int GLU_NURBS_ERROR26 = 100276;

        public const int GLU_NURBS_ERROR27 = 100277;

        public const int GLU_NURBS_ERROR28 = 100278;

        public const int GLU_NURBS_ERROR29 = 100279;

        public const int GLU_NURBS_ERROR3 = 100253;

        public const int GLU_NURBS_ERROR30 = 100280;

        public const int GLU_NURBS_ERROR31 = 100281;

        public const int GLU_NURBS_ERROR32 = 100282;

        public const int GLU_NURBS_ERROR33 = 100283;

        public const int GLU_NURBS_ERROR34 = 100284;

        public const int GLU_NURBS_ERROR35 = 100285;

        public const int GLU_NURBS_ERROR36 = 100286;

        public const int GLU_NURBS_ERROR37 = 100287;

        public const int GLU_NURBS_ERROR4 = 100254;

        public const int GLU_NURBS_ERROR5 = 100255;

        public const int GLU_NURBS_ERROR6 = 100256;

        public const int GLU_NURBS_ERROR7 = 100257;

        public const int GLU_NURBS_ERROR8 = 100258;

        public const int GLU_NURBS_ERROR9 = 100259;

        public const int GLU_NURBS_MODE = 100160;

        public const int GLU_NURBS_MODE_EXT = 100160;

        public const int GLU_NURBS_NORMAL = 100166;

        public const int GLU_NURBS_NORMAL_DATA = 100172;

        public const int GLU_NURBS_NORMAL_DATA_EXT = 100172;

        public const int GLU_NURBS_NORMAL_EXT = 100166;

        public const int GLU_NURBS_RENDERER = 100162;

        public const int GLU_NURBS_RENDERER_EXT = 100162;

        public const int GLU_NURBS_TESSELLATOR = 100161;

        public const int GLU_NURBS_TESSELLATOR_EXT = 100161;

        public const int GLU_NURBS_TEXTURE_COORD = 100168;

        public const int GLU_NURBS_TEXTURE_COORD_DATA = 100174;

        public const int GLU_NURBS_TEX_COORD_DATA_EXT = 100174;

        public const int GLU_NURBS_TEX_COORD_EXT = 100168;

        public const int GLU_NURBS_VERTEX = 100165;

        public const int GLU_NURBS_VERTEX_DATA = 100171;

        public const int GLU_NURBS_VERTEX_DATA_EXT = 100171;

        public const int GLU_NURBS_VERTEX_EXT = 100165;

        public const int GLU_OBJECT_PARAMETRIC_ERROR = 100208;

        public const int GLU_OBJECT_PARAMETRIC_ERROR_EXT = 100208;

        public const int GLU_OBJECT_PATH_LENGTH = 100209;

        public const int GLU_OBJECT_PATH_LENGTH_EXT = 100209;

        public const int GLU_OUTLINE_PATCH = 100241;

        public const int GLU_OUTLINE_POLYGON = 100240;

        public const int GLU_OUTSIDE = 100020;

        public const int GLU_OUT_OF_MEMORY = 100902;

        public const int GLU_PARAMETRIC_ERROR = 100216;

        public const int GLU_PARAMETRIC_TOLERANCE = 100202;

        public const int GLU_PATH_LENGTH = 100215;

        public const int GLU_POINT = 100010;

        public const int GLU_SAMPLING_METHOD = 100205;

        public const int GLU_SAMPLING_TOLERANCE = 100203;

        public const int GLU_SILHOUETTE = 100013;

        public const int GLU_SMOOTH = 100000;

        public const int GLU_TESS_BEGIN = 100100;

        public const int GLU_TESS_BEGIN_DATA = 100106;

        public const int GLU_TESS_BOUNDARY_ONLY = 100141;

        public const int GLU_TESS_COMBINE = 100105;

        public const int GLU_TESS_COMBINE_DATA = 100111;

        public const int GLU_TESS_COORD_TOO_LARGE = GLU_TESS_ERROR5;

        public const int GLU_TESS_EDGE_FLAG = 100104;

        public const int GLU_TESS_EDGE_FLAG_DATA = 100110;

        public const int GLU_TESS_END = 100102;

        public const int GLU_TESS_END_DATA = 100108;

        public const int GLU_TESS_ERROR = 100103;

        public const int GLU_TESS_ERROR1 = 100151;

        public const int GLU_TESS_ERROR2 = 100152;

        public const int GLU_TESS_ERROR3 = 100153;

        public const int GLU_TESS_ERROR4 = 100154;

        public const int GLU_TESS_ERROR5 = 100155;

        public const int GLU_TESS_ERROR6 = 100156;

        public const int GLU_TESS_ERROR7 = 100157;

        public const int GLU_TESS_ERROR8 = 100158;

        public const int GLU_TESS_ERROR_DATA = 100109;

        public const double GLU_TESS_MAX_COORD = 1.0e150;

        public const int GLU_TESS_MISSING_BEGIN_CONTOUR = GLU_TESS_ERROR2;

        public const int GLU_TESS_MISSING_BEGIN_POLYGON = GLU_TESS_ERROR1;

        public const int GLU_TESS_MISSING_END_CONTOUR = GLU_TESS_ERROR4;

        public const int GLU_TESS_MISSING_END_POLYGON = GLU_TESS_ERROR3;

        public const int GLU_TESS_NEED_COMBINE_CALLBACK = GLU_TESS_ERROR6;

        public const int GLU_TESS_TOLERANCE = 100142;

        public const int GLU_TESS_VERTEX = 100101;

        public const int GLU_TESS_VERTEX_DATA = 100107;

        public const int GLU_TESS_WINDING_ABS_GEQ_TWO = 100134;

        public const int GLU_TESS_WINDING_NEGATIVE = 100133;

        public const int GLU_TESS_WINDING_NONZERO = 100131;

        public const int GLU_TESS_WINDING_ODD = 100130;

        public const int GLU_TESS_WINDING_POSITIVE = 100132;

        public const int GLU_TESS_WINDING_RULE = 100140;

        public const int GLU_TRUE = 1;

        public const int GLU_UNKNOWN = 100124;

        public const int GLU_U_STEP = 100206;

        public const int GLU_VERSION = 100800;

        public const bool GLU_VERSION_1_1 = true;

        public const bool GLU_VERSION_1_2 = true;

        public const bool GLU_VERSION_1_3 = true;

        public const int GLU_VERTEX = 100101;

        public const int GLU_V_STEP = 100207;

        #endregion

        #region Поля

        private static NurbsBeginCallback _nurbsBeginCallback;

        private static NurbsBeginDataCallback _nurbsBeginDataCallback;

        private static NurbsColorCallback _nurbsColorCallback;

        private static NurbsColorDataCallback _nurbsColorDataCallback;

        private static NurbsEndCallback _nurbsEndCallback;

        private static NurbsEndDataCallback _nurbsEndDataCallback;

        private static NurbsErrorCallback _nurbsErrorCallback;

        private static NurbsNormalCallback _nurbsNormalCallback;

        private static NurbsNormalDataCallback _nurbsNormalDataCallback;

        private static NurbsTexCoordCallback _nurbsTexCoordCallback;

        private static NurbsTexCoordDataCallback _nurbsTexCoordDataCallback;

        private static NurbsVertexCallback _nurbsVertexCallback;

        private static NurbsVertexDataCallback _nurbsVertexDataCallback;

        private static QuadricErrorCallback _quadricErrorCallback;

        private static TessBeginCallback _tessBeginCallback;

        private static TessBeginDataCallback _tessBeginDataCallback;

        private static TessCombineCallback _tessCombineCallback;

        private static TessCombineCallback1 _tessCombineCallback1;

        private static TessCombineDataCallback _tessCombineDataCallback;

        private static TessEdgeFlagCallback _tessEdgeFlagCallback;

        private static TessEdgeFlagDataCallback _tessEdgeFlagDataCallback;

        private static TessEndCallback _tessEndCallback;

        private static TessEndDataCallback _tessEndDataCallback;

        private static TessErrorCallback _tessErrorCallback;

        private static TessErrorDataCallback _tessErrorDataCallback;

        private static TessVertexCallback _tessVertexCallback;

        private static TessVertexCallback1 _tessVertexCallback1;

        private static TessVertexDataCallback _tessVertexDataCallback;

        #endregion

        #region Методы

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsBeginCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsBeginDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsColorCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsColorDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsEndCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsEndDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsErrorCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsNormalCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsNormalDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsTexCoordCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsTexCoordDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsVertexCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluNurbsCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsVertexDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluQuadricCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluQuadricCallback([In] GluQuadric quad, int which, [In] QuadricErrorCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessBeginCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessBeginDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessCombineCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessCombineCallback1 func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessCombineDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessEdgeFlagCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessEdgeFlagDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessEndCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessEndDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessErrorCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessErrorDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessVertexCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessVertexCallback1 func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION, EntryPoint = "gluTessCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void __gluTessCallback([In] GluTesselator tess, int which, [In] TessVertexDataCallback func);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluBeginCurve([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluBeginPolygon([In] GluTesselator tess);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluBeginSurface([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluBeginTrim([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe int gluBuild1DMipmapLevels(int target, int internalFormat, int width, int format, int type, int level, int min, int max, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe int gluBuild1DMipmaps(int target, int internalFormat, int width, int format, int type, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe int gluBuild2DMipmapLevels(int target, int internalFormat, int width, int height, int format, int type, int level, int min, int max, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe int gluBuild2DMipmaps(int target, int internalFormat, int width, int height, int format, int type, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe int gluBuild3DMipmapLevels(int target, int internalFormat, int width, int height, int depth, int format, int type, int level, int min, int max, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe int gluBuild3DMipmaps(int target, int internalFormat, int width, int height, int depth, int format, int type, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluCheckExtension(string extensionName, string extensionString);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluCylinder([In] GluQuadric quad, double baseRadius, double topRadius, double height, int slices, int stacks);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluDeleteNurbsRenderer([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluDeleteQuadric([In] GluQuadric quad);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluDeleteTess([In] GluTesselator tess);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluDisk([In] GluQuadric quad, double innerRadius, double outerRadius, int slices, int loops);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluEndCurve([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluEndPolygon([In] GluTesselator tess);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluEndSurface([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluEndTrim([In] GluNurbs nurb);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern string gluErrorString(int errorCode);

        public static string GluErrorStringWin(int errorCode)
        {
            return gluErrorUnicodeStringEXT(errorCode);
        }

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern string gluErrorUnicodeStringEXT(int errorCode);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluGetNurbsProperty([In] GluNurbs nurb, int property, [Out] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluGetNurbsProperty([In] GluNurbs nurb, int property, out float data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluGetNurbsProperty([In] GluNurbs nurb, int property, [Out] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern string gluGetString(int name);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluGetTessProperty([In] GluTesselator tess, int which, [Out] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluGetTessProperty([In] GluTesselator tess, int which, out double data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluGetTessProperty([In] GluTesselator tess, int which, [Out] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluLoadSamplingMatrices([In] GluNurbs nurb, [In] float[] modelMatrix, [In] float[] projectionMatrix, [In] int[] viewport);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluLookAt(double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ, double upX, double upY, double upZ);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern GluNurbs gluNewNurbsRenderer();

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern GluQuadric gluNewQuadric();

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern GluTesselator gluNewTess();

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNextContour([In] GluTesselator tess, int type);

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsBeginCallback func)
        {
            _nurbsBeginCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsBeginCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsBeginDataCallback func)
        {
            _nurbsBeginDataCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsBeginDataCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsColorCallback func)
        {
            _nurbsColorCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsColorCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsColorDataCallback func)
        {
            _nurbsColorDataCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsColorDataCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsEndCallback func)
        {
            _nurbsEndCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsEndCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsEndDataCallback func)
        {
            _nurbsEndDataCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsEndDataCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsErrorCallback func)
        {
            _nurbsErrorCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsErrorCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsNormalCallback func)
        {
            _nurbsNormalCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsNormalCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsNormalDataCallback func)
        {
            _nurbsNormalDataCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsNormalDataCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsTexCoordCallback func)
        {
            _nurbsTexCoordCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsTexCoordCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsTexCoordDataCallback func)
        {
            _nurbsTexCoordDataCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsTexCoordDataCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsVertexCallback func)
        {
            _nurbsVertexCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsVertexCallback);
        }

        public static void gluNurbsCallback([In] GluNurbs nurb, int which, [In] NurbsVertexDataCallback func)
        {
            _nurbsVertexDataCallback = func;
            __gluNurbsCallback(nurb, which, _nurbsVertexDataCallback);
        }

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] byte[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] byte[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] byte[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] double[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] double[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] double[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] short[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] short[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] short[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] int[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] int[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] int[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] float[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] float[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] float[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] ushort[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] ushort[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] ushort[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] uint[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] uint[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] uint[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackData([In] GluNurbs nurb, [In] IntPtr userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe void gluNurbsCallbackData([In] GluNurbs nurb, [In] void* userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] byte[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] byte[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] byte[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] double[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] double[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] double[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] short[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] short[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] short[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] int[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] int[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] int[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] float[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] float[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] float[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] ushort[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] ushort[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] ushort[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] uint[] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] uint[,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] uint[,,] userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] IntPtr userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe void gluNurbsCallbackDataEXT([In] GluNurbs nurb, [In] void* userData);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCurve([In] GluNurbs nurb, int knotCount, [In] float[] knots, int stride, [In] float[] control, int order, int type);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsCurve([In] GluNurbs nurb, int knotCount, [In] float[] knots, int stride, [In] float[,] control, int order, int type);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsProperty([In] GluNurbs nurb, int property, float val);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsSurface([In] GluNurbs nurb,
            int sKnotCount,
            [In] float[] sKnots,
            int tKnotCount,
            [In] float[] tKnots,
            int sStride,
            int tStride,
            float[] control,
            int sOrder,
            int tOrder,
            int type);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsSurface([In] GluNurbs nurb,
            int sKnotCount,
            [In] float[] sKnots,
            int tKnotCount,
            [In] float[] tKnots,
            int sStride,
            int tStride,
            float[,] control,
            int sOrder,
            int tOrder,
            int type);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluNurbsSurface([In] GluNurbs nurb,
            int sKnotCount,
            [In] float[] sKnots,
            int tKnotCount,
            [In] float[] tKnots,
            int sStride,
            int tStride,
            float[,,] control,
            int sOrder,
            int tOrder,
            int type);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluOrtho2D(double left, double right, double bottom, double top);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluPartialDisk([In] GluQuadric quad, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluPerspective(double fovY, double aspectRatio, double zNear, double zFar);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluPickMatrix(double x, double y, double width, double height, [In] int[] viewport);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluProject(double objX,
            double objY,
            double objZ,
            [In] double[] modelMatrix,
            [In] double[] projectionMatrix,
            [In] int[] viewport,
            out double winX,
            out double winY,
            out double winZ);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluPwlCurve([In] GluNurbs nurb, int count, [In] float[] data, int stride, int type);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluPwlCurve([In] GluNurbs nurb, int count, [In] float[,] data, int stride, int type);

        public static void gluQuadricCallback([In] GluQuadric quad, int which, [In] QuadricErrorCallback func)
        {
            _quadricErrorCallback = func;
            __gluQuadricCallback(quad, which, _quadricErrorCallback);
        }

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluQuadricDrawStyle([In] GluQuadric quad, int drawStyle);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluQuadricNormals([In] GluQuadric quad, int normal);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluQuadricOrientation([In] GluQuadric quad, int orientation);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluQuadricTexture([In] GluQuadric quad, int texture);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluScaleImage(int format, int widthIn, int heightIn, int typeIn, [In] IntPtr dataIn, int widthOut, int heightOut, int typeOut, [Out] IntPtr dataOut);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluScaleImage(int format, int widthIn, int heightIn, int typeIn, [In] byte[] dataIn, int widthOut, int heightOut, int typeOut, [Out] byte[] dataOut);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluSphere([In] GluQuadric quad, double radius, int slices, int stacks);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginContour([In] GluTesselator tess);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessBeginPolygon([In] GluTesselator tess, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe void gluTessBeginPolygon([In] GluTesselator tess, [In] void* data);

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessBeginCallback func)
        {
            _tessBeginCallback = func;
            __gluTessCallback(tess, which, _tessBeginCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessBeginDataCallback func)
        {
            _tessBeginDataCallback = func;
            __gluTessCallback(tess, which, _tessBeginDataCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessCombineCallback func)
        {
            _tessCombineCallback = func;
            __gluTessCallback(tess, which, _tessCombineCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessCombineCallback1 func)
        {
            _tessCombineCallback1 = func;
            __gluTessCallback(tess, which, _tessCombineCallback1);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessCombineDataCallback func)
        {
            _tessCombineDataCallback = func;
            __gluTessCallback(tess, which, _tessCombineDataCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessEdgeFlagCallback func)
        {
            _tessEdgeFlagCallback = func;
            __gluTessCallback(tess, which, _tessEdgeFlagCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessEdgeFlagDataCallback func)
        {
            _tessEdgeFlagDataCallback = func;
            __gluTessCallback(tess, which, _tessEdgeFlagDataCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessEndCallback func)
        {
            _tessEndCallback = func;
            __gluTessCallback(tess, which, _tessEndCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessEndDataCallback func)
        {
            _tessEndDataCallback = func;
            __gluTessCallback(tess, which, _tessEndDataCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessErrorCallback func)
        {
            _tessErrorCallback = func;
            __gluTessCallback(tess, which, _tessErrorCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessErrorDataCallback func)
        {
            _tessErrorDataCallback = func;
            __gluTessCallback(tess, which, _tessErrorDataCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessVertexCallback func)
        {
            _tessVertexCallback = func;
            __gluTessCallback(tess, which, _tessVertexCallback);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessVertexCallback1 func)
        {
            _tessVertexCallback1 = func;
            __gluTessCallback(tess, which, _tessVertexCallback1);
        }

        public static void gluTessCallback([In] GluTesselator tess, int which, [In] TessVertexDataCallback func)
        {
            _tessVertexDataCallback = func;
            __gluTessCallback(tess, which, _tessVertexDataCallback);
        }

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessEndContour([In] GluTesselator tess);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessEndPolygon([In] GluTesselator tess);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessNormal([In] GluTesselator tess, double x, double y, double z);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessProperty([In] GluTesselator tess, int which, double data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] byte[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] byte[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] byte[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] double[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] double[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] double[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] short[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] short[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] short[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] int[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] int[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] int[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] float[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] float[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] float[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] ushort[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] ushort[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] ushort[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] uint[] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] uint[,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] uint[,,] data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] IntPtr data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), CLSCompliant(false), SuppressUnmanagedCodeSecurity]
        public static extern unsafe void gluTessVertex([In] GluTesselator tess, [In] double[] location, [In] void* data);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluUnProject(double winX,
            double winY,
            double winZ,
            [In] double[] modelMatrix,
            [In] double[] projectionMatrix,
            [In] int[] viewport,
            out double objX,
            out double objY,
            out double objZ);

        [DllImport(DLL_NAME, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int gluUnProject4(double winX,
            double winY,
            double winZ,
            double clipW,
            [In] double[] modelMatrix,
            [In] double[] projectionMatrix,
            [In] int[] viewport,
            double nearVal,
            double farVal,
            out double objX,
            out double objY,
            out double objZ,
            out double objW);

        #endregion

        #region Вложенный класс: GluNurbs

        /// <summary>
        ///     Defines a GLU NURBS object.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GluNurbs
        {
            private readonly IntPtr Data;
        }

        #endregion

        #region Вложенный класс: GluNurbsObj

        [StructLayout(LayoutKind.Sequential)]
        public struct GluNurbsObj
        {
            private readonly IntPtr Data;
        }

        #endregion

        #region Вложенный класс: GluQuadric

        [StructLayout(LayoutKind.Sequential)]
        public struct GluQuadric
        {
            private readonly IntPtr Data;
        }

        #endregion

        #region Вложенный класс: GluQuadricObj

        [StructLayout(LayoutKind.Sequential)]
        public struct GluQuadricObj
        {
            private readonly IntPtr Data;
        }

        #endregion

        #region Вложенный класс: GluTesselator

        [StructLayout(LayoutKind.Sequential)]
        public struct GluTesselator
        {
            private readonly IntPtr Data;
        }

        #endregion

        #region Вложенный класс: GluTesselatorObj

        [StructLayout(LayoutKind.Sequential)]
        public struct GluTesselatorObj
        {
            private readonly IntPtr Data;
        }

        #endregion

        #region Вложенный класс: GluTriangulatorObj

        [StructLayout(LayoutKind.Sequential)]
        public struct GluTriangulatorObj
        {
            private readonly IntPtr Data;
        }

        #endregion
    }
}