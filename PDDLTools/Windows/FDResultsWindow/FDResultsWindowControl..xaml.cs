using PDDLTools.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDDLTools.Windows.FDResultsWindow
{
    public partial class FDResultsWindowControl : UserControl
    {
        private bool _isLoaded = false;
        public FDResultsWindowControl()
        {
            InitializeComponent();
        }

        public async Task SetupResultDataAsync(FDResults data)
        {
            while(!_isLoaded)
                await Task.Delay(100);

            MainDataGrid.ItemsSource = new List<FDResults>() { data };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }
    }
}