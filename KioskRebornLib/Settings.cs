﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KioskRebornLib
{
    public class Settings
    {
        [JsonIgnore]
        #pragma warning disable CS8604 // Possible null reference argument.
        public static string PATH = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "KioskReborn");
        #pragma warning restore CS8604 // Possible null reference argument.

        [JsonIgnore]
        private static string CONFIG = Path.Combine(PATH, "settings.json");

        [JsonConverter(typeof(StringEnumConverter))]
        public enum Colors { Classic, Blue, Dark, Green, Gray, LJU }

        [JsonProperty]
        public Colors Color { get; set; }
        [JsonProperty]
        public string Background { get; set; }
        [JsonProperty]
        public WBrowser Browser { get; set; }
        [JsonProperty]
        public List<Favorite> Favorites { get; set; }

        private Settings()
        {
            Color = Colors.Classic;
            Background = Path.Combine(PATH, "Images", "background.jpg");
            Browser = new WBrowser();
            Favorites = new List<Favorite>();
        }

        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        #pragma warning disable CS8603 // Possible null reference return.
        public static Settings Get()
        {
            Settings settings;

            if (!Directory.Exists(PATH))
            {
                Directory.CreateDirectory(PATH);
            }

            string passwd = Path.Combine(Settings.PATH, "passwd");

            if (!File.Exists(passwd))
            {
                File.WriteAllText(passwd, "Password");
            }

            if (!File.Exists(CONFIG))
            {
                settings = new Settings();

                settings.Favorites = new List<Favorite>
                {
                    new Favorite("Google", "https://google.com", 180),
                    new Favorite("Microsoft", "https://microsoft.com", 180)
                };

                File.WriteAllText(CONFIG, JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            else
            {

                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(CONFIG));

            }


            return settings;

        }
        #pragma warning restore CS8603 // Possible null reference return.
        #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        public void Save()
        {
            File.WriteAllText(CONFIG, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public class WBrowser
        {
            [JsonProperty]
            public bool Enable { get; set; }
            [JsonProperty]
            public string URL { get; set; }
            [JsonProperty]
            public bool AllowExit { get; set; }
            [JsonProperty]
            public bool AutoStart { get; set; }

            public WBrowser()
            {
                URL = "https://duckduckgo.com";
                Enable = true;
                AutoStart = false;
                AllowExit = true;
            }
        }

        public class Favorite
        {
            [JsonProperty]
            public string Name { get; set; }
            [JsonProperty]
            public string URL { get; set; }
            [JsonProperty]
            public int Width { get; set; }

            public Favorite(string Name, string URL, int Width)
            {
                this.Name = Name;
                this.URL = URL;
                this.Width = Width;
            }
        }
    }
}
