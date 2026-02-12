using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing; 

class Program
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out Point lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    public struct Point { public int X; public int Y; }

    private const int MAXIMIZE = 3;
    static bool isLocked = true;

    static void Main()
    {
        Console.Title = "КУРСОР В КЛЕТКЕ - УРОВЕНЬ БЕЗУМИЯ";
        ShowWindow(GetConsoleWindow(), MAXIMIZE);

        Thread magnetThread = new Thread(CursorMagnet);
        magnetThread.Start();

        string captcha = GenerateGodzillaCaptcha(200);
        string input = "";

        while (input != captcha)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("КЛЕТКА АКТИВИРОВАНА. ВЫХОДА НЕТ.");
            Console.WriteLine("ВВЕДИТЕ ИЛИ СКОПИРУЙТЕ 200 СИМВОЛОВ, ПОКА КУРСОР ТЯНЕТ В БЕЗДНУ:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + captcha + "\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ВВОД: ");
            input = Console.ReadLine();

            if (input != captcha)
            {
                captcha = GenerateGodzillaCaptcha(200); 
                Console.Beep(300, 500);
            }
        }

        isLocked = false;
        Console.WriteLine("СИСТЕМА РАЗБЛОКИРОВАНА.");
    }

    static void CursorMagnet()
    {

        int centerX = 960;
        int centerY = 540;

        while (isLocked)
        {
            Point currentPos;
            GetCursorPos(out currentPos);

            int deltaX = centerX - currentPos.X;
            int deltaY = centerY - currentPos.Y;


            if (Math.Abs(deltaX) > 50 || Math.Abs(deltaY) > 50)
            {
       
                int stepX = currentPos.X + (deltaX > 0 ? 5 : -5);
                int stepY = currentPos.Y + (deltaY > 0 ? 5 : -5);
                SetCursorPos(stepX, stepY);
            }

            Thread.Sleep(10);
        }
    }

    static string GenerateGodzillaCaptcha(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?/`~";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}