using Bunnymail.Models;
using System.Text.Json;

namespace Bunnymail.Configuration
{
    public static class ConfigFile
    {
        public static string FilePath { get; set; }

        public static Dictionary<string, TemplateOptions> Data { get; set; } = new();

        public static void Save()
        {
            string json = JsonSerializer.Serialize(Data);
            File.WriteAllText(FilePath, json);
        }

        public static void Load()
        {
            string dir = Path.GetDirectoryName(FilePath) ?? "/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                Data = JsonSerializer.Deserialize<Dictionary<string, TemplateOptions>>(json) ?? new Dictionary<string, TemplateOptions>();
            }
        }

    }
}
