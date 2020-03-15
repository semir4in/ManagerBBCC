using System.Collections.Generic;

using ManagerBBCC.Main.Functions;
using ManagerBBCC.Main.Classes;
using System.Linq;
using System;

namespace ManagerBBCC.Main.Data
{
    public class MetaData : JsonFunctionBase
    {
        public MetaData()
        {
            this.Entries = new List<Entry>();

            this.Filtered = new List<Entry>();
            this.TagPool = new List<TagFilter>();

            this.SelectedEntryIDs = new List<int>();
            this.NextID = 0;
        }

        public List<Entry> Entries { get; set; }

        public List<Entry> Filtered { get; set; }
        public List<TagFilter> TagPool { get; set; }


        public List<int> SelectedEntryIDs { get; set; }
        public int NextID { get; set; }

        public void CheckPoolCount()
        {
            this.TagPool.Clear();

            bool noTagFlag = false;
            foreach (Entry entry in this.Entries)
            {
                List<string> tags = new List<string>();
                if (entry.Tags.Count == 0)
                {
                    noTagFlag = true;
                    tags.Add(Config.NoTag);
                }
                else
                {
                    tags.AddRange(entry.Tags);
                }
                
                foreach (string tag in tags)
                {
                    TagFilter tokenFilter = this.TagPool.Find(x => x.Tag == tag);
                    if (tokenFilter == null)
                    {
                        this.TagPool.Add(new TagFilter()
                        {
                            Tag = tag,
                            IDs = new List<int>() { entry.ID },
                        });
                    }
                    else
                    {
                        tokenFilter.IDs.Add(entry.ID);
                    }
                }
            }

            this.TagPool = this.TagPool.OrderBy(x => x.Tag).ToList();

            if (noTagFlag)
            {
                TagFilter noTagFilter = this.TagPool.Find(x => x.Tag == Config.NoTag);
                if (this.TagPool.Remove(noTagFilter))
                {
                    this.TagPool.Insert(0, noTagFilter);
                }
            }
        }

        public string GetJavaScript(bool validOnly)
        {
            var sorted = this.Entries
                .Where(x => !validOnly || x.IsValid)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToList()
                .ConvertAll(x => x.JavaScriptString);

            return $"dcConsData = [\n\t{string.Join(",\n\t", sorted)}\n];";
        }

        public string GetJson(bool validOnly)
        {
            var sorted = this.Entries
                .Where(x => !validOnly || x.IsValid)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToList()
                .ConvertAll(x => x.JSONString);

            return $"{{\n\t\"dccons\": [\n\t\t{string.Join(",\n\t\t", sorted)}\n\t]\n}}";
        }

    }
}
