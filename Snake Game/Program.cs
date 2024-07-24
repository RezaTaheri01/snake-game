using System;
using System.Threading;
using System.Runtime.InteropServices; // related to ConsoleHelper

namespace Snake_Game
{
    class Program
    {
        // Speed and Win score
        static int easy = 65;
        static int medium = 35;
        static int hard = 25;
        static int maxSnakeLen = 200; // The Snake Array Length
        static int winLen = 100; // lower than maxSnakeLen (score = 85000)
        // Window size
        static int win_w = 68; // even (most be greater than 67)
        static int win_h = 24; // even (most be greater than 23)
        static short fontSize = 20;
        // Colors
        static ConsoleColor bg = ConsoleColor.DarkCyan;
        static ConsoleColor snakeHeadC = ConsoleColor.DarkYellow;
        static ConsoleColor snakeBodyC = ConsoleColor.Yellow;
        static ConsoleColor titleC = ConsoleColor.DarkMagenta;
        static ConsoleColor textC = ConsoleColor.White;
        static ConsoleColor blockC = ConsoleColor.Black;
        static ConsoleColor scoreC = ConsoleColor.Yellow;
        static ConsoleColor hookC = ConsoleColor.Yellow;

        // Others (fix ones)
        static int speed = easy; // base on ms
        static int direction = 0; // Right 0, Down 1, Left 2, Up 3
        static bool gameOver = false;
        static int snakeLenTmp = 0;
        static int snakeLen = 0;
        static int score = 0; // Calculate base on length
        static int[] snakeX = new int[maxSnakeLen];
        static int[] snakeY = new int[maxSnakeLen];
        static int tmp = 0;
        static int hookX, hookY;

        static void Main(string[] args)
        {
            Play();
        }
        
        static void Init()
        {
            gameOver = false;
            direction = 0;
            score = 0;
            snakeLenTmp = 0;
            Array.Clear(snakeX, 0, snakeX.Length);
            Array.Clear(snakeY, 0, snakeY.Length);

            Console.SetBufferSize(win_w, win_h);
            Console.SetWindowSize(win_w, win_h);
            ConsoleHelper.SetCurrentFont("Consolas", fontSize);
            Console.CursorVisible = false;
            Console.BackgroundColor = bg;
            Console.Clear();

            snakeLen = 15;
            int x = 20; int y = 3;
            for (int i = 0; i < snakeLen; i++)
            {
                snakeX[i] = x; snakeY[i] = y;
                x = x - 1;
            }
        }

