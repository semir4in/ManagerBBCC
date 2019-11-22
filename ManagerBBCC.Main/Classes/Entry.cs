using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Newtonsoft.Json;

namespace ManagerBBCC.Main.Classes
{
    public class Entry
    {
        public Entry()
        {
            this.Keywords = new List<string>();
            this.Tags = new List<string>();
        }

        public int ID { get; set; }
        public int Order { get; set; } 
        public string Name { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Tags { get; set; }
        public string LocalPath { get; set; }

        [JsonIgnore]
        public string JoinedKeyword => string.Join(", ", this.Keywords);
        [JsonIgnore]
        public string JoinedTag => string.Join(", ", this.Tags);
        [JsonIgnore]
        public string ImageInfo
        {
            get
            {
                string output = this.Name;

                if (this.LocalPath != null)
                {
                    if (File.Exists(this.LocalPath))
                    {
                        using (Bitmap bitmap = new Bitmap(this.LocalPath))
                        {
                            output += $"\n[ {Path.GetExtension(this.LocalPath).ToUpper().Remove(0,1)}, {bitmap.Width} x {bitmap.Height}p ]";
                        }
                    }
                }

                return output;
            }
        }

        public string JavaScriptString
        {
            get
            {
                return $"{{ {Config.JsNameKey}: \"{this.Name}\""
                   + $", {Config.JsKeywordKey}: [{this.JoinedStringProperty(this.Keywords)}]"
                   + $", {Config.JsTagKey}: [{this.JoinedStringProperty(this.Tags)}] }}";
            }
        }

        public string JSONString
        {
            get
            {
                string onlinePath = string.Format(Config.ImagePathSeed,
                    Core.Setting.GithubUserName,
                    Core.Setting.GithubRepositoryName,
                    this.Name);

                return $"{{ \"{Config.JsKeywordKey}\": [{this.JoinedStringProperty(this.Keywords)}]"
                   + $", \"{Config.JsTagKey}\": [{this.JoinedStringProperty(this.Tags)}]"
                   + $", \"{Config.JSONPathKey}\": \"{onlinePath}\" }}";
            }
        }

        private string JoinedStringProperty(List<string> list)
        {
            return string.Join(", ", list.ConvertAll(x => $"\"{x}\""));
        }
    }
}
