using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

using Jint.Parser;
using Jint.Parser.Ast;

using ManagerBBCC.Main.Classes;
using Newtonsoft.Json.Linq;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        public static void BeepSound()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        public static void SetStatus(string message)
        {
            Core.MainWindow.StatusBlock.Text = message;
        }

        public static bool CheckSetting()
        {
            if (!Directory.Exists(Config.LocalAppDataPath))
            {
                Directory.CreateDirectory(Config.LocalAppDataPath);
                Core.Setting.Save();
                return false;
            }

            if (!File.Exists(Config.SettingFilePath))
            {
                Core.Setting.Save();
                return false;
            }

            Core.Setting.Save();
            return true;
        }

        public static bool CheckEntries()
        {
            if (!Core.IsBBCCFolderValid) return false;

            bool output = false;
            if (Core.ImportA(Core.ImageFolderPath)) output = true;
            if (Core.ImportB(Path.Combine(Core.DataFolderPath, Config.JavaScriptFileName))) output = true;
            if (Core.ImportC(Path.Combine(Core.DataFolderPath, Config.JsonFileName))) output = true;

            return output;
        }

        public static bool SyncEntries()
        {
            if (!Core.IsBBCCFolderValid) return false;

            bool output = false;
            if (Core.ExportB(Path.Combine(Core.DataFolderPath, Config.JavaScriptFileName), false)) output = true;
            if (Core.ExportC(Path.Combine(Core.DataFolderPath, Config.JsonFileName), false)) output = true;

            return output;
        }

        /// <summary>
        /// Import entry from image
        /// </summary>
        /// <returns></returns>
        public static bool ImportA(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return false;

            List<string> imagePaths = Directory.GetFiles(folderPath)
                .Where(x => Config.ImageExtensions.Contains($"*{Path.GetExtension(x)}"))
                .ToList();

            foreach (string tokenPath in imagePaths)
            {
                Entry tokenEntry = new Entry()
                {
                    Name = Path.GetFileName(tokenPath),
                    LocalPath = tokenPath,
                };

                Entry overlappedEntry = Core.Meta.Entries.ToList().Find(x => x.Name == tokenEntry.Name);
                if (overlappedEntry == null)
                {
                    tokenEntry.ID = Core.PushID();
                    Core.Meta.Entries.Add(tokenEntry);
                }
                else
                {
                    overlappedEntry.LocalPath = tokenEntry.LocalPath;
                }
            }

            return true;
        }

        /// <summary>
        /// Import entry from javascript property
        /// </summary>
        /// <returns></returns>
        public static bool ImportB(string jsPath)
        {
            if (!File.Exists(jsPath)) return false;

            // Parse 'javascript' file
            string rawString = File.ReadAllText(jsPath);
            JavaScriptParser parser = new JavaScriptParser(false);
            Program program = parser.Parse(rawString);
            ExpressionStatement expression = program.Body.First() as ExpressionStatement;
            AssignmentExpression assignment = expression.Expression as AssignmentExpression;
            ArrayExpression topArray = assignment.Right as ArrayExpression;

            // Rebuild entry
            foreach (ObjectExpression tokenObject in topArray.Elements)
            {
                Property nameProperty = PropertyFromObject(tokenObject, Config.JsNameKey);
                Property keywordProperty = PropertyFromObject(tokenObject, Config.JsKeywordKey);
                Property tagProperty = PropertyFromObject(tokenObject, Config.JsTagKey);

                string name = (nameProperty == null) ? null : (nameProperty.Value as Literal).Value as string;
                List<string> keywords = StringsFromProperty(keywordProperty);
                List<string> tags = StringsFromProperty(tagProperty);

                Entry tokenEntry = new Entry();
                if (name != null) tokenEntry.Name = name;
                if (keywords.Count > 0) tokenEntry.Keywords = keywords.OrderBy(x => x).ToList();
                if (tags.Count > 0) tokenEntry.Tags = tags.OrderBy(x => x).ToList();

                Entry overlappedEntry = Core.Meta.Entries.ToList().Find(x => x.Name == tokenEntry.Name);
                if (overlappedEntry == null)
                {
                    tokenEntry.ID = Core.PushID();
                    Core.Meta.Entries.Add(tokenEntry);
                }
                else
                {
                    overlappedEntry.Tags.AddRange(tokenEntry.Tags);
                    overlappedEntry.Tags = new List<string>(overlappedEntry.Tags.Distinct().OrderBy(x => x));

                    overlappedEntry.Keywords.AddRange(tokenEntry.Keywords);
                    overlappedEntry.Keywords = new List<string>(overlappedEntry.Keywords.Distinct());
                }
            }

            return true;

            Property PropertyFromObject(ObjectExpression @object, string key)
            {
                return @object.Properties.First(x => (x.Key as Identifier).Name == key);
            }
            List<string> StringsFromProperty(Property property)
            {
                return (property == null)
                    ? new List<string>()
                    : (property.Value as ArrayExpression).Elements
                        .ToList()
                        .ConvertAll(x => (x as Literal).Value as string);
            }
        }

        /// <summary>
        /// Import entry from json property
        /// </summary>
        /// <returns></returns>
        public static bool ImportC(string jsonPath)
        {
            if (!File.Exists(jsonPath)) return false;

            // Parse 'json' file
            string rawString = File.ReadAllText(jsonPath);
            JToken listToken = JObject.Parse(rawString)[Config.JsonHeaderKey];

            if (listToken == null) return false;

            // Rebuild entry
            foreach (JObject entryJson in JArray.FromObject(listToken))
            {
                JToken keywordToken = entryJson[Config.JsonKeywordKey];
                if (keywordToken == null) continue;
                List<string> keywords = JArray.FromObject(keywordToken).ToObject<List<string>>();

                JToken tagToken = entryJson[Config.JsonTagKey];
                if (tagToken == null) continue;
                List<string> tags = JArray.FromObject(tagToken).ToObject<List<string>>();

                JToken pathToken = entryJson[Config.JsonPathKey];
                if (pathToken == null) continue;
                string path = pathToken.ToString();

                string name = path.Split('/').Last();
                if (name.Length == 0) continue;

                Entry tokenEntry = new Entry
                {
                    Name = name,
                    Url = path,
                };
                if (keywords.Count > 0) tokenEntry.Keywords = keywords.OrderBy(x => x).ToList();
                if (tags.Count > 0) tokenEntry.Tags = tags.OrderBy(x => x).ToList();

                Entry overlappedEntry = Core.Meta.Entries.ToList().Find(x => x.Name == tokenEntry.Name);
                if (overlappedEntry == null)
                {
                    tokenEntry.ID = Core.PushID();
                    Core.Meta.Entries.Add(tokenEntry);
                }
                else
                {
                    overlappedEntry.Url = path;

                    overlappedEntry.Tags.AddRange(tokenEntry.Tags);
                    overlappedEntry.Tags = new List<string>(overlappedEntry.Tags.Distinct().OrderBy(x => x));

                    overlappedEntry.Keywords.AddRange(tokenEntry.Keywords);
                    overlappedEntry.Keywords = new List<string>(overlappedEntry.Keywords.Distinct().OrderBy(x => x));
                }
            }

            return true;
        }

        //public static bool ExportA(string folderPath, bool validOnly)
        //{
        //    // Can not save images from data
        //    return true;
        //}

        public static bool ExportB(string jsPath, bool validOnly)
        {
            string jsDirPath = Path.GetDirectoryName(jsPath);

            if (!Directory.Exists(jsDirPath)) Directory.CreateDirectory(jsDirPath);

            File.WriteAllText(jsPath, Core.Meta.GetJavaScript(validOnly));

            return true;
        }

        public static bool ExportC(string jsonPath, bool validOnly)
        {
            string jsonDirPath = Path.GetDirectoryName(jsonPath);

            if (!Directory.Exists(jsonDirPath)) Directory.CreateDirectory(jsonDirPath);

            File.WriteAllText(jsonPath, Core.Meta.GetJson(validOnly));

            return true;
        }


        public static void CheckTagPoolFromEntry()
        {
            Core.Meta.CheckPoolCount();
            Core.MainWindow.TagPoolCount = Core.Meta.TagPool.Count;
            
        }

        public static void CheckControls()
        {
            Core.DisplayEntries();
            Core.DisplayTags();

            Core.MainWindow.BBCCRootBox.Text = Core.Setting.BBCCPath ?? "";
            //Core.MainWindow.RefreshEntryButton.IsEnabled = Core.IsDataFolderValid;
            //Core.MainWindow.OpenBBCCRootButton.IsEnabled = Core.IsDataFolderValid;
        }

        public static void DisplayEntries()
        {
            List<Entry> actives = ((Core.Meta.Filtered.Count > 0) ? Core.Meta.Filtered.Distinct() : Core.Meta.Entries)
                .OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();

            Core.MainWindow.EntryListView.ItemsSource = null;
            Core.MainWindow.EntryListView.ItemsSource = actives;
        }

        public static void DisplayTags()
        {
            Core.MainWindow.TagListView.ItemsSource = null;
            Core.MainWindow.TagListView.ItemsSource = Core.Meta.TagPool;
        }
    }
}
