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
            "___    ____    ______\n" +
            "\\  \\  / |  \\  /  |  | Viceserver\n" +
            " \\  \\/  |   \\/   |  | Modpack\n" +
            "  \\    /|  |\\/|  |  | Installer " + Program.VERSION + "\n" +
            "   \\__/	|__|  |__|__| Copyright 2021 - Elia Vicentini";

        public static void CheckInstallerVersion()
        {
            ShowWaitingTask.StartTask("chk-i");

            if (!File.Exists(DataManager.settings_info["general"]["installer_config"].ToString()))
            {
                // "Cartella 'temp' non trovata. Creazione in corso"
                ShowWaitingTask.UserTasks["chk-i"]["result"] = false;
                ShowWaitingTask.FinishTask("chk-i");

                Console.Write("\n·    Creating a new environment for the installer ");
                ShowWaitingTask.StartTask("crt-i");

                // Creation of the new environment for the installer
                DataManager.CreateInstallerConfig();

                ShowWaitingTask.FinishTask("crt-i");
            }
            else
            {
                ShowWaitingTask.FinishTask("chk-i");
            }
        }

        public static void RedrawCmd(string stage)
        {
            // Wait 1 second to redraw the cmd
            Thread.Sleep(2000);
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }

            if (stage is "initial-setup")
            {
                Console.WriteLine(title);

                Console.Write("\n·    Checking the modpack settings ");
                CheckInstallerVersion();
                
            }
            else if (stage is "test")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("·    Checking modpack installations ");
                Console.ResetColor();

                Console.Write("\nPress any key to exit .. ");
            }
        }
    }

    public static class ShowWaitingTask
    {
        public static string[] ch = { "\\", "|", "/", "-" };

        // This Array is formed by custom Threads created by the user
        public static List<Thread> UserThreads = new List<Thread>();
        public static Dictionary<string, Dictionary<string, Boolean>> UserTasks = new Dictionary<string, Dictionary<string, Boolean>>();

        public static void StartTask(string task)
        {
            Thread thread = new Thread(() => DrawWaitingTask(task));
            thread.Name = task;

            UserThreads.Add(thread);
            UserTasks[task] = new Dictionary<string, Boolean>();
            
            UserTasks[task]["startTask"] = true;
            UserTasks[task]["result"] = true;

            thread.Start();
        }

        public static void FinishTask(string task)
        {
            foreach (Thread th in UserThreads)
            {
                if (th.Name.Equals(task))
                {
                    UserTasks[task]["startTask"] = false;
                    while (th.IsAlive)
                    {
                        
                    }
                    break;
                }
            }
        }

        public static void DrawWaitingTask(string task)
        {

            (int sx, int col) = Console.GetCursorPosition();

            int counter = 0;
            do
            {
                Console.SetCursorPosition(2, Console.CursorTop);
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
                
            } while (UserTasks[task]["startTask"]);

            if (UserTasks[task]["result"])
            {
                Console.SetCursorPosition(2, col);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("✓");
                Console.ResetColor();
            }
            else
            {
                Console.SetCursorPosition(2, col);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("X");
                Console.ResetColor();
                UserTasks[task]["result"] = true;
            }
        }

    }

}
