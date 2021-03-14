using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;


namespace ViceserverModpackInstaller
{
    public static class Utilities
    {
        private static readonly string update_link = "https://api.github.com/repos/IlVice26/vicepack-installer/releases/latest";
        private static string title =
            " _     _ __    __ __\n" +
            "\\  \\  / |  \\  /  |  | Viceserver\n" +
            " \\  \\/  |   \\/   |  | Modpack\n" +
            "  \\    /|  |\\/|  |  | Installer v1.6\n" +
            "   \\__/	|__|  |__|__| Copyright 2021 - Elia Vicentini";

        public static void CheckInstallerVersion()
        {
            ShowWaitingTask.StartTask("chk-i");

            string username = Environment.UserName;
            string installer_dir = "C:\\Users\\" + username + "\\Appdata\\Roaming\\vmi";

            if (!Directory.Exists(installer_dir))
            {
                // "Cartella 'temp' non trovata. Creazione in corso"
                ShowWaitingTask.result = false;
                ShowWaitingTask.FinishTask("chk-i");

                Console.Write("\n· Creating a new environment for the installer ");
                ShowWaitingTask.StartTask("crt-i");
                
                // Creation of the new environment for the installer
                Directory.CreateDirectory(installer_dir);
                
                ShowWaitingTask.FinishTask("crt-i");
            } else {
                ShowWaitingTask.FinishTask("chk-i");
            }
        }

        public static void RedrawCmd(string stage)
        {
            // Wait 1 second to redraw the cmd
            Thread.Sleep(2000);
            Console.Clear();

            if (stage is "initial-setup")
            {
                Console.WriteLine(title);

                Console.Write("\n· Checking the modpack settings ");
                CheckInstallerVersion();

                Console.WriteLine("\n\nTask Finished");

            }
            else if (stage is "test")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("· Checking modpack installations ");
                Console.ResetColor();

                Console.Write("\nPress any key to exit .. ");
            }
        }
    }

    public static class ShowWaitingTask
    {

        public static Boolean startTask = true;
        public static Boolean result = true;
        public static string[] ch = { "\\", "|", "/", "-" };

        // This Array is formed by custom Threads created by the user
        public static List<Thread> UserThreads = new List<Thread>();
        public static List<string> UserTasks = new List<string>();

        public static void StartTask(string task)
        {
            Thread thread = new Thread(new ThreadStart(DrawWaitingTask));
            thread.Name = task;

            UserThreads.Add(thread);
            UserTasks.Add(task);

            startTask = true;
            thread.Start();
        }

        public static void FinishTask(string task)
        {
            foreach (Thread th in UserThreads)
            {
                if (th.Name.Equals(task))
                {
                    startTask = false;
                    while (th.IsAlive)
                    {

                    }
                    break;
                }
            }
        }

        public static void DrawWaitingTask()
        {

            (int sx, int col) = Console.GetCursorPosition();

            int counter = 0;
            do
            {
                if (counter == 3)
                {
                    Console.Write(ch[counter]);
                    counter = 0;
                }
                else
                {
                    Console.Write(ch[counter]);
                    counter++;
                }
                Thread.Sleep(350);
                Console.SetCursorPosition(sx, Console.CursorTop);
            } while (startTask);

            if (result)
            {
                Console.SetCursorPosition(sx, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("✓");
                Console.ResetColor();
            } else {
                Console.SetCursorPosition(sx, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("X");
                Console.ResetColor();
                result = true;
            }
        }

    }

}