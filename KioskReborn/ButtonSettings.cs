using KioskRebornLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace KioskReborn
{
    public class ButtonSettings
    {
        public string name;
        public string executable;
        public string arguments;
        public string text;
        public string icon;
        public bool startUp;

        public ButtonSettings(string name, string executable, string arguments = "none", string text = "none", string icon = "none", bool startUp = false)
        {
            this.name = name;
            this.executable = executable;
            this.arguments = arguments;
            this.text = text;
            this.icon = icon;
            this.startUp = startUp;
        }

        public static List<ButtonSettings> getButtons()
        {
            string path = Path.Combine(Settings.PATH, "buttons.json");

            List<ButtonSettings> buttons;

            if (!File.Exists(path))
            {
                string icons = Path.Combine(Settings.PATH, "Images");

                buttons = new List<ButtonSettings>
                {
                    new ButtonSettings("SampleEdge", @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "--kiosk http://google.com --edge-kiosk-type=fullscreen --no-first-run", "none", @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", false),
                    new ButtonSettings("SampleExcel", @"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE", "none", "none", @"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE", false),
                    new ButtonSettings("SampleWord", @"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE", "none", "none", @"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE", false),
                    new ButtonSettings("SampleCalc", "UWP", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App", "none", icons + @"\calculator.png", false),
                    new ButtonSettings("SampleNotepad", "UWP", "Microsoft.WindowsNotepad_8wekyb3d8bbwe!App", "none", icons + @"\notepad.png", false),
                    new ButtonSettings("UniqueNameHere", "add executable path here", "add arguments here or 'none'", "add button text here or 'none'", "path to .ico, .png or .exe for image or 'none'", false)
                };

                File.WriteAllText(path, JsonConvert.SerializeObject(buttons, Formatting.Indented));
            }
            else
            {
                buttons = JsonConvert.DeserializeObject<List<ButtonSettings>>(File.ReadAllText(path));
            }

            return buttons;
        }

        public static ButtonSettings getButton(string name)
        {
            foreach (ButtonSettings buttonSettings in ButtonSettings.getButtons())
            {
                if (buttonSettings.name.Equals(name))
                {
                    return buttonSettings;
                }
            }

            return null;
        }
    }
}

