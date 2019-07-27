using System;
using System.Runtime.InteropServices;

namespace IllusionDragAndDrop.Shared.WinAPI
{
    [ComImport(), Guid("00000122-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleDropTarget
    {
        [PreserveSig]
        int OleDragEnter(
            [In, MarshalAs(UnmanagedType.Interface)]
                object pDataObj,
            [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState,
            [In]
                POINTSTRUCT pt,
            [In, Out]
                ref int pdwEffect);

        [PreserveSig]
        int OleDragOver(
            [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState,
            [In]
                POINTSTRUCT pt,
            [In, Out]
                ref int pdwEffect);

        [PreserveSig]
        int OleDragLeave();

        [PreserveSig]
        int OleDrop(
            [In, MarshalAs(UnmanagedType.Interface)]
                object pDataObj,
            [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState,
            [In]
                POINTSTRUCT pt,
            [In, Out]
                ref int pdwEffect);
    }
}
