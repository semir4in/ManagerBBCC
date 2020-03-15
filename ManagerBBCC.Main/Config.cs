using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManagerBBCC.Main
{
    public static class Config
    {
        public const int ThreadSleepTimeout = 50;
        public const int ThreadLongSleepTimeout = 500;

        // Assembly
        public static readonly string ProjectName = Assembly.GetEntryAssembly().GetName().Name;
        public static readonly Version ProjectVersion = Assembly.GetExecutingAssembly().GetName().Version;

        // AppData and Configuration
        public static readonly string LocalAppDataRootPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string LocalAppDataPath = Path.Combine(Config.LocalAppDataRootPath, Config.ProjectName);
        public static readonly string SettingFileName = "setting.json";
        public static readonly string SettingFilePath = Path.Combine(Config.LocalAppDataPath, Config.SettingFileName);

        //
        public static readonly List<string> ImageExtensions = new List<string>()
        {
            "*.png",
            "*.jpg",
            "*.gif",
        };

        public const string ImageFolderSubPath = "images\\dccon";
        public const string DataFolderSubPath = "lib";

        public const string JavaScriptFileName = "dccon_list.js";
        public const string JsonFileName = "list_data.json";

        public const string JsNameKey = "name";
        public const string JsKeywordKey = "keywords";
        public const string JsTagKey = "tags";

        public const string JsonPathKey = "path";
        public const string JsonHeaderKey = "dccons";
        public const string JsonKeywordKey = "keywords";
        public const string JsonTagKey = "tags";

        public static string ImagePathSeed
        {
            get
            {
                string[] imagePathSplit = Config.ImageFolderSubPath.Split('\\');
                return $"https://raw.githubusercontent.com/{{0}}/{{1}}/master/{imagePathSplit.First()}/{imagePathSplit.Last()}/{{2}}";
            }
        }

        public const string NoTag = "태그없음";
        public const string OverlappedNameToken = "##overlapped##";

        public static readonly Encoding TextEncoding = Encoding.Unicode;
        public static readonly JsonSerializerSettings JsonSerializerSetting = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter()
                {
                    NamingStrategy = new DefaultNamingStrategy(),
                },
                new IsoDateTimeConverter()
                {
                    Culture = CultureInfo.CurrentCulture,
                    DateTimeStyles = DateTimeStyles.AdjustToUniversal,
                },
            }
        };

        public const string DiscordUrl = "https://discord.gg/T9sg3yX";
        public const string GithubProjectUrl = "https://github.com/semir4in/ManagerBBCC";

        public static readonly string Title = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
        public const int VersionMajorNumber = 0;
        public static string VersionString => $"{Config.VersionMajorNumber}.{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
        public static string VersionShortString
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{Config.VersionMajorNumber}.{version.Major}.{version.Minor}";
            }
        }
        public static Version Version => Version.Parse(Config.VersionShortString);
    }
}
