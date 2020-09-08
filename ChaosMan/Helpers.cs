using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ChaosMan
{
    public static class Helpers
    {
        public static int ReadSettingInt(string name, int defaultValue)
        {
            var value = defaultValue;
            var key = Registry.CurrentUser.OpenSubKey("Software");

            key = key?.OpenSubKey("ChaosMan");

            if (key != null)
            {
                value = (int)key.GetValue(name, defaultValue);

                key.Close();
            }

            return value;
        }

        public static void WriteSettingInt(string name, int value)
        {
            var softwareKey = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey key = null;

            if (softwareKey != null)
            {
                key = softwareKey.OpenSubKey("ChaosMan", true) ?? softwareKey.CreateSubKey("ChaosMan");
            }

            if (key != null)
            {
                key.SetValue(name, value);

                key.Close();
            }

            softwareKey?.Close();
        }
    }

    internal class WinAPI
    {
        [DllImport("User32.dll")]
        public static extern IntPtr SetParent(IntPtr hChild, IntPtr hNewParent);

        [DllImport("User32.dll")]
        public static extern Int32 GetClientRect(IntPtr hWnd, out Rectangle rect);

        [DllImport("User32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        [DllImport("User32.dll")]
        public static extern Int32 GetWindowLong(IntPtr hWnd, int nIndex);

        public const int
            GWL_STYLE = (-16),
            WS_CHILD = 0x40000000;
    }
}
