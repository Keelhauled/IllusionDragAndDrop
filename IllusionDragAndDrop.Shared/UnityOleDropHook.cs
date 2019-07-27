using IllusionDragAndDrop.Shared.WinAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IllusionDragAndDrop.Shared
{
    public class UnityOleDropHook : IDropTarget
    {
        public delegate void DroppedFilesEvent(List<string> aPathNames, POINT aDropPoint);
        public event DroppedFilesEvent OnDroppedFiles;

        private uint threadId = NativeMethods.GetCurrentThreadId();
        private IntPtr mainWindow;

        public UnityOleDropHook()
        {
            if(threadId > 0)
                mainWindow = GetMainWindow(threadId, "UnityWndClass");
        }

        public static IntPtr GetMainWindow(uint aThreadId, string aClassName = null)
        {
            IntPtr win = IntPtr.Zero;
            NativeMethods.EnumThreadWindows(aThreadId, (W, _) =>
            {
                if(NativeMethods.IsWindowVisible(W) && (win == null || (aClassName != null && NativeMethods.GetClassName(W) == aClassName)))
                    win = W;
                return true;
            }, IntPtr.Zero);
            return win;
        }

        public void RegisterDragDrop()
        {
            NativeMethods.OleInitialize(IntPtr.Zero);
            int n = NativeMethods.RegisterDragDrop(mainWindow, new DropTarget(this));
            if(n != 0 && n != NativeConstants.DRAGDROP_E_ALREADYREGISTERED)
                throw new Win32Exception(n);
        }

        public void RevokeDragDrop()
        {
            int n = NativeMethods.RevokeDragDrop(mainWindow);
            if(n != 0 && n != NativeConstants.DRAGDROP_E_NOTREGISTERED)
                throw new Win32Exception(n);
            NativeMethods.OleUninitialize();
        }

        public void OnDragEnter(DragEventArgs e)
        {
            Console.WriteLine(nameof(OnDragEnter));
        }

        public void OnDragLeave(EventArgs e)
        {
            Console.WriteLine(nameof(OnDragLeave));
        }

        public void OnDragDrop(DragEventArgs e)
        {
            Console.WriteLine(nameof(OnDragDrop));
        }

        public void OnDragOver(DragEventArgs e)
        {
            Console.WriteLine(nameof(OnDragOver));
        }
    }
}
