using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SharpLib.Wpf.Dialogs.Interop
{
    // Dummy base interface for CommonFileDialog coclasses
    internal interface NativeCommonFileDialog
    { }

    // ---------------------------------------------------------
    // Coclass interfaces - designed to "look like" the object 
    // in the API, so that the 'new' operator can be used in a 
    // straightforward way. Behind the scenes, the C# compiler
    // morphs all 'new CoClass()' calls to 'new CoClassWrapper()'
    [ComImport,
    Guid(IidGuid.IFileOpenDialog), 
    CoClass(typeof(FileOpenDialogRCW))]
    internal interface NativeFileOpenDialog : IFileOpenDialog
    {
    }

    [ComImport,
    Guid(IidGuid.IFileSaveDialog),
    CoClass(typeof(FileSaveDialogRCW))]
    internal interface NativeFileSaveDialog : IFileSaveDialog
    {
    }

    [ComImport,
    Guid(IidGuid.IKnownFolderManager),
    CoClass(typeof(KnownFolderManagerRCW))]
    internal interface KnownFolderManager : IKnownFolderManager
    {
    }

    // ---------------------------------------------------
    // .NET classes representing runtime callable wrappers
    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(ClsidGuid.FileOpenDialog)]
    internal class FileOpenDialogRCW
    {
    }

    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(ClsidGuid.FileSaveDialog)]
    internal class FileSaveDialogRCW
    {
    }

    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(ClsidGuid.KnownFolderManager)]
    internal class KnownFolderManagerRCW
    {
    }


    // TODO: make these available (we'll need them when passing in 
    // shell items to the CFD API
    //[ComImport,
    //Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"),
    //CoClass(typeof(ShellItemClass))]
    //internal interface ShellItem : IShellItem
    //{
    //}

    //// NOTE: This GUID is for CLSID_ShellItem, which
    //// actually implements IShellItem2, which has lots of 
    //// stuff we don't need
    //[ComImport,
    //ClassInterface(ClassInterfaceType.None),
    //TypeLibType(TypeLibTypeFlags.FCanCreate)]
    //internal class ShellItemClass
    //{
    //}
}
