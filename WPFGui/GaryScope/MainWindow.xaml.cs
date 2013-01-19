using System;
using System.Windows;

using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;

namespace GaryScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            var ds = new EnumerableDataSource<ScopeSample>(viewModel.Trace1);
            ds.SetXMapping(pi => pi.Time.TimeOfDay.TotalSeconds);
            ds.SetYMapping(pi => pi.Value);
            LineGraph chart = plotter.AddLineGraph(ds, 2.0, String.Format("{0} - {1}", "Scope", "Trace 1"));
        }

    }
}
