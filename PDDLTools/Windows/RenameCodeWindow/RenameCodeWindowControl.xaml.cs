using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDDLTools.Windows.RenameCodeWindow
{
    /// <summary>
    /// Interaction logic for RenameCodeWindowControl.xaml
    /// </summary>
    public partial class RenameCodeWindowControl : UserControl
    {
        public enum ReplaceTypes { None, Predicate, Parameter, Type };
        private INode _node;
        private ReplaceTypes _replaceType;
        private bool _isLoaded = false;

        public RenameCodeWindowControl()
        {
            InitializeComponent();
        }

        public async Task UpdateReplaceDataAsync(INode node, string text, ReplaceTypes replaceType)
        {
            while (!_isLoaded)
                await Task.Delay(100);
            _node = node;
            ReplaceTextTextbox.Text = text;
            _replaceType = replaceType;
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RaiseScopeLevelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LowerScopeLevelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }
    }
}
