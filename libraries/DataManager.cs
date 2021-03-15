using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.Json;


namespace ViceserverModpackInstaller
{

    public static class DataManager
    {

        private static string settings_link = "http://viceserver.vpsgh.it/files/json/settings.json";

        public static void CreateConfig()
        {
            if (!File.Exists(Utilities.installer_dir + "\\config.json"))
            {

            }
        }

        public static void PathResolver()
        {
            if (File.Exists(Utilities.installer_dir + "\\settings.json"))
            {
                Console.WriteLine("settings.json founded");
            }
            else
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(
                        new System.Uri(settings_link),
                        Utilities.installer_dir + "\\settings.json"
                    );
                };
            }
        }

    }

}