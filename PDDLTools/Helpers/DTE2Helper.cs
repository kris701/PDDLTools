﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Helpers
{
    public static class DTE2Helper
    {
        public static EnvDTE80.DTE2 GetDTE2()
        {
            return Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
        }

        public static async Task<bool> IsValidFileOpenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                EnvDTE80.DTE2 _applicationObject = GetDTE2();
                if (_applicationObject == null)
                    return false;
                var uih = _applicationObject.ActiveDocument;
                if (uih == null)
                    return false;
                var extension = await GetSourceFileExtensionAsync();
                if (extension != Constants.PDDLExt)
                    return false;
                return true;
            } catch
            {
                return false;
            }
        }

        public static async Task<bool> IsFullyVSOpenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            if (_applicationObject == null)
                return false;
            return _applicationObject.Solution.IsOpen;
        }

        public static async Task<string> GetSourceFilePathAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            if (_applicationObject.ActiveDocument == null)
                return null;
            var uih = _applicationObject.ActiveDocument;
            return uih.FullName;
        }

        public static async Task<string> GetSourceFileNameAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            var uih = _applicationObject.ActiveDocument;
            return uih.Name;
        }

        public static async Task<string> GetSourceFileExtensionAsync()
        {
            string name = await GetSourceFileNameAsync();
            return name.Substring(name.LastIndexOf('.'));
        }

        public static async Task<string> GetSourcePathAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            var uih = _applicationObject.ActiveDocument;
            return uih.Path;
        }

        public static async Task SaveActiveDocumentAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            var uih = _applicationObject.ActiveDocument;
            if (uih != null)
                uih.Save();
        }

        public static async Task FocusActiveDocumentAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            var uih = _applicationObject.ActiveDocument;
            if (uih != null)
                uih.Activate();
        }

        public static async Task<List<string>> GetAllOpenDocumentsAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            var documents = _applicationObject.Documents;
            List<string> result = new List<string>();
            foreach (Document document in documents)
                result.Add(document.FullName);
            return result;
        }

        public static async Task<string> GetSelectedTextAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            var uih = _applicationObject.ActiveDocument;
            var value = ComUtils.Get(uih.Selection, "Text").ToString();
            return value;
        }

        public static async Task<string> GetSourceFilePathFromSolutionExploreAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            UIHierarchy uih = _applicationObject.ToolWindows.SolutionExplorer;
            Array selectedItems = (Array)uih.SelectedItems;
            if (null != selectedItems)
            {
                if (selectedItems.Length > 1)
                    return null;
                foreach (UIHierarchyItem selItem in selectedItems)
                {
                    ProjectItem prjItem = selItem.Object as ProjectItem;
                    
                    string filePath = prjItem.Properties.Item("FullPath").Value.ToString();
                    return filePath;
                }
            }
            return null;
        }

        public static async Task<bool> IsItemInPDDLProjectAsync(string item)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (item == null)
                return false;

            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            EnvDTE.Projects projects = _applicationObject.Solution.Projects;
            for (int i = 1; i < projects.Count + 1; i++)
            {
                var proj = projects.Item(i);
                if (new Guid(proj.Kind) == new Guid(Constants.PDDLProjectTypeID))
                {
                    foreach (ProjectItem file in proj.ProjectItems)
                    {
                        string filePath = file.Properties.Item("FullPath").Value.ToString();
                        if (filePath == item)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
