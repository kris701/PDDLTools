using PDDLTools.Commands;
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
    internal sealed class SelectDomainListCommand : BaseCommand
    {
        public override int CommandId { get; } = 259;
        public static SelectDomainListCommand Instance { get; internal set; }
        public static string ActiveDocumentComboboxName = "Active Document (If Valid)";
        public static string NoneFoundComboboxName = "No open valid PDDL domains found";

        private SelectDomainListCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectDomainListCommand(package, await InitializeCommandServiceAsync(package));
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
                    var possibleDomains = GetDomainsOnly(allDocuments);
                    if (possibleDomains.Count == 0)
                        possibleDomains.Add(NoneFoundComboboxName);
                    possibleDomains.Insert(0, ActiveDocumentComboboxName);
                    Marshal.GetNativeVariantForObject(possibleDomains.ToArray(), pOutValue);
                }
            }
        }

        private List<string> GetDomainsOnly(List<string> source)
        {
            List<string> resultList = new List<string>();

            foreach(var possibleDomain in source)
                if (PDDLHelper.IsFileDomain(possibleDomain))
                    resultList.Add(possibleDomain);

            return resultList;
        }
    }
}
