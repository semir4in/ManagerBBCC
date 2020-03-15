using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ManagerBBCC.Main.Functions;
using Newtonsoft.Json;

namespace ManagerBBCC.Main.Classes
{
    public class Entry : JsonFunctionBase
    {
        public Entry()
        {
            this.ID = -1;
            this.Order = -1;

            this.Keywords = new List<string>();
            this.Tags = new List<string>();

            this.LocalPath = "";
        }

        public int ID { get; set; }
        public int Order { get; set; } 
        public string Name { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Tags { get; set; }
        public string Url { get; set; }
        public string LocalPath { get; set; }


        [JsonIgnore]
        public string EntryString => this.Serialize();
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
        [JsonIgnore]
        public string JavaScriptString
        {
            get
            {
                return $"{{ {Config.JsNameKey}: \"{this.Name}\""
                   + $", {Config.JsKeywordKey}: [{this.JoinedStringProperty(this.Keywords)}]"
                   + $", {Config.JsTagKey}: [{this.JoinedStringProperty(this.Tags)}] }}";
            }
        }
        [JsonIgnore]
        public string JSONString
        {
            get
            {
                if (Core.Setting.GithubSaved)
                {
                    string onlinePath = string.Format(Config.ImagePathSeed,
                    Core.Setting.GithubUserName,
                    Core.Setting.GithubRepositoryName,
                    this.Name);

                    return $"{{ " + $"\"{Config.JsKeywordKey}\": [{this.JoinedStringProperty(this.Keywords)}]"
                         + $", " + $"\"{Config.JsTagKey}\": [{this.JoinedStringProperty(this.Tags)}]"
                         + $", " + $"\"{Config.JsonPathKey}\": \"{onlinePath}\""
                         + $" }}";
                }
                else
                {
                    return $"{{ " + $"\"{Config.JsKeywordKey}\": [{this.JoinedStringProperty(this.Keywords)}]"
                         + $", " + $"\"{Config.JsTagKey}\": [{this.JoinedStringProperty(this.Tags)}]"
                         + $" }}";
                }
            }
        }
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                bool output = true;

                if (this.ID < 0) output = false;
                if (this.Name.Length == 0) output = false;
                if (this.Keywords.Count == 0) output = false;
                if (this.Tags.Count == 0) output = false;
                if (!File.Exists(this.LocalPath)) output = false;

                return output;
            }
        }
        private string JoinedStringProperty(List<string> list)
        {
            return string.Join(", ", list.ConvertAll(x => $"\"{x}\""));
        }
    }
}