        static void Menu()
        {
            {
                int x_pos = (win_w / 2); // - base on title len
                int arrow_pos = x_pos - 6;

                int y_pos_start = (win_h / 2) - 6;

                int startY = y_pos_start + 2; // arrow
                int lastY = y_pos_start + 12; // arrow

                int Y = y_pos_start + 2; // arrow start variable

                Console.SetCursorPosition(x_pos - 6, y_pos_start);
                Console.ForegroundColor = titleC;
                Console.WriteLine("SNEAKY SNAKE");
                Console.ForegroundColor = textC;
                Console.SetCursorPosition(x_pos - 2, y_pos_start + 2);
                Console.Write("Easy");
                Console.SetCursorPosition(x_pos - 3, y_pos_start + 4);
                Console.Write("Medium");
                Console.SetCursorPosition(x_pos - 2, y_pos_start + 6);
                Console.Write("Hard");
                Console.SetCursorPosition(x_pos - 3, y_pos_start + 8);
                Console.Write("Custom");
                Console.SetCursorPosition(x_pos - 2, y_pos_start + 10);
                Console.Write("Redo");
                Console.SetCursorPosition(x_pos - 2, y_pos_start + 12);
                Console.Write("Exit");
                Console.SetCursorPosition(arrow_pos, Y);
                Console.Write(">");

                while (true)
                {
                    Console.SetCursorPosition(arrow_pos, Y);
                    Console.Write(">");
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (Y != lastY)
                        {
                            Console.SetCursorPosition(arrow_pos, Y);
                            Console.Write("  ");
                            Y += 2;
                        }
                    }
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (Y != startY)
                        {
                            Console.SetCursorPosition(arrow_pos, Y);
                            Console.Write("  ");
                            Y -= 2;
                        }
                    }
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (Y == y_pos_start + 2)
                        {
                            speed = easy;
                        }
                        else if (Y == y_pos_start + 4)
                        {
                            speed = medium;
                        }
                        else if (Y == y_pos_start + 6)
                        {
                            speed = hard;
                        }
                        else if (Y == y_pos_start + 8)
                        {
                            Console.SetCursorPosition(x_pos + 6, y_pos_start + 8);
                            Console.CursorVisible = true;
                            Console.Write("Speed : ");
                            int a = medium;
                            try
                            {
                                a = int.Parse(Console.ReadLine());
                            }
                            catch
                            {
                                a = medium;
                            }
                            if (a > 15 && a < 75)
                            {
                                speed = a;
                            }
                            else
                            {
                                speed = medium;
                            }
                            Console.CursorVisible = false;
                        }
                        else if (Y == y_pos_start + 10)
                        {
                            Play();
                        }
                        else if (Y == y_pos_start + 12)
                        {
                            Exit();
                        }
                        break;
                    }
                }
                Console.Clear();
                Block();
            }
        }

        static void Play()
        {
            Init();
            Menu();
            Block(); // last line of console contain a bug 
            Createhook(true);
            while (true)
            {
                ShowScore();
                DrawSnake();
                DrawHook();
                if (score == (winLen - 15) * 1000) // 15 is the defualt len
                {
                    EndMessage("You Won :)");
                }
                Thread.Sleep(speed);
                EraseTail();
                CheckHook();
                SnakeMove();
                ChangeDirection();
                GameOverCheck();
            }
        }

        static void Block()
        {
            for (int ip = 0; ip < win_w; ip++)
            {
                Console.SetCursorPosition(ip, win_h - 1);
                Console.ForegroundColor = blockC;
                Console.BackgroundColor = blockC;
                Console.Write('.');
            }
            for (int ip = 0; ip < win_w; ip++)
            {
                Console.SetCursorPosition(ip, win_h - 2);
                Console.ForegroundColor = bg;
                Console.BackgroundColor = bg;
                Console.Write('.');
            }
        }

        static void DrawSnake()
        {
            for (int i = 0; i < snakeLen; i++)
            {
                Console.SetCursorPosition(snakeX[i], snakeY[i]);
                if (i == 0)
                {
                    Console.ForegroundColor = snakeHeadC;
                    Console.BackgroundColor = snakeHeadC;
                    Console.Write("+");
                    Console.ForegroundColor = bg;
                    Console.BackgroundColor = bg;

                }
                else
                {
                    Console.ForegroundColor = snakeBodyC;
                    Console.BackgroundColor = snakeBodyC;
                    Console.Write("*");
                    Console.ForegroundColor = bg;
                    Console.BackgroundColor = bg;

                }
            }
        }

        static void EraseTail()
        {
            tmp = snakeLen - 1;
            Console.SetCursorPosition(snakeX[tmp], snakeY[tmp]);
            Console.BackgroundColor = bg;
            Console.ForegroundColor = bg;
            Console.Write(" ");
        }

        static void SnakeMove()
        {
            for (int i = snakeLen - 1; i > 0; i­­--)
            {
                snakeX[i] = snakeX[i - 1];
                snakeY[i] = snakeY[i - 1];
            }

            // right
            if (direction == 0)
            {
                if (snakeX[0] < win_w - 1)
                {
                    snakeX[0] = snakeX[0] + 1;
                }
                else
                    snakeX[0] = 0;
            }
            // left
            if (direction == 2)
            {
                if (snakeX[0] > 0)
                {
                    snakeX[0] = snakeX[0] - 1;
                }

                else
                    snakeX[0] = win_w - 1;
            }
            // up           
            if (direction == 3)
            {
                if (snakeY[0] > 0)
                    snakeY[0] = snakeY[0] - 1;
                else
                    snakeY[0] = win_h - 2;
            }
            // down
            if (direction == 1)
            {
                if (snakeY[0] < win_h - 2)
                    snakeY[0] = snakeY[0] + 1;

                else
                    snakeY[0] = 0;
            }
        }

        static void ChangeDirection()
        {
            Console.SetCursorPosition(0, win_h - 1);
            Console.ForegroundColor = blockC;
            Console.BackgroundColor = blockC;
            Console.Write('.');

            if (Console.KeyAvailable == false) return;
            ConsoleKeyInfo keyinfo = Console.ReadKey();

            if (keyinfo.Key == ConsoleKey.UpArrow && direction != 1)
            {
                direction = 3;
            }

            else if (keyinfo.Key == ConsoleKey.DownArrow && direction != 3)
            {
                direction = 1;
            }

            else if (keyinfo.Key == ConsoleKey.RightArrow && direction != 2)
            {
                direction = 0;
            }

            else if (keyinfo.Key == ConsoleKey.LeftArrow && direction != 0)
            {
                direction = 2;
            }
            else if (keyinfo.Key == ConsoleKey.Escape)
            {
                Console.ForegroundColor = bg;
                Console.BackgroundColor = bg;
                Menu();
            }
        }

        static void GameOverCheck()
        {
            snakeLenTmp = snakeLen;
            for (int i = 1; i < snakeLen; i++)
            {
                if (snakeX[0] == snakeX[i] && snakeY[0] == snakeY[i])
                {
                    gameOver = true;
                }
            }
            if (gameOver == true)
            {
                EndMessage("GameOver");
            }
        }

        static void EndMessage(string msg)
        {
            int length = msg.Length;
            Console.ForegroundColor = textC;
            Console.BackgroundColor = bg;
            Console.SetCursorPosition(win_w / 2 - (length / 2), win_h / 2 - 2);
            Console.WriteLine(msg);
            Console.SetCursorPosition(win_w / 2 - 15, win_h / 2 + 2);
            Console.WriteLine("Do you want to play again? y/n");
            //Console.SetCursorPosition(win_w / 2 + 16, win_h / 2 + 2);
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (keyInfo.KeyChar == 'y' || keyInfo.KeyChar == 'Y')
            {
                Console.Clear();
                Play();
            }
            else if (keyInfo.KeyChar == 'n' || keyInfo.KeyChar == 'N')
            {
                Exit();
            }
        }

        static void ShowScore()
        {
            Console.SetCursorPosition(win_w/2 - 2, 0);
            Console.ForegroundColor = scoreC;
            Console.BackgroundColor = bg;
            Console.Write(score);
        }

        static void Createhook(bool first)
        {
            if (first)
            {
                hookX = 65;
                hookY = 3;
            }
            else
            {
                Random rand = new Random();
                hookX = rand.Next(0, win_w - 1);
                hookY = rand.Next(1, win_h - 2);
                for (int i = 0; i < snakeLen; i++)
                {
                    if (snakeX[i] == hookX && snakeY[i] == hookY)
                    {
                        Createhook(false);
                    }
                }
            }
        }

        static void DrawHook()
        {
            Console.SetCursorPosition(hookX, hookY);
            Console.BackgroundColor = bg;
            Console.ForegroundColor = hookC;
            Console.CursorVisible = false;
            Console.Write('$');
            Console.ForegroundColor = bg;
            Console.BackgroundColor = bg;
        }

        static void CheckHook()
        {
            for (int i = 0; i < snakeLen; i++)
            {
                if (snakeX[0] == hookX && snakeY[0] == hookY)
                {
                    score += 1000;
                    if (score % 10000 == 0 && speed > 1)
                    {
                        speed -= 2; // for draw and move balance
                    }
                    snakeLen+=1;
                    Createhook(false);
                    break;
                }
            }

        }

        static void Exit()
        {
            int xPos = win_w / 2;
            int yPos = win_h / 2;
            Console.Clear();
            Console.ForegroundColor = textC;
            Console.SetCursorPosition(xPos - 5, yPos); // len("Exiting...") = 10 => 10 / 2 = 5
            Console.WriteLine("Exiting.");
            Thread.Sleep(500);
            Console.SetCursorPosition(xPos - 5, yPos);
            Console.WriteLine("Exiting..");
            Thread.Sleep(500);
            Console.SetCursorPosition(xPos - 5, yPos);
            Console.WriteLine("Exiting...");
            Thread.Sleep(500);
            System.Environment.Exit(0);
        }
    }

    // to change console font size
    public static class ConsoleHelper
    {
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);


        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo
        {
            internal int cbSize;
            internal int FontIndex;
            internal short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
            public string FontName;
        }

        public static FontInfo[] SetCurrentFont(string font, short fontSize = 0)
        {
            Console.WriteLine("Set Current Font: " + font);

            FontInfo before = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };

            if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
            {

                FontInfo set = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };

                // Get some settings from current font.
                if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
                {
                    var ex = Marshal.GetLastWin32Error();
                    Console.WriteLine("Set error " + ex);
                    throw new System.ComponentModel.Win32Exception(ex);
                }

                FontInfo after = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>()
                };
                GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

                return new[] { before, set, after };
            }
            else
            {
                var er = Marshal.GetLastWin32Error();
                Console.WriteLine("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }
    }
}
