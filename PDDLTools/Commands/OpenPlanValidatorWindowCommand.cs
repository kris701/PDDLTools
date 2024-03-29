﻿using Microsoft.VisualStudio.Shell;
using PDDLTools.Windows.PDDLVisualiserWindow;
using PDDLTools.Windows.PlanValidatorWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal sealed class OpenPlanValidatorWindowCommand : BaseCommand<OpenPlanValidatorWindowCommand>
    {
        public override int CommandId { get; } = 269;

        private OpenPlanValidatorWindowCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new OpenPlanValidatorWindowCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            await OpenWindowOfTypeAsync(typeof(PlanValidatorWindow));
        }
    }
}
