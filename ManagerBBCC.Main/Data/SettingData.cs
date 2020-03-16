using ManagerBBCC.Main.Classes;
using ManagerBBCC.Main.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerBBCC.Main.Data
{
    public class SettingData : JsonFunctionBase
    {

        private bool _doLoadBbccWarning = true;
        public bool DoLoadBbccWarning
        {
            get => this._doLoadBbccWarning;
            set
            {
                if (this._doLoadBbccWarning == value) return;

                this._doLoadBbccWarning = value;
                this.Save();
            }
        }

        private string _bbccPath = "";
        public string BbccPath
        {
            get => this._bbccPath;
            set
            {
                if (this._bbccPath == value) return;

                this._bbccPath = value;
                this.Save();
            }
        }

        private bool _githubSaved = false;
        public bool GithubSaved
        {
            get => this._githubSaved;
            set
            {
                if (this._githubSaved == value) return;

                this._githubSaved = value;
                this.Save();
            }
        }

        private string _githubUserName = "";
        public string GithubUserName
        {
            get => this._githubUserName;
            set
            {
                if (this._githubUserName == value) return;

                this._githubUserName = value;
                //this.Save();
            }
        }

        private string _githubRepositoryName = "";
        public string GithubRepositoryName
        {
            get => this._githubRepositoryName;
            set
            {
                if (this._githubRepositoryName == value) return;

                this._githubRepositoryName = value;
                //this.Save();
            }
        }

#if DEBUG
        private bool _openDebugWhenStartup = true;
        public bool OpenDebugWhenStartup
        {
            get => this._openDebugWhenStartup;
            set
            {
                if (this._openDebugWhenStartup == value) return;

                this._openDebugWhenStartup = value;
                this.Save();
            }
        }
#endif

        public static SettingData Load()
        {
            return SettingData.Load(Config.SettingFilePath);
        }
        public static SettingData Load(string path)
        {
            bool failFlag = false;
            string rawString;
            if (File.Exists(path))
            {
                rawString = File.ReadAllText(path, Config.TextEncoding);
            }
            else
            {
                rawString = "";
                failFlag = true;
            }

            SettingData settingData = null;
            try
            {
                settingData = JsonConvert.DeserializeObject<SettingData>(rawString, Config.JsonSerializerSetting);
            }
            catch
            {
                failFlag = true;
            }

            if (settingData == null) failFlag = true;
            if (failFlag)
            {
                settingData = new SettingData();
                settingData.Save(path);
            }

            return settingData;
        }

        public void Save()
        {
            this.Save(Config.SettingFilePath);
        }
    }
}
