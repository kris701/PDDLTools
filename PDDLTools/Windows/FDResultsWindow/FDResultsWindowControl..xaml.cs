using PDDLTools.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDDLTools
{
    public partial class FDResultsWindowControl : UserControl
    {
        public FDResultsWindowControl(FDResults data)
        {
            this.InitializeComponent();

            if (data.WasSolutionFound)
            {
                WasSolutionFoundLabel.Content = "A Solution Was Found!";
                WasSolutionFoundLabel.Foreground = Brushes.DarkGreen;
            }
            else
            {
                WasSolutionFoundLabel.Content = "No Solution Was Found!";
                WasSolutionFoundLabel.Foreground = Brushes.DarkRed;
            }
        }
    }
}