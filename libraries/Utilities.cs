using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using Octokit;


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

        public static void CheckInstallerEnvironment()
        {
            ShowWaitingTask.StartTask("chk-e");

            if (!File.Exists(DataManager.settings_info["general"]["installer_config"].ToString()))
            {
                // "Cartella 'temp' non trovata. Creazione in corso"
                ShowWaitingTask.UserTasks["chk-i"]["result"] = false;
                ShowWaitingTask.FinishTask("chk-e");

                Console.Write("\n·    Creating a new environment for the installer ");
                ShowWaitingTask.StartTask("crt-e");

                // Creation of the new environment for the installer
                DataManager.CreateInstallerConfig();

                ShowWaitingTask.FinishTask("crt-e");
            }

            ShowWaitingTask.FinishTask("chk-e");
        }

        private static void CheckInstallerVersion()
        {
            ShowWaitingTask.StartTask("chk-v");

            /*
            
            GitHubClient client = new GitHubClient(new ProductHeaderValue("IlVice26"));
            List<Release> releases = (List<Release>) client.Repository.Release.GetAll("IlVice26", "vicepack-installer").ToList();

            Version latestGitHubVersion = new Version(releases[0].TagName);
            Version localVersion = new Version(Program.VERSION);

            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            if (versionComparison < 0)
            {
                //The version on GitHub is more up to date than this local release.
            }
            else if (versionComparison > 0)
            {
                //This local version is greater than the release version on GitHub.
            }
            else
            {
                //This local Version and the Version on GitHub are equal.
            }

            */

            ShowWaitingTask.FinishTask("chk-v");

            // Console.WriteLine(latestGitHubVersion.ToString());
        }

        public static void RedrawCmd(string stage)
        {
            // Wait 2 second to redraw the cmd
            Thread.Sleep(2000);
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }

            if (stage is "initial-setup")
            {
                Console.WriteLine(title);

                Console.Write("\n·    Checking modpack environment");
                CheckInstallerEnvironment();
            }
            else if (stage is "check-version")
            {
                Console.WriteLine(title);

                Console.Write("\n·    Checking modpack version");
                CheckInstallerVersion();
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
            if (!Console.IsOutputRedirected)
            {
                Thread thread = new Thread(() => DrawWaitingTask(task));
                thread.Name = task;

                UserThreads.Add(thread);
                UserTasks[task] = new Dictionary<string, Boolean>();

                UserTasks[task]["startTask"] = true;
                UserTasks[task]["result"] = true;

                thread.Start();
            }
        }

        public static void FinishTask(string task)
        {
            if (!Console.IsOutputRedirected)
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
        }

        public static void DrawWaitingTask(string task)
        {

            (int sx, int col) = Console.GetCursorPosition();

            int counter = 0;
            do
            {
                Console.SetCursorPosition(2, col);
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
