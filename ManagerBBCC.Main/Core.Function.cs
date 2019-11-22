using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

using Jint.Parser;
using Jint.Parser.Ast;

using ManagerBBCC.Main.Classes;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        public static bool CheckSetting()
        {
            if (!Directory.Exists(Config.LocalAppDataPath))
            {
                Directory.CreateDirectory(Config.LocalAppDataPath);
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

        public static bool ImportEntryFromImage()
        {
            if (!Directory.Exists(Core.ImageFolderPath)) return false;

            List<string> imagePaths = Directory.GetFiles(Core.ImageFolderPath)
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
                    Core.Meta.Entries.Add(tokenEntry);
                }
                else
                {
                    overlappedEntry.LocalPath = tokenEntry.LocalPath;
                }
            }

            return true;
        }
        public static bool UpdateImage()
        {
            return true;
        }

        public static bool ImportEntryFromProperty()
        {
            string jsFilePath = Path.Combine(Core.DataFolderPath, Config.JavaScriptFileName);
            if (File.Exists(jsFilePath))
            {
                // Parse 'javascript' file
                string rawString = File.ReadAllText(jsFilePath);
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
        public static bool UpdateProperty()
        {


            return true;
        }


        public static void CheckTagPoolFromEntry()
        {
            Core.Meta.CheckPoolCount();
        }

        public static void CheckControls()
        {
            Core.MainWindow.BBCCRootBox.Text = Core.Setting.BBCCPath ?? "";
            Core.MainWindow.RefreshEntryButton.IsEnabled = Core.IsDataFolderValid;
            //Core.MainWindow.OpenBBCCRootButton.IsEnabled = Core.IsDataFolderValid;
        }

        public static void DisplayEntries()
        {
            Core.MainWindow.EntryDataGrid.ItemsSource = null;
            Core.MainWindow.EntryDataGrid.ItemsSource = (Core.Meta.Active.Count == 0)
                ? Core.Meta.Entries
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .ToList()
                : Core.Meta.Active
                    .Distinct()
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .ToList();
        }

        public static void DisplayTags()
        {
            Core.MainWindow.TagListView.ItemsSource = null;
            Core.MainWindow.TagListView.ItemsSource = Core.Meta.TagPool;
        }
    }
}
