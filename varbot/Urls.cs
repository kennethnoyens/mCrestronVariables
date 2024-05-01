using Crestron.SimplSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varbot
{
    internal class Urls : List<Url>
    {
        private static string jsonFileName = @"/user/urls.json";

        public Urls()
        {
            CrestronConsole.PrintLine("Loading urls from json");

            if (!File.Exists(jsonFileName))
            {
                CrestronConsole.PrintLine("File with urls doesn't exist");
                return;
            }
            try
            {
                this.AddRange(JsonConvert.DeserializeObject<List<Url>>(File.ReadAllText(jsonFileName)) ?? new List<Url>());
                CrestronConsole.PrintLine($"{this.Count} magic urls loaded from json");
            }
            catch (Exception ex)
            {
                ErrorLog.Error("Error while reading magic urls from json file: {0}", ex.Message);
            }

        }

        public void AddUrl(string url)
        {
            Url newUrl = new Url(url);
            this.Add(newUrl);
            SaveToJson();
        }

        private void SaveToJson()
        {
            CrestronConsole.PrintLine("Writing json...");
            try
            {
                string Stringoutput = JsonConvert.SerializeObject(this);
                CrestronConsole.PrintLine("Writing json: " + Stringoutput);
                File.WriteAllText(jsonFileName, Stringoutput);
            }
            catch (Exception ex)
            {
                ErrorLog.Error("Error while writing json file containing magic urls: " + ex.Message);
            }
        }

    }

    internal class Url
    {
        public string url;

        public Url(string _url)
        {
            this.url = _url;
        }
    }
}
