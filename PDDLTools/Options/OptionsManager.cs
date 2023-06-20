using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft;
using Microsoft.VisualStudio.Threading;
using System.Diagnostics;

namespace PDDLTools.Options
{
    public class OptionsManager
    {
        private readonly string SettingsCategory = "PDDL Tools";

        public static OptionsManager Instance = null;
        private WritableSettingsStore _userSettingsStore;

        private static AsyncLazy<ShellSettingsManager> _settingsManager = new AsyncLazy<ShellSettingsManager>(GetSettingsManagerAsync, ThreadHelper.JoinableTaskFactory);
        private static async Task<ShellSettingsManager> GetSettingsManagerAsync()
        {
            var svc = await AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(SVsSettingsManager)) as IVsSettingsManager;

            Assumes.Present(svc);

            return new ShellSettingsManager(svc);
        }

        public async Task LoadSettingsAsync()
        {
            SettingsManager settingsManager = await _settingsManager.GetValueAsync();
            _userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!_userSettingsStore.CollectionExists(SettingsCategory))
                _userSettingsStore.CreateCollection(SettingsCategory);

            _FDPath = GetValueOrSetDefault("FDPath", "");
            _PythonPrefix = GetValueOrSetDefault("PythonPrefix", "python");
            _FDFileExecutionTimeout = GetValueOrSetDefault("FDFileExecutionTimeout", 10);
            _EngineOptions = GetValueOrSetDefault("EngineOptions", "--search \"astar(lmcut())\";--search \"lazy_greedy([ff(), cea()], [ff(), cea()])\"");
            _OpenResultReport = GetValueOrSetDefault("OpenResultReport", true);
            _OpenSASSolutionVisualiser = GetValueOrSetDefault("OpenSASSolutionVisualiser", true);
            _IsFirstStart = GetValueOrSetDefault("IsFirstStart", true);
            _VALPath = GetValueOrSetDefault("VALPath", "");
        }

        private string GetValueOrSetDefault(string id, string defValue)
        {
            if (!_userSettingsStore.PropertyExists(SettingsCategory, id))
                _userSettingsStore.SetString(SettingsCategory, id, defValue);
            return _userSettingsStore.GetString(SettingsCategory, id);
        }
        private int GetValueOrSetDefault(string id, int defValue)
        {
            if (!_userSettingsStore.PropertyExists(SettingsCategory, id))
                _userSettingsStore.SetInt32(SettingsCategory, id, defValue);
            return _userSettingsStore.GetInt32(SettingsCategory, id);
        }
        private bool GetValueOrSetDefault(string id, bool defValue)
        {
            if (!_userSettingsStore.PropertyExists(SettingsCategory, id))
                _userSettingsStore.SetBoolean(SettingsCategory, id, defValue);
            return _userSettingsStore.GetBoolean(SettingsCategory, id);
        }

        private string _FDPath = "";
        public string FDPath
        {
            get { return _FDPath; }
            set
            {
                if (Directory.Exists(value) && File.Exists(Path.Combine(value, "fast-downward.py")))
                {
                    _userSettingsStore.SetString(SettingsCategory, "FDPath", value);
                    _FDPath = value;
                }
                else
                    MessageBox.Show("Error, invalid path to fast downward!");
            }
        }

        private string _PythonPrefix = "python";
        public string PythonPrefix
        {
            get { return _PythonPrefix; }
            set
            {
                if (ExistsOnPath(value) || ExistsOnPath(value + ".exe"))
                {
                    _userSettingsStore.SetString(SettingsCategory, "PythonPrefix", value);
                    _PythonPrefix = value;
                }
                else
                    MessageBox.Show("Error, given python prefix is not an environment variable!");
            }
        }
        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }
        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        private int _FDFileExecutionTimeout = 10;
        public int FDFileExecutionTimeout
        {
            get { return _FDFileExecutionTimeout; }
            set
            {
                if (value > 0)
                {
                    _userSettingsStore.SetInt32(SettingsCategory, "FDFileExecutionTimeout", value);
                    _FDFileExecutionTimeout = value;
                }
                else
                    MessageBox.Show("Timeout must be larger than 0!");  
            }
        }

        private string _EngineOptions = "--search \"astar(lmcut())\";--search \"lazy_greedy([ff(), cea()], [ff(), cea()])\"";
        public string EngineOptions
        {
            get { return _EngineOptions; }
            set
            {
                _userSettingsStore.SetString(SettingsCategory, "EngineOptions", value);
                _EngineOptions = value;
            }
        }

        private bool _OpenResultReport = true;
        public bool OpenResultReport
        {
            get { return _OpenResultReport; }
            set
            {
                _userSettingsStore.SetBoolean(SettingsCategory, "OpenResultReport", value);
                _OpenResultReport = value;
            }
        }

        private bool _OpenSASSolutionVisualiser = true;
        public bool OpenSASSolutionVisualiser
        {
            get { return _OpenSASSolutionVisualiser; }
            set
            {
                _userSettingsStore.SetBoolean(SettingsCategory, "OpenSASSolutionVisualiser", value);
                _OpenSASSolutionVisualiser = value;
            }
        }

        private bool _IsFirstStart = true;
        public bool IsFirstStart
        {
            get { return _IsFirstStart; }
            set
            {
                _userSettingsStore.SetBoolean(SettingsCategory, "IsFirstStart", value);
                _IsFirstStart = value;
            }
        }

        private string _VALPath = "";
        public string VALPath
        {
            get { return _VALPath; }
            set
            {
                if (Directory.Exists(value) && File.Exists(Path.Combine(value, "Validate.exe")))
                {
                    _userSettingsStore.SetString(SettingsCategory, "VALPath", value);
                    _VALPath = value;
                }
                else
                    MessageBox.Show("Error, invalid path to VAL executables!");
            }
        }
    }
}
