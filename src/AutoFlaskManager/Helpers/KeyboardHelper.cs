using PoeHUD.Controllers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FlaskManager
{
    class KeyboardHelper
    {
        private readonly GameController gameHandle;
        private float CurLatency;

        public KeyboardHelper(GameController g)
        {
            gameHandle = g;
        }

        public void setLatency(float latency)
        {
            CurLatency = latency;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, UIntPtr lParam);
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        public void KeyDown(Keys Key)
        {
            SendMessage(gameHandle.Window.Process.MainWindowHandle, 0x100, (int)Key, 0);
        }
        public void KeyUp(Keys Key)
        {
            SendMessage(gameHandle.Window.Process.MainWindowHandle, 0x101, (int)Key, 0);
        }
        public bool KeyPressRelease(Keys key)
        {
            KeyDown(key);
            int lat = (int)(CurLatency);
            if (lat < 1000)
            {
                Thread.Sleep((int)(lat));
                return true;
            }
            else
            {
                Thread.Sleep(1000);
                return false;
            }
            // working as a double key.
            //KeyUp(key);
        }
        private void Write(string text, params object[] args)
        {
            foreach (var character in string.Format(text, args))
            {
                PostMessage(gameHandle.Window.Process.MainWindowHandle, 0x0102, new UIntPtr(character), UIntPtr.Zero);
            }
        }
    }
}
