﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace uWidgets.Windows.Services;

public static class WindowExtensions
{
    public static void DisableSnapping(this Window window)
    {
        const int WM_SYSCOMMAND = 0x112;
        const int SC_MOVE = 0xF010;
        const int WM_MOUSELEAVE = 0x2A2;

        var source = PresentationSource.FromVisual(window) as HwndSource;
        source?.AddHook(DisableSnappingHook);

        IntPtr DisableSnappingHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND)
            {
                if ((wParam.ToInt32() & ~0x0F) == SC_MOVE)
                    window.ResizeMode = ResizeMode.NoResize;
            }
            else if (msg == WM_MOUSELEAVE)
            {
                window.ResizeMode = ResizeMode.CanResize;
            }

            return IntPtr.Zero;
        }
    }

    public static void ResizeEnd(this Window window, Action action)
    {
        const int WM_EXITSIZEMOVE = 0x0232;
        
        var source = PresentationSource.FromVisual(window) as HwndSource;
        source?.AddHook(ResizeEndHook);

        IntPtr ResizeEndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_EXITSIZEMOVE)
            {
                action();
            }
            return IntPtr.Zero;
        }
    }
    
    public static void RemoveFromAltTab(this Window window)
    {
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int GWL_EXSTYLE = -20;

        var handle = new WindowInteropHelper(window).Handle;

        var exStyle = (int)GetWindowLong(handle, GWL_EXSTYLE);

        exStyle |= WS_EX_TOOLWINDOW;
        SetWindowLong(handle, GWL_EXSTYLE, (IntPtr)exStyle);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    private static void SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        int error;
        IntPtr result;

        SetLastError(0);

        if (IntPtr.Size == 4)
        {
            var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
            error = Marshal.GetLastWin32Error();
            result = new IntPtr(tempResult);
        }
        else
        {
            result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
            error = Marshal.GetLastWin32Error();
        }

        if (result == IntPtr.Zero && error != 0) throw new Win32Exception(error);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private static int IntPtrToInt32(IntPtr intPtr)
    {
        return unchecked((int)intPtr.ToInt64());
    }

    [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
    private static extern void SetLastError(int dwErrorCode);
}