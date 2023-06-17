﻿using Microsoft.VisualStudio.Settings;
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

            if (_userSettingsStore.CollectionExists(SettingsCategory))
            {
                _FDPath = _userSettingsStore.GetString(SettingsCategory, "FDPath");
                _PythonPrefix = _userSettingsStore.GetString(SettingsCategory, "PythonPrefix");
                _FDFileExecutionTimeout = _userSettingsStore.GetInt32(SettingsCategory, "FDFileExecutionTimeout");
                _SearchOptions = _userSettingsStore.GetString(SettingsCategory, "SearchOptions");
                _OpenResultReport = _userSettingsStore.GetBoolean(SettingsCategory, "OpenResultReport");
                _OpenSASSolutionVisualiser = _userSettingsStore.GetBoolean(SettingsCategory, "OpenSASSolutionVisualiser");
                _IsFirstStart = _userSettingsStore.GetBoolean(SettingsCategory, "IsFirstStart");
            }
            else
            {
                _userSettingsStore.CreateCollection(SettingsCategory);
                FDPath = "";
                PythonPrefix = "python";
                FDFileExecutionTimeout = 10;
                SearchOptions = "astar(lmcut());lazy_greedy([ff(), cea()], [ff(), cea()])";
                OpenResultReport = true;
                OpenSASSolutionVisualiser = true;
                IsFirstStart = true;
            }
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
                _userSettingsStore.SetString(SettingsCategory, "PythonPrefix", value);
                _PythonPrefix = value;
            }
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

        private string _SearchOptions = "astar(lmcut());lazy_greedy([ff(), cea()], [ff(), cea()])";
        public string SearchOptions
        {
            get { return _SearchOptions; }
            set
            {
                _userSettingsStore.SetString(SettingsCategory, "SearchOptions", value);
                _SearchOptions = value;
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
    }
}