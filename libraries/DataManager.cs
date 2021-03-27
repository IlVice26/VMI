using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ViceserverModpackInstaller
{

    public static class DataManager
    {

        private static string settings_link = "http://viceserver.vpsgh.it/files/json/settings.json";
        public static string username = Environment.UserName;

        public static void CreateConfig()
        {
            if (!File.Exists(Utilities.installer_dir + "\\config.json"))
            {

            }
        }

        public static void PathResolver()
        {
            ShowWaitingTask.StartTask("rsv-s");
                
            if (!File.Exists(Utilities.installer_dir + "\\settings.json"))
            {
                Console.WriteLine("Test settings.json not founded");
                using (var client = new WebClient())
                {
                    client.DownloadFile(
                        new System.Uri(settings_link),
                        Utilities.installer_dir + "\\settings.json"
                    );
                };

            }

            dynamic settingsJson = LoadJson(Utilities.installer_dir + "\\settings.json");

            // Fix dir_instances
            string dir_instances = settingsJson["settings"]["instances_folder"]["dir_instances"];

            if (dir_instances.Contains("%"))
            {
                string[] temp_var_dir = dir_instances.Split("%");
                if (temp_var_dir[1].Equals("home"))
                {
                    dir_instances = "C:\\Users\\" + username + temp_var_dir[2];
                    settingsJson["settings"]["instances_folder"]["dir_instances"] = dir_instances; 
                }
            }

            // Fix minecraft section
            string mc_folder = settingsJson["settings"]["minecraft"]["mc_folder"];
            string mc_versions = settingsJson["settings"]["minecraft"]["mc_versions"];
            string launcher_profiles = settingsJson["settings"]["minecraft"]["launcher_profiles"];

            if (mc_folder.Contains("%"))
            {
                string[] temp_var_dir = mc_folder.Split("%");
                if (temp_var_dir[1].Equals("dir_instances"))
                {
                    mc_folder = dir_instances + temp_var_dir[2];
                    settingsJson["settings"]["minecraft"]["mc_folder"] = mc_folder; 
                }
            }

            if (mc_versions.Contains("%"))
            {
                string[] temp_var_dir = mc_versions.Split("%");
                if (temp_var_dir[1].Equals("mc_folder"))
                {
                    mc_versions = mc_folder + temp_var_dir[2];
                    settingsJson["settings"]["minecraft"]["mc_versions"] = mc_versions; 
                }
            }

            if (launcher_profiles.Contains("%"))
            {
                string[] temp_var_dir = launcher_profiles.Split("%");
                if (temp_var_dir[1].Equals("mc_folder"))
                {
                    launcher_profiles = mc_folder + temp_var_dir[2];
                    settingsJson["settings"]["minecraft"]["launcher_profiles"] = launcher_profiles; 
                }
            }

            // Fix installer section
            string installer_folder = settingsJson["settings"]["general"]["installer_folder"];
            string installer_config = settingsJson["settings"]["general"]["installer_config"];

            if (installer_folder.Contains("%"))
            {
                string[] temp_var_dir = installer_folder.Split("%");
                if (temp_var_dir[1].Equals("dir_instances"))
                {
                    installer_folder = dir_instances + temp_var_dir[2];
                    settingsJson["settings"]["general"]["installer_folder"] = installer_folder; 
                }
            }

            if (installer_config.Contains("%"))
            {
                string[] temp_var_dir = installer_config.Split("%");
                if (temp_var_dir[1].Equals("installer_folder"))
                {
                    installer_config = installer_folder + temp_var_dir[2];
                    settingsJson["settings"]["general"]["installer_config"] = installer_config; 
                }
            }

            ShowWaitingTask.FinishTask("rsv-s");
        }

        public static dynamic LoadJson(string file)
        {
            try
            {
                using (StreamReader filereader = new StreamReader(file))
                {
                    string json = filereader.ReadToEnd();
                    dynamic jsonFile = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    return jsonFile;
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException("File non trovato: " + file
                + "\nException: " + ex.Message);
            }
        }

    }

}