using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ViceserverModpackInstaller
{

    public static class DataManager
    {

        private static string settings_link = "http://viceserver.vpsgh.it/files/json/settings.json";
        public static string username = Environment.UserName;
        public static dynamic settings = PathResolver();
        public static dynamic settings_info = JsonConvert.DeserializeObject<Dictionary<string, object>>(GetSettingsInfo());
        public static dynamic modpacks_info = JsonConvert.DeserializeObject<Dictionary<string, object>>(GetModpacksInfo());
        public static List<string> modpacks_names = GetModpacksNames();

        public static void CreateInstallerConfig()
        {
            if (!Directory.Exists(settings_info["general"]["installer_folder"].ToString()))
            {
                Directory.CreateDirectory(settings_info["general"]["installer_folder"].ToString());
            }

            if (!File.Exists(settings_info["general"]["installer_config"].ToString()))
            {
                File.WriteAllText(
                    settings_info["general"]["installer_config"].ToString(),
                    settings_info["general"]["installer_config_template"].ToString()
                );
            }
        }

        public static void CreateModpackConfig(string modpack)
        {
            if (modpacks_names.Contains(modpack))
            {
                if (!File.Exists(modpacks_info[modpack]["profile"]["gameDir"].ToString() + "\\config.json"))
                {
                    File.WriteAllText(
                        modpacks_info[modpack]["profile"]["gameDir"].ToString() + "\\config.json",
                        settings_info["general"]["first_install_config_template"].ToString()
                    );
                }
            }
        }

        public static void CreateGameDirectory(string modpack)
        {
            if (modpacks_names.Contains(modpack))
            {
                if (!Directory.Exists(modpacks_info[modpack]["profile"]["gameDir"].ToString()))
                {
                    Directory.CreateDirectory(
                        modpacks_info[modpack]["profile"]["gameDir"].ToString()
                    );
                }
            }
        }

        private static string GetDirInstancesOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                dynamic settingsJson = LoadJson("tmp/settings.json");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    string dir_instances = settingsJson["settings"]["instances_folder"]["dir_instances_osx"];

                    if (dir_instances.Contains("%"))
                    {
                        string[] temp_var_dir = dir_instances.Split("%");
                        if (temp_var_dir[1].Equals("home"))
                        {
                            dir_instances = "/Users/" + username + temp_var_dir[2];
                            return dir_instances;
                        }
                    }

                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string dir_instances = settingsJson["settings"]["instances_folder"]["dir_instances_lnx"];

                    if (dir_instances.Contains("%"))
                    {
                        string[] temp_var_dir = dir_instances.Split("%");
                        if (temp_var_dir[1].Equals("home"))
                        {
                            dir_instances = "/home/";
                            return dir_instances;
                        }
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dynamic settingsJson = LoadJson("tmp/settings.json");
                string dir_instances = settingsJson["settings"]["instances_folder"]["dir_instances_win"];

                if (dir_instances.Contains("%"))
                {
                    string[] temp_var_dir = dir_instances.Split("%");
                    if (temp_var_dir[1].Equals("home"))
                    {
                        dir_instances = "C:\\Users\\" + username + temp_var_dir[2];
                        return dir_instances;
                    }
                }
            }
            return null;
        }

        private static dynamic GetSettingsJsonOS()
        {
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory(
                   "tmp"
                );
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!File.Exists("tmp/settings.json"))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(
                            new System.Uri(settings_link),
                            "tmp/settings.json"
                        );
                    };
                }
                return LoadJson("tmp/settings.json");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!File.Exists("tmp\\settings.json"))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(
                            new System.Uri(settings_link),
                            "tmp\\settings.json"
                        );
                    };
                }
                return LoadJson("tmp\\settings.json");
            }
            return null;
        }

        private static dynamic PathResolver()
        {

            // Variables already fixed by GetDirInstancesOS and GetSettingsJsonOS
            dynamic settingsJson = GetSettingsJsonOS();
            string dir_instances = GetDirInstancesOS();

            // Fix minecraft section
            string mc_folder = settingsJson["settings"]["minecraft"]["mc_folder"];
            string mc_versions = settingsJson["settings"]["minecraft"]["mc_versions"];
            string launcher_profiles = settingsJson["settings"]["minecraft"]["launcher_profiles"];

            if (mc_folder.Contains("%"))
            {
                string[] temp_var_dir = mc_folder.Split("%");
                if (temp_var_dir[1].Contains("dir_instances"))
                {
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string unix_str = temp_var_dir[2].Replace("\\", "/");
                        mc_folder = dir_instances + unix_str;
                        settingsJson["settings"]["minecraft"]["mc_folder"] = mc_folder;
                    }
                    else
                    {
                        mc_folder = dir_instances + temp_var_dir[2];
                        settingsJson["settings"]["minecraft"]["mc_folder"] = mc_folder;
                    }

                }
            }

            if (mc_versions.Contains("%"))
            {
                string[] temp_var_dir = mc_versions.Split("%");
                if (temp_var_dir[1].Equals("mc_folder"))
                {
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string unix_str = temp_var_dir[2].Replace("\\", "/");
                        mc_versions = mc_folder + unix_str;
                        settingsJson["settings"]["minecraft"]["mc_versions"] = mc_versions;
                    }
                    else
                    {
                        mc_versions = mc_folder + temp_var_dir[2];
                        settingsJson["settings"]["minecraft"]["mc_versions"] = mc_versions;
                    }

                }
            }

            if (launcher_profiles.Contains("%"))
            {
                string[] temp_var_dir = launcher_profiles.Split("%");
                if (temp_var_dir[1].Equals("mc_folder"))
                {
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string unix_str = temp_var_dir[2].Replace("\\", "/");
                        launcher_profiles = mc_folder + unix_str;
                        settingsJson["settings"]["minecraft"]["launcher_profiles"] = launcher_profiles;
                    }
                    else
                    {
                        launcher_profiles = mc_folder + temp_var_dir[2];
                        settingsJson["settings"]["minecraft"]["launcher_profiles"] = launcher_profiles;
                    }

                }
            }

            // Fix installer section
            string installer_folder = settingsJson["settings"]["general"]["installer_folder"];
            string installer_config = settingsJson["settings"]["general"]["installer_config"];

            if (installer_folder.Contains("%"))
            {
                string[] temp_var_dir = installer_folder.Split("%");
                if (temp_var_dir[1].Contains("dir_instances"))
                {
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string unix_str = temp_var_dir[2].Replace("\\", "/");
                        installer_folder = dir_instances + unix_str;
                        settingsJson["settings"]["general"]["installer_folder"] = installer_folder;
                    }
                    else
                    {
                        installer_folder = dir_instances + temp_var_dir[2];
                        settingsJson["settings"]["general"]["installer_folder"] = installer_folder;
                    }

                }
            }

            if (installer_config.Contains("%"))
            {
                string[] temp_var_dir = installer_config.Split("%");
                if (temp_var_dir[1].Equals("installer_folder"))
                {
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string unix_str = temp_var_dir[2].Replace("\\", "/");
                        installer_config = installer_folder + unix_str;
                        settingsJson["settings"]["general"]["installer_config"] = installer_config;
                    }
                    else
                    {
                        installer_config = installer_folder + temp_var_dir[2];
                        settingsJson["settings"]["general"]["installer_config"] = installer_config;
                    }

                }
            }

            foreach (JProperty item in settingsJson["modpacks"])
            {
                string modpack_gameDir = settingsJson["modpacks"][item.Name.ToString()]["profile"]["gameDir"].ToString();

                if (modpack_gameDir.Contains("%"))
                {
                    string[] temp_var_dir = modpack_gameDir.Split("%");
                    if (temp_var_dir[1].Contains("dir_instances"))
                    {
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            string unix_str = temp_var_dir[2].Replace("\\", "/");
                            modpack_gameDir = dir_instances + unix_str;
                            settingsJson["modpacks"][item.Name.ToString()]["profile"]["gameDir"] = modpack_gameDir;
                        }
                        else
                        {
                            installer_config = dir_instances + temp_var_dir[2];
                            settingsJson["modpacks"][item.Name.ToString()]["profile"]["gameDir"] = modpack_gameDir;
                        }

                    }
                }
            }

            return settingsJson;

        }

        private static dynamic GetSettingsInfo()
        {
            return settings["settings"].ToString();
        }

        private static dynamic GetModpacksInfo()
        {
            return settings["modpacks"].ToString();
        }

        public static List<string> GetModpacksNames()
        {
            Dictionary<string, object>.KeyCollection mp_kc = modpacks_info.Keys;
            List<string> names = new List<string>();

            foreach (string item in mp_kc)
            {
                names.Add(item);
            }

            return names;
        }

        public static Dictionary<string, object> LoadJson(string file)
        {
            try
            {
                using (StreamReader filereader = new StreamReader(file))
                {
                    string json = filereader.ReadToEnd();
                    Dictionary<string, object> jsonFile = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
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