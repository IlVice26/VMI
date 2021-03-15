using System;
using Terminal.Gui;

namespace ViceserverModpackInstaller
{
    class Program
    {
        public static void Main()
        {
            // Clear the console
            Console.Clear();
 
            /**
            Tasks of Viceserver Modpack Installer
            - 1) It checks the version of the installer, if it's outdated, it runs the updater
            - 2) It checks the installed official Viceserver modpacks
            - 3) If an official modpack is not installed, it gives the possibility to the user to install it
            */
            Utilities.RedrawCmd("initial-setup");

            DataManager.PathResolver();
            // Utilities.RedrawCmd("test");
            Console.ReadLine();
        }
    }
}
