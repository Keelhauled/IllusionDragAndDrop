using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IllusionDragAndDrop.Shared
{
    public class DragDropWndProc
    {
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam);
        WndProcDelegate newWndProc;

        public delegate void DragDropEvent(List<string> aPathNames, WinAPI.POINT aDropPoint);
        public event DragDropEvent OnDragDrop;

        public delegate void DragHoverEvent(WinAPI.POINT aHoverPoint);
        public event DragHoverEvent OnDragHover;

        IntPtr hMainWindow;
        IntPtr oldWndProcPtr;
        IntPtr newWndProcPtr;
        bool hooked = false;

        IntPtr mouseHook;
        WinAPI.LowLevelMouseProc mouseCallback;
        bool mouseDown = false;
        IntPtr activeDragWindow;

        public DragDropWndProc()
        {
            hMainWindow = WinAPI.GetForegroundWindow();
        }

        public void InstallHook()
        {
            if(!hooked)
            {
                newWndProc = new WndProcDelegate(WndProc);
                newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);
                oldWndProcPtr = WinAPI.SetWindowLongPtr(hMainWindow, -4, newWndProcPtr);
                WinAPI.DragAcceptFiles(hMainWindow, true);

                mouseCallback = MouseCallback;
                var hModule = WinAPI.GetModuleHandle(null);
                mouseHook = WinAPI.SetWindowsHookEx(WinAPI.HookType.WH_MOUSE_LL, mouseCallback, hModule, 0);
                Console.WriteLine($"Mouse hook = {mouseHook}");

                Console.WriteLine("Installing hooks");
                hooked = true;
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

                WinAPI.UnhookWindowsHookEx(mouseHook);
                mouseHook = IntPtr.Zero;

                Console.WriteLine("Uninstalling hooks");
                hooked = false;
            }
        }

        IntPtr WndProc(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam)
        {
            if(msg == WinAPI.WM.DROPFILES)
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
            else if(msg == WinAPI.WM.NCHITTEST)
            {
                if(mouseDown && hMainWindow != activeDragWindow)
                {
                    var point = new WinAPI.POINT();
                    WinAPI.GetCursorPos(ref point);
                    WinAPI.MapWindowPoints(IntPtr.Zero, hMainWindow, ref point, 1);
                    OnDragHover?.Invoke(point);
                    Console.WriteLine(point);
                }
            }

            return WinAPI.CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
        }

        IntPtr MouseCallback(int nCode, WinAPI.WM wParam, IntPtr lParam)
        {
            //if(wParam != WinAPI.WM.MOUSEFIRST)
            //{
            //    if(wParam == WinAPI.WM.LBUTTONDOWN)
            //    {
            //        activeDragWindow = WinAPI.GetActiveWindow();
            //        mouseDown = true;
            //        Console.WriteLine("LBUTTONDOWN");
            //    }
            //    else if(wParam == WinAPI.WM.LBUTTONUP)
            //    {
            //        activeDragWindow = IntPtr.Zero;
            //        mouseDown = false;
            //        Console.WriteLine("LBUTTONUP");
            //    } 
            //}

            return WinAPI.CallNextHookEx(mouseHook, nCode, wParam, lParam);
        }
    }
}
