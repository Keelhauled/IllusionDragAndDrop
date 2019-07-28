using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IllusionDragAndDrop.Shared
{
    public class DragDropWndProc
    {
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        WndProcDelegate newWndProc;

        public delegate void DragDropEvent(List<string> aPathNames, WinAPI.POINT aDropPoint);
        public event DragDropEvent OnDragDrop;

        public delegate void DragHoverEvent(WinAPI.POINT aHoverPoint);
        public event DragHoverEvent OnDragHover;

        IntPtr hMainWindow;
        IntPtr oldWndProcPtr;
        IntPtr newWndProcPtr;
        bool hooked = false;

        public DragDropWndProc()
        {
            hMainWindow = WinAPI.GetForegroundWindow();
        }

        public void InstallHook()
        {
            if(!hooked)
            {
                newWndProc = new WndProcDelegate(wndProc);
                newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);
                oldWndProcPtr = WinAPI.SetWindowLongPtr(hMainWindow, -4, newWndProcPtr);
                WinAPI.DragAcceptFiles(hMainWindow, true);
                hooked = true;

                Console.WriteLine("Installing hooks");
            }
        }

        public void UninstallHook()
        {
            if(hooked)
            {
                WinAPI.SetWindowLongPtr(hMainWindow, -4, oldWndProcPtr);
                hMainWindow = IntPtr.Zero;
                oldWndProcPtr = IntPtr.Zero;
                newWndProcPtr = IntPtr.Zero;
                newWndProc = null;
                hooked = false;

                Console.WriteLine("Uninstalling hooks");
            }
        }

        IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            var message = (WinAPI.WM)msg;

            if(message == WinAPI.WM.DROPFILES)
            {
                Console.WriteLine("Dropping files");

                WinAPI.DragQueryPoint(wParam, out WinAPI.POINT pos);

                // 0xFFFFFFFF as index makes the method return the number of files
                uint n = WinAPI.DragQueryFile(wParam, 0xFFFFFFFF, null, 0);
                var sb = new System.Text.StringBuilder(1024);

                List<string> result = new List<string>();
                for(uint i = 0; i < n; i++)
                {
                    int len = (int)WinAPI.DragQueryFile(wParam, i, sb, 1024);
                    result.Add(sb.ToString(0, len));
                    sb.Length = 0;
                }
                WinAPI.DragFinish(wParam);
                OnDragDrop?.Invoke(result, pos);
            }
            else if(message == WinAPI.WM.NCHITTEST)
            {
                if(WinAPI.IsKeyPushedDown(WinAPI.VK.LBUTTON) || WinAPI.IsKeyPushedDown(WinAPI.VK.RBUTTON))
                {
                    if(WinAPI.GetActiveWindow() != hMainWindow)
                    {
                        var point = new WinAPI.POINT();
                        WinAPI.GetCursorPos(ref point);
                        WinAPI.MapWindowPoints(IntPtr.Zero, hMainWindow, ref point, 1);
                        OnDragHover?.Invoke(point);
                    }
                }
            }

            return WinAPI.CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
        }
    }
}
