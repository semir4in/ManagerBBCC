using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ManagerBBCC.Main.Classes
{
    public class TagFilter
    {
        public string Tag { get; set; }
        public List<int> IDs { get; set; }
        public int Count => this.IDs.Count;

        [JsonIgnore]
        public string FilterString => $"#{this.Tag} ({this.Count})";
    }
}
