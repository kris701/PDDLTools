﻿using PDDLTools.Commands;
using PDDLTools.Helpers;
using PDDLTools.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO.Packaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Task = System.Threading.Tasks.Task;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using PDDLParser.Helpers;

namespace PDDLTools.Commands
{
    internal sealed class SelectProblemListCommand : BaseCommand
    {
        public override int CommandId { get; } = 266;
        public static SelectProblemListCommand Instance { get; internal set; }
        public static string ActiveDocumentComboboxName = "Active Document (If Valid)";
        public static string NoneFoundComboboxName = "No open valid PDDL problems found";

        private SelectProblemListCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectProblemListCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                IntPtr pOutValue = eventArgs.OutValue;
                if (pOutValue != IntPtr.Zero)
                {
                    var allDocuments = await DTE2Helper.GetAllOpenDocumentsAsync();
                    var possibleProblems = GetProblemsOnly(allDocuments);
                    if (possibleProblems.Count == 0)
                        possibleProblems.Add(NoneFoundComboboxName);
                    possibleProblems.Insert(0, ActiveDocumentComboboxName);
                    Marshal.GetNativeVariantForObject(possibleProblems.ToArray(), pOutValue);
                }
            }
        }

        private List<string> GetProblemsOnly(List<string> source)
        {
            List<string> resultList = new List<string>();

            foreach(var possibleDomain in source)
                if (PDDLHelper.IsFileProblem(possibleDomain))
                    resultList.Add(possibleDomain);

            return resultList;
        }
    }
}
