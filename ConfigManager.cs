using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ConfigManager
{
    private static string ConfigFn = Directory.GetCurrentDirectory() + "\\Config.json";

    private static string GetDefaultConfig()
    {
        Dictionary<string, string> DefaultConfigRaw = new Dictionary<string, string> { };
        DefaultConfigRaw.Add("poll_time", "5000");
        DefaultConfigRaw.Add("lastfm_user", "YOUR LAST.FM USERNAME");
        DefaultConfigRaw.Add("api_key", "GET AN API KEY AT https://www.last.fm/api/account/create");
        return JsonConvert.SerializeObject(DefaultConfigRaw);
    }

	public static Dictionary<string, string>? GetConfig()
    {
        if (!File.Exists(ConfigFn))
        {
            File.WriteAllText(ConfigFn, GetDefaultConfig());
            Console.WriteLine("No config was found, a new one has been created.");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please edit the config file before you continue. [!]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press any key to continue (after editing the config).");
            Console.ReadKey();
        }
        string FileText = File.ReadAllText(ConfigFn);
        Dictionary<string, string>? Config = JsonConvert.DeserializeObject<Dictionary<string, string>>(FileText);
        return Config;
    }
}
