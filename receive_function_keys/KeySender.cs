using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace receive_function_keys
{
    public static class KeySender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private const int INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public static void SendKey(Keys key, int holdTimeMs = 50)
        {
            // Extract modifiers
            bool shift = (key & Keys.Shift) == Keys.Shift;
            bool ctrl = (key & Keys.Control) == Keys.Control;
            bool alt = (key & Keys.Alt) == Keys.Alt;

            // Extract base key (remove modifiers)
            Keys baseKey = key & ~Keys.Modifiers;

            List<INPUT> inputs = new List<INPUT>();

            // Modifiers Down
            if (shift) AddKeyInput(inputs, Keys.ShiftKey, false);
            if (ctrl) AddKeyInput(inputs, Keys.ControlKey, false);
            if (alt) AddKeyInput(inputs, Keys.Menu, false);

            // Base Key Down
            if (baseKey != Keys.None) AddKeyInput(inputs, baseKey, false);

            // Send Down Events
            if (inputs.Count > 0)
            {
                SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
            }

            // Wait
            if (holdTimeMs > 0)
            {
                System.Threading.Thread.Sleep(holdTimeMs);
            }

            inputs.Clear();

            // Base Key Up
            if (baseKey != Keys.None) AddKeyInput(inputs, baseKey, true);

            // Modifiers Up
            if (alt) AddKeyInput(inputs, Keys.Menu, true);
            if (ctrl) AddKeyInput(inputs, Keys.ControlKey, true);
            if (shift) AddKeyInput(inputs, Keys.ShiftKey, true);

            // Send Up Events
            if (inputs.Count > 0)
            {
                SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
            }
        }

        private static void AddKeyInput(List<INPUT> inputs, Keys k, bool up)
        {
            INPUT input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.u = new InputUnion();
            input.u.ki = new KEYBDINPUT();
            input.u.ki.wVk = (ushort)k;
            input.u.ki.dwFlags = up ? KEYEVENTF_KEYUP : 0;

            if (IsExtendedKey(k))
            {
                input.u.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
            }

            inputs.Add(input);
        }

        private static bool IsExtendedKey(Keys k)
        {
            return k == Keys.Up || k == Keys.Down || k == Keys.Left || k == Keys.Right ||
                   k == Keys.Insert || k == Keys.Delete || k == Keys.Home || k == Keys.End ||
                   k == Keys.Prior || k == Keys.Next ||
                   k == Keys.NumLock || k == Keys.PrintScreen || k == Keys.Divide ||
                   k == Keys.RControlKey || k == Keys.RMenu;
        }
    }
}
