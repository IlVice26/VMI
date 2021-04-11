using System;
using System.Runtime;

namespace ViceserverModpackInstaller
{
    class Program
    {

        public static readonly string VERSION = "v1.6";

        public static void Main()
        {
            /**
            Tasks of Viceserver Modpack Installer
            - 1) It checks the version of the installer, if it's outdated, it runs the updater
            - 2) It checks the installed official Viceserver modpacks
            - 3) If an official modpack is not installed, it gives the possibility to the user to install it
            */
            Utilities.RedrawCmd("initial-setup");
            // Utilities.RedrawCmd("check-version");
            Console.SetCursorPosition(0, Console.CursorTop + 2);
        }
    }
}
